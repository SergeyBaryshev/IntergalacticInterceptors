using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntergalacticInterceptors
{
	partial class Weapons
	{
		public sealed class RocketLauncher : Weapons
		{
			///<summary>Генерация экземпляра.</summary>
			///<param name="level">Минимальный уровень.</param>
			internal RocketLauncher(int level) : base(Generate(level)) { }

			///<summary>Загрузка экземпляра.</summary>
			///<param name="specs">Спецификация.</param>
			internal RocketLauncher(Specifications specs) : base(specs) { }

			public static void Shoot(Ships ship)
			{
				if (ship.Player.CheckAmmunition(Arsenal.RocketLauncher))
				{
					Enum_RocketLauncher rocket = (Enum_RocketLauncher)ship.Player[Arsenal.RocketLauncher].GetSpecifications.Type;
					if (rocket == Enum_RocketLauncher.Spread)
						_ = new Projectiles.Spread(ship);
					else if (rocket == Enum_RocketLauncher.Homing)
						_ = new Projectiles.Homing(ship);
					else if (rocket == Enum_RocketLauncher.Torpedo)
						_ = new Projectiles.Torpedo(ship);
					else if (rocket == Enum_RocketLauncher.Tracer)
						_ = new Projectiles.Tracer(ship);
				}
			}

			///<summary>Генерирует новый экземпляр ракетомёта по заданому уровню редкости.</summary>
			private static Specifications Generate(int level)
			{
				Specifications Generate = default;
				Generate.Class = Arsenal.RocketLauncher;
				Generate.Type = InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_RocketLauncher)).Length - 1) + 1;
				Generate.Description = (Enum_Description)InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_Description)).Length);
				Generate.Level = level;
				Generate.Rarity = GenerateRarity(ref level);
				if (Generate.Type == (int)Enum_RocketLauncher.Torpedo)
				{
					Generate.Damage = InterInter.Randomizer.Next(10, 40);
					Generate.Rate = InterInter.Randomizer.Next(1, 4);
					Generate.Velocity = InterInter.Randomizer.Next(100, 200);
					Generate.Capacity = InterInter.Randomizer.Next(4) * 25 + 100;
				}
				else if (Generate.Type == (int)Enum_RocketLauncher.Homing)
				{
					Generate.Damage = InterInter.Randomizer.Next(20, 40);
					Generate.Rate = InterInter.Randomizer.Next(1, 4);
					Generate.Velocity = InterInter.Randomizer.Next(100, 150);
					Generate.Capacity = InterInter.Randomizer.Next(2) * 100 + 200;
				}
				else if (Generate.Type == (int)Enum_RocketLauncher.Spread)
				{
					Generate.Damage = InterInter.Randomizer.Next(5, 10);
					Generate.Rate = InterInter.Randomizer.Next(5, 10);
					Generate.Velocity = InterInter.Randomizer.Next(100) + 200;
					Generate.Capacity = InterInter.Randomizer.Next(2) * 100 + 300;
				}
				else if (Generate.Type == (int)Enum_RocketLauncher.Tracer)
				{
					Generate.Damage = InterInter.Randomizer.Next(20, 40);
					Generate.Rate = InterInter.Randomizer.Next(1, 4);
					Generate.Velocity = InterInter.Randomizer.Next(100, 150);
					Generate.Capacity = InterInter.Randomizer.Next(4) * 25 + 50;
				}
				Generate.Damage = (int)(Generate.Damage * (1 + level / 10.0F));
				Generate.Strength = InterInter.Randomizer.Next(10);
				Generate.Criticality = InterInter.Randomizer.Next(10);
				return Generate;
			}

			public override void Dispose()
			{
			}

			public override void Peek()
			{
				throw new NotImplementedException();
			}

			public override void Pop()
			{
				throw new NotImplementedException();
			}

			public override void Push(System.Numerics.Quaternion orientation, System.Numerics.Vector3 position)
			{
				throw new NotImplementedException();
			}
		}
	}
}