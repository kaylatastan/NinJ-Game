using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ninj.Net
{
	public partial class Form1 : Form
	{

		private Label scoreLabel;
		private byte firstlaunch = 0;
		private const int MinY = 0;
		private enum CatcherState
		{
			Idle,
			Idle1,
			Run1
		}
		private CatcherState currentCatcherState = CatcherState.Idle1;
		private Dictionary<CatcherState, string> catcherGifs;

		private int MaxY => ClientSize.Height;

		private System.Windows.Forms.Timer spawnTimer;
		private List<Fruit> fruitList;
		private Random random = new Random();
		private int score = 0;
		private int lvl = 0;

		private System.Media.SoundPlayer backgroundMusicPlayer;
		private const int InitialTimerInterval = 121;
		private const int DefaultFruitSpeed = 10;
		private const int DefaultPoint = 1;
		private const int DefaultSpecialItem = 0;
		private bool gameStarted = false;
		private bool isGameOver = false;


		private readonly int SpawnRate = 1000;
		private readonly int MaxFruits = 10;

		private PictureBox catcherPictureBox;
		private Timer catchTimer;
		private int catchTimerTickCounter;

		public Form1()
		{
			InitializeComponent();
			DoubleBuffered = true;
			InitializeFruits();
			InitializeTimers();
			InitializeCatcher();
			InitializeScoreLabel();
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;

			backgroundMusicPlayer = new System.Media.SoundPlayer("Zelda-Game-song.wav");
			backgroundMusicPlayer.PlayLooping();

			timer1.Stop();
			spawnTimer.Stop();
		}

		private void InitializeScoreLabel()
		{

			scoreLabel = new Label
			{
				Text = $"Score: {score} ",
				Location = new Point(40, 40),
				Font = new Font(Font.FontFamily, 10, FontStyle.Bold),
				ForeColor = Color.Black,
				BackColor = Color.White,
				AutoSize = true,
				Size = new System.Drawing.Size(100, 30)
			};
			//scoreLabel.Visible = false;

			Controls.Add(scoreLabel);
		}
		private void InitializeTimers()
		{
			timer1.Interval = InitialTimerInterval;
			timer1.Tick += timer1_Tick;

			spawnTimer = new System.Windows.Forms.Timer();
			spawnTimer.Tick += spawnTimer_Tick;
			SetRandomSpawnInterval();
			spawnTimer.Start();

			
			catchTimer = new Timer();
			catchTimer.Interval = 50; 
			catchTimer.Tick += catchTimer_Tick;
			catchTimer.Start();
		}

		private void InitializeCatcher()
		{
			catcherGifs = new Dictionary<CatcherState, string>
			{
				{ CatcherState.Idle, "idle.png"} ,
				{ CatcherState.Idle1, "idle.png"} ,
				{ CatcherState.Run1, "run1.png"}
			};

			catcherPictureBox = new PictureBox
			{
				Size = new Size(55, 90),
				Location = new Point(ClientSize.Width / 2 - 25, ClientSize.Height - 125),
				SizeMode = PictureBoxSizeMode.StretchImage,
				BackColor = Color.Transparent
			};

			UpdateCatcherImage();
			Controls.Add(catcherPictureBox);
		}

		private void UpdateCatcherImage()
		{
			string imagePath = catcherGifs[currentCatcherState];
			catcherPictureBox.Image = Image.FromFile(imagePath);
		}

		private void UpdateCatcherState()
		{
			UpdateCatcherImage();

			if (IsKeyDown(Keys.Left) && catcherPictureBox.Left > 0)
			{
				currentCatcherState = CatcherState.Run1;
				catcherPictureBox.Left -= 15;
			}
			else if (IsKeyDown(Keys.Right) && catcherPictureBox.Right < ClientSize.Width)
			{
				currentCatcherState = CatcherState.Run1;
				catcherPictureBox.Left += 15;
			}
			else
			{
				currentCatcherState = CatcherState.Idle;
			}

			UpdateCatcherImage();
		}


		private void spawnTimer_Tick(object sender, EventArgs e)
		{
			
			SpawnFruit();
			
			SetRandomSpawnInterval();
		}

		private void SpawnFruit()
		{

			
			Fruit fruit = GetRandomFruit();

			fruitList.Add(fruit);
			Controls.Add(fruit.PictureBox);

		}

		private Fruit GetRandomFruit()
		{
			
			List<Fruit> fruits = new List<Fruit>
	{
		new Fruit("*Apple", random.Next(0, ClientSize.Width - 50), 0, "pixil-frame-0 (5).png")
		{
			FruitSpeed = 10,
			Point = 50
		},

		new Fruit("*Carrot", random.Next(0, ClientSize.Width - 50), 0, "carrot.png")
		{
			FruitSpeed = 15,
			Point = 2
		},
		new Fruit("Chicken", random.Next(0, ClientSize.Width - 50), 0, "chicken.png")
		{
			FruitSpeed = 15,
			Point = 1
		},
		new Fruit("Davşan", random.Next(0, ClientSize.Width - 50), 0, "davşan.png")
		{
			FruitSpeed = 15,
			Point = 6
		},
		new Fruit("Heart", random.Next(0, ClientSize.Width - 50), 0, "heart.png")
		{
			FruitSpeed = 15,
			Point = 5
		},
		new Fruit("**Cow", random.Next(0, ClientSize.Width - 50), 0, "pixil-frame-0 (6).png")
		{
			FruitSpeed = 15,
			Point = 5
		},

		new Fruit("Pumpkin", random.Next(0, ClientSize.Width - 50), 0, "pumpkin.png")
		{
			FruitSpeed = 16,
			Point = 2
		},
		new Fruit("Fire", random.Next(0, ClientSize.Width - 50), 0, "fire.png")
		{
			FruitSpeed = 10,
			Point = 0 
        }

		};


			
			Fruit randomFruit = fruits[random.Next(fruits.Count)];

			
			Fruit fruit = new Fruit(randomFruit.Name, randomFruit.X, randomFruit.Y, randomFruit.ImagePath)
			{
				FruitSpeed = randomFruit.FruitSpeed,
				Point = randomFruit.Point
			};

			return fruit;
		}
		private void GameOver()
		{
			if (isGameOver)
				return;

			isGameOver = true;

			backgroundMusicPlayer.Stop();
			
			DialogResult result = MessageBox.Show("Game Over guwl! Do you want to restart?", "Game Over", MessageBoxButtons.YesNo);

			if (result == DialogResult.Yes)
			{
				
				ResetGame();
			}
			else
			{
				
				Application.Exit();
			}

			//Memory Leak stopped yavrum
			timer1.Stop();
			spawnTimer.Stop();
			catchTimer.Stop();
		}

		private void SetRandomSpawnInterval()
		{
			
			int randomInterval = random.Next(SpawnRate / 2, SpawnRate * 2);
			spawnTimer.Interval = randomInterval;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			
			List<Fruit> disposedFruits = new List<Fruit>();

			foreach (var fruit in fruitList)
			{
				fruit.Move();

				if (fruit.Y >= MaxY)
				{
					
					if (IsFruitInCatchingRange(fruit) == 1)
					{

						if (fruit.Point == 0)
						{

							GameOver();
						}
						score += fruit.Point; 




						Controls.Remove(fruit.PictureBox);
						fruit.PictureBox.Dispose();
						disposedFruits.Add(fruit);
						UpdateScoreAndLives();
					}
				}
			}

			
			foreach (var disposedFruit in disposedFruits)
			{
				fruitList.Remove(disposedFruit);
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		private byte IsFruitInCatchingRange(Fruit fruit)
		{
			Rectangle catcherBounds = catcherPictureBox.Bounds;
			Rectangle fruitBounds = fruit.PictureBox.Bounds;


			if (catcherBounds.IntersectsWith(fruitBounds))
			{

				if (fruit.Point == 0)
				{ GameOver(); }

				score += fruit.Point; 


				Timer catchAnimationTimer = new Timer();
				catchAnimationTimer.Interval = 15;
				catchAnimationTimer.Tick += (sender, e) =>
				{
					if (fruit.PictureBox.Width > 0 && fruit.PictureBox.Height > 0)
					{
						fruit.PictureBox.Width -= 5;
						fruit.PictureBox.Height -= 5;
					}
					else
					{
						catchAnimationTimer.Stop();
						Controls.Remove(fruit.PictureBox);
						fruit.PictureBox.Dispose();
						fruitList.Remove(fruit);
						UpdateScoreAndLives();
					}
				};
				catchAnimationTimer.Start();
				return 1;

			}
			return 2;
		}

		private void UpdateScoreAndLives()
		{
			if (score > 200 && lvl == 0)
			{
				timer1.Interval = timer1.Interval * (3/2);
				lvl = 1;
			}
			else if (score > 300 && lvl == 1)
			{
				timer1.Interval = timer1.Interval * (3/2) ;
				lvl = 2;

			}
			else if (score > 400 && lvl == 2)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 3;

			}
			else if (score > 450 && lvl == 3)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 4;

			}
			else if (score > 500 && lvl == 4)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 5;

			}
			else if (score > 550 && lvl == 5)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 6;

			}
			else if (score > 600 && lvl == 6)
			{
				timer1.Interval = timer1.Interval / 2;
				lvl = 7;

			}
			else if (score > 700 && lvl == 7)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 8;

			}
			else if (score > 750 && lvl == 8)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 9;

			}
			else if (score > 750 && lvl == 9)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 10;

			}
			else if (score > 790 && lvl == 10)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 11;

			}
			else if (score > 800 && lvl == 11)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 12;

			}
			else if (score > 850 && lvl == 12)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 13;

			}
			else if (score > 900 && lvl == 13)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 14;

			}
			else if (score > 920 && lvl == 14)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 15;

			}
			else if (score > 950 && lvl == 15)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 16;

			}
			else if (score > 1000 && lvl == 16)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 17;

			}
			else if (score > 1050 && lvl == 17)
			{
				timer1.Interval = timer1.Interval * (3 / 2);
				lvl = 18;

			}

			scoreLabel.Text = $"Score: {score} ";
		}



		private void InitializeFruits()
		{
			fruitList = new List<Fruit>();
			
		}


		private void catchTimer_Tick(object sender, EventArgs e)
		{
			if (!isGameOver)  
			{
				UpdateCatcherState();

				
				if (IsKeyDown(Keys.Left) && catcherPictureBox.Left > 0)
				{
					catcherPictureBox.Left -= 5; 
				}
				else if (IsKeyDown(Keys.Right) && catcherPictureBox.Right < ClientSize.Width)
				{
					catcherPictureBox.Left += 5; 
				}

				List<Fruit> copyOfFruitList = new List<Fruit>(fruitList);

				foreach (var fruit in copyOfFruitList)
				{
					IsFruitInCatchingRange(fruit);
				}
			}
		}



		private bool IsKeyDown(Keys key)
		{
			return (GetKeyState((int)key) & 0x8000) != 0;
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern short GetKeyState(int key);

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void ResetGame()
		{
			
			backgroundMusicPlayer.Stop();

			
			score = 0;
			lvl = 0;

			
			foreach (var fruit in fruitList)
			{
				Controls.Remove(fruit.PictureBox);
				fruit.Dispose();
			}
			fruitList.Clear();

			UpdateScoreAndLives();

			
			isGameOver = false;

			
			catcherPictureBox.Location = new Point(ClientSize.Width / 2 - 25, ClientSize.Height - 125);

		
			timer1.Start();
			spawnTimer.Start();
			catchTimer.Start();

			
			backgroundMusicPlayer = new System.Media.SoundPlayer("Zelda-Game-song.wav");
			backgroundMusicPlayer.PlayLooping();
		}





		private void button1_Click(object sender, EventArgs e)
		{
			if (!gameStarted)
			{
				
				ResetGame();
				timer1.Start();
				spawnTimer.Start();
				button1.Text = "Stop Game";
				gameStarted = true;
			}
			else
			{
				
				timer1.Stop();
				spawnTimer.Stop();
				button1.Text = "Start Game";
				MessageBox.Show("Oyun durdu. Yeniden başlamak için Start Game düğmesine tıklayın.");
				gameStarted = false;
			}
		}

	}

	public class Fruit : IDisposable 
	{
		public string Name { get; }
		public int X { get; private set; }
		public int Y { get; private set; }
		public string ImagePath { get; }
		public PictureBox PictureBox { get; }

		public int FruitSpeed { get; set; }
		public byte Point { get; set; }
		public byte SpecialItem { get; set; }

		public Fruit(string name, int x, int y, string imagePath)
		{
			Name = name;
			X = x;
			Y = y;
			ImagePath = imagePath;
			PictureBox = CreatePictureBox();
		}

		private PictureBox CreatePictureBox()
		{
			PictureBox pictureBox = new PictureBox
			{
				Name = Name,
				Size = new Size(50, 50),
				Location = new Point(X, Y),
				Image = Image.FromFile(ImagePath),
				SizeMode = PictureBoxSizeMode.StretchImage,
				BackColor = Color.Transparent,

			};

			return pictureBox;
		}

		public void Move()
		{
			Y += FruitSpeed;
			PictureBox.Location = new Point(X, Y);
		}

		public void Dispose()
		{
			PictureBox.Dispose();
		}
		public bool IsCaught()
		{

			return false ;
		}
	}
}
