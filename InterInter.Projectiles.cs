using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IntergalacticInterceptors
{
	///<summary>Представляет из себя экземпляр снаряда в сцене.</summary>
	internal abstract partial class Projectiles : Imitator.Common.Entity
	{
		///<summary>Владелец снаряда.</summary>
		internal readonly Ships Owner;
		///<summary>Класс оружия.</summary>
		internal readonly Weapons.Arsenal WeaponClass;
		///<summary>Расстояние появления снаряда от центра корабля.</summary>
		public float Distance { get; set; } = 20.0F;

		///<summary>Выстрел снаряда.</summary>
		///<param name="ship">Кто стреляет?</param>
		///<param name="weapClass">Чем стреляет?</param>
		///<param name="angle">Угол отклонения выстрела.</param>
		internal Projectiles(Ships ship, Weapons.Arsenal weapClass, float angle = 0F)
		{
			this.Owner = ship;
			this.WeaponClass = weapClass;
			Weapons currentWeapon = this.Owner.Player[this.WeaponClass];
			string current= Weapons.GetWeaponType(this.WeaponClass, currentWeapon.GetSpecifications.Type).ToString("G");
			int knockback = System.Math.Max(1, currentWeapon.GetSpecifications.Strength);
			Quaternion orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, angle); 
			if (this.Owner is Ships.Stinger)
			{
				this.Health = 2;
				orientation = this.Owner.Physic.Node.Orientation * orientation;
			}
			else if (this.Owner is Ships.Enemy)
			{
				this.Health = 3;
			}
			else
				return;

			Vector3 normalView = Vector3.Transform(Vector3.UnitZ, orientation);
			Vector3 position = this.Owner.Physic.Node.Position + normalView * this.Distance;

			this.Physic = Variants.Imitator.Scene.Body.Add(nameof(Physic) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, nameof(Projectiles), current, nameof(Physic) + ".obj", "defaultobject"), orientation, position, knockback);
			if (this.Physic != null)
			{
				this.Physic.Node.Gravity = Vector3.Zero;
				this.Physic.Node.Velocity = normalView * currentWeapon.GetSpecifications.Velocity;
			}

			this.Render = Variants.Imitator.Scene.Body.Add(nameof(Render) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, nameof(Projectiles), current, nameof(Render) + ".obj", "defaultobject"), orientation, position);
			if (this.Render != null)
			{
				this.Render.Node.ParentNode = this.Physic.Node;
				Imitator.Common.Intelligence.SetNoClipAndNullGravity(this.Render, true);
			}

			this.Emitter = Variants.Imitator.Scene.Emitter.Add(nameof(Emitter) + this.UniqueID, orientation, position);
			if (this.Emitter != null)
			{
				this.Emitter.Node.ParentNode = this.Render.Node;
				this.Emitter.SoundPlay(System.IO.Path.Combine(InterInter.RootPath, nameof(Projectiles), current, "Shot.wav"));
			}
		}

		///<summary>Общее для всех снарядов поведение.</summary>
		private void CommonBehavior()
		{
			this.Health -= (float)Variants.Imitator.Physics.ElapsedTime.TotalSeconds;
			System.Collections.Generic.List<Variants.Imitator.Engine.Contact> contacts = this.Physic.Node.Contacts();
			if (contacts != null)
			{
				foreach (Variants.Imitator.Engine.Contact contact in contacts)
				{
					if (contact.Node?.BaseObject != null)
					{
						if (Imitator.Common.Entity.Item(contact.Node.BaseObject.Name) is Ships target)
							if ((this.Owner is Ships.Enemy && target is Ships.Stinger) || (this.Owner is Ships.Stinger && target is Ships.Enemy))
								target.Interact(this.Owner, contact, this.WeaponClass);
						this.Health = 0;
						return;
					}
				}
			}
		}

		///<summary>Возвращает список снарядов.</summary>
		public static System.Collections.Generic.List<Projectiles> List => Imitator.Common.Entity.Collection.OfType<Projectiles>().ToList();
	}
}