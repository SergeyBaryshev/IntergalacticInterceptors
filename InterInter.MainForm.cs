using System;
using System.Numerics;
using System.Drawing;
using System.Windows.Forms;

namespace IntergalacticInterceptors
{
	internal sealed partial class InterInter : Form
	{
		///<summary>Генератор псевдослучайных целых чисел.</summary>
		internal static readonly Random Randomizer = new Random();
		///<summary>Корневая папка "Content".</summary>
		internal static readonly string RootPath = "Content";
		private static readonly Action DelegateFramework = new Action(Framework);
		private static (DateTime, int, int) FPS_Counter;
		internal readonly Font FontTitle = new Font("Arial", 24, FontStyle.Bold);
		internal readonly Font FontHeader = new Font("Impact", 80, FontStyle.Regular);
		internal static Vector3 Game_CameraShaking;
		internal static float Game_CameraDistance = 500.0F;
		internal static int Game_RoundCount;
		private static Point Game_MenuPosition;
		private static Gameplay gameplayReference;
		internal static Gameplay Gameplay
		{
			get
			{
				return gameplayReference;
			}
			set
			{
				if (gameplayReference != null)
					gameplayReference.Dispose();
				gameplayReference = value;
			}
		}


		public InterInter()
		{
			InitializeComponent();
		}

		[STAThread]
		internal static void Main()
		{
			Variants.Imitator.Maths.Run<InterInter>(DelegateFramework);
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			this.Text = $"{Application.ProductName} - {Application.CompanyName}";
			Variants.Imitator.Sound.DistanceScaler = Game_CameraDistance;
			Variants.Imitator.Physics.Gravity = Vector3.Zero;
			CreateMenuBackground();
		}

		private static void Framework()
		{
			InterInter mainForm = Variants.Imitator.Maths.Forms.GetInstance<InterInter>();
			if (mainForm != null)
			{
				if (FPS_Counter.Item1 < DateTime.Now - TimeSpan.FromSeconds(1.0))
				{
					FPS_Counter.Item1 = DateTime.Now;
					FPS_Counter.Item3 = FPS_Counter.Item2;
					FPS_Counter.Item2 = 0;
				}
				FPS_Counter.Item2 = 1 + FPS_Counter.Item2;
				string fps = $"FPS {FPS_Counter.Item3}";
				mainForm.MainCamera.DrawString(fps, new Point(mainForm.MainCamera.Width - (int)(fps.Length * mainForm.Font.Height /1.5), 0), mainForm.Font, Color.LawnGreen);

				if (MenuState != MenuEntries.Gameplay)
				{
					Imitator.Common.UserInterface.DrawString(mainForm.MainCamera, Application.ProductName, new Point(mainForm.MainCamera.Width / 2, 0), mainForm.FontHeader, Color.White);
					Game_MenuPosition = new Point(mainForm.MainCamera.Width / 10, mainForm.MainCamera.Height / 2);
				}

				switch (MenuState)
				{
					case MenuEntries.Main:
						Menu_Main(mainForm);
						break;
					case MenuEntries.Start:
						Menu_Start(mainForm);
						break;
					case MenuEntries.Difficulty:
						Menu_Difficulty(mainForm);
						break;
					case MenuEntries.Options:
						Menu_Options(mainForm);
						break;
					case MenuEntries.Controls:
						Menu_Controls(mainForm);
						break;
					case MenuEntries.Graphics:
						Menu_Graphics(mainForm);
						break;
					case MenuEntries.Sound:
						Menu_Sound(mainForm);
						break;
					case MenuEntries.Language:
						Menu_Language(mainForm);
						break;
					case MenuEntries.Gameplay:
						InterInter.Gameplay.Update(mainForm);
						break;
				}
			}
		}

