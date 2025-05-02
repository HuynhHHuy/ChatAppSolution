using ChatApp.Client.Hub;
using ChatApp.Client.Services;
using ChatApp.Common.DAO;
using ChatApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ChatApp.Client.Views
{
    public partial class MainForm : Form
    {
        private readonly string email;
        private List<UserDTO> friends;
        private StatusAccountHub _statusHub;

        //Circular avatar
        private void MakePictureBoxCircular(PictureBox pictureBox)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, pictureBox.Width - 1, pictureBox.Height - 1);
            pictureBox.Region = new Region(path);
        }
        public MainForm(string email)
        {
            InitializeComponent();

            this.email = email;
            MakePictureBoxCircular(ptbAvatar);
            _statusHub = new StatusAccountHub();
            ListFriend();
        }

        private void ListFriend()
        {
            pnFriendList.Controls.Clear();
            friends = FriendDAO.Instance.GetFriends(email);
            if (friends.Count == 0)
            {
                Label label = new Label();
                label.Text = "Không có bạn bè nào";
                label.AutoSize = true;
                label.Font = new Font("Segoe UI", 10, FontStyle.Italic);
                label.Location = new Point(10, 10);
                pnFriendList.Controls.Add(label);
            }
            else
            {
                int yOffset = 10;
                foreach (var friend in friends)
                {
                    Panel userPanel = CreateFriendPanel(friend, yOffset);
                    pnFriendList.Controls.Add(userPanel);
                    yOffset += userPanel.Height + 10;
                }
            }
        }

        private void btbSearch_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Label label = new Label();
                label.Text = "User " + i;
                label.AutoSize = true;
                label.Location = new Point(0, i * 20);
                pnUsers.Controls.Add(label);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            pnUsers.Controls.Clear();

            string query = tbSearch.Text.Trim();
            List<UserDTO> userAccounts = AccountDAO.Instance.SearchUsersByEmail(query);

            if (userAccounts.Count == 0)
            {
                Label label = new Label();
                label.Text = "Không tìm thấy người dùng nào";
                label.AutoSize = true;
                label.Font = new Font("Segoe UI", 10, FontStyle.Italic);
                label.Location = new Point(10, 10);
                pnUsers.Controls.Add(label);
            }
            else
            {
                int yOffset = 10;
                foreach (var user in userAccounts)
                {
                    Panel userPanel = CreateUserPanel(user, yOffset);
                    pnUsers.Controls.Add(userPanel);
                    yOffset += userPanel.Height + 10;
                }
            }
        }

        private Panel CreateUserPanel(UserDTO user, int top)
        {
            Panel panel = new Panel();
            panel.Size = new Size(260, 70);
            panel.Location = new Point(10, top);
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            // Avatar
            PictureBox avatar = new PictureBox();
            avatar.Size = new Size(50, 50);
            avatar.Location = new Point(10, 10);
            avatar.SizeMode = PictureBoxSizeMode.Zoom;
            string defaultAvatarUrl = "https://miamistonesource.com/wp-content/uploads/2018/05/no-avatar-25359d55aa3c93ab3466622fd2ce712d1.jpg";

            string avatarUrl = string.IsNullOrEmpty(user.AvatarUrl) ? defaultAvatarUrl : user.AvatarUrl;

            try
            {
                avatar.LoadAsync(avatarUrl);
            }
            catch
            {
                avatar.LoadAsync(defaultAvatarUrl);
            }

            MakePictureBoxCircular(avatar);

            // Tên người dùng
            Label nameLabel = new Label();
            nameLabel.Text = user.FullName;
            nameLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(70, 10);

            // Email
            LinkLabel emailLabel = new LinkLabel();
            emailLabel.Text = user.Email;
            emailLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            emailLabel.AutoSize = true;
            emailLabel.Location = new Point(70, 35);
            emailLabel.LinkColor = Color.Blue;

            panel.Controls.Add(avatar);
            panel.Controls.Add(nameLabel);
            panel.Controls.Add(emailLabel);

            return panel;
        }
        private Panel CreateFriendPanel(UserDTO user, int top)
        {
            Panel panel = new Panel();
            panel.Size = new Size(450, 70);
            panel.Location = new Point(10, top);
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            // Avatar
            PictureBox avatar = new PictureBox();
            avatar.Size = new Size(50, 50);
            avatar.Location = new Point(10, 10);
            avatar.SizeMode = PictureBoxSizeMode.Zoom;
            string defaultAvatarUrl = "https://miamistonesource.com/wp-content/uploads/2018/05/no-avatar-25359d55aa3c93ab3466622fd2ce712d1.jpg";

            string avatarUrl = string.IsNullOrEmpty(user.AvatarUrl) ? defaultAvatarUrl : user.AvatarUrl;

            try
            {
                avatar.LoadAsync(avatarUrl);
            }
            catch
            {
                avatar.LoadAsync(defaultAvatarUrl);
            }

            MakePictureBoxCircular(avatar);

            // Tên người dùng
            Label nameLabel = new Label();
            nameLabel.Text = user.FullName;
            nameLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(70, 10);

            // Email
            LinkLabel emailLabel = new LinkLabel();
            emailLabel.Text = user.Email;
            emailLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            emailLabel.AutoSize = true;
            emailLabel.Location = new Point(70, 35);
            emailLabel.LinkColor = Color.Blue;

            //Status
            Label statusLabel = new Label();
            statusLabel.Name = "StatusLabel";
            statusLabel.Text = user.Status == "online" ? "🟢 Online" : "⚫ Offline";
            statusLabel.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            statusLabel.AutoSize = true;
            statusLabel.ForeColor = user.Status == "online" ? Color.Green : Color.Gray;
            statusLabel.Location = new Point(350, 25);

            panel.Controls.Add(avatar);
            panel.Controls.Add(nameLabel);
            panel.Controls.Add(emailLabel);
            panel.Controls.Add(statusLabel);

            return panel;
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetOfflineStatusInDB();
            if (_statusHub != null)
            {
                await _statusHub.SetOffline(email);
                await _statusHub.DisconnectAsync();
            }
        }

        private void SetOnlineStatusInDB()
        {
            var updateFields = new Dictionary<string, object>
            {
                { "status", "online" }
            };
            var condition = new Dictionary<string, object>
            {
                { "email", email }
            };
            AccountDAO.Instance.UpdateFields(updateFields, condition);
        }
        private void SetOfflineStatusInDB()
        {
            var updateFields = new Dictionary<string, object>
            {
                { "status", "offline" }
            };
            var condition = new Dictionary<string, object>
            {
                { "email", email }
            };
            AccountDAO.Instance.UpdateFields(updateFields, condition);
        }
        private void UpdateFriendStatus(string email, string newStatus)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateFriendStatus(email, newStatus)));
                return;
            }

            foreach (Control control in pnFriendList.Controls)
            {
                if (control is Panel panel)
                {
                    foreach (Control child in panel.Controls)
                    {
                        if (child is LinkLabel emailLabel && emailLabel.Text == email)
                        {
                            Label statusLabel = panel.Controls.OfType<Label>()
                                .FirstOrDefault(l => l.Name == "StatusLabel");

                            if (statusLabel != null)
                            {
                                statusLabel.Text = newStatus == "online" ? "🟢 Online" : "⚫ Offline";

                                statusLabel.ForeColor = newStatus == "online" ? Color.Green : Color.Gray;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await _statusHub.SetOnline(email);
            SetOnlineStatusInDB();

            await _statusHub.ConnectAsync((friendEmail, status) =>
            {
                UpdateFriendStatus(friendEmail, status);
            });
        }

        private async void btnLogout_Click(object sender, EventArgs e)
        {

            SetOfflineStatusInDB();
            if (_statusHub != null)
            {
                await _statusHub.SetOffline(email);
                await _statusHub.DisconnectAsync();
            }
            string str = "";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\Services\auth.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(file))
            {
                writer.Write(str);
            }

            this.Hide();
            using (LoginForm loginForm = new LoginForm())
            {
                loginForm.ShowDialog();
            }
            this.Close();
        }
    }
}
