using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using IntroSE.Kanban.Backend.DataAccessLayer.Mappers;

namespace IntroSE.Kanban.Backend.BusinessLayer
{

    public class BoardController
    {
        private static Dictionary<string, HashSet<Board>> boards = new Dictionary<string, HashSet<Board>>();
        public static Dictionary<string, HashSet<Board>> Boards { get => boards; }
        private BoardMapper boardMapper;
        private ColumnMapper columnMapper;
        log4net.ILog logger = Utility.Logger.GetLogger();
        public int nextBoardID { get; private set; }

        public BoardController()
        {
            //DBConnector.GetInstance(); // to initialize db
            nextBoardID = 0;
            columnMapper = new ColumnMapper();
            boardMapper = new BoardMapper();
        }
    

       
        /// <summary>
        /// Get a specific board by name and user email
        /// </summary>
        /// <param name="email"> user whose board will be returned</param>
        /// <param name="boardID">board to be returned</param>
        /// <returns>the board that was looked for, null if no such board exists</returns>
        public Board GetBoard(string email, string boardName)
        {
            HashSet<Board> userBoards = boards[email];
            foreach (Board b in userBoards)
            {
                if (b.Name == boardName)
                {
                    return b;
                }
            }
            return null;
        }

        /// <summary>
        /// Get a specific board by id
        /// </summary>
        /// <param name="boardID">id of the desired board</param>
        /// <returns>Board if found, else null</returns>
        public Board GetBoard(int boardID)
        {
            foreach (HashSet<Board> set in boards.Values)
            {
                foreach (Board b in set)
                {
                    if (b.Id == boardID)
                    {
                        return b;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a board to the user with the given email
        /// </summary>
        /// <param name="email">email of the user to add the board to</param>
        /// <param name="name">name of the new board</param>
        /// <param name="uc">sent as arguement to be able to pass the board to the user object</param>
        /// <exception cref="Exception">throws exception if the user already has a board with that name</exception>
        public void AddBoard(string email, string name, UserController uc)
        {
            Board b = new Board(name, nextBoardID, email);
            if (uc.AddBoard(email, b)) {
                b.AddBoard();
                boards[email].Add(b);
                logger.Info("board " + name + " created for user " + email);
                nextBoardID++;
            }
            else {
                throw new Exception("User already has a board with that name");
            }
        }

        /// <summary>
        /// Called when a successful registeration occurs. Adds a new (key,value) pair to the user boards dictionary
        /// </summary>
        /// <param name="email">newly registered user</param>
        public void Register(string email)
        {
            if (!boards.ContainsKey(email))
            {
                boards.Add(email, new HashSet<Board>());
            }
        }

        /// <summary>
        /// Deletes a board entirely
        /// </summary>
        /// <param name="email">email of the user deleting the board. Must be the owner of the board</param>
        /// <param name="boardName">name of the baord to be deleted</param>
        /// <exception cref="Exception">throws exception if no such board exists</exception>
        public void RemoveBoard(string email, string boardName)
        {
            Board board = GetBoard(email, boardName);
            if (board == null)
            {
                logger.Warn("Failed to remove board " + boardName + ", because a board with that name doesn't exists.");
                throw new Exception("The board '" + boardName + "' does not exist");
            }
            board.RemoveBoard(email);
            boards[email].Remove(board);
            logger.Info("board " + boardName + " removed from user " + email);
        }


        /// <summary>
        /// limit the maximum number of tasks in a specified column
        /// </summary>
        /// <param name="email">email of the user requesting to limit the number of tasks</param>
        /// <param name="boardName">name of the board</param>
        /// <param name="columnNumber">column ordinal. 0 = backlog, 1 = in progress, 2 = done</param>
        /// <param name="newLimit">new limit of the max number of tasks</param>
        /// <exception cref="Exception">throws exception if no such board exists</exception>
        public void LimitColumnTasks(string email, string boardName, int columnNumber, int newLimit)
        {
            Board board = GetBoard(email, boardName);
            if (board == null)
            {
                logger.Warn("Failed to limit tasks in board " + boardName + ", because a board with that name doesn't exists.");
                throw new Exception("The board '" + boardName + "' does not exist");
            }
            board.LimitColumnTasks(columnNumber, newLimit);
        }

        /// <summary>
        /// gets the max number of tasks in a specified column
        /// </summary>
        /// <param name="email">email of the user making the request</param>
        /// <param name="boardName">name of the board that owns the column</param>
        /// <param name="columnNumber">column ordinal. 0 = backlog, 1 = in progress, 2 = done</param>
        /// <returns>max number of tasks that the column can hold</returns>
        /// <exception cref="Exception">throws exception if no such board exists</exception>
        public int GetColumnLimit(string email, string boardName, int columnNumber)
        {
            Board board = GetBoard(email, boardName);
            if (board == null)
            {
                throw new Exception("The board '" + boardName + "' does not exist");
            }
            return board.GetColumnLimit(boardName, columnNumber);
        }

        /// <summary>
        /// gets a columns name
        /// </summary>
        /// <param name="email">email of the user making the request</param>
        /// <param name="boardName">name of the board that owns the column</param>
        /// <param name="columnOrdinal">column ordinal. 0 = backlog, 1 = in progress, 2 = done</param>
        /// <returns>name of the column</returns>
        /// <exception cref="Exception">throws expection if no such board exists, or if the column ordinal is invalid</exception>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            Board board = GetBoard(email, boardName);
            if (board == null)
                throw new Exception("The board '" + boardName + "' does not exist");
            Column column = board.GetColumn(columnOrdinal);
            if (column == null)
                throw new Exception("Invalid column");
            return column.Name;
        }


        public void DeleteData()
        {
            boards.Clear();
            nextBoardID = 0;
        }

        /// <summary>
        /// gets a specific column, and all of its tasks
        /// </summary>
        /// <param name="email">email of the user making the request</param>
        /// <param name="boardName">name of the board that owns the column</param>
        /// <param name="columnOrdinal">0 = backlog, 1 = in progress, 2 = done</param>
        /// <returns>a list with all the tasks that are in the column</returns>
        /// <exception cref="Exception">throws exception if no such board exists, or if the column number is invalid</exception>
        public List<Task> GetColumn(string email, string boardName, int columnOrdinal)
        {
            Board board = GetBoard(email, boardName);
            if (board == null)
                throw new Exception("The board '" + boardName + "' does not exist");
            Column column = board.GetColumn(columnOrdinal);
            if (column == null)
                throw new Exception("Invalid column");
            return column.GetTasksList();
        }

        /// <summary>
        /// add a task to a specified board, the task is automatically assigned to the backlog of the board
        /// </summary>
        /// <param name="email">email of the user adding the task</param>
        /// <param name="boardName">name of the board to add the task to</param>
        /// <param name="title">title of the new task</param>
        /// <param name="description">description of the new task</param>
        /// <param name="dueDate">due date of the new task</param>
        /// <exception cref="Exception">throws exception if the board doesn't exist</exception>
        public void AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            Board board = GetBoard(email, boardName);
            if (board == null)
                throw new Exception("The board '" + boardName + "' does not exist");
            board.AddTask(email, title, description, dueDate);
        }

        /// <summary>
        /// advance a task from its current column (or state) to the next one.
        /// </summary>
        /// <param name="email">email of the user advancing the task</param>
        /// <param name="boardName">name of the board that the task is in</param>
        /// <param name="columnOrdinal">0 = backlog, 1 = in progress, 2 = done</param>
        /// <param name="taskId">id of the advancing task</param>
        /// <exception cref="Exception">throws exception if no such board exists</exception>
        public void AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            Board board = GetBoard(email, boardName);
            if (board == null)
                throw new Exception("The board '" + boardName + "' does not exist");
            board.AdvanceTask(email, columnOrdinal, taskId);
        }

        /// <summary>
        /// get a list of all the in progress tasks of a user
        /// </summary>
        /// <param name="email">email of the user making the reuqest</param>
        /// <returns>list of all the in progress tasks of the specified user</returns>
        public List<Task> InProgressTasks(string email)
        {
            HashSet<Board> boardList = boards[email];
            List<Task> tasks = new List<Task>();
            foreach (Board b in boardList) {
                tasks.AddRange(b.getInProgressTasks());
            }
            return tasks;
        }

        /// <summary>
        /// gets a specific task
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <param name="boardName">name of the board that holds the task</param>
        /// <param name="columnOrdinal">column ordinal. 0 = backlog, 1 = in progress, 2 = done</param>
        /// <param name="taskId">id of the task </param>
        /// <returns>the specified task</returns>
        public Task GetTaskInColumn(string email, string boardName, int columnOrdinal, int taskId)
        {
            Board b = GetBoard(email, boardName);
            if(b == null)
                return null;
            return b.GetTask(columnOrdinal, taskId);
        }

        /// <summary>
        /// assign a task to a specific user
        /// </summary>
        /// <param name="assigner">email of the user making the request</param>
        /// <param name="boardName">name of the board that holds the task</param>
        /// <param name="columnOrdinal">ordinal of the column the task is in. 0 = backlog, 1 = in progress, 2 = done</param>
        /// <param name="taskID">id of the task</param>
        /// <param name="assignee">email of the user that is being assigned</param>
        /// <exception cref="Exception">throws expection if the board doesn't exist</exception>
        public void AssignTask(string assigner, string boardName, int columnOrdinal, int taskID, string assignee)
        {
            Board b = GetBoard(assigner, boardName);
            if (b == null)
            {
                logger.Warn(assigner + " attempted to access a board that doesn't exist");
                throw new Exception("The user " + assigner + " doesn't have a board named: '" + boardName + "'");
            }
            b.AssignTask(assigner, columnOrdinal, taskID, assignee);
        }

        /// <summary>
        /// load all the board related data from the database to the RAM
        /// </summary>
        /// <exception cref="Exception">throws exception if there was an error loading the data</exception>
        public void LoadData()
        {
            HashSet<BoardDTO> dtos = boardMapper.LoadData();
            Dictionary<int, HashSet<Column>> columns = columnMapper.LoadData();
            foreach (BoardDTO dto in dtos)
            {
                if (!boards.ContainsKey(dto.Owner))
                {
                    boards.Add(dto.Owner, new HashSet<Board>());
                }
                Board board = new Board(dto);
                if (!columns.ContainsKey(dto.Id))
                {
                    throw new Exception("Error loading columns for board with id: " + board.Id);
                }
                board.FillColumns(columns[dto.Id]);
                boards[dto.Owner].Add(board);
                
            }

        }
        /// <summary>
        /// adds a user to the member set of the board
        /// </summary>
        /// <param name="email">emial of the user joining the board</param>
        /// <param name="boardID">id of the board to join</param>
        /// <param name="uc">added as a parameter so the user can obtain the board</param>
        /// <exception cref="Exception">throws exception if the board doesn't exist</exception>
        public void JoinBoard(string email, int boardID, UserController uc)
        {
            Board b = GetBoard(boardID);
            if (b == null)
            {
                logger.Warn(email + " attempted to join a board that doesn't exist");
                throw new Exception("A board with id: " + boardID + " does not exist");
            }
            if (uc.JoinBoard(email, b))
            {
                b.AddUser(email);
                boards[email].Add(b);
            }
            else
            {
                throw new Exception("User already has a board with that name");
            }
        }

        /// <summary>
        /// leave a board that the user is a member of 
        /// </summary>
        /// <param name="email">email of the user making the request</param>
        /// <param name="boardID">id of the board to leave</param>
        /// <exception cref="Exception">throws exception if the board doesn't exist</exception>
        public void LeaveBoard(string email, int boardID)
        {
            Board b = GetBoard(boardID);
            if (b == null)
            {
                logger.Warn(email + " attempted to leave a board that doesn't exist");
                throw new Exception("The board '" + boardID + "' does not exist");
            }
            b.RemoveUser(email);
            boards[email].Remove(b);
        }

        /// <summary>
        /// transfer ownership from the board owner to a different user
        /// </summary>
        /// <param name="currentOwnerEmail">email of the current owner</param>
        /// <param name="newOwnerEmail">email of the new owner</param>
        /// <param name="boardName">name of the board</param>
        /// <exception cref="Exception">throws exception if the board doesn't exist</exception>
        public void TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            Board b = GetBoard(currentOwnerEmail, boardName);
            if (b == null)
            {
                logger.Warn(currentOwnerEmail + " attempted to change thr owner of a board that doesn't exist");
                throw new Exception("The board '" + boardName + "' does not exist");
            }
            b.ChangeOwner(currentOwnerEmail, newOwnerEmail);
        }

