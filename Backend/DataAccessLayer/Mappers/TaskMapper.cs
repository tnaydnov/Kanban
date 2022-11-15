using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.BusinessLayer;
using System.Globalization;

namespace IntroSE.Kanban.Backend.DataAccessLayer.Mappers
{
    public class TaskMapper
    {

        public TaskMapper() { }
        /// <summary>
        /// Loading the tasks and returns a HashSet of the taskDTO's
        /// </summary>
        /// <returns></returns>

        public HashSet<TaskDTO> LoadData()
        {
            string query = $"SELECT * FROM Tasks";
            SQLiteDataReader res = DBConnector.GetInstance().ExecuteQuery(query);
            HashSet<TaskDTO> result = new HashSet<TaskDTO>();
            while (res.Read())
            {
                int boardID = res.GetInt32(res.GetOrdinal("boardID"));
                int columnOrdinal = res.GetInt32(res.GetOrdinal("columnOrdinal"));
                int id = res.GetInt32(res.GetOrdinal("id"));
                string title = res.GetString(res.GetOrdinal("title"));
                string description = res.GetString(res.GetOrdinal("description"));
                DateTime dueDate = Convert.ToDateTime(res.GetString(res.GetOrdinal("dueDate")));
                string assignee = res.GetString(res.GetOrdinal("assignee"));
                TaskDTO taskDTO = new TaskDTO(boardID, columnOrdinal, id, title, description, dueDate);
                taskDTO.AssigneeEmail = assignee;
                Task task = new Task(taskDTO);
                result.Add(taskDTO);
            }
            DBConnector.GetInstance().close();
            return result;
        }
    }
}
