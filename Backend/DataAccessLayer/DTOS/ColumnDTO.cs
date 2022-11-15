using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class ColumnDTO
    {
        /// <summary>
        /// The name of the column - 'backlog' / 'in progress' / 'done'
        /// </summary>
        private string name;
        public string Name
        {
            get => name;
        }
        /// <summary>
        /// The id of the board the task is in
        /// </summary>
        private int boardID;
        public int BoardID
        {
            get => boardID;
        }
        /// <summary>
        /// The number of the column -
        /// 0 for 'backlog', 1 for 'in progress', 2 for 'done'
        /// </summary>
        private int ordinal;
        public int Ordinal
        {
            get => ordinal;
        }
        /// <summary>
        /// The limit on the maximum tasks available in this column
        /// </summary>
        private int maxTasks;
        public int MaxTasks
        {
            get => maxTasks;
            set => maxTasks = value;
        }
        /// <summary>
        /// The tasks in this column
        /// </summary>
        private HashSet<TaskDTO> tasks;
        public HashSet<TaskDTO> Tasks
        {
            get => tasks;
            set => tasks = value;
        }

        public ColumnDTO(int boardID, string name, HashSet<TaskDTO> tasks)
        {
            this.boardID = boardID;
            this.name = name;
            this.maxTasks = int.MaxValue;
            if (name == "backlog")
            {
                ordinal = 0;
            }
            else if (name == "in progress")
            {
                ordinal = 1;
            }
            else
            {
                ordinal = 2;
            }
            this.tasks = tasks;
            tasks = new HashSet<TaskDTO>();
        }

        public ColumnDTO(Column column)
        {
            boardID = column.BoardID;
            name = column.Name;
            maxTasks = column.MaxTasks;
            if (column.Name == "backlog")
            {
                ordinal = 0;
            }
            else if (column.Name == "in progress")
            {
                ordinal = 1;
            }
            else
            {
                ordinal = 2;
            }
            tasks = new HashSet<TaskDTO>();
            for (int i = 0; i < column.Tasks.Count; i++)
            {
                TaskDTO t = new TaskDTO(column.Tasks[i]);
                tasks.Add(t);
            }
        }
        private void GeneralNonQuery(string query, string badMsg)
        {
            if (!DBConnector.GetInstance().ExecuteNonQuery(query))
            {
                throw new Exception(badMsg);
            }
        }
        /// <summary>
        /// Remove a task
        /// </summary>
        /// <param name="id">The task id</param>
        public void RemoveTask(int id)
        {
            string query = $"DELETE FROM Tasks WHERE id = {id} AND boardId = {boardID}";
            GeneralNonQuery(query, "Something went wrong");
            foreach(TaskDTO task in tasks)
            {
                if(task.Id == id)
                {
                    tasks.Remove(task);
                    break;
                }
            }
        }
        /// <summary>
        /// Add a task
        /// </summary>
        /// <param name="taskDTO">The task</param>
        public void AddTask(TaskDTO taskDTO)
        {
            string query = $"INSERT INTO Tasks(boardID, columnOrdinal,id, title, description, dueDate, assignee) VALUES({taskDTO.BoardID},{taskDTO.ColumnOrdinal},{taskDTO.Id},'{taskDTO.Title}','{taskDTO.Description}','{taskDTO.DueDate}', 'null')";
            GeneralNonQuery(query, "A task with this id already exists");
        }
        /// <summary>
        /// Set a limitation on the tasks number in this column
        /// </summary>
        /// <param name="newLimit"></param>
        public void SetMax(int newLimit)
        {
            string query = $"UPDATE Columns SET maxTasks = '{newLimit}' WHERE columnOrdinal = {ordinal} AND boardID = {boardID}";
            maxTasks = newLimit;
            GeneralNonQuery(query, "Something went wrong");
        }
        /// <summary>
        /// Adds the column to the db
        /// </summary>
        public void AddColumnToDB()
        {
            string query = $"INSERT INTO Columns(boardID,columnOrdinal,maxTasks) VALUES({boardID},{ordinal},{maxTasks})";
            GeneralNonQuery(query, "Something went wrong");
        }
    }
}
