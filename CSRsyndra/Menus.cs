using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using static CSRsyndra.Skins;
using CSRsyndra.Managers;
using EloBuddy.SDK;

namespace CSRsyndra
{
    internal class Menus
    {
        public const string DrawingsMenuId = "drawingsmenuid";
        public const string MiscMenuId = "miscmenuid";
        public static Menu FirstMenu;
        public static Menu DrawingsMenu;
        public static Menu ComboMenu;
        public static Menu HarassMenu;
        public static Menu LaneClearMenu;
        public static Menu FleeMenu;
        public static Menu MiscMenu;
        public static Menu KillStealMenu;

        public static ColorSlide QColorSlide;
        public static ColorSlide WColorSlide;
        public static ColorSlide EColorSlide;
        public static ColorSlide QEColorSlide;
        public static ColorSlide RColorSlide;
        public static ColorSlide DamageIndicatorColorSlide;

        public static void CreateMenu()
        {

            FirstMenu = MainMenu.AddMenu("CSR " + Player.Instance.ChampionName,
                Player.Instance.ChampionName.ToLower() + "Syndra");
            ComboMenu = FirstMenu.AddSubMenu("+ Combo +");
            HarassMenu = FirstMenu.AddSubMenu("+ Harass +");
            LaneClearMenu = FirstMenu.AddSubMenu("+ LaneClear +");
            FleeMenu = FirstMenu.AddSubMenu("+ Flee +");
            KillStealMenu = FirstMenu.AddSubMenu("+ Killsteal +");
            DrawingsMenu = FirstMenu.AddSubMenu("+ Drawings +", DrawingsMenuId);
            MiscMenu = FirstMenu.AddSubMenu("+ Misc +", MiscMenuId);
            
            
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("Q", new CheckBox("- Use Q"));
            ComboMenu.Add("W", new CheckBox("- Use W"));
            ComboMenu.Add("QE", new CheckBox("- Use Q - E"));
            ComboMenu.Add("R", new CheckBox("- Use R"));
            ComboMenu.Add("ItemManager.ZhonyasHourglass", new CheckBox("- Use when you go died Zhonya"));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("R usage on");
            foreach (var Enemy in EntityManager.Heroes.Enemies)
            {
                ComboMenu.Add(Enemy.ChampionName, new CheckBox("R on " + Enemy.ChampionName));
            }
            ComboMenu.AddSeparator();
            ComboMenu.Add("Ignite", new CheckBox("- Use Ignite"));
            ComboMenu.Add("Managers.YasuoWallManager", new CheckBox("- No R when Yasuo use Wall"));

            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("Q", new CheckBox("- Use Q"));
            HarassMenu.Add("W", new CheckBox("- Use W"));
            HarassMenu.Add("Qe", new CheckBox("- Use Q - E"));
            HarassMenu.Add("manaSlider", new Slider ("Mana must be higher than [{0}%] to use Harass Spells", 50, 0 , 100));

            HarassMenu.AddGroupLabel("Auto Harass");
            HarassMenu.Add("AutoQ", new CheckBox("- Q",false));
            HarassMenu.Add("AutoW", new CheckBox("- W", false));
            HarassMenu.AddLabel("*Thick this and it will Q and W from itself*");


            LaneClearMenu.AddGroupLabel("Lane Clear Settings");
            LaneClearMenu.Add("Q", new CheckBox("- Use Q"));
            LaneClearMenu.Add("W", new CheckBox("- Use W"));
            LaneClearMenu.Add("E", new CheckBox("- Use E"));
            LaneClearMenu.Add("manaSlider", new Slider("Mana must be higher than [{0}%] to use LaneClear Spells", 50, 0, 100));
            LaneClearMenu.AddSeparator();
            LaneClearMenu.AddGroupLabel("Jungle Clear Settings");
            LaneClearMenu.Add("QJungle", new CheckBox("- Use Q"));
            LaneClearMenu.Add("WJungle", new CheckBox("- Use W"));
            LaneClearMenu.Add("EJungle", new CheckBox("- Use E", false));
            LaneClearMenu.Add("ManaSliderJungle", new Slider("Mana must be higher than [{0}%] to use JungleClear", 50, 0, 100));

            KillStealMenu.AddGroupLabel("Killsteal Settings");
            KillStealMenu.Add("Q", new CheckBox("- Use Q"));
            KillStealMenu.Add("W", new CheckBox("- Use W"));
            KillStealMenu.Add("E", new CheckBox("- Use E"));
            KillStealMenu.Add("R", new CheckBox("- Use R",false));


            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("QE", new CheckBox("- QE on enemy"));
            MiscMenu.AddGroupLabel("Misc");
            MiscMenu.Add("Interrupt", new CheckBox("- Interrupt"));
            MiscMenu.Add("Gapcloser", new CheckBox("- Gapcloser"));

            MiscMenu.AddGroupLabel("Skin Changer");

            var skinList = SkinsDB.FirstOrDefault(list => list.Champ == Player.Instance.Hero);
            if (skinList != null)
            {
                MiscMenu.Add("SkinComboBox", new ComboBox("Choose the skin", skinList.Skins));
                MiscMenu.Get<ComboBox>("skinComboBox").OnValueChange +=
                    delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        Player.Instance.SetSkinId(sender.CurrentValue);
                    };
            }

