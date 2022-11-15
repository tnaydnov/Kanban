using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    // this kind of dto actaully plays the role of both dao and dto.
    // it holds both the access to the db and the info about the object, and each dto is owned by a bo
    public class BoardDTO
    {
        private int id;
        public int Id { get => id; }
        private string name;
        public string Name { get => name; }
        private int nextTaskID;
        public int NextTaskID { get => nextTaskID; }
        private string owner;
        public string Owner { get => owner; }
        private HashSet<string> users;
        public HashSet<string> Users { get => users; }

        public BoardDTO(int id, string name, string owner, int nextTaskID, HashSet<string> users)
        {
            this.id = id;
            this.name = name;
            this.owner = owner;
            this.nextTaskID = nextTaskID;
            this.users = users;
        }


        public BoardDTO(int id, string name, string owner, int nextTaskID, ColumnDTO backlog, ColumnDTO inProgress, ColumnDTO done, HashSet<string> users)
        {
            this.id = id;
            this.name = name;
            this.owner = owner;
            this.nextTaskID = nextTaskID;

/*            this.backlog = backlog;
            this.inProgress = inProgress;
            this.done = done;

            this.users = users;*/
        }
        /// <summary>
        /// a recurring pattern that was extracted to a function
        /// </summary>
        /// <param name="query"></param>
        /// <param name="badMsg"></param>
        /// <exception cref="Exception"></exception>
        private void GeneralNonQuery(string query, string badMsg)
        {
            if (!DBConnector.GetInstance().ExecuteNonQuery(query))
            {
                throw new Exception(badMsg);
            }
        }

        
        public void AddBoard(string email, int id, string name)
        {
            string query = $"INSERT INTO Boards(id, name, nextTaskID, owner) VALUES({id},'{name}',{0},'{email}')";
            GeneralNonQuery(query, "A board with this id already exists");
        }

        public void ChangeOwner(string newOwner)
        {
            string boardsUpdate = $"UPDATE Boards SET owner = '{newOwner}' WHERE id = {id}";
            GeneralNonQuery(boardsUpdate, "Something went wrong");
            UsersBoardsDTO ub = new UsersBoardsDTO();
            ub.AddUserToBoard(owner, id);
            ub.RemoveUserFromBoard(newOwner, id);
            owner = newOwner;
        }

        public void RemoveBoard()
        {
            string query = $"PRAGMA foreign_keys = 1; DELETE FROM Boards WHERE id = {id}";
            GeneralNonQuery(query, "Something went wrong");

        }

        public void AddUser(string email)
        {
            new UsersBoardsDTO().AddUserToBoard(email, id);
        }

        public void RemoveUser(string email)
        {
            new UsersBoardsDTO().RemoveUserFromBoard(email, id);
        }

        public void AdvanceTask(ColumnDTO currentColDTO, ColumnDTO nextColDTO, TaskDTO taskDTO)
        {
            //nextColDTO.AddTask(taskDTO);
            //currentColDTO.RemoveTask(taskDTO.Id);
            taskDTO.AdvanceTask();
        }

        public void nextTaskIdPlusPlus()
        {
            nextTaskID++;
            string query = $"UPDATE Boards SET nextTaskID = {nextTaskID} WHERE id = {id}";
            GeneralNonQuery(query, "Something went wrong");
        }
    }
}