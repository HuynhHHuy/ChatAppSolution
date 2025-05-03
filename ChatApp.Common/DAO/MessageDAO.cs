using ChatApp.Common.DTOs;
using System.Data;

namespace ChatApp.Common.DAO
{
    public class MessageDAO
    {
        private static MessageDAO instance;
        public static MessageDAO Instance
        {
            get { if (instance == null) instance = new MessageDAO(); return instance; }
            private set { instance = value; }
        }
        private MessageDAO() { }

        public List<MessageDTO> GetMessagesOneOnOne(string senderEmail, string receiverEmail)
        {
            string query = @"
            SELECT 
                m.id,
                sender.email AS SenderEmail,
                receiver.email AS ReceiverEmail,
                m.group_id AS GroupId,
                m.message AS Message,
                m.message_type AS MessageType,
                m.sent_at AS SentAt
            FROM Messages m
            JOIN Users sender ON m.sender_id = sender.id
            JOIN Users receiver ON m.receiver_id = receiver.id
            WHERE 
                (sender.email = @param0 AND receiver.email = @param1)
                OR
                (sender.email = @param1 AND receiver.email = @param0)
            ORDER BY m.sent_at ASC;
            ";

            var parameters = new object[] { senderEmail, receiverEmail };
            var dataTable = DataProvider.Instance.ExcuteQuery(query, parameters);

            List<MessageDTO> messages = new List<MessageDTO>();

            foreach (DataRow row in dataTable.Rows)
            {
                MessageDTO message = new MessageDTO
                {
                    Id = Convert.ToInt32(row["Id"]),
                    SenderEmail = row["SenderEmail"].ToString(),
                    ReceiverEmail = row["ReceiverEmail"].ToString(),
                    GroupId = row["GroupId"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["GroupId"]),
                    Message = row["Message"].ToString(),
                    MessageType = row["MessageType"].ToString(),
                    SentAt = Convert.ToDateTime(row["SentAt"])
                };
                messages.Add(message);
            }

            return messages;
        }

        public bool InsertMessage(string senderEmail, string receiverEmail, string message, string messageType, DateTime sentAt, int? groupId = null)
        {
            string query = @"
            INSERT INTO Messages (sender_id, receiver_id, group_id, message, message_type, sent_at)
            VALUES (
                (SELECT id FROM Users WHERE email = @param0),
                (SELECT id FROM Users WHERE email = @param1),
                @param2,
                @param3,
                @param4,
                @param5
            );
            ";

            object[] parameters = new object[]
            {
                senderEmail,
                receiverEmail,
                (object?)groupId ?? DBNull.Value,
                message,
                messageType,
                sentAt
            };

            int result = DataProvider.Instance.ExcuteNonQuery(query, parameters);
            return result > 0;
        }

    }
}