        /// <summary>
        /// update the title of the specified task
        /// </summary>
        /// <param name="email">email fo the user making the update</param>
        /// <param name="boardName">name of the board that owns the task</param>
        /// <param name="columnOrdinal">column ordinal. 0 = backlog, 1 = in progress, 2 = done</param>
        /// <param name="taskId">id of the task that will be edited</param>
        /// <param name="newTitle">new title of the task</param>
        /// <exception cref="Exception">throws exception if the board or task don't exist</exception>
        public void UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string newTitle)
        {
            if (GetTaskInColumn(email, boardName, columnOrdinal, taskId) == null)
            {
                throw new Exception("The specified task does not exist.");
            }
            Board board = GetBoard(email, boardName);
            if (board == null)
            {
                throw new Exception("The specified board does not exist.");
            }
            board.UpdateTaskTitle(email, columnOrdinal, taskId, newTitle);
        }

        /// <summary>
        /// update the description of the specified task
        /// </summary>
        /// <param name="email">email fo the user making the update</param>
        /// <param name="boardName">name of the board that owns the task</param>
        /// <param name="columnOrdinal">column ordinal. 0 = backlog, 1 = in progress, 2 = done</param>
        /// <param name="taskId">id of the task that will be edited</param>
        /// <param name="newDesc">new description of the task</param>
        /// <exception cref="Exception">throws exception if the board or task don't exist</exception>
        public void UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string newDesc)
        {
            if (GetTaskInColumn(email, boardName, columnOrdinal, taskId) == null)
            {
                throw new Exception("The specified task does not exist.");
            }
            Board board = GetBoard(email, boardName);
            if (board == null)
            {
                throw new Exception("The specified board does not exist.");
            }
            board.UpdateTaskDescription(email, columnOrdinal, taskId, newDesc);
        }


        /// <summary>
        /// update the due date of the specified task
        /// </summary>
        /// <param name="email">email fo the user making the update</param>
        /// <param name="boardName">name of the board that owns the task</param>
        /// <param name="columnOrdinal">column ordinal. 0 = backlog, 1 = in progress, 2 = done</param>
        /// <param name="taskId">id of the task that will be edited</param>
        /// <param name="newDueDate">new description of the task</param>
        /// <exception cref="Exception">throws exception if the board or task don't exist</exception>
        public void UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime newDueDate)
        {
            if (GetTaskInColumn(email, boardName, columnOrdinal, taskId) == null)
            {
                throw new Exception("The specified task does not exist.");
            }
            Board board = GetBoard(email, boardName);
            if (board == null)
            {
                throw new Exception("The specified board does not exist.");
            }
            board.UpdateTaskDueDate(email, columnOrdinal, taskId, newDueDate);
        }
    }
}

