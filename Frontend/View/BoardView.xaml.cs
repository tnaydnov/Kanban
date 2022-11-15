using Frontend.Model;
using Frontend.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : Window
    {
        private BoardViewModel bvm;
        public BoardViewModel Bvm { get => bvm; }

        private UserModel user;

        public BoardView(UserModel user, BoardViewModel bvm)
        {
            this.user = user;
            this.bvm = bvm;
            DataContext = bvm;
            InitializeComponent();
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            UserBoardsView view = new UserBoardsView(user);
            view.Show();
            this.Close();
        }
    }
}
