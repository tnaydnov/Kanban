using Frontend.Model;
using Frontend.View;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public MainViewModel ViewModel { get => viewModel; set => viewModel = value; }
        public MainWindow()
        {
            this.DataContext = new MainViewModel();
            this.viewModel = (MainViewModel) DataContext;
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterView registerView = new RegisterView();
            registerView.Show();
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserModel user = viewModel.Login(LoginEmail.Text, LoginPassword.Password);
            if (user != null)
            {
                UserBoardsView userBoardsView = new UserBoardsView(user);
                userBoardsView.Show();
                this.Close();
            }
        }

    }
}
