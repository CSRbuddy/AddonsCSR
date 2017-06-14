using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace CSRriven
{
    public class MenuBase
    {
        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static Menu Main, drawMenu, comboMenu, harassMenu, farmMenu, miscMenu;

        public static int lastq;
        public static int lastw;
        public static int laste;
        public static int lastr;
        public static int lastaa;
        public static int lasthd;
        
        public static bool didq;
        public static bool didw;
        public static bool dide;
        public static bool didws;
        public static bool didaa;
        public static bool didhd;
        public static int Qcount;
        public static int pc;

        public static float truerange;
        public static Vector3 movepos;

        public static AIHeroClient player { get { return ObjectManager.Player; } }

        internal static AIHeroClient myTarget { get; set; }
        internal static AIHeroClient Me => ObjectManager.Player;

        public static bool fightingLogic;
        
        public static bool CheckUlt()
        {
            if (Player.Instance.HasBuff("RivenFengShuiEngine"))
            {
                return true;
            }
            return false;
        }
    }
}
