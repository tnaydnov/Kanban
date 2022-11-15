using Frontend.Model;
using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    public class MainViewModel : NotifiableObject
    {
        public BackendController Controller { get; private set; }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                this.Email = value;
                RaisePropertyChanged("Email");
            }
        }
        private string password;
        public string Password
        {
            get => password;
            set
            {
                this.password = value;
                RaisePropertyChanged("Password");
            }
        }

        private string message;
        public string Message
        {
            get => message;
            set
            {
                this.message = value;
                RaisePropertyChanged("Message");
            }
        }

        public MainViewModel()
        {
            Controller = BackendController.Instance;
        }
        internal UserModel Register(string email, string password)
        {
            try
            {
                return Controller.Register(email, password);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        internal UserModel Login(string email, string password)
        {
            Message = "";
            try
            {
                return Controller.Login(email, password);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }
    }
}
