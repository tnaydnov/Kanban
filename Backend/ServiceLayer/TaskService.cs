using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using Newtonsoft.Json;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class TaskService
    {
        private UserController uc;
        public UserController UC
        {
            get => uc;
        }
        private BoardController bc;
        public BoardController BC
        {
            get => bc;
        }

        private static int MAX_TASK_DESC_LENGTH = 300;
        private static int MAX_TASK_TITLE_LENGTH = 50;

        public TaskService(UserController uc, BoardController bc)
        {
            this.uc = uc;
            this.bc = bc;
        }


        /// <summary>
        /// update an existing tasks' title
        /// </summary>
        /// <param name="email">user holding the board that holds the task</param>
        /// <param name="boardName">board holding the column that holds task</param>
        /// <param name="columnOrdinal">column holding the task</param>
        /// <param name="taskId">the task's id</param>
        /// <param name="newTitle">the new title</param>
        /// <returns>Json response with the result of the procedure</returns>
        public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string newTitle)
        {
            if (email == null || boardName == null)
            {
                Response response = new Response("Email & board name cannot be null", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            email = email.ToLower();
            if (string.IsNullOrEmpty(newTitle) || string.IsNullOrWhiteSpace(newTitle))
            {
                Response response = new Response("Cannot have an empty title.", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            if (newTitle.Length > MAX_TASK_TITLE_LENGTH)
            {
                Response response = new Response("Title is too long. Max number of characters is 50", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            if (!uc.exists(email)) //The user doesn't exist
            {
                Response response = new Response("The user trying to access does not exist.", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            if (!uc.IsLoggedIn(email)) //The user isn't logged in
            {
                Response response = new Response("The user trying to access is not logged in.", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            try
            {
                bc.UpdateTaskTitle(email, boardName, columnOrdinal, taskId, newTitle);
                return "{}";
            }
            catch(Exception e)
            {
                Response response = new Response(e.Message, true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        /// <summary>
        /// update an existing tasks description
        /// </summary>
        /// <param name="email">user holding the board that holds the task</param>
        /// <param name="boardName">board holding the column that holds task</param>
        /// <param name="columnOrdinal">column holding the task</param>
        /// <param name="taskId">the task's id</param>
        /// <param name="newDesc">the new description</param>
        /// <returns>Json response with the result of the procedure</returns>
        public string UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string newDesc)
        {
            if (email == null)
            {
                Response response = new Response("Email cannot be null");
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            email = email.ToLower();
            if (newDesc == null)
            {
                Response response = new Response("Description can't be null", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            if (newDesc.Length > MAX_TASK_DESC_LENGTH)
            {
                Response response = new Response("Description is too long. Max number of characters is 300", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            if (!uc.exists(email)) //The user doesn't exist
            {
                Response response = new Response("The user trying to access does not exist.", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            if (!uc.IsLoggedIn(email)) //The user isn't logged in
            {
                Response response = new Response("The user trying to access is not logged in.", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            try
            {
                bc.UpdateTaskDescription(email, boardName, columnOrdinal, taskId, newDesc);
                return "{}";
            }
            catch (Exception e)
            {
                Response response = new Response(e.Message, true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }

        /// <summary>
        /// update an existing task's due date
        /// </summary>
        /// <param name="email">user holding the board that holds the task</param>
        /// <param name="boardName">board holding the column that holds task</param>
        /// <param name="columnOrdinal">column holding the task</param>
        /// <param name="taskId">the task's id</param>
        /// <param name="newDueDate">the new due date</param>
        /// <returns>Json response with the result of the procedure</returns>
        public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime newDueDate)
        {
            if (newDueDate < DateTime.Now)
                return JsonConvert.SerializeObject(new Response("Due date cannot be in the past.", true), Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            if (email == null)
            {
                Response response = new Response("Email cannot be null", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            email = email.ToLower();
            if (!uc.exists(email)) //The user doesn't exist
            {
                Response response = new Response("The user trying to access does not exist.", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            if (!uc.IsLoggedIn(email)) //The user isn't logged in
            {
                Response response = new Response("The user trying to access is not logged in.", true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            try
            {
                bc.UpdateTaskDueDate(email, boardName, columnOrdinal, taskId, newDueDate);
                return "{}";
            }
            catch (Exception e)
            {
                Response response = new Response(e.Message, true);
                return JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
        }
    }
}
