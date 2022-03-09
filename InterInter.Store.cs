using System.Linq;

namespace IntergalacticInterceptors
{
	partial class Gameplay
	{
		///<summary>Магазин оружия.</summary>
		internal sealed class Store : Gameplay
		{
			///<summary>Сгенерированный прилавок оружия.</summary>
			private readonly System.Collections.Generic.List<Weapons> Storage = new System.Collections.Generic.List<Weapons>();
			///<summary>Максимальный уровень от уровня покупателя <see cref="Buyer"/>.</summary>
			public static int MaxFromLevel { get; set; } = 4;
			///<summary>Устанавливает соотношение цены продажи к покупке.</summary>
			public static float SellScale { get; set; } = 2;
			///<summary>Покупатель в данный момент.</summary>
			private Players Buyer => Players.Collection.Count > 0 ? Players.Collection[0] : null;

			///<summary>Очищает всю сцену и заполняет магазин.</summary>
			internal Store():base()
			{
				int count = InterInter.Randomizer.Next(20);
				for (; count > 0; count -= 1)
					Storage.Add(new Weapons.MachineGun(System.Math.Max(InterInter.Randomizer.Next(MaxFromLevel) - MaxFromLevel / 2 + Buyer.Status.Level, 0)));

				count = InterInter.Randomizer.Next(20);
				for (; count > 0; count -= 1)
					Storage.Add(new Weapons.PlasmaGun(System.Math.Max(InterInter.Randomizer.Next(MaxFromLevel) - MaxFromLevel / 2 + Buyer.Status.Level, 0)));

				count = InterInter.Randomizer.Next(20);
				for (; count > 0; count -= 1)
					Storage.Add(new Weapons.RocketLauncher(System.Math.Max(InterInter.Randomizer.Next(MaxFromLevel) - MaxFromLevel / 2 + Buyer.Status.Level, 0)));

				count = InterInter.Randomizer.Next(20);
				for (; count > 0; count -= 1)
					Storage.Add(new Weapons.GrenadeLauncher(System.Math.Max(InterInter.Randomizer.Next(MaxFromLevel) - MaxFromLevel / 2 + Buyer.Status.Level, 0)));

				count = InterInter.Randomizer.Next(20);
				for (; count > 0; count -= 1)
					Storage.Add(new Weapons.Robot(System.Math.Max(InterInter.Randomizer.Next(MaxFromLevel) - MaxFromLevel / 2 + Buyer.Status.Level, 0)));

				Buyer.ReloadAmmunition(Weapons.Arsenal.None);
			}

			internal override void Update(InterInter mainForm)
			{
				base.CommonBehavior(mainForm);
				Shopping(mainForm, true, new System.Drawing.Point(mainForm.MainCamera.Width / 7, mainForm.MainCamera.Height / 2));
				Shopping(mainForm, false, new System.Drawing.Point(mainForm.MainCamera.Width / 7, 0));
				if (Variants.Imitator.Input.MouseButtons.Left == Imitator.Common.UserInterface.Button(mainForm.MainCamera, Localizator.Text[Localizator.Keys.Menu_Exit], new System.Drawing.Point(mainForm.MainCamera.Width / 32, mainForm.MainCamera.Height / 2), mainForm.Font, System.Drawing.Color.Yellow, System.Drawing.Color.Black))
				{
					Buyer.StateSave();
					InterInter.MenuState = InterInter.MenuEntries.Main;
				}
				if (Variants.Imitator.Input.MouseButtons.Left == Imitator.Common.UserInterface.Button(mainForm.MainCamera, Localizator.Text[Localizator.Keys.Game_Galaxian], new System.Drawing.Point(mainForm.MainCamera.Width - mainForm.MainCamera.Width / 8, mainForm.MainCamera.Height / 2), mainForm.Font, System.Drawing.Color.Yellow, System.Drawing.Color.Black))
				{
					Buyer.StateSave();
					InterInter.MenuState = InterInter.MenuEntries.Gameplay;
					InterInter.Gameplay = new Gameplay.Galaxian();
					InterInter.CursorControl();
				}
				if (Variants.Imitator.Input.MouseButtons.Left == Imitator.Common.UserInterface.Button(mainForm.MainCamera, Localizator.Text[Localizator.Keys.Game_Arkanoid], new System.Drawing.Point(mainForm.MainCamera.Width - mainForm.MainCamera.Width / 8, mainForm.MainCamera.Height / 2 + mainForm.Font.Height * 3), mainForm.Font, System.Drawing.Color.Yellow, System.Drawing.Color.Black))
				{
					Buyer.StateSave();
					InterInter.MenuState = InterInter.MenuEntries.Gameplay;
					InterInter.Gameplay = new Gameplay.Arkanoid();
					InterInter.CursorControl();
				}
			}

