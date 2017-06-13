using CSRsyndra;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace CSRsyndra
{
    internal static class EventsManager
    {
        public static Vector3 SpherePos;
        public static void Initialize()
        {
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.Q)
            {
                SpherePos = args.End;
            }
        }

        private static void Gapcloser_OnGapcloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {
            if (sender.IsEnemy && sender is AIHeroClient && SpellsManager.E.IsReady() && Menus.MiscMenu["GapCloser"].Cast<CheckBox>().CurrentValue)
            {
                SpellsManager.E.Cast(sender.Position);
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {

            if (args.DangerLevel == DangerLevel.High && sender.IsEnemy && sender is AIHeroClient && SpellsManager.E.IsReady() && Menus.MiscMenu["Interrupt"].Cast<CheckBox>().CurrentValue)
            {
                SpellsManager.E.Cast(sender);
            }
        }
    }
}