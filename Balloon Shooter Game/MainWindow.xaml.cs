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
using System.Windows.Threading;
using System.Runtime.CompilerServices;


namespace Balloon_Shooter_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool moveLeft, moveRight;
        List<Rectangle> itemsToRemove = new List<Rectangle>();
        Random random = new Random();

        int balloonSpriteCounter;
        int balloonCounter = 100;
        int playerSpeed = 10;
        int limit = 50;
        int score = 0;
        int damage = 0;

        Rect playerHitBox;

        public MainWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += gameEngine;
            gameTimer.Start();
            MyCanvas.Focus();
            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri(@"C:\Users\user\source\repos\Balloon Shooter Game\Balloon Shooter Game\images\dark.jpg"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0, 0, 0.15, 0.15);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            MyCanvas.Background = bg;



            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri(@"C:\Users\user\source\repos\Balloon Shooter Game\Balloon Shooter Game\images\player.png.png"));
            player.Fill = playerImage;
            
        }


        private void onKeyDown(object sender, KeyEventArgs e)
        {


            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight = true;
            }

        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }



            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };


                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                MyCanvas.Children.Add(newBullet);
            }

        }

        private void makeBalloons()
        {
            ImageBrush balloonSprite = new ImageBrush();

            balloonSpriteCounter = random.Next(1, 6);

            switch (balloonSpriteCounter)
            {
                case 1:
                    balloonSprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\user\source\repos\Balloon Shooter Game\Balloon Shooter Game\images\balloon1.png"));
                    break;
                case 2:
                    balloonSprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\user\source\repos\Balloon Shooter Game\Balloon Shooter Game\images\balloon2.png"));
                    break;
                case 3:
                    balloonSprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\user\source\repos\Balloon Shooter Game\Balloon Shooter Game\images\balloon3.png"));
                    break;
                case 4:
                    balloonSprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\user\source\repos\Balloon Shooter Game\Balloon Shooter Game\images\balloon4.png"));
                    break;
                case 5:
                    balloonSprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\user\source\repos\Balloon Shooter Game\Balloon Shooter Game\images\balloon5.png"));
                    break;
                default:
                    balloonSprite.ImageSource = new BitmapImage(new Uri(@"C:\Users\user\source\repos\Balloon Shooter Game\Balloon Shooter Game\images\balloon5.png"));
                    break;
            }



            Rectangle newBalloon = new Rectangle
            {
                Tag = "balloon",
                Height = 50,
                Width = 56,
                Fill = balloonSprite
            };


            Canvas.SetTop(newBalloon, -100);
            Canvas.SetLeft(newBalloon, random.Next(30, 430));
            MyCanvas.Children.Add(newBalloon);


            GC.Collect();
        }



        private void gameEngine(object sender, EventArgs e)
        {
            
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            balloonCounter--;

            scoreText.Content = "Score: " + score; 
            damageText.Content = "Damage: " + damage;

            if (balloonCounter < 0)
            {
                makeBalloons(); 
                balloonCounter = limit; 
            }

            if (moveLeft && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
            }

            if (moveRight && Canvas.GetLeft(player) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }

         

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

           
                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }

                   
                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
 
                        if (y is Rectangle && (string)y.Tag == "balloon")
                        {
                            
                            Rect balloon = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(balloon))
                            {

                                itemsToRemove.Add(x); 
                                itemsToRemove.Add(y); 
                                score++; 
                            }
                        }

                    }
                }

                
                if (x is Rectangle && (string)x.Tag == "balloon")
                {
                    
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10); //pomjeri balon ka dolje

                    Rect balloon = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetTop(x) + 150 > 700)//ako je balon došao do dna
                    {
                        itemsToRemove.Add(x);
                        damage += 10; 
                    }

                    
                    if (playerHitBox.IntersectsWith(balloon)) //ako se sudare
                    {
                        damage += 5; 
                        itemsToRemove.Add(x); 
                    }
                }


            }


            if (score > 5)
            {
                limit = 20; 
            }

          
            if (damage > 99)
            {
                gameTimer.Stop(); 
                damageText.Content = "Damage: 100"; 
                damageText.Foreground = Brushes.Red; 
                var result = MessageBox.Show("Well Done!" + Environment.NewLine + "Score: " + score  + Environment.NewLine + "Do you want start new game?", "Game Finished!", MessageBoxButton.YesNo, MessageBoxImage.Question );
               
                if (result.Equals(MessageBoxResult.Yes))
                {
                    this.Hide();
                    MainWindow window = new MainWindow();
                    window.Show();
                    this.Close();
                }
                else
                {
                    Application.Current.Shutdown();
                }

            }

            
            foreach (Rectangle y in itemsToRemove)
            {
                MyCanvas.Children.Remove(y);
            }


        }
    }

}
