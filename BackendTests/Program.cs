using IntroSE.Kanban.Backend;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.BusinessLayer;

class Program
{
    static void Main(String[] args)
    {
        GradingService gradingService = new GradingService();
        BoardServiceTest boardServiceTest = new BoardServiceTest();
        UserServiceTest userServiceTest = new UserServiceTest();
        TaskServiceTest taskServiceTest = new TaskServiceTest();
        DBConnector.GetInstance().ResetDB();
        boardServiceTest.RunTests();
        userServiceTest.RunTests();
        taskServiceTest.runTests();
    }
}


