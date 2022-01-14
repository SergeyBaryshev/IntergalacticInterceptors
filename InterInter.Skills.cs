namespace IntergalacticInterceptors
{
    internal abstract class Skills
    {
        ///<summary>Список целей, на которые действует эффект.</summary>
        internal enum Target : int
        { 
            ///<summary>Шар.</summary>
            Ball,
            ///<summary>Игрок.</summary>
            Player,
            ///<summary>Напарник.</summary>
            Partner,
            ///<summary>Враг.</summary>
            Enemy,
            ///<summary>Барьер.</summary>
            Barrier,
            ///<summary>Пулемёт.</summary>
            MachineGun,
            ///<summary>Плазмомёт.</summary>
            PlasmaGun,
            ///<summary>Ракетомёт.</summary>
            RocketLauncher,
            ///<summary>Гранатомёт.</summary>
            GrenadeLauncher,
            ///<summary>Робот.</summary>
            Robot
        }

        ///<summary>Параметр, который применяет эффект.</summary>
        internal enum Parameter : int
        { 
            ///<summary>Жизнь или патроны.</summary>
            Health_Ammo,
            ///<summary>Броня или урон.</summary>
            Armor_Damage,
            ///<summary>Скорость.</summary>
            Speed
        }

        ///<summary>Эффект.</summary>
        internal struct Effect
        {
            ///<summary>Количество.</summary>
            public float Capacity;
            ///<summary>Продолжительность.</summary>
            public float Duration;
            ///<summary>Шанс.</summary>
            public float Chance;
            ///<summary>Тип цели.</summary>
            public Target Target;
            ///<summary>Параметы цели.</summary>
            public Parameter Parameter;

            public Effect(Target target, Parameter parameter, float capacity, float duration, float chance = 0F)
            {
                this.Target = target;
                this.Parameter = parameter;
                this.Capacity = capacity;
                this.Duration = duration;
                this.Chance = chance;
            }

            public override string ToString()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(this.Target.ToString("G"));
                sb.Append(" +");
                sb.Append(this.Capacity.ToString());
                sb.Append(" ");
                sb.Append(this.Parameter.ToString("G"));
                return sb.ToString();
            }
        }

        ///<summary>Варианты эффектов.</summary>
        internal enum Type : int
        { 
            ///<summary>Нет.</summary>
            None,
            ///<summary>Магнитная битка для шара.</summary>
            MagneticBat,
            ///<summary>Магнитный прицел для шара.</summary>
            MagneticSight,
            ///<summary>Размножение шара (кроме последнего все шары при касании барьера уничтожаются).</summary>
            QuadruplingBall,
            ///<summary>Усилитель шара.</summary>
            WeightingBall,
            ///<summary>Замедлитель шара.</summary>
            DecelerateBall,
            ///<summary>Ускоритель шара.</summary>
            AccelerateBall,
            ///<summary>Усилитель мощности оружия х2.</summary>
            WeaponPowerAmplifierX2,
            ///<summary>Усилитель скорости оружия х2.</summary>
            WeaponSpeedBoosterX2,
            ///<summary>Защита барьера.</summary>
            BarrierProtection,
            ///<summary>Добавление клона-битки (CPU).</summary>
            CloneBat,
            ///<summary>Восстановление энергии 50%.</summary>
            EnergyRecovery,
            ///<summary>Пополнение боезапаса ракет и бомб.</summary>
            ReplenishmentOfAmmunition,
            ///<summary>Расширение битки (работает до удара шара по барьеру).</summary>
            BatExtension,
            ///<summary>Заморозка всех мобов.</summary>
            Freeze,
            ///<summary>ЕМР бомба (-50% жизней ниже по уровню мобов).</summary>
            EMP,
            ///<summary>Ограниченная зона.</summary>
            RestrictedArea,
            ///<summary>Замедление игрока.</summary>
            PlayerSlowdown,
            ///<summary>Невидимый прицел.</summary>
            InvisibleSight,
            ///<summary>Полупрозрачный шар.</summary>
            TranslucentBall,
            ///<summary>Отказ оружия.</summary>
            WeaponFailure,
            ///<summary>Дикий шар (пытается уничтожить всех мобов).</summary>
            WildBall,
            ///<summary>Бесполезный шар (ничего не уничтожается).</summary>
            UselessBall,
            ///<summary>Сумасшедший шар (отскакивает в случайную сторону).</summary>
            CrazyBall,
            ///<summary>Инвертор (меняется управление на обратное).</summary>
            InvertedControl,
            ///<summary>Взрывной шар (при каждом ударе - взрыв).</summary>
            ExplosiveBall
        }
    }
}