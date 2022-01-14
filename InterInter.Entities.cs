using System.Linq;
using System.Numerics;

namespace IntergalacticInterceptors
{
	internal abstract class Entities
	{

		///<summary>Астероид.</summary>
		internal sealed class Asteroid
		{

		}


		///<summary>Шар.</summary>
		internal sealed class Ball : Imitator.Common.Entity
		{
			internal Ball(Vector3 position)
			{
				this.Physic = Variants.Imitator.Scene.Body.Add(nameof(Physic) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, "Activities", nameof(Ball), nameof(Physic) + ".obj", nameof(Ball) + "_" + nameof(Physic)), System.Numerics.Quaternion.Identity, position, 0);
				if (this.Physic != null)
				{
					this.Physic.Node.Gravity = System.Numerics.Vector3.Zero;
					this.SetRandomVelocity();
				}

				this.Render = Variants.Imitator.Scene.Body.Add(nameof(Render) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, "Activities", nameof(Ball), nameof(Render) + ".obj", nameof(Ball) + "_" + nameof(Render)), System.Numerics.Quaternion.Identity, position);
				if (this.Render != null)
				{
					this.Render.Node.ParentNode = this.Physic.Node;
					//this.View.Node.Parent.ByDirection = False;
					Imitator.Common.Intelligence.SetNoClipAndNullGravity(this.Render, true);
				}
			}

			public void SetRandomVelocity()
			{
				this.Physic.Node.Velocity = System.Numerics.Vector3.Normalize(new Vector3(InterInter.Randomizer.Next(630) / 100.0F, 0, InterInter.Randomizer.Next(630) / 100.0F)) * 100;
			}

			internal override void Dispose() { }

			public override void Interact(Imitator.Common.Entity agent, params object[] args) { }

			internal override void Update()
			{
				this.Render.Node.Rotation = new Vector3((float)(System.Math.Atan2(-this.Physic.Node.Velocity.X, -this.Physic.Node.Velocity.Z)),
			-this.Physic.Node.Velocity.X, -this.Physic.Node.Velocity.Z);

				System.Collections.Generic.List<Variants.Imitator.Engine.Contact> contacts = this.Physic.Node.Contacts();
				if (contacts != null)
				{
					foreach (Variants.Imitator.Engine.Contact contact in contacts)
					{
						if (contact.Node?.BaseObject != null)
						{
							if (Imitator.Common.Entity.Item(contact.Node.BaseObject.Name) is Ships.Enemy target)
							{
								target.Dead = true;
								target.Health = 1F;
								target.Emitter.SoundPlay(System.IO.Path.Combine(InterInter.RootPath, nameof(Ships), "ricochet." + InterInter.Randomizer.Next(3) + ".wav"), false);
								this.SetRandomVelocity();
							}
						}
					}
				}

				if (InterInter.Gameplay is Gameplay.Galaxian)
					UpdateGalaxian();
			}

