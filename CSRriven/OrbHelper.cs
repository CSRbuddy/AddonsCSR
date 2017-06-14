namespace CSRriven
{
    using System;
    using EloBuddy;
    using EloBuddy.SDK;
    using System.Collections.Generic;
    using SharpDX;
    using System.Linq;
    using Color = System.Drawing.Color;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    class OrbHelper : MenuBase
    {
        private static readonly string[] NoAttacks =
        {
            "volleyattack", "volleyattackwithsound",
            "jarvanivcataclysmattack", "monkeykingdoubleattack", "shyvanadoubleattack", "shyvanadoubleattackdragon",
            "zyragraspingplantattack", "zyragraspingplantattack2", "zyragraspingplantattackfire",
            "zyragraspingplantattack2fire", "viktorpowertransfer", "sivirwattackbounce", "asheqattacknoonhit",
            "elisespiderlingbasicattack", "heimertyellowbasicattack", "heimertyellowbasicattack2",
            "heimertbluebasicattack", "annietibbersbasicattack", "annietibbersbasicattack2",
            "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack", "yorickspectralghoulbasicattack",
            "malzaharvoidlingbasicattack", "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3",
            "kindredwolfbasicattack"
        };

        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "masteryidoublestrike", "quinnwenhanced", "renektonexecute", "renektonsuperexecute",
            "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff"
        };

        private static int _autoattackCounter;
        public static int LastAATick;
        public static int BrainFarmInt = -100;
        private static int LastAttackCommandT;
        private static int LastMoveCommandT;
        private static bool _missileLaunched;
        private static bool DisableNextAttack;
        public static bool Attack = true;
        public static bool Move = true;
        public static bool isNone = false;
        public static bool isCombo = false;
        public static bool isBurst = false;
        private static float _minDistance = 400;
        private static AttackableUnit _lastTarget;
        private static Vector3 LastMoveCommandPosition = Vector3.Zero;
        private static List<Obj_AI_Base> MinionListAA = new List<Obj_AI_Base>();
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);
        public delegate void AfterAttackEvenH(AttackableUnit unit, AttackableUnit target);
        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
        public delegate void OnAttackEvenH(AttackableUnit unit, AttackableUnit target);
        public delegate void OnNonKillableMinionH(AttackableUnit minion);
        public delegate void OnTargetChangeH(AttackableUnit oldTarget, AttackableUnit newTarget);
        public static event AfterAttackEvenH AfterAttack;
        public static event BeforeAttackEvenH BeforeAttack;
        public static event OnAttackEvenH OnAttack;
        public static event OnNonKillableMinionH OnNonKillableMinion;
        public static event OnTargetChangeH OnTargetChange;

        static OrbHelper()
        {
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += OnDoCast;
            Obj_AI_Base.OnBasicAttack += OnBasicAttack;
        }
        
        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            if (sender == null || !sender.IsMe || Args.Animation == null)
            {
                return;
            }

            var time = 0;
            var canAttack = false;

            switch (Args.Animation)
            {
                case "Spell1a": //Q1
                    time = 291;
                    //time = getCheckBoxItem(comboMenu, "Qtimer") ? 1 : 291;
                    canAttack = true;
                    lastq = Core.GameTickCount;
                    break;
                case "Spell1b": //Q2
                    time = 291;
                    //time = getCheckBoxItem(comboMenu, "Qtimer") ? 1 : 291;
                    canAttack = true;
                    lastq = Core.GameTickCount;
                    break;
                case "Spell1c": //Q3
                    time = 391;
                    //time = getCheckBoxItem(comboMenu, "Qtimer") ? 1 : 391;
                    canAttack = true;
                    lastq = Core.GameTickCount;
                    break;
                case "Spell2": //W
                    time = 50;
                    canAttack = false;
                    break;
                case "Spell3": //E
                    time = 0;
                    canAttack = true;
                    break;
                case "Spell4a": //R1
                    time = 50;
                    canAttack = true;
                    break;
                case "Spell4b": //R2
                    time = 180;
                    canAttack = true;
                    break;
                default:
                    time = 0;
                    canAttack = true;
                    break;
            }

            if (time > 0)
            {
                if (Orbwalker.ManualCancel || !isNone)
                {
                    if (Orbwalker.CalcutalePing)
                    {
                        if (time - Game.Ping > 0)
                        {
                            Core.DelayAction(() => Cancel(Args.Animation, canAttack), time - Game.Ping);
                        }
                        else
                        {
                            Core.DelayAction(() => Cancel(Args.Animation, canAttack), 1);
                        }
                    }
                    else
                    {
                        Core.DelayAction(() => Cancel(Args.Animation, canAttack), time);
                    }
                }
            }
        }

        private static void Cancel(string Name, bool canAttack)
        {
            ResetAutoAttackTimer();
            Player.DoEmote(Emote.Dance);

            if (canAttack)
            {
                if (Orbwalker.GetOrbTar != null && !Orbwalker.GetOrbTar.IsDead)
                {
                    Orbwalk(Orbwalker.GetOrbTar, Game.CursorPos);
                }
                else
                {
                    Orbwalk(null, Game.CursorPos);
                }
            }
            else
            {
                Core.DelayAction(() => Attack = false, 1);
                Core.DelayAction(() => Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(Game.CursorPos, +10).To3DWorld()), 2);
                Core.DelayAction(() => Attack = true, 350 + Game.Ping);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (unit == null || args.SData == null)
            {
                return;
            }

            if (unit.IsMe)
            {
                if (IsAutoAttack(args.SData.Name))
                {
                    FireOnAttack(unit, _lastTarget);
                    return;
                }
            }         
        }

        internal static void OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe && (Args.Target is Obj_AI_Base || Args.Target is Obj_BarracksDampener || Args.Target is Obj_HQ))
            {
                LastAATick = Core.GameTickCount - Game.Ping / 2;
                _missileLaunched = false;
                LastMoveCommandT = 0;
                _autoattackCounter++;

                var spell = Args.Target as Obj_AI_Base;

                if (spell != null)
                {
                    var target = spell;

                    if (target.IsValid)
                    {
                        FireOnTargetSwitch(target);
                        _lastTarget = target;
                    }
                }
            }

            FireOnAttack(sender, _lastTarget);
        }

        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                var ping = Game.Ping;

                if (ping <= 30)
                {
                    Core.DelayAction(() => OnDoCast_Delayed(sender, Args), 30 - ping);
                    return;
                }

                OnDoCast_Delayed(sender, Args);
            }
        }

        private static void OnDoCast_Delayed(AttackableUnit sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender == null || Args.SData == null)
            {
                return;
            }

            if (IsAutoAttack(Args.SData.Name))
            {
                FireAfterAttack(sender, Args.Target as AttackableUnit);
                _missileLaunched = true;
            }
        }

        private static void FireBeforeAttack(AttackableUnit target)
        {
            if (BeforeAttack != null)
            {
                BeforeAttack(new BeforeAttackEventArgs { Target = target });
            }
            else
            {
                DisableNextAttack = false;
            }
        }

        private static void FireAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (AfterAttack != null && target.IsValidTarget())
            {
                AfterAttack(unit, target);
            }
        }

        private static void FireOnAttack(AttackableUnit unit, AttackableUnit target)
        {
            OnAttack?.Invoke(unit, target);
        }

        private static void FireOnNonKillableMinion(AttackableUnit minion)
        {
            OnNonKillableMinion?.Invoke(minion);
        }

        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null && (!_lastTarget.IsValidTarget() || _lastTarget != newTarget))
            {
                OnTargetChange(_lastTarget, newTarget);
            }
        }

        public static bool CanAttack()
        {
            return Core.GameTickCount + Game.Ping / 2 + 25 >= LastAATick + ObjectManager.Player.AttackDelay * 1000;
        }

        public static bool CanMove(float extraWindup, bool disableMissileCheck = false)
        {
            if (_missileLaunched && !disableMissileCheck)
            {
                return true;
            }

            return Core.GameTickCount + Game.Ping / 2
                   >= LastAATick + ObjectManager.Player.AttackCastDelay * 1000 + extraWindup;
        }

        public static void MoveTo(Vector3 position, float holdAreaRadius = 0, bool overrideTimer = false,
            bool randomizeMinDistance = true)
        {
            var playerPosition = ObjectManager.Player.ServerPosition;

            if (playerPosition.Distance(position, true) < holdAreaRadius * holdAreaRadius)
            {
                if (ObjectManager.Player.Path.Length > 0)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, playerPosition);
                    LastMoveCommandPosition = playerPosition;
                    LastMoveCommandT = Core.GameTickCount - 70;
                }

                return;
            }

            var point = position;

            if (ObjectManager.Player.Distance(point, true) < 150 * 150)
            {
                point = playerPosition.Extend(
                    position,
                    randomizeMinDistance ? (_random.NextFloat(0.6f, 1) + 0.2f) * _minDistance : _minDistance).To3DWorld();
            }

            var angle = 0f;
            var currentPath = ObjectManager.Player.GetWaypoints();

            if (currentPath.Count > 1 && currentPath.PathLength() > 100)
            {
                var movePath = ObjectManager.Player.GetPath(point);

                if (movePath.Length > 1)
                {
                    var v1 = currentPath[1] - currentPath[0];
                    var v2 = movePath[1] - movePath[0];

                    angle = v1.AngleBetween(v2.To2D());

                    var distance = movePath.Last().To2D().Distance(currentPath.Last(), true);

                    if ((angle < 10 && distance < 500 * 500) || distance < 50 * 50)
                    {
                        return;
                    }
                }
            }

            if (Core.GameTickCount - LastMoveCommandT < 70 + Math.Min(60, Game.Ping) && !overrideTimer
                && angle < 60)
            {
                return;
            }

            if (angle >= 60 && Core.GameTickCount - LastMoveCommandT < 60)
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, point);
            LastMoveCommandPosition = point;
            LastMoveCommandT = Core.GameTickCount;
        }

        public static void Orbwalk(AttackableUnit target, Vector3 position, float extraWindup = 90,
            float holdAreaRadius = 0, bool randomizeMinDistance = true)
        {
            if (Core.GameTickCount - LastAttackCommandT < 70 + Math.Min(60, Game.Ping))
            {
                return;
            }

            if (target.IsValidTarget() && CanAttack() && Attack)
            {
                DisableNextAttack = false;
                FireBeforeAttack(target);

                if (!DisableNextAttack)
                {
                    _missileLaunched = false;

                    if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                    {
                        LastAttackCommandT = Core.GameTickCount;
                        _lastTarget = target;
                    }

                    return;
                }
            }

            if (CanMove(extraWindup) && Move)
            {
                MoveTo(position, Math.Max(holdAreaRadius, 30), false, randomizeMinDistance);
            }
        }

        public static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius;

            if (target.IsValidTarget())
            {
                return result + target.BoundingRadius;
            }

            return result;
        }

        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.IsValidTarget())
            {
                return false;
            }

            var myRange = GetRealAutoAttackRange(target);
 
            var basetarget = target as Obj_AI_Base;

            return
                Vector2.DistanceSquared(
                    basetarget?.ServerPosition.To2D() ?? target.Position.To2D(),
                    ObjectManager.Player.ServerPosition.To2D()) <= myRange * myRange;
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower()))
                   || Attacks.Contains(name.ToLower());
        }

        public static void ResetAutoAttackTimer()
        {
            EloBuddy.SDK.Orbwalker.ResetAutoAttack();
            LastAATick = 0;
        }

        public enum OrbwalkingMode
        {
            Combo,
            Burst,
            None
        }

        public class BeforeAttackEventArgs : EventArgs
        {
            public AttackableUnit Target;
            public Obj_AI_Base Unit = ObjectManager.Player;
            private bool _process = true;

            public bool Process
            {
                get
                {
                    return _process;
                }
                set
                {
                    DisableNextAttack = !value;
                    _process = value;
                }
            }
        }

        public sealed class Orbwalker
        {
            private readonly AIHeroClient Player;
            private OrbwalkingMode _mode = OrbwalkingMode.None;
            private Vector3 _orbwalkingPoint;
            private static readonly List<Orbwalker> Instances = new List<Orbwalker>();

            public Orbwalker()
            {
                Move = true;
                Player = ObjectManager.Player;
                Game.OnUpdate += OnUpdate;
                Instances.Add(this);
            }

            internal static bool ManualCancel => getCheckBoxItem(comboMenu, "manualCancel");

            internal static bool CalcutalePing => getCheckBoxItem(comboMenu, "manualCancelPing");

            internal static AttackableUnit GetOrbTar { get; set; }

            public OrbwalkingMode ActiveMode
            {
                get
                {
                    if (this._mode != OrbwalkingMode.None)
                    {
                        return this._mode;
                    }

                    if (getKeyBindItem(comboMenu, "combokey"))
                    {
                        EloBuddy.SDK.Orbwalker.DisableAttacking = true;
                        EloBuddy.SDK.Orbwalker.DisableMovement = true;
                        return OrbwalkingMode.Combo;
                    }
                    else
                    {
                        EloBuddy.SDK.Orbwalker.DisableAttacking = false;
                        EloBuddy.SDK.Orbwalker.DisableMovement = false;
                    }
                    if (getKeyBindItem(miscMenu, "shycombo"))
                    {
                        EloBuddy.SDK.Orbwalker.DisableAttacking = true;
                        EloBuddy.SDK.Orbwalker.DisableMovement = true;
                        return OrbwalkingMode.Burst;
                    }
                    else
                    {
                        EloBuddy.SDK.Orbwalker.DisableAttacking = false;
                        EloBuddy.SDK.Orbwalker.DisableMovement = false;
                    }

                    return OrbwalkingMode.None;
                }
                set
                {
                    _mode = value;
                }
            }

            private void OnUpdate(EventArgs args)
            {
                orbwalkerMode();

                if (ActiveMode == OrbwalkingMode.None)
                {
                    return;
                }

                var target = GetTarget();
                GetOrbTar = target;

                Orbwalk(
                   target, _orbwalkingPoint.To2D().IsValid() ? _orbwalkingPoint : Game.CursorPos,
                   80,
                   70);
            }

            private void orbwalkerMode()
            {
                isNone = ActiveMode == OrbwalkingMode.None;
                isCombo = ActiveMode == OrbwalkingMode.Combo;
                isBurst = ActiveMode == OrbwalkingMode.Burst;
            }

            public AttackableUnit GetTarget()
            {
                AttackableUnit result = null;
                var mode = ActiveMode;

                {
                    if (!ObjectManager.Player.UnderTurret(true) || mode == OrbwalkingMode.Combo || mode == OrbwalkingMode.Burst)
                    {
                        var target = TargetSelector.GetTarget(from h in EntityManager.Heroes.Enemies
                                                              where h.IsValidTarget() && EloBuddy.Player.Instance.IsInAutoAttackRange(h) && (!EloBuddy.SDK.Orbwalker.IsRanged || Prediction.Position.Collision.GetYasuoWallCollision(EloBuddy.Player.Instance.ServerPosition, h.ServerPosition) == Vector3.Zero)
                                                              select h, DamageType.Physical);

                        if (target.IsValidTarget() && InAutoAttackRange(target))
                        {
                            return target;
                        }
                    }
                }

                return result;
            }           
        }
    }
}
