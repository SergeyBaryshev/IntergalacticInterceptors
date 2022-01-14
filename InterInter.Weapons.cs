using System.Linq;
using System.Numerics;

namespace IntergalacticInterceptors
{
	///<summary>Индивидуальные параметры каждого оружия.</summary>
	internal abstract partial class Weapons : Variants.Imitator.Engine.BaseObject, Imitator.Common.Inventory<Weapons.Arsenal, Weapons.Specifications>.IBehavior
	{
		public Arsenal ItemKey => this.GetSpecifications.Class;
		public (Quaternion, Vector3)? Respawn { get; set; }
		///<summary>Остаток боеприпасов.</summary>
		public float Capacity { get; set; }
		///<summary>Откат нагрева.</summary>
		public float Recovery { get; set; }
		public int Stock { get; set; }

		internal readonly Specifications GetSpecifications;

		internal Weapons(Specifications specs)
		{
			this.GetSpecifications = specs;
			this.Capacity = this.GetSpecifications.Capacity;
			this.Recovery = 0F;
			this.Stock = 0;
		}

		public override string ToString()
		{
			if (this.GetSpecifications.Class == Arsenal.None)
				return Arsenal.None.ToString("G");
			return GetWeaponType(this.GetSpecifications.Class, this.GetSpecifications.Type).ToString("G") + " (Lv." + this.GetSpecifications.Level + ")";
		}

		///<summary>Расчёт ущерба от попадания из оружия от игроков и врагов.</summary>
		internal static float CalculateDamage(Ships ship, Variants.Imitator.Engine.Contact contact, Weapons.Arsenal weapClass)
		{
			if (ship != null && contact.Node?.BaseObject != null && ship.Player[weapClass]!=null)
			{
				if (Imitator.Common.Entity.Item(contact.Node.BaseObject.Name) is Ships target)
					return ship.Player[weapClass].GetSpecifications.Damage + (ship.Player[weapClass].GetSpecifications.Damage * 0.3F * (ship.Player.Status.Level - target.Player.Status.Level));
			}
			return 0;
		}

		internal struct Specifications : Imitator.Common.Inventory<Arsenal, Specifications>.IQuantity
		{
			///<summary>Класс данного оружия.</summary>
			internal Arsenal Class;
			///<summary>Подкласс данного оружия.</summary>
			internal int Type;
			///<summary>Уровень оружия.</summary>
			internal int Level;
			///<summary>Урон оружия (единиц на выстрел).</summary>
			internal int Damage;
			///<summary>Темп оружия (выстрелов в секунду).</summary>
			internal float Rate;
			///<summary>Скорость снаряда (метров в секунду).</summary>
			internal int Velocity;
			///<summary>Убойность оружия (метров за выстрел).</summary>
			internal int Strength;
			///<summary>Критичность оружия (шанс мгновенного убийства).</summary>
			internal int Criticality;
			///<summary>Редкость оружия (коэфф. усиления).</summary>
			internal Enum_Rarity Rarity;
			///<summary>Описание оружия.</summary>
			internal Enum_Description Description;

			///<summary>Боезапас оружия (расход на выстрел).</summary>
			public float Capacity { get; set; }
			///<summary>Цена оружия (в §).</summary>
			public int Stock { get { return (int)(this.Damage * this.Rate * 10 + this.Capacity + this.Strength * 10); } set { throw new System.NotImplementedException(); } }

			public void Dispose()
			{
				throw new System.NotImplementedException();
			}
		}

		///<summary>Генерирует уровень редкости.</summary>
		private static Enum_Rarity GenerateRarity(ref int level)
		{
			int rnd = InterInter.Randomizer.Next(100);
			if (rnd > 95)
			{
				level += 4;
				return Enum_Rarity.Magenta;
			}
			else if (rnd > 90)
			{
				level += 3;
				return Enum_Rarity.Orange;
			}
			else if (rnd > 85)
			{
				level += 2;
				return Enum_Rarity.Green;
			}
			else if (rnd > 80)
			{
				level += 1;
				return Enum_Rarity.Blue;
			}
			else if (rnd < 5)
			{
				level -= 1;
				return Enum_Rarity.Gray;
			}
			else
				return Enum_Rarity.White;
		}

