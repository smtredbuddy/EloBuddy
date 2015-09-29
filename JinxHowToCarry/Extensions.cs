using System;
using System.Linq;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace JinxRED
{
    class Extensions : JinxREDMain
    {
        #region --- Buffs/Actives
        public static float EnemyCheck(AIHeroClient target, int range)
        {
            var enemies =
                HeroManager.Enemies.Where(e => e.IsEnemy && e.IsValid && !e.IsMinion && !e.IsDead).ToList();

            return enemies.Count;
        }
        public static float MinionCheck(AttackableUnit target, int range)
        {
            var minions =
                ObjectManager.Get<Obj_AI_Base>().Where(a => a.IsEnemy
                && a.Distance(target.Position) < range && a.IsMinion && !a.IsDead).ToList();

            return minions.Count;
        }
        public static float KillableMinionCheck(AIHeroClient target, float range)
        {

            var kminions = ObjectManager.Get<Obj_AI_Base>().Where(a => a.IsEnemy
                && a.Distance(target.Position) < range && a.IsMinion && a.Health < Player.GetAutoAttackDamage(a)).ToList();

            return kminions.Count;

        }
        public static bool FishBone
        {
            get { return ObjectManager.Player.AttackRange > 530f; }
        }
        public static float QAddRange
        {
            get { return 50 + 25 * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level; }
        }
        public static float MaxRange
        {
           get { return MiniGunRange + QAddRange; }
        }
        public static float MiniGunRange
        {
            get { return 525; }
        }
        public static Obj_AI_Base GetETarget()
        {
            return HeroManager.Enemies.Where(x => E.IsReady() 
            && !x.HasBuffOfType(BuffType.SpellImmunity) && x.IsEnemy 
            && !x.IsFacing(Player) 
            && x.IsMoving && x.IsValidTarget(E.Range)).OrderBy(x => x.Distance(Player, false)).FirstOrDefault();
        }
        public static float RDamage(Obj_AI_Base target)
        {
            var level = R.Level;

            if (level == 0)
                return 0; ;

            if (target.Distance(Player) < 1500)
            {
                return Player.CalculateDamageOnUnit(target, DamageType.Physical,
                    (float)
                      (new double[] { 25, 35, 45 }[level - 1]
                         + new double[] { 25, 30, 35 }[level - 1] / 100
                              * (target.MaxHealth - target.Health)
                                    + 0.1 * Player.FlatPhysicalDamageMod) + MissingHpPercentage(target) - (target.AttackShield + target.MagicShield));
            }

            return Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)
                    (new double[] { 250, 350, 450 }[level - 1] +
                     new double[] { 25, 30, 35 }[level - 1] / 100 * (target.MaxHealth - target.Health) +
                     1 * Player.FlatPhysicalDamageMod) + MissingHpPercentage(target) - (target.AttackShield + target.MagicShield));
        }
        public static float MissingHpPercentage(Obj_AI_Base target)
        {
            if (R.Level == 0) return 0;

            if (R.Level == 1)
            {
                return ((target.MaxHealth - target.Health) / 100) * 5;
            }
            if (R.Level == 2)
            {
                return ((target.MaxHealth - target.Health) / 100) * 10;
            }
            if (R.Level == 3)
            {
                return ((target.MaxHealth - target.Health) / 100) * 16;
            }

            return 0;

        }

        public static int SpeedStacks
        {
            get
            {
                return
                    ObjectManager.Player.Buffs.Where(buff => buff.DisplayName.ToLower() == "jinxqramp")
                        .Select(buff => buff.Count)
                        .FirstOrDefault();
            }
        }
        public static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var duration = unit.Buffs.Where(buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare)).Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
                 return (duration - Game.Time);
        }
        public static double UnitIsSlowed(Obj_AI_Base unit)
        {
            var duration = unit.Buffs.Where(buff =>
            buff.IsActive && Game.Time <= buff.EndTime &&
            (buff.Type == BuffType.Snare || buff.Type == BuffType.Slow)).Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (duration - Game.Time);
        }
        public static bool EonCastList(string Spell)
        {
            switch (Spell)
            {
                case "ThreshQ":
                    return true;
                case "KatarinaR":
                    return true;
                case "AlZaharNetherGrasp":
                    return true;
                case "GalioIdolOfDurand":
                    return true;
                case "LuxMaliceCannon":
                    return true;
                case "MissFortuneBulletTime":
                    return true;
                case "RocketGrabMissile":
                    return true;
                case "CaitlynPiltoverPeacemaker":
                    return true;
                case "EzrealTrueshotBarrage":
                    return true;
                case "InfiniteDuress":
                    return true;
                case "VelkozR":
                    return true;
            }
            return false;
        }

        //public static bool harass_Q = Combat["harass.Q"].Cast<CheckBox>().CurrentValue;
        //public static bool harass_W = Combat["harass.aa"].Cast<CheckBox>().CurrentValue;

        //public static float harass_Mana = Combat["harass.mana"].Cast<Slider>().CurrentValue;

        #endregion 
    }
}
