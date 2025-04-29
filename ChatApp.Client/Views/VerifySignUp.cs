using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;
using ChatApp.Common.DAO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatApp.Client.Views
{
    public partial class VerifySignUp : Form
    {
        private string correctCode;
        private string inputCode = "";
        private string emailAddress;
        private int countdownSeconds = 300;
        private System.Windows.Forms.Timer countdownTimer;
        private RegisterForm registerForm;
        private List<TextBox> codeBoxes;
        public VerifySignUp(string verificationCode, string email, RegisterForm parentForm)
        {
            InitializeComponent();
            correctCode = verificationCode;
            emailAddress = email;
            registerForm = parentForm;

            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();

            UpdateCountdownLabel();
        }

        // Call api /email/send/verifycode
        private async Task SendVerifyCodeEmailAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5000/");
                    var request = new
                    {
                        Email = emailAddress,
                        VerifyCode = correctCode
                    };
                    var json = System.Text.Json.JsonSerializer.Serialize(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("email/send/verifycode", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Verification email sent successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send email. StatusCode: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending email: {ex.Message}");
            }
        }

        private bool DeleteAccountFromDB()
        {
            int result = AccountDAO.Instance.DeleteAccountByEmail(emailAddress);
            return result == 1;
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownSeconds--;
            UpdateCountdownLabel();

            if (countdownSeconds <= 0)
            {
                DeleteAccountFromDB();
                countdownTimer.Stop();
                this.Hide();
                this.Close();
                registerForm.Show();
            }
        }

        private void UpdateCountdownLabel()
        {
            int minutes = countdownSeconds / 60;
            int seconds = countdownSeconds % 60;
            lbCountDown.Text = $"{minutes:D2}:{seconds:D2}";
        }

        //Send verify code for the first time loading
        private async void VerifySignUp_Load(object sender, EventArgs e)
        {
            await SendVerifyCodeEmailAsync();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void VerifySignUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            DeleteAccountFromDB();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            DeleteAccountFromDB();
            RegisterForm registerForm = new RegisterForm();
            this.Hide();
            registerForm.ShowDialog();
            this.Close();
        }

        private void tbVerifyCode_TextChanged(object sender, EventArgs e)
        {
            if (tbVerifyCode.Text.Length >= 6)
            {
                if (tbVerifyCode.Text == correctCode) 
                { 
                    lbHelpText.Text = "Xác thực thành công!";
                }
                else
                {
                    lbHelpText.Text = "Mã xác thực không chính xác!";
                }
            }
            else
            {
                lbHelpText.Text = "";
            }
        }
    }
}