		internal static System.Enum GetWeaponType(Arsenal данный, int вид)
		{
			if (данный == Arsenal.MachineGun)
				return (Enum_MachineGun)вид;
			else if (данный == Arsenal.PlasmaGun)
				return (Enum_PlasmaGun)вид;
			else if (данный == Arsenal.RocketLauncher)
				return (Enum_RocketLauncher)вид;
			else if (данный == Arsenal.GrenadeLauncher)
				return (Enum_GrenadeLauncher)вид;
			else if (данный == Arsenal.Robot)
				return (Enum_Robot)вид;
			else if (данный == Arsenal.Enemy)
				return (Enum_Enemy)вид;
			else
				return null;
		}

		public abstract void Peek();
		public abstract void Pop();
		public abstract void Push(Quaternion orientation, Vector3 position);
		public abstract void Dispose();

		///<summary>Типы вооружения.</summary>
		internal enum Arsenal : int
		{
			None,
			///<summary>Пулемёт.</summary>
			MachineGun,
			///<summary>Плазмомёт.</summary>
			PlasmaGun,
			///<summary>Ракетомёт.</summary>
			RocketLauncher,
			///<summary>Гранатомёт.</summary>
			GrenadeLauncher,
			///<summary>Робот.</summary>
			Robot,
			///<summary>Враги.</summary>
			Enemy
		}

		///<summary>Виды огнестрельного вооружения.</summary>
		internal enum Enum_MachineGun : int
		{
			///<summary>Пулемёт не выбран.</summary>
			None,
			///<summary>Стандартный пулемёт.</summary>
			Minigun,
			///<summary>Стреляет очередями со свойством 1й пули.</summary>
			Burst,
			///<summary>Дробовик.</summary>
			Shotgun,
			///<summary>Снайперка (мощная и медленная).</summary>
			Sniper
		}

		///<summary>Виды пламенного вооружения.</summary>
		internal enum Enum_PlasmaGun : int
		{
			///<summary>Лазер не выбран.</summary>
			None,
			///<summary>Скорострельный импульсный лазер.</summary>
			Spray,
			///<summary>Заряжаемый импульс.</summary>
			Charge,
			///<summary>Импульсы отражаются от объектов.</summary>
			Ricochet,
			///<summary>Пролетают насквозь.</summary>
			Piercer,
			///<summary>Окрашивает объекты.</summary>
			Marker,
			///<summary>Мощнейший прямой луч, уничтожающий всё на поверхности астероида.</summary>
			Apocalipse
		}

		///<summary>Виды ракетного вооружения.</summary>
		internal enum Enum_RocketLauncher : int
		{
			///<summary>Ракеты не выбраны.</summary>
			None,
			///<summary>Прямолетящие ракеты.</summary>
			Torpedo,
			///<summary>Самонаводящиеся ракеты.</summary>
			Homing,
			///<summary>Разлетающиеся ракеты.</summary>
			Spread,
			///<summary>Трассирующая ракета за курсором.</summary>
			Tracer
		}

		///<summary>Виды ядерного вооружения.</summary>
		internal enum Enum_GrenadeLauncher : int
		{
			///<summary>Гранаты не выбраны.</summary>
			None,
			///<summary>Взрыв большого радиуса.</summary>
			Bomb,
			///<summary>Располагаются на поле для уничтожения мобов и их пуль.</summary>
			Mine
		}

		///<summary>Виды электронного вооружения.</summary>
		internal enum Enum_Robot : int
		{
			///<summary>Робот не выбран.</summary>
			None,
			///<summary>Доп. защита барьера, но не корабля.</summary>
			Shield,
			///<summary>Починка корабля.</summary>
			Repair,
			///<summary>Спутниковый защитный робот.</summary>
			Zond,
			///<summary>Автономный атакующий робот.</summary>
			Drone
		}

		///<summary>Виды вражеского вооружения.</summary>
		internal enum Enum_Enemy : int
		{
			///<summary>Без вооружения.</summary>
			None,
			///<summary>Импульс.</summary>
			Impulse
		}

		///<summary>Варианты качества и редкости оружия.</summary>
		internal enum Enum_Rarity : int
		{
			///<summary>Худшее качество, стоит дешевле.</summary>
			Gray = -8355712,
			///<summary>Стандартное качество.</summary>
			White = -1,
			///<summary>Хорошее качество, уровень +1.</summary>
			Blue = -16728065,
			///<summary>Лучшее качество, уровень +2.</summary>
			Green = -16711936,
			///<summary>Наилучшее качество, уровень +3.</summary>
			Orange = -23296,
			///<summary>Высшее качество, уровень +4.</summary>
			Magenta = -65281
		}

