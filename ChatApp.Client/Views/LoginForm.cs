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
            btnVisiblePassword.Text = "��n";
        }
        else
        {
            tbPassword.UseSystemPasswordChar = true;
            btnVisiblePassword.Text = "Hi��n";
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
            lbHelpText.Text = "Vui lo�ng nh��p email va� m��t kh��u";
            lbHelpText.ForeColor = Color.Red;
            lbHelpText.Visible = true;
            return;
        }
        else if (!isValidEmail(email))
        {
            lbHelpText.Text = "Email kh�ng h��p l��";
            lbHelpText.ForeColor = Color.Red;
            lbHelpText.Visible = true;
            return;
        }
        else if (password.Length < 6)
        {
            lbHelpText.Text = "M��t kh��u pha�i co� i�t nh��t 6 ky� t��";
            lbHelpText.ForeColor = Color.Red;
            lbHelpText.Visible = true;
            return;
        }

        if (CheckAccount(email, password))
        {
            MessageBox.Show("��ng nh��p tha�nh c�ng", "Th�ng ba�o", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            lbHelpText.Text = "Email ho��c m��t kh��u kh�ng �u�ng";
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
