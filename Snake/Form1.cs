using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        public Form1()
        {
            InitializeComponent();
            //Set settings to default
            new Settings();

            //Set game speed and start timer
            GameTimer.Interval = 1000 / Settings.Speed;
            GameTimer.Tick += UpdateScreen;
            GameTimer.Start();

            //Start the game
            StartGame();
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;
            lblScore.Visible = true;
            new Settings();

            //Create new player object
            Snake.Clear();
            Circle head = new Circle();
            head.x = 10;
            head.y = 5;
            Snake.Add(head);

            lblScore.Text = Settings.Score.ToString();
            CreateFood();
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            //Check for Game Over
            if(Settings.GameOver)
            {
               //Check if enter is pressed
                if(Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;

                MovePlayer();
            }
            pbGameField.Invalidate();
        }

        private void pbGameField_Draw(object sender, PaintEventArgs e)
        {
            Graphics gf = e.Graphics;

            if(!Settings.GameOver)
            {
                                
                //Draw snake
                for (int i = Snake.Count - 1; i >= 0; i--)
                {
                    Brush snakeColor;
                    if (i == 0)
                        snakeColor = Brushes.Orchid;
                    else
                        snakeColor = Brushes.PaleGreen;

                    //Draw Snake
                    gf.FillEllipse(snakeColor,
                        new Rectangle(Snake[i].x * Settings.Width,
                                      Snake[i].y * Settings.Height,
                                      Settings.Width, Settings.Height));

                    //Draw food
                    gf.FillEllipse(Brushes.Red,
                        new Rectangle(food.x * Settings.Width,
                                      food.y * Settings.Height,
                                      Settings.Width,
                                      Settings.Height));
                }
            }
            else
            {
                string gameOver = "Game over \nYour final score is: "
                     + Settings.Score + "\nPress Enter to try again!";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
                lblScore.Visible = false;
            }
        }

        //Create food randomly
        private void CreateFood()
        {
            int maxXPos = pbGameField.Size.Width / Settings.Width;
            int maxYPos = pbGameField.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle
            {
                x = random.Next(0, maxXPos),
                y = random.Next(0, maxYPos)
            };
        }

        private void MovePlayer()
        {
            //Make snake body follow its movement pattern
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if(i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].x++;
                            break;

                        case Direction.Left:
                            Snake[i].x--;
                            break;

                        case Direction.Up:
                            Snake[i].y--;
                            break;

                        case Direction.Down:
                            Snake[i].y++;
                            break;
                    }

                    int maxXPos = pbGameField.Size.Width / Settings.Width; 
                    int maxYPos = pbGameField.Size.Height / Settings.Height;

                    //Game boundary death
                    if(Snake[i].x < 0 || Snake[i].y < 0 ||
                    Snake[i].x >= maxXPos || Snake[i].y >= maxYPos)
                    {
                        Die();
                    }

                    //Determine if snake collides with body
                    for(int j = 1; j < Snake.Count; j++)
                    {
                        if(Snake[i].x == Snake[j].x &&
                            Snake[i].y == Snake[j].y)
                        {
                            Die();
                        }
                    }
                    //Eat food
                    if(Snake[0].x == food.x && Snake[0].y == food.y)
                    {
                        Eat();
                    }
                }
                else
                {
                    //Move body
                    Snake[i].x = Snake[i - 1].x;
                    Snake[i].y = Snake[i - 1].y;
                }
            }
        }

        private void Eat()
        {
            Snake.Add(food);

            //Update score
            Settings.Score += Settings.Points;
            lblScore.Text = Settings.Score.ToString();

            CreateFood();
        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }
    }
}
