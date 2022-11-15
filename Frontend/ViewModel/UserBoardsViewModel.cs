using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    internal class UserBoardsViewModel
    {
        private UserModel user;
        public UserModel User { get => user; }
        private ObservableCollection<BoardModel> boards;
        public ObservableCollection<BoardModel> Boards { get => boards; }

        public UserBoardsViewModel(UserModel user)
        {
            this.user = user;
            this.boards = user.Boards;
        }
        internal BoardViewModel GetBoardViewModel(string boardName)
        {
            foreach (BoardModel board in boards)
            {
                if (board.BoardName == boardName)
                {
                    return new BoardViewModel(board);
                }
            }
            return null;
        }

        internal void Logout()
        {
            BackendController.Instance.Logout(user.Email);
        }
    }
}
