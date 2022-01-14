namespace IntergalacticInterceptors
{
    internal abstract class Localizator 
    {
        private static readonly string[] Languages = System.IO.Directory.GetFiles(System.IO.Path.Combine("Content", "Languages"), "*.txt", System.IO.SearchOption.TopDirectoryOnly);
        internal static readonly Imitator.Common.Localizator<EnumPhrases> Phrase = new Imitator.Common.Localizator<EnumPhrases> { Language = Languages[0] };

        internal enum EnumPhrases : int
        { 
            New_game,
            Continue,
            Options,
            Exit,
            Graphics,
            Sound,
            Controls,
            Language,
            SinglePlayer,
            MultyPlayer,
            Inventory,
            Ammunition,
            MachineGun,
            PlasmaGun,
            RocketLauncher,
            GrenadeLauncher,
            Robot,
            BuyAmmo,
            Shop,
            Show_Shop,
            Show_Inv,
            Getin_Shop,
            Getin_Test,
            Getin_Start
        }
    }
}