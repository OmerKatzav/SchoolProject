using FFMpegCore;
using FFMpegCore.Pipes;
using NAudio.Wave;
using Shared;

namespace Client;

public partial class MainForm : Form
{
    private readonly DI.IServiceProvider _serviceProvider;
    private WaveOutEvent? _outputDevice;
    private WaveFileReader? _currentReader;
    private int _currentChunkIndex;
    private double? _totalTime;
    private double? _secondsPerChunk;
    private byte[]?[]? _mediaChunks;
    private Guid _mediaId;
    private CancellationTokenSource? _playbackCts;
    private bool _isPaused;

    private double CurrentTime
    {
        get
        {
            return _outputDevice is null ? 0 : _currentChunkIndex * _secondsPerChunk!.Value + _currentReader!.CurrentTime.TotalSeconds;
        }
        set
        {
            var newChunkIndex = (int)(value / (_secondsPerChunk ?? throw new InvalidOperationException("Seconds per chunk is null")));
            if (newChunkIndex < 0 || newChunkIndex >= (_mediaChunks?.Length ?? throw new InvalidOperationException("Media chunks are null")))
            {
                throw new ArgumentOutOfRangeException(nameof(value), @"Current time is out of range");
            }
            _currentChunkIndex = newChunkIndex;
            StartNextChunk().GetAwaiter().GetResult();
            _currentReader!.CurrentTime = TimeSpan.FromSeconds((long)(value - _currentChunkIndex * _secondsPerChunk!.Value));
        }
    }

