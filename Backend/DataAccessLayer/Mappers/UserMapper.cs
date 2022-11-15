using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.Mappers
{

    public class UserMapper
    {


        public UserMapper()
        {

        }

        /// <summary>
        /// load all user related data to the system from the database
        /// </summary>
        /// <returns>set of user DTOs</returns>

        public HashSet<UserDTO> LoadData()
        {
            HashSet<UserDTO> UserData = new HashSet<UserDTO>();
            string query = "SELECT * from Users";
            DBConnector db = DBConnector.GetInstance();
            SQLiteDataReader dbReader = db.ExecuteQuery(query);
            while (dbReader.Read())
            {
                string email = dbReader.GetString(0);
                string password = dbReader.GetString(1);
                UserData.Add(new UserDTO(email, password));
            }
            DBConnector.GetInstance().close();
            return UserData;

        }

        /// <summary>
        /// delete all data from the database
        /// </summary>
        public void DeleteData()
        {
                DBConnector db = DBConnector.GetInstance();
                db.ResetDB();              
        }
    }
}
