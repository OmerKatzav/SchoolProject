using ProtoBuf;
using Shared;

namespace Client;

public partial class SignUpForm : Form
{
    private readonly DI.IServiceProvider _serviceProvider;

    public SignUpForm(DI.IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private async void signUpBtn_Click(object sender, EventArgs e)
    {
        try
        {
            signUpBtn.Enabled = false;
            var loginService = _serviceProvider.GetService<ILoginService>();
            var captchaService = _serviceProvider.GetService<ICaptchaService>();
            var tokenStorageService = _serviceProvider.GetService<ITokenStorageService>();
            var captchaToken = tokenStorageService.CaptchaToken;
            if (captchaToken is null || !await captchaService.ValidateCaptchaAsync(captchaToken))
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
            await loginService.RegisterAsync(usernameBox.Text, passwordBox.Text, emailBox.Text, captchaToken);
            var emailCodeForm = new EmailCodeForm();
            emailCodeForm.ShowDialog(this);
            if (emailCodeForm.DialogResult != DialogResult.OK || string.IsNullOrEmpty(emailCodeForm.EmailCode))
            {
                MessageBox.Show(@"Email code validation failed or cancelled. Please try again.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var emailCode = emailCodeForm.EmailCode;
            var registerToken = Serializer.Deserialize<RegisterToken>(new ReadOnlyMemory<byte>(Convert.FromBase64String(emailCode)));
            tokenStorageService.AuthToken = await loginService.FinishRegisterAsync(registerToken, passwordBox.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"An error occurred: {ex.Message}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            signUpBtn.Enabled = true;
        }
    }
}