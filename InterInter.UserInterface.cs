namespace IntergalacticInterceptors
{
	internal abstract class UserInterface
	{
		///<summary>Максимальное количество видимых элементов в списке.</summary>
		public static int MaxStockView { get; set; } = 10;
		private static System.Drawing.Point MouseLastPosition = System.Drawing.Point.Empty;
		internal static System.Collections.Generic.Dictionary<int, int> ScrollPosition = new System.Collections.Generic.Dictionary<int, int>();
		///<summary>Покупатель в данный момент.</summary>
		private static Players.Human Viewer => Players.Collection.Count > 0 ? (Players.Human)Players.Collection[0] : null;

		///<summary>Выводит список с прокруткой.</summary>
		public static System.ValueTuple<Variants.Imitator.Input.MouseButtons, int> Scroll(InterInter mainForm, int id, System.Drawing.Point position, int interval, System.Drawing.Color textColor, System.Drawing.Color outlineColor, System.Collections.Generic.List<Weapons> entries)
		{
			var Scroll = new System.ValueTuple<Variants.Imitator.Input.MouseButtons, int>(Variants.Imitator.Input.MouseButtons.None, -1);
			if (!ScrollPosition.ContainsKey(id))
				ScrollPosition.Add(id, 0);
			int alpha = textColor.A;
			int highLighted = (200 * alpha) / System.Byte.MaxValue;

			string scr = "█" + System.Environment.NewLine + "█" + System.Environment.NewLine + "█";
			System.Drawing.Rectangle rect = mainForm.MainCamera.DrawString(scr, new System.Drawing.Point(position.X, position.Y + ScrollPosition[id]), mainForm.Font, System.Drawing.Color.White, true);
			System.Drawing.Point m = Variants.Imitator.Input.MousePosition();
			if (rect.Contains(m))
			{
				highLighted = (System.Byte.MaxValue * alpha) / System.Byte.MaxValue;
				if (Variants.Imitator.Input.MouseButton(Variants.Imitator.Input.MouseButtons.Left, 0))
					ScrollPosition[id] += m.Y - MouseLastPosition.Y;
			}
			MouseLastPosition = m;

			int maxStock = System.Math.Min(entries.Count, MaxStockView);

			if (entries.Count > MaxStockView)
			{
				if (Imitator.Common.UserInterface.Button(mainForm.MainCamera, (ScrollPosition[id] == 0) ? "●" : "▲", position, mainForm.Font, System.Drawing.Color.FromArgb(highLighted, textColor), outlineColor) == Variants.Imitator.Input.MouseButtons.Left && ScrollPosition[id] > 0)
					ScrollPosition[id] -= 1;
				if (Imitator.Common.UserInterface.Button(mainForm.MainCamera, (ScrollPosition[id] == entries.Count - maxStock ? "●" : "▼"), new System.Drawing.Point(position.X, position.Y + (maxStock - 1) * (mainForm.Font.Height + interval)), mainForm.Font, System.Drawing.Color.FromArgb(highLighted, textColor), outlineColor) == Variants.Imitator.Input.MouseButtons.Left && ScrollPosition[id] < entries.Count - maxStock)
					ScrollPosition[id] += 1;
			}
			ScrollPosition[id] = System.Math.Min(System.Math.Max(ScrollPosition[id], 0), entries.Count - maxStock);

			//rect = camera.DrawString(scr, position.X, position.Y + position.Width, Color.FromArgb(highLighted, color))
			//camera.DrawString(((int)(position.Width + MaxStockView - 2 - MaxStockView / entries.Count * MaxStockView) * (camera.Font.Height + position.Height)).ToString, position.X, position.Y - camera.Font.Height, Color.FromArgb(highLighted, color))

			for (highLighted = ScrollPosition[id] + maxStock - 1; highLighted >= ScrollPosition[id]; highLighted -= 1)
			{
				Variants.Imitator.Input.MouseButtons clickResult = Imitator.Common.UserInterface.Button(mainForm.MainCamera, entries[highLighted].ToString(), new System.Drawing.Point(position.X + (int)(mainForm.Font.SizeInPoints), position.Y + (highLighted - ScrollPosition[id]) * (mainForm.Font.Height + interval)), mainForm.Font, System.Drawing.Color.FromArgb((int)entries[highLighted].GetSpecifications.Rarity));
				if (clickResult == Variants.Imitator.Input.MouseButtons.Middle)
					Scroll.Item2 = highLighted;
				else if (clickResult == Variants.Imitator.Input.MouseButtons.Left || clickResult == Variants.Imitator.Input.MouseButtons.Right)
				{
					Scroll.Item1 = clickResult;
					Scroll.Item2 = highLighted;
				}
			}
			return Scroll;
		}

		public static string Diagram(Players player, Weapons.Arsenal arsenal)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (player[arsenal].GetSpecifications.Capacity > 0)
			{
				sb.Append(player[arsenal].Capacity.ToString("0. "));
				int blocks = (int)(7 * player[arsenal].Capacity / player[arsenal].GetSpecifications.Capacity);
				sb.Append("▂▃▄▅▆▇█", 0, blocks);
			}
			return sb.ToString();
		}

		public static string Indicator(Players player, Weapons.Arsenal arsenal)
		{
			return player[arsenal].Capacity.ToString("0. / ") + player[arsenal].GetSpecifications.Capacity;
		}

		public static void AmmunitionStats(InterInter mainForm)
		{
			var sb = new System.Text.StringBuilder();
			Weapons instance = Viewer[Weapons.Arsenal.MachineGun];
			if (instance != null)
			{
				if (instance.GetSpecifications.Type != 0)
				{
					sb.Append(instance.GetSpecifications.Description.ToString("G"));
					sb.Append(' ');
				}
				sb.AppendLine(Weapons.GetWeaponType(Weapons.Arsenal.MachineGun, instance.GetSpecifications.Type).ToString("G"));
				sb.Append(Diagram(Viewer, Weapons.Arsenal.MachineGun));
				Window(mainForm, sb.ToString(), mainForm.Font.Height + (int)(InterInter.Game_CameraShaking.X), mainForm.MainCamera.Height - mainForm.Font.Height * 10 + (int)(InterInter.Game_CameraShaking.Y), (Viewer.MachineGun_PlasmaGun ? System.Drawing.Color.Blue : System.Drawing.Color.LawnGreen), Localizator.Phrase[Localizator.EnumPhrases.MachineGun]);
			}

			sb.Clear();
			instance = Viewer[Weapons.Arsenal.PlasmaGun];
			if (instance != null)
			{
				if (instance.GetSpecifications.Type != 0)
				{
					sb.Append(instance.GetSpecifications.Description.ToString("G"));
					sb.Append(' ');
				}
				sb.AppendLine(Weapons.GetWeaponType(Weapons.Arsenal.PlasmaGun, instance.GetSpecifications.Type).ToString("G"));
				sb.Append(Diagram(Viewer, Weapons.Arsenal.PlasmaGun));
				Window(mainForm, sb.ToString(), mainForm.Font.Height + (int)(InterInter.Game_CameraShaking.X), mainForm.MainCamera.Height - mainForm.Font.Height * 5 + (int)(InterInter.Game_CameraShaking.Y), (Viewer.MachineGun_PlasmaGun ? System.Drawing.Color.LawnGreen : System.Drawing.Color.Blue), Localizator.Phrase[Localizator.EnumPhrases.PlasmaGun]);
			}

			sb.Clear();
			instance = Viewer[Weapons.Arsenal.RocketLauncher];
			if (instance != null)
			{
				if (instance.GetSpecifications.Type != 0)
				{
					sb.Append(instance.GetSpecifications.Description.ToString("G"));
					sb.Append(' ');
				}
				sb.AppendLine(Weapons.GetWeaponType(Weapons.Arsenal.RocketLauncher, instance.GetSpecifications.Type).ToString("G"));
				sb.Append(Indicator(Viewer, Weapons.Arsenal.RocketLauncher));
				Window(mainForm, sb.ToString(), mainForm.MainCamera.Width - mainForm.Font.Height * 10 + (int)(InterInter.Game_CameraShaking.X), mainForm.MainCamera.Height - mainForm.Font.Height * 10 + (int)(InterInter.Game_CameraShaking.Y), (Viewer.RocketLauncher_GrenadeLauncher ? System.Drawing.Color.Blue : System.Drawing.Color.LawnGreen), Localizator.Phrase[Localizator.EnumPhrases.RocketLauncher]);
			}

			sb.Clear();
			instance = Viewer[Weapons.Arsenal.GrenadeLauncher];
			if (instance != null)
			{
				if (instance.GetSpecifications.Type != 0)
				{
					sb.Append(instance.GetSpecifications.Description.ToString("G"));
					sb.Append(' ');
				}
				sb.AppendLine(Weapons.GetWeaponType(Weapons.Arsenal.GrenadeLauncher, instance.GetSpecifications.Type).ToString("G"));
				sb.Append(Indicator(Viewer, Weapons.Arsenal.GrenadeLauncher));
				Window(mainForm, sb.ToString(), mainForm.MainCamera.Width - mainForm.Font.Height * 10 + (int)(InterInter.Game_CameraShaking.X), mainForm.MainCamera.Height - mainForm.Font.Height * 5 + (int)(InterInter.Game_CameraShaking.Y), (Viewer.RocketLauncher_GrenadeLauncher ? System.Drawing.Color.LawnGreen : System.Drawing.Color.Blue), Localizator.Phrase[Localizator.EnumPhrases.GrenadeLauncher]);
			}
		}

		public static void PlayerStats(InterInter mainForm)
		{
			System.Drawing.Size windowSize = default;
			System.Text.StringBuilder sb = new System.Text.StringBuilder("Enemies:");
			sb.AppendLine();
			foreach (Players player in Players.Collection)
			{
				player.UpdateExperience();
				if (player.Fraction == Players.Fractions.Humans)
					windowSize += Window(mainForm,
					"HP: " + (player.Ship is null ? Players.CalculateHealth(player.Status.Level, 0).ToString() : player.Ship.Health.ToString("0.")) + System.Environment.NewLine +
					"CR: " + player.Status.Credits + System.Environment.NewLine +
					"LV: " + player.Status.Level + System.Environment.NewLine +
					"XP: " + player.Status.Experience,
					(int)(mainForm.Font.SizeInPoints + InterInter.Game_CameraShaking.X), (windowSize.Height * mainForm.Font.Height) + (int)InterInter.Game_CameraShaking.Y, System.Drawing.Color.White, player.ToString());
				else
					sb.AppendLine(player.ToString());
			}
			if (Variants.Imitator.Console.Debug)
				mainForm.MainCamera.DrawString(sb.ToString(), new System.Drawing.Point((int)(mainForm.Font.SizeInPoints + InterInter.Game_CameraShaking.X), (windowSize.Height * mainForm.Font.Height) + (int)InterInter.Game_CameraShaking.Y), mainForm.Font, System.Drawing.Color.White);
		}

		public static void RoundStats(InterInter mainForm)
		{
			UserInterface.Window(mainForm,
							 "Frags: " + Gameplay.FragsCount + System.Environment.NewLine +
							 "Time: " + Gameplay.TimeCount.ToString("0.00") + System.Environment.NewLine + System.Environment.NewLine +
							 "RECORDS" + System.Environment.NewLine +
							 "Frags: " + Gameplay.LocalRecord.Item1 + System.Environment.NewLine +
							 "Time: " + Gameplay.LocalRecord.Item2.ToString("0.00"), mainForm.MainCamera.Width - mainForm.Font.Height * 10 + (int)(InterInter.Game_CameraShaking.X), (int)(InterInter.Game_CameraShaking.Y), System.Drawing.Color.White, "ROUND: " + Gameplay.RoundCount);
		}

		public static string WeaponStats(Weapons Параметры, int denominator)
		{
			return ("Damage: " + Параметры.GetSpecifications.Damage + System.Environment.NewLine +
				"Rate: " + Параметры.GetSpecifications.Rate + System.Environment.NewLine +
				"Speed: " + (Параметры.GetSpecifications.Velocity > 0 ? Параметры.GetSpecifications.Velocity.ToString() : "∞") + System.Environment.NewLine +
				"Capacity: " + Параметры.GetSpecifications.Capacity + System.Environment.NewLine +
				"Knockback: " + Параметры.GetSpecifications.Strength + System.Environment.NewLine +
				"Critical: " + Параметры.GetSpecifications.Criticality + System.Environment.NewLine +
				System.Environment.NewLine + "COST: §" + Параметры.GetSpecifications.Stock / denominator);
		}

		public static System.Drawing.Size Window(InterInter mainForm, string In_Text, int In_PosX, int In_PosY, System.Drawing.Color In_Color, string In_Title = "")
		{
			int height = mainForm.Font.Height;
			int width = In_Title.Length;
			string[] lines = In_Text.Split(System.Environment.NewLine.Substring(1).ToCharArray());
			int eachLine;
			for (eachLine = lines.Length - 1; eachLine >= 0; eachLine -= 1)
				width = System.Math.Max(width, lines[eachLine].Length);
			width += 1;
			var sb = new System.Text.StringBuilder();
			sb.Append('╒');
			sb.Append('█', In_Title.Length + 1);
			sb.Append('═', width - In_Title.Length - 1);
			sb.AppendLine("╕");
			sb.Append('├');
			sb.Append('─', width);
			sb.AppendLine("┤");
			for (eachLine = lines.Length - 1; eachLine >= 0; eachLine -= 1)
			{
				sb.Append('│');
				sb.Append('█', width);
				sb.AppendLine("│");
			}
			sb.Append('└');
			sb.Append('─', width);
			sb.Append('┘');
			mainForm.MainCamera.DrawString(sb.ToString(), new System.Drawing.Point(In_PosX - height, In_PosY), mainForm.Font, System.Drawing.Color.DimGray);
			mainForm.MainCamera.DrawString(In_Title, new System.Drawing.Point(In_PosX, In_PosY), mainForm.Font, System.Drawing.Color.White);
			mainForm.MainCamera.DrawString(In_Text, new System.Drawing.Point(In_PosX, In_PosY + height * 2), mainForm.Font, In_Color);
			return new System.Drawing.Size(width, sb.ToString().Split(System.Environment.NewLine.Substring(1).ToCharArray()).Length);
		}
	}
}