using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class TaskServiceTest
{

    public TaskServiceTest() { }


    public void runTests()
    {
        DBConnector.GetInstance().ResetDB();
        editTaskDescriptionTest();
        DBConnector.GetInstance().ResetDB();
        editTaskTitleTest();
        DBConnector.GetInstance().ResetDB();
    }

    ///<summary>
    ///This function test Requirement 14,15
    ///</summary>
    public void editTaskTitleTest()
    {
        Console.WriteLine("Editing task of a user. should succeed");
        GradingService gradingService = new GradingService();
        gradingService.Register("gal@gmail.com", "123456Aa");
        gradingService.Login("gal@gmail.com", "123456Aa");
        gradingService.AddBoard("gal@gmail.com", "Board1");
        string res = gradingService.AddTask("gal@gmail.com", "Board1", "task1", "testing task1", new DateTime());
        res = gradingService.UpdateTaskTitle("gal@gmail.com", "Board1", 0, 1, "task2");
        Console.WriteLine(res);

        Console.WriteLine("-----------------------");
        Console.WriteLine("Editing task of an non exist user. should fail");
        gradingService.Login("tomer@gmail.com", "123456Aa");
        gradingService.AddBoard("tomer@gmail.com", "Board1");
        res = gradingService.AddTask("tomer@gmail.com", "Board1", "task1", "testing rask1", new DateTime());
        Console.WriteLine(res);

        Console.WriteLine("-----------------------");
        Console.WriteLine("editing a task that not exist. should fail");
        res = gradingService.UpdateTaskTitle("gal@gmail.com", "Board1", 0, 2, "task2");
        Console.WriteLine(res);

        Console.WriteLine("-----------------------");
        Console.WriteLine("editing a task with an empty name, should fail");
        gradingService.Register("itay@gmail.com", "123456Aa");
        gradingService.Login("itay@gmail.com", "123456Aa");
        gradingService.AddBoard("itay@gmail.com", "Board2");
        gradingService.AddTask("itay@gmail.com", "Board2", "", "testing task2", new DateTime());
        res = gradingService.UpdateTaskTitle("itay@gmail.com", "Board1", 0, 1, "");
        Console.WriteLine(res);


    }



    ///<summary>
    ///This function test Requirement 14,15
    ///</summary>
    public void editTaskDescriptionTest()
    {
        Console.WriteLine("Editing task to a user. should succeed");
        GradingService gradingService = new GradingService();
        gradingService.Register("gal@gmail.com", "123456Aa");
        gradingService.Login("gal@gmail.com", "123456Aa");
        gradingService.AddBoard("gal@gmail.com", "Board1");
        string res = gradingService.AddTask("gal@gmail.com", "Board1", "task1", "testing task1", new DateTime());
        Console.WriteLine(res);
        res = gradingService.UpdateTaskDescription("gal@gmail.com", "Board1", 0, 1, "testing task2");
        Console.WriteLine(res);

        Console.WriteLine("-----------------------");
        Console.WriteLine("Editing task to an non exist user. should fail");
        gradingService.Login("tomer@gmail.com", "123456Aa");
        gradingService.AddBoard("tomer@gmail.com", "Board1");
        res = gradingService.AddTask("tomer@gmail.com", "Board1", "task1", "testing rask1", new DateTime());
        res = gradingService.UpdateTaskDescription("tomer@gmail.com", "Board1", 0, 2, "testing task2");
        Console.WriteLine(res);

        Console.WriteLine("-----------------------");
        Console.WriteLine("editing a task that not exist. should fail");
        res = gradingService.UpdateTaskDescription("gal@gmail.com", "Board1", 0, 3, "task3");
        Console.WriteLine(res);

        Console.WriteLine("-----------------------");
        Console.WriteLine("editing a task with an empty name, should fail");
        gradingService.Register("itay@gmail.com", "123456Aa");
        gradingService.Login("itay@gmail.com", "123456Aa");
        gradingService.AddBoard("itay@gmail.com", "Board2");
        gradingService.AddTask("itay@gmail.com", "Board2", "task7", "testing task7", new DateTime());
        res = gradingService.UpdateTaskDescription("itay@gmail.com", "Board2", 0, 2, " ");
        Console.WriteLine(res);

    }


    /*
    ///<summary>
    ///This function test Requirement 14,15
    ///</summary>
    public void editTaskDueDateTest()
    {

    }
    */
}
