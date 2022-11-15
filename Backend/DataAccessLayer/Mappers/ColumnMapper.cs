using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.DataAccessLayer.Mappers
{
    public class ColumnMapper
    {
        public TaskMapper taskMapper;
        public TaskMapper TaskMapper
        {
            get => taskMapper;
            set => taskMapper = value;
        }
        public ColumnMapper() {
            taskMapper = new TaskMapper();
        }
        /// <summary>
        /// Loading the specified column and the relevant tasks
        /// </summary>
        /// <returns></returns>
        
        public Dictionary<int, HashSet<Column>> LoadData()
        {
            HashSet<TaskDTO> tasks = taskMapper.LoadData();
            string query = $"SELECT * FROM Columns";
            SQLiteDataReader res = DBConnector.GetInstance().ExecuteQuery(query);
            HashSet<TaskDTO> result = new HashSet<TaskDTO>();
            Dictionary<int, HashSet<Column>> output = new Dictionary<int, HashSet<Column>>();
            while (res.Read())
            {
                int boardID = res.GetInt32(res.GetOrdinal("boardID"));
                int columnOrdinal = res.GetInt32(res.GetOrdinal("columnOrdinal"));
                int maxTasks = res.GetInt32(res.GetOrdinal("maxTasks"));
                string columnName;
                if (columnOrdinal == 0)
                {
                    columnName = "backlog";
                }
                else if (columnOrdinal == 1)
                {
                    columnName = "inProgress";
                }
                else
                {
                    columnName = "done";
                }
                HashSet<TaskDTO> filteredTasks = new HashSet<TaskDTO>();
                foreach (TaskDTO task in tasks)
                {
                    if (task.BoardID == boardID && task.ColumnOrdinal == columnOrdinal)
                    {
                        filteredTasks.Add(task);
                    }
                }
                ColumnDTO columnDTO = new ColumnDTO(boardID, columnName, filteredTasks);
                columnDTO.SetMax(maxTasks);
                Column column = new Column(columnDTO);
                if (!output.ContainsKey(boardID))
                {
                    output.Add(boardID, new HashSet<Column>());
                }
                output[boardID].Add(column);
            }
            DBConnector.GetInstance().close();
            return output;
        }
    }
}
