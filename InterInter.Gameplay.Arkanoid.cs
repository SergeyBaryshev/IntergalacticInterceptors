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
		///<summary>Аналог игры "Arkanoid".</summary>
		internal sealed class Arkanoid : Gameplay
		{
			///<summary>Скорость передвижения.</summary>
			private static float PlayerSpeed { get; set; } = 50;
			private static Variants.Imitator.Scene.Voxel Asteroid;

			internal Arkanoid():base()
			{
				Variants.Imitator.Scene.Camera.Default.Node.Orientation = Quaternion.Identity;
				Variants.Imitator.Scene.Camera.Default.Node.Position = new Vector3(0, 0, InterInter.Game_CameraDistance);
				Variants.Imitator.Scene.Voxel.Add("Asteroid", System.IO.Path.Combine(InterInter.RootPath, "Asteroids", "test.bmp"), Vector3.Zero, Vector3.Zero, Vector3.One * 10);
				Asteroid = Variants.Imitator.Scene.Voxel.Item("Asteroid");
				_ = new Ships.Stinger(Players.Human.List[0], Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI), new Vector3(0, 0, 250));
				for (int index = 0; index < 10; index += 1)
				{
					_ = new Players.Robot(Players.Fractions.Aliens, Players.Human.List[0].Status.Level);
					_ = new Ships.Enemy(Players.Robot.List[index], Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI), new Vector3(Galaxian.BattleField.Left + index * 40, 0, Galaxian.BattleField.Top));
				}
			}

			private static void ControlBlob(InterInter mainForm, Variants.Imitator.Engine.Blob blob)
			{
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad5)) blob.Position = blob.Position - new Vector3((float)(Math.Sin(mainForm.MainCamera.Node.Direction.X)), 0, (float)(Math.Cos(mainForm.MainCamera.Node.Direction.X)));
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad8)) blob.Position = blob.Position + new Vector3((float)(Math.Sin(mainForm.MainCamera.Node.Direction.X)), 0, (float)(Math.Cos(mainForm.MainCamera.Node.Direction.X)));
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad4)) blob.Position = blob.Position - new Vector3((float)(Math.Cos(mainForm.MainCamera.Node.Direction.X)), 0, -(float)(Math.Sin(mainForm.MainCamera.Node.Direction.X)));
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad6)) blob.Position = blob.Position + new Vector3((float)(Math.Cos(mainForm.MainCamera.Node.Direction.X)), 0, -(float)(Math.Sin(mainForm.MainCamera.Node.Direction.X)));
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad0)) blob.Position = blob.Position + Vector3.UnitY;
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad2)) blob.Position = blob.Position - Vector3.UnitY;

				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad7, -1)) blob.Radius = blob.Radius + 1.0F;
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad1, -1)) blob.Radius = blob.Radius - 1.0F;
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad9)) blob.Strength = blob.Strength + 1.0F;
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.NumPad3)) blob.Strength = blob.Strength - 1.0F;
				mainForm.MainCamera.DrawString("Blob:" + blob.ToString(), new System.Drawing.Point(0, 480), mainForm.Font, System.Drawing.Color.White);
			}

			internal override void Update(InterInter mainForm)
			{
				base.CommonBehavior(mainForm);
				ControlBlob(mainForm, Asteroid.Blobs[0]);

				//if(Imitator.Input.Keyboard(Keys.Add, -1) )
				//    ReDim Preserve Asteroid.Blobs(Asteroid.Blobs.GetUpperBound(0) + 1)
				//    Asteroid.Blobs(Asteroid.Blobs.GetUpperBound(0)).Position = {100 * Rnd(), 100 * Rnd(), 100 * Rnd()}
				//    Asteroid.Blobs(Asteroid.Blobs.GetUpperBound(0)).Radius = 10
				//    Asteroid.Blobs(Asteroid.Blobs.GetUpperBound(0)).Strength = 20
				//End If

				System.Numerics.Vector3 velocity = default;
				for (int eachBlob = Asteroid.Blobs.GetUpperBound(0); eachBlob > 0; eachBlob -= 1)
				{
					velocity = mainForm.MainCamera.Project(Asteroid.Blobs[eachBlob].Position);
					if (velocity.Z > mainForm.MainCamera.NearClip && velocity.Z < mainForm.MainCamera.FarClip)
						mainForm.MainCamera.DrawString(eachBlob + " - " + Asteroid.Blobs[eachBlob].Position.ToString(), new System.Drawing.Point((int)(velocity.X), (int)(velocity.Y)), mainForm.Font, System.Drawing.Color.White);
				}

				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.W))
					velocity.Z = -PlayerSpeed;
				else if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.S))
					velocity.Z = +PlayerSpeed;
				else
					velocity.Z = 0;

				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.A))
					velocity.X = -PlayerSpeed;
				else if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.D))
					velocity.X = +PlayerSpeed;
				else
					velocity.X = 0;

				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.LControlKey))
					mainForm.MainCamera.Node.Velocity.Y = -PlayerSpeed;
				else if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.Space))
					mainForm.MainCamera.Node.Velocity.Y = +PlayerSpeed;
				else
					mainForm.MainCamera.Node.Velocity.Y = 0;

				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.LShiftKey))
					velocity *= 2;

				mainForm.MainCamera.Node.Velocity.X = (float)(Math.Sin(mainForm.MainCamera.Node.Direction.X) * velocity.Z) + (float)(Math.Cos(mainForm.MainCamera.Node.Direction.X) * velocity.X);
				mainForm.MainCamera.Node.Velocity.Z = (float)(Math.Cos(mainForm.MainCamera.Node.Direction.X) * velocity.Z) - (float)(Math.Sin(mainForm.MainCamera.Node.Direction.X) * velocity.X);

				mainForm.MainCamera.Node.Rotation = new Vector3(Variants.Imitator.Input.MouseVelocity(), 0);
				mainForm.MainCamera.Node.Rotation = new Vector3(-(float)(mainForm.MainCamera.Node.Rotation.Y * Math.Cos(mainForm.MainCamera.Node.Direction.X)), -mainForm.MainCamera.Node.Rotation.X, (float)(mainForm.MainCamera.Node.Rotation.Y * Math.Sin(mainForm.MainCamera.Node.Direction.X)));
				mainForm.MainCamera.Node.Direction = new Vector3(mainForm.MainCamera.Node.Direction.X, (float)(Math.Min(Math.Max(-Math.PI / 2, mainForm.MainCamera.Node.Direction.Y), Math.PI / 2)), 0);

			}

			public override void Dispose()
			{
				Variants.Imitator.Scene.Voxel.Remove(Asteroid.Name);
			}
		}
	}
}