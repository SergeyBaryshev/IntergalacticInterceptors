﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IntergalacticInterceptors
{
	partial class Weapons
	{
		public sealed class Robot : Weapons
		{
			///<summary>Генерация экземпляра.</summary>
			///<param name="level">Минимальный уровень.</param>
			internal Robot(int level) : base(Generate(level)) { }

			///<summary>Загрузка экземпляра.</summary>
			///<param name="specs">Спецификация.</param>
			internal Robot(Specifications specs) : base(specs) { }

			public static void Shoot(Ships ship)
			{
				if (ship.Player.CheckAmmunition(Arsenal.Robot))
				{
					//Enum_Robot robot = (Enum_Robot)ship.Player[Enum_WeaponClass.Robot].Вид; 
				}
			}

			///<summary>Генерирует новый экземпляр робота по заданому уровню редкости.</summary>
			private static Specifications Generate(int level)
			{
				Specifications Generate = default;
				Generate.Class = Arsenal.Robot;
				Generate.Type = InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_Robot)).Length - 1) + 1;
				Generate.Description = (Enum_Description)InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_Description)).Length);
				Generate.Level = level;
				Generate.Rarity = GenerateRarity(ref level);
				if (Generate.Type == (int)Enum_Robot.Shield)
				{
					Generate.Damage = InterInter.Randomizer.Next(100, 200);
					Generate.Damage = (int)(Generate.Damage * (1 + level / 10.0F));
					Generate.Strength = InterInter.Randomizer.Next(10);
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