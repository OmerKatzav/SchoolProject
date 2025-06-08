using Shared;

namespace Client;

public partial class AddMediaForm : Form
{
    private DI.IServiceProvider _serviceProvider;
    private Stream? _thumbnailFile;
    private Stream? _mediaFile;

    public AddMediaForm(DI.IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private void pickThumbnailBtn_Click(object sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = @"Select Thumbnail",
            Multiselect = false
        };
        dialog.ShowDialog(this);
        _thumbnailFile = dialog.OpenFile();
        thumbnailFileLabel.Text = dialog.FileName;
        thumbnailPictureBox.Image = Image.FromStream(_thumbnailFile);
        _thumbnailFile.Position = 0;
    }

    private void pickMediaBtn_Click(object sender, EventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = @"Select Media",
            Multiselect = false
        };
        dialog.ShowDialog(this);
        _mediaFile = dialog.OpenFile();
        mediaFileLabel.Text = dialog.FileName;
    }

    private async void UploadBtn_Click(object sender, EventArgs e)
    {
        try
        {
            if (_thumbnailFile is null || _mediaFile is null)
            {
                MessageBox.Show(@"Please select both thumbnail and media files.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show(@"Please enter a name for the media.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            var mediaFile = new byte[_mediaFile.Length];
            await _mediaFile.ReadExactlyAsync(mediaFile, 0, mediaFile.Length);
            var thumbnailFile = new byte[_thumbnailFile.Length];
            await _thumbnailFile.ReadExactlyAsync(thumbnailFile, 0, thumbnailFile.Length);
            await _serviceProvider.GetService<IMediaService>().InsertMediaAsync(_serviceProvider.GetService<ITokenStorageService>().AuthToken!, nameTextBox.Text, thumbnailFile, mediaFile);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"Failed to upload media: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _thumbnailFile?.Dispose();
            _mediaFile?.Dispose();
            Close();
        }
    }
}