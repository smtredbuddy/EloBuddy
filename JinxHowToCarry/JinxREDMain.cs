
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
    class JinxREDMain
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Initialize;
        }
        public static int Wlastcastattempt = 0;
        public static int Rlastcastattempt = 0;
        public static Spell.Skillshot W, R, E;
        public static Spell.Active Q;
        public static Menu Jinx, Combat, Farm, Prediction, Drawing, Misc, Skin;
        private const string ChampionName = "Jinx";

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }
        private static void Initialize(EventArgs args)
        {
            if (Player.BaseSkinName != ChampionName) return;
            Bootstrap.Init(null);

            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Skillshot(SpellSlot.W, 1450, SkillShotType.Linear, 600, 3300, 60);
            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Circular, 1200, 1750, 150);
            R = new Spell.Skillshot(SpellSlot.R, 3250, SkillShotType.Linear, 500, 1700, 140);

            W.AllowedCollisionCount = 0;           


            //Minion Aoe Rocket Splash - RocketUlt Aoe Slpash kill enemu
            //MiniGun inside normal AA range
            //Outside Rocket
            //if outside Rocket range use W
            //R hitchance
            //Laneclear Q if can hit Slider amount
            //Lasthit met rocket uit range
            //AA Killsteal with attackspeed check
            //E logic
            //Auto W CC, Auto E CC - Auto E ProcessSpellCast

            //9-21-2015 FIX W/R CAST + ADD E CAST + PROBEER OM BOUDING RADIUS TARGET TO ADDED (ZAL WEL LUKKEN <3)

            //Main Page
            Jinx = MainMenu.AddMenu("JinxHTC", "JinxHTC");
            Jinx.AddGroupLabel("Jinx: How To Carry - EloBuddy");
            Jinx.AddLabel("Compatible patch version = Patch 5.18+");
            Jinx.AddLabel("By SmTRED for EloBuddy - HU3 HU3 BRBR");

            //Combat Menu 
            Combat = Jinx.AddSubMenu("Combat Settings", "Combat Settings");
            Combat.AddGroupLabel("Combo Settings");
            Combat.Add("combo.Q", new CheckBox("Use Q Switch Login in Combo", true));
            Combat.Add("combo.W", new CheckBox("Use W in Combo", true));
            Combat.Add("combo.E", new CheckBox("Use E in Combo", true));
            Combat.Add("combo.R", new CheckBox("Use R in Combo", true));

            //W
            Combat.AddGroupLabel("Advanced W ");
            Combat.Add("adv.w.dis", new Slider("Min Distance to Cast W  ", 200, 1, 1000));

            //E
            Combat.AddGroupLabel("Advanced E ");
            Combat.Add("adv.e.cc", new CheckBox("Use E on CC'd targets", true)); //DONE
            Combat.Add("adv.e.spell", new CheckBox("Use E on Immobile Spellcasts", true)); //DONE
            Combat.Add("adv.e.gap", new CheckBox("Use E on Gapclosers", true)); //DONE

            //R
            Combat.AddGroupLabel("Advanced R ");
            Combat.Add("adv.R.ks", new CheckBox("Use [R] for Killstealing", true)); //DONE

            Combat.AddGroupLabel("Harass/Mixed Mode");
            Combat.Add("harass.Q", new CheckBox("Use Q Switch Logic in Harass", true));
            Combat.Add("harass.W", new CheckBox("Use W in Harass", true));

            //

            Farm = Jinx.AddSubMenu("Farm Settings", "Farm Settings");
            Farm.AddGroupLabel("Lane/Jungle Settings");
            Farm.Add("lane.Q", new CheckBox("Use Q switch logic", true));
            Farm.Add("lane.Q.mana", new Slider("Fishbone Min. Mana Percentage", 75, 0, 100));

            Farm.Add("lane.Q.aoe", new CheckBox("Use Fishbones for AoE clear", true));
            Farm.Add("lane.Q.lasthit", new CheckBox("Use Q to lasthit out of range minions", true));
            Farm.Add("lane.Q.count", new Slider("Fishbones AoE minions hit", 3, 0, 10));

            //Prediction Menu
            Prediction = Jinx.AddSubMenu("Prediction Settings", "Prediction Settings");

            //prediction W
            Prediction.AddGroupLabel("W Prediction Hitchance Slider");
            var hitchancesliderW = Prediction.Add("HitchanceW", new Slider("WHitchance", 2, 0, 2));
            var modeArrayW = new[] { "Low", "Medium", "High" };
            hitchancesliderW.DisplayName = modeArrayW[hitchancesliderW.CurrentValue];
            hitchancesliderW.OnValueChange +=
            delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = modeArrayW[changeArgs.NewValue];
            };

            //prediction R
            Prediction.AddGroupLabel("R Prediction Hitchance Slider");
            var hitchancesliderR = Prediction.Add("HitchanceR", new Slider("RHitchance", 2, 0, 2));
            var modeArrayR = new[] { "Low", "Medium", "High" };
            hitchancesliderR.DisplayName = modeArrayR[hitchancesliderR.CurrentValue];
            hitchancesliderR.OnValueChange +=
            delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = modeArrayR[changeArgs.NewValue];
            };

            //Draw Menu
            Drawing = Jinx.AddSubMenu("Draw Settings", "Draw Settings");
            Drawing.AddGroupLabel("Spell Drawings");
            Drawing.Add("draw.Q", new CheckBox("Draw MiniGun/Rocket Range", true));
            Drawing.Add("draw.W", new CheckBox("Draw W Range", true));
            Drawing.Add("draw.E", new CheckBox("Draw E Range", true));
            Drawing.Add("draw.R", new CheckBox("Draw R Range", true));
            Drawing.AddGroupLabel("Misc Drawings");

            Drawing.Add("draw.w.pred1", new CheckBox("Draw W Cast Position", true));
            Drawing.Add("draw.r.pred1", new CheckBox("Draw R Cast Position", true));

            Drawing.Add("print.debug.chat1", new CheckBox("Print Debug Messages", false));


            //skin
            Skin = Jinx.AddSubMenu("Skin Manager", "Skin Manager");
            Skin.AddGroupLabel("Skin Manager");
            var skin = Skin.Add("SkinID1", new Slider("Skin", 0, 0, 2));
            var SkinID = new[] { "Classic", "Mafia", "Firecracker" };
            skin.DisplayName = SkinID[skin.CurrentValue];
            skin.OnValueChange +=
            delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = SkinID[changeArgs.NewValue];
            };


            Game.OnTick += OrbwalkerModes;
           // Game.OnTick += ActiveEvents;
            Plugins.Drawings.DrawEvent();
            Obj_AI_Base.OnProcessSpellCast += Tickcount;
            Obj_AI_Base.OnProcessSpellCast += EventE;
            Orbwalker.OnPreAttack += PreAttack;
            Gapcloser.OnGapcloser += GapCloser;

            Printmsg("Jinx: How To Carry Loaded - Patch Version = " + Game.Version);
            Printmsg("Version 1.0 Beta - RED SERIES! - Aqui e BR !");
        }

        private static void EventE(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //E on Spellcast
            if (sender == null || !sender.IsValid || !sender.IsEnemy) return;
            var e_SPELL = Combat["adv.e.spell"].Cast<CheckBox>().CurrentValue;
            if (E.IsReady() && e_SPELL && Extensions.EonCastList(args.SData.Name))
                E.Cast(sender);
        }

        private static void GapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var e_GAP = Combat["adv.e.gap"].Cast<CheckBox>().CurrentValue;
            //cast [E] at end position of le gapcloser
            if (E.IsReady() && e_GAP && e.End.Distance(Player.Position) <= 200)
                E.Cast(e.End);
        }

        private static void ActiveEvents(EventArgs args)
        {
            var combo_R = Combat["combo.R"].Cast<CheckBox>().CurrentValue;
            var R_KS = Combat["adv.R.ks"].Cast<CheckBox>().CurrentValue;
            //ActiveEvent Killsteal [R]
            var enemies = HeroManager.AllHeroes.Where(a => a.IsEnemy && a.IsValid);
            foreach (var enemy in enemies)
            {
                if (enemy == null || !enemy.IsValid) return;
                var aa = Player.GetAutoAttackDamage(enemy);
                var inMrange = Player.ServerPosition.Distance(enemy.ServerPosition) < 510;
                var outMrange = Player.ServerPosition.Distance(enemy.ServerPosition) > 525;

                if (enemy.Health < aa * 3 && inMrange || enemy.Health < aa * 2 && outMrange && Player.Distance(enemy.Position) < Extensions.MaxRange + enemy.BoundingRadius)
                    return;

                if (R.IsReady() && enemy.IsValidTarget(R.Range) && R_KS && Environment.TickCount - Wlastcastattempt > 800)
                {
                    if (R.IsReady() && enemy.Health < Extensions.RDamage(enemy))
                        R.Cast(enemy);
                }
            }

            //ActiveEvent [E] on CC
            foreach (var enemy in enemies)
            {
                if (enemy == null) return;

                var e_CC = Combat["adv.e.cc"].Cast<CheckBox>().CurrentValue;
                if (E.IsReady() && enemy.IsValidTarget(E.Range))
                {
                    //CC cast
                    if (Extensions.UnitIsImmobileUntil(enemy) >= E.CastDelay - 0.5 && e_CC)
                        E.Cast(enemy);
                }
            }

        }

        private static void PreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var combo = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            if (combo) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var lane_Q = Farm["lane.Q"].Cast<CheckBox>().CurrentValue;
                var lane_Q_aoe = Farm["lane.Q.aoe"].Cast<CheckBox>().CurrentValue;
                var lane_Q_lasthit = Farm["lane.Q.lasthit"].Cast<CheckBox>().CurrentValue;

                foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsEnemy && t.IsValidTarget(Extensions.MaxRange + t.BoundingRadius)))
                {
                    var inMrange = Player.ServerPosition.Distance(target) < 525;
                    var outMrange = Player.Position.Distance(target) > 525;
                    var inMax = Player.Position.Distance(target) <= Extensions.MaxRange + target.BoundingRadius;

                    target = turret;
                    {
                        if (Extensions.FishBone && inMrange && lane_Q)
                        {
                            Q.Cast();
                        }
                    }
                }
                foreach (var minion in ObjectManager.Get<Obj_AI_Base>().Where(m => m.IsEnemy && m.IsValidTarget(Extensions.MaxRange + m.BoundingRadius) && m.IsMinion))
                {
                    if (!Q.IsReady() || !lane_Q) return;

                    target = minion;

                    var lane_Q_count = Farm["lane.Q.count"].Cast<Slider>().CurrentValue;
                    var lane_Mana = Farm["lane.Q.mana"].Cast<Slider>().CurrentValue;
                    var inMrange = Player.ServerPosition.Distance(target) < 525;
                    var outMrange = Player.Position.Distance(target) > 525;
                    var inMax = Player.Position.Distance(target) < Extensions.MaxRange + target.BoundingRadius;
                    var mana = Player.ManaPercent > lane_Mana;

                    //AoE check
                    if (!Extensions.FishBone && inMax 
                        && Extensions.MinionCheck(target, 200) >= lane_Q_count && lane_Q_aoe && mana)
                    {
                        Q.Cast();
                        return;
                    }
                    if (Extensions.FishBone && inMax
                        && Extensions.MinionCheck(target, 200) >= lane_Q_count && lane_Q_aoe && mana)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                        return;
                    }
                    if (!Extensions.FishBone && minion.Health < Player.GetAutoAttackDamage(minion) * 1.1
                        && mana && lane_Q_lasthit && outMrange && Extensions.KillableMinionCheck(Player, Extensions.MaxRange + minion.BoundingRadius) < 1 && outMrange)
                    {
                        Q.Cast();
                        return;
                    }
                    if (Extensions.FishBone && minion.Health < Player.GetAutoAttackDamage(minion) * 1.1 
                        && mana && lane_Q_lasthit && outMrange && Extensions.KillableMinionCheck(Player, Extensions.MaxRange + minion.BoundingRadius) < 1 && outMrange)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                        return;
                    }

                    if (Extensions.FishBone && inMrange && lane_Q)
                    {
                        Q.Cast();
                        return;
                    }
                    if (Extensions.FishBone && mana && lane_Q)
                    {
                        Q.Cast();
                        return;
                    }
                    if (Extensions.FishBone && Player.Position.CountEnemiesInRange(2000) < 1)
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void Tickcount(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try {
                if (sender == null) return;

                if (args.SData.Name == "JinxWMissile")
                {
                    Wlastcastattempt = Environment.TickCount;
                    Printconsole("[W] Casted!");
                }
                if (args.SData.Name == "JinxR")
                {
                    Rlastcastattempt = Environment.TickCount;
                    Printconsole("[R] Casted!");
                }
            }
            catch (Exception)
            {
                //Error spamm not sure what causes it will have to look at it later.
                Console.WriteLine("Error. Zone: OnProcessSpellcast - Addon: JinxRED");
            }
            }

        private static void OrbwalkerModes(EventArgs args)
        {
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    combo();
                    break;
                case Orbwalker.ActiveModes.Harass:
                    harass();
                    break;
                case Orbwalker.ActiveModes.LaneClear:
                    break;
                case Orbwalker.ActiveModes.JungleClear:
                    break;
            }

            //SkinChanger Update
            SkinChanger();
        }
        static void harass()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (target == null || !target.IsValid) return;

            var harass_W = Combat["harass.W"].Cast<CheckBox>().CurrentValue;
            var harass_Q = Combat["harass.Q"].Cast<CheckBox>().CurrentValue;

            if (W.IsReady() && target.IsValidTarget(W.Range) && W.GetPrediction(target).HitChance >= WPrediction() && harass_W)
            {
                var aa = Player.GetAutoAttackDamage(target);
                var inMrange = Player.ServerPosition.Distance(target.ServerPosition) < 500;
                var outMrange = Player.ServerPosition.Distance(target.ServerPosition) > 510;

                if (inMrange && target.Health < aa * 1) return;

                var SuperHit = target.Distance(W.GetPrediction(target).CastPosition) < 350 && target.HasRecentlyChangedPath();

                if (W.IsReady() && SuperHit)
                    W.Cast(target);

                if (W.IsReady() && outMrange && W.GetPrediction(target).HitChance >= WPrediction())
                    W.Cast(target);
            }

            if (Q.IsReady() && target.IsValidTarget(Extensions.MaxRange + target.BoundingRadius) && harass_Q)
            {
                var inMrange = Player.ServerPosition.Distance(target.Position) < 525;
                var outMrange = Player.Position.Distance(target.Position) > 525;


                if (Q.IsReady() && target.Position.CountEnemiesInRange(200) >= 2 && !Extensions.FishBone)
                {
                    Q.Cast();
                    Printconsole("Switched to: Rocket - Reason: AOE");
                    return;
                }
                if (Q.IsReady() && outMrange && !Extensions.FishBone)
                {
                    Q.Cast();
                    Printconsole("Switched to: Rocket - Reason: Out of Range");
                    return;
                }
                if (Q.IsReady() && inMrange && Extensions.FishBone)
                {
                    Q.Cast();
                    Printconsole("Switched to: Rocket - Reason: in Mini Range");
                    return;
                }
            }
        }
        static void combo()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (target == null || !target.IsValid) return;
            var combo_Q = Combat["combo.Q"].Cast<CheckBox>().CurrentValue;
            var combo_W = Combat["combo.W"].Cast<CheckBox>().CurrentValue;
            var combo_E = Combat["Combo.E"].Cast<CheckBox>().CurrentValue;
            var combo_R = Combat["combo.R"].Cast<CheckBox>().CurrentValue;

            //WCAST
            if (W.IsReady() && target.IsValidTarget(W.Range) && W.GetPrediction(target).HitChance >= WPrediction() && combo_W)
            {
                var aa = Player.GetAutoAttackDamage(target);
                var inMrange = Player.ServerPosition.Distance(target.ServerPosition) < 500;
                var outMrange = Player.ServerPosition.Distance(target.ServerPosition) > 510;

                if (inMrange && target.Health < aa * 1) return;

                var SuperHit = target.Distance(W.GetPrediction(target).CastPosition) < 350 && target.HasRecentlyChangedPath();

                if (W.IsReady() && SuperHit)
                    W.Cast(target);

                if (W.IsReady() && outMrange && W.GetPrediction(target).HitChance >= WPrediction() && Environment.TickCount - Rlastcastattempt > 800)
                    W.Cast(target);
            }

            //RCAST
            if (R.IsReady()  && combo_R)
            {
                var aa = Player.GetAutoAttackDamage(target);

                var inMrange = Player.ServerPosition.Distance(target.ServerPosition) < 510;
                var outMrange = Player.ServerPosition.Distance(target.ServerPosition) > 525;
                var Rpred1 = R.GetPrediction(target).CastPosition.Distance(Player.Position) > 350;

                if (target.HasBuffOfType(BuffType.Snare) && R.IsReady() && Extensions.UnitIsImmobileUntil(target) >= R.CastDelay && target.HealthPercent < 35)
                    R.Cast(target.Position);

                if (target.Health < aa * 3 && inMrange || target.Health < aa * 2 && outMrange && Player.Distance(target.Position) < Extensions.MaxRange + target.BoundingRadius)
                    return;

                if (Extensions.UnitIsImmobileUntil(target) >= R.CastDelay && target.HealthPercent < 50 && R.GetPrediction(target).HitChance >= HitChance.Low
                    && Environment.TickCount - Wlastcastattempt > 800)
                   {
                    R.Cast(target);
                   }

                if (R.IsReady() && target.Health < Extensions.RDamage(target)
                    || R.IsReady() && Extensions.UnitIsSlowed(target) > R.CastDelay
                    && target.Health < Extensions.RDamage(target) && target.Distance(Player.Position) < 1750
                    && Environment.TickCount - Wlastcastattempt > 800)
                {
                    R.Cast(target);
                }
            }

            //ECAST
            if (E.IsReady() && target.IsValidTarget(E.Range) && combo_E && Environment.TickCount - Wlastcastattempt > 1200)
            {
                var aa = Player.GetAutoAttackDamage(target);

                if (Extensions.GetETarget() != null)
                    E.Cast(Extensions.GetETarget());

                if (E.GetPrediction(target).HitChance >= HitChance.High && target.IsMoving && !target.HasRecentlyChangedPath() && target.HasBuffOfType(BuffType.Slow))
                    E.Cast(target);

                if (E.GetPrediction(target).HitChance >= HitChance.High && target.IsMoving && !target.HasRecentlyChangedPath() 
                    && target.IsMelee && target.Distance(Player.Position) < 325 && target.IsFacing(Player))
                    E.Cast(target);
            }

            //QCAST
            if (Q.IsReady() && target.IsValidTarget(Extensions.MaxRange + target.BoundingRadius) && combo_Q)
            {
                var inMrange = Player.ServerPosition.Distance(target.Position) < 525;
                var outMrange = Player.Position.Distance(target.Position) > 515;


                if (Q.IsReady() && target.Position.CountEnemiesInRange(200) >= 2 && !Extensions.FishBone)
                {
                    Q.Cast();
                    Printconsole("Switched to: Rocket - Reason: AOE");
                    return;
                }
                if (Q.IsReady() && outMrange && !Extensions.FishBone)
                {
                    Q.Cast();
                    Printconsole("Switched to: Rocket - Reason: Out of Range");
                    return;
                }
                if (Q.IsReady() && inMrange && Extensions.FishBone)
                {
                    Q.Cast();
                    Printconsole("Switched to: Rocket - Reason: in Mini Range");
                    return;
                }
            }
        }

        private static void SkinChanger()
        {
            var mode = Skin["SkinID1"].DisplayName;


            switch (mode)
            {
                case "Classic":
                    Player.SetSkinId(0);
                    break;
                case "Mafia":
                    Player.SetSkinId(1);
                    break;
                case "Firecracker":
                    Player.SetSkinId(2);
                    break;
            }

        }
        public static HitChance WPrediction()
        {
            var mode = Prediction["HitchanceW"].DisplayName;
            switch (mode)
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
            }
            return HitChance.High;
        }
        public static HitChance RPrediction()
        {
            var mode = Prediction["HitchanceR"].DisplayName;
            switch (mode)
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
            }
            return HitChance.High;
        }

        public static void Printconsole(string message)
        {
            var debugprint = Drawing["print.debug.chat1"].Cast<CheckBox>().CurrentValue;

            if (!debugprint)
                return;
            EloBuddy.Chat.Print(
                "<font color='#FFB90F'>[Console]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        public static void Printmsg(string message)
        {
            EloBuddy.Chat.Print(
                "<font color='#00ff00'>[JinxRED]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
    }
}