		private Variants.Imitator.Scene.Camera MainCameraReference;
		internal Variants.Imitator.Scene.Camera MainCamera
		{
			get
			{
				if ((this.MainCameraReference == null) || !this.MainCameraReference.Actual)
				{
					this.MainCameraReference = Variants.Imitator.Scene.Camera.Default;
				}
				return this.MainCameraReference;
			}
		}

		private static void CreateMenuBackground()
		{
			Variants.Imitator.Scene.Camera.Default.Node.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -0.3f);

			//if (Variants.Imitator.Scene.Body.Add("Earth", System.IO.Path.Combine(InterInter.RootPath, "planets", "earth.3ds", "0"), Vector3.UnitY, new Vector3(-1, -0.6F, -2)) != null)
			//{
			//    Variants.Imitator.Scene.Body.Item("Earth").Node.Rotation.Y = 0.002F;
			//}

			//if (Variants.Imitator.Scene.Body.Add("Moon", System.IO.Path.Combine(RootPath, "planets", "earth.3ds", "0"), Vector3.UnitY, new Vector3(20, -5, -40)) != null)
			//{
			//    Variants.Imitator.Scene.Body.Item("Moon").get_Material(0).BaseFile = System.IO.Path.Combine(RootPath, "planets", "moon.jpg");
			//}

			if (Variants.Imitator.Scene.Body.Add("station", System.IO.Path.Combine(RootPath, "stations", "base", "station_base.3ds", "0"), Quaternion.Identity, new Vector3(0.3F, -0.4F, -1)) != null)
			{
				Variants.Imitator.Scene.Body.Item("station").Node.Rotation.Y = -0.01F;
			}
		}

		///<summary>Пункты меню.</summary>
		internal enum MenuEntries : byte
		{
			///<summary>Главное меню.</summary>
			Main,
			///<summary>Начало игры.</summary>
			Start,
			///<summary>Сложность.</summary>
			Difficulty,
			///<summary>Настройки.</summary>
			Options,
			///<summary>Графика.</summary>
			Graphics,
			///<summary>Звук.</summary>
			Sound,
			///<summary>Управление.</summary>
			Controls,
			///<summary>Язык.</summary>
			Language,
			///<summary>Игровой процесс.</summary>
			Gameplay,
		};
		internal static MenuEntries MenuState = MenuEntries.Main;

		internal static void CursorControl()
		{
			if (MenuState == MenuEntries.Gameplay && !(Gameplay is Gameplay.Store))
			{
				Variants.Imitator.Input.CursorToggle(false);
				Variants.Imitator.Input.MouseCentered = true;
				Variants.Imitator.Physics.TimeScale = 1.0F;
			}
			else
			{
				Variants.Imitator.Input.CursorToggle(true);
				Variants.Imitator.Input.MouseCentered = false;
				Variants.Imitator.Physics.TimeScale = 0.0F;
			}
		}

		private static void Menu_Main(InterInter mainForm)
		{
			int result = Imitator.Common.UserInterface.List(mainForm.MainCamera, Game_MenuPosition, mainForm.FontTitle, Color.White, Color.Empty, 0, System.Windows.Forms.VisualStyles.ContentAlignment.Left, InterInter.Gameplay != null ? Localizator.Phrase[Localizator.EnumPhrases.Continue] : Localizator.Phrase[Localizator.EnumPhrases.New_game], Localizator.Phrase[Localizator.EnumPhrases.Options], Localizator.Phrase[Localizator.EnumPhrases.Exit]);

			if (result == 1 || result == -1)
			{
				if (InterInter.Gameplay != null)
				{
					MenuState = MenuEntries.Gameplay;
					CursorControl();
				}
				else if (result == 1)
					MenuState = MenuEntries.Start;
			}

			else if (result == 2)
				MenuState = MenuEntries.Options;

			else if (result == 3)
			{
				My.Settings.Save();
				Application.Exit();
			}
		}

