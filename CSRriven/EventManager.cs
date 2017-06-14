using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSRriven
{
    class EventManager : MenuBase
    {     
        private static readonly string[] minionlist =
        {
            // summoners rift
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp", 
            
            // twisted treeline
            "TT_NGolem5", "TT_NGolem2", "TT_NWolf6", "TT_NWolf3",
            "TT_NWraith1", "TT_Spider"
        };       
        

        public static float xtra(float dmg)
        {
            return SpellManager.R.IsReady() ? (float)(dmg + (dmg * 0.2)) : dmg;
        }

        public static bool IsLethal(Obj_AI_Base unit)
        {
            return DamageManager.GetComboDamage(unit) / 1.65 >= unit.Health;
        }

        public static Vector2 ExtendDir(Vector2 pos, Vector2 dir, float distance)
        {
            return pos + dir * distance;
        }

        public static Vector3 ExtendDir(Vector3 pos, Vector3 dir, float distance)
        {
            return pos + dir * distance;
        }

        public static AIHeroClient _sh;
        public static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (ulong)0x513)
            {
                _sh = EntityManager.Heroes.Enemies
                     .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000)
                     .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            }
        }

        public static AIHeroClient riventarget()
        {
            var cursortarg = EntityManager.Heroes.Enemies
                .Where(x => x.Distance(Game.CursorPos) <= 1400 && x.Distance(player.ServerPosition) <= 1400)
                .OrderBy(x => x.Distance(Game.CursorPos)).FirstOrDefault(x => x.IsValidTarget());

            var closetarg = EntityManager.Heroes.Enemies
                .Where(x => x.Distance(player.ServerPosition) <= SpellManager.E.Range + 100)
                .OrderBy(x => x.Distance(player.ServerPosition)).FirstOrDefault(x => x.IsValidTarget());

            return _sh ?? cursortarg ?? closetarg;
        }

        public static bool wrektAny()
        {
            return (player.GetEnemiesInRange(1250).Any(ez => getCheckBoxItem(comboMenu, "w" + ez.ChampionName)));               
        }

        public static bool rrektAny()
        {
            return (player.GetEnemiesInRange(1250).Any(ez => getCheckBoxItem(comboMenu, "r" + ez.ChampionName)));
        }

        public static bool CastQ(Obj_AI_Base target)
        {
            if (target == null || target.IsDead || !target.IsValidTarget() || SpellManager.Q.Level == 0 || !SpellManager.Q.IsReady())
            {
                return false;
            }
            return true;
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (SpellManager.W.Level > 0)
            {
                if (Me.HasBuff("RivenFengShuiEngine"))
                    SpellManager.W.Range = 330;
                else
                    SpellManager.W.Range = 260;
            }
          
            // my radius
            truerange = player.AttackRange + player.Distance(player.BBox.Minimum) + 1;

            // if no valid target cancel to cursor pos
            if (!ComboManager.qtarg.IsValidTarget(truerange + 100))
                ComboManager.qtarg = player;

            if (!riventarget().IsValidTarget())
                _sh = null;

            // move target position
            if (ComboManager.qtarg != player && ComboManager.qtarg.Distance(player.ServerPosition) < SpellManager.R2.Range)
                movepos = player.Position.Extend(Game.CursorPos, player.Distance(Game.CursorPos) + 500).To3DWorld();

            // move to game cursor pos
            if (ComboManager.qtarg == player)
                movepos = player.ServerPosition + (Game.CursorPos - player.ServerPosition).Normalized() * 125;

            ComboManager.CastSkillOnupdate();
            ComboManager.CombatDelay();

            if (getCheckBoxItem(miscMenu, "skinHack"))
            {
                player.SetSkinId(getSliderItem(miscMenu, "SkinID"));
            }

            if (riventarget().IsValidTarget())
            {
                if (getKeyBindItem(comboMenu, "combokey"))
                {
                    ComboManager.ComboTarget(riventarget());
                }
            }

            if (getKeyBindItem(miscMenu, "shycombo"))
            {
                if (riventarget().IsValidTarget())
                {
                    ComboManager.SomeDash(riventarget());

                    if (SpellManager.W.IsReady() && riventarget().Distance(player.ServerPosition) <= SpellManager.W.Range + 50)
                    {
                        checkr();
                        SpellManager.W.Cast();
                    }

                    else if (SpellManager.Q.IsReady() && riventarget().Distance(player.ServerPosition) <= truerange + 100)
                    {
                        checkr();

                        if (!didaa && Core.GameTickCount - lasthd >= 300)
                        {
                            if (Core.GameTickCount - lastw >= 300 + Game.Ping)
                            {
                                Player.CastSpell(SpellSlot.Q, riventarget().ServerPosition);
                            }
                        }
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && riventarget().IsValidTarget())
            {
                HarassManager.HarassTarget(riventarget());
            }

            if (player.IsValid && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ClearManager.LaneClear();
            }
            if (player.IsValid && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ClearManager.JungClear();
            }

            if (player.IsValid && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                FleeManager.Flee();
            }

            r2Kill();
            r2Maxdamage();

            fightingLogic = player.CountAllyChampionsInRange(1500) > 1 && player.CountEnemyChampionsInRange(1350) > 2 || player.CountEnemyChampionsInRange(1200) > 2;
        }

        public static void r2Maxdamage()
        {
            if (CheckUlt() && getCheckBoxItem(comboMenu, "usews") && SpellManager.R.IsReady())
            {              
                #region MaxDmage

                if (getBoxItem(comboMenu, "wsmode") == 1) // max damage
                {
                    if (riventarget().IsValidTarget(SpellManager.R.Range) && !riventarget().IsZombie && riventarget().DistanceToPlayer() < 600)
                    {
                        if (DamageManager.GetRDamage(riventarget()) / riventarget().MaxHealth * 100 >= 50)
                        {
                            var p = SpellManager.R2.GetPrediction(riventarget());
                            if (p.HitChance >= HitChance.Medium && !didaa && !riventarget().HasBuff("kindredrnodeathbuff"))
                            {
                                if (!fightingLogic || getCheckBoxItem(comboMenu, "r" + riventarget().ChampionName) || (fightingLogic && !rrektAny()))
                                {
                                    SpellManager.R2.Cast(p.CastPosition);
                                }
                            }
                        }

                        if (SpellManager.Q.IsReady() && Qcount < 2 || SpellManager.Q.IsReady(2) && Qcount < 2)
                        {
                            var aadmg = player.GetAutoAttackDamage(riventarget(), true) * 2;
                            var currentrdmg = DamageManager.GetRDamage(riventarget());
                            var qdmg = DamageManager.GetQDamage(riventarget()) * 2;

                            var damage = aadmg + currentrdmg + qdmg;

                            if (xtra((float)damage) >= riventarget().Health || riventarget().CountEnemiesInRange(275) >= 2)
                            {
                                if (riventarget().Distance(player.ServerPosition) <= truerange + SpellManager.Q.Range)
                                {
                                    var p = SpellManager.R2.GetPrediction(riventarget());
                                    if (!riventarget().HasBuff("kindredrnodeathbuff") && !didaa)
                                    {
                                        if (p.HitChance >= HitChance.High && !didaa)
                                        {
                                            if (!fightingLogic || getCheckBoxItem(comboMenu, "r" + riventarget().ChampionName) || (fightingLogic && !rrektAny()))
                                            {
                                                SpellManager.R2.Cast(p.CastPosition);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion           
            }
        }

        public static void r2Kill()
        {
            if (CheckUlt() && getCheckBoxItem(comboMenu, "usews") && SpellManager.R.IsReady())
            {
                #region Killsteal
                foreach (var t in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(SpellManager.R.Range)))
                {
                    if (getCheckBoxItem(comboMenu, "logicR") && Player.Instance.IsInAutoAttackRange(t))
                    {
                        if (player.GetAutoAttackDamage(t, true) * 3 >= t.Health)
                        {
                            if (player.HealthPercent > 75)
                            {
                                if (t.CountEnemiesInRange(400) > 1 && t.CountEnemiesInRange(400) <= 2)
                                {
                                    if (t.HealthPercent < 20 ||
                                    (t.Health > DamageManager.GetRDamage(t) + Me.GetAutoAttackDamage(t) * 2 && t.HealthPercent < 40) ||
                                    (t.Health <= DamageManager.GetRDamage(t)) ||
                                    (t.Health <= DamageManager.GetComboDamage(t)))
                                    {
                                        var pred = SpellManager.R2.GetPrediction(t);
                                        if (pred.HitChance == (HitChance)getBoxItem(comboMenu, "rhitc") + 4 && !didaa && !t.HasBuff("kindredrnodeathbuff"))
                                        {
                                            SpellManager.R2.Cast(pred.CastPosition);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (DamageManager.GetRDamage(t) > t.Health && t.DistanceToPlayer() < 600)
                    {
                        var pred = SpellManager.R2.GetPrediction(t);
                        if (pred.HitChance == (HitChance)getBoxItem(comboMenu, "rhitc") + 4 && !didaa && !t.HasBuff("kindredrnodeathbuff"))
                        {
                            SpellManager.R2.Cast(pred.CastPosition);
                        }
                    }

                    if (SpellManager.W.IsReady() && t.Distance(player) <= 250 && DamageManager.GetRDamage(t) + DamageManager.GetWDamage(t) >= t.HealthPercent)
                    {
                        var p = SpellManager.R2.GetPrediction(t);
                        if (p.HitChance == (HitChance)getBoxItem(comboMenu, "rhitc") + 4 && !didaa && !t.HasBuff("kindredrnodeathbuff"))
                        {
                            SpellManager.R2.Cast(p.CastPosition);
                        }
                    }
                }

                #endregion
            }
        }

        #region Riven: Misc 

        public static void DoOneQ(Vector3 pos)
        {
            if (SpellManager.Q.IsReady() && Core.GameTickCount - lastq > 5000)
            {
                if (SpellManager.Q.Cast(pos))
                {
                    lastq = Core.GameTickCount;
                    didq = true;
                }
            }
        }
        #region Riven: Semi Q 

        public static void SemiQ()
        {
            if (getCheckBoxItem(harassMenu, "semiq"))
            {
                if (SpellManager.Q.IsReady() && ComboManager.qtarg != null)
                {
                    if (ComboManager.qtarg.IsValidTarget(SpellManager.Q.Range + 100) &&
                        !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                        !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) &&
                        !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                        !getKeyBindItem(comboMenu, "combokey") &&
                        !getKeyBindItem(miscMenu, "shycombo"))
                    {
                        if (ComboManager.qtarg.IsValid<AIHeroClient>() && !ComboManager.qtarg.UnderTurret(true))
                            SpellManager.Q.Cast(ComboManager.qtarg.ServerPosition);
                    }

                    if (!getKeyBindItem(comboMenu, "combokey") &&
                        !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) &&
                        !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) &&
                        !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                        !getKeyBindItem(miscMenu, "shycombo"))
                    {
                        if (ComboManager.qtarg.IsValidTarget(SpellManager.Q.Range + 100) && !ComboManager.qtarg.Name.Contains("Mini"))
                        {
                            if (!ComboManager.qtarg.Name.StartsWith("Minion") && minionlist.Any(name => ComboManager.qtarg.Name.StartsWith(name)))
                            {
                                SpellManager.Q.Cast(ComboManager.qtarg.ServerPosition);
                            }
                        }

                        if (ComboManager.qtarg.IsValidTarget(SpellManager.Q.Range + 100))
                        {
                            if (ComboManager.qtarg.IsValid<AIHeroClient>() || ComboManager.qtarg.IsValid<Obj_AI_Turret>())
                            {
                                if (CheckUlt())
                                    SpellManager.Q.Cast(ComboManager.qtarg.ServerPosition);
                            }
                        }
                    }
                }
            }
        }

        #endregion
        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsMe || sender.IsAlly || sender == null)
                return;

            if (!getCheckBoxItem(miscMenu, "wint"))
                return;

            if (getCheckBoxItem(miscMenu, "wint") && SpellManager.W.IsReady())
            {
                if (!sender.Position.UnderTurret(true))
                {
                    if (sender.IsValidTarget(SpellManager.W.Range))
                        SpellManager.W.Cast();

                    if (sender.IsValidTarget(SpellManager.W.Range + SpellManager.E.Range) && SpellManager.E.IsReady())
                    {
                        Player.CastSpell(SpellSlot.E, sender.ServerPosition);
                    }
                }
            }
            if (getCheckBoxItem(miscMenu, "qint") && SpellManager.Q.IsReady() && Qcount >= 2)
            {
                if (!sender.Position.UnderTurret(true))
                {
                    if (sender.IsValidTarget(SpellManager.Q.Range))
                        Player.CastSpell(SpellSlot.Q, sender.ServerPosition);

                    if (sender.IsValidTarget(SpellManager.Q.Range + SpellManager.E.Range) && SpellManager.E.IsReady())
                    {
                        Player.CastSpell(SpellSlot.E, sender.ServerPosition);
                    }
                }
            }
        }

        public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsMe || sender.IsAlly || sender == null)
                return;

            if (getCheckBoxItem(miscMenu, "wgap") && SpellManager.W.IsReady())
            {
                if (sender.IsValidTarget(SpellManager.W.Range))
                {
                    if (!sender.ServerPosition.UnderTurret(true))
                    {
                        if (!fightingLogic || getCheckBoxItem(comboMenu, "w" + sender.ChampionName) || fightingLogic && !wrektAny())
                        {
                            SpellManager.W.Cast();
                        }
                    }
                }
            }
        }       
        #endregion


        #region Riven: Check R
        public static void checkr()
        {
            if (!SpellManager.R.IsReady() || CheckUlt() || !getKeyBindItem(comboMenu, "user"))
            {
                return;
            }

            var targets = EntityManager.Heroes.Enemies.Where(ene => ene.IsValidTarget(SpellManager.R.Range));
            var heroes = targets as IList<AIHeroClient> ?? targets.ToList();

            foreach (var target in heroes)
            {
                if (Qcount > getSliderItem(comboMenu, "userq"))
                {
                    return;
                }

                if (target.Health / target.MaxHealth * 100 <= getSliderItem(comboMenu, "overk") && IsLethal(target))
                {
                    if (heroes.Count() < 2)
                    {
                        continue;
                    }
                }

                if (getBoxItem(comboMenu, "ultwhen") == 2)
                    SpellManager.R.Cast();

                if (SpellManager.Q.IsReady() || Core.GameTickCount - lastq < 1000 && Qcount < 3)
                {
                    if (heroes.Count() < 2)
                    {
                        if (target.Health / target.MaxHealth * 100 <= getSliderItem(comboMenu, "overk") && IsLethal(target))
                            return;
                    }

                    if (heroes.Count(ene => ene.Distance(player.ServerPosition) <= 750) > 1)
                        SpellManager.R.Cast();

                    if (getBoxItem(comboMenu, "ultwhen") == 0)
                    {
                        if ((DamageManager.GetComboDamage(target) / 1.3) >= target.Health && target.Health >= (DamageManager.GetComboDamage(target) / 1.8))
                        {
                            SpellManager.R.Cast();
                        }
                    }

                    if (getBoxItem(comboMenu, "ultwhen") == 1)
                    {
                        if (DamageManager.GetComboDamage(target) >= target.Health && target.Health >= DamageManager.GetComboDamage(target) / 1.8)
                        {
                            SpellManager.R.Cast();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
