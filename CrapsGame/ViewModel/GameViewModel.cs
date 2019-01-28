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
    public class GameViewModel : INotifyPropertyChanged
    {
        CrapsGameDBEntities CrapsGameDBEntities = new CrapsGameDBEntities();
        public enum statusType { WIN, LOSE, PLAY }
        int numOfRolls = 1;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyName != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        #region Constructors for VM
        public GameViewModel()
        {
            Initialize();
        }
        #endregion

        #region Commands
        public ICommand RollDiceCommand { get; set; }

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

        private Game _game { get; set; }
        public Game Game
        {
            get
            {
                return _game;
            }
            set
            {
                _game = value;

            }
        }

        private string _message { get; set; }
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        private int? _points { get; set; }
        public int? Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
                NotifyPropertyChanged("Points");
            }
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            RollDiceCommand = new ViewModelCommand(RollDice, CanRollDice);
            ListOfGames = new List<Game>();
            LoadGameHistory();
        }

        private void LoadGameHistory()
        {
            if (HelperClass.CurrentPlayerID != null)
            {
                ListOfGames = new List<Game>();
                ListOfGames = CrapsGameDBEntities.Games?.Where(x => x.PlayerID == HelperClass.CurrentPlayerID)?.ToList();
            }
        }

        private bool CanRollDice(object arg)
        {
            return true;
        }

        private void RollDice(object obj)
        {
            if (HelperClass.CurrentPlayerID != null && Player == null)
                Player = CrapsGameDBEntities.Players?.Where(x => x.PlayerID == HelperClass.CurrentPlayerID)?.FirstOrDefault();

            if (Player != null)
            {
                Game = new Game();
                Points = Game.Points;
                Game.PlayerID = Player.PlayerID;
                Play(Game);
                Points = Game.Points;

                CrapsGameDBEntities.Games.Add(Game);

                if (Game.Status == statusType.WIN.ToString())
                    Message = "Congratulations! You rolled " + Game.SumOfDices + " You WIN!";
                else if (Game.Status == statusType.LOSE.ToString())
                    Message = "Sorry, You rolled " + Game.SumOfDices + " You Lose";
                else
                    Message = "You rolled " + Game.SumOfDices + ", keep Rolling...";

                SaveChanges("NoMessageBox");
                LoadGameHistory();
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

        public void Play(Game game)
        {
            DiceRolls rolls = new DiceRolls();

            game.SumOfDices = rolls.RollDice();
            CheckGameStatus(game);
        }

        public void CheckGameStatus(Game game)
        {
            if ((game.SumOfDices == 7 || game.SumOfDices == 11) && numOfRolls == 1)
                game.Status = statusType.WIN.ToString();
            else if ((game.SumOfDices == 2 || game.SumOfDices == 3 || game.SumOfDices == 12) && numOfRolls == 1)
                game.Status = statusType.LOSE.ToString();
            else if ((game.SumOfDices == 4 || game.SumOfDices == 5 || game.SumOfDices == 6 || game.SumOfDices == 8 ||
                game.SumOfDices == 9 || game.SumOfDices == 10) && numOfRolls == 1)
            {
                game.Points = game.SumOfDices;
                game.Status = statusType.PLAY.ToString();
                numOfRolls++;
            }
            else if (game.SumOfDices == 7 && numOfRolls > 1)
            {
                game.Points = 0;
                game.Status = statusType.LOSE.ToString();
                numOfRolls = 1;
            }
            else if (game.SumOfDices == Points && numOfRolls > 1)
            {
                game.Points = 0;
                game.Status = statusType.WIN.ToString();
                numOfRolls = 1;
            }
        }
        #endregion

    }
}
