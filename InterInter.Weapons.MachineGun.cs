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
		internal sealed class MachineGun : Weapons
		{
			///<summary>Генерация экземпляра.</summary>
			///<param name="level">Минимальный уровень.</param>
			internal MachineGun(int level) : base(Generate(level)) { }

			///<summary>Загрузка экземпляра.</summary>
			///<param name="specs">Спецификация.</param>
			internal MachineGun(Specifications specs) : base(specs) { }

			public static void Shoot(Ships ship)
			{
				if (ship.Player.CheckAmmunition(Arsenal.MachineGun))
				{
					Enum_MachineGun machine = (Enum_MachineGun)ship.Player[Arsenal.MachineGun].GetSpecifications.Type;
					ship.Emitter.SoundPlay(System.IO.Path.Combine(InterInter.RootPath, nameof(Projectiles), machine.ToString("G"), "Shot.wav"));
					Imitator.Common.Bullet.Create(System.IO.Path.Combine(InterInter.RootPath, "Particles", "Bullet.bmp"), ship.Physic.Node.Position, Entities.Sight.Position3D, 1000, 1);

					if (Entities.Sight.Contacts != null && (machine == Enum_MachineGun.Minigun | machine == Enum_MachineGun.Sniper))
					{
						foreach (Variants.Imitator.Engine.Contact contact in Entities.Sight.Contacts)
						{
							if (contact.Node?.BaseObject != null)
							{
								if (Imitator.Common.Entity.Item(contact.Node.BaseObject.Name) is Ships.Enemy target)
									target.Interact(ship, contact, Weapons.Arsenal.MachineGun);
								//TODO: сделать по одной искре на контакт.
							}
						}
					}
				}
			}

			///<summary>Генерирует новый экземпляр пулемёта по заданому уровню редкости.</summary>
			private static Specifications Generate(int level)
			{
				Specifications Generate = default;
				Generate.Class = Arsenal.MachineGun;
				Generate.Type = InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_MachineGun)).Length - 1) + 1;
				Generate.Description = (Enum_Description)InterInter.Randomizer.Next(System.Enum.GetNames(typeof(Enum_Description)).Length);
				Generate.Level = level;
				Generate.Velocity = 0;
				Generate.Rarity = GenerateRarity(ref level);
				if (Generate.Type == (int)Enum_MachineGun.Minigun)
				{
					Generate.Damage = InterInter.Randomizer.Next(10, 20);
					Generate.Rate = InterInter.Randomizer.Next(5, 10);
					Generate.Capacity = InterInter.Randomizer.Next(4) * 50 + 100;
				}
				else if (Generate.Type == (int)Enum_MachineGun.Sniper)
				{
					Generate.Damage = InterInter.Randomizer.Next(100, 200);
					Generate.Rate = InterInter.Randomizer.Next(1, 6) / 10.0F;
					Generate.Capacity = InterInter.Randomizer.Next(4) * 10 + 10;
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