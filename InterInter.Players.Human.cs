using System.Linq;
using System.Numerics;

namespace IntergalacticInterceptors
{
	partial class Players
	{
		internal sealed class Human : Players
		{
			internal bool MachineGun_PlasmaGun, RocketLauncher_GrenadeLauncher;

			public Human(string newName, int level) : base(newName)
			{
				this.Fraction = Fractions.Humans;
				this.Status.Level = level;
				this.StateLoad();
				this.ReloadAmmunition(Weapons.Arsenal.None);
			}

			public override void Interact(Imitator.Common.Entity agent, params object[] args)
			{
				InterInter.Game_CameraShaking = new Vector3(InterInter.Randomizer.Next(2) == 0 ? -10 : +10, InterInter.Randomizer.Next(2) == 0 ? -10 : +10, 0);
			}

			public override void Update()
			{
				if (InterInter.Gameplay is Gameplay.Galaxian && this.Ship != null && !this.Ship.Dead && Ships.Collection.Contains(this.Ship))
				{
					if (My.Settings.ControlType == 0)
						Control_KeyboardMove_MouseAim();
					else
						Control_MouseMove_KeyboardRotate();

					if (Variants.Imitator.Input.MouseWheel() == 1) this.MachineGun_PlasmaGun = !this.MachineGun_PlasmaGun;
					if (Variants.Imitator.Input.MouseWheel() == -1) this.RocketLauncher_GrenadeLauncher = !this.RocketLauncher_GrenadeLauncher;
					Ships.Stinger stinger = (Ships.Stinger)this.Ship;
					stinger.Control_PrimaryFire = Variants.Imitator.Input.MouseButton(Variants.Imitator.Input.MouseButtons.Left, 0) ? (this.MachineGun_PlasmaGun ? Weapons.Arsenal.PlasmaGun : Weapons.Arsenal.MachineGun) : Weapons.Arsenal.None;
					stinger.Control_SecondaryFire = Variants.Imitator.Input.MouseButton(Variants.Imitator.Input.MouseButtons.Right, 0) ? (this.RocketLauncher_GrenadeLauncher ? Weapons.Arsenal.GrenadeLauncher : Weapons.Arsenal.RocketLauncher) : Weapons.Arsenal.None;
				}
			}

			///<summary>Тип управления: мышь - перемещение, клавиатура - вращение.</summary>
			private void Control_MouseMove_KeyboardRotate()
			{
				Vector2 mouse = Variants.Imitator.Input.MouseVelocity() * My.Settings.MouseSensitivity;
				this.Ship.Physic.Node.Velocity.X = mouse.X;
				this.Ship.Physic.Node.Velocity.Z = mouse.Y;

				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.A))
					this.Ship.Physic.Node.Rotation.Y = 1f;
				else if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.D))
					this.Ship.Physic.Node.Rotation.Y = -1f;
				else
					this.Ship.Physic.Node.Rotation.Y = 0.0F;

				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.W))
					Entities.Sight.Distance = System.Math.Min(200, Entities.Sight.Distance + 5.0F);
				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.S))
					Entities.Sight.Distance = System.Math.Max(20, Entities.Sight.Distance - 5.0F);

				Vector3 viewNormal = Vector3.Transform(-Vector3.UnitZ, this.Ship.Physic.Node.Orientation);
				this.Ship.Physic.Node.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)System.Math.Atan2(-viewNormal.X, -viewNormal.Z)) *
					Quaternion.CreateFromAxisAngle(Vector3.UnitX, System.Math.Max(-1, System.Math.Min(+1, -this.Ship.Physic.Node.Velocity.Z / (My.Settings.MouseSensitivity * 10)))) *
					Quaternion.CreateFromAxisAngle(Vector3.UnitZ, System.Math.Max(-1, System.Math.Min(+1, this.Ship.Physic.Node.Velocity.X / (My.Settings.MouseSensitivity * 10))));

				Entities.Sight.Position3D = this.Ship.Physic.Node.Position;
			}

			///<summary>Тип управления: клавиатура - перемещение, мышь - прицеливание.</summary>
			private void Control_KeyboardMove_MouseAim()
			{
				Vector2 mouse = Variants.Imitator.Input.MouseVelocity() * My.Settings.MouseSensitivity;
				Entities.Sight.Position2D = new Vector3(mouse.X * 5, mouse.Y * 5, 0f) * (float)Variants.Imitator.Physics.ElapsedTime.TotalSeconds;

				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.A))
					this.Ship.Physic.Node.Velocity.X = -100;
				else if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.D))
					this.Ship.Physic.Node.Velocity.X = 100;
				else
					this.Ship.Physic.Node.Velocity.X = 0f;

				if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.W))
					this.Ship.Physic.Node.Velocity.Z = -100;
				else if (Variants.Imitator.Input.KeyboardButton(System.Windows.Forms.Keys.S))
					this.Ship.Physic.Node.Velocity.Z = 100;
				else
					this.Ship.Physic.Node.Velocity.Z = 0f;

				Vector3 viewNormal = this.Ship.Physic.Node.Position - Entities.Sight.Position3D;
				this.Ship.Physic.Node.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)System.Math.Atan2(-viewNormal.X, -viewNormal.Z));
			}

			//public System.Drawing.Color ЦветПуль
			//{
			//	get
			//	{
			//		int value = System.Math.Max(0, System.Math.Min(System.Byte.MaxValue, (int)(this.RestAndRate[Weapons.Arsenal.MachineGun].Item1 * System.Byte.MaxValue / (this[Weapons.Arsenal.MachineGun].GetSpecifications.Stability + 1))));
			//		return System.Drawing.Color.FromArgb(System.Byte.MaxValue - value, value, 0);
			//	}
			//}

			//public System.Drawing.Color ЦветПлазмы
			//{
			//	get
			//	{
			//		int value = System.Math.Max(0, System.Math.Min(System.Byte.MaxValue, (int)(this.RestAndRate[Weapons.Arsenal.PlasmaGun].Item1 * System.Byte.MaxValue / (this[Weapons.Arsenal.PlasmaGun].GetSpecifications.Stability + 1))));
			//		return System.Drawing.Color.FromArgb(System.Byte.MaxValue - value, value, 0);
			//	}
			//}

			///<summary>Возвращает список людей-игроков.</summary>
			public static System.Collections.Generic.List<Human> List => Library.FindAll((Players currentPlayer) => currentPlayer is Human).ConvertAll((Players currentPlayer) => (Human)currentPlayer);
		}
	}
}