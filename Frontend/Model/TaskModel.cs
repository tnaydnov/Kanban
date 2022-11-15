using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public class TaskModel
    {
        private int id;
        public int Id { get => id; }
        private DateTime creationTime;
        public DateTime CreationTime { get => creationTime; set => creationTime = value; }

        private string title;
        public string Title { get => title; set => title = value; }
        private string description;
        public string Description { get => description; set => description = value; }
        private DateTime dueDate;
        public DateTime DueDate { get => dueDate; set => dueDate = value;}

        [Newtonsoft.Json.JsonConstructor]
        public TaskModel(int id, DateTime creationTime, string title, string desc, DateTime dueDate)
        {
            this.id = id;
            this.creationTime = creationTime;
            this.title = title;
            this.description = desc;
            this.dueDate = dueDate;
        }


        public override string ToString()
        {
            return $"Title: {title}\nDescription: {description}\nDue Date: {dueDate}\n";
        }


    }
}
