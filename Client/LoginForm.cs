using ProtoBuf;
using Shared;

namespace Client;

public partial class LoginForm : Form
{
    private readonly DI.IServiceProvider _serviceProvider;

    public LoginForm(DI.IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private async void loginBtn_Click(object sender, EventArgs e)
    {
        loginBtn.Enabled = false;
        try
        {
            var loginService = _serviceProvider.GetService<ILoginService>();
            var captchaService = _serviceProvider.GetService<ICaptchaService>();
            var tokenStorageService = _serviceProvider.GetService<ITokenStorageService>();
            var captchaToken = tokenStorageService.CaptchaToken;
            if (captchaToken == null || !await captchaService.ValidateCaptchaAsync(captchaToken))
            {
                var captchaForm = new CaptchaForm(_serviceProvider);
                try
                {
                    captchaForm.ShowDialog(this);
                    if (captchaForm.DialogResult != DialogResult.OK)
                    {
                        MessageBox.Show(@"Captcha validation failed or cancelled. Please try again.", @"Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    captchaToken = tokenStorageService.CaptchaToken!;
                }
                finally
                {
                    captchaForm.Dispose();
                }
            }
            await loginService.LoginAsync(usernameBox.Text, passwordBox.Text, captchaToken);
            var emailCodeForm = new EmailCodeForm();
            emailCodeForm.ShowDialog(this);
            var tempTokenCode = emailCodeForm.EmailCode;
            if (emailCodeForm.DialogResult != DialogResult.OK || string.IsNullOrEmpty(tempTokenCode))
            {
                MessageBox.Show(@"Email code validation failed or cancelled. Please try again.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var tempToken = Serializer.Deserialize<AuthToken>(new ReadOnlyMemory<byte>(Convert.FromBase64String(tempTokenCode)));
            var token = await loginService.RefreshTokenAsync(tempToken);
            tokenStorageService.AuthToken = token;
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"An error occurred: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            loginBtn.Enabled = true;
        }
    }

    private void signUpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        var signUpForm = new SignUpForm(_serviceProvider);
        signUpForm.ShowDialog(this);
        if (signUpForm.DialogResult == DialogResult.OK)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show(@"Sign up cancelled or failed. Please try again.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void forgotPasswordLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        var forgotPasswordForm = new ForgotPasswordForm(_serviceProvider);
        forgotPasswordForm.ShowDialog(this);
        if (forgotPasswordForm.DialogResult != DialogResult.OK)
        {
            MessageBox.Show(@"Forgot password process cancelled or failed. Please try again.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}