			private void Shopping(InterInter mainForm, bool inInventory, System.Drawing.Point position)
			{
				mainForm.MainCamera.DrawString(Localizator.Text[inInventory ? Localizator.Keys.Game_Inventory : Localizator.Keys.Game_Shop], position, mainForm.FontTitle, System.Drawing.Color.Green);
				for (Weapons.Arsenal eachClass = Weapons.Arsenal.Robot; eachClass >= Weapons.Arsenal.MachineGun; eachClass -= 1)
				{
					System.Collections.Generic.List<Weapons> stockByClass = inInventory ?
						Buyer.Weaponry.Where((Weapons current) => current.GetSpecifications.Class == eachClass).ToList() :
						Storage.Where((Weapons current) => current.GetSpecifications.Class == eachClass).ToList();
					System.Drawing.Point scrollPos = new System.Drawing.Point(position.X + ((int)eachClass - 1) * mainForm.MainCamera.Width / 7, position.Y + mainForm.FontTitle.Height);
					mainForm.MainCamera.DrawString(Localizator.Text[(Localizator.Keys)System.Enum.Parse(typeof(Localizator.Keys), $"Game_{System.Enum.GetName(typeof(Weapons.Arsenal), eachClass)}", true)], scrollPos, mainForm.Font, System.Drawing.Color.White);
					scrollPos.Offset(0, mainForm.Font.Height);
					System.ValueTuple<Variants.Imitator.Input.MouseButtons, int> clickResult = UserInterface.Scroll(mainForm, (int)(inInventory ? eachClass + 10 : eachClass), scrollPos, 0, System.Drawing.Color.White, System.Drawing.Color.Black, stockByClass);
					if (inInventory)
					{
						int index = stockByClass.IndexOf(Buyer[(eachClass)]) - UserInterface.ScrollPosition[(int)(inInventory ? eachClass + 10 : eachClass)];
						if (index >= 0 && index < UserInterface.MaxStockView)
							mainForm.MainCamera.DrawString("▶", new System.Drawing.Point(scrollPos.X - (int)(mainForm.Font.SizeInPoints), scrollPos.Y + index * mainForm.Font.Height), mainForm.Font, System.Drawing.Color.Red);
					}

					if (clickResult.Item2 >= 0)
					{
						scrollPos = Variants.Imitator.Input.MousePosition() + System.Windows.Forms.SystemInformation.CursorSize;
						UserInterface.Window(mainForm, UserInterface.WeaponStats(stockByClass[clickResult.Item2], (int)(inInventory ? SellScale : 1)), scrollPos.X, scrollPos.Y, System.Drawing.Color.White, stockByClass[clickResult.Item2].GetSpecifications.Description.ToString("G") + " " + Weapons.GetWeaponType((eachClass), stockByClass[clickResult.Item2].GetSpecifications.Type).ToString("G"));
						if (!inInventory)
						{
							if (clickResult.Item1 == Variants.Imitator.Input.MouseButtons.Left)
							{
#if (DEBUG)
								if (!Variants.Imitator.Console.Debug && Buyer.Status.Credits < stockByClass[clickResult.Item2].GetSpecifications.Stock)
								{
									continue;
								}
#endif
								Buyer.Add(stockByClass[clickResult.Item2]);
								Storage.Remove(stockByClass[clickResult.Item2]);
								Emitter(mainForm.MainCamera.Name).SoundPlay(System.IO.Path.Combine(InterInter.RootPath, "User Interface", "Shop.Item.Buy.wav"));
								Buyer.Status.Credits -= stockByClass[clickResult.Item2].GetSpecifications.Stock;
							}
						}
						else
						{
							if (clickResult.Item1 == Variants.Imitator.Input.MouseButtons.Left)
							{
								Buyer[eachClass] = stockByClass[clickResult.Item2];
								Buyer.ReloadAmmunition(Weapons.Arsenal.None);
							}
							else if (clickResult.Item1 == Variants.Imitator.Input.MouseButtons.Right)
							{
								Storage.Add(stockByClass[clickResult.Item2]);
								Buyer.Remove(stockByClass[clickResult.Item2]);
								Emitter(mainForm.MainCamera.Name).SoundPlay(System.IO.Path.Combine(InterInter.RootPath, "User Interface", "Shop.Item.Sell.wav"));
								Buyer.Status.Credits += (int)(stockByClass[clickResult.Item2].GetSpecifications.Stock / SellScale);
							}
						}
					}
				}
			}

			private static Variants.Imitator.Scene.Emitter Emitter(string name)
			{
				var Emitter = Variants.Imitator.Scene.Emitter.Item(name);
				if (Emitter != null)
					return Emitter;
				return Variants.Imitator.Scene.Emitter.Add(name, System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero);
			}

			public override void Dispose()
			{
				foreach (Weapons weapon in Storage)
					weapon.Dispose();
				Storage.Clear();
			}
		}
	}
}