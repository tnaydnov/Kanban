using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.Mappers;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    public class UserController
    {
        log4net.ILog logger = Utility.Logger.GetLogger();

        //UserEmail, User//
        private Dictionary<string, User> users;

        private UserMapper usm;

        public UserController()
        {
            users = new Dictionary<string, User>();
            usm = new UserMapper();
        }

        /// <summary>
        /// check if the user is logged in or loggedout
        /// </summary>
        /// <param name="email"> Email of the user</param>
        /// <returns> Bool by the statment of the proprety </returns>
        public bool IsLoggedIn(string email)
        {
            email = email.ToLower();
            if (!users.ContainsKey(email))
            {
                return false;
            }
            User user = users[email];
            if (user.IsLoggedIn)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// create new user 
        /// </summary>
        /// <param name="email"> Email of the new user</param>
        /// <param name="password"> Password of the new user </param>
        /// <returns> string if the creation failed or not </returns>
        public void createUser(string email, string password)
        {
            if (!users.ContainsKey(email))
            {
                User user = new User(email, password);
                user.RegisterUser(email, password);
                users.Add(email, user);
                logger.Info("User: " + email + ", registered successfully");
            }
            else
            {
                logger.Warn("Failed to create user " + email + ", because a user with that name already exists.");
                throw new Exception("This email is already taken");
            }
        }

        /// <summary>
        /// get a user by his email
        /// </summary>
        /// <param name="email"></param>
        /// <returns> User Object </returns>
        public User GetUser(string email)
        {
            if (exists(email))
            {
                return users[email];
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// check if the user is sign up
        /// </summary>
        /// <param name="email"> Email of the candidate user </param>
        /// <returns>Bool statement</returns>
        public bool exists(string email)
        {
            email = email.ToLower();
            if (users.ContainsKey(email))
                return true;
            else
                return false;
        }

        /// <summary>
        /// login function for a user that signed up
        /// </summary>
        /// <param name="email"> Email of a user  </param>
        /// <param name="password"> Password of the user </param>
        /// <returns></returns>
        public void login(string email, string password)
        {
            string trueEmail = email;
            email = email.ToLower();
            if (GetUser(email) != null)
            {
                if (GetUser(email).Password == password)
                {
                    GetUser(email).logIn();
                    logger.Info("The user " + email + " logged in successfully");
                }
                else
                {
                    logger.Warn("Failed to login the user " + trueEmail + ", because there is no match between the email and password");
                    throw new Exception("Email or password is incorrect");
                }
            }
            else
            {
                logger.Warn("Failed to log in, user " + trueEmail + " is not exists");
                throw new Exception("The user " + trueEmail + " does not exist");
            }
        }

        /// <summary>
        /// logout function for a user that signed up
        /// </summary>
        /// <param name="email"> Email of a user  </param>
        /// <returns></returns>
        public void LogOut(string email)
        {
            if (GetUser(email) != null)
            {
                if (GetUser(email).IsLoggedIn == true)
                {
                    GetUser(email).logOut();
                    logger.Info("The user " + email + " logged out successfully");
                }
                else
                {
                    logger.Warn("Failed to logout the user " + email + ", because the user is not connected");
                    throw new Exception("The user logged out unsuccesssfully");
                }
            }
            else
            {
                logger.Warn("Failed to logout user " + email + ", because a user with that name is not exists");
                throw new Exception("The user " + email + " does not exist");
            }
        }

        public List<Task> InProgressTasks(string email)
        {
            return users[email].InProgressTasks();
        }

        /// <summary>
        /// change Password for a user
        /// </summary>
        /// <param name="email"> Email of the user </param>
        /// <param name="oldPassword"> Old password of the user </param>
        /// <param name="newPassword"> New password for a user </param>
        /// <returns></returns>
        public void changePassword(string email, string oldPassword, string newPassword)
        {
            if (GetUser(email) != null)
            {
                if (GetUser(email).IsLoggedIn == true)
                {
                    if (users[email].Password == oldPassword)
                    {
                        GetUser(email).setPassword(newPassword);
                        logger.Info("The password of the user " + email + " changed successfully");
                    }
                    else
                    {
                        logger.Warn("Faild to change password of the user " + email + ", because there is no match between the email and password");
                        throw new Exception("There is no match between the password and the user name");
                    }
                }
                else
                {
                    logger.Warn("The user's password can not be changed, because the user " + email + " is not connected");
                    throw new Exception("This user is not connected, password can't be changed");
                }
            }
            else
            {
                logger.Warn("Failed to change the user's password at " + email + ", because a user with that email is not exists");
                throw new Exception("The user " + email + " does not exist");
            }
        }

        public void DeleteData()
        {
            users.Clear();
            usm.DeleteData();
        }

        public void LoadData()
        {
            HashSet<UserDTO> userDTOs = usm.LoadData();
            foreach (UserDTO userDTO in userDTOs)
            {
                if (!users.ContainsKey(userDTO.Email))
                {
                    users.Add(userDTO.Email, new User(userDTO.Email, userDTO.Password));
                }

            }

            Dictionary<string, HashSet<Board>> boards = BoardController.Boards;
            foreach (string email in users.Keys)
            {
                if (boards.ContainsKey(email))
                {
                    foreach (Board board in boards[email])
                    {
                        if (board.Owner == email)
                        {
                            GetUser(email).MyBoards.Add(board);
                        }
                        else
                        {
                            GetUser(email).CommonBoards.Add(board);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// remove board- by name from the user's MyBoards list
        /// </summary>
        /// <param name="email"> Email of the user </param>
        /// <param name="name"> Name the candidate board to be remove</param>
        /// <returns> Bool statment of the procedure </returns>
        public bool RemoveBoard(string email, string name)
        {
            User user = users[email];
            return user.RemoveBoard(name);
        }


        /// <summary>
        /// Add board to a users board list- the list of boards that the user is the owner of them
        /// </summary>
        /// <param name="email"> Email of a user </param>
        /// <param name="board"> Board the the user will be the owner </param>
        /// <returns> Bool- if the procedure succeed or not </returns>
        public bool AddBoard(string email, Board board)
        {
            User user = users[email];
            return user.AddBoard(board);
        }

        /// <summary>
        /// leave board that the user is taking apart, not the owner of them
        /// </summary>
        /// <param name="email"> Email of the user </param>
        /// <param name="boardID"> ID of the board that the user should leave </param>
        public void LeaveBoard(string email, int boardID)
        {
            User user = users[email];
            user.LeaveBoard(boardID);
        }

        /// <summary>
        /// get boards of a user by the email
        /// </summary>
        /// <param name="email"> Email of the user </param>
        /// <returns> return list of boards- by their id </returns>
        public Response GetUserBoards(string email)
        {
            User user = users[email];
             return user.GetUserBoards();
        }

        /// <summary>
        /// Transfer the ownership of a board to another user
        /// </summary>
        /// <param name="currentOwnerEmail"> Email of the owner user</param>
        /// <param name="newOwnerEmail"> Email of the new owner- user </param>
        /// <param name="boardName"> name of the board we want to trasfer the ownership</param>
        /// <returns> string- the result of the transfer</returns>

        public void TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            if (GetUser(newOwnerEmail) != null) // no need to check currentOwnerEmail, since he's already logged in so cannot be null
            {
                User currentOwner = users[currentOwnerEmail];
                User newOwner = users[newOwnerEmail];
                if (newOwner.CheckIfCanAddBoard(boardName) || currentOwnerEmail == newOwnerEmail)
                {
                    Board board = currentOwner.renounceOwnership(boardName);
                    if (board != null)
                    {
                        newOwner.takeOwnership(board, currentOwnerEmail);
                    }
                    else
                    {
                        throw new Exception($"{currentOwnerEmail} has no board called '{boardName}' ");
                    }
                }
                
                else
                {
                    throw new Exception($"{newOwnerEmail} already has a board called {boardName}");
                }
            }
            else
            {
                throw new Exception($"{newOwnerEmail} is not registered, can not transfer the ownership of '{boardName}");
            }
            
        }
        public bool JoinBoard(string email, Board b)
        {
            User user = users[email];
            return user.JoinBoard(b);
        }
    }
}