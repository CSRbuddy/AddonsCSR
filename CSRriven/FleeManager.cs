namespace CSRriven
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using SharpDX;
    using System.Linq;

    internal class FleeManager : MenuBase
    {
        #region Public Methods and Operators

        public static Vector3 GetFirstWallPoint(Vector3 start, Vector3 end, int step = 1)
        {
            if (!start.IsValid() || !end.IsValid())
            {
                return Vector3.Zero;
            }

            var distance = start.Distance(end);

            for (var i = 0; i < distance; i = i + step)
            {
                var newPoint = start.Extend(end, i).To3DWorld();

                if (NavMesh.GetCollisionFlags(newPoint) == CollisionFlags.Wall || newPoint.IsWall())
                {
                    return newPoint;
                }
            }

            return Vector3.Zero;
        }

        public static float GetWallWidth(Vector3 start, Vector3 direction, int maxWallWidth = 350, int step = 1)
        {
            var thickness = 0f;

            if (start.IsValid() && direction.IsValid())
            {
                for (var i = 0; i < maxWallWidth; i = i + step)
                {
                    if (NavMesh.GetCollisionFlags(start.Extend(direction, i)) == CollisionFlags.Wall
                        || start.Extend(direction, i).IsWall())
                    {
                        thickness += step;
                    }
                    else
                    {
                        return thickness;
                    }
                }
            }
            return thickness;
        }

        public static bool IsWallDash(Obj_AI_Base unit, float dashRange, float minWallWidth = 100)
        {
            return IsWallDash(unit.ServerPosition, dashRange, minWallWidth);
        }

        public static bool IsWallDash(Vector3 position, float dashRange, float minWallWidth = 100)
        {
            var dashEndPos = ObjectManager.Player.Position.Extend(position, dashRange).To3DWorld();

            var firstWallPoint = GetFirstWallPoint(ObjectManager.Player.Position, dashEndPos);

            if (firstWallPoint.Equals(Vector3.Zero))
            {
                return false;
            }

            if (dashEndPos.IsWall())
            {
                // End Position is in Wall
                var wallWidth = GetWallWidth(firstWallPoint, dashEndPos);

                if (wallWidth > minWallWidth && wallWidth < dashRange)
                {
                    return true;
                }
            }
            else
            {
                // End Position is not a Wall
                return true;
            }

            return false;
        }

        #endregion

        #region Riven: Flee

        public static void Flee()
        {
            if (getCheckBoxItem(miscMenu, "WallFlee") && ObjectManager.Player.CountEnemiesInRange(1500) == 0)
            {
                var end = player.ServerPosition.Extend(Game.CursorPos, 350).To3DWorld();
                var isWallDash = IsWallDash(end, 350);

                var eend = player.ServerPosition.Extend(Game.CursorPos, 350).To3DWorld();
                var wallE = GetFirstWallPoint(player.ServerPosition, eend);
                var wallPoint = GetFirstWallPoint(player.ServerPosition, end);

                player.GetPath(wallPoint);

                if (SpellManager.Q.IsReady() && Qcount < 2)
                {
                    Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                }

                if (Qcount != 2 || !isWallDash) return;

                Player.IssueOrder(GameObjectOrder.MoveTo, wallPoint);

                if (SpellManager.E.IsReady() && wallPoint.Distance(player.ServerPosition) <= SpellManager.E.Range)
                {
                    Player.CastSpell(SpellSlot.E, wallE);

                    Core.DelayAction(() => Player.CastSpell(SpellSlot.Q, wallPoint), 190);
                }

                if (wallPoint.Distance(player.ServerPosition) <= 65)
                {
                    Player.CastSpell(SpellSlot.Q, wallPoint);
                }
            }
            else
            {
                var enemy = EntityManager.Heroes.Enemies.Where(target => InRange(target) && SpellManager.W.IsReady());

                var x = player.Position.Extend(Game.CursorPos, 300).To3DWorld();

                var targets = enemy as AIHeroClient[] ?? enemy.ToArray();

                if (SpellManager.W.IsReady() && targets.Any())
                {
                    foreach (var target in targets)
                    {
                        if (InRange(target))
                        {
                            SpellManager.W.Cast();
                        }
                    }
                }

                if (SpellManager.Q.IsReady() && !player.IsDashing())
                {
                    Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                }
                if (SpellManager.E.IsReady() && !player.IsDashing())
                {
                    Player.CastSpell(SpellSlot.E, x);
                }
            }
        }
        public static bool InRange(AttackableUnit x)
        {
            return ObjectManager.Player.HasBuff("RivenFengShuiEngine")
            ? player.Distance(x) <= 330
            : player.Distance(x) <= 265;
        }
        #endregion
    }
}
