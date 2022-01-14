using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IntergalacticInterceptors
{
	partial class Ships
	{
		///<summary>Корабль врага.</summary>
		internal sealed class Enemy : Ships
		{
			///<summary>Управление выстрелом.</summary>
			internal Vector3? Control_Shoot;

			private enum Model : int
			{
				Crab,
				Eye
			}

			internal Enemy(Players enemy, System.Numerics.Quaternion orientation, System.Numerics.Vector3 position) : base(enemy)
			{
				this.Health = Players.CalculateHealth(this.Player.Status.Level, 4);

				string current = ((Model)InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Model)).Length)).ToString("G");

				this.Physic = Variants.Imitator.Scene.Body.Add(nameof(Physic) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, nameof(Ships), current, nameof(Physic) + ".obj", current + "_" + nameof(Physic)), orientation, position, 10);
				if (this.Physic != null)
					this.Physic.Node.Gravity = System.Numerics.Vector3.Zero;

				this.Render = Variants.Imitator.Scene.Body.Add(nameof(Render) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, nameof(Ships), current, nameof(Render) + ".obj", current + "_" + nameof(Render)), orientation, position);
				if (this.Render != null)
				{
					this.Render.Node.ParentNode = this.Physic.Node;
					Imitator.Common.Intelligence.SetNoClipAndNullGravity(this.Render, true);
				}

				this.Emitter = Variants.Imitator.Scene.Emitter.Add(nameof(Emitter) + this.UniqueID, orientation, position);
				if (this.Emitter != null)
					this.Emitter.Node.ParentNode = this.Render.Node;
			}

			internal override void Dispose()
			{
				//Variants.Imitator.Scene.Emitter explosion = Variants.Imitator.Scene.Emitter.Add("Explosion" + this.UniqueID, System.Numerics.Vector3.Zero, this.Physic.Node.Position);
				//if (explosion != null)
				//{
				//	explosion.Material = System.IO.Path.Combine(InterInter.RootPath, "Particles", "Explosion.png");
				//	//explosion.Particle.Trigger = "Explosion";
				//	//explosion.Particle.Limit = 60;
				//	explosion.SoundPlay(System.IO.Path.Combine(InterInter.RootPath, "Particles", "Explosion." + InterInter.Randomizer.Next(4) + ".wav"), false);
				//	//Imitator.Common.Particle.Create(10000, Nothing, explosion);
				//}

				//if (InterInter.Randomizer.Next(20) == 0)
				//{
				//	//Imitator.Common.Entity.Library.Add(New Entities.Bonus(New Skill.Effect(Skill.Target.RocketLauncher, Skill.Parameter.Health_Ammo, 100, 0), this.Physic.Node.Position))
				//}
			}

			public override void Interact(Imitator.Common.Entity agent, params object[] args)
			{
				this.Player.Interact(agent, args);
				Ships ship = (Ships)agent;
				Variants.Imitator.Engine.Contact contact = (Variants.Imitator.Engine.Contact)args[0];
				Weapons.Arsenal arsenal = (Weapons.Arsenal)args[1];
				float damage = Weapons.CalculateDamage(ship, contact, arsenal);
				if (!this.Dead && damage > 0f)
				{
					base.DamageSounds();
					this.Health -= damage;
					if (this.Health <= 0f)
					{
						ship.Player.Status.Experience -= (int)this.Health;
						ship.Player.Status.Credits -= (int)this.Health;
						Gameplay.Galaxian.FragsCount += 1;
						this.Dead = true;
						this.Health = 1f + (float)InterInter.Randomizer.NextDouble() * 3f;
						this.Render.Node.ParentNode = null;
						this.Render.Node.Rotation = new Vector3((float)InterInter.Randomizer.NextDouble() * 2 - 1, (float)InterInter.Randomizer.NextDouble() * 2 - 1, (float)InterInter.Randomizer.NextDouble() * 2 - 1);
						this.Render.Node.Velocity = new Vector3((float)InterInter.Randomizer.NextDouble() * 2 - 1, (float)InterInter.Randomizer.NextDouble() * 2 - 1, (float)InterInter.Randomizer.NextDouble() * 2 - 1) * 100;
						Imitator.Common.Intelligence.SetNoClipAndNullGravity(this.Render, false);
						this.Render.TotalMass = this.Physic.TotalMass;
					}
				}
			}

			internal override void Update()
			{
				base.GeneralBehavior();
				if (!this.Dead)
				{
					InterInter mainForm = Variants.Imitator.Maths.Forms.GetInstance<InterInter>();
					if (mainForm != null)
					{
						if (this.Control_Shoot != null)
							Weapons.Enemy.Shoot(this, (float)System.Math.Atan2(-this.Control_Shoot.Value.X, -this.Control_Shoot.Value.Z));
						else
							this.Player.UpdateAmmunition();

						System.Numerics.Vector3 projection = mainForm.MainCamera.Project(this.Physic.Node.Position);
						if (mainForm != null && (projection - Entities.Sight.Position2D).Length() < Entities.Sight.Attraction)
							Imitator.Common.UserInterface.DrawString(mainForm.MainCamera, "LV." + this.Player.Status.Level + " (" + this.Health.ToString("0.") + "HP)", new System.Drawing.Point((int)(projection.X), (int)(projection.Y - mainForm.Font.Height * 3)), mainForm.Font, this.Color);
					}
				}
			}

			///<summary>Возвращает список кораблей врага.</summary>
			public static System.Collections.Generic.List<Enemy> List => Imitator.Common.Entity.Collection.Where((Imitator.Common.Entity currentEntity) => currentEntity is Enemy).ToList().ConvertAll((Imitator.Common.Entity currentEntity) => (Enemy)currentEntity);
		}
	}
}