			internal void UpdateGalaxian()
			{
				//Dim pos2d As new Point((int)(ball.Grave.Node(0).Position.X), (int)(ball.Grave.Node(0).Position.Z))
				//if(Not Galaxian.BattleField.Contains(pos2d) )
				//	Dim normal As Numerics.Vector3 = -Numerics.Vector3.Normalize(ball.Grave.Node(0).Position)
				//	if(Numerics.Vector3.Dot(ball.Grave.Node(0).Velocity, normal) < 0 )
				//		ball.Grave.Node(0).Velocity = Numerics.Vector3.Reflect(ball.Grave.Node(0).Velocity, normal)
				//	End If
				//	Dim v As Numerics.Vector3 = Imitator.Scene.Camera.Default.Project(ball.Grave.Node(0).Position)
				//	Imitator.Scene.Camera.Default.DrawString(pos2d.ToString, new Point((int)(v.X), (int)(v.Y)), Inter.Font, Color.White)
				//End If
				if (this.Physic.Node.Position.X > Gameplay.Galaxian.BattleField.Right)
					this.Physic.Node.Velocity = System.Numerics.Vector3.Reflect(this.Physic.Node.Velocity, -System.Numerics.Vector3.UnitX);
				if (this.Physic.Node.Position.X < Gameplay.Galaxian.BattleField.Left)
					this.Physic.Node.Velocity = System.Numerics.Vector3.Reflect(this.Physic.Node.Velocity, System.Numerics.Vector3.UnitX);
				if (this.Physic.Node.Position.Z > Gameplay.Galaxian.BattleField.Bottom)
					this.Physic.Node.Velocity = System.Numerics.Vector3.Reflect(this.Physic.Node.Velocity, System.Numerics.Vector3.UnitZ);
				if (this.Physic.Node.Position.Z < Gameplay.Galaxian.BattleField.Top)
					this.Physic.Node.Velocity = System.Numerics.Vector3.Reflect(this.Physic.Node.Velocity, -System.Numerics.Vector3.UnitZ);
				this.Physic.Node.Position.Y = 0;
				this.Physic.Node.Velocity.Y = 0;
			}
		}

		///<summary>Бонус.</summary>
		internal sealed class Bonus : Imitator.Common.Entity
		{
			internal Skills.Effect Effect;

			internal Bonus(Skills.Effect effect, Vector3 position)
			{
				this.Effect = effect;
				this.Health = 10;

				this.Physic = Variants.Imitator.Scene.Body.Add(nameof(Physic) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, "Activities", nameof(Bonus), nameof(Physic) + ".obj"), System.Numerics.Quaternion.Identity, position);
				if (this.Physic != null)
					this.Physic.Node.Gravity = System.Numerics.Vector3.Zero;

				this.Render = Variants.Imitator.Scene.Body.Add(nameof(Render) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, "Activities", nameof(Bonus), nameof(Render) + ".obj"), System.Numerics.Quaternion.Identity, position);
				if (this.Render != null)
				{
					this.Render.Node.ParentNode = this.Physic.Node;
					Imitator.Common.Intelligence.SetNoClipAndNullGravity(this.Render, true);
				}
			}

			public override void Interact(Imitator.Common.Entity agent, params object[] args) { }

			internal override void Update()
			{
				if (this.Health > 0)
				{
					InterInter mainForm = Variants.Imitator.Maths.Forms.GetInstance<InterInter>();
					Vector3 v = mainForm.MainCamera.Project(this.Physic.Node.Position);
					if (v.Z > mainForm.MainCamera.NearClip && v.Z < mainForm.MainCamera.FarClip)
						Imitator.Common.UserInterface.DrawString(mainForm.MainCamera, this.Effect.ToString(), new System.Drawing.Point((int)(v.X), (int)(v.Y - mainForm.Font.Height * 3)), mainForm.Font, this.Color);

					System.Collections.Generic.List<Variants.Imitator.Engine.Contact> contacts = this.Physic.Node.Contacts();
					if (contacts != null)
					{
						for (int eachC = contacts.Count - 1; eachC > 0; eachC -= 1)
						{
							if (contacts[eachC].Node?.BaseObject != null)
							{
								if (contacts[eachC].Node.BaseObject is Ships.Stinger entity)
								{
									entity.Player.ReloadAmmunition(Weapons.Arsenal.RocketLauncher, (int)(this.Effect.Capacity));
									this.Health = 0;
									return;
									//this.Health -= Imitator.Maths.Timer.Interval * (1 + this.Player.Уровень / 10.0F)
									//this.Emitter.SoundPlay(IO.Path.Combine(Inter.RootPath, "Ships", "damage." & Inter.Game_Ramdomizer.Next(5) & ".wav"), False)
									//Inter.Game_Shaking = new Numerics.Vector3(If(Inter.Game_Ramdomizer.Next(2) = 0, -10, +10), If(Inter.Game_Ramdomizer.Next(2) = 0, -10, +10), 0)
								}
							}
						}
					}
				}
				this.Health -= (float)Variants.Imitator.Physics.ElapsedTime.TotalSeconds;
			}

			internal static bool FindAll(Imitator.Common.Entity current)
			{
				return current is Bonus;
			}

			internal override void Dispose() { }
		}

		///<summary>Прицел.</summary>
		internal abstract class Sight
		{
			private static Vector3 position2D, position3D;
			internal static Variants.Imitator.Element.Material DefaultMaterial => System.IO.Path.Combine(InterInter.RootPath, "User Interface", "Aim.Default.png");
			internal static Variants.Imitator.Element.Material TargetMaterial => System.IO.Path.Combine(InterInter.RootPath, "User Interface", "Aim.Target.png");
			internal static Vector3 Size2D => DefaultMaterial.get_Resolution(Variants.Imitator.Element.Material.TextureType.Base) / 2;
			internal static System.Collections.Generic.List<Ships.Enemy> TargetList;
			internal static System.Collections.Generic.List<Variants.Imitator.Engine.Contact> Contacts;
			///<summary>Радиус зоны показа информации о враге.</summary>
			public static float Attraction { get; set; } = 100;
			///<summary>Дистанция прицела от положения корабля.</summary>
			public static float Distance { get; set; } = 120;

			internal static void Update(InterInter mainForm)
			{
				Contacts = Variants.Imitator.Physics.RayCastByTarget(mainForm.MainCamera.Node.Position, position3D);
				TargetList = Ships.Enemy.List.Where(IsOnSight).ToList();
				mainForm.MainCamera.DrawSprite(TargetList.Count > 0 ? TargetMaterial : DefaultMaterial, Variants.Imitator.Element.Material.TextureType.Base, System.Drawing.RectangleF.FromLTRB(0, 0, 1, 1), new System.Drawing.Rectangle((int)(position2D.X - Size2D.X / 2), (int)(position2D.Y - Size2D.Y / 2), (int)Size2D.X, (int)Size2D.Y), System.Numerics.Vector3.Zero, System.Drawing.Color.White);
				if (Variants.Imitator.Console.Debug)
					mainForm.MainCamera.DrawString(position3D.ToString() + System.Environment.NewLine + position2D.ToString(), new System.Drawing.Point(mainForm.MainCamera.Width / 2, 0), mainForm.Font, System.Drawing.Color.White);
			}

			internal static Vector3 Position2D
			{
				get
				{
					return position2D;
				}
				set
				{
					Variants.Imitator.Scene.Camera currentCamera = Variants.Imitator.Scene.Camera.Default;
					position2D = currentCamera.Project(position3D);
					position2D += new Vector3(value.X, value.Y, 0);
					position2D.X = System.Math.Max(position2D.X, Size2D.X / 2);
					position2D.Y = System.Math.Max(position2D.Y, Size2D.Y / 2);
					position2D.X = System.Math.Min(position2D.X, currentCamera.Width - Size2D.X / 2);
					position2D.Y = System.Math.Min(position2D.Y, currentCamera.Height - Size2D.Y / 2);
					position3D = currentCamera.Unproject(position2D);
				}
			}

			internal static Vector3 Position3D
			{
				get
				{
					return position3D;
				}
				set
				{
					Variants.Imitator.Scene.Camera currentCamera = Variants.Imitator.Scene.Camera.Default;
					position3D = value;
					position3D.Z -= Distance;
					position2D = currentCamera.Project(position3D);
					position2D.X = System.Math.Max(position2D.X, Size2D.X / 2);
					position2D.Y = System.Math.Max(position2D.Y, Size2D.Y / 2);
					position2D.X = System.Math.Min(position2D.X, currentCamera.Width - Size2D.X / 2);
					position2D.Y = System.Math.Min(position2D.Y, currentCamera.Height - Size2D.Y / 2);
					position3D = currentCamera.Unproject(position2D);
				}
			}

			private static bool IsOnSight(Imitator.Common.Entity current)
			{
				if (Contacts != null)
				{
					foreach (Variants.Imitator.Engine.Contact contact in Contacts)
					{
						if (contact.Node?.BaseObject != null && string.Equals(contact.Node.BaseObject.Name, nameof(Imitator.Common.Entity.Physic) + current.UniqueID, System.StringComparison.OrdinalIgnoreCase))
							return true;
					}
				}
				return false;
			}
		}
	}
}