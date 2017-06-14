using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using SharpDX;
using EloBuddy.SDK.Events;

namespace CSRriven
{
    class ComboManager : MenuBase
    {
        public static Obj_AI_Base qtarg; 

        public static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;
            switch (args.Animation)
            {
                case "Spell1a":
                    Core.DelayAction(() =>
                    {
                        Chat.Say("/d");
                        OrbHelper.ResetAutoAttackTimer();
                        Orbwalker.ResetAutoAttack();
                        //Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(Game.CursorPos, +10).To3DWorld());
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }, 291);
                    break;
                case "Spell1b":
                    Core.DelayAction(() =>
                    {
                        Chat.Say("/d");
                        OrbHelper.ResetAutoAttackTimer();
                        Orbwalker.ResetAutoAttack();
                        //Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(Game.CursorPos, +10).To3DWorld());
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }, 291);
                    break;
                case "Spell1c":
                    Core.DelayAction(() =>
                    {
                        Chat.Say("/d");
                        OrbHelper.ResetAutoAttackTimer();
                        Orbwalker.ResetAutoAttack();
                        //Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(Game.CursorPos, +10).To3DWorld());
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }, (getCheckBoxItem(comboMenu, "TheshyQ") ? 0 : 391));
                    break;
                    /*
                case "Spell2":
                    Core.DelayAction(() =>
                    {
                        Chat.Print("aaa");
                        Chat.Say("/d");
                        Orbwalker.ResetAutoAttack();
                        Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(Game.CursorPos, +10).To3DWorld());
                    }, (getSliderItem(comboMenu, "QD") * 10) + 1);
                    break;
                case "Spell3":
                    Core.DelayAction(() =>
                    {
                        Chat.Print("aaa");
                        Chat.Say("/d");
                        Orbwalker.ResetAutoAttack();
                        Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(Game.CursorPos, +10).To3DWorld());
                    }, (getSliderItem(comboMenu, "QD") * 10) + 1);
                    break;//*/
                case "Spell4a":
                    lastr = Core.GameTickCount;
                    break;
            }
        }
        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.W || args.Slot == SpellSlot.E)
            {
                OrbHelper.ResetAutoAttackTimer();
                //Orbwalker.ResetAutoAttack();
            }
        }
        public static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !args.SData.IsAutoAttack())
            {
                return;
            }          
            if (sender.IsMe && args.SData.IsAutoAttack())
            {
                qtarg = args.Target as Obj_AI_Base;
                lastaa = Core.GameTickCount;
                didaa = false;
            }           

            if (sender.IsMe && args.SData.IsAutoAttack())
            {
                var aiHero = args.Target as AIHeroClient;

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    var aiMob = args.Target as Obj_AI_Minion;
                    var unit = args.Target as AttackableUnit;
                    if (unit != null && !EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(1200)))
                    {
                        if (SpellManager.Q.IsReady() && !didaa)
                        {
                            Player.CastSpell(SpellSlot.Q, unit.Position);
                        }
                    }
                }

                if (getKeyBindItem(miscMenu, "shycombo"))
                {
                    if (EventManager.riventarget().IsValidTarget() && !EventManager.riventarget().IsZombie &&
                       !EventManager.riventarget().HasBuff("kindredrnodeathbuff"))
                    {
                        if (shy() && EventManager.CheckUlt())
                        {
                            if (EventManager.riventarget().HasBuffOfType(BuffType.Stun))
                                SpellManager.R2.Cast(EventManager.riventarget().ServerPosition);

                            var p = SpellManager.R2.GetPrediction(EventManager.riventarget());
                            if (p.HitChance == EloBuddy.SDK.Enumerations.HitChance.Medium && !EventManager.riventarget().HasBuffOfType(BuffType.Stun))
                            {
                                SpellManager.R2.Cast(p.CastPosition);
                            }
                        }
                    }
                }

                if (getKeyBindItem(comboMenu, "combokey") || getKeyBindItem(miscMenu, "shycombo"))
                {
                    if (SpellManager.E.IsReady() && EventManager.riventarget().IsValidTarget(SpellManager.E.Range + 200))
                    {
                        if (player.Health / player.MaxHealth * 100 <= getSliderItem(comboMenu, "vhealth"))
                        {
                            if (getCheckBoxItem(comboMenu ,"usecomboe") && !didaa)
                            {
                                if (aiHero != null && aiHero.IsValidTarget(600))
                                {
                                    Player.CastSpell(SpellSlot.E, aiHero.ServerPosition);
                                }
                            }
                        }                       
                    }
                    if (getCheckBoxItem(comboMenu, "ComboEGap") && SpellManager.E.IsReady() && aiHero.IsValidTarget(600) && aiHero.DistanceToPlayer() > OrbHelper.GetRealAutoAttackRange(Me) + 50)
                    {
                        Player.CastSpell(SpellSlot.E, aiHero.Position);
                    }
                }
                
                if (getKeyBindItem(comboMenu, "combokey") && aiHero.IsValidTarget())
                {
                    if (SpellManager.W.IsReady() && EventManager.riventarget().Distance(player.ServerPosition) <= SpellManager.W.Range)
                    {
                        if (getCheckBoxItem(comboMenu, "usecombow") && !didaa)
                        {
                            if (!EventManager.fightingLogic || (EventManager.fightingLogic && !EventManager.wrektAny()) || getCheckBoxItem(comboMenu, "w" + aiHero.ChampionName))
                            {
                                if (Core.GameTickCount - lasthd < 1000)
                                {
                                    SpellManager.W.Cast();
                                }

                                if (aiHero.HealthPercent < player.HealthPercent
                                    || (int)aiHero.HealthPercent == (int)player.HealthPercent)
                                {
                                    if (Qcount >= 2 || !SpellManager.Q.IsReady() || player.Distance(aiHero) > truerange)
                                    {
                                        SpellManager.W.Cast();
                                    }
                                }
                                else
                                {
                                    SpellManager.W.Cast();
                                }
                            }
                        }
                    }

                    if (SpellManager.Q.IsReady() && EventManager.riventarget().Distance(player.ServerPosition) <= SpellManager.Q.Range + 100)
                    {
                        if (getBoxItem(comboMenu, "wsmode") == 1 && EventManager.IsLethal(EventManager.riventarget()))
                        {
                            if (Qcount == 2 && SpellManager.E.IsReady() && !didaa)
                            {
                                Player.CastSpell(SpellSlot.E, EventManager.riventarget().ServerPosition);
                            }
                        }

                        if (getCheckBoxItem(comboMenu, "safeq"))
                        {
                            var endq = player.Position.Extend(EventManager.riventarget().Position, SpellManager.Q.Range + 35);

                            if (endq.CountEnemyChampionsInRange(200) <= 2)

                            {
                                Player.CastSpell(SpellSlot.Q, EventManager.riventarget().ServerPosition);
                            }
                        }

                        else
                        {
                            Player.CastSpell(SpellSlot.Q, EventManager.riventarget().ServerPosition);
                        }
                    }
                }   
            }

            if (sender.IsMe && args.SData.IsAutoAttack())
            {
                EventManager.SemiQ();
            }
        }

        #region Riven : Some Dash
        public static bool canburst()
        {
            if (EventManager.riventarget() == null || !SpellManager.R.IsReady())
            {
                return false;
            }

            if (EventManager.IsLethal(EventManager.riventarget()) && getBoxItem(comboMenu, "multib") == 0)
            {
                return true;
            }

            if (getKeyBindItem(miscMenu, "shycombo"))
            {
                if (shy())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool shy()
        {
            if (SpellManager.R.IsReady() && EventManager.riventarget() != null && getBoxItem(comboMenu, "multib") != 0)
            {
                return true;
            }

            return false;
        }

        private static void doFlash()
        {
            if (EventManager.riventarget() != null && (canburst() || shy()))
            {
                if (!SpellManager.Flash.IsReady() || !getCheckBoxItem(comboMenu, "flashb"))
                    return;

                if (getKeyBindItem(miscMenu, "shycombo"))
                {
                    if (EventManager.riventarget().Distance(player.ServerPosition) > SpellManager.E.Range + 50 &&
                        EventManager.riventarget().Distance(player.ServerPosition) <= SpellManager.E.Range + SpellManager.W.Range + 275)
                    {
                        if (player.Spellbook.CastSpell(SpellManager.flash, EventManager.riventarget().ServerPosition.Extend(player.ServerPosition, 125).To3DWorld()))
                        {
                            Chat.Say("/d");
                            Orbwalker.ResetAutoAttack();
                        }
                    }
                }
            }
        }

        public static void SomeDash(AIHeroClient target)
        {
            if (!getKeyBindItem(miscMenu, "shycombo") ||
                !target.IsValid<AIHeroClient>() || EventManager.CheckUlt())
                return;

            if (EventManager.riventarget() == null || !SpellManager.R.IsReady())
                return;

            if (SpellManager.Flash.IsReady() && SpellManager.W.IsReady() && (canburst() || shy()) && getBoxItem(comboMenu, "multib") != 2)
            {
                if (SpellManager.E.IsReady() && target.Distance(player.ServerPosition) <= SpellManager.E.Range + SpellManager.W.Range + 275)
                {
                    if (target.Distance(player.ServerPosition) > SpellManager.E.Range + truerange + 50)
                    {
                        Player.CastSpell(SpellSlot.E, target.ServerPosition);
                        if (!EventManager.CheckUlt())
                            SpellManager.R.Cast();
                    }
                }

                if (!SpellManager.E.IsReady() && target.Distance(player.ServerPosition) <= SpellManager.W.Range + 275)
                {
                    if (target.Distance(player.ServerPosition) > truerange + 50)
                    {
                        if (!EventManager.CheckUlt())
                            SpellManager.R.Cast();
                    }
                }
            }

            else
            {
                if (SpellManager.E.IsReady() && target.Distance(player.ServerPosition) <= SpellManager.E.Range + SpellManager.W.Range - 25)
                {
                    if (target.Distance(player.ServerPosition) > truerange + 50)
                    {
                        Player.CastSpell(SpellSlot.E, target.ServerPosition);

                        if (!EventManager.CheckUlt())
                            SpellManager.R.Cast();
                    }
                }

                if (!SpellManager.E.IsReady() && target.Distance(player.ServerPosition) <= SpellManager.W.Range - 10)
                {
                    if (!EventManager.CheckUlt())
                        SpellManager.R.Cast();
                }
            }
        }

        #endregion

        #region Riven: Combo

        public static void ComboTarget(AIHeroClient target)
        {
            var ende = player.Position.Extend(target.Position, SpellManager.E.Range + 35);
            var catchRange = SpellManager.E.IsReady() ? SpellManager.E.Range + truerange + SpellManager.W.Range : truerange + SpellManager.W.Range;

            if (target.Distance(player.ServerPosition) <= SpellManager.E.Range + 100 && SpellManager.Q.IsReady())
            {
                if (Core.GameTickCount - lastw < 500 && Core.GameTickCount - lasthd < 1000)
                {
                    if (target.Distance(player.ServerPosition) <= SpellManager.E.Range + 100 && SpellManager.Q.IsReady())
                    {
                        EventManager.DoOneQ(target.ServerPosition);
                    }
                }
            }

            if (Qcount == 2
                && target.Distance(player) >= player.AttackRange
                && target.Distance(player) <= 650
                && getCheckBoxItem(comboMenu, "Q3Wall")
                && SpellManager.E.IsReady())
            {
                var wallPoint = FleeManager.GetFirstWallPoint(player.Position, player.Position.Extend(target.Position, 650).To3DWorld());

                player.GetPath(wallPoint);

                if (!SpellManager.E.IsReady() || wallPoint.Distance(player.Position) > SpellManager.E.Range || !wallPoint.IsValid())
                {
                    return;
                }
                Player.CastSpell(SpellSlot.E, wallPoint);

                Core.DelayAction(() => Player.CastSpell(SpellSlot.Q, wallPoint), 190); //Q.Cast(wallPoint));

                if (wallPoint.Distance(player.Position) <= 100)
                {
                    Player.CastSpell(SpellSlot.Q, wallPoint);
                }
            }

            if (SpellManager.E.IsReady() && getCheckBoxItem(comboMenu, "usecomboe")
                && target.Distance(player.ServerPosition) > truerange + 100
                && (target.Distance(player.ServerPosition) <= SpellManager.E.Range + SpellManager.W.Range
                || EventManager.CheckUlt() && target.Distance(player.ServerPosition) > truerange + 200)
                || target.Distance(player.ServerPosition) <= SpellManager.E.Range + SpellManager.W.Range + SpellManager.Q.Range / 2f && SpellManager.R.IsReady()
                && (Qcount == 2 && EventManager.IsLethal(target) || Qcount == 2 && target.CountEnemiesInRange(SpellManager.W.Range + 35) >= 2))
            {
                if (!didaa)
                {
                    if (getCheckBoxItem(comboMenu, "safee"))
                    {
                        if (ende.CountEnemiesInRange(200) <= 2)
                        {
                            Player.CastSpell(SpellSlot.E, target.ServerPosition);
                        }
                    }

                    else
                    {
                        Player.CastSpell(SpellSlot.E, target.ServerPosition);
                    }

                    if (target.Distance(player.ServerPosition) <= SpellManager.E.Range + SpellManager.W.Range)
                    {
                        EventManager.checkr();

                        if (!canburst() && EventManager.CheckUlt() && Qcount != 2)
                        {
                            if (Item.CanUseItem(3077))
                                Item.UseItem(3077);
                            if (Item.CanUseItem(3074))
                                Item.UseItem(3074);
                        }
                    }

                    if (!canburst() && Qcount != 2)
                    {
                        if (Item.CanUseItem(3077))
                            Item.UseItem(3077);
                        if (Item.CanUseItem(3074))
                            Item.UseItem(3074);
                    }
                }
            }

            if (SpellManager.W.IsReady() && getCheckBoxItem(comboMenu, "usecombow") && target.Distance(player.ServerPosition) <= SpellManager.W.Range)
            {
                if (Core.GameTickCount - lasthd > 1500)
                {
                    EventManager.checkr();

                    if (getCheckBoxItem(comboMenu, "usecombow") && !didaa)
                    {
                        if (!EventManager.fightingLogic ||
                              (EventManager.fightingLogic && !EventManager.wrektAny()) ||
                                getCheckBoxItem(comboMenu, "w" + target.ChampionName))
                        {
                            if (target.HealthPercent < player.HealthPercent
                                || (int)target.HealthPercent == (int)player.HealthPercent)
                            {
                                if (Qcount >= 2 || !SpellManager.Q.IsReady() || player.Distance(target) > truerange)
                                {
                                    SpellManager.W.Cast();
                                }
                            }
                            else
                            {
                                SpellManager.W.Cast();
                            }
                        }
                    }
                    if (getCheckBoxItem(comboMenu, "ComboWLogic") && SpellManager.W.IsReady() && target.IsValidTarget(SpellManager.W.Range))
                    {
                        if (Qcount == 0 && SpellManager.W.Cast())
                        {
                            return;
                        }

                        if (SpellManager.Q.IsReady() && Qcount > 1 && SpellManager.W.Cast())
                        {
                            return;
                        }

                        if (Me.HasBuff("RivenFeint") && SpellManager.W.Cast())
                        {
                            return;
                        }

                        if (!target.IsFacing(Me) && SpellManager.W.Cast())
                        {
                            return;
                        }
                    }
                }
            }

            if (getCheckBoxItem(comboMenu, "useQgap") /*&& !SpellManager.E.IsReady()*/ && SpellManager.Q.IsReady() && (target.Distance(player.ServerPosition) > catchRange || target.Distance(player.ServerPosition) < 300))
            {
                if (Core.GameTickCount - lastq >= getSliderItem(comboMenu, "gaptimeQ") * 10)
                {
                    if (SpellManager.Q.IsReady() && Core.GameTickCount - laste >= 1000)
                    {
                        Player.CastSpell(SpellSlot.Q, EventManager.riventarget().ServerPosition);
                    }
                }
                if (target.Distance(player.ServerPosition) < SpellManager.E.Range + 150 && SpellManager.E.IsReady() && Core.GameTickCount - lastq >= 2000 && Qcount < 3 && Qcount >= 1)
                {
                    Player.CastSpell(SpellSlot.E, EventManager.riventarget().ServerPosition);
                }
            }
            else
            {
                if (target.Distance(player.ServerPosition) <= SpellManager.E.Range + SpellManager.W.Range)
                {
                    EventManager.checkr();
                }
            }

            if (getCheckBoxItem(comboMenu, "useQgap") && SpellManager.Q.IsReady() && Core.GameTickCount - lastq > 3600 && !Me.IsDashing() &&
                    target.IsValidTarget(480) && target.DistanceToPlayer() > OrbHelper.GetRealAutoAttackRange(Me) + 50)
            {
                var pred = SpellManager.Q.GetPrediction(target);

                if (pred.UnitPosition != Vector3.Zero &&
                    (pred.UnitPosition.DistanceToPlayer() < target.DistanceToPlayer() ||
                     pred.UnitPosition.Distance(target.Position) <= target.DistanceToPlayer()) && EventManager.CastQ(target))
                {
                    return;
                }
            }          
        }

        #endregion

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            
            if (!didq)
            {
                var targ = (AttackableUnit)args.Target;
                if (targ != null)
                {
                    didaa = true;
                }
            }
            
            var a = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(player.AttackRange + 360));

            var targets = a as AIHeroClient[] ?? a.ToArray();

            foreach (var target in targets)
            {
                if ((target.HasBuff("FioraW") || target.HasBuff("PopyW")) && Qcount == 2)
                {
                    return;
                }
            }

            switch (args.SData.Name)
            {
                case "ItemTiamatCleave":
                    lasthd = Core.GameTickCount;
                    didhd = true;
                    didaa = false;

                    if (qtarg != null && (!SpellManager.W.IsReady() || !getCheckBoxItem(comboMenu, "usecombow")))
                    {
                        if (getKeyBindItem(comboMenu, "combokey")
                        || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && !qtarg.UnderTurret(true))
                        {
                            if (!getKeyBindItem(miscMenu, "shycombo"))
                            {
                                if (SpellManager.Q.IsReady() && qtarg.Distance(player.ServerPosition) <= 350)
                                {
                                    Core.DelayAction(() => EventManager.DoOneQ(qtarg.ServerPosition), 250);
                                }
                            }
                        }
                    }

                    if (getBoxItem(comboMenu, "wsmode") == 1 || getKeyBindItem(miscMenu, "shycombo"))
                    {
                        if (getKeyBindItem(comboMenu, "combokey")
                        || getKeyBindItem(miscMenu, "shycombo"))
                        {
                            if (canburst() && EventManager.CheckUlt())
                            {
                                if (EventManager.riventarget().IsValidTarget() && !EventManager.riventarget().IsZombie && !EventManager.riventarget().HasBuff("kindredrnodeathbuff"))
                                {
                                    if (!EventManager.fightingLogic || getCheckBoxItem(comboMenu, "r" + EventManager.riventarget().ChampionName) ||
                                         EventManager.fightingLogic && !EventManager.rrektAny() || getKeyBindItem(miscMenu, "shycombo"))
                                    {
                                        Core.DelayAction(() =>
                                            {
                                                if (EventManager.riventarget().HasBuffOfType(BuffType.Stun))
                                                    SpellManager.R2.Cast(EventManager.riventarget().ServerPosition);
                                                else
                                                    SpellManager.R2.Cast(EventManager.riventarget());
                                            }, 240 - Game.Ping / 2);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "RivenTriCleave":
                    Qcount += 1;
                    didq = true;
                    didaa = false;
                    lastq = Core.GameTickCount;
                    break;
                case "RivenMartyr":
                    didw = true;
                    lastw = Core.GameTickCount;

                    break;
                case "RivenFeint":
                    dide = true;
                    didaa = false;
                    laste = Core.GameTickCount;

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                    {
                        if (EventManager.CheckUlt() && SpellManager.R.IsReady() && Qcount == 2 && SpellManager.Q.IsReady())
                        {
                            var btarg = TargetSelector.GetTarget(SpellManager.R2.Range, DamageType.Physical);
                            if (btarg.IsValidTarget())
                                SpellManager.R2.Cast(btarg);
                            else
                                SpellManager.R2.Cast(Game.CursorPos);
                        }
                    }

                    if (getKeyBindItem(miscMenu, "shycombo"))
                    {
                        if (Qcount == 2 && !EventManager.CheckUlt() && SpellManager.R.IsReady() && EventManager.riventarget() != null)
                        {
                            EventManager.checkr();
                            Core.DelayAction(() => SpellManager.Q.Cast(EventManager.riventarget().ServerPosition), 240 - Game.Ping);
                        }
                    }

                    if (getKeyBindItem(comboMenu, "combokey"))
                    {
                        if (Qcount == 2 && SpellManager.R.IsReady() && EventManager.riventarget() != null &&
                            (!EventManager.CheckUlt() || Core.GameTickCount - laste < 1000))
                        {
                            Core.DelayAction(EventManager.checkr, 100);
                        }
                    }

                    break;
                case "RivenFengShuiEngine":
                    doFlash();

                    if (getKeyBindItem(comboMenu, "combokey"))
                    {
                        if (Qcount == 2 && SpellManager.R.IsReady() && EventManager.riventarget() != null && Core.GameTickCount - laste < 1000)
                        {
                            Core.DelayAction(() => Player.CastSpell(SpellSlot.Q, EventManager.riventarget().ServerPosition), 200 - Game.Ping / 2);
                        }
                    }

                    break;
                case "RivenIzunaBlade":
                    didws = true;

                    if (Qcount == 2 && SpellManager.Q.IsReady() && EventManager.riventarget().IsValidTarget(SpellManager.R2.Range))
                        Player.CastSpell(SpellSlot.Q, EventManager.riventarget().ServerPosition);


                    if (SpellManager.W.IsReady() && EventManager.riventarget().IsValidTarget(SpellManager.W.Range + 35))
                        SpellManager.W.Cast();

                    else if (SpellManager.Q.IsReady() && EventManager.riventarget().IsValidTarget(SpellManager.E.Range + SpellManager.Q.Range))
                        Player.CastSpell(SpellSlot.Q, EventManager.riventarget().ServerPosition);

                    break;
            }
    }

        #region Riven: Aura

        public static void CastSkillOnupdate()
        {
            if (!player.IsDead)
            {
                foreach (var buff in player.Buffs)
                {
                    if (buff.Name == "rivenpassiveaaboost")
                        pc = buff.Count;
                }

                if (player.HasBuff("RivenTriCleave") && !getKeyBindItem(miscMenu, "shycombo"))
                {
                    if (player.GetBuff("RivenTriCleave").EndTime - Game.Time <= 0.25f)
                    {
                        if (!player.IsRecalling() && !player.Spellbook.IsChanneling)
                        {
                            var qext = player.ServerPosition.To2D() + player.Direction.To2D().Perpendicular() * SpellManager.Q.Range + 100;

                            if (getCheckBoxItem(comboMenu, "keepq"))
                            {
                                if (qext.To3D().CountEnemyChampionsInRange(200) <= 1 && !qext.To3D().UnderTurret(true))
                                {
                                    Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                                }
                            }
                        }
                    }
                }

                if (SpellManager.R.IsReady() && EventManager.CheckUlt() && getCheckBoxItem(comboMenu, "keepr"))
                {
                    if (player.GetBuff("RivenFengShuiEngine").EndTime - Game.Time <= 0.25f)
                    {
                        if (!EventManager.riventarget().IsValidTarget(SpellManager.R2.Range) || EventManager.riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            if (SpellManager.E.IsReady() && EventManager.CheckUlt())
                                Player.CastSpell(SpellSlot.E, Game.CursorPos);

                            SpellManager.R2.Cast(Game.CursorPos);
                        }
                        var p = SpellManager.R2.GetPrediction(EventManager.riventarget());
                        if (p.HitChance == EloBuddy.SDK.Enumerations.HitChance.High && EventManager.riventarget().IsValidTarget(SpellManager.R2.Range) && !EventManager.riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            SpellManager.R2.Cast(p.CastPosition);
                        }
                    }
                }

                if (!player.HasBuff("rivenpassiveaaboost"))
                    Core.DelayAction(() => pc = 1, 1000);

                if (Qcount > 2)
                    Core.DelayAction(() => Qcount = 0, 1000);
            }
        }

        public static void CombatDelay()
        {
            if (didaa && Core.GameTickCount + Game.Ping / 2 + 25 >= lastaa + player.AttackDelay * 1000)
                didaa = false;

            if (didhd && Core.GameTickCount - lasthd >= 250)
                didhd = false;

            if (didq && Core.GameTickCount - lastq >= 500)
                didq = false;

            if (didw && Core.GameTickCount - lastw >= 266)
                didw = false;

            if (dide && Core.GameTickCount - laste >= 350)
                dide = false;

            if (didws && Core.GameTickCount - laste >= 366)
                didws = false;
        }
        #endregion
    }
}
