using System;
using EloBuddy;
using EloBuddy.SDK.Events;

namespace CSRriven
{
    class Program
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Riven)
            {
                return;
            }

            MenuManager.LoadMenu();
            Game.OnUpdate += EventManager.Game_OnUpdate;
            Game.OnWndProc += EventManager.Game_OnWndProc;
            Obj_AI_Base.OnProcessSpellCast += ComboManager.OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += ComboManager.Obj_AI_Base_OnSpellCast;
            Obj_AI_Base.OnSpellCast += ClearManager.CoreClear_OnSpellCast;
            Obj_AI_Base.OnPlayAnimation += ComboManager.OnPlayAnimation;
            Spellbook.OnCastSpell += ComboManager.Spellbook_OnCastSpell;
            Interrupter.OnInterruptableSpell += EventManager.Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += EventManager.Gapcloser_OnGapcloser;
            Drawing.OnDraw += DrawManager.Drawing_OnDraw;
            Drawing.OnDraw += DrawManager.Drawing_Spot;
            Drawing.OnEndScene += DrawManager.Drawing_OnEndScene;
            //Obj_AI_Base.OnProcessSpellCast += LogicManager.Dodge_OnProcessSpellCast;
            Chat.Print("<font color=\"#ca0711\" >Welcome to a reworked version of CSRriven</font><font color=\"#ffffff\" >Riven Box Box</font><font color=\"#ca0711\" >Kappa</font>");
        }
    }
}
