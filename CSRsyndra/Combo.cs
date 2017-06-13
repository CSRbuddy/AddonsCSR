using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Linq;
using static CSRsyndra.Menus;

namespace CSRsyndra
{
    internal static class Combo
    {
        public static AIHeroClient myhero
        {
            get { return ObjectManager.Player; }
        }

        private static int lastWCast;

        public static void Execute()
        {
            


            var target = TargetSelector.GetTarget(SpellsManager.W.Range, DamageType.Magical);

            if ((target == null) || target.IsInvulnerable)
                return;

            if (ComboMenu["QE"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(SpellsManager.QE.Range) && SpellsManager.Q.IsReady() && SpellsManager.E.IsReady())
            {
                var pred = SpellsManager.Q.GetPrediction(target);
                SpellsManager.Q.Cast(Player.Instance.Position.Extend(pred.CastPosition, SpellsManager.E.Range - 10).To3D());
                SpellsManager.E.Cast(Player.Instance.Position.Extend(pred.CastPosition, SpellsManager.E.Range - 10).To3D());
                
            }

            if (ComboMenu["Q"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(SpellsManager.Q.Range) && SpellsManager.Q.IsReady())
                {
                    var pred = SpellsManager.Q.GetPrediction(target);
                    SpellsManager.Q.Cast(pred.CastPosition);
                }

            if (ComboMenu[target.ChampionName].Cast<CheckBox>().CurrentValue && SpellsManager.R.IsReady() && target.IsValidTarget(SpellsManager.R.Range) && !target.HasUndyingBuff() &&
                Prediction.Health.GetPrediction(target, SpellsManager.R.CastDelay) <=
                SpellsManager.RDamage(SpellSlot.R, target))
            {
                SpellsManager.R.Cast(target);
            }

            //Cast W
            if (ComboMenu["W"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(SpellsManager.W.Range) && SpellsManager.W.IsReady())
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


            var Summ = TargetSelector.GetTarget(Ignite.Range, DamageType.Mixed);

            if ((Summ == null) || Summ.IsInvulnerable)
                return;
            //Ignite
            if (ComboMenu["Ignite"].Cast<CheckBox>().CurrentValue)
                if (Player.Instance.CountEnemyChampionsInRange(600) >= 1 && Ignite.IsReady() && Ignite.IsLearned && Summ.IsValidTarget(Ignite.Range))
                    if (target.Health >
                  target.GetTotalDamage())
                        Ignite.Cast(Summ);


        }

        public static Spell.Targeted Ignite = new Spell.Targeted(ReturnSlot("summonerdot"), 600);

        public static SpellSlot ReturnSlot(string Name)
        {
            return Player.Instance.GetSpellSlotFromName(Name);
        }
    }
}
    


