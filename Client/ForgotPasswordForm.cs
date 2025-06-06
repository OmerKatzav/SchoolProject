using ProtoBuf;
using Shared;

namespace Client;

public partial class ForgotPasswordForm : Form
{
    private readonly DI.IServiceProvider _serviceProvider;

    public ForgotPasswordForm(DI.IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private async void submitBtn_Click(object sender, EventArgs e)
    {
        try
        {
            submitBtn.Enabled = false;
            var loginService = _serviceProvider.GetService<ILoginService>();
            var captchaService = _serviceProvider.GetService<ICaptchaService>();
            var tokenStorageService = _serviceProvider.GetService<ITokenStorageService>();
            var captchaToken = tokenStorageService.CaptchaToken;
            if (captchaToken == null || !await captchaService.ValidateCaptchaAsync(captchaToken))
            {
                var captchaForm = new CaptchaForm(_serviceProvider);
                captchaForm.ShowDialog(this);
                if (captchaForm.DialogResult != DialogResult.OK)
                {
                    MessageBox.Show(@"Captcha validation failed or cancelled. Please try again.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                captchaToken = tokenStorageService.CaptchaToken!;
            }
            await loginService.ForgotPasswordAsync(emailBox.Text, passwordBox.Text, captchaToken);
            var emailCodeForm = new EmailCodeForm();
            emailCodeForm.ShowDialog(this);
            var emailCode = emailCodeForm.EmailCode;
            if (emailCodeForm.DialogResult != DialogResult.OK || string.IsNullOrEmpty(emailCode))
            {
                MessageBox.Show(@"Email code validation failed or cancelled. Please try again.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var tempToken = Serializer.Deserialize<AuthToken>(new ReadOnlyMemory<byte>(Convert.FromBase64String(emailCode)));
            await loginService.FinishForgotPasswordAsync(tempToken, passwordBox.Text);
            MessageBox.Show(@"Password reset successfully. You can now log in with your new password.", @"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"An error occurred: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            submitBtn.Enabled = true;
        }
    }
}