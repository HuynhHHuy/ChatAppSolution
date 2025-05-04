using ChatApp.Client.Services;
using ChatApp.Common.DAO;
using ChatApp.Common.DTOs;
using ChatApp.Client.Hub;
using Microsoft.VisualBasic.ApplicationServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;


namespace ChatApp.Client.Views
{
    public partial class ChatRoom : Form
    {
        private readonly string _fromEmail;
        private readonly string _toEmail;
        private UserDTO _user;
        private StatusAccountHub _statusHub;
        private ChatOneOnOneHub _chatOneOnOneHub;
        private UserService _userService;
        private CircularPictureBoxService _circularPictureBoxService;
        private List<MessageDTO> _messages;
        public ChatRoom(string fromEmail, string toEmail)
        {
            InitializeComponent();

            _fromEmail = fromEmail;
            _toEmail = toEmail;

            // Initialize services
            _userService = new UserService();
            _circularPictureBoxService = new CircularPictureBoxService();

            // Start the socket hub
            _statusHub = new StatusAccountHub();
            _chatOneOnOneHub = new ChatOneOnOneHub(_fromEmail);

            // Load the toUser data
            _user = AccountDAO.Instance.GetUserInfoByEmail(_toEmail);

            // UI initialization
            lbName.Text = _user.FullName;
            lbStatus.Text = _user.Status;
            lbStatus.ForeColor = _user.Status == "online" ? Color.Green : Color.Red;
            lbStatus.Text = _user.Status == "online" ? "Online" : "Offline";
            string defaultAvatarUrl = "https://miamistonesource.com/wp-content/uploads/2018/05/no-avatar-25359d55aa3c93ab3466622fd2ce712d1.jpg";
            string avatarUrl = string.IsNullOrEmpty(_user.AvatarUrl) ? defaultAvatarUrl : _user.AvatarUrl;
            ptbAvatar.LoadAsync(avatarUrl);

            ptbAvatar.LoadCompleted += (s, e) =>
            {
                _circularPictureBoxService.MakePictureBoxCircular(ptbAvatar);
            };
            LoadMessagesToUI();
        }

        // Handling update status for to user
        private void UpdateToUserStatus(string email, string status)
        {
            if (email == _toEmail)
            {
                lbStatus.Text = status;
                lbStatus.ForeColor = status == "online" ? Color.Green : Color.Red;
                lbStatus.Text = status == "online" ? "Online" : "Offline";
            }
        }

        // Handling recived message from to user
        private void UpdateToUserMessage(string senderId, string message, string type)
        {
            Invoke((MethodInvoker)delegate
            {
                var msg = new MessageDTO
                {
                    SenderEmail = senderId,
                    ReceiverEmail = _fromEmail,
                    Message = message,
                    MessageType = type,
                    SentAt = DateTime.Now
                };
                AddMessageToUI(msg);
            });
        }


