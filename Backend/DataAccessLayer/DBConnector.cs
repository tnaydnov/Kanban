using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class DBConnector
    {
        private SQLiteConnection conn;
        private static DBConnector instance = new DBConnector(); 
        //relative path 
        string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));

        private DBConnector()
        {
            conn = new SQLiteConnection($"Data Source= {path}; Version = 3; New = True; Compress = True; ");
            try
            {
                conn.Open();
                CreateTables();
            }
            catch (Exception ex) { }

            finally
            {
                conn.Close();
            }
        }

        public static DBConnector GetInstance()
        {
            return instance;
        }

        public void close()
        {
            conn.Close();
        }

        /// <summary>
        /// used for testing purposes
        /// </summary>
        public void ResetDB()
        {
            try
            {
                conn.Open();
            }
            catch (Exception)
            {

            }
            
            SQLiteCommand cmd = conn.CreateCommand();
            string query = "DELETE FROM Users";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            query = "DELETE FROM UsersBoards";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            query = "DELETE FROM Boards";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            query = "DELETE FROM Columns";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            query = "DELETE FROM Tasks";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            conn.Close();

        }

        private void CreateTables()
        {
            SQLiteCommand cmd = conn.CreateCommand();
            string query = "CREATE TABLE IF NOT EXISTS Users(" +
               "email VARCHAR(200)," +
               "password VARCHAR(200)," +
               "PRIMARY KEY (email)" +
               ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            query = "CREATE TABLE IF NOT EXISTS UsersBoards(" +
                "userEmail VARCHAR(200)," +
                "boardID INTEGER," +
                "PRIMARY KEY (userEmail,boardID)," +
                "FOREIGN KEY (userEmail) REFERENCES Users(email) ON DELETE CASCADE," +
                "FOREIGN KEY (boardID) REFERENCES Boards(id) ON DELETE CASCADE" +
                ")";

            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            query = "CREATE TABLE IF NOT EXISTS Boards(" +
                "id INTEGER," +
                "name VARCHAR(200)," +
                "nextTaskID INTEGER," +
                "owner VARCHAR(200)," +
                "PRIMARY KEY (id)" +
                ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            query = "CREATE TABLE IF NOT EXISTS Columns(" +
                "boardID INTEGER," +
                "columnOrdinal INTEGER," +
                "maxTasks INTEGER," +
                "PRIMARY KEY (boardID, columnOrdinal)," +
                "FOREIGN KEY (boardID) REFERENCES Boards(id) ON DELETE CASCADE" +
                ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            query = "CREATE TABLE IF NOT EXISTS Tasks(" +
                "boardID INTEGER," +
                "columnOrdinal INTEGER," +
                "id INTEGER," +
                "title STRING," +
                "description STRING," +
                "dueDate DATETIME," +
                "assignee STRING," +
                "PRIMARY KEY (boardID,id)," +
                "FOREIGN KEY (boardID) REFERENCES Boards(id)" +
                ")";
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }


        public SQLiteDataReader ExecuteQuery(string nq)
        {
            try
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = nq;
                SQLiteDataReader res = cmd.ExecuteReader();
                return res;
            }
            catch (Exception e)
            {
                try
                {
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = nq;
                    SQLiteDataReader res = cmd.ExecuteReader();
                    return res;
                }
                catch(Exception e1)
                {
                    return null;
                }

                return null;
            }
            
        }

        public bool ExecuteNonQuery(string nq){

            try
            {
                //Console.WriteLine(nq);
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = nq;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                try {
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = nq;
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception e2) { 
                    return false; 
                }
            }
/*            finally
            {
                conn.Close();
            }*/
        }


    }
}