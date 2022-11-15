using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Frontend.ViewModel;

namespace Frontend.Model
{
    public class UserModel
    {
        private string email;
        public string Email { get => email; }

        private ObservableCollection<BoardModel> boards;
        public ObservableCollection<BoardModel> Boards { get => boards; }
        public UserModel(string email)
        {
            this.email = email;
            boards = new ObservableCollection<BoardModel>();
            List<string> boardNames = BackendController.Instance.GetUserBoards(email);
            foreach (string boardName in boardNames)
            {
                boards.Add(new BoardModel(email, boardName));
            }
        }
    }
}
