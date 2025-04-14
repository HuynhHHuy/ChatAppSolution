using ChatApp.Common.DAO;

namespace ChatApp.Client;

public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();
    }

    private void LoginForm_Load(object sender, EventArgs e)
    {

    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void label3_Click(object sender, EventArgs e)
    {

    }

    private void label4_Click(object sender, EventArgs e)
    {

    }

    private void btnVisiblePassword_Click(object sender, EventArgs e)
    {
        if (tbPassword.UseSystemPasswordChar)
        {
            tbPassword.UseSystemPasswordChar = false;
            btnVisiblePassword.Text = "ÂÒn";
        }
        else
        {
            tbPassword.UseSystemPasswordChar = true;
            btnVisiblePassword.Text = "Hiêòn";
        }
    }

    private void btnLogIn_EnabledChanged(object sender, EventArgs e)
    {
        //if (string.IsNullOrWhiteSpace(tbPassword.Text) || string.IsNullOrWhiteSpace(tbEmail.Text))
        //{
        //    btnLogIn.Enabled = false;
        //}
        //else
        //{
        //    btnLogIn.Enabled = true;
        //}
    }

    private void btnLogIn_Click(object sender, EventArgs e)
    {
        string email = tbEmail.Text;
        string password = tbPassword.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            lbHelpText.Text = "Vui loÌng nhâòp email vaÌ mâòt khâÒu";
            lbHelpText.ForeColor = Color.Red;
            lbHelpText.Visible = true;
            return;
        }
        else if (!isValidEmail(email))
        {
            lbHelpText.Text = "Email không hõòp lêò";
            lbHelpText.ForeColor = Color.Red;
            lbHelpText.Visible = true;
            return;
        }
        else if (password.Length < 6)
        {
            lbHelpText.Text = "Mâòt khâÒu phaÒi coì iìt nhâìt 6 kyì týò";
            lbHelpText.ForeColor = Color.Red;
            lbHelpText.Visible = true;
            return;
        }

        if (CheckAccount(email, password))
        {
            MessageBox.Show("Ðãng nhâòp thaÌnh công", "Thông baìo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            lbHelpText.Text = "Email hoãòc mâòt khâÒu không ðuìng";
            lbHelpText.ForeColor = Color.Red;
            lbHelpText.Visible = true;
            return;
        }
    }

    private bool CheckAccount(string email, string password)
    {
        return AccountDAO.Instance.CheckAccount(email, password);
    }

    private bool isValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private void tbEmail_TextChanged(object sender, EventArgs e)
    {
        lbHelpText.Visible = false;
    }

    private void tbPassword_TextChanged(object sender, EventArgs e)
    {
        lbHelpText.Visible = false;
    }

}
