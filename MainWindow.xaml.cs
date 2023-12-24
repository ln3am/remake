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
using Remaster;

namespace remake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random;
        public MainWindow()
        {
            InitializeComponent();
            this.Focusable = true;
        }
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            random = new Random();
            this.Focus();
            PlayingField.ScoreBock = ScoreBlock;
            PlayingField.TileGrid = TileGrid;
            PlayingField.UpgradeButton = UpgradeButton;
            PlayingField.UIDispatcher = Dispatcher;

            PlayingField.StartTimeTick();
            LocalCreateTileGrid();
            
            PlayingField.GetPlayerTile().SetShapeObject(TileShapeObject.Player, Colours.VioletGradient());

            ShapeEnemy se = new ShapeEnemy();
            PlayingField.UpdateGameInfo();

            void LocalCreateTileGrid()
            {
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
                        tile.UIDispatcher = Dispatcher;
                        Grid.SetRow(tile, row);
                        Grid.SetColumn(tile, col);
                        TileGrid.Children.Add(tile);
                        if (random.Next(0, 30) <= 0 && !tile.IsPlayerTile) tile.SetPoint(1);
                    }
                }
            }
        }
        private void OnUpgradeClick(object sender, RoutedEventArgs e)
        {
            PlayingField.UpgradeClick();
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
