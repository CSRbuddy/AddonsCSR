﻿using System;
using System.Linq;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Constants;
using SharpDX;

namespace CSRriven
{
    class Program
    {
        public static Spell.Active Q;
        public static Spell.Active E;
        public static Spell.Skillshot R;
        public static Spell.Active W;
        public static Spell.Active R1;

        static Spell.Targeted Smite = null;
        public static bool EnableR;
        public static int LastCastQ;
        public static int LastCastW;
        private static int lastwd;
        private static readonly float _barLength = 104;
        private static readonly float _xOffset = 2;
        private static readonly float _yOffset = 9;
        private static bool ssfl;
        public static int QCount;
        public static Menu Menu, FarmingMenu, MiscMenu, DrawMenu, HarassMenu, ComboMenu, Skin, DelayMenu, SmiteMenu;
        static Item Healthpot;
        public static SpellSlot SmiteSlot = SpellSlot.Unknown;
        public static SpellSlot IgniteSlot = SpellSlot.Unknown;
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
        private static Spell.Targeted _ignite;

        public static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
            if (Player.Instance.Hero != Champion.Riven) return;
            YasuoWallManager.Initialize();
            WallJump.InitSpots();
            Itemlogic.Init();
           
        }
        
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }


        }

        private static string Smitetype
        {
            get
            {
                if (SmiteBlue.Any(i => Item.HasItem(i)))
                    return "s5_summonersmiteplayerganker";

                if (SmiteRed.Any(i => Item.HasItem(i)))
                    return "s5_summonersmiteduel";

                if (SmiteGrey.Any(i => Item.HasItem(i)))
                    return "s5_summonersmitequick";

                if (SmitePurple.Any(i => Item.HasItem(i)))
                    return "itemsmiteaoe";

                return "summonersmite";
            }
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Riven") return;

            Q = new Spell.Active(SpellSlot.Q, 300);
            E = new Spell.Active(SpellSlot.E, 325);
            R = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 45)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Active(SpellSlot.W,
                        (uint)
                            (70 + Player.Instance.BoundingRadius +
                             (Player.Instance.HasBuff("RivenFengShuiEngine") ? 195 : 120)));
            R1 = new Spell.Active(SpellSlot.R);



            Healthpot = new Item(2003, 0);
            if (HasSpell("SummonerDot"))
            {
                _ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            }
            Chat.Print("Welcome, CSR Riven Lodead. ", Color.Brown);
            Menu = MainMenu.AddMenu("CSR Riven", "CSR Riven");

            ComboMenu = Menu.AddSubMenu("+ Combo Settings +", "ComboSettings");
            ComboMenu.AddLabel("Combo Settings");
            ComboMenu.Add("QCombo", new CheckBox("Use Q"));
            ComboMenu.Add("WCombo", new CheckBox("Use W"));
            ComboMenu.Add("ECombo", new CheckBox("Use E"));
            ComboMenu.Add("RCombo", new CheckBox("Use R"));
            ComboMenu.Add("R2Combo", new CheckBox("Use R2(enemy killable)"));
            ComboMenu.Add("FlashW", new KeyBind("Flash W", false, KeyBind.BindTypes.HoldActive, '5'));
            ComboMenu.Add("FlashBurst", new KeyBind("Burst", false, KeyBind.BindTypes.HoldActive, 'G'));
            ComboMenu.AddLabel("Burst = Select Target And Burst Key");
            ComboMenu.AddLabel("The flash has usesh");
            ComboMenu.AddLabel("If not perform without a flash");
            ComboMenu.Add("ForcedR", new KeyBind("Forced R", true, KeyBind.BindTypes.PressToggle, 'Z'));
            ComboMenu.Add("useTiamat", new CheckBox("Use Items"));
            ComboMenu.AddLabel("R Settings");
            ComboMenu.Add("RCantKill", new CheckBox("Cant Kill with Combo", false));
            ComboMenu.Add("REnemyCount", new Slider("Enemy Count >= ", 0, 0, 4));

            HarassMenu = Menu.AddSubMenu("+ Harass Settings +", "HarassSettings");
            HarassMenu.AddLabel("Harass Settings");
            HarassMenu.Add("QHarass", new CheckBox("Use Q"));
            HarassMenu.Add("WHarass", new CheckBox("Use W"));
            HarassMenu.Add("EHarass", new CheckBox("Use E"));
            var Style = HarassMenu.Add("harassstyle", new Slider("Harass Style", 0, 0, 2));
            Style.OnValueChange += delegate
            {
                Style.DisplayName = "Harass Style: " + new[] { "Q,Q,W,Q and E back", "E,H,Q3,W", "E,H,AA,Q,W" }[Style.CurrentValue];
            };
            Style.DisplayName = "Harass Style: " + new[] { "Q,Q,W,Q and E back", "E,H,Q3,W", "E,H,AA,Q,W" }[Style.CurrentValue];

            FarmingMenu = Menu.AddSubMenu("+ Farm Settings +", "FarmSettings");
            FarmingMenu.AddLabel("Lane Clear");
            FarmingMenu.Add("QLaneClear", new CheckBox("Use Q LaneClear"));
            FarmingMenu.Add("WLaneClear", new CheckBox("Use W LaneClear"));
            FarmingMenu.Add("ELaneClear", new CheckBox("Use E LaneClear"));

            FarmingMenu.AddLabel("Jungle Clear");
            FarmingMenu.Add("QJungleClear", new CheckBox("Use Q in Jungle"));
            FarmingMenu.Add("WJungleClear", new CheckBox("Use W in Jungle"));
            FarmingMenu.Add("EJungleClear", new CheckBox("Use E in Jungle"));

            FarmingMenu.AddLabel("Last Hit");
            FarmingMenu.Add("Qlasthit", new CheckBox("Use Q LastHit"));
            FarmingMenu.Add("Wlasthit", new CheckBox("Use W LastHit"));
            FarmingMenu.Add("Elasthit", new CheckBox("Use E LastHit"));

            MiscMenu = Menu.AddSubMenu("+ More Settings +", "Misc");
            MiscMenu.AddLabel("Auto");
            MiscMenu.Add("UseShield", new CheckBox("Use Shield(E)"));
            MiscMenu.Add("AutoIgnite", new CheckBox("Auto Ignite"));
            MiscMenu.Add("AutoQSS", new CheckBox("Auto QSS"));
            MiscMenu.Add("AutoW", new CheckBox("Auto W"));
            MiscMenu.AddLabel("Keep Alive Settings");
            MiscMenu.Add("Alive.Q", new CheckBox("Keep Q Alive"));
            MiscMenu.Add("Alive.R", new CheckBox("Use R2 Before Expire"));
            MiscMenu.AddLabel("Activator");
            MiscMenu.Add("useHP", new CheckBox("Use Health Potion"));
            MiscMenu.Add("useHPV", new Slider("HP < %", 45, 0, 100));
            MiscMenu.Add("useElixir", new CheckBox("Use Elixir"));
            MiscMenu.Add("useElixirCount", new Slider("EnemyCount > ", 1, 0, 4));
            MiscMenu.Add("useCrystal", new CheckBox("Use Refillable Potions"));

            MiscMenu.AddGroupLabel("Auto Level UP(No Work, wait I Fix)");
            MiscMenu.Add("activateAutoLVL", new CheckBox("Activate Auto Leveler", false));
            MiscMenu.AddLabel("The Auto Leveler will always Focus R than the rest of the Spells");
            MiscMenu.Add("firstFocus", new ComboBox("1 Spell to Focus", new List<string> { "Q", "E", "W" }));
            MiscMenu.Add("secondFocus", new ComboBox("2 Spell to Focus", new List<string> { "Q", "E", "W" }, 1));
            MiscMenu.Add("thirdFocus", new ComboBox("3 Spell to Focus", new List<string> { "Q", "E", "W" }, 2));
            MiscMenu.Add("delaySlider", new Slider("Delay Slider", 200, 150, 500));

            DelayMenu = Menu.AddSubMenu("+ Humanizer +", "Delay");
            DelayMenu.Add("useHumanizer", new CheckBox("Use Humanizer?", false));
            DelayMenu.Add("spell1a1b", new Slider("Q1,Q2 Delay(ms)", 290, 100, 400));
            DelayMenu.Add("spell1c", new Slider("Q3 Delay(ms)", 390, 100, 400));
            DelayMenu.Add("spell2", new Slider("W Delay(ms)", 120, 100, 400));
            DelayMenu.Add("spell4a", new Slider("R Delay(ms)", 0, 0, 400));
            DelayMenu.Add("spell4b", new Slider("R2 Delay(ms)", 100, 50, 400));

            Skin = Menu.AddSubMenu("+ Skin Changer +", "SkinChanger");
            Skin.Add("checkSkin", new CheckBox("Use Skin Changer"));
            Skin.Add("skin.Id", new Slider("Skin", 4, 0, 6));

            DrawMenu = Menu.AddSubMenu("+ Draw Settings +", "Drawings");
            DrawMenu.Add("drawStatus", new CheckBox("Draw Status"));
            DrawMenu.Add("drawCombo", new CheckBox("Draw Combo Range"));
            DrawMenu.Add("drawFBurst", new CheckBox("Draw Flash Burst Range"));
            DrawMenu.Add("DrawDamage", new CheckBox("Draw Damage Bar"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Obj_AI_Base.OnProcessSpellCast += OnCasting;
        }

        private static bool InWRange(GameObject target) => (Player.HasBuff("RivenFengShuiEngine") && target != null) ?
                    330 >= _Player.Distance(target.Position) : 265 >= _Player.Distance(target.Position);

        private static void OnCasting(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == Player.Instance.Type && MiscMenu["UseShield"].Cast<CheckBox>().CurrentValue)
            {
                var epos = Player.Instance.ServerPosition +
                          (Player.Instance.ServerPosition - sender.ServerPosition).Normalized() * 300;

                if (Player.Instance.Distance(sender.ServerPosition) <= args.SData.CastRange)
                {
                    switch (args.SData.TargettingType)
                    {
                        case SpellDataTargetType.Unit:

                            if (args.Target.NetworkId == _Player.NetworkId)
                            {
                                if (!args.SData.Name.Contains("NasusW"))
                                {
                                    if (E.IsReady()) Player.CastSpell(SpellSlot.E, epos);
                                }
                            }

                            break;
                        case SpellDataTargetType.SelfAoe:

                            if (E.IsReady()) Player.CastSpell(SpellSlot.E, epos);

                            break;
                    }
                    if (args.SData.Name.Contains("IreliaEquilibriumStrike"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (W.IsReady() && InWRange(sender)) W.Cast();
                            else if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("TalonCutthroat"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RenektonPreExecute"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("GarenRPreCast"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("GarenQAttack"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("XenZhaoThrust3"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarQ"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDash"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDashAADummy"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("TwitchEParticle"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("FizzPiercingStrike"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("HungeringStrike"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaRTrigger"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (W.IsReady() && InWRange(sender)) W.Cast();
                            else if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaE"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (W.IsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingSpinToWin"))
                    {
                        if (args.Target.NetworkId == _Player.NetworkId)
                        {
                            if (E.IsReady()) E.Cast();
                            else if (W.IsReady()) W.Cast();
                        }
                    }
                }
            }
        }

        private static void DoQSS()
        {
            if (!MiscMenu["AutoQSS"].Cast<CheckBox>().CurrentValue) return;

            if (Item.HasItem(3139) && Item.CanUseItem(3139) && ObjectManager.Player.CountEnemyChampionsInRange(1800) > 0)
            {
                Core.DelayAction(() => Item.UseItem(3139), 1);
            }

            if (Item.HasItem(3140) && Item.CanUseItem(3140) && ObjectManager.Player.CountEnemyChampionsInRange(1800) > 0)
            {
                Core.DelayAction(() => Item.UseItem(3140), 1);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            var HPpot = MiscMenu["useHP"].Cast<CheckBox>().CurrentValue;
            var HPv = MiscMenu["useHPv"].Cast<Slider>().CurrentValue;
            //            var t = TargetSelector.GetTarget(Smite.Range, DamageType.Magical);

            if (LastCastQ + 3600 < Environment.TickCount)
            {
                QCount = 0;
            }
            if (MiscMenu["Alive.Q"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsRecalling() && QCount < 3 && QCount > 0 && LastCastQ + 3480 < Environment.TickCount && Player.Instance.HasBuff("RivenTriCleaveBuff") && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Player.CastSpell(SpellSlot.Q,
                    Orbwalker.LastTarget != null && Orbwalker.LastAutoAttack - Environment.TickCount < 3000
                        ? Orbwalker.LastTarget.Position
                        : Game.CursorPos);
                return;
            }
            foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies)
            {
                if (HPpot && Player.Instance.HealthPercent < HPv && _Player.Distance(enemy) < 2000)
                {
                    if (Item.HasItem(Healthpot.Id) && Item.CanUseItem(Healthpot.Id) && !Player.HasBuff("RegenerationPotion"))
                    {
                        Healthpot.Cast();
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
            Auto();
        }

        private static void Auto()
        {
            var w = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (w != null && w.IsValidTarget(W.Range) && MiscMenu["AutoW"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast();
            }
            if (_Player.HasBuffOfType(BuffType.Stun) || _Player.HasBuffOfType(BuffType.Taunt) || _Player.HasBuffOfType(BuffType.Polymorph) || _Player.HasBuffOfType(BuffType.Frenzy) || _Player.HasBuffOfType(BuffType.Fear) || _Player.HasBuffOfType(BuffType.Snare) || _Player.HasBuffOfType(BuffType.Suppression))
            {
                DoQSS();
            }
            if (HasSpell("SummonerDot"))
            {
                if (MiscMenu["AutoIgnite"].Cast<CheckBox>().CurrentValue)
                {
                    if (!_ignite.IsReady() || Player.Instance.IsDead) return;
                    foreach (
                        var source in
                            EntityManager.Heroes.Enemies
                                .Where(
                                    a => a.IsValidTarget(_ignite.Range) &&
                                        a.Health < 50 + 20 * Player.Instance.Level - (a.HPRegenRate / 5 * 3)))
                    {
                        _ignite.Cast(source);
                        return;
                    }
                }
            }
            if (_Player.SkinId != Skin["skin.Id"].Cast<Slider>().CurrentValue)
            {
                if (checkSkin())
                {
                    Player.SetSkinId(SkinId());
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name.ToLower().Contains(W.Name.ToLower()))
            {
                LastCastW = Environment.TickCount;
                return;
            }
            if (args.Target is Obj_AI_Turret || args.Target is Obj_Barracks || args.Target is Obj_BarracksDampener ||
                args.Target is Obj_Building)
                if (args.Target.IsValid && args.Target != null && Q.IsReady() && FarmingMenu["QLaneClear"].Cast<CheckBox>().CurrentValue &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                    Player.CastSpell(SpellSlot.Q, (Obj_AI_Base)args.Target);
            AIHeroClient client = args.Target as AIHeroClient;
            if (client != null)
            {
                var target = client;
                if (!target.IsValidTarget()) return;
                if (ComboMenu["FlashBurst"].Cast<KeyBind>().CurrentValue)
                {
                    Core.DelayAction(ForceItem, 50);

                    if (R.IsReady() && R.Name == "rivenizunablade")
                    {
                        ssfl = false;
                        Core.DelayAction(ForceItem, 50);
                        ForceR();
                        R.Cast(target);
                    }
                    else if (Q.IsReady())
                    {
                        Core.DelayAction(ForceItem, 50);
                        Player.CastSpell(SpellSlot.Q, target.Position);
                    }
                    return;
                }
            }
            if (args.SData.Name.ToLower().Contains(Q.Name.ToLower()))
            {
                LastCastQ = Environment.TickCount;
                if (!MiscMenu["Alive.Q"].Cast<CheckBox>().CurrentValue) return;
                Core.DelayAction(() =>
                {
                    if (!Player.Instance.IsRecalling() && QCount <= 2)
                    {
                        Player.CastSpell(SpellSlot.Q,
                            Orbwalker.LastTarget != null && Orbwalker.LastAutoAttack - Environment.TickCount < 3000
                                ? Orbwalker.LastTarget.Position
                                : Game.CursorPos);
                    }
                }, 3480);
                return;
            }
        }

        private static void ForceItem()
        {
            if (Item.HasItem(3074) && Item.CanUseItem(3074))
            {
                Item.UseItem(3074);
                return;
            }
            else if (Item.HasItem(3077) && Item.CanUseItem(3077))
            {
                Item.UseItem(3077);
                return;
            }
        }

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;
            var t = 0;
            switch (args.Animation)
            {
                case "Spell1a":
                    if (DelayMenu["useHumanizer"].Cast<CheckBox>().CurrentValue)
                    {
                        t = DelayMenu["spell1a1b"].Cast<Slider>().CurrentValue;
                        QCount = 1;
                    }
                    else
                    {
                        t = 290;
                        QCount = 1;
                    }
                    break;
                case "Spell1b":
                    if (DelayMenu["useHumanizer"].Cast<CheckBox>().CurrentValue)
                    {
                        t = DelayMenu["spell1a1b"].Cast<Slider>().CurrentValue;
                        QCount = 2;
                    }
                    else
                    {
                        t = 290;
                        QCount = 2;
                    }
                    break;
                case "Spell1c":
                    if (DelayMenu["useHumanizer"].Cast<CheckBox>().CurrentValue)
                    {
                        t = DelayMenu["spell1c"].Cast<Slider>().CurrentValue;
                        QCount = 0;
                    }
                    else
                    {
                        t = 390;
                        QCount = 0;
                    }
                    break;
                case "Spell2":
                    if (DelayMenu["useHumanizer"].Cast<CheckBox>().CurrentValue)
                    {
                        t = DelayMenu["spell2"].Cast<Slider>().CurrentValue;
                    }
                    else
                    {
                        t = 110;
                    }
                    break;
                case "Spell3":
                    break;
                case "Spell4a":
                    if (DelayMenu["useHumanizer"].Cast<CheckBox>().CurrentValue)
                    {
                        t = DelayMenu["spell4a"].Cast<Slider>().CurrentValue;
                    }
                    else
                    {
                        t = 0;
                    }
                    break;
                case "Spell4b":
                    if (DelayMenu["useHumanizer"].Cast<CheckBox>().CurrentValue)
                    {
                        t = DelayMenu["spell4b"].Cast<Slider>().CurrentValue;
                    }
                    else
                    {
                        t = 100;
                    }
                    break;
            }
            if (t != 0 && ((Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None) || ComboMenu["FlashBurst"].Cast<KeyBind>().CurrentValue))
            {
                Orbwalker.ResetAutoAttack();
                Core.DelayAction(CancelAnimation, t - Game.Ping);
            }
        }

        private static void CancelAnimation()
        {
            Player.DoEmote(Emote.Dance);
            Orbwalker.ResetAutoAttack();
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            var target = args.Target as Obj_AI_Base;

            // Hydra
            if (args.SData.Name.ToLower().Contains("itemtiamatcleave"))
            {
                Orbwalker.ResetAutoAttack();
                if (W.IsReady())
                {
                    var target2 = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                    if (target2 != null || Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None)
                    {
                        Player.CastSpell(SpellSlot.W);
                    }
                }
                return;
            }

            //W
            if (args.SData.Name.ToLower().Contains(W.Name.ToLower()))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (Player.Instance.HasBuff("RivenFengShuiEngine") && R.IsReady() &&
                        ComboMenu["R2Combo"].Cast<CheckBox>().CurrentValue)
                    {
                        ssfl = true;
                        var target2 = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                        if (target2 != null &&
                            (target2.Distance(Player.Instance) < W.Range &&
                             target2.Health >
                             Player.Instance.CalculateDamageOnUnit(target2, DamageType.Physical, Damage.WDamage()) ||
                             target2.Distance(Player.Instance) > W.Range) &&
                            Player.Instance.CalculateDamageOnUnit(target2, DamageType.Physical,
                                Damage.RDamage(target2)) > target2.Health)
                        {
                            R.Cast(target2);
                        }
                    }
                }

                target = (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                          Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                    ? TargetSelector.GetTarget(E.Range + W.Range, DamageType.Physical)
                    : (Obj_AI_Base)Orbwalker.LastTarget;
                if (Q.IsReady() && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None || ComboMenu["FlashBurst"].Cast<KeyBind>().CurrentValue && target != null)
                {
                    Player.CastSpell(SpellSlot.Q, target.Position);
                    return;
                }
                return;
            }

            //E
            if (args.SData.Name.ToLower().Contains(E.Name.ToLower()))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (Player.Instance.HasBuff("RivenFengShuiEngine") && R.IsReady() &&
                        ComboMenu["R2Combo"].Cast<CheckBox>().CurrentValue)
                    {
                        ssfl = true;
                        var target2 = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                        if (target2 != null &&
                            Player.Instance.CalculateDamageOnUnit(target2, DamageType.Physical,
                                (Damage.RDamage(target2))) > target2.Health)
                        {
                            R.Cast(target2);
                            return;
                        }
                    }
                    if ((EnableR == true) && R.IsReady() &&
                        !Player.Instance.HasBuff("RivenFengShuiEngine") &&
                        ComboMenu["RCombo"].Cast<CheckBox>().CurrentValue)
                    {
                        ssfl = false;
                        ForceR();
                        Player.CastSpell(SpellSlot.R);
                    }
                    target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                    if (target != null && Player.Instance.Distance(target) < W.Range)
                    {
                        Player.CastSpell(SpellSlot.W);
                        return;
                    }
                }
            }

            //Q
            if (args.SData.Name.ToLower().Contains(Q.Name.ToLower()))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (Player.Instance.HasBuff("RivenFengShuiEngine") && R.IsReady() &&
                        ComboMenu["R2Combo"].Cast<CheckBox>().CurrentValue)
                    {
                        var target2 = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                        if (target2 != null &&
                            (target2.Distance(Player.Instance) < 300 &&
                             target2.Health >
                             Player.Instance.CalculateDamageOnUnit(target2, DamageType.Physical, Damage.QDamage()) ||
                             target2.Distance(Player.Instance) > 300) &&
                            Player.Instance.CalculateDamageOnUnit(target2, DamageType.Physical,
                                Damage.RDamage(target2)) > target2.Health)
                        {
                            R.Cast(target2);
                        }
                    }
                }
                return;
            }

            if (args.SData.IsAutoAttack() && target != null)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    ComboAfterAa(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    HarassAfterAa(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    JungleAfterAa(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && target.IsMinion())
                {
                    LastHitAfterAa(target);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && target.IsMinion())
                {
                    LaneClearAfterAa(target);
                }
            }
        }

        private static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                float passivenhan;
                if (_Player.Level >= 18)
                {
                    passivenhan = 0.5f;
                }
                else if (_Player.Level >= 15)
                {
                    passivenhan = 0.45f;
                }
                else if (_Player.Level >= 12)
                {
                    passivenhan = 0.4f;
                }
                else if (_Player.Level >= 9)
                {
                    passivenhan = 0.35f;
                }
                else if (_Player.Level >= 6)
                {
                    passivenhan = 0.3f;
                }
                else if (_Player.Level >= 3)
                {
                    passivenhan = 0.25f;
                }
                else
                {
                    passivenhan = 0.2f;
                }
                if (Item.HasItem(3074)) damage = damage + _Player.GetAutoAttackDamage(enemy) * 0.7f;
                if (W.IsReady()) damage = damage + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.W);
                if (Q.IsReady())
                {
                    var qnhan = 4 - QCount;
                    damage = damage + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.Q) * qnhan +
                             _Player.GetAutoAttackDamage(enemy) * qnhan * (1 + passivenhan);
                }
                damage = damage + _Player.GetAutoAttackDamage(enemy) * (1 + passivenhan);
                if (R.IsReady())
                {
                    return damage * 1.2f + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.R);
                }

                return damage;
            }
            return 0;
        }

        public static void ComboAfterAa(Obj_AI_Base target)
        {
            try
            {
                if (Player.Instance.HasBuff("RivenFengShuiEngine") && R.IsReady() &&
                    ComboMenu["R2Combo"].Cast<CheckBox>().CurrentValue)
                {
                    if (Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, Damage.RDamage(target)) + Player.Instance.GetAutoAttackDamage(target, true) > target.Health)
                    {
                        ssfl = true;
                        R.Cast(target);
                        return;
                    }
                }
                if (ComboMenu["WCombo"].Cast<CheckBox>().CurrentValue && W.IsReady() &&
                    W.IsInRange(target))
                {
                    Core.DelayAction(ForceItem, 50);
                    Player.CastSpell(SpellSlot.W);
                    return;
                }
                if (ComboMenu["QCombo"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    Player.CastSpell(SpellSlot.Q, target.Position);
                    return;
                }
                Core.DelayAction(ForceItem, 50);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public static void HarassAfterAa(Obj_AI_Base target)
        {
            if (HarassMenu["WHarass"].Cast<CheckBox>().CurrentValue && W.IsReady() &&
                W.IsInRange(target))
            {
                Core.DelayAction(ForceItem, 50);
                Player.CastSpell(SpellSlot.W);
                return;
            }
            if (HarassMenu["QHarass"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Player.CastSpell(SpellSlot.Q, target.Position);
                return;
            }
            Core.DelayAction(ForceItem, 50);
        }

        public static void LastHitAfterAa(Obj_AI_Base target)
        {

            var unitHp = target.Health - Player.Instance.GetAutoAttackDamage(target, true);
            if (unitHp > 0)
            {
                if (FarmingMenu["QLastHit"].Cast<CheckBox>().CurrentValue && Q.IsReady() &&
                    Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, Damage.QDamage()) >
                    unitHp)
                {
                    Player.CastSpell(SpellSlot.Q, target.Position);
                    return;
                }
                if (FarmingMenu["WLastHit"].Cast<CheckBox>().CurrentValue && W.IsReady() &&
                    W.IsInRange(target) &&
                    Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, Damage.WDamage()) >
                    unitHp)
                {
                    Player.CastSpell(SpellSlot.W);
                }
            }

        }

        public static void LaneClearAfterAa(Obj_AI_Base target)
        {
            try
            {
                var unitHp = target.Health - Player.Instance.GetAutoAttackDamage(target, true);
                if (unitHp > 0)
                {
                    if (FarmingMenu["QLaneClear"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                    {
                        Player.CastSpell(SpellSlot.Q, target.Position);
                        return;
                    }
                    if (FarmingMenu["WLaneClear"].Cast<CheckBox>().CurrentValue && W.IsReady() &&
                        W.IsInRange(target))
                    {
                        Player.CastSpell(SpellSlot.W);
                        return;
                    }
                }
                else
                {
                    List<Obj_AI_Minion> minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.Position, Q.Range).Where(a => a.NetworkId != target.NetworkId).ToList();
                    if (FarmingMenu["QLaneClear"].Cast<CheckBox>().CurrentValue && Q.IsReady() && minions.Any())
                    {
                        Player.CastSpell(SpellSlot.Q, minions[0].Position);
                        return;
                    }
                    minions = minions.Where(a => a.Distance(Player.Instance) < W.Range).ToList();
                    if (FarmingMenu["WLaneClear"].Cast<CheckBox>().CurrentValue && W.IsReady() &&
                        W.IsInRange(target) && minions.Any())
                    {
                        Player.CastSpell(SpellSlot.W);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
        }

        public static void JungleAfterAa(Obj_AI_Base target)
        {

            {
                if (FarmingMenu["WJungleClear"].Cast<CheckBox>().CurrentValue && W.IsReady() &&
                    W.IsInRange(target))
                {
                    Core.DelayAction(ForceItem, 50);
                    Player.CastSpell(SpellSlot.W);
                    return;
                }
                if (FarmingMenu["QJungleClear"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    Player.CastSpell(SpellSlot.Q, target.Position);
                    return;
                }
                Core.DelayAction(ForceItem, 50);
            }
        }

        public static int SkinId()
        {
            return Skin["skin.Id"].Cast<Slider>().CurrentValue;
        }

        public static bool checkSkin()
        {
            return Skin["checkSkin"].Cast<CheckBox>().CurrentValue;
        }

        private static void Combo()
        {

            if (Orbwalker.IsAutoAttacking) return;
            var target = TargetSelector.GetTarget(E.Range + W.Range + 200, DamageType.Physical);
            if (target == null) return;
            var useQ = ComboMenu["QCombo"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["WCombo"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["ECombo"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["RCombo"].Cast<CheckBox>().CurrentValue;
            var useItem = ComboMenu["useTiamat"].Cast<CheckBox>().CurrentValue;
            EnableR = false;
            try
            {
                if (R.IsReady() && Player.Instance.HasBuff("RivenFengShuiEngine") &&
                     ComboMenu["R2Combo"].Cast<CheckBox>().CurrentValue)
                {
                    if (EntityManager.Heroes.Enemies.Where(
                            enemy =>
                                enemy.IsValidTarget(R.Range) &&
                                enemy.Health <
                                Player.Instance.CalculateDamageOnUnit(enemy, DamageType.Physical,
                                    Damage.RDamage(enemy))).Any(enemy => R.Cast(enemy)))
                    {
                        ssfl = true;
                        return;
                    }
                }

                if (ComboMenu["RCombo"].Cast<CheckBox>().CurrentValue && R.IsReady() && !Player.Instance.HasBuff("RivenFengShuiEngine"))
                {
                    if ((ComboMenu["RCantKill"].Cast<CheckBox>().CurrentValue &&
                        target.Health > Damage.ComboDamage(target, true)
                        && target.Health < Damage.ComboDamage(target)
                        && target.Health > Player.Instance.GetAutoAttackDamage(target, true) * 2) ||
                        (ComboMenu["REnemyCount"].Cast<Slider>().CurrentValue > 0 &&
                        Player.Instance.CountEnemyChampionsInRange(600) >= ComboMenu["REnemyCount"].Cast<Slider>().CurrentValue) || IsRActive)
                    {
                        ssfl = false;
                        EnableR = true;
                        ForceR();
                    }
                    if (ComboMenu["ForcedR"].Cast<KeyBind>().CurrentValue)
                    {
                        ssfl = false;
                        EnableR = true;
                        ForceR();
                    }
                }

                if (ComboMenu["ECombo"].Cast<CheckBox>().CurrentValue && target.Distance(Player.Instance) > W.Range && E.IsReady())
                {
                    if (Item.HasItem(3142) && Item.CanUseItem(3142))
                    {
                        Item.UseItem(3142);
                    }
                    Player.CastSpell(SpellSlot.E, target.Position);
                    return;
                }

                if (ComboMenu["ECombo"].Cast<CheckBox>().CurrentValue && target.Distance(Player.Instance) < W.Range && E.IsReady())
                {
                    Player.CastSpell(SpellSlot.E, Game.CursorPos);
                    return;
                }

                if (ComboMenu["WCombo"].Cast<CheckBox>().CurrentValue &&
                target.Distance(Player.Instance) <= W.Range && W.IsReady())
                {
                    Core.DelayAction(ForceItem, 50);
                    Player.CastSpell(SpellSlot.W);
                    return;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void Flee()
        {
            var x = _Player.Position.Extend(Game.CursorPos, 300);
            if (Q.IsReady() && !_Player.IsDashing()) Player.CastSpell(SpellSlot.Q, Game.CursorPos);
            if (E.IsReady() && !_Player.IsDashing()) Player.CastSpell(SpellSlot.E, x.To3D());

            var target = TargetSelector.GetTarget(E.Range + W.Range + 200, DamageType.Physical);
            if (target == null) return;
            if (target.Distance(Player.Instance) <= W.Range && W.IsReady())
            {
                Core.DelayAction(ForceItem, 50);
                Player.CastSpell(SpellSlot.W);
                return;

            }

        }

        public static void Harass()
        {
            if (Orbwalker.IsAutoAttacking) return;

            var target = TargetSelector.GetTarget(E.Range + W.Range, DamageType.Physical);

            {
                if (target == null) return;

                if (HarassMenu["EHarass"].Cast<CheckBox>().CurrentValue &&
                    (target.Distance(Player.Instance) > W.Range &&
                     target.Distance(Player.Instance) < E.Range + W.Range ||
                     IsRActive && R.IsReady() &&
                     target.Distance(Player.Instance) < E.Range + 265 + Player.Instance.BoundingRadius) &&
                    E.IsReady())
                {
                    Player.CastSpell(SpellSlot.E, target.Position);
                    return;
                }

                if (HarassMenu["WHarass"].Cast<CheckBox>().CurrentValue &&
                    target.Distance(Player.Instance) <= W.Range && W.IsReady())
                {
                    ForceItem();
                    Player.CastSpell(SpellSlot.W);
                    return;
                }
            }
        }

        private static void JungleClear()
        {
            var minion =
                 EntityManager.MinionsAndMonsters.Monsters.OrderByDescending(a => a.MaxHealth)
                     .FirstOrDefault(a => a.Distance(Player.Instance) < Player.Instance.GetAutoAttackRange(a));

            {
                if (minion == null) return;

                if (FarmingMenu["QJungleClear"].Cast<CheckBox>().CurrentValue && Q.IsReady() &&
                       minion.Health <=
                       Player.Instance.CalculateDamageOnUnit(minion, DamageType.Physical, Damage.QDamage()))
                {
                    Player.CastSpell(SpellSlot.Q, minion.Position);
                    return;
                }

                if (FarmingMenu["EJungleClear"].Cast<CheckBox>().CurrentValue && (!W.IsReady() && !Q.IsReady() || Player.Instance.HealthPercent < 20) && E.IsReady() &&
                    LastCastW + 750 < Environment.TickCount)
                {
                    Player.CastSpell(SpellSlot.E, minion.Position);
                }
            }
        }

        public static void LaneClear()
        {
            try
            {
                Orbwalker.ForcedTarget = null;
                {
                    if (Orbwalker.IsAutoAttacking || LastCastQ + 260 > Environment.TickCount) return;
                    foreach (
                        var minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(a => a.IsValidTarget(W.Range)))
                    {
                        if (FarmingMenu["QLaneClear"].Cast<CheckBox>().CurrentValue && Q.IsReady() &&
                            minion.Health <=
                            Player.Instance.CalculateDamageOnUnit(minion, DamageType.Physical, Damage.QDamage()))
                        {
                            Player.CastSpell(SpellSlot.Q, minion.Position);
                            return;
                        }
                        if (FarmingMenu["WLaneClear"].Cast<CheckBox>().CurrentValue && W.IsReady() &&
                            minion.Health <=
                            Player.Instance.CalculateDamageOnUnit(minion, DamageType.Physical, Damage.WDamage()))
                        {
                            Player.CastSpell(SpellSlot.W);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

        }

        public static void LastHit()
        {
            Orbwalker.ForcedTarget = null;
            {
                if (Orbwalker.IsAutoAttacking) return;

                foreach (
                    var minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(a => a.IsValidTarget(W.Range)))
                {
                    if (FarmingMenu["QLastHit"].Cast<CheckBox>().CurrentValue && Q.IsReady() &&
                        minion.Health <=
                        Player.Instance.CalculateDamageOnUnit(minion, DamageType.Physical, Damage.QDamage()))
                    {
                        Player.CastSpell(SpellSlot.Q, minion.Position);
                        return;
                    }
                    if (FarmingMenu["WLastHit"].Cast<CheckBox>().CurrentValue && W.IsReady() &&
                        minion.Health <=
                        Player.Instance.CalculateDamageOnUnit(minion, DamageType.Physical, Damage.WDamage()))
                    {
                        Player.CastSpell(SpellSlot.W);
                        return;
                    }
                }
            }

        }

        public static bool IsRActive
        {
            get
            {
                return ComboMenu["ForcedR"].Cast<KeyBind>().CurrentValue &&
                       ComboMenu["RCombo"].Cast<CheckBox>().CurrentValue;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawCombo"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(SharpDX.Color.Red, Q.Range + E.Range, _Player.Position);
            }
            if (DrawMenu["drawFBurst"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(SharpDX.Color.Green, 425 + E.Range, _Player.Position);
            }
            if (DrawMenu["drawStatus"].Cast<CheckBox>().CurrentValue)
            {
                var pos = Drawing.WorldToScreen(Player.Instance.Position);
                Drawing.DrawText((int)pos.X - 45, (int)pos.Y + 40, Color.DarkGray, "Forced R: " + IsRActive);
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (_Player.IsDead)
                return;
            if (!DrawMenu["DrawDamage"].Cast<CheckBox>().CurrentValue) return;
            foreach (var aiHeroClient in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValid && h.IsHPBarRendered && h.IsEnemy))
            {
                if (aiHeroClient.Distance(_Player) < 5000)
                {
                    var pos = new Vector2(aiHeroClient.HPBarPosition.X + _xOffset, aiHeroClient.HPBarPosition.Y + _yOffset);
                    var fullbar = (_barLength) * (aiHeroClient.HealthPercent / 100);
                    var damage = (_barLength) *
                                     ((getComboDamage(aiHeroClient) / aiHeroClient.MaxHealth) > 1
                                         ? 1
                                         : (getComboDamage(aiHeroClient) / aiHeroClient.MaxHealth));
                    Line.DrawLine(Color.Gray, 9f, new Vector2(pos.X, pos.Y),
                        new Vector2(pos.X + (damage > fullbar ? fullbar : damage), pos.Y));
                    Line.DrawLine(Color.Black, 9, new Vector2(pos.X + (damage > fullbar ? fullbar : damage) - 2, pos.Y), new Vector2(pos.X + (damage > fullbar ? fullbar : damage) + 2, pos.Y));
                }
                else
                {
                    return;
                }
            }
        }

        private static void ForceR()
        {
            ssfl = (R.IsReady() && R.Name == "RivenFengShuiEngine" && R1.Cast());
            Core.DelayAction(() => ssfl = true, 500);
        }

    }
}