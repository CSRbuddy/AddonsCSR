using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System.Linq;

namespace CSRriven
{
    class HarassManager : MenuBase
    {
        #region Riven: Harass

        public static void HarassTarget(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            Vector3 qpos;
            switch (getBoxItem(harassMenu, "qtoo"))
            {
                case 0:
                    qpos = player.ServerPosition + (player.ServerPosition - target.ServerPosition).Normalized() * 500;
                    break;
                case 1:
                    var tt = ObjectManager.Get<Obj_AI_Turret>().Where(t => (t.IsAlly)).OrderBy(t => t.Distance(player.Position)).First();
                    if (tt != null)
                        qpos = tt.Position;
                    else
                        qpos = player.ServerPosition +
                                (player.ServerPosition - target.ServerPosition).Normalized() * 500;
                    break;
                default:
                    qpos = Game.CursorPos;
                    break;
            }

            if (Qcount == 2 && !didaa && SpellManager.Q.IsReady())
            {
                if (!SpellManager.E.IsReady())
                {
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;

                    if (Player.IssueOrder(GameObjectOrder.MoveTo, qpos))
                    {
                        Core.DelayAction(() =>
                        {
                            Orbwalker.DisableAttacking = false;
                            Orbwalker.DisableMovement = false;
                            Player.CastSpell(SpellSlot.Q, qpos);
                        }, 175 - Game.Ping / 2);
                    }
                }
            }

            if (SpellManager.E.IsReady() && (Qcount == 3 || !SpellManager.Q.IsReady() && Qcount == 0))
            {
                if (player.Distance(target.ServerPosition) <= 300)
                {
                    if (getCheckBoxItem(harassMenu, "usegaph") && !didaa)
                        Player.CastSpell(SpellSlot.E, qpos);
                }
            }

            if (!target.ServerPosition.UnderTurret(true))
            {
                if (SpellManager.Q.IsReady() && !didaa && (Qcount < 2 || SpellManager.E.IsReady()))
                {
                    if (target.Distance(player.ServerPosition) <= truerange + SpellManager.Q.Range)
                    {
                        Player.CastSpell(SpellSlot.Q, target.ServerPosition);
                    }
                }
            }

            if (SpellManager.E.IsReady() && !didaa && SpellManager.Q.IsReady() && Qcount < 1 &&
                target.Distance(player.ServerPosition) > truerange + 100 &&
                target.Distance(player.ServerPosition) <= SpellManager.E.Range + truerange + 50)
            {
                if (!target.ServerPosition.UnderTurret(true))
                {
                    if (getCheckBoxItem(harassMenu, "usegaph") && !didaa)
                    {
                        Player.CastSpell(SpellSlot.E, target.ServerPosition);
                    }
                }
            }

            else if (SpellManager.W.IsReady() && !didaa && target.Distance(player.ServerPosition) <= SpellManager.W.Range + 10)
            {
                if (!player.ServerPosition.UnderTurret(true))
                {
                    if (getCheckBoxItem(harassMenu, "useharassw") && !didaa)
                    {
                        SpellManager.W.Cast();
                    }
                }
            }
        }

        #endregion
    }
}
