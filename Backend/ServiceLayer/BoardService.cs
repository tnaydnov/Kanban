using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using System.Text.Json;
using Newtonsoft.Json;
using IntroSE.Kanban.Backend.Utility;
using log4net;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class BoardService
    {
        private BoardController bc;
        public BoardController Bc { get => bc; }
        private ILog logger = Logger.GetLogger();
        private static int MAX_TASK_DESC_LENGTH = 300;
        private static int MAX_TASK_TITLE_LENGTH = 50;
        public BoardService()
        {
            bc = new BoardController();
        }

        /// <summary>
        /// this function dynamically invokes methods in a more functional programming paradigm
        /// </summary>
        /// <param name="method">function to be invoked</param>
        /// <param name="ret">value to return if no errors occured</param>
        /// <param name="args">the method's arguments</param>
        /// <returns>json string of the result of the procedure</returns>
        private string InvokeMethod(Delegate method, string ret, params object[] args)
        {
            try
            {
                method.DynamicInvoke(args);
                return ret;
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new Response(ex.Message, true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        /// <summary>
        /// Called when a successful registeration occurs. Adds a new (key,value) pair to the user boards dictionary
        /// </summary>
        /// <param name="email">newly registered user</param>

        public void Register(string email)
        {
            bc.Register(email);
        }

        /// <summary>
        /// Add a new board to a user
        /// </summary>
        /// <param name="email">email of the user to add the board to</param>
        /// <param name="name"> name of the board that is being added</param>
        /// <returns>Response indicating the outcome of the procedure</returns>
        public string AddBoard(string email, string name, UserService US)
        {
/*            if (name != null && IsEmptyOrWhiteSpace(name))
            {
                logger.Warn(email + " tried to create a board with invalid name");
                throw new Exception("Cannot have an empty board name");
            }*/
            if (name == null || string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(name))
            {
                logger.Warn(email + " tried to create a board with invalid name");
                return JsonConvert.SerializeObject(new Response("Cannot have an empty board name", true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            try {
                bc.AddBoard(email, name, US.Uc);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new Response(e.Message, true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        /// <summary>
        /// Removes a board from a given user
        /// </summary>
        /// <param name="email">user that made the request</param>
        /// <param name="boardName">board to be removed</param>
        /// <returns>Reponse with the outcome of the procedure</returns>
        public string RemoveBoard(string email, string name) {
            try
            {
                bc.RemoveBoard(email, name);
                return "{}";
            }
            catch(Exception e)
            {
                return JsonConvert.SerializeObject(new Response(e.Message, true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            //return InvokeMethod(new Action<string, string>(bc.RemoveBoard), "{}", email, name);
        }


        /// <summary>
        /// This method adds a new task to the specified user.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>Response with user-email, unless an error occurs.</returns>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            if (dueDate < DateTime.Now)
                return JsonConvert.SerializeObject(new Response("Due date cannot be in the past.", true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (string.IsNullOrWhiteSpace(title) || title.Length > MAX_TASK_TITLE_LENGTH || string.IsNullOrEmpty(title))
            {
                Response response = new Response("Invalid title. A valid  title must have up to 50 characters and cannot be empty.", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            if (description == null)
            {
                description = "";
            }
            if (description.Length > MAX_TASK_DESC_LENGTH)
            {
                Response response = new Response("Description too long", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            try
            {
                bc.AddTask(email, boardName, title, description, dueDate);
                return "{}";    
            }
            catch (Exception e)
            {
                Response response = new Response(e.Message, true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        /// <summary>
        /// This method advances a task to the next column in a user's board
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column Id. The first column is identified by 0, the Id increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task Id</param>
        /// <returns>The string "{}", unless an error occurs.</returns>
        public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                bc.AdvanceTask(email, boardName,columnOrdinal, taskId);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new Response(e.Message, true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            //return InvokeMethod(new Action<string, string, int ,int>(bc.AdvanceTask), "{}", email, boardName, columnOrdinal, taskId);
        }

        public void DeleteData()
        {
            bc.DeleteData();
        }

        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column Id. The first column is identified by 0, the Id increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>Response with the result of the procedure</returns>
        public string LimitColumn(string email, string boardName, int columnNumber, int newLimit) {
            try
            {
                bc.LimitColumnTasks(email, boardName, columnNumber, newLimit);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new Response(e.Message, true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            //return InvokeMethod(new Action<string, string, int, int>(bc.LimitColumnTasks), "{}", email, boardName, columnNumber, newLimit);
        }

        public string LoadData()
        {
            try
            {
                bc.LoadData();
                Logger.GetLogger().Info("board data loaded successfully");
                return "{}";
            }
            catch (Exception e)
            {
                Response response = new Response(e.Message, true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }


        /// <summary>
        /// This method assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user performing the action</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column number. The first column is 0, the number increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified a task ID</param>        
        /// <param name="emailAssignee">Email of the asignee user</param>
        /// <returns>An empty response, unless an error occurs</returns>
        public string AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {
            try
            {
                bc.AssignTask(email, boardName, columnOrdinal, taskID, emailAssignee);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new Response(e.Message, true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            //return InvokeMethod(new Action<string, string, int, int, string>(bc.AssignTask), "{}", email, boardName, columnOrdinal, taskID, emailAssignee);
        }

        /// <summary>
        /// Gets the specified column's tasks limit
        /// </summary>
        /// <param name="email">user sending the reuqest</param>
        /// <param name="boardName">board in which the column appears</param>
        /// <param name="columnNumber">The column Id. The first column is identified by 0, the Id increases by 1 for each column</param>
        /// <returns>Json response with the limit of the column, unless an error occurs.</returns>
        public string GetColumnLimit(string email, string boardName, int columnNumber)
        {
            try
            {
                int output = bc.GetColumnLimit(email, boardName, columnNumber);
                Response response = new Response(output);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception e)
            {
                Response response = new Response(e.Message, true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        /// <summary>
        /// Gets a column name
        /// </summary>
        /// <param name="email"> user that holds the column</param>
        /// <param name="boardName">board that holds the column</param>
        /// <param name="columnNumber">The column Id. The first column is identified by 0, the Id increases by 1 for each column</param>
        /// <returns> Json response with the name of the column, unless an error occurs.</returns>
        public string GetColumnName(string email, string boardName, int columnNumber)
        {
            try
            {
                string output = bc.GetColumnName(email, boardName, columnNumber);
                Response response = new Response(output);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception e)
            {
                Response response = new Response(e.Message, true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        public string JoinBoard(string email, int boardID, UserService US)
        {
            try
            {
                bc.JoinBoard(email, boardID, US.Uc);
                return "{}";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            //return InvokeMethod(new Action<string, int, UserController>(bc.JoinBoard), "{}", email, boardID, US.Uc);
        }


        /// <summary>
        /// This method returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column Id. The first column is identified by 0, the Id increases by 1 for each column</param>
        /// <returns>Response with a list of the column's tasks, unless an error occurs.</returns>
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            try
            {
                List<BusinessLayer.Task> output = bc.GetColumn(email, boardName, columnOrdinal);
                Response response = new Response(output);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception e)
            {
                Response response = new Response(e.Message, true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }


        /// <summary>
        /// returns the user's in progress tasks
        /// </summary>
        /// <param name="email">user requesting to view the tasks</param>
        /// <returns>json string of the list of tasks</returns>
        public string InProgressTasks(string email)
        {
            try
            {
                List<BusinessLayer.Task> output = bc.InProgressTasks(email);
                Response response = new Response(output);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception e)
            {
                Response response = new Response(e.Message, true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }


        /// <summary>
        /// This method removes a user from the members list of a board.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="boardID">The board's ID</param>
        /// <returns>An empty response, unless an error occurs</returns>
        public string LeaveBoard(string email, int boardID)
        {
            try
            {
                bc.LeaveBoard(email, boardID);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new Response(e.Message, true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            //return InvokeMethod(new Action<string, int>(bc.LeaveBoard), "{}", email, boardID);
        }

        /// <summary>
        /// This method transfers a board ownership.
        /// </summary>
        /// <param name="currentOwnerEmail">Email of the current owner.</param>
        /// <param name="newOwnerEmail">Email of the new owner</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>An empty response, unless an error occurs</returns>
        public string TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            try
            {
                bc.TransferOwnership(currentOwnerEmail, newOwnerEmail, boardName);
                return "{}";
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(new Response(e.Message, true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            // return InvokeMethod(new Action<string, string, string> (bc.TransferOwnership), "{}", currentOwnerEmail, newOwnerEmail, boardName);
        }
    }
}
