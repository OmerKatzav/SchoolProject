using Shared;

namespace Client;

public partial class CaptchaForm : Form
{
    private readonly DI.IServiceProvider _serviceProvider;
    private byte[]? _captchaSignature;
    private CaptchaData? _captchaData;
    private bool _isRefreshing;

    public CaptchaForm(DI.IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private async void CaptchaForm_Load(object sender, EventArgs e)
    {
        try
        {
            await RefreshCaptcha();
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"Failed to load captcha: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }
    }

    private async Task RefreshCaptcha()
    {
        submitBtn.Enabled = false;
        refreshBtn.Enabled = false;
        var captchaService = _serviceProvider.GetService<ICaptchaService>();
        var (captchaData, signature) = await captchaService.GetCaptchaAsync();
        captchaPictureBox.Image = Image.FromStream(new MemoryStream(captchaData.CaptchaImage));
        _captchaData = captchaData;
        _captchaSignature = signature;
        var captchaTimer = new System.Windows.Forms.Timer
        {
            Interval = 1000
        };
        captchaTimer.Tick += CaptchaTimer_Tick;
        captchaTimer.Start();
        submitBtn.Enabled = true;
        refreshBtn.Enabled = true;
    }

    private async void CaptchaTimer_Tick(object? sender, EventArgs e)
    {
        try
        {
            captchaExpirationLabel.Text = $@"Captcha expires in {_captchaData!.Expiration - DateTime.UtcNow:\mm:\ss}";
            if (DateTime.UtcNow < _captchaData.Expiration || _isRefreshing) return;
            _isRefreshing = true;
            try
            {
                await RefreshCaptcha();
            }
            finally
            {
                _isRefreshing = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"Failed to refresh captcha: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void refreshBtn_Click(object sender, EventArgs e)
    {
        try
        {
            refreshBtn.Enabled = false;
            submitBtn.Enabled = false;
            await RefreshCaptcha();
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"Failed to refresh captcha: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            refreshBtn.Enabled = true;
            submitBtn.Enabled = true;
        }
    }

    private async void submitBtn_Click(object sender, EventArgs e)
    {
        try
        {
            submitBtn.Enabled = false;
            refreshBtn.Enabled = false;
            if (string.IsNullOrWhiteSpace(captchaTextBox.Text))
            {
                MessageBox.Show(@"Please enter the captcha solution.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var captchaToken = new CaptchaToken(new FullCaptchaData(_captchaData!, captchaTextBox.Text), _captchaSignature!);
            if (!await _serviceProvider.GetService<ICaptchaService>().ValidateCaptchaAsync(captchaToken))
            {
                MessageBox.Show(@"Captcha validation failed. Please try again.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var tokenStorageService = _serviceProvider.GetService<ITokenStorageService>();
            tokenStorageService.CaptchaToken = captchaToken;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"An error occurred while submitting the captcha: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            submitBtn.Enabled = true;
            refreshBtn.Enabled = true;
        }
    }
}