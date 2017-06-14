using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;

namespace CSRriven
{
    class MenuManager : MenuBase
    {
        public static OrbHelper.Orbwalker Orbwalkerhep { get; set; }
        public static void LoadMenu()
        {
            Main = MainMenu.AddMenu("CSR Riven", "CSR Riven");
            Orbwalkerhep = new OrbHelper.Orbwalker();
            comboMenu = Main.AddSubMenu("+ Combo +");
            comboMenu.Add("combokey", new KeyBind("Combo", false, KeyBind.BindTypes.HoldActive, 32));
            comboMenu.Add("manualCancel", new CheckBox("Semi Cancel Animation"));
            comboMenu.Add("manualCancelPing", new CheckBox("Cancel Animation Calculate Ping?"));
            comboMenu.AddGroupLabel("Q Config");
            comboMenu.Add("Q3Wall", new CheckBox("Q3 Over Wall", true));
            comboMenu.Add("keepq", new CheckBox("Use Q Before Expiry", true));
            comboMenu.Add("useQgap", new CheckBox("Gapclose with Q", true));
            comboMenu.Add("gaptimeQ", new Slider("Gapclose Q Delay (ms)", 115, 0, 200));
            comboMenu.Add("safeq", new CheckBox("Block Q into multiple Enemies", true));
            comboMenu.Add("TheshyQ", new CheckBox("The Shy Combo", false));
            

            comboMenu.AddGroupLabel("W Config");
            comboMenu.Add("usecombow", new CheckBox("Use W in Combo", true));
            comboMenu.Add("ComboWLogic", new CheckBox("Use W Logic", true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Instance.Team))
                comboMenu.Add("w" + enemy.ChampionName, new CheckBox("Only W if it hit : " + enemy.ChampionName));
                       
            comboMenu.AddGroupLabel("E Config");
            comboMenu.Add("ComboEGap", new CheckBox("Use E Gapcloser", true));
            comboMenu.Add("usecomboe", new CheckBox("Use E in Combo", true));
            comboMenu.Add("vhealth", new Slider("Use E if HP% <=", 60));
            comboMenu.Add("safee", new CheckBox("Block E into multiple Enemies", true));

            comboMenu.AddGroupLabel("R1 Config");
            comboMenu.Add("useignote", new CheckBox("Combo with Ignite", true));
            comboMenu.Add("user", new KeyBind("Use R1 in Combo", false, KeyBind.BindTypes.PressToggle, 'H'));
            comboMenu.Add("ultwhen", new ComboBox("Use R1 when", new[] { "Normal Kill", "Hard Kill", "Always" }, 2));
            comboMenu.Add("overk", new Slider("Dont R1 if target HP % <=", 25, 1, 99));
            comboMenu.Add("userq", new Slider("Use only if Q Count <=", 2, 1, 3));
            comboMenu.Add("multib", new ComboBox("Burst when", new[] { "Damage Check", "Always" }, 1));
            comboMenu.Add("flashb", new CheckBox("-> Flash in Burst", true));
            comboMenu.AddGroupLabel("R2 Config");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Instance.Team))
                comboMenu.Add("r" + enemy.ChampionName, new CheckBox("Only R2 if it hit : " + enemy.ChampionName));


            comboMenu.Add("usews", new CheckBox("Use R2 in Combo", true));
            comboMenu.Add("rhitc", new ComboBox("-> HitChance", new[] { "Medium", "High", "Very High" }, 2));
            comboMenu.Add("logicR", new CheckBox("Logic R2", false));
            comboMenu.Add("wsmode", new ComboBox("Use R2 when", new[] {"Only KillSteal", "Max Damage" }, 1));
            comboMenu.Add("keepr", new CheckBox("Use R2 Before Expiry"));


            harassMenu = Main.AddSubMenu("+ Harass +");
            harassMenu.Add("useharassw", new CheckBox("Use W in Harass"));
            harassMenu.Add("usegaph", new CheckBox("Use E in Harass"));
            harassMenu.Add("qtoo", new ComboBox("Use Escape/Flee:", new[] { "Away from Target", "To Ally Turret", "To Cursor" }, 1));
            harassMenu.Add("useitemh", new CheckBox("Use Tiamat/Hydra", true));
            harassMenu.Add("semiq", new CheckBox("Auto Q Harass/Jungle", true));


            miscMenu = Main.AddSubMenu("+ Misc +");
            miscMenu.Add("shycombo", new KeyBind("Burst Combo", false, KeyBind.BindTypes.HoldActive, 'T'));
            miscMenu.Add("qint", new CheckBox("Interrupt with 3rd Q", true));
            miscMenu.Add("wint", new CheckBox("Use on Interrupt", true));
            miscMenu.Add("wgap", new CheckBox("Use on Gapcloser", true));
            miscMenu.Add("dodge", new CheckBox("DodgeE", true));
            miscMenu.Add("WallFlee", new CheckBox("WallJump in Flee", true));
            miscMenu.Add("skinHack", new CheckBox("Skin Change"));
            miscMenu.Add("SkinID", new Slider("Skin", 0, 0, 8));


            farmMenu = Main.AddSubMenu("+ Farm +");
            farmMenu.AddGroupLabel("Jung Clear");
            farmMenu.Add("usejungleq", new CheckBox("Use Q in Jungle", true));
            farmMenu.Add("usejunglew", new CheckBox("Use W in Jungle", true));
            farmMenu.Add("usejunglee", new CheckBox("Use E in Jungle", true));
           
            farmMenu.AddGroupLabel("+ Lane Clear +");
            farmMenu.Add("clearnearenemy", new CheckBox("Dont Clear Near Enemy", true));
            farmMenu.Add("uselaneq", new CheckBox("Use Q in WaveClear", true));
            farmMenu.Add("uselanew", new CheckBox("Use W in WaveClear", true));
            farmMenu.Add("usewlaneaa", new CheckBox("Use W on Unkillable Minion", true));
            farmMenu.Add("wminion", new Slider("Use W in WaveClear Minions >=", 3, 0, 5));
            farmMenu.Add("uselanee", new CheckBox("Use E in WaveClear", true));

            drawMenu = Main.AddSubMenu("+ Draw +");
            drawMenu.Add("drawAlwaysR", new CheckBox("Draw Always R Status", false));
            drawMenu.Add("drawTimer1", new CheckBox("Draw Q Expiry Time", false));
            drawMenu.Add("drawTimer2", new CheckBox("Draw R Expiry Time", true));
            drawMenu.Add("drawengage", new CheckBox("Draw Engage Range", false));
            drawMenu.Add("drawr2", new CheckBox("Draw R2 Range", false));
            drawMenu.Add("drawburst", new CheckBox("Draw Burst Range", false));
            drawMenu.Add("drawf", new CheckBox("Draw Target", true));
            drawMenu.Add("draGetWDamage", new CheckBox("Draw Combo Damage Fill", true));
            drawMenu.Add("fleeSpot", new CheckBox("Draw Flee Spots", true));
        }
    }
}
