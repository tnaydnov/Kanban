using Frontend.Model;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Frontend.ViewModel
{
    public class BackendController
    {
        private ServiceController service;
        private BackendController()
        {
            /*1. One user (email: “mail@mail.com”, password: “Password1”).
2. One board (for this user), named "board1", that has three tasks, one in each column.
3. One board (for this user), named "board2", with no tasks.
*/

            service = new ServiceController();
            service.Register("mail@mail.com", "Password1");
            service.AddBoard("mail@mail.com", "board1");
            service.AddTask("mail@mail.com", "board1", "task1", "wow what a great kanban project", new DateTime(2025, 01, 01));
            service.AddTask("mail@mail.com", "board1", "task2", "dont you agree?", new DateTime(2025, 01, 01));
            service.AddTask("mail@mail.com", "board1", "task3", "i think it deserves 100", new DateTime(2025, 01, 01));
            service.AdvanceTask("mail@mail.com", "board1", 0, 2);
            service.AdvanceTask("mail@mail.com", "board1", 1, 2);
            service.AdvanceTask("mail@mail.com", "board1", 0, 1);


            service.AddBoard("mail@mail.com", "board2");
            service.Logout("mail@mail.com");

        }
        private static BackendController instance = new BackendController();
        public static BackendController Instance  
        {
                get 
                {
                    return instance;
                }
        }
        /*
        public BackendController()
        {
             service = new ServiceController();
        }
        */
        internal UserModel Login(string email, string password) // if there's a problem, try putting back the "new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }" in the login function in the backend
        {
            ResponseT<string> res = JsonConvert.DeserializeObject<ResponseT<string>>(service.Login(email, password));
            if (res.ErrorOccured())
            {
                throw new Exception(res.ErrorMessage);
            }
            return new UserModel(email);
        }

        internal UserModel Register(string email, string password) // if there's a problem, try putting back the "new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }" in the login function in the backend
        {
            ResponseT<string> res = JsonConvert.DeserializeObject<ResponseT<string>>(service.Register(email, password));
            if (res.ErrorOccured())
            {
                throw new Exception(res.ErrorMessage);
            }
            return new UserModel(email);
        }

        internal List<string> GetUserBoards(string email)
        {
            ResponseT<List<string>> res = JsonConvert.DeserializeObject<ResponseT<List<string>>>(service.GetUserBoards(email));
            if (res.ErrorOccured())
            {
                throw new Exception(res.ErrorMessage);
            }
            return res.ReturnValue;
        }

        internal ColumnModel GetColumn(string email, string boardName, int columnOrdinal)
        {
            ResponseT<List<TaskModel>> tasks = JsonConvert.DeserializeObject<ResponseT<List<TaskModel>>>(service.GetColumn(email, boardName, columnOrdinal));
            if (tasks.ErrorOccured())
            {
                throw new Exception(tasks.ErrorMessage);
            }
            return new ColumnModel(tasks.ReturnValue);
        }

        internal void Logout(string email)
        {
            ResponseT<string> tasks = JsonConvert.DeserializeObject<ResponseT<string>>(service.Logout(email));
            if (tasks.ErrorOccured())
            {
                throw new Exception(tasks.ErrorMessage);
            }
        }





    }
}