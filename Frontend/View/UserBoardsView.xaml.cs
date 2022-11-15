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
    /// Interaction logic for UserBoardsView.xaml
    /// </summary>
    public partial class UserBoardsView : Window
    {
        UserBoardsViewModel ubvm;
        public UserBoardsView(UserModel user)
        {
            this.ubvm = new UserBoardsViewModel(user);
            DataContext = ubvm;
            InitializeComponent();
        }


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string boardName = UserBoards.SelectedItem.ToString();
            BoardViewModel bvm = ubvm.GetBoardViewModel(boardName);
            if (bvm != null)
            {
                BoardView boardView = new BoardView(ubvm.User ,bvm);
                boardView.Show();
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Logout_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ubvm.Logout();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
