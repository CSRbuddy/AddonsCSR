using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System;
using static CSRsyndra.Combo;



namespace CSRsyndra
{
    internal static class AutoHarass
    {
        private static int lastWCast;
        public static void Execute6()
        {
            var qtarget = TargetSelector.GetTarget(SpellsManager.Q.Range, DamageType.Magical);

            if ((qtarget == null) || qtarget.IsInvulnerable)
                return;
            //Cast Q
            if (Menus.HarassMenu["AutoQ"].Cast<CheckBox>().CurrentValue)
                if (qtarget.IsValidTarget(SpellsManager.Q.Range) && SpellsManager.Q.IsReady())
                    SpellsManager.Q.Cast(qtarget);
        }

        public static void Execute7()
        {
            var wtarget = TargetSelector.GetTarget(SpellsManager.W.Range, DamageType.Magical);

            if ((wtarget == null) || wtarget.IsInvulnerable)
                return;
            //Cast W
            if (Menus.HarassMenu["AutoW"].Cast<CheckBox>().CurrentValue && wtarget.IsValidTarget(SpellsManager.W.Range) && SpellsManager.W.IsReady())
            {
                var pred = SpellsManager.W.GetPrediction(wtarget);

                if (!myhero.HasBuff("SyndraW") && lastWCast + 500 < Environment.TickCount)
                {
                    SpellsManager.W.Cast(Functions.GrabWPost());
                    lastWCast = Environment.TickCount;
                }
                if (myhero.HasBuff("SyndraW") && lastWCast + 200 < Environment.TickCount)
                {
                    SpellsManager.W.Cast(pred.CastPosition);
                    lastWCast = Environment.TickCount;

                }
            }
        }
    }
}