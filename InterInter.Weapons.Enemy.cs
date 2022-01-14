using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IntergalacticInterceptors
{
	partial class Weapons
	{
		public sealed class Enemy : Weapons
		{
			///<summary>Генерация экземпляра.</summary>
			///<param name="level">Минимальный уровень.</param>
			internal Enemy(int level) : base(Generate(level)) { }

			///<summary>Загрузка экземпляра.</summary>
			///<param name="specs">Спецификация.</param>
			internal Enemy(Specifications specs) : base(specs) { }

			public static void Shoot(Ships ship, float angle)
			{
				if (ship.Player.CheckAmmunition(Arsenal.Enemy))
				{
					Enum_Enemy enemy = (Enum_Enemy)ship.Player[Arsenal.Enemy].GetSpecifications.Type;
					if (enemy == Enum_Enemy.Impulse)
						_ = new Projectiles.Impulse(ship, angle);
				}
			}

			///<summary>Генерирует новый экземпляр вражеского вооружения по заданому уровню редкости.</summary>
			private static Specifications Generate(int level)
			{
				Specifications Generate = default;
				Generate.Class = Arsenal.Enemy;
				Generate.Type = InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_Enemy)).Length - 1) + 1;
				Generate.Description = (Enum_Description)InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_Description)).Length);
				Generate.Level = level;
				Generate.Rarity = GenerateRarity(ref level);
				if (Generate.Type == (int)Enum_Enemy.Impulse)
				{
					Generate.Damage = InterInter.Randomizer.Next(10, 20);
					Generate.Damage = (int)(Generate.Damage * (1 + level / 10.0F));
					Generate.Rate = (float)InterInter.Randomizer.NextDouble();
					Generate.Velocity = InterInter.Randomizer.Next(50, 100);
					Generate.Capacity = 100;
					Generate.Strength = 1;
					Generate.Criticality = InterInter.Randomizer.Next(10);
				}
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

			public override void Push(Quaternion orientation, Vector3 position)
			{
				throw new NotImplementedException();
			}
		}
	}
}