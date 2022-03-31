using System.Linq;
using System.Numerics;

namespace IntergalacticInterceptors
{
	///<summary>Основа для всех кораблей.</summary>
	internal abstract partial class Ships : Imitator.Common.Entity
	{
		///<summary>Игрок, управляющий данным кораблём.</summary>
		internal readonly Players Player;
		///<summary>Является ли корабль уничтоженным.</summary>
		internal bool Dead;

		internal Ships(Players player)
		{
			if (player != null)
			{
				this.Player = player;
				this.Player.Ship = this;
			}
		}

		///<summary>Вопроизводит звуки ущерба в зависимости от стороны конфликта.</summary>
		internal void DamageSounds()
		{
			if (this is Ships.Enemy)
				this.Emitter.SoundPlay(System.IO.Path.Combine(InterInter.RootPath, nameof(Ships), "ricochet." + InterInter.Randomizer.Next(3) + ".wav"));
			else if (this is Ships.Stinger)
				this.Emitter.SoundPlay(System.IO.Path.Combine(InterInter.RootPath, nameof(Ships), "damage." + InterInter.Randomizer.Next(5) + ".wav"));
		}

		///<summary>Вопроизводит звуки взрыва в зависимости от стороны конфликта.</summary>
		internal void ExplosionSounds()
		{
			if (this is Ships.Enemy)
				this.Emitter.SoundPlay(System.IO.Path.Combine(InterInter.RootPath, "Particles", "Explosion." + InterInter.Randomizer.Next(4) + ".wav"), false);
			else if (this is Ships.Stinger)
				this.Emitter.SoundPlay(System.IO.Path.Combine(InterInter.RootPath, "Particles", "explobig.wav"), false);
		}

		///<summary>Проверка состояния корабля на различные обстоятельства.</summary>
		///<remarks>
		///1. Если игрок не является участником игры, корабль удаляется.<para/>
		///2. Контроль уничтоженного, но всё ещё существующего корабля.<para/>
		///</remarks>
		internal void GeneralBehavior()
		{
			if (this.Player != null && !Players.Collection.Contains(this.Player))
			{
				if (!this.Dead)
					this.Health = 1f;
				this.Dead = true;
			}
			if (this.Dead)
			{
				this.Health -= (float)Variants.Imitator.Physics.ElapsedTime.TotalSeconds;
				if (this.Physic != null)
				{
					Variants.Imitator.Scene.Body.Remove(this.Physic.Name);
					this.Physic = null;
					this.ExplosionSounds();
				}
			}
		}

		///<summary>Присоединяемый предмет.</summary>
		public Variants.Imitator.Scene.Body Attach
		{
			get
			{
				return this.GetObject<Variants.Imitator.Scene.Body>(nameof(Attach));
			}
			set
			{
				this.SetObject<Variants.Imitator.Scene.Body>(nameof(Attach), value);
			}
		}

		///<summary>Возвращает список кораблей врага.</summary>
		public static System.Collections.Generic.List<Ships> List => Imitator.Common.Entity.Collection.OfType<Ships>().ToList();
	}
}