using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class UserDTO
    {
        private string email;
        public string Email
        {
            get => email;
            set => email = value;
        }
        private string password;
        public string Password
        {
            get => password;
            set => password = value;
        }

        public UserDTO(string email, string password)
        {
            this.email = email;
            this.password = password;
        }

        private void GeneralNonQuery(string query, string badMsg)
        {
            if (!DBConnector.GetInstance().ExecuteNonQuery(query))
            {
                throw new Exception(badMsg);
            }
        }


        /// <summary>
        /// Register user in the database
        /// </summary>
        /// <param name="email"> Email of the user </param>
        /// <param name="password"> Password of the user </param>
        /// <returns></returns>
        
        public void RegisterUser(string email, string password)
        {
            string query = $"INSERT INTO Users(email, password) VALUES('{email}', '{password}')";
            GeneralNonQuery(query, "A user with this email already exists");
        }
    }
}