using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace CSRriven
{
    class ClearManager : MenuBase
    {
        #region Riven: Lane/Jungle

        public static void CoreClear_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            
                var aiBase = args.Target as Obj_AI_Base;
                if (aiBase != null && aiBase.IsValidTarget())
                {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    if (SpellManager.W.IsReady() && !aiBase.Name.Contains("Mini") && getCheckBoxItem(farmMenu, "usejunglew"))
                    {
                        if (aiBase is Obj_AI_Minion && aiBase.Distance(player.ServerPosition) <= SpellManager.W.Range + 25)
                        {
                            SpellManager.W.Cast();
                        }
                    }

                    if (SpellManager.Q.IsReady() && !didaa && getCheckBoxItem(farmMenu, "usejungleq") && !aiBase.IsMinion)
                    {
                        if (aiBase.Distance(player.ServerPosition) <= SpellManager.Q.Range + 90)
                        {
                            if (!(aiBase is Obj_AI_Turret)) return;

                            if (ComboManager.qtarg != null && ComboManager.qtarg.NetworkId == aiBase.NetworkId)
                                Player.CastSpell(SpellSlot.Q, aiBase.ServerPosition);
                        }
                    }
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    if (SpellManager.Q.IsReady() && !didaa && getCheckBoxItem(farmMenu, "uselaneq"))
                    {
                        if (aiBase.Distance(player.ServerPosition) <= SpellManager.Q.Range + 90)
                        {
                            if (!(aiBase is Obj_AI_Turret)) return;

                            if (ComboManager.qtarg != null && ComboManager.qtarg.NetworkId == aiBase.NetworkId)
                                Player.CastSpell(SpellSlot.Q, aiBase.ServerPosition);
                        }
                    }
                }
                }

                if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)|| Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) && !player.UnderTurret(true))
                {
                    if (!EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(1400)))
                    {
                        if (Core.GameTickCount - lasthd < 1600 && args.Target is Obj_AI_Minion)
                        {
                            if (SpellManager.W.IsReady() && args.Target.Position.Distance(player.ServerPosition) <= SpellManager.W.Range + 25)
                            {
                                SpellManager.W.Cast();
                            }
                        }
                    }
                }
        
        }

        public static void JungClear()
        {
            var jminions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, 1000, true);
            foreach (var unit in jminions.Where(x => x.IsValidTarget(Player.Instance.AttackRange)))
            {
                if (Core.GameTickCount - lastw < 1000 && Core.GameTickCount - lasthd < 1000)
                {
                    if (unit.Distance(player.ServerPosition) <= SpellManager.Q.Range + 90 && SpellManager.Q.IsReady())
                    {
                        EventManager.DoOneQ(unit.ServerPosition);
                    }
                }

                if (Core.GameTickCount - laste < 600)
                {
                    if (unit.Distance(player.ServerPosition) <= SpellManager.W.Range + 45)
                    {
                        if (Item.CanUseItem(3077))
                            Item.UseItem(3077);
                        if (Item.CanUseItem(3074))
                            Item.UseItem(3074);
                    }
                }

                if (SpellManager.E.IsReady() && !didaa && getCheckBoxItem(farmMenu, "usejunglee"))
                {
                    if (player.Health / player.MaxHealth * 100 <= 70 || unit.Distance(player.ServerPosition) > truerange + 30)
                    {
                        SpellManager.E.Cast(Game.CursorPos);
                    }
                }

                if (SpellManager.E.IsReady() && !didaa && getCheckBoxItem(farmMenu, "usejunglee"))
                {
                    if (!SpellManager.Q.IsReady() && !SpellManager.W.IsReady())
                    {
                        SpellManager.E.Cast(Game.CursorPos);
                    }
                }

                if (SpellManager.W.IsReady() && !didaa && getCheckBoxItem(farmMenu, "usejunglew"))
                {
                    if (unit.Distance(player.ServerPosition) <= SpellManager.W.Range + 25)
                    {
                        SpellManager.W.Cast();
                    }
                }
            }
        }

        public static void LaneClear()
        {
            var Minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, 600f);
            foreach (var unit in Minions.Where(x => x.IsMinion))
            {
                if (player.CountEnemiesInRange(900) >= 1 && getCheckBoxItem(farmMenu, "clearnearenemy"))
                {
                    return;
                }

                if (didaa && !Orbwalker.CanAutoAttack && DamageManager.GetWDamage(unit) >= unit.Health && unit.IsValidTarget(SpellManager.W.Range))
                {
                    if (getCheckBoxItem(farmMenu, "usewlaneaa") && SpellManager.W.IsReady())
                    {
                        SpellManager.W.Cast();
                    }
                }

                if (SpellManager.W.IsReady())
                {
                    if (Minions.Count(m => m.Distance(player.ServerPosition) <= SpellManager.W.Range + 10) >= getSliderItem(farmMenu, "wminion"))
                    {
                        if (!didaa && getCheckBoxItem(farmMenu, "uselanew"))
                        {
                            if (Item.CanUseItem(3077))
                                Item.UseItem(3077);
                            if (Item.CanUseItem(3074))
                                Item.UseItem(3074);

                            SpellManager.W.Cast();
                        }
                    }
                }

                if (SpellManager.E.IsReady() && !player.ServerPosition.Extend(unit.ServerPosition, SpellManager.E.Range).To3DWorld().UnderTurret(true))
                {
                    if (unit.Distance(player.ServerPosition) > truerange + 30)
                    {
                        if (!didaa && getCheckBoxItem(farmMenu, "uselanee"))
                        {
                            Player.CastSpell(SpellSlot.E, unit.ServerPosition);
                        }
                    }

                    else if (player.Health / player.MaxHealth * 100 <= 70)
                    {
                        if (!didaa && getCheckBoxItem(farmMenu, "uselanee"))
                        {
                            Player.CastSpell(SpellSlot.E, unit.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
