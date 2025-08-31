using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 }
        };

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private readonly BlurEffect newBlurEffect = new BlurEffect();

        public static MediaPlayer player = new MediaPlayer();
        System.Media.SoundPlayer sound = new System.Media.SoundPlayer("Hover.wav");

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
            player.Open(new Uri("music_zapsplat_easy_cheesy.mp3", UriKind.Relative));
            player.Play();
        }

        private async Task RunGame()
        {
            newBlurEffect.Radius = 0;

            Draw();
            await ShowCountDown();

            tb_overlay.Visibility = Visibility.Hidden;
            br_gridBorder.Effect = newBlurEffect;

            await GameLoop();
            await ShowGameOver();

            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (tb_overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();

                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            ug_gameGrid.Rows = rows;
            ug_gameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    ug_gameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            tb_scoreText.Text = $"SCORE {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridValue = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridValue];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                tb_overlay.Text = i.ToString();
                await Task.Delay(1000);
            }
        }

        private async Task ShowGameOver()
        {
            await Task.Delay(100);

            newBlurEffect.Radius = 15;
            br_gridBorder.Effect = newBlurEffect;

            tb_overlay.Visibility = Visibility.Visible;
            tb_overlay.Text = "PRESS ANY KEY TO START";
        }

        private void ImagePlay_MouseEnter(object sender, MouseEventArgs e)
        {
            img_play.Source = new BitmapImage(new Uri("/assets/PlayActive.png", UriKind.Relative));
            sound.Play();
        }

        private void ImageOptions_MouseEnter(object sender, MouseEventArgs e)
        {
            img_options.Source = new BitmapImage(new Uri("/assets/OptionsActive.png", UriKind.Relative));
            sound.Play();
        }

        private void ImageExit_MouseEnter(object sender, MouseEventArgs e)
        {
            img_exit.Source = new BitmapImage(new Uri("/assets/ExitActive.png", UriKind.Relative));
            sound.Play();
        }

        private void ImagePlay_MouseLeave(object sender, MouseEventArgs e)
        {
            img_play.Source = new BitmapImage(new Uri("/assets/Play.png", UriKind.Relative));
        }

        private void ImageOptions_MouseLeave(object sender, MouseEventArgs e)
        {
            img_options.Source = new BitmapImage(new Uri("/assets/Options.png", UriKind.Relative));
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            img_snake.Visibility = Visibility.Hidden;
            sp_buttons.Visibility = Visibility.Hidden;

            tb_scoreText.Visibility = Visibility.Visible;
            br_gridBorder.Visibility = Visibility.Visible;

            br_overlay.Visibility = Visibility.Visible;
            tb_overlay.Visibility = Visibility.Visible;
        }

        private void ButtonOptions_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow optionsWindow = new OptionsWindow();
            optionsWindow.ShowDialog();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ImageExit_MouseLeave(object sender, MouseEventArgs e)
        {
            img_exit.Source = new BitmapImage(new Uri("/assets/Exit.png", UriKind.Relative));
        }
    }
}
