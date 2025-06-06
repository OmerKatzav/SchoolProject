namespace Client;

public partial class EmailCodeForm : Form
{
    public string? EmailCode;
    public EmailCodeForm()
    {
        InitializeComponent();
    }

    private void confirmBtn_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        EmailCode = codeBox.Text;
        codeBox.Clear();
        Close();
    }
}