using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public class ColumnModel
    {
        private ObservableCollection<TaskModel> tasks;
        public ObservableCollection<TaskModel> Tasks { get => tasks; }

        public ColumnModel(List<TaskModel> tasks)
        {
            this.tasks = new ObservableCollection<TaskModel>(tasks);
        }
    }
}