    public MainForm(DI.IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        try
        {
            await ReloadMedias();
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"An error occurred while loading media: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task ReloadMedias(string searchQuery = "")
    {
        var mediaService = _serviceProvider.GetService<IMediaService>();
        var tokenStorageService = _serviceProvider.GetService<ITokenStorageService>();
        var authToken = tokenStorageService.AuthToken;
        if (authToken is null || !await _serviceProvider.GetService<ILoginService>().ValidateTokenAsync(authToken))
        {
            MessageBox.Show(@"You must be logged in to view media.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        var mediaIds = string.IsNullOrEmpty(searchQuery)
            ? await mediaService.GetMediaIdsAsync(authToken)
            : await mediaService.SearchMediasAsync(authToken, searchQuery);
        var tasks = mediaIds.Select(async id => (id, await mediaService.GetMediaNameAsync(authToken, id), await mediaService.GetMediaThumbnailAsync(authToken, id))).ToArray();
        var medias = await Task.WhenAll(tasks);
        mediaListView.Items.Clear();
        thumbnailImageList.Images.Clear();
        foreach (var (id, name, thumbnailData) in medias)
        {
            using var ms = new MemoryStream(thumbnailData);
            var thumbnailImage = Image.FromStream(ms);
            thumbnailImageList.Images.Add(id.ToString(), thumbnailImage);
            var item = new ListViewItem(name) { Tag = id, ImageKey = id.ToString() };
            mediaListView.Items.Add(item);
        }
    }

    private async void searchBox_TextChanged(object sender, EventArgs e)
    {
        try
        {
            await ReloadMedias(searchBox.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"An error occurred while searching: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void playBtn_Click(object sender, EventArgs e)
    {
        try
        {
            playBtn.Enabled = false;
            if (mediaListView.SelectedItems.Count == 0)
            {
                MessageBox.Show(@"Please select a media to play.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var selectedItem = mediaListView.SelectedItems[0];
            if ((Guid)selectedItem.Tag! == _mediaId) return;
            if (_playbackCts is not null)
            {
                await _playbackCts.CancelAsync();
            }
            _mediaId = (Guid)selectedItem.Tag!;
            _playbackCts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                try
                {
                    _playbackCts.Token.ThrowIfCancellationRequested();
                    var mediaService = _serviceProvider.GetService<IMediaService>();
                    var tokenStorageService = _serviceProvider.GetService<ITokenStorageService>();
                    var authToken = tokenStorageService.AuthToken;
                    if (authToken is null || !await _serviceProvider.GetService<ILoginService>().ValidateTokenAsync(authToken))
                    {
                        throw new InvalidOperationException(@"You must be logged in to play media.");
                    }
                    var chunkMetadata = await mediaService.GetChunkMetadataAsync(authToken, _mediaId);
                    _totalTime = chunkMetadata.Length;
                    _secondsPerChunk = chunkMetadata.SecondsPerChunk;
                    _mediaChunks = new byte[]?[(int)Math.Ceiling(chunkMetadata.Length / chunkMetadata.SecondsPerChunk)];
                    var abrService = _serviceProvider.GetService<IAbrService>();
                    double lastBandwidth = 0;
                    _playbackCts.Token.Register(() => abrService.StopMedia(_mediaId));
                    while (true)
                    {
                        for (var i = 0; i < _currentChunkIndex; i++)
                        {
                            _mediaChunks[i] = null;
                        }
                        var downloadChunkIndex = -1;
                        for (var chunkIndex = _currentChunkIndex + 1; chunkIndex < _mediaChunks.Length; chunkIndex++)
                        {
                            if (_mediaChunks[chunkIndex] is not null) continue;
                            downloadChunkIndex = chunkIndex;
                            break;
                        }
                        if (downloadChunkIndex == -1)
                        {
                            continue;
                        }

                        var chunkBitrateIndex = await abrService.GetNewChunkIdAsync(_mediaId, chunkMetadata, downloadChunkIndex, CurrentTime, downloadChunkIndex * chunkMetadata.SecondsPerChunk - CurrentTime, lastBandwidth);
                        byte[] newChunk;
                        var abrCts = new CancellationTokenSource();
                        try
                        {
                            newChunk = await mediaService.GetChunkAsync(authToken, _mediaId, downloadChunkIndex, chunkBitrateIndex,
                                metadata =>
                                {
                                    lastBandwidth = metadata.ResponseBandwidth ?? throw new InvalidOperationException("Metadata does not contain response bandwidth");
                                    var bytesLeft = metadata.ResponseLength - metadata.ResponseBytesRead ?? throw new InvalidOperationException("Metadata does not contain response length and response bytes read");
                                    if (abrService.ShallAbandon(_mediaId, chunkMetadata, downloadChunkIndex, CurrentTime, downloadChunkIndex * chunkMetadata.SecondsPerChunk - CurrentTime, chunkBitrateIndex, bytesLeft)) abrCts.Cancel();
                                },
                                CancellationTokenSource.CreateLinkedTokenSource(_playbackCts.Token, abrCts.Token).Token);
                        }
                        catch (OperationCanceledException)
                        {
                            await Task.Delay(100, _playbackCts.Token);
                            continue;
                        }

                        using var chunkStream = new MemoryStream(newChunk);
                        using var ffmpegStream = new MemoryStream();

                        await FFMpegArguments
                            .FromPipeInput(new StreamPipeSource(new MemoryStream(newChunk)))
                            .OutputToPipe(new StreamPipeSink(ffmpegStream), o => o.ForceFormat("wav"))
                            .ProcessAsynchronously();

                        ffmpegStream.Seek(0, SeekOrigin.Begin);
                        _mediaChunks[downloadChunkIndex] = ffmpegStream.ToArray();
                    }

                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Invoke(() => MessageBox.Show($@"An error occurred while downloading media: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
                }
            }, _playbackCts.Token);

            _currentChunkIndex = 0;
            await StartNextChunk();

            playbackBar.Enabled = true;
            playbackBar.Value = 0;
            playbackBar.Maximum = (int)Math.Ceiling(_totalTime ?? throw new InvalidOperationException("Total time is null"));

            pauseBtn.Enabled = true;
            _isPaused = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"An error occurred while playing media: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            playBtn.Enabled = true;
        }
    }

    private async Task StartNextChunk()
    {
        _outputDevice?.Dispose();
        _outputDevice = new WaveOutEvent();
        _currentReader?.Dispose();
        while (_mediaChunks![_currentChunkIndex] is null) await Task.Delay(100);
        _currentReader = new WaveFileReader(new MemoryStream(_mediaChunks![_currentChunkIndex]!));
        _outputDevice.PlaybackStopped += async (_, _) =>
        {
            if (_outputDevice!.PlaybackState != PlaybackState.Stopped) return;
            _currentChunkIndex++;
            await StartNextChunk();
        };
        _outputDevice.Init(_currentReader);
        _outputDevice.Play();
    }

    private void pauseBtn_Click(object sender, EventArgs e)
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            pauseBtn.Text = @"Resume";
            _outputDevice?.Pause();
        }
        else
        {
            pauseBtn.Text = @"Pause";
            _outputDevice?.Play();
        }
    }

    private void playbackBar_Scroll(object sender, EventArgs e)
    {
        var wantedTime = playbackBar.Value / (double)playbackBar.Maximum * (_totalTime ?? throw new InvalidOperationException("Total time is null"));
        if (wantedTime < 0 || wantedTime > _totalTime) return;
        CurrentTime = wantedTime;
        if (_isPaused) _outputDevice?.Pause();
    }
}