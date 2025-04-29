using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Common.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;
        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return instance; }
            private set { instance = value; }
        }
        private AccountDAO() { }

        public bool CheckAccount(string email, string password)
        {
            string query = "SELECT password FROM Users WHERE email = @param0";
            var parameters = new object[] { email };

            var dataTable = DataProvider.Instance.ExcuteQuery(query, parameters);

            if (dataTable.Rows.Count > 0)
            {
                string hashedPassword = dataTable.Rows[0]["password"].ToString();

                return HandlePassword.VerifyPassword(password, hashedPassword);
            }

            return false;
        }

        public bool CheckExistEmail(string email)
        {
            string query = "SELECT email FROM Users WHERE email = @param0";
            var parameters = new object[] { email };
            var dataTable = DataProvider.Instance.ExcuteQuery(query, parameters);
            return dataTable.Rows.Count > 0;
        }

        public int InsertAccountNotVerify(string email, string password, string randomCode, string name)
        {
            string query = "INSERT INTO Users (email, password, verify_code, full_name) VALUES (@param0, @param1, @param2, @param3)";
            var parameters = new object[] { email, HandlePassword.HashPassword(password), randomCode, name };
            return DataProvider.Instance.ExcuteNonQuery(query, parameters);
        }

        public int DeleteAccountByEmail(string email)
        {
            string query = "DELETE FROM Users WHERE email = @param0";
            var parameters = new object[] { email };
            return DataProvider.Instance.ExcuteNonQuery(query, parameters);
        }
    }
}
