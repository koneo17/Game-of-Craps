using CrapsGame.Command;
using CrapsGame.Model;
using CrapsGame.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CrapsGame.ViewModel
{
    public class PlayerViewModel : INotifyPropertyChanged
    {
        CrapsGameDBEntities CrapsGameDBEntities = new CrapsGameDBEntities();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyName != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        #region Constructors for VM
        public PlayerViewModel()
        {
            Initialize();
        }
        #endregion

        #region Commands
        public ICommand LogInCommand { get; set; }
        public ICommand CreateNewAccountCommand { get; set; }
        public ICommand SavePlayerCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand DeletePlayerCommand { get; set; }
        public ICommand ClearPlayerGameHistoryCommand { get; set; }
        public ICommand EditPlayerCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        #endregion

        #region Proprieties
        private List<Game> _listOfGames { get; set; }
        public List<Game> ListOfGames
        {
            get
            {
                return _listOfGames;
            }
            set
            {
                _listOfGames = value;
                NotifyPropertyChanged("ListOfGames");
            }
        }

        private Player _player { get; set; }
        public Player Player
        {
            get
            {
                return _player;
            }
            set
            {
                _player = value;

            }
        }

        private string _errorMessage { get; set; }
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        private string _userName { get; set; }
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                NotifyPropertyChanged("UserName");
            }
        }

        private string _password { get; set; }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                NotifyPropertyChanged("Password");
            }
        }

        #endregion

        #region Methods
        private void Initialize()
        {

            LogInCommand = new ViewModelCommand(LogIn, CanLogIn);
            CreateNewAccountCommand = new ViewModelCommand(CreateNewAccount, CanCreateNewAccount);
            ListOfGames = new List<Game>();
            SavePlayerCommand = new ViewModelCommand(SavePlayer, CanSavePlayer);
            CancelCommand = new ViewModelCommand(Cancel, CanCancel);
            DeletePlayerCommand = new ViewModelCommand(DeletePlayer, CanDeletePlayer);
            ClearPlayerGameHistoryCommand = new ViewModelCommand(ClearPlayerGameHistory, CanClear);
            EditPlayerCommand = new ViewModelCommand(EditPlayer, CanEditPlayer);
            LogOutCommand = new ViewModelCommand(LogOut, CanLogOut);
            LoadPlayerInfo();
        }

        private bool CanEditPlayer(object arg)
        {
            return true;
        }

        private void EditPlayer(object obj)
        {
            ShowPlayerDetailWindow();
            CloseCurrentWindow();
        }

        public void ShowPlayerDetailWindow()
        {
            PlayerDetailWindow showPlayerDetailWindow = new PlayerDetailWindow();
            showPlayerDetailWindow.Show();
        }

        public void ShowMainWindow()
        {
            MainWindow showMainWindow = new MainWindow();
            showMainWindow.Show();
        }

        public void ShowAddNewWindow()
        {
            AddNewWindow showAddNewWindow = new AddNewWindow();
            showAddNewWindow.Show();
        }

        public void ShowLogInWindow()
        {
            LogInWindow showLogInWindow = new LogInWindow();
            showLogInWindow.Show();
        }

        public void CloseCurrentWindow()
        {
            var currentWindow = Application.Current.Windows[0];
            currentWindow.Close();
        }

        private bool CanLogOut(object arg)
        {
            return true;
        }

        private void LogOut(object obj)
        {
            UserName = string.Empty;
            Password = string.Empty;
            HelperClass.CurrentPlayerID = null;
            ShowLogInWindow();
            CloseCurrentWindow();
        }

        private bool CanClear(object arg)
        {
            return true;
        }

        public void ClearPlayerGameHistory(object obj)
        {
            if (HelperClass.CurrentPlayerID != null)
            {
                var AllGames = CrapsGameDBEntities.Games?.Where(x => x.PlayerID == HelperClass.CurrentPlayerID)?.ToList();
                foreach (var g in AllGames)
                    CrapsGameDBEntities.Games.Remove(g);

                SaveChanges("");
                ListOfGames = new List<Game>();
            }
        }

        public void SaveChanges(string s)
        {
            try
            {
                CrapsGameDBEntities.SaveChanges();
                if (s != "NoMessageBox")
                    MessageBox.Show("Success!");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e);
            }
        }

        private void LoadPlayerInfo()
        {
            if (HelperClass.CurrentPlayerID != null)
            {
                Player = CrapsGameDBEntities.Players?.Where(x => x.PlayerID == HelperClass.CurrentPlayerID)?.FirstOrDefault();
                UserName = Player.UserName;
                Password = Player.Password;
                ListOfGames = CrapsGameDBEntities.Games?.Where(x => x.PlayerID == HelperClass.CurrentPlayerID)?.ToList();
            }
        }

        private bool CanDeletePlayer(object arg)
        {
            return true;
        }

        private void DeletePlayer(object obj)
        {
            if (HelperClass.CurrentPlayerID != null)
            {
                ClearPlayerGameHistory(obj);
                Player = CrapsGameDBEntities.Players?.Where(x => x.PlayerID == HelperClass.CurrentPlayerID)?.FirstOrDefault();
                CrapsGameDBEntities.Players.Remove(Player);
                SaveChanges("NoMessageBox");
                UserName = string.Empty;
                Password = string.Empty;
                HelperClass.CurrentPlayerID = null;

                LogInWindow showLogInWindow = new LogInWindow();
                showLogInWindow.Show();
                CloseCurrentWindow();
            }
        }

        private bool CanCancel(object arg)
        {
            return true;
        }

        private void Cancel(object obj)
        {
            string Win = (string)obj;
            if (Win == "BackToLogInWindow")
                ShowLogInWindow();
            else
                ShowMainWindow();

            CloseCurrentWindow();
        }

        private bool CanSavePlayer(object arg)
        {
            return true;
        }

        private void SavePlayer(object obj)
        {
            string action = (string)obj;
            ErrorMessage = string.Empty;
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                ErrorMessage = "Please, enter username and password.";
            else
            {
                if (action == "CreateNewPlayer")
                {
                    Player = CrapsGameDBEntities.Players?.Where(x => x.UserName == UserName && x.Password == Password)?.FirstOrDefault();
                    if (Player != null)
                        ErrorMessage = "This username and password already exist!";
                    else
                    {
                        Player = new Player();
                        Player.UserName = UserName;
                        Player.Password = Password;

                        CrapsGameDBEntities.Players.Add(Player);

                        SaveChanges("");
                        HelperClass.CurrentPlayerID = Player.PlayerID;
                        ShowMainWindow();
                        CloseCurrentWindow();
                    }
                }
                else if (action == "UpdatePlayer")
                {
                    if (HelperClass.CurrentPlayerID != null)
                    {
                        Player = CrapsGameDBEntities.Players?.Where(x => x.PlayerID == HelperClass.CurrentPlayerID)?.FirstOrDefault();

                        Player.UserName = UserName;
                        Player.Password = Password;
                        SaveChanges("");
                        HelperClass.CurrentPlayerID = Player.PlayerID;
                        ShowMainWindow();
                        CloseCurrentWindow();
                    }
                }
            }
        }

        private bool CanLogIn(object arg)
        {
            return true;
        }

        private bool CanCreateNewAccount(object arg)
        {
            return true;
        }

        private void LogIn(object obj)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                ErrorMessage = "Please, enter your username and password.";
            else
            {
                Player = CrapsGameDBEntities.Players?.Where(x => x.UserName == UserName && x.Password == Password)?.FirstOrDefault();
                if (Player == null)
                    ErrorMessage = "No match for this credentials";
                else
                {

                    HelperClass.CurrentPlayerID = Player.PlayerID;
                    ShowMainWindow();
                    CloseCurrentWindow();
                }
            }

        }

        private void CreateNewAccount(object obj)
        {
            ShowAddNewWindow();
            CloseCurrentWindow();
        }
        #endregion

    }
}

