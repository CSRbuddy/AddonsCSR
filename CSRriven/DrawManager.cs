using EloBuddy;
using System;
using System.Linq;
using EloBuddy.SDK.Rendering;
using SharpDX;
using EloBuddy.SDK;

namespace CSRriven
{
    class DrawManager : MenuBase
    {
        public static void Drawing_Spot(EventArgs args)
        {
            if (!player.IsDead)
            {
                if (getCheckBoxItem(drawMenu, "fleeSpot"))
                {
                    var end = player.ServerPosition.Extend(Game.CursorPos, 350).To3D();
                    var isWallDash = FleeManager.IsWallDash(end, 350);

                    var wallPoint = FleeManager.GetFirstWallPoint(player.ServerPosition, end);

                    if (!isWallDash || wallPoint.Distance(player.ServerPosition) > 600)
                    {
                        return;
                    }

                    Circle.Draw(Color.DarkSlateGray, 60, wallPoint);
                    Circle.Draw(Color.White, 60, end);
                }
            }
        }
        public static void Drawing_OnDraw(EventArgs args)
        {         
            if (!player.IsDead)
            {
                if (EventManager.riventarget().IsValidTarget(int.MaxValue))
                {
                    var tpos = Drawing.WorldToScreen(EventManager.riventarget().Position);

                    if (getCheckBoxItem(drawMenu, "drawf"))
                    {
                        Circle.Draw(Color.GreenYellow, 120, EventManager.riventarget().Position);
                    }

                    if (EventManager.riventarget().HasBuff("Stun"))
                    {
                        var b = EventManager.riventarget().GetBuff("Stun");
                        if (b.Caster.IsMe && b.EndTime - Game.Time > 0)
                        {
                            Drawing.DrawText(tpos[0], tpos[1], System.Drawing.Color.Lime, "STUNNED " + (b.EndTime - Game.Time).ToString("F"));
                        }
                    }
                }
                
                if (EventManager._sh.IsValidTarget())
                {
                    if (getCheckBoxItem(drawMenu, "drawf"))
                    {
                        Circle.Draw(Color.GreenYellow, 90, EventManager._sh.Position);
                    }
                }
                
                if (getCheckBoxItem(drawMenu, "drawengage"))
                {
                    Circle.Draw(Color.White, player.AttackRange + SpellManager.E.Range, player.Position);
                }

                if (getCheckBoxItem(drawMenu, "drawr2"))
                {
                    Circle.Draw(Color.White, SpellManager.R2.Range, player.Position);
                }
                
                if (getCheckBoxItem(drawMenu, "drawburst") && (ComboManager.canburst() || ComboManager.shy()) && EventManager.riventarget().IsValidTarget())
                {
                    var xrange = getCheckBoxItem(comboMenu, "flashb") && SpellManager.Flash.IsReady() ? 255 : 0;
                    Circle.Draw(Color.LawnGreen, SpellManager.E.Range + SpellManager.W.Range - 25 + xrange, EventManager.riventarget().Position);
                }

                var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);

                
                if (Qcount != 0 && getCheckBoxItem(drawMenu, "drawTimer1"))
                {
                    Drawing.DrawText((int)Drawing.WorldToScreen(player.Position).X - 60, (int)Drawing.WorldToScreen(player.Position).Y + 43, System.Drawing.Color.Red, "Q Expiry =>  " + ((double)(lastq - Core.GameTickCount + 3800) / 1000).ToString("0.0") + "S");
                }

                if (Player.HasBuff("RivenFengShuiEngine") && getCheckBoxItem(drawMenu, "drawTimer2"))
                {
                    Drawing.DrawText((int)Drawing.WorldToScreen(player.Position).X - 60, (int)Drawing.WorldToScreen(player.Position).Y + 65, System.Drawing.Color.Yellow, "R Expiry =>  " + (((double)lastr - Core.GameTickCount + 15000) / 1000).ToString("0.0") + "S");
                }
                if (getCheckBoxItem(drawMenu, "drawAlwaysR"))
                {
                    Drawing.DrawText(heropos.X - 35, heropos.Y + 20, System.Drawing.Color.DodgerBlue, "Always R (H) (     )");
                    Drawing.DrawText(heropos.X + 60, heropos.Y + 20, getKeyBindItem(comboMenu, "user") ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red, getKeyBindItem(comboMenu, "user") ? "On" : "Off");
                }
            }
        }

        private static HpBarIndicator hpi = new HpBarIndicator();

        public static void Drawing_OnEndScene(EventArgs args)
        {
            if (!getCheckBoxItem(drawMenu, "draGetWDamage"))
                return;

            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(ene => ene.IsValidTarget() && !ene.IsZombie))
            {
                var color = SpellManager.R.IsReady() && EventManager.IsLethal(enemy)
                    ? new ColorBGRA(0, 255, 0, 90)
                    : new ColorBGRA(255, 255, 0, 90);

                hpi.unit = enemy;
                hpi.draGetWDamage(DamageManager.GetComboDamage(enemy), color);
            }
        }
    }
}
