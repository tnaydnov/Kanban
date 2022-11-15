using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.Mappers
{
    public class BoardMapper
    {
        public BoardMapper() {
        }

        /// <summary>
        /// load all the board related data to the system from the DB
        /// </summary>
        /// <returns>a set of all the boards in the system</returns>
        
        public HashSet<BoardDTO> LoadData()
        {
            SQLiteDataReader res = DBConnector.GetInstance().ExecuteQuery("SELECT * FROM Boards");
            HashSet<BoardDTO> data = new HashSet<BoardDTO>();
            Dictionary<int, HashSet<string>> users = new UsersBoardsDTO().LoadData();
            while (res.Read())
            {
                int id = res.GetInt32(res.GetOrdinal("id"));
                string name = res.GetString(res.GetOrdinal("name"));
                int nextTaskID = res.GetInt32(res.GetOrdinal("nextTaskID"));
                string owner = res.GetString(res.GetOrdinal("owner"));
                if (!users.ContainsKey(id))
                {
                    data.Add(new BoardDTO(id, name, owner, nextTaskID, new HashSet<string>()));
                }
                else
                {
                    data.Add(new BoardDTO(id, name, owner, nextTaskID, users[id]));
                }

            }

            DBConnector.GetInstance().close();
            return data;
        }
    }
}
