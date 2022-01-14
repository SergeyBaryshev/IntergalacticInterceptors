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
		internal sealed class PlasmaGun : Weapons
		{
			///<summary>Генерация экземпляра.</summary>
			///<param name="level">Минимальный уровень.</param>
			internal PlasmaGun(int level) : base(Generate(level)) { }

			///<summary>Загрузка экземпляра.</summary>
			///<param name="specs">Спецификация.</param>
			internal PlasmaGun(Specifications specs) : base(specs) { }

			public static void Shoot(Ships ship)
			{
				if (ship.Player.CheckAmmunition(Arsenal.PlasmaGun))
				{
					Enum_PlasmaGun plasma = (Enum_PlasmaGun)ship.Player[Arsenal.PlasmaGun].GetSpecifications.Type;
					if (plasma == Enum_PlasmaGun.Spray)
						_ = new Projectiles.Spray(ship);
				}
			}

			///<summary>Генерирует новый экземпляр плазмомёта по заданому уровню редкости.</summary>
			private static Specifications Generate(int level)
			{
				Specifications Generate = default;
				Generate.Class = Arsenal.PlasmaGun;
				Generate.Type = InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_PlasmaGun)).Length - 1) + 1;
				Generate.Description = (Enum_Description)InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_Description)).Length);
				Generate.Level = level;
				Generate.Rarity = GenerateRarity(ref level);
				if (Generate.Type == (int)Enum_PlasmaGun.Spray)
				{
					Generate.Damage = InterInter.Randomizer.Next(10, 20);
					Generate.Rate = InterInter.Randomizer.Next(10, 20);
					Generate.Velocity = InterInter.Randomizer.Next(200, 300);
					Generate.Capacity = InterInter.Randomizer.Next(4) * 50 + 300;
				}
				//else if (Generate.Type == (int)Enum_PlasmaGun.Ricochet)
				//{
				//	Generate.Damage = InterInter.Randomizer.Next(20, 40);
				//	Generate.Rate = InterInter.Randomizer.Next(1, 4);
				//	Generate.Velocity = InterInter.Randomizer.Next(50, 100);
				//	Generate.Capacity = InterInter.Randomizer.Next(4) * 10 + 10;
				//}
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