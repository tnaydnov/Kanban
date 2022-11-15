using System;
using System.Globalization;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.BusinessLayer;
using System.Text.Json;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend
{
    class BoardServiceTest
    {

        public BoardServiceTest()
        {

        }
        public void RunTests()
        {
            DBConnector.GetInstance().ResetDB();
            //addTaskTest();
            //DBConnector.GetInstance().ResetDB();
            //removeBoardTest();
            //DBConnector.GetInstance().ResetDB();
            //addBoardTest();
            //DBConnector.GetInstance().ResetDB();
            advanceTaskPhaseTest("task1");
            DBConnector.GetInstance().ResetDB();
        }


        ///<summary>
        ///This function test Requirement 12
        ///</summary>
        public void addTaskTest()
        {
            Console.WriteLine("Adding task to a user. Should succeed");
            GradingService gradingService = new GradingService();
            gradingService.Register("gal@gmail.com", "123456Ab");
            gradingService.Login("gal@gmail.com", "123456Ab");
            gradingService.AddBoard("gal@gmail.com", "Board1");
            string res = gradingService.AddTask("gal@gmail.com", "Board1", "Task1", "Testing task1", new DateTime());
            gradingService.Logout("gal@gmail.com");
            Console.WriteLine(res);
            

            Console.WriteLine("-----------------------");
            Console.WriteLine("Adding task to a user, title- up to 50. Should fail");//fix this. should return error
            gradingService.Register("tomer@gmail.com", "123456Aa");
            gradingService.Login("tomer@gmail.com", "123456Aa");
            gradingService.AddBoard("tomer@gmail.com", "Board3");
            res = gradingService.AddTask("tomer@gmail.com", "Board3", "zzzzzzzzzzxxxxxxxxxxssssssssssaaaaaaaaaaddddddddddd", "testing task", new DateTime());
            gradingService.Logout("tomeR@gmail.com");
            Console.WriteLine(res);

            Console.WriteLine("-----------------------");//fix this. should return error
            Console.WriteLine("Adding task to a user, description- up to 300. Should fail");
            gradingService.Register("Omer@gmail.ac.il", "123456Aa");
            gradingService.Login("Omer@gmail.ac.il", "123456Aa");
            gradingService.AddBoard("Omer@gmail.ac.il", "Board8");
            res = gradingService.AddTask("Omer@gmail.ac.il", "Board8","testing task", "zzzzzzzzzzxxxxxxxxxssssssssssaaaaaaaaaadddddddddddzzzzzzzzzzxxxxxxxxxxssssssssssaaaaaaaaaadddddddddddzzzzzzzzzzxxxxxxxxxxssssssssssaaaaaaaaaadddddddddddzzzzzzzzzzxxxxxxxxxxssssssssssaaaaaaaaaadddddddddddzzzzzzzzzzxxxxxxxxxxssssssssssaaaaaaaaaadddddddddddzzzzzzzzzzxxxxxxxxxxssssssssssaaaaaaaaaadddddddddddzzzzzzzzzzxxxxxxxxxxssssssssxssaaaaaaaaaaddddddddddd", new DateTime());
            gradingService.Logout("omer@gmail.com");
            Console.WriteLine(res);

            Console.WriteLine("-----------------------");//fix this. should return error
            Console.WriteLine("adding task with empty title. should fail");
            gradingService.AddBoard("Omer@gmail.ac.il", "Board6");
            res = gradingService.AddTask("Omer@gmail.ac.il", "Board6", "", "testing task", new DateTime());
            Console.WriteLine(res);

            Console.WriteLine("-----------------------");
            Console.WriteLine("Adding task to an non exist user. Should fail");
            gradingService.Login("itay@gmail.com", "123456Aa");
            gradingService.AddBoard("itay@gmail.com", "Board1");
            res = gradingService.AddTask("itay@gmail.com", "Board1", "Task1", "Testing task1", new DateTime());
            gradingService.Logout("itay@gmail.com");
            Console.WriteLine(res);

            Console.WriteLine("-----------------------");
            Console.WriteLine("Adding task to a user that not connected. Should fail");
            gradingService.Register("itay@gmail.com", "123456Aa");
            gradingService.AddBoard("itay@gmail.com", "Board3");
            res = gradingService.AddTask("itay@gmail.com", "Board1", "Task1", "Testing task1", new DateTime());
            Console.WriteLine(res);
        }



        /*
        ///<summary>
        ///This function test Requirement 9
        ///</summary>
        public void RemoveTaskTest()
        {


        }
        */






        ///<summary>
        ///This function test Requirement 9
        ///</summary>
        public void addBoardTest()
        {
            GradingService gradingService = new GradingService();
            Console.WriteLine("add two boards with the same name. should fail");
            gradingService.Register("gal@gmail.com", "123456Aa");
            gradingService.Login("gal@gmail.com", "123456Aa");
            gradingService.AddBoard("gal@gmail.com", "Board1");
            gradingService.AddTask("gal@gmail.com", "Board1", "task", "testing task", new DateTime());
            //Console.WriteLine(res);
            string res = gradingService.AddBoard("gal@gmail.com", "Board1");
            Console.WriteLine(res);

            Console.WriteLine("-----------------------");
            Console.WriteLine("adding boards with the same name to a different users. should succeed");
            gradingService.Register("gal@gmail.com", "123456Aa");
            gradingService.Login("gal@gmail.com", "123456Aa");
            res = gradingService.AddBoard("gal@gmail.com", "Board2");
            Console.WriteLine(res);
            gradingService.Logout("gal@gmail.com");
            gradingService.Register("itay@gmail.com", "123456Aa");
            gradingService.Login("itay@gmail.com", "123456Aa");
            res = gradingService.AddBoard("itay@gmail.com", "Board2");
            gradingService.Logout("itay@gmail.com");
            Console.WriteLine(res);

            Console.WriteLine("-----------------------");
            Console.WriteLine("adding an empty board name. should fail");
            gradingService.Register("omer@gmail.com", "123456Aa");
            gradingService.Login("omer@gmail.com", "123456Aa");
            res = gradingService.AddBoard("omer@gmail.com", " ");
            Console.WriteLine(res);

            Console.WriteLine("-----------------------");
            Console.WriteLine("adding an empty board name. should fail");
            res = gradingService.AddBoard("omer@gmail.com", "");
            gradingService.Logout("omer@gmail.com");
            Console.WriteLine(res);
        }





        ///<summary>
        ///This function test Requirement 9
        ///</summary>
        public void removeBoardTest()
        {
            GradingService gradingService = new GradingService();
            Console.WriteLine("-----------------------");
            Console.WriteLine("Remove board from a user. should succeed");
            gradingService.Register("gal@gmail.com", "123456Aa");
            gradingService.Login("gal@gmail.com", "123456Aa");
            string res = gradingService.AddBoard("gal@gmail.com", "Board4");
            Console.WriteLine(res);
            gradingService.RemoveBoard("gal@gmail.com", "Board4");
            Console.WriteLine(res);
            gradingService.Logout("gal@gmail.com");

            Console.WriteLine("-----------------------");
            Console.WriteLine("Remove board from a user that not logged in. nshould fail");
            gradingService.Register("itay@gmail.com", "123456Aa");
            gradingService.AddBoard("itay@gmail.com", "Board5");
            res = gradingService.RemoveBoard("itay@gmail.com", "Board5");
            Console.WriteLine(res);

            Console.WriteLine("-----------------------");
            Console.WriteLine("Remove board that not exist from a user. should fail");
            gradingService.Register("gal@gmail.com", "123456Aa");
            gradingService.Login("gal@gmail.com", "123456Aa");
            res = gradingService.RemoveBoard("gal@gmail.com", "Board2");
            Console.WriteLine(res);
        }



        ///<summary>
        ///This function test Requirement 13
        ///</summary>
        public void advanceTaskPhaseTest(string Title)
        {
            Console.WriteLine("----------------");
            GradingService gradingService = new GradingService();
            Console.WriteLine("advace task twice. should succeed");
            gradingService.Register("gal@gmail.com", "123456Aa");
            gradingService.Login("gal@gmail.com", "123456Aa");
            gradingService.AddBoard("gal@gmail.com", "Board1");
            gradingService.AddTask("gal@gmail.com", "Board1", Title, "testing task1", new DateTime());
            gradingService.AssignTask("gal@gmail.com", "Board1", 0, 0, "gal@gmail.com");
            string res = gradingService.AdvanceTask("gal@gmail.com", "Board1", 0, 0);
            Console.WriteLine(res);
            res = gradingService.AdvanceTask("gal@gmail.com", "Board1", 1, 0);
            gradingService.Logout("gal@gmail.com");
            Console.WriteLine(res);

            Console.WriteLine("-----------------------");
            Console.WriteLine("advace task three times. should fail");
            gradingService.Register("itay@gmail.com", "123456Aa");
            gradingService.Login("itay@gmail.com", "123456Aa");
            gradingService.AddBoard("itay@gmail.com", "Board2");
            gradingService.AddTask("itay@gmail.com", "Board2", Title, "testing task1", new DateTime());
            res = gradingService.AdvanceTask("itay@gmail.com", "Board2", 0, 0);
            Console.Write(res);
            res = gradingService.AdvanceTask("itay@gmail.com", "Board2", 1, 0);
            Console.Write(res);
            res = gradingService.AdvanceTask("itay@gmail.com", "Board2", 2, 0);
            Console.Write(res);

            Console.WriteLine("-----------------------");
            Console.WriteLine("advace unexist task. should fail");
            gradingService.Register("gal@gmail.com", "123456Aa");
            gradingService.Login("gal@gmail.com", "123456Aa");
            gradingService.AddBoard("gal@gmail.com", "Board2");
            gradingService.AddTask("gal@gmail.com", "Board2", Title, "testing task9", new DateTime());
            res = gradingService.AdvanceTask("gal@gmail.com", "Board2", 0, 4);
            gradingService.Logout("gal@gmail.com");

            Console.Write(res);
        }
    }
}