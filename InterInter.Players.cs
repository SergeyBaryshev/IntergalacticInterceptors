namespace IntergalacticInterceptors
{
	internal abstract partial class Players : Variants.Imitator.Engine.BaseObject, System.IDisposable
	{
		///<summary>Внутренний список игроков.</summary>
		private static readonly System.Collections.Generic.List<Players> Library = new System.Collections.Generic.List<Players>();
		///<summary>Внешний список игроков.</summary>
		internal static readonly System.Collections.ObjectModel.ReadOnlyCollection<Players> Collection = new System.Collections.ObjectModel.ReadOnlyCollection<Players>(Library);
		///<summary>Внутренний инвентарь вооружения.</summary>
		private readonly System.Collections.Generic.List<Weapons> Inventory = new System.Collections.Generic.List<Weapons>();
		///<summary>Внешний инвентарь вооружения.</summary>
		internal readonly System.Collections.ObjectModel.ReadOnlyCollection<Weapons> Weaponry;

		///<summary>Стороны конфликта.</summary>
		internal enum Fractions : int
		{
			///<summary>Чужие.</summary>
			Aliens,
			///<summary>Люди.</summary>
			Humans
		}
				
		///<summary>Предпочтительная для игрока фракция.</summary>
		internal Fractions Fraction;
		/// <summary>Псевдоним игрока.</summary>
		private string NickName;
		///<summary>Ссылка на подчинённый корабль.</summary>
		///<remarks>Может быть <see langword="null"/>.</remarks>
		internal Ships Ship;
		///<summary>Сохраняемая статистика.</summary>
		internal PlayerState Status;

		private static readonly System.Action AutoStart = AutoStartNew();
		private static System.Action AutoStartNew()
		{
			_ = new Human(System.Environment.UserName, 0);
			var autoStartNew = new System.Action(AutoUpdate);
			Variants.Imitator.Maths.AddClientMethod(autoStartNew);
			return autoStartNew;
		}

		private static void AutoUpdate()
		{
			if (AutoStart != null)
			{
				Library.RemoveAll((Players player) => { player.Update(); return false; });
			}
		}

		///<summary>Инициализация по умолчанию.</summary>
		internal Players(string nickname)
		{
			this.NickName = nickname;
			this.Status.Credits = 1000;
			this.Status.Specifications = new Weapons.Specifications[] { };
			this.Status.Installed = new int[System.Enum.GetNames(typeof(Weapons.Arsenal)).Length - 1];
			this.Status.Experience = 0;
			this.Status.Level = 0;
			this.Weaponry = new System.Collections.ObjectModel.ReadOnlyCollection<Weapons>(Inventory);
			Library.Add(this);
		}

		///<summary>Для сохранения используется именно эта структура.</summary>
		//[Serializable]
		internal struct PlayerState
		{
			///<summary>Сохраняемые деньги.</summary>
			public int Credits;
			///<summary>Сохраняемый опыт.</summary>
			public int Experience;
			///<summary>Сохраняемый уровень.</summary>
			public int Level;
			///<summary>Сохраняемые спецификации.</summary>
			public Weapons.Specifications[] Specifications;
			///<summary>Сохраняемое смонтированное вооружение.</summary>
			public int[] Installed;
		}

		internal void StateLoad()
		{
			PlayerState status = Variants.Imitator.Content.Deserialize<PlayerState>(My.Settings.State);
			if (status.Specifications != null)
			{
				this.Status = status;
				foreach (Weapons.Specifications specs in this.Status.Specifications)
				{
					if (specs.Class == Weapons.Arsenal.MachineGun)
						this.Inventory.Add(new Weapons.MachineGun(specs));
					else if (specs.Class == Weapons.Arsenal.PlasmaGun)
						this.Inventory.Add(new Weapons.PlasmaGun(specs));
					else if (specs.Class == Weapons.Arsenal.RocketLauncher)
						this.Inventory.Add(new Weapons.RocketLauncher(specs));
					else if (specs.Class == Weapons.Arsenal.GrenadeLauncher)
						this.Inventory.Add(new Weapons.GrenadeLauncher(specs));
					else if (specs.Class == Weapons.Arsenal.Robot)
						this.Inventory.Add(new Weapons.Robot(specs));
				}
			}
		}

		internal void StateSave()
		{
			this.Status.Specifications = this.Inventory.ConvertAll((Weapons currentWeapon) => currentWeapon.GetSpecifications).ToArray();
			My.Settings.State = Variants.Imitator.Content.Serialize(this.Status);
		}

		///<summary>Проверка остатков и охлаждения вооружения.</summary>
		public bool CheckAmmunition(Weapons.Arsenal weapClass)
		{
			Weapons instance = this[weapClass];
			if (instance.Capacity > 0 && instance.Recovery < (float)Variants.Imitator.Physics.CurrentTime.TotalSeconds - 1.0F / instance.GetSpecifications.Rate)
			{
				instance.Capacity -= 1;
				instance.Recovery = (float)Variants.Imitator.Physics.CurrentTime.TotalSeconds;
				return true;
			}
			return false;
		}

		///<summary>Перезарядка вооружения.</summary>
		public void ReloadAmmunition(Weapons.Arsenal weapClass, int amount = int.MaxValue)
		{
			Weapons instance;
			if (weapClass == Weapons.Arsenal.None)
				for (Weapons.Arsenal index = Weapons.Arsenal.MachineGun; index <= Weapons.Arsenal.GrenadeLauncher; index += 1)
				{
					instance = this[index];
					if (instance != null)
					{
						instance.Capacity = System.Math.Min(instance.GetSpecifications.Capacity, amount);
						instance.Recovery = 0;
					}
				}
			else
			{
				instance = this[weapClass];
				instance.Capacity = System.Math.Min(instance.GetSpecifications.Capacity, instance.Capacity + amount);
				instance.Recovery = 0;
			}
		}

		///<summary>Обновление состояния вооружения игрока.</summary>
		public void UpdateAmmunition()
		{
			foreach (Weapons.Arsenal index in new Weapons.Arsenal[] { Weapons.Arsenal.MachineGun, Weapons.Arsenal.PlasmaGun, Weapons.Arsenal.Enemy })
			{
				Weapons instance = this[index];
				if (instance != null)
					instance.Capacity = System.Math.Min(instance.GetSpecifications.Capacity, instance.Capacity + (float)Variants.Imitator.Physics.ElapsedTime.TotalSeconds * instance.GetSpecifications.Rate);
			}
		}

		///<summary>Реакция игрока на воздействие агента.</summary>
		public abstract void Interact(Imitator.Common.Entity agent, params object[] args);
		///<summary>Осуществляет управление персонажа игроком.</summary>
		public abstract void Update();

		///<summary>Обновление опыта и уровня игрока.</summary>
		public void UpdateExperience()
		{
			int maxExperience = 1000 * (this.Status.Level + 1);
			this.Status.Level += (int)(System.Math.Truncate((float)(this.Status.Experience / maxExperience)));
			this.Status.Experience %= maxExperience;
		}

		public static int CalculateHealth(int level, int extra)
		{
			return level * (10 + InterInter.Randomizer.Next(extra)) + 100;
		}

		///<summary>Получает или назначает в указанной ячейке смонтированное вооружение.</summary>
		public Weapons this[Weapons.Arsenal weapClass]
		{
			get
			{
				if (weapClass != Weapons.Arsenal.None && this.Inventory.Count > 0)
				{
					int index = this.Status.Installed[(int)weapClass - 1];
					if (index >= 0 && index < this.Inventory.Count)
					{
						Weapons instance = this.Inventory[index];
						if (instance.GetSpecifications.Class == weapClass)
							return instance;
					}
				}
				return null;
			}
			set
			{
				int index = this.Inventory.IndexOf(value);
				if (index != -1 && value.GetSpecifications.Class == weapClass && value.GetSpecifications.Level <= this.Status.Level)
					this.Status.Installed[(int)weapClass - 1] = index;
			}
		}

		///<summary>Добавление единицы вооружения в инвентарь игрока, назначение его выбранным для вооружения.</summary>
		public void Add(Weapons value)
		{
			this.Inventory.Add(value);
			Weapons current = this[value.GetSpecifications.Class];
			if (current == null || current.GetSpecifications.Class == Weapons.Arsenal.None)
				this.Status.Installed[(int)value.GetSpecifications.Class - 1] = this.Inventory.Count - 1;
		}

		///<summary>Удаление единицы вооружения из инвентаря игрока, снятие с вооружения на предыдущее.</summary>
		public void Remove(Weapons value)
		{
			int index = this.Inventory.IndexOf(value);
			this.Inventory.Remove(value);
			for (Weapons.Arsenal eachClass = Weapons.Arsenal.MachineGun; eachClass < Weapons.Arsenal.Robot; eachClass += 1)
				if (index < this.Status.Installed[(int)eachClass - 1])
					this.Status.Installed[(int)eachClass - 1] -= 1;
		}

		public void Dispose()
		{
			Library.Remove(this);
		}

		public override string ToString()
		{
			return this.NickName;
		}
	}
}