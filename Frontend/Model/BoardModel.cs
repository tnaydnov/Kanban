using Frontend.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{

    public class BoardModel
    {
        
        private string boardName;
        public string BoardName { get => boardName; }
        private ColumnModel backlog;
        public ColumnModel Backlog { get => backlog; }

        private ColumnModel inProgress;
        public ColumnModel InProgress { get => inProgress; }
        private ColumnModel done;
        public ColumnModel Done { get => done; }
        public BoardModel(string email, string boardName)
        {
            this.boardName = boardName;
            backlog = BackendController.Instance.GetColumn(email, boardName, 0);
            inProgress = BackendController.Instance.GetColumn(email, boardName, 1);
            done = BackendController.Instance.GetColumn(email, boardName, 2);
        }


        public override string ToString()
        {
            return boardName;
        }

    }
}
