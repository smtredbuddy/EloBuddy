using System;
using System.Linq;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace JinxRED.Plugins
{
    class Drawings : JinxREDMain
    {
        public static void DrawEvent()
        {
            EloBuddy.Drawing.OnDraw += DrawRanges;
            EloBuddy.Drawing.OnDraw += TargetDraws;
        }


        private static void TargetDraws(EventArgs args)
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (target != null)
            {
                if (target == null) return;

                var wprediction = W.GetPrediction(target).CastPosition;
                var rprediction = R.GetPrediction(target).CastPosition;
                var wpred = Drawing["draw.w.pred1"].Cast<CheckBox>().CurrentValue;
                var rpred = Drawing["draw.r.pred1"].Cast<CheckBox>().CurrentValue;

                if (rpred)
                {
                    var targetpos = EloBuddy.Drawing.WorldToScreen(target.Position);

                    if (targetpos == null) return;

                    EloBuddy.Drawing.DrawText(targetpos[0] - 10, targetpos[1], System.Drawing.Color.Gold, Extensions.RDamage(target).ToString());

                    if (rprediction == null) return;

                    EloBuddy.Drawing.DrawCircle(rprediction, R.Width, System.Drawing.Color.Red);
                }
                if (wpred)
                {
                    if (wprediction == null) return;
                    EloBuddy.Drawing.DrawCircle(wprediction, W.Width, System.Drawing.Color.Gold);

                }
            }


            foreach (var buff in Player.Buffs)
            {
                if (buff.Name == "jinxpassivekill")
                {
                    var targetpos = EloBuddy.Drawing.WorldToScreen(Player.Position);
                    EloBuddy.Drawing.DrawText(targetpos[0] - 10, targetpos[1], System.Drawing.Color.Gold, "" + (buff.EndTime - Game.Time));
                    break;

                }
            }

            var targetz = Orbwalker.GetTarget();
            if (targetz != null)
            {
                    EloBuddy.SDK.Rendering.Circle.Draw(Color.Tomato, targetz.BoundingRadius, targetz.Position);
            }

        }

        private static void DrawRanges(EventArgs args)
        {
            var drawq = Drawing["draw.Q"].Cast<CheckBox>().CurrentValue;
            var draww = Drawing["draw.W"].Cast<CheckBox>().CurrentValue;
            var drawe = Drawing["draw.E"].Cast<CheckBox>().CurrentValue;
            var drawr = Drawing["draw.R"].Cast<CheckBox>().CurrentValue;

            if (Extensions.FishBone)
            {
                var linem = Color.Red;
                var liner = Color.Green;

                if (drawq)
                    if (Q.Level > 0)
                    {
                        EloBuddy.SDK.Rendering.Circle.Draw(linem, Extensions.MiniGunRange + Player.BoundingRadius, Player.Position);
                        EloBuddy.SDK.Rendering.Circle.Draw(liner, Extensions.MaxRange + Player.BoundingRadius, Player.Position);
                    }
            }
            if (!Extensions.FishBone)
            {
                var linem = Color.Green;
                var liner = Color.Red;

                if (drawq)
                    if (Q.Level > 0)
                    {
                        EloBuddy.SDK.Rendering.Circle.Draw(linem, Extensions.MiniGunRange + Player.BoundingRadius, Player.Position);
                        EloBuddy.SDK.Rendering.Circle.Draw(liner, Extensions.MaxRange + Player.BoundingRadius, Player.Position);
                    }
            }

            if (draww)
                if (W.Level > 0)
                    EloBuddy.SDK.Rendering.Circle.Draw(W.IsReady() ? Color.BlueViolet : Color.Red, W.Range, Player.Position);

            if (drawe)
                if (E.Level > 0)
                    EloBuddy.SDK.Rendering.Circle.Draw(E.IsReady() ? Color.CadetBlue : Color.Red, E.Range, Player.Position);

            if (drawr)
                if (R.Level > 0)
                    EloBuddy.SDK.Rendering.Circle.Draw(R.IsReady() ? Color.Purple : Color.Red, R.Range, Player.Position);
        }
    }
}
