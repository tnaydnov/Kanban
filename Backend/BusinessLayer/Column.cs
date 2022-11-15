using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Column
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
        /// The limit on the maximum tasks available in this column
        /// </summary>
        private int maxTasks;
        public int MaxTasks
        {
            get => maxTasks;
            set => maxTasks = value;
        }
        /// <summary>
        /// List of tasks in this column
        /// </summary>
        private List<Task> tasks;
        public List<Task> Tasks
        {
            get => tasks;
            set => tasks = value;
        }
        /// <summary>
        /// The id of the board the task is in
        /// </summary>
        private int boardID;
        public int BoardID { get => boardID; }
        /// <summary>
        /// The dto of the column
        /// </summary>
        private ColumnDTO dto;
        public ColumnDTO DTO
        {
            get => dto;
            set => dto = value;
        }

        private log4net.ILog logger = Utility.Logger.GetLogger();

        public Column(string name, int boardID)
        {
            this.name = name;
            this.tasks = new List<Task>();
            this.maxTasks = int.MaxValue; //If there's no limit on number of tasks, the value is the maximum value of int
            this.boardID = boardID;
            this.dto = new ColumnDTO(this);

        }

        public Column(ColumnDTO column)
        {
            this.name = column.Name;
            this.maxTasks = column.MaxTasks;
            this.dto = column;
            tasks = new List<Task>();
            this.dto = column;
            this.boardID = column.BoardID;
            foreach (TaskDTO task in column.Tasks)
            {
                Task t = new Task(task.BoardID, task.ColumnOrdinal, task.Id, task.Title, task.Description, task.DueDate);
                this.tasks.Add(t);
            }
        }

        /// <summary>
        /// Add a task to this column
        /// </summary>
        /// <param name="task">The task</param>
        /// <exception cref="Exception"></exception>


        public void AddTask(Task task)
        {
            if (tasks.Contains(task))
            {
                logger.Warn("Cannot add task because it already exists.");
                throw new Exception("This task already exists.");
            }
            for (int i = 0; i < tasks.Count; i++)
            {
                if (task.Id == tasks[i].Id)
                {
                    logger.Warn("Cannot add task because a task with the same Id already exists.");
                    throw new Exception("A task with the same Id already exists.");
                }
            }
            if (tasks.Count == maxTasks)
            {
                logger.Warn("Cannot add task because there are maximum tasks in this column.");
                throw new Exception("The maximum capacity of tasks in this column is full.");
            }
            tasks.Add(task);
            logger.Info("Added task: " + task.Title);
            dto.AddTask(task.DTO);
        }
        /// <summary>
        /// Add a task to this column
        /// </summary>
        /// <param name="ID">The task id</param>
        /// <param name="title">The task's title</param>
        /// <param name="description">The task's description</param>
        /// <param name="dueDate">The task's due date</param>
        /// <exception cref="Exception"></exception>
        public void AddTask(int boardID, int columnOrdinal, int ID, string title, string description, DateTime dueDate)
        {
            if (tasks.Count == maxTasks)
            {
                logger.Warn("Cannot add task because there are maximum tasks in this column.");
                throw new Exception("The maximum capacity of tasks in this column is full.");
            }
            for (int i = 0; i < tasks.Count; i++)
            {
                if (ID == tasks[i].Id)
                {
                    logger.Warn("Cannot add task because a task with the same Id already exists.");
                    throw new Exception("A task with the same Id already exists.");
                }
            }
            Task newTask = new Task(boardID, columnOrdinal, ID, title, description, dueDate);
            tasks.Add(newTask);
            logger.Info("Added task: " + newTask.Title);
            dto.AddTask(newTask.DTO);
        }
        /// <summary>
        /// Add the column to the db
        /// </summary>
        public void AddColumnToDB()
        {
            dto.AddColumnToDB();
        }

        /// <summary>
        /// Remove a task from this column
        /// </summary>
        /// <param name="task">The task we need to remove</param>
        /// <exception cref="Exception"></exception>
        public void removeAdvancingTask(Task task)
        {
            if (!tasks.Contains(task))
            {
                logger.Warn("Cannot remove task because it doesn't exist.");
                throw new Exception("The task doesn't exist.");
            }
            logger.Info("Task: " + task.Title + " is removed.");
            tasks.Remove(task);
        }

        /// <summary>
        /// Update a task's title
        /// </summary>
        /// <param name="task">The task</param>
        /// <param name="newTitle">The new title</param>
        /// <exception cref="Exception"></exception>
        public void UpdateTaskTitle(string email, Task task, string newTitle)
        {
            if (!tasks.Contains(task))
            {
                logger.Warn("Cannot edit task because it doesn't exist.");
                throw new Exception("The task doesn't exist in this column");
            }
            task.UpdateTaskTitle(email, newTitle);
            logger.Info("Task Title changed to: " + newTitle);
        }
        /// <summary>
        /// Update a task's description
        /// </summary>
        /// <param name="task">The task</param>
        /// <param name="newDescription">The new description</param>
        /// <exception cref="Exception"></exception>
        public void UpdateTaskDescription(string email, Task task, string newDescription)
        {
            if (!tasks.Contains(task))
            {
                logger.Warn("Cannot edit task because it doesn't exist.");
                throw new Exception("That task doesn't exist in this column.");
            }
            task.UpdateTaskDescription(email, newDescription);
            logger.Info("Task Description changed to: " + newDescription);
        }
        /// <summary>
        /// Update a task's due date
        /// </summary>
        /// <param name="task">The task</param>
        /// <param name="newDueDate">The new due date</param>
        /// <exception cref="Exception"></exception>
        public void UpdateTaskDueDate(string email, Task task, DateTime newDueDate)
        {
            if (!tasks.Contains(task))
            {
                logger.Warn("Cannot edit task because it doesn't exist.");
                throw new Exception("That task doesn't exist in this column.");
            }
            task.UpdateTaskDueDate(email, newDueDate);
            logger.Info("Task due date changed to: " + newDueDate);
        }
        /// <summary>
        /// Adding a task to this column after getting advanced from the previous column
        /// </summary>
        /// <param name="t">The task</param>
        /// <exception cref="Exception"></exception>
        public void takeAdvancingTask(Task t)
        {
            if (tasks.Count == maxTasks)
            {
                logger.Warn("Cannot add task because there are maximum tasks in this column.");
                throw new Exception("The maximum capacity of tasks in this column is full.");
            }
            tasks.Add(t);
        }

        /// <summary>
        /// Set a limit of the column's tasks
        /// </summary>
        /// <param name="maxTasks">The new limit</param>
        public void SetMax(int maxTasks)
        {
            if (maxTasks < tasks.Count)
            {
                logger.Warn("Cannot change a column limitation to a smaller value than the tasks' amount");
                throw new Exception("Cannot change a column limitation to a smaller value than the tasks' amount");
            }
            logger.Info("Column's tasks limit was changed to: " + maxTasks);
            this.maxTasks = maxTasks;
            dto.SetMax(maxTasks);
        }
        /// <summary>
        /// Get a task by the id
        /// </summary>
        /// <param name="taskID">The task id</param>
        /// <returns></returns>
        public Task GetTask(int taskID)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].Id == taskID)
                {
                    return tasks[i];
                }
            }
            return null;
        }
        /// <summary>
        /// Get a list of tasks in this column
        /// </summary>
        /// <returns></returns>
        public List<Task> GetTasksList()
        {
            return tasks;
        }
        /// <summary>
        /// Get a list of the assigned tasks of a user
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <returns></returns>
        public List<Task> GetAllAssignedTasks(string email)
        {
            List<Task> output = new List<Task>();
            foreach(Task task in tasks)
            {
                if (task.AssigneeEmail == email)
                {
                    output.Add(task);
                }
            }
            return output;
        }
        /// <summary>
        /// Unassign all tasks from a user
        /// </summary>
        /// <param name="email">The user's email</param>
        public void UnassignTasks(string email)
        {
            foreach (Task task in tasks)
            {
                if (task.AssigneeEmail.Equals(email))
                {
                    task.UnassignTask();
                }
            }
            logger.Info("Unassigned " + email + " from all tasks");
        }
    }
}