        // Loading messages to UI
        private async void LoadMessagesToUI()
        {
            var messages = MessageDAO.Instance.GetMessagesOneOnOne(_fromEmail, _toEmail);

            pnMess.SuspendLayout();
            pnMess.Controls.Clear();
            pnMess.AutoScroll = true;
            pnMess.HorizontalScroll.Maximum = 0;
            pnMess.HorizontalScroll.Visible = false;
            pnMess.AutoScrollMinSize = new Size(0, 0);

            int y = 10;

            foreach (var msg in messages)
            {
                Control messageControl;

                if (msg.MessageType == "image")
                {
                    PictureBox pb = new PictureBox();
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
                    pb.Size = new Size(200, 200);

                    try
                    {
                        if (Uri.IsWellFormedUriString(msg.Message, UriKind.Absolute))
                        {
                            using (var client = new HttpClient())
                            {
                                var imageBytes = await client.GetByteArrayAsync(msg.Message);
                                using (var ms = new MemoryStream(imageBytes))
                                {
                                    pb.Image = Image.FromStream(ms);
                                }
                            }
                        }
                        else if (File.Exists(msg.Message))
                        {
                            pb.Image = Image.FromFile(msg.Message);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy ảnh: " + msg.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message);
                    }
                    messageControl = pb;
                }

                else // text
                {
                    Label lblMsg = new Label();
                    lblMsg.AutoSize = true;
                    lblMsg.MaximumSize = new Size(pnMess.Width - 60, 0);
                    lblMsg.Text = msg.Message;
                    lblMsg.Font = new Font("Segoe UI", 10);
                    lblMsg.BackColor = msg.SenderEmail == _fromEmail ? Color.LightBlue : Color.LightGray;
                    lblMsg.Padding = new Padding(8);
                    lblMsg.Margin = new Padding(0);
                    messageControl = lblMsg;
                }

                messageControl.Left = msg.SenderEmail == _fromEmail
                    ? pnMess.Width - messageControl.Width - 30
                    : 10;
                messageControl.Top = y;

                pnMess.Controls.Add(messageControl);
                y += messageControl.Height + 10;
            }

            pnMess.ResumeLayout();
            pnMess.VerticalScroll.Value = pnMess.VerticalScroll.Maximum;
            pnMess.PerformLayout();
        }

        private async void AddMessageToUI(MessageDTO msg)
        {
            pnMess.SuspendLayout();

            Control messageControl;

            if (msg.MessageType == "image")
            {
                PictureBox pb = new PictureBox();
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.Size = new Size(200, 200);

                try
                {
                    if (Uri.IsWellFormedUriString(msg.Message, UriKind.Absolute))
                    {
                        using (var client = new HttpClient())
                        {
                            var imageBytes = await client.GetByteArrayAsync(msg.Message);
                            using (var ms = new MemoryStream(imageBytes))
                            {
                                pb.Image = Image.FromStream(ms);
                            }
                        }
                    }
                    else if (File.Exists(msg.Message))
                    {
                        pb.Image = Image.FromFile(msg.Message);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy ảnh: " + msg.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message);
                }
                messageControl = pb;
            }

            else
            {
                Label lblMsg = new Label();
                lblMsg.AutoSize = true;
                lblMsg.MaximumSize = new Size(pnMess.Width - 60, 0);
                lblMsg.Text = msg.Message;
                lblMsg.Font = new Font("Segoe UI", 10);
                lblMsg.BackColor = msg.SenderEmail == _fromEmail ? Color.LightBlue : Color.LightGray;
                lblMsg.Padding = new Padding(8);
                lblMsg.Margin = new Padding(0);
                messageControl = lblMsg;
            }

            int y = pnMess.Controls.Count > 0
                ? pnMess.Controls[pnMess.Controls.Count - 1].Bottom + 10
                : 10;

            messageControl.Left = msg.SenderEmail == _fromEmail
                ? pnMess.Width - messageControl.Width - 30
                : 10;
            messageControl.Top = y;

            pnMess.Controls.Add(messageControl);
            pnMess.ResumeLayout();

            pnMess.VerticalScroll.Value = pnMess.VerticalScroll.Maximum;
            pnMess.PerformLayout();
        }


        // Closing the chat room
        private async void ChatRoom_FormClosing(object sender, FormClosingEventArgs e)
        {
            _userService.SetOfflineStatusInDB(_fromEmail);
            if (_statusHub != null)
            {
                await _statusHub.SetOffline(_fromEmail);
                await _statusHub.DisconnectAsync();
            }
        }

        // Loading the chat room
        private async void ChatRoom_Load(object sender, EventArgs e)
        {
            await _statusHub.SetOnline(_fromEmail);
            _userService.SetOnlineStatusInDB(_fromEmail);

            await _statusHub.ConnectAsync((friendEmail, status) =>
            {
                UpdateToUserStatus(friendEmail, status);
            });

            await _chatOneOnOneHub.ConnectAsync((senderId, data, messageType) =>
            {
                if (messageType == "text")
                {
                    string message = System.Text.Encoding.UTF8.GetString(data);
                    UpdateToUserMessage(senderId, message, "text");
                }
                else if (messageType == "image")
                {
                    string imageUrl = System.Text.Encoding.UTF8.GetString(data);
                    UpdateToUserMessage(senderId, imageUrl, "image");
                }
            });

        }

        // Sending message to the to user
        private async void btnSendMess_Click(object sender, EventArgs e)
        {
            string message = tbMess.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                await _chatOneOnOneHub.SendMessageAsync(_toEmail, data, "text");

                MessageDAO.Instance.InsertMessage(_fromEmail, _toEmail, message, "text", DateTime.Now);

                /*var tempMsg = new MessageDTO
                {
                    SenderEmail = _fromEmail,
                    ReceiverEmail = _toEmail,
                    Message = message,
                    MessageType = "text",
                    SentAt = DateTime.Now
                };
                AddMessageToUI(tempMsg);*/
                tbMess.Clear();
            }
        }

        private async void btnUploadImg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    byte[] imageBytes = File.ReadAllBytes(filePath);

                    await _chatOneOnOneHub.SendMessageAsync(_toEmail, imageBytes, "image");

                    var tempMsg = new MessageDTO
                    {
                        SenderEmail = _fromEmail,
                        ReceiverEmail = _toEmail,
                        Message = filePath,
                        MessageType = "image",
                        SentAt = DateTime.Now
                    };

                    AddMessageToUI(tempMsg);
                }
            }
        }
    }
}