		///<summary>Описание оружия.</summary>
		internal enum Enum_Description : int
		{
			///<summary>Ужасный.</summary>
			Horrible,
			///<summary>Свирепый.</summary>
			Fierce,
			///<summary>Жестокий.</summary>
			Cruel,
			///<summary>Большой.</summary>
			Large,
			///<summary>Массивный.</summary>
			Massive,
			///<summary>Опасный.</summary>
			Dangerous,
			///<summary>Дикий.</summary>
			Savage,
			///<summary>Колкий.</summary>
			Sharp,
			///<summary>Заостренный.</summary>
			Pointy,
			///<summary>Крошечный.</summary>
			Tiny,
			///<summary>Страшный.</summary>
			Terrible,
			///<summary>Маленький.</summary>
			Small,
			///<summary>Тупой.</summary>
			Dull,
			///<summary>Несчастливый.</summary>
			Unhappy,
			///<summary>Громоздкий.</summary>
			Bulky,
			///<summary>Постыдный.</summary>
			Shameful,
			///<summary>Тяжелый.</summary>
			Heavy,
			///<summary>Лёгкий.</summary>
			Light,
			///<summary>Зрячий.</summary>
			Sighted,
			///<summary>Скоростной.</summary>
			Rapid,
			///<summary>Стремительный.</summary>
			Hasty,
			///<summary>Устрашающий.</summary>
			Intimidating,
			///<summary>Смертоносный.</summary>
			Deadly,
			///<summary>Стойкий.</summary>
			Staunch,
			///<summary>Ужасный.</summary>
			Awful,
			///<summary>Летаргический.</summary>
			Lethargic,
			///<summary>Неуклюжий.</summary>
			Awkward,
			///<summary>Мощный.</summary>
			Powerful,
			///<summary>Мистический.</summary>
			Mystic,
			///<summary>Искусный.</summary>
			Adept,
			///<summary>Мастерский.</summary>
			Masterful,
			///<summary>Неумелый.</summary>
			Inept,
			///<summary>Невежественный.</summary>
			Ignorant,
			///<summary>Невменяемый.</summary>
			Deranged,
			///<summary>Интенсивный.</summary>
			Intense,
			///<summary>Запретный.</summary>
			Taboo,
			///<summary>Небесный.</summary>
			Celestial,
			///<summary>Яростный.</summary>
			Furious,
			///<summary>Острый.</summary>
			Keen,
			///<summary>Превосходный.</summary>
			Superior,
			///<summary>Сильный.</summary>
			Forceful,
			///<summary>Сломанный.</summary>
			Broken,
			///<summary>Поврежденный.</summary>
			Damaged,
			///<summary>Поддельный.</summary>
			Shoddy,
			///<summary>Быстрый.</summary>
			Quick,
			///<summary>Проворный.</summary>
			Agile,
			///<summary>Ловкий.</summary>
			Nimble,
			///<summary>Убийственный.</summary>
			Murderous,
			///<summary>Медлительный.</summary>
			Slow,
			///<summary>Вялый.</summary>
			Sluggish,
			///<summary>Ленивый.</summary>
			Lazy,
			///<summary>Раздражающий.</summary>
			Annoying,
			///<summary>Угрожающий.</summary>
			Nasty,
			///<summary>Маниакальный.</summary>
			Manic,
			///<summary>Пагубный.</summary>
			Hurtful,
			///<summary>Крепкий.</summary>
			Strong,
			///<summary>Отталкивающий.</summary>
			Unpleasant,
			///<summary>Слабый.</summary>
			Weak,
			///<summary>Беспощадный.</summary>
			Ruthless,
			///<summary>Безумный.</summary>
			Frenzying,
			///<summary>Божественный.</summary>
			Godly,
			///<summary>Демонический.</summary>
			Demonic,
			///<summary>Усердный.</summary>
			Zealous,
			///<summary>Легендарный.</summary>
			Legendary,
			///<summary>Нереальный.</summary>
			Unreal,
			///<summary>Мифический.</summary>
			Mythical
		}
	}
}