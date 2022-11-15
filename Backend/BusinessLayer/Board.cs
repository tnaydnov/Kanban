using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class Board
    {
        private string name;
        public string Name
        {
            get => name;
            set => name = value;
        }
        private Column backlog;
        public Column Backlog { get => backlog;}

        private Column inProgress;
        public Column InProgress { get => inProgress;}
        private Column done;
        public Column Done { get => done; }

        private int nextTaskID;

        private int id;
        public int Id { get => id; }

        private string owner;
        public string Owner { get => owner; }
        

        private HashSet<string> usernames;

        private BoardDTO dto;


        private log4net.ILog logger = Utility.Logger.GetLogger();

        public Board(string boardName, int id, string creatorName)
        {
            name = boardName;
            backlog = new Column("backlog", id);
            inProgress = new Column("in progress", id);
            done = new Column("done", id);
            this.id = id;
            nextTaskID = 0;
            usernames = new HashSet<string>();
            owner = creatorName;
            dto = new BoardDTO(id, name, creatorName, nextTaskID, usernames);
        }

        public Board(BoardDTO boardDTO)
        {
            this.dto = boardDTO;
            this.name = boardDTO.Name;
            this.owner = boardDTO.Owner;
            this.usernames = boardDTO.Users;
            this.nextTaskID = boardDTO.NextTaskID;
            this.id = boardDTO.Id;
        }

        
        public List<Task> getInProgressTasks()
        {
            return inProgress.GetTasksList();
        }

        /// <summary>
        /// retrieves a column based on the column ordinal
        /// </summary>
        /// <param name="columnNumber"> 0 = backlog, 1 = in progress, 2 = done</param>
        /// <returns> column object based on the given ordinal </returns>
        public Column GetColumn(int columnNumber)
        {
            if (columnNumber > 2 || columnNumber < 0)
                return null;
            if (columnNumber == 0)
            {
                return backlog;
            }
            else if (columnNumber == 1)
            {
                return inProgress;
            }
            else  // (columnNumber == 2)
            {
                return done;
            }
        }

        /// <summary>
        /// limit the maximum number of tasks in a specified column
        /// </summary>
        /// <param name="columnNumber">column ordinal. 0 = backlog, 1 = in progress, 2 = done</param>
        /// <param name="newLimit">new limit of the max number of tasks</param>
        /// <exception cref="Exception">throws exception if the new limit is an invalid number</exception>
        public void LimitColumnTasks(int columnNumber, int newLimit)
        {
            if (newLimit < -1)
            {
                logger.Warn("Failed to limit column tasks due to invalid limit");
                throw new Exception("Invalid limitation of tasks");
            }
            if (newLimit % 1 != 0)
            {
                logger.Warn("Failed to limit column tasks due to invalid limit");
                throw new Exception("Invalid limitation of tasks");
            }
            if (newLimit == -1)
            {
                newLimit = int.MaxValue;
            }

            Column col = GetColumn(columnNumber);
            if (col == null)
            {
                logger.Warn("Failed to limit column tasks due to invalid column ordinal");
                throw new Exception("Invalid column");
            }
            col.SetMax(newLimit);
            logger.Info("Max tasks limited to " + newLimit);
        }

        /// <summary>
        /// handles adding a board to the database
        /// </summary>
        public void AddBoard() { 
            dto.AddBoard(owner, id, name);
            backlog.AddColumnToDB();
            inProgress.AddColumnToDB();
            done.AddColumnToDB();
        }

        /// <summary>
        /// delete a board and all of it's contents
        /// </summary>
        /// <param name="email">deleter's email</param>
        /// <returns></returns>
        public void RemoveBoard(string email)
        {
            if (email != owner)
            {
                logger.Info("Non owner attempted to delete board");
                throw new Exception("Only board owner can delete a board");
            }
            dto.RemoveBoard();
        }


        /// <summary>
        /// gets the limit of tasks of the specified column
        /// </summary>
        /// <param name="boardName">name of the board that owns the task</param>
        /// <param name="columnNumber">column ordinal</param>
        /// <returns></returns>
        /// <exception cref="Exception">throws exception if column number is invalid</exception>
        public int GetColumnLimit(string boardName, int columnNumber)
        {
            Column col = GetColumn(columnNumber);
            if (col == null)
                throw new Exception("Invalid column");
            if (col.MaxTasks == int.MaxValue)
                return -1;
            return col.MaxTasks;
        }
        /// <summary>
        /// Add a task to this board
        /// </summary>
        /// <param name="email">The user to add the task to</param>
        /// <param name="title">The title of the task</param>
        /// <param name="description">The description of the task</param>
        /// <param name="dueDate">The due date of the task</param>
        /// <exception cref="Exception"></exception>
        public void AddTask(string email, string title, string description, DateTime dueDate)
        {
            if (IsInBoard(email))
            {
                backlog.AddTask(id, 0,nextTaskID, title, description, dueDate);
                nextTaskID++;
                dto.nextTaskIdPlusPlus();
                logger.Info("Task was successfully added");
            }
            else
            {
                logger.Warn("Cannot add a task since the email provided is not registered in the board");
                throw new Exception("Cannot add a task since the email provided is not registered in the board");
            }
        }

        /// <summary>
        /// checks if a user is a member of the board (either owner or just a member)
        /// </summary>
        /// <param name="email">email of the user to be checked</param>
        /// <returns>true if the user is in the board</returns>
        public bool IsInBoard(string email)
        {
            return owner == email || usernames.Contains(email);
        }

        /// <summary>
        /// advance a task to the next column. cannot adnvace past "done" column
        /// </summary>
        /// <param name="email">email of the user that is assigned to the task</param>
        /// <param name="columnOrdinal">column number</param>
        /// <param name="taskId">id of the task to be advanced</param>
        /// <exception cref="Exception">throws exception if the column number is invallid, if the user is not assigned to the task, or if the task doesn't exist</exception>
        public void AdvanceTask(string email, int columnOrdinal, int taskId)
        {
            
            Column c = GetColumn(columnOrdinal);
            if (c == null)
            {
                logger.Warn("Cannot advance task since an invalid column ordinal was entered");
                throw new Exception("Task could not advance because of a wrong column ordinal");
            }
            Task t = c.GetTask(taskId);
            if (t == null)
            {
                logger.Warn("Cannot advance task because it doesn't exist");
                throw new Exception("Task could not advance because of a wrong task id");
            }
            if (!t.IsAssigned(email))
            {
                logger.Warn("Cannot advance task because the user advancing it is not assigned to it");
                throw new Exception("Cannot advance task because the user advancing it is not assigned to it");
            }
            if (c == backlog)
            {
                inProgress.takeAdvancingTask(t);
                //dto.AdvanceTask(c.dto, inProgress.dto, t.DTO);
                backlog.removeAdvancingTask(t);
                t.AdvanceTask();
                logger.Info("Task " + t.Title + " advanced");
            }
            else if (c == inProgress)
            {
                done.takeAdvancingTask(t);
                //dto.AdvanceTask(c.dto, done.dto, t.DTO);
                inProgress.removeAdvancingTask(t);
                t.AdvanceTask();
                logger.Info("Task " + t.Title + " advanced");
            }
            else // (c == done)
            {
                logger.Warn("Failed to advance task because the task is already done");
                throw new Exception("Failed to advance task because the task is already done");
            }
        }

        /// <summary>
        /// fill the columns of the boards with the given hash set
        /// </summary>
        /// <param name="columns">hsould have 3 columns - one for each column of the board</param>
        public void FillColumns(HashSet<Column> columns)
        {
            foreach (Column c in columns)
            {
                if(c == null)
                {
                    throw new Exception("Error in assigning columns to board");
                }
                else if (c.Name == "backlog")
                {
                    backlog = c;
                }
                else if (c.Name == "in progress")
                {
                    inProgress = c;
                }
                else if (c.Name == "done")
                {
                    done = c;
                }
            }
        }

        /// <summary>
        /// add a user to the members of the board
        /// </summary>
        /// <param name="email">email of the user to be added</param>
        /// <exception cref="Exception">throws exception if the user is already in the board</exception>
        public void AddUser(string email)
        {
/*            if (usernames.Contains(email) || owner == email)
            {
                logger.Warn(email + " attempted to join a board that he's already in");
                throw new Exception(email + " is already in " + name);
            }*/
            dto.AddUser(email);
            logger.Info(email + " added to board " + name);
            usernames.Add(email);
        }

        /// <summary>
        /// remove user from the board
        /// </summary>
        /// <param name="email">email of the user to be removed</param>
        /// <exception cref="Exception">throws exception if the user is not in the board, or if the user is the owner</exception>
        public void RemoveUser(string email)
        {
            if (usernames.Contains(email))
            {
                dto.RemoveUser(email);
                usernames.Remove(email);
                UnassignTasks(email);
                logger.Info(email + " removed from board " + name);
            }
            else if (owner == email)
            {
                logger.Warn("Attempt to remove board owner failed");
                throw new Exception("Cannot remove owner from board without providing another owner");
            }
            else
            {
                logger.Warn("Attempt to remove user from board that the user was not in");
                throw new Exception(email + " is not in " + name);
            }
        }



        /// <summary>
        /// unassign all tasks of the user
        /// </summary>
        /// <param name="email">user to be unassinged</param>
        private void UnassignTasks(string email)
        {
            backlog.UnassignTasks(email);
            inProgress.UnassignTasks(email);
        }

        /// <summary>
        /// change board owner
        /// </summary>
        /// <param name="currentOwner"></param>
        /// <param name="newOwner"></param>
        /// <exception cref="Exception">throws exception if the owner email deosn't match the actual owner, or if the new owner is not in the board</exception>
        public void ChangeOwner(string currentOwner, string newOwner)
        {
            if (currentOwner != owner)
            {
                logger.Warn("Attempt to change owner of board failed due to incorrect currentOwner name");
                throw new Exception("Failed to change owner because " + currentOwner + " is not the owner of the board");
            }
            if (!usernames.Contains(newOwner) && owner != newOwner)
            {
                logger.Warn("Attempt to change owner of board failed due to newOwner not in the board");
                throw new Exception("Failed to change owner because " + newOwner + " is not the in the board");

            }
            dto.ChangeOwner(newOwner);
            usernames.Add(owner);
            usernames.Remove(newOwner);
            owner = newOwner;
            logger.Info("Owner changed successfully");
        }

        /// <summary>
        /// get a task by the task id and column ordinal
        /// </summary>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public Task GetTask(int columnOrdinal, int taskId)
        {
            try
            {
                Column column = GetColumn(columnOrdinal);
                if (column == null)
                    return null;
                Task t = column.GetTask(taskId);
                if (t != null)
                    return t;
                return null;
            }
            catch (FormatException)
            {
                return null;
            }

        }

        /// <summary>
        /// gets all the assigned tasks of the user
        /// </summary>
        /// <param name="email"></param>
        /// <returns>list of tasks that are assigned to the user</returns>
        public List<Task> GetAllAssignedTasks(string email)
        {
            if (!IsInBoard(email))
            {
                logger.Warn("Attempt to get all asigned tasks to user not in board");
                return null;
            }
            logger.Info(email + " got all his assigned tasks");
            return inProgress.GetAllAssignedTasks(email);   
        }


        /// <summary>
        /// assign a task to a specific user
        /// </summary>
        /// <param name="assigner"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskID"></param>
        /// <param name="assignee"></param>
        /// <exception cref="Exception">thrwos excpetion if the user is not in the board, or if the task doesn't exist</exception>
        public void AssignTask(string assigner, int columnOrdinal, int taskID, string assignee)
        {   
            if (columnOrdinal == 2)
            {
                logger.Warn(assigner + " attempted to assign a task that is done");
                throw new Exception("Cannot change a task that is already done");
            }
            Task t = GetTask(columnOrdinal, taskID);
            if (t == null)
            {
                logger.Warn(assigner + " attempted to reassign a task that doesn't exist");
                throw new Exception("Task does not exist");
            }
            if (!IsInBoard(assignee) || !IsInBoard(assigner))
            {
                throw new Exception("Assigner or assignee not in board");
            }
            t.AssignTask(assigner, assignee);
        }

        /// <summary>
        /// update the title of a specified task
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <param name="newTitle"></param>
        /// <exception cref="Exception">thrwos exception if the task doesn't exist</exception>
        public void UpdateTaskTitle(string email, int columnOrdinal, int taskId, string newTitle)
        {
            Column column = GetColumn(columnOrdinal);
            if (column == null)
            {
                throw new Exception("Invalid column ordinal.");
            }
            if (column.Name == "done")
            {
                throw new Exception("Cannot edit tasks that are done.");
            }
            Task task = GetTask(columnOrdinal, taskId);
            column.UpdateTaskTitle(email, task, newTitle);
        }

        /// <summary>
        /// update the description of a specified task
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <param name="newTitle"></param>
        /// <exception cref="Exception">thrwos exception if the task doesn't exist</exception>
        public void UpdateTaskDescription(string email, int columnOrdinal, int taskId, string newDesc)
        {
            Column column = GetColumn(columnOrdinal);
            if (column == null)
            {
                throw new Exception("Invalid column ordinal.");
            }
            if (column.Name == "done")
            {
                throw new Exception("Cannot edit tasks that are done.");
            }
            Task task = GetTask(columnOrdinal, taskId);
            column.UpdateTaskDescription(email, task, newDesc);
        }

        /// <summary>
        /// update the due date of a specified task
        /// </summary>
        /// <param name="email"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <param name="newTitle"></param>
        /// <exception cref="Exception">thrwos exception if the task doesn't exist</exception>
        public void UpdateTaskDueDate(string email, int columnOrdinal, int taskId, DateTime newDueDate)
        {
            Column column = GetColumn(columnOrdinal);
            if (column == null)
            {
                throw new Exception("Invalid column ordinal.");
            }
            if (column.Name == "done")
            {
                throw new Exception("Cannot edit tasks that are done.");
            }
            Task task = GetTask(columnOrdinal, taskId);
            column.UpdateTaskDueDate(email, task, newDueDate);
        }
    }
}