            DrawingsMenu.AddGroupLabel("Setting");
            DrawingsMenu.Add("readyDraw", new CheckBox(" - Draw Spell Range only if Spell is Ready."));
            DrawingsMenu.Add("damageDraw", new CheckBox(" - Draw Damage Indicator."));
            DrawingsMenu.Add("perDraw", new CheckBox(" - Draw Damage Indicator Percent."));
            DrawingsMenu.Add("statDraw", new CheckBox(" - Draw Damage Indicator Statistics.", false));
            DrawingsMenu.AddGroupLabel("Spells");
            DrawingsMenu.Add("readyDraw", new CheckBox(" - Draw Spell Range only if Spell is Ready."));
            DrawingsMenu.Add("qDraw", new CheckBox("- draw Q"));
            DrawingsMenu.Add("wDraw", new CheckBox("- draw W"));
            DrawingsMenu.Add("eDraw", new CheckBox("- draw E"));
            DrawingsMenu.Add("qeDraw", new CheckBox("- draw QE"));
            DrawingsMenu.Add("rDraw", new CheckBox("- draw R"));
            DrawingsMenu.AddLabel("It will only draw if ready");
            DrawingsMenu.AddGroupLabel("Drawings Color");
            QColorSlide = new ColorSlide(DrawingsMenu, "qColor", Color.CornflowerBlue, "Q Color:");
            WColorSlide = new ColorSlide(DrawingsMenu, "wColor", Color.White, "W Color:");
            EColorSlide = new ColorSlide(DrawingsMenu, "eColor", Color.Coral, "E Color:");
            RColorSlide = new ColorSlide(DrawingsMenu, "rColor", Color.Red, "R Color:");
            DamageIndicatorColorSlide = new ColorSlide(DrawingsMenu, "healthColor", Color.Gold,
                "DamageIndicator Color:");

            MiscMenu.AddGroupLabel("Auto Level UP");
            MiscMenu.Add("activateAutoLVL", new CheckBox("Activate Auto Leveler", false));
            MiscMenu.AddLabel("The Auto Leveler will always Focus R than the rest of the Spells");
            MiscMenu.Add("firstFocus", new ComboBox("1 Spell to Focus", new List<string> { "Q", "W", "E" }));
            MiscMenu.Add("secondFocus", new ComboBox("2 Spell to Focus", new List<string> { "Q", "W", "E" }, 1));
            MiscMenu.Add("thirdFocus", new ComboBox("3 Spell to Focus", new List<string> { "Q", "W", "E" }, 2));
            MiscMenu.Add("delaySlider", new Slider("Delay Slider", 200, 150, 500));
        }
    }
}