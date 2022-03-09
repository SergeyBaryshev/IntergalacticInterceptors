namespace IntergalacticInterceptors
{
	internal abstract class Localizator
	{
		private static readonly string[] Languages = System.IO.Directory.GetFiles(System.IO.Path.Combine(InterInter.RootPath, "Languages"), "*.txt", System.IO.SearchOption.TopDirectoryOnly);
		public static readonly Imitator.Common.Localizator<Keys> Text = new Imitator.Common.Localizator<Keys> { Language = Languages[Index] };

		///<summary>Активный по индексу <see cref="Index"/> язык текста.</summary>
		///<returns>Имя языка.</returns>
		public static string LanguageName
		{
			get
			{
				return System.IO.Path.GetFileNameWithoutExtension(Languages[Index]);
			}
		}

		///<summary>Возвращает или задаёт текст локализации.</summary>
		///<returns>Порядковый индекс языка.</returns>
		public static int Index
		{
			get
			{
				return My.Settings.LanguageIndex;
			}
			set
			{
				if (value < 0 || value >= Languages.Length)
					value = 0;
				My.Settings.LanguageIndex = value;
				Text.Language = Languages[value];
			}
		}

		internal enum Keys : int
		{
			Menu_Yes,
			Menu_No,
			Menu_NewGame,
			Menu_Continue,
			Menu_Options,
			Menu_Exit,
			Menu_Graphics,
			Menu_Sound,
			Menu_Music,
			Menu_Controls,
			Menu_KeyboardMove_MouseAim,
			Menu_MouseMove_KeyboardAim,
			Menu_Language,
			Menu_SinglePlayer,
			Menu_MultyPlayer,
			Game_Shop,
			Game_Inventory,
			Game_Ammunition,
			Game_MachineGun,
			Game_PlasmaGun,
			Game_RocketLauncher,
			Game_GrenadeLauncher,
			Game_Robot,
			Game_BuyAmmo,
			Game_Arkanoid,
			Game_Galaxian
		}
	}
}