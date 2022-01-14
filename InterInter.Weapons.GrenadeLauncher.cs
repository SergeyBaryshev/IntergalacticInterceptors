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
		public sealed class GrenadeLauncher : Weapons
		{
			///<summary>Генерация экземпляра.</summary>
			///<param name="level">Минимальный уровень.</param>
			internal GrenadeLauncher(int level) : base(Generate(level)) { }

			///<summary>Загрузка экземпляра.</summary>
			///<param name="specs">Спецификация.</param>
			internal GrenadeLauncher(Specifications specs) : base(specs) { }

			public static void Shoot(Ships ship)
			{
				if (ship.Player.CheckAmmunition(Arsenal.GrenadeLauncher))
				{
					Enum_GrenadeLauncher grenade = (Enum_GrenadeLauncher)ship.Player[Arsenal.GrenadeLauncher].GetSpecifications.Type;
					if (grenade == Enum_GrenadeLauncher.Bomb)
					{ }
				}
			}

			///<summary>Генерирует новый экземпляр гранатомёта по заданому уровню редкости.</summary>
			private static Specifications Generate(int level)
			{
				Specifications Generate = default;
				Generate.Class = Arsenal.GrenadeLauncher;
				Generate.Type = InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_GrenadeLauncher)).Length - 1) + 1;
				Generate.Description = (Enum_Description)InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_Description)).Length);
				Generate.Level = level;
				Generate.Rarity = GenerateRarity(ref level);
				if (Generate.Type == (int)Enum_GrenadeLauncher.Bomb)
				{
					Generate.Damage = InterInter.Randomizer.Next(100, 200);
					Generate.Rate = InterInter.Randomizer.Next(1, 6) / 10.0F;
					Generate.Velocity = InterInter.Randomizer.Next(100, 150);
					Generate.Capacity = InterInter.Randomizer.Next(3) * 5 + 5;
				}
				else if (Generate.Type == (int)Enum_GrenadeLauncher.Mine)
				{
					Generate.Damage = InterInter.Randomizer.Next(100, 200);
					Generate.Rate = InterInter.Randomizer.Next(5, 10) / 10.0F;
					Generate.Velocity = InterInter.Randomizer.Next(1000, 1500);
					Generate.Capacity = InterInter.Randomizer.Next(3) * 100 + 100;
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

			public override void Push(Quaternion orientation, Vector3 position)
			{
				throw new NotImplementedException();
			}
		}
	}
}