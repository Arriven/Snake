using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game1Try
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private List<Obstacle> Environment = new List<Obstacle>();
        private Circle food = new Circle();
        public Form1()
        {
            InitializeComponent();

            new Settings();

            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();
           
            StartGame();   
        }

        private void StartGame()
        {
            Random r = new Random();
            new Settings();
            labelGameOver.Visible = false;

            Environment.Clear();
            for (int i = 0; i <= 5; i++)
            {
                Obstacle obs = new Obstacle();
                if (i == 0)
                {
                    obs.X = r.Next(1, pbCanvas.Size.Width / Settings.Width);
                    obs.Y = r.Next(1, pbCanvas.Size.Height / Settings.Height);
                }
                else
                {
                    obs.X = Environment.Last().X;
                    obs.Y = Environment.Last().Y+1;
                }
                Environment.Add(obs);
            }
            for (int i = 0; i <= 5; i++)
            {
                Obstacle obs = new Obstacle();
                if (i == 0)
                {
                    obs.X = r.Next(1, pbCanvas.Size.Width / Settings.Width);
                    obs.Y = r.Next(1, pbCanvas.Size.Height / Settings.Height);
                }
                else
                {
                    obs.X = Environment.Last().X + 1;
                    obs.Y = Environment.Last().Y;
                }
                Environment.Add(obs);
            }
            for (int i = 0; i <= 5; i++)
            {
                Obstacle obs = new Obstacle();
                if (i == 0)
                {
                    obs.X = r.Next(1, pbCanvas.Size.Width / Settings.Width);
                    obs.Y = r.Next(1, pbCanvas.Size.Height / Settings.Height);
                }
                else
                {
                    obs.X = Environment.Last().X;
                    obs.Y = Environment.Last().Y + 1;
                }
                Environment.Add(obs);
            }


            Snake.Clear();
            Circle head = new Circle();
            head.X = 10;
            head.Y = 5;
            Snake.Add(head);

            labelScore.Text = Settings.Score.ToString();
            GenerateFood();
        }
        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

            bool k = true;
            do
            {
                k = false;
                Random random = new Random();
                food = new Circle();
                food.X = random.Next(0, maxXPos);
                food.Y = random.Next(0, maxYPos);
                for (int i = 0; i < Environment.Count; i++)
                {
                    if (food.X >= Environment[i].X &&
                        food.Y >= Environment[i].Y)
                    { k = true; }
                }
            } while (k);
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            if (Settings.GameOver)
            {
                if (Input.keyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.keyPressed(Keys.Right) && Settings.direction != Direction.Left)
                {
                    Settings.direction = Direction.Right;
                }
                else if (Input.keyPressed(Keys.Left) && Settings.direction != Direction.Right)
                {
                    Settings.direction = Direction.Left;
                }
                else if(Input.keyPressed(Keys.Up) && Settings.direction != Direction.Down)
                {
                    Settings.direction = Direction.Up;
                }
                else if (Input.keyPressed(Keys.Down) && Settings.direction != Direction.Up)
                {
                    Settings.direction = Direction.Down;
                }
                MovePlayer();
            }

            pbCanvas.Invalidate();
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                Brush snakeColour;

                for(int i = 0; i < Environment.Count; i++)
                {
                    canvas.FillRectangle(Brushes.Gray, new Rectangle(Environment[i].X * Settings.Width,
                                                                     Environment[i].Y * Settings.Height,
                                                                     Settings.Width,
                                                                     Settings.Height));
                }

                canvas.FillEllipse(Brushes.Red, new Rectangle(food.X * Settings.Width,
                                                             food.Y * Settings.Height,
                                                             Settings.Width, Settings.Height));


                for (int i = 0; i< Snake.Count; i++)
                {
                    if (i == 0) { snakeColour = Brushes.Black; }
                    else { snakeColour = Brushes.Green; }

                    canvas.FillEllipse(snakeColour, new Rectangle(Snake[i].X * Settings.Width, 
                                                                  Snake[i].Y * Settings.Height, 
                                                                  Settings.Width, Settings.Height));
                }
            }
            else
            {
                string gameOver = "Game Over \nYour final score is:" + Settings.Score + "\nPress Enter to try again";
                labelGameOver.Text = gameOver;
                labelGameOver.Visible = true;
            }
        }

        private void MovePlayer()
        {
            for(int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height / Settings.Height;
                    if(Snake[0].X<0 || Snake[0].Y<0 ||Snake[0].X>=maxXPos||Snake[0].Y>= maxYPos)
                    {
                        if (Snake[0].X < 0) { Snake[0].X += maxXPos; }
                        else if (Snake[0].Y < 0) { Snake[0].Y += maxYPos; }
                        else if (Snake[0].X >=maxXPos) { Snake[0].X -= maxXPos; }
                        else if (Snake[0].Y >=maxYPos) { Snake[0].Y -= maxYPos; }
                    }
                    for(int j = 1;j<Snake.Count; j++)
                    {
                        if(Snake[i].X==Snake[j].X&& Snake[i].Y == Snake[j].Y)
                        {
                            Die();
                        }
                    }
                    for(int j = 0; j < Environment.Count; j ++)
                    {
                        {
                            if (Snake[i].X == Environment[j].X&&
                                Snake[i].Y == Environment[j].Y)
                            { Die(); }
                        }

                    }
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }
                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }
        private void Eat()
        {
            Circle food = new Circle();
            food.X = Snake[Snake.Count - 1].X;
            food.Y = Snake[Snake.Count - 1].Y;

            Snake.Add(food);
            Settings.Score += Settings.Points;
            labelScore.Text = Settings.Score.ToString();

            GenerateFood();
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
