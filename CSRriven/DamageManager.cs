using EloBuddy;
using EloBuddy.SDK;
using System;

namespace CSRriven
{
    class DamageManager : MenuBase
    {
        #region Riven: Math/Damage

        internal static float GetComboDamage(Obj_AI_Base target)
        {
            if (target == null || target.IsDead || target.IsZombie)
            {
                return 0;
            }

            var damage = 0d;

            //damage += GetIgniteDmage(target);
            damage += GetQDamage(target);
            damage += GetWDamage(target);
            damage += GetEDamage(target);
            damage += GetRDamage(target);

            if (!ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                damage += ObjectManager.Player.GetAutoAttackDamage(target);
            }

            if (ObjectManager.Player.HasBuff("SummonerExhaust"))
            {
                damage = damage * 0.6f;
            }

            if (target.CharData.BaseSkinName == "Moredkaiser")
            {
                damage -= target.Mana;
            }

            if (target.HasBuff("GarenW"))
            {
                damage = damage * 0.7f;
            }

            if (target.HasBuff("ferocioushowl"))
            {
                damage = damage * 0.7f;
            }

            if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
            {
                damage -= target.Mana / 2f;
            }

            return (float)damage;
        }

        internal static double GetRivenPassive
        {
            get
            {
                if (ObjectManager.Player.Level == 18)
                {
                    return 0.5;
                }

                if (ObjectManager.Player.Level >= 15)
                {
                    return 0.45;
                }

                if (ObjectManager.Player.Level >= 12)
                {
                    return 0.4;
                }

                if (ObjectManager.Player.Level >= 9)
                {
                    return 0.35;
                }

                if (ObjectManager.Player.Level >= 6)
                {
                    return 0.3;
                }

                if (ObjectManager.Player.Level >= 3)
                {
                    return 0.25;
                }

                return 0.2;
            }
        }

        internal static float GetQDamage(Obj_AI_Base target)
        {
            if (SpellManager.Q.Level == 0 || !SpellManager.Q.IsReady())
            {
                return 0f;
            }

            var qhan = 3 - Qcount;

            return
                (float)
                (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) * qhan +
                 ObjectManager.Player.GetAutoAttackDamage(target) * qhan * (1 + GetRivenPassive));
        }

        internal static float GetWDamage(Obj_AI_Base target)
        {
            if (SpellManager.W.Level == 0 || !SpellManager.W.IsReady())
            {
                return 0f;
            }

            return (float)ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
        }

        internal static float GetEDamage(Obj_AI_Base target)
        {
            if (SpellManager.E.Level == 0 || !SpellManager.E.IsReady())
            {
                return 0f;
            }

            return (float)ObjectManager.Player.GetAutoAttackDamage(target);
        }

        internal static float GetRDamage(Obj_AI_Base target)
        {
            if (SpellManager.R.Level == 0 || !SpellManager.R.IsReady())
            {
                return 0f;
            }

            return (float)Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (new [] { 80, 120, 160 }[SpellManager.R.Level - 1] +
                 0.6f * ObjectManager.Player.FlatPhysicalDamageMod) *
                (1 + (target.MaxHealth - target.Health) /
                 target.MaxHealth > 0.75f
                    ? 0.75f
                    : (target.MaxHealth - target.Health) / target.MaxHealth) * 8 / 3);
        }
        #endregion
    }
}
