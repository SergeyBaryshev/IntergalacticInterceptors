using System;
using System.Linq;
using System.Numerics;

namespace IntergalacticInterceptors
{
	///<summary>Базовый супер-класс для различных режимов игры.</summary>
	internal abstract partial class Gameplay : IDisposable
	{
		///<summary>Количество сбитых кораблей противника.</summary>
		internal static int FragsCount = 0;
		///<summary>Количество сыгранных раундов.</summary>
		internal static int RoundCount = 0;
		///<summary>Количество времени в данном раунде.</summary>
		internal static float TimeCount = 0;
		///<summary>Рекорды: Item1 - frags, Item2 - time.</summary>
		internal static System.ValueTuple<int, float> LocalRecord;

		internal Gameplay()
		{
			Imitator.Common.Entity.Clear();
			Variants.Imitator.Scene.Clear(true);
			//Variants.Imitator.Scene.Trigger.Add("Explosion", System.IO.Path.Combine(InterInter.RootPath, "Scripts", "Explosions.vb"), "Explosion", Quaternion.Identity, System.Numerics.Vector3.Zero);
			FragsCount = 0;
			RoundCount += 1;
			TimeCount = 0;
			StarField = true;
			//InterInter.CurrentMusic = string.Empty;
		}

		public bool StarField
		{
			get
			{
				return Variants.Imitator.Scene.Effect.Item(nameof(StarField)) != null;
			}
			set
			{
				if (value && Variants.Imitator.Scene.Effect.Add(nameof(StarField), System.IO.Path.Combine(InterInter.RootPath, "Scripts", "StarField.fx"), "StarTech") != null)
				{
					//Variants.Imitator.Scene.Camera.Default.ScreenMaterialName = .Item(nameof(StarField)).get_Material(Variants.Imitator.Scene.Effect.Item(nameof(StarField)).MaterialCount).Name;
					Variants.Imitator.Scene.Camera.Default.ScreenMaterialName = $"{nameof(Variants.Imitator.Scene.Effect)}:0:{nameof(StarField)}";
				}
				else if (!value)
					Variants.Imitator.Scene.Effect.Remove(nameof(StarField));
			}
		}

		///<summary>Общее для всех игровых режимов поведение.</summary>
		private void CommonBehavior(InterInter mainForm)
		{
			UserInterface.AmmunitionStats(mainForm);
			UserInterface.PlayerStats(mainForm);
			UserInterface.RoundStats(mainForm);

			InterInter.Game_CameraShaking *= -(0.999F - Math.Min(2.0F / 3.0F, (float)Variants.Imitator.Physics.ElapsedTime.TotalSeconds * 10.0F));

			if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.Escape, -1))
			{
				InterInter.MenuState = InterInter.MenuEntries.Main;
				InterInter.CursorControl();
			}

			if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.F1, -1))
				_ = new Players.Robot(Players.Fractions.Humans, Players.Human.List[0].Status.Level);

			if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.F2, -1))
				Players.Robot.List.Where((Players player) => player.Fraction == Players.Fractions.Humans).FirstOrDefault()?.Dispose();

			if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.F3, -1))
				_ = new Players.Robot(Players.Fractions.Aliens, Players.Human.List[0].Status.Level);

			if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.F4, -1))
				Players.Robot.List.Where((Players player) => player.Fraction == Players.Fractions.Aliens).FirstOrDefault()?.Dispose();

			if (Variants.Imitator.Console.Debug)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.AppendLine($"Bullets: {Imitator.Common.Bullet.Count(System.IO.Path.Combine(InterInter.RootPath, "Particles", "Bullet.bmp"))}");
				Imitator.Common.UserInterface.DrawString(mainForm.MainCamera, sb.ToString(), new System.Drawing.Point(mainForm.MainCamera.Width / 2, mainForm.MainCamera.Height - mainForm.Font.Height * sb.ToString().Split(Environment.NewLine.ToCharArray()).Length), mainForm.Font, System.Drawing.Color.Blue);
			}
		}

		public abstract void Dispose();
		internal abstract void Update(InterInter mainForm);
	}
}