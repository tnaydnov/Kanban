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
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Window
    {
        MainViewModel viewModel;
        public RegisterView()
        {
            this.viewModel = new MainViewModel();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            UserModel user = viewModel.Register(RegisterEmail.Text, RegisterPassword.Text);
            if (user != null)
            {
                UserBoardsView userBoardsView = new UserBoardsView(user);
                userBoardsView.Show();
                this.Close();
            }
        }

        private void BackToLogin(object sender, RoutedEventArgs e)
        {
            MainWindow view = new MainWindow();
            view.Show();
            this.Close();
        }
    }
}