		private static void Menu_Start(InterInter mainForm)
		{
			int result = Imitator.Common.UserInterface.List(mainForm.MainCamera, Game_MenuPosition, mainForm.FontTitle, Color.White, Color.Empty, 0, System.Windows.Forms.VisualStyles.ContentAlignment.Left, Localizator.Phrase[Localizator.EnumPhrases.SinglePlayer], Localizator.Phrase[Localizator.EnumPhrases.MultyPlayer]);

			if (result == -1)
				MenuState = MenuEntries.Main;

			else if (result == 1)
				MenuState = MenuEntries.Difficulty;
		}

		private static void Menu_Difficulty(InterInter mainForm)
		{
			int result = Imitator.Common.UserInterface.List(mainForm.MainCamera, Game_MenuPosition, mainForm.FontTitle, Color.White, Color.Empty, 0, System.Windows.Forms.VisualStyles.ContentAlignment.Left, "Easy: 5 rounds", "Normal: 7 rounds", "Hard: 10 rounds");

			if (result == -1)
				MenuState = MenuEntries.Start;
			else if (result >= 1 && result <= 3)
			{
				MenuState = MenuEntries.Gameplay;
				Game_RoundCount = new int[] { 5, 7, 10 }[result - 1];
				InterInter.Gameplay = new Gameplay.Store();
			}
		}

		private static void Menu_Options(InterInter mainForm)
		{
			int result = Imitator.Common.UserInterface.List(mainForm.MainCamera, Game_MenuPosition, mainForm.FontTitle, Color.White, Color.Empty, 0, System.Windows.Forms.VisualStyles.ContentAlignment.Left, Localizator.Phrase[Localizator.EnumPhrases.Controls], Localizator.Phrase[Localizator.EnumPhrases.Graphics], Localizator.Phrase[Localizator.EnumPhrases.Sound], Localizator.Phrase[Localizator.EnumPhrases.Language]);

			if (result == -1)
				MenuState = MenuEntries.Main;

			else if (result == 1)
				MenuState = MenuEntries.Controls;

			else if (result == 2)
				MenuState = MenuEntries.Graphics;

			else if (result == 3)
				MenuState = MenuEntries.Sound;

			else if (result == 4)
				MenuState = MenuEntries.Language;
		}

		private static void Menu_Controls(InterInter mainForm)
		{
			int result = Imitator.Common.UserInterface.List(mainForm.MainCamera, Game_MenuPosition, mainForm.FontTitle, Color.White, Color.Empty, 0, System.Windows.Forms.VisualStyles.ContentAlignment.Left, Localizator.Phrase[Localizator.EnumPhrases.Controls]);

			if (result == -1)
				MenuState = MenuEntries.Options;
		}

		private static void Menu_Graphics(InterInter mainForm)
		{
			int result = Imitator.Common.UserInterface.List(mainForm.MainCamera, Game_MenuPosition, mainForm.FontTitle, Color.White, Color.Empty, 0, System.Windows.Forms.VisualStyles.ContentAlignment.Left, Localizator.Phrase[Localizator.EnumPhrases.Graphics]);

			if (result == -1)
				MenuState = MenuEntries.Options;
		}

		private static void Menu_Sound(InterInter mainForm)
		{
			int result = Imitator.Common.UserInterface.List(mainForm.MainCamera, Game_MenuPosition, mainForm.FontTitle, Color.White, Color.Empty, 0, System.Windows.Forms.VisualStyles.ContentAlignment.Left, Localizator.Phrase[Localizator.EnumPhrases.Sound]);

			if (result == -1)
				MenuState = MenuEntries.Options;
		}

		private static void Menu_Language(InterInter mainForm)
		{
			int result = Imitator.Common.UserInterface.List(mainForm.MainCamera, Game_MenuPosition, mainForm.FontTitle, Color.White, Color.Empty, 0, System.Windows.Forms.VisualStyles.ContentAlignment.Left, Localizator.Phrase[Localizator.EnumPhrases.Language]);

			if (result == -1)
				MenuState = MenuEntries.Options;
		}
	}
}