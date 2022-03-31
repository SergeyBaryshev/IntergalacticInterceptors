using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace IntergalacticInterceptors
{
	partial class Ships
	{
		///<summary>Корабль игрока.</summary>
		internal sealed class Stinger : Ships
		{
		internal Weapons.Arsenal Control_PrimaryFire, Control_SecondaryFire;

			internal Stinger(Players player, System.Numerics.Quaternion orientation, System.Numerics.Vector3 position) : base(player)
			{
				this.Health = Players.CalculateHealth(this.Player.Status.Level, 0);

				this.Physic = Variants.Imitator.Scene.Body.Add(nameof(Physic) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, nameof(Ships), nameof(Stinger), nameof(Physic) + ".obj", nameof(Stinger) + "_" + nameof(Physic)), orientation, position, 10);
				if (this.Physic != null)
					this.Physic.Node.Gravity = System.Numerics.Vector3.Zero;

				this.Render = Variants.Imitator.Scene.Body.Add(nameof(Render) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, nameof(Ships), nameof(Stinger), nameof(Render) + ".obj", nameof(Stinger) + "_" + nameof(Render)), orientation, position);
				if (this.Render != null)
				{
					this.Render.Node.ParentNode = this.Physic.Node;
					Imitator.Common.Intelligence.SetNoClipAndNullGravity(this.Render, true);
				}

				this.Attach = Variants.Imitator.Scene.Body.Add(nameof(Attach) + this.UniqueID, System.IO.Path.Combine(InterInter.RootPath, nameof(Ships), nameof(Stinger), nameof(Attach) + ".obj", "Gun_" + nameof(Attach)), orientation, position, 1);
				if (this.Attach != null)
				{
					this.Attach.Node.ParentNode = this.Render.Node;
					Imitator.Common.Intelligence.SetNoClipAndNullGravity(this.Attach, true);
				}

				this.Emitter = Variants.Imitator.Scene.Emitter.Add(nameof(Emitter) + this.UniqueID, orientation, position);
				if (this.Emitter != null)
					this.Emitter.Node.ParentNode = this.Render.Node;
			}

			internal override void Dispose()
			{
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
						this.Dead = true;
						this.Health = 1f + (float)InterInter.Randomizer.NextDouble() * 3f;
						this.Render.Node.ParentNode = null;
						this.Render.Node.Rotation = new Vector3((float)InterInter.Randomizer.NextDouble() * 2 - 1, (float)InterInter.Randomizer.NextDouble() * 2 - 1, (float)InterInter.Randomizer.NextDouble() * 2 - 1);
						this.Render.Node.Velocity = new Vector3((float)InterInter.Randomizer.NextDouble() * 2 - 1, (float)InterInter.Randomizer.NextDouble() * 2 - 1, (float)InterInter.Randomizer.NextDouble() * 2 - 1) * 100;
						Imitator.Common.Intelligence.SetNoClipAndNullGravity(this.Render, false);
						this.Render.TotalMass = this.Physic.TotalMass;
						{// some tests...
							Variants.Imitator.Scene.Trigger tr = Variants.Imitator.Scene.Trigger.Item("Explosion");
							Variants.Imitator.Engine.Particle p = new Variants.Imitator.Engine.Particle() { Index = 10 };
							tr.Execute("Create", p);
							p.ToString();
						}
					}
				}
			}

			internal override void Update()
			{
				base.GeneralBehavior();
				if (!this.Dead)
				{
					System.Collections.Generic.List<Variants.Imitator.Engine.Contact> contacts = this.Physic.Node.Contacts();
					if (contacts != null)
					{
						foreach (Variants.Imitator.Engine.Contact contact in contacts)
						{
							if (contact.Node?.BaseObject != null)
							{
								if (Imitator.Common.Entity.Item(contact.Node.BaseObject.Name) is Ships.Enemy target)
									this.Interact(target, contact, Weapons.Arsenal.Enemy);
								break;
							}
						}
					}

					if (!WeaponFire(this.Control_PrimaryFire) && !WeaponFire(this.Control_SecondaryFire))
						this.Player.UpdateAmmunition();

					if (InterInter.Gameplay is Gameplay.Galaxian)
					{
						UpdateGalaxian();
					}
				}
			}

			private bool WeaponFire(Weapons.Arsenal weapClass)
			{
				switch (weapClass)
				{
					case Weapons.Arsenal.MachineGun:
						Weapons.MachineGun.Shoot(this);
						return true;
					case Weapons.Arsenal.PlasmaGun:
						Weapons.PlasmaGun.Shoot(this);
						return true;
					case Weapons.Arsenal.RocketLauncher:
						Weapons.RocketLauncher.Shoot(this);
						return true;
					case Weapons.Arsenal.GrenadeLauncher:
						Weapons.GrenadeLauncher.Shoot(this);
						return true;
				}
				return false;
			}

			private void UpdateGalaxian()
			{
				this.Physic.Node.Position.X = System.Math.Max(Gameplay.Galaxian.BattleField.Left, System.Math.Min(Gameplay.Galaxian.BattleField.Right, this.Physic.Node.Position.X));
				this.Physic.Node.Position.Z = System.Math.Max(Gameplay.Galaxian.BattleField.Top, System.Math.Min(Gameplay.Galaxian.BattleField.Bottom, this.Physic.Node.Position.Z));
				this.Physic.Node.Position.Y = 0;
				this.Physic.Node.Rotation = System.Numerics.Vector3.Zero;
			}

			///<summary>Возвращает список кораблей игрока.</summary>
			public new static System.Collections.Generic.List<Stinger> List => Imitator.Common.Entity.Collection.OfType<Stinger>().ToList();
		}
	}
}