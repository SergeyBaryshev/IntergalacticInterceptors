using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IntergalacticInterceptors
{
	partial class Gameplay
	{
		///<summary>Аналог игры "Space Invaders".</summary>
		internal sealed class Galaxian : Gameplay
		{
			///<summary>Прямоуголное поле боя.</summary>
			///<remarks>Направление вверх -Z, вниз +Z.</remarks>
			internal static readonly System.Drawing.Rectangle BattleField = System.Drawing.Rectangle.FromLTRB(-200, -150, 200, 150);

			internal Galaxian() : base()
			{
				Variants.Imitator.Scene.Camera.Default.Node.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)-Math.PI / 2);
				Variants.Imitator.Scene.Camera.Default.Node.Position = new Vector3(0, InterInter.Game_CameraDistance, 0);
				_ = new Ships.Stinger(Players.Human.List[0], Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI), new Vector3(0, 0, BattleField.Bottom));
				_ = new Entities.Ball(Vector3.Zero);
			}

			internal override void Update(InterInter mainForm)
			{
				base.CommonBehavior(mainForm);

				foreach (Players.Robot robot in Players.Robot.List)
					if (robot.Ship is null || robot.Ship.Dead || !Ships.Collection.Contains(robot.Ship))
						if (robot.Fraction == Players.Fractions.Aliens)
							_ = new Ships.Enemy(robot, Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI), new Vector3(BattleField.Left + InterInter.Randomizer.Next(400), 0, BattleField.Top - 50));
						else if (robot.Fraction == Players.Fractions.Humans)
							_ = new Ships.Stinger(robot, Quaternion.Identity, Vector3.Zero);

				if ((Players.Human.List[0].Ship == null || Players.Human.List[0].Ship.Dead) && Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.Space, -1))
					_ = new Ships.Stinger(Players.Human.List[0], Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI), new Vector3(0, 0, BattleField.Bottom));

				if (Ships.Stinger.List.Count == 0)
				{
					LocalRecord.Item1 = Math.Max(LocalRecord.Item1, FragsCount);
					LocalRecord.Item2 = Math.Max(LocalRecord.Item2, TimeCount);
					//InterInter.RegistrySave("Galaxian", LocalRecord);
					//Players.Collection[0].StateSave();
					InterInter.Game_CameraShaking = Vector3.Zero;
					InterInter.MenuState = InterInter.MenuEntries.Gameplay;
					InterInter.Gameplay = new Gameplay.Store();
					InterInter.CursorControl();
				}

				TimeCount += (float)Variants.Imitator.Physics.ElapsedTime.TotalSeconds;
				Entities.Sight.Update(mainForm);
			}

			public override void Dispose()
			{
			}
		}
	}
}