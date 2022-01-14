using System;
using System.Linq;
using System.Numerics;

namespace IntergalacticInterceptors
{
	partial class Players
	{
		internal sealed class Robot : Players
		{
			///<summary>Граничная дистанция в метрах.</summary>
			internal static float FlockRadius { get; set; } = 50;

			private static string GenerateNickname()
			{ 
				string[] names = System.Enum.GetNames(typeof(System.Windows.Forms.Shortcut));
				return names[InterInter.Randomizer.Next(names.Length)];
			}

			public Robot(Fractions fraction, int level) : base(GenerateNickname())
			{
				this.Fraction = fraction;
				this.Status.Credits = InterInter.Randomizer.Next(10000);
				this.Status.Experience = InterInter.Randomizer.Next(1000);
				this.Status.Level = System.Math.Max(InterInter.Randomizer.Next(4) - 2 + level, 0);
				if (this.Fraction == Fractions.Aliens)
					this.Add(new Weapons.Enemy(level));
				else
					this.Add(new Weapons.RocketLauncher(level));
				this.ReloadAmmunition(Weapons.Arsenal.None);
			}

			public override void Interact(Imitator.Common.Entity agent, params object[] args) { }

			public override void Update()
			{
				if (InterInter.MenuState == InterInter.MenuEntries.Gameplay && this.Ship != null && !this.Ship.Dead && Ships.Collection.Contains(this.Ship))
					if (this.Ship is Ships.Stinger)
						UpdateGalaxianStinger();
					else
						UpdateGalaxianEnemy();
			}

			internal void UpdateGalaxianEnemy()
			{
				this.Ship.Physic.Node.Velocity.X = System.Math.Max(System.Math.Min(this.Ship.Physic.Node.Velocity.X + (InterInter.Randomizer.Next(11) - 5) * Variants.Imitator.Physics.TimeScale, 10), -10);
				this.Ship.Physic.Node.Velocity.Z = System.Math.Max(System.Math.Min(this.Ship.Physic.Node.Velocity.Z + (InterInter.Randomizer.Next(11) - 3) * Variants.Imitator.Physics.TimeScale, 50), 0);
				this.Ship.Physic.Node.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.Atan2(this.Ship.Physic.Node.Velocity.X, this.Ship.Physic.Node.Velocity.Z));
				if (System.Math.Abs(this.Ship.Physic.Node.Position.X) > Gameplay.Galaxian.BattleField.Right)
					this.Ship.Physic.Node.Velocity.X = System.Math.Abs(this.Ship.Physic.Node.Velocity.X) * System.Math.Sign(Gameplay.Galaxian.BattleField.Right - this.Ship.Physic.Node.Position.X);
				if (this.Ship.Physic.Node.Position.Z > Gameplay.Galaxian.BattleField.Bottom + 50)
					this.Ship.Physic.Node.Position.Z = Gameplay.Galaxian.BattleField.Top - 50;
				this.Ship.Physic.Node.Rotation = Vector3.Zero;
				this.Ship.Physic.Node.Position.Y = 0;

				Weapons.Enum_Enemy currentWeapon = (Weapons.Enum_Enemy)InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Weapons.Enum_Enemy)).Length - 1) + 1;
				if (currentWeapon != Weapons.Enum_Enemy.None)
				{
					(this.Ship as Ships.Enemy).Control_Shoot = FindClosestTarget();
				}
			}

			internal void UpdateGalaxianStinger()
			{
				Vector3? closestTarget = FindClosestTarget();
				if (closestTarget != null)
				{
					this.Ship.Physic.Node.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.Atan2(-closestTarget.Value.X, -closestTarget.Value.Z));
					Ships.Stinger stinger = (Ships.Stinger)this.Ship;
					stinger.Control_SecondaryFire = Weapons.Arsenal.RocketLauncher;
				}

				Vector3 closestDistance = -this.Ship.Physic.Node.Position * (float)Variants.Imitator.Physics.ElapsedTime.TotalSeconds;
				foreach (Ships.Enemy eachChar in Ships.Enemy.List)
				{
					if (!eachChar.Dead)
					{
						Vector3 dist = this.Ship.Physic.Node.Position - eachChar.Physic.Node.Position;
						float radius = FlockRadius * FlockRadius / (dist.X * dist.X + dist.Z * dist.Z);
						if (radius < 1) continue;
						float angle = (float)(System.Math.Atan2(dist.X, dist.Z));
						closestDistance.X = closestDistance.X + (float)(System.Math.Sin(angle));
						closestDistance.Z = closestDistance.Z + (float)(System.Math.Cos(angle));
						closestDistance = closestDistance * radius;
					}
				}
				this.Ship.Physic.Node.Velocity = this.Ship.Physic.Node.Velocity + closestDistance;

				this.Ship.Physic.Node.Position.X = System.Math.Max(Gameplay.Galaxian.BattleField.Left, System.Math.Min(Gameplay.Galaxian.BattleField.Right, this.Ship.Physic.Node.Position.X));
				this.Ship.Physic.Node.Position.Z = System.Math.Max(Gameplay.Galaxian.BattleField.Top, System.Math.Min(Gameplay.Galaxian.BattleField.Bottom, this.Ship.Physic.Node.Position.Z));
				this.Ship.Physic.Node.Position.Y = 0;
			}

			private Vector3? FindClosestTarget()
			{
				System.Collections.Generic.List<Ships> targets = this.Fraction == Fractions.Humans ? Ships.Enemy.List.Cast<Ships>().ToList() : Ships.Stinger.List.Cast<Ships>().ToList();
				var closestDistance = new Vector3(1000);
				int closestIndex = -1;
				for (int currentIndex = targets.Count - 1; currentIndex >= 0; currentIndex -= 1)
				{
					if (!targets[currentIndex].Dead)
					{
						Vector3 currentDistance = this.Ship.Physic.Node.Position - targets[currentIndex].Physic.Node.Position;
						if (currentDistance.LengthSquared() < closestDistance.LengthSquared())
						{
							closestDistance = currentDistance;
							closestIndex = currentIndex;
						}
					}
				}
				if (closestIndex > -1)
				{
					return Vector3.Normalize(closestDistance);
				}
				return null;
			}
			
			public override string ToString()
			{
				return $"[bot] {base.ToString()}";
			}

			///<summary>Возвращает список игроков с искусственным интеллектом.</summary>
			public static System.Collections.Generic.List<Robot> List => Library.FindAll((Players currentPlayer) => currentPlayer is Robot).ConvertAll((Players currentPlayer) => (Robot)currentPlayer);
		}
	}
}