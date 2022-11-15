using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Task
    {

        /// <summary>
        /// Id of the board the task is in
        /// </summary>
        private int boardID;
        [Newtonsoft.Json.JsonIgnore]
        public int BoardID
        {
            get => boardID;
        }
        /// <summary>
        /// The number of the column the task is in.
        /// 0 for 'backlog', 1 for 'in progress', 2 for 'done'
        /// </summary>
        private int columnOrdinal;
        [Newtonsoft.Json.JsonIgnore]
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
        [Newtonsoft.Json.JsonIgnore]
        public string AssigneeEmail
        {
            get => assigneeEmail;
            set => assigneeEmail = value;
        }
        /// <summary>
        /// A dto of the task
        /// </summary>
        private TaskDTO dto;
        [Newtonsoft.Json.JsonIgnore]
        public TaskDTO DTO
        {
            get => dto;
            set => dto = value;
        }

        private log4net.ILog logger = Utility.Logger.GetLogger();


        public Task(int boardID, int columnOrdinal, int ID, string title, string description, DateTime dueDate)
        {
            this.boardID = boardID;
            this.columnOrdinal = columnOrdinal;
            this.id = ID;
            this.title = title;
            this.description = description;
            this.creationTime = DateTime.Now;
            this.dueDate = dueDate;
            this.assigneeEmail = "";
            dto = new TaskDTO(boardID, columnOrdinal, ID, title, description, dueDate);
        }

        public Task(TaskDTO dto) 
        {
            this.boardID = dto.BoardID;
            this.columnOrdinal = dto.ColumnOrdinal;
            this.id = dto.Id;
            this.title = dto.Title;
            this.description = dto.Description;
            this.creationTime = dto.CreationTime;
            this.dueDate = dto.DueDate;
            this.assigneeEmail = dto.AssigneeEmail;
            this.dto = dto;
        }

        /// <summary>
        /// Update a task's title
        /// </summary>
        /// <param name="newTitle">The new title</param>
        
        public void UpdateTaskTitle(string email, string newTitle)
        {
            if (assigneeEmail != "" && assigneeEmail != email)
            {
                throw new Exception("The user that's trying to change the title is not the assignee.");
            }
            title = newTitle;
            dto.UpdateTaskTitle(newTitle);
        }
        /// <summary>
        /// Update a task's description
        /// </summary>
        /// <param name="newDescription">The new description</param>
        public void UpdateTaskDescription(string email, string newDescription)
        {
            if (assigneeEmail != "" && assigneeEmail != email)
            {
                throw new Exception("The user that's trying to change the description is not the assignee.");
            }
            description = newDescription;
            dto.UpdateTaskDescription(newDescription);
        }
        /// <summary>
        /// Update a task's due date
        /// </summary>
        /// <param name="newDueDate">The new due date</param>
        public void UpdateTaskDueDate(string email, DateTime newDueDate)
        {
            if (assigneeEmail != "" && assigneeEmail != email)
            {
                throw new Exception("The user that's trying to change the due date is not the assignee.");
            }
            dueDate = newDueDate;
            dto.UpdateTaskDueDate(newDueDate);
        }
        /// <summary>
        /// Assign a task to a user
        /// </summary>
        /// <param name="assigner">The user assigning</param>
        /// <param name="assignee">The user assigned</param>
        /// <exception cref="Exception">The user can't assign this task</exception>
        public void AssignTask(string assigner, string assignee)
        {
            if (assigneeEmail != "" && assigneeEmail != assigner)
            {
                logger.Warn("Failed to assign task because the assigner doesn't have permissions");
                throw new Exception("User can't assign to this task");
            }
            assigneeEmail = assignee;
            logger.Info("Assigned " + assignee + " to the task: " + Id);
            dto.AssignTask(assigner, assignee);
        }
        /// <summary>
        /// Returns if the user is assigned to this task
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <returns></returns>
        public bool IsAssigned(string email)
        {
            if (email != null && email == assigneeEmail)
            {
                return true;
            }
            if (assigneeEmail == "")
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Unassign this task from its' assignee
        /// </summary>
        public void UnassignTask()
        {
            assigneeEmail = "";
            dto.UnassignTask();
        }
        /// <summary>
        /// Advance the task to the next column
        /// </summary>
        public void AdvanceTask()
        {
            columnOrdinal = columnOrdinal + 1;
            dto.AdvanceTask();
        }
    }
}