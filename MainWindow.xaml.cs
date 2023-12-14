using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace remake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random;
        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            this.Focusable = true;
        }
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            random = new Random();
            StartTimeEvent();
            this.Focus();
            PlayingField.ScoreBock = ScoreBlock;
            PlayingField.TileGrid = TileGrid;
            PlayingField.UpgradeButton = UpgradeButton;

            for (int i = 0; i < PlayingField.GridSquare; i++)
            {
                TileGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                TileGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
            }
            for (int row = 0; row < PlayingField.GridSquare; row++)
            {
                for (int col = 0; col < PlayingField.GridSquare; col++)
                {
                    Tile tile = new Tile();
                    tile.X = col;
                    tile.Y = row;
                    Grid.SetRow(tile, row);
                    Grid.SetColumn(tile, col);
                    TileGrid.Children.Add(tile);
                    if (random.Next(0, 30) <= 0 && !Player.IsPlayerOnTile(tile)) tile.SetPlayerPoint();
                }
            }
            PlayingField.GetPlayerTile().MovePlayerTo(Direction.Down); //gets the default tile
            PlayingField.UpdateGameInfo();
        }
        private void OnUpgradeClick(object sender, RoutedEventArgs e)
        {
            PlayingField.UpgradeClick();
        }
        private void StartTimeEvent()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (sender, e) =>
            {
                PlayingField.AddPointOnMap();
                PlayingField.UpdateGameInfo();
            };
            timer.Start();
        }
       
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.W: 
                    Player.MovePlayer(TileGrid, Direction.Up);
                    break;
                case Key.A:
                    Player.MovePlayer(TileGrid, Direction.Left);
                    break;
                case Key.S:
                    Player.MovePlayer(TileGrid, Direction.Down);
                    break;
                case Key.D:
                    Player.MovePlayer(TileGrid, Direction.Right);
                    break;
            }
        }
    }
}
