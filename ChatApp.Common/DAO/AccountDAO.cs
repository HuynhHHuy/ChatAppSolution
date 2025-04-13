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

    }
}
