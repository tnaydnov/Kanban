using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class TaskDTO
    {
        /// <summary>
        /// Id of the board the task is in
        /// </summary>
        private int boardID;
        public int BoardID
        {
            get => boardID;
        }
        /// <summary>
        /// The number of the column the task is in.
        /// 0 for 'backlog', 1 for 'in progress', 2 for 'done'
        /// </summary>
        private int columnOrdinal;
        public int ColumnOrdinal
        {
            get => columnOrdinal;
            set => columnOrdinal = value;
        }
        /// <summary>
        /// The id of the task
        /// </summary>
        private int id;
        public int Id
        {
            get => id;
        }
        /// <summary>
        /// The creation time of the task
        /// </summary>
        private DateTime creationTime;
        public DateTime CreationTime
        {
            get => creationTime;
        }
        /// <summary>
        /// The name of the task
        /// </summary>
        private string title;
        public string Title
        {
            get => title;
        }
        /// <summary>
        /// The description of the task
        /// </summary>
        private string description;
        public string Description
        {
            get => description;
            set => description = value;
        }
        /// <summary>
        /// The due date of the task
        /// </summary>
        private DateTime dueDate;
        public DateTime DueDate
        {
            get => dueDate;
            set => dueDate = value;
        }
        /// <summary>
        /// The task's assigned email - the owner of the task
        /// </summary>
        private string assigneeEmail;
        public string AssigneeEmail
        {
            get => assigneeEmail;
            set => assigneeEmail = value;
        }

        public TaskDTO(int boardID, int columnOrdinal, int ID, string name, string description, DateTime dueDate)
        {
            this.boardID = boardID;
            this.columnOrdinal = columnOrdinal;
            this.id = ID;
            this.title = name;
            this.description = description;
            this.creationTime = DateTime.Now;
            this.dueDate = dueDate;
            this.assigneeEmail = "";
        }

        public TaskDTO(BusinessLayer.Task task)
        {
            this.boardID = task.BoardID;
            this.columnOrdinal = task.ColumnOrdinal;
            this.id = task.Id;
            this.title = task.Title;
            this.description = task.Description;
            this.dueDate = task.DueDate;
            this.creationTime = task.CreationTime;
            this.assigneeEmail = "";
        }

        private void GeneralNonQuery(string query, string badMsg)
        {
            if (!DBConnector.GetInstance().ExecuteNonQuery(query))
            {
                throw new Exception(badMsg);
            }
        }
        /// <summary>
        /// Update a task's title
        /// </summary>
        /// <param name="newTitle">The new title</param>
        
        public void UpdateTaskTitle(string newTitle)
        {
            string query = $"UPDATE Tasks SET title = '{newTitle}' WHERE id = {Id} AND boardId = {boardID}";
            GeneralNonQuery(query, "Something went wrong");
            title = newTitle;
        }
        /// <summary>
        /// Update a task's description
        /// </summary>
        /// <param name="newDesc">The new description</param>
        public void UpdateTaskDescription(string newDesc)
        {
            string query = $"UPDATE Tasks SET description = '{newDesc}' WHERE id = {Id} AND boardId = {boardID}";
            GeneralNonQuery(query, "Something went wrong");
            description = newDesc;
        }

        /// <summary>
        /// Update a task's due date
        /// </summary>
        /// <param name="newDueDate">The new due date</param>
        public void UpdateTaskDueDate(DateTime newDueDate)
        {
            string query = $"UPDATE Tasks SET dueDate = '{newDueDate}' WHERE id = {Id} AND boardId = {boardID}";
            GeneralNonQuery(query, "Something went wrong");
            dueDate = newDueDate;
        }
        /// <summary>
        /// Advanced the task to the next column
        /// </summary>
        public void AdvanceTask()
        {
            columnOrdinal = columnOrdinal + 1;
            string query = $"UPDATE Tasks SET columnOrdinal = {columnOrdinal} WHERE boardID = {boardID} " +
                $"AND id = {id}";
            GeneralNonQuery(query, "Something went wrong");
        }
        /// <summary>
        /// Assign a task to a user
        /// </summary>
        /// <param name="assigner">The assigner</param>
        /// <param name="assignee">The assigned user</param>
        public void AssignTask(string assigner, string assignee)
        {
            string query = $"UPDATE Tasks SET assignee = '{assignee}' WHERE id = {Id} AND boardId = {boardID}";
            GeneralNonQuery(query, "Something went wrong");
            assigneeEmail = assignee;
        }
        /// <summary>
        /// Unassign task from its' user
        /// </summary>
        public void UnassignTask()
        {
            assigneeEmail = "";
            string query = $"UPDATE Tasks SET assignee = '' WHERE id = {Id} AND boardId = {boardID}";
            GeneralNonQuery(query, "Something went wrong");
        }
    }
}