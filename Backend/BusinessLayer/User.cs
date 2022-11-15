using IntroSE.Kanban.Backend.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class User
    {
        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get => isLoggedIn;
            set => isLoggedIn = value;
        }

        private string email;
        public string Email
        {
            get => email;
            set => email = value;
        }

        private string password;
        public string Password {
            get => password;
            set => password = value;
        }

        private HashSet<Board> myBoards;
        public HashSet<Board> MyBoards
        {
            get => myBoards;
            set => myBoards = value;
        }

        // MyBoards is a set with string key
        private HashSet<Board> commonBoards;
        public HashSet<Board> CommonBoards
        {
            get => commonBoards;
            set => commonBoards = value;
        }
        // CommonBoards is a set with int key
        private UserDTO dto;

        log4net.ILog logger = Utility.Logger.GetLogger();

        public User(string email, string password)
        {
            this.email = email;
            this.password = password;
            MyBoards = new HashSet<Board>();
            CommonBoards = new HashSet<Board>();
            this.isLoggedIn = false;
            dto = new UserDTO(email, password);
        }

        /// <summary>
        /// set for the email field 
        /// </summary>
        /// <param name="email"> the new email to set </param>
        public void setEmail(string email)
        {
            logger.Info($"User {this.email} changed the email to {email}");
            this.email = email;
        }

        //switch mode of the field
        public void logIn()
        {
            if (IsLoggedIn)
            {
                logger.Warn("User can't login because already logged in");
                throw new Exception("User is already logged in");
            }
            this.isLoggedIn = true;
        }

        //switch mode of the field
        public void logOut()
        {
            
            this.isLoggedIn = false;
        }

        /// <summary>
        /// set new password 
        /// </summary>
        /// <param name="password"> new password to be set</param>
        public void setPassword(string password)
        {
            logger.Info($"User {this.email} changed the password");
            this.password = password;
        }

        /// <summary>
        /// Registers a user to the system
        /// </summary>
        /// <param name="email"> email to be registered </param>
        /// <param name="password"> password of the user </param>
        public void RegisterUser(string email, string password)
        {
            dto.RegisterUser(email, password);
        }

        /// <summary>
        /// Add board to a users board list- the list of boards that the user is the owner of them
        /// </summary>
        /// <param name="email"> Email of a user </param>
        /// <param name="board"> Board the the user will be the owner </param>
        /// <returns> Bool- if the procedure succeed or not </returns>

        public bool AddBoard(Board b)
        {

            foreach (Board board in CommonBoards)
            {
                if (b.Name == board.Name)
                {
                    logger.Warn($"User {this.email} can not join to {b.Name} ");
                    return false;
                }
            }
            foreach (Board board in MyBoards)
            {
                if (b.Name == board.Name)
                {
                    logger.Warn($"User {this.email} can not join to {b.Name} ");
                    return false;
                }
            }

            logger.Info($"User {this.email} joined to '{b.Name}'");
            MyBoards.Add(b);
            return true;

        }

        /// <summary>
        /// remove board- by name from the user's MyBoards list
        /// </summary>
        /// <param name="email"> Email of the user </param>
        /// <param name="name"> Name the candidate board to be remove</param>
        /// <returns> Bool statment of the procedure </returns>
        public bool RemoveBoard(string name)
        {
            foreach(Board board in MyBoards)
            {
                if(board.Name == name)
                {
                    MyBoards.Remove(board);
                    logger.Info($"User {this.email} is not the owner of '{board.Name}'");
                    return true;
                }
            }
            logger.Warn($"User {this.email} can't remove board called '{name}', this board is not exist");
            return false;
        }

        /// <summary>
        /// leave board that the user is taking apart, not the owner of them
        /// </summary>
        /// <param name="email"> Email of the user </param>
        /// <param name="boardID"> ID of the board that the user should leave </param>
        public void LeaveBoard(int boardID)
        {
            foreach(Board board in CommonBoards)
            {
                if(board.Id == boardID)
                {
                    CommonBoards.Remove(board);
                    logger.Info($"User {this.email} is not taking apart of board called: '{board.Name}'");
                }
            }
            logger.Warn($"User {this.email} can't leave board with id '{boardID}'");
        }

        /// <summary>
        /// get boards of a user by the email
        /// </summary>
        /// <param name="email"> Email of the user </param>
        /// <returns> return list of boards- by their id </returns>
        public Response GetUserBoards()
        {
            List<string> boards = new List<string>();
            foreach(Board board in MyBoards)
            {
                boards.Add(board.Name);
            }
            foreach (Board board in CommonBoards)
            {
                boards.Add(board.Name);
            }
            Response r = new Response(boards);
            return r;
        }
        
        /// <summary>
        /// renounce ownership of board, move the board from the owner list boards to the common list boards
        /// </summary>
        /// <param name="boardName"> name of the candidate board</param>
        /// <returns> Board </returns>
        public Board renounceOwnership(string boardName) {
            foreach(Board board in MyBoards)
            {
                if(board.Name == boardName)
                {
                    MyBoards.Remove(board);
                    CommonBoards.Add(board);
                    logger.Info($"User {this.email} gave up the ownership of the board '{board.Name}'");
                    return board;
                }
            }
            return null;
        }

        public List<Task> InProgressTasks()
        {
            List<Task> res = new List<Task>();
            foreach (Board b in MyBoards)
            {
                res.AddRange(b.GetAllAssignedTasks(email));
            }
            foreach (Board b in CommonBoards)
            {
                res.AddRange(b.GetAllAssignedTasks(email));
            }
            return res;
        }

        /// <summary>
        /// user take ownership of a board
        /// </summary>
        /// <param name="board"> board that sent from renounceOwnership </param>
        /// <param name="currentUser"> New owner of the board</param>
        public void takeOwnership(Board board, string currentUser)  {
            board.ChangeOwner(currentUser, this.email);
            if (!MyBoards.Contains(board))
            {
                MyBoards.Add(board);
                logger.Info($"User {currentUser} took the ownership of the board '{board.Name}'");
            }

        }

        /// <summary>
        /// check if the user can take the ownership
        /// </summary>
        /// <param name="boardName"> name of the candidate board </param>
        /// <returns> Bool statment of the procedure </returns>
        public bool CheckIfCanAddBoard(string boardName)
        {
            foreach(Board board in MyBoards)
            {
                if(board.Name == boardName)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// user will take apart in a board, he will not the owner 
        /// </summary>
        /// <param name="b"> Board that the user will take apart in</param>
        /// <returns> Bool statement of the procedure </returns>
        public bool JoinBoard(Board b)
        {
            foreach(Board board in CommonBoards)
            {
                if(b.Name == board.Name && !b.IsInBoard(email))
                {
                    logger.Warn($"User {this.email} can not join to {b.Name} ");
                    return false;
                }
            }

            foreach (Board board in MyBoards)
            {
                if (b.Name == board.Name && !b.IsInBoard(email))
                {
                    logger.Warn($"User {this.email} can not join to {b.Name} ");
                    return false;
                }
            }
           
            logger.Info($"User {this.email} joined to '{b.Name}'");
            CommonBoards.Add(b);
            return true;
            
        }
    }
} 