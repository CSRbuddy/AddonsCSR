using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System;
using static CSRsyndra.Combo;

namespace CSRsyndra
{
    internal static class Harass
    {

        private static int lastWCast;

        public static void Execute1()
        {

            var target = TargetSelector.GetTarget(SpellsManager.W.Range, DamageType.Magical);

            if ((target == null) || target.IsInvulnerable)
                return;
            //Cast Q
            if (Menus.HarassMenu["Q"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(SpellsManager.Q.Range) && SpellsManager.Q.IsReady())
                    SpellsManager.Q.Cast(target);

            //cast Q - E
            if (Menus.HarassMenu["Qe"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(SpellsManager.QE.Range) && SpellsManager.Q.IsReady() && SpellsManager.E.IsReady())
            {
                var pred = SpellsManager.Q.GetPrediction(target);
                SpellsManager.Q.Cast(Player.Instance.Position.Extend(pred.CastPosition, SpellsManager.E.Range - 10).To3D());
                SpellsManager.E.Cast(Player.Instance.Position.Extend(pred.CastPosition, SpellsManager.E.Range - 10).To3D());

            }

            //Cast W
            if (Menus.HarassMenu["W"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(SpellsManager.W.Range) && SpellsManager.W.IsReady())
                {
                    var pred = SpellsManager.W.GetPrediction(target);

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