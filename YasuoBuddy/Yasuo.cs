using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using YasuoBuddy.EvadePlus;

namespace YasuoBuddy
{
    internal class Yasuo
    {
        public static Menu Menu, ComboMenu, HarassMenu, FarmMenu, FleeMenu, DrawMenu, MiscSettings;
        private static int _cleanUpTime;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Yasuo) return;

            Menu = MainMenu.AddMenu("YasuoBuddy", "yasuobuddyfluxy");

            ComboMenu = Menu.AddSubMenu("連技", "yasuCombo");
            ComboMenu.AddGroupLabel("連技設定");
            ComboMenu.Add("combo.Q", new CheckBox("使用Q"));
            ComboMenu.Add("combo.E", new CheckBox("使用W"));
            ComboMenu.Add("combo.stack", new CheckBox("疊Q3"));
            ComboMenu.Add("combo.leftclickRape", new CheckBox("Left Click Rape"));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("R 設定");
            ComboMenu.Add("combo.R", new CheckBox("使用R"));
            ComboMenu.Add("combo.RTarget", new CheckBox("當目標人物被吹起時用R"));
            ComboMenu.Add("combo.RKillable", new CheckBox("用R當對方能被殺死時"));
            ComboMenu.Add("combo.MinTargetsR", new Slider("最少的人數去使用R", 2, 1, 5));

            HarassMenu = Menu.AddSubMenu("騷擾", "yasuHarass");
            HarassMenu.AddGroupLabel("騷擾設定");
            HarassMenu.Add("harass.Q", new CheckBox("使用Q"));
            HarassMenu.Add("harass.E", new CheckBox("使用E"));
            HarassMenu.Add("harass.stack", new CheckBox("疊Q3"));

            FarmMenu = Menu.AddSubMenu("農兵設定", "yasuoFarm");
            FarmMenu.AddGroupLabel("農兵設定");
            FarmMenu.AddLabel("尾兵");
            FarmMenu.Add("LH.Q", new CheckBox("使用Q"));
            FarmMenu.Add("LH.E", new CheckBox("使用E"));
            FarmMenu.Add("LH.EUnderTower", new CheckBox("在塔下使用E", false));

            FarmMenu.AddLabel("清線");
            FarmMenu.Add("WC.Q", new CheckBox("使用Q"));
            FarmMenu.Add("WC.E", new CheckBox("使用E"));
            FarmMenu.Add("WC.EUnderTower", new CheckBox("在塔下使用E", false));

            FarmMenu.AddLabel("打野");
            FarmMenu.Add("JNG.Q", new CheckBox("使用Q"));
            FarmMenu.Add("JNG.E", new CheckBox("使用E"));

            FleeMenu = Menu.AddSubMenu("躲避技能", "yasuoFlee");
            FleeMenu.AddGroupLabel("躲避技能設定");
            FleeMenu.Add("Flee.E", new CheckBox("使用E"));
            FleeMenu.Add("Flee.stack", new CheckBox("疊Q3"));
            FleeMenu.AddGroupLabel("躲避設定");
            FleeMenu.Add("Evade.E", new CheckBox("使用E去閃避技能"));
            FleeMenu.Add("Evade.W", new CheckBox("使用W去閃避技能"));
            FleeMenu.Add("Evade.WDelay", new Slider("人性化延遲 (ms)", 0, 0, 1000));

            MiscSettings = Menu.AddSubMenu("雜項設定");
            MiscSettings.AddGroupLabel("搶頭設定");
            MiscSettings.Add("KS.Q", new CheckBox("使用Q"));
            MiscSettings.Add("KS.E", new CheckBox("使用E"));
            MiscSettings.AddGroupLabel("自動Q設定");
            MiscSettings.Add("Auto.Q3", new CheckBox("使用Q3"));
            MiscSettings.Add("Auto.Active", new KeyBind("自動Q敵人", false, KeyBind.BindTypes.PressToggle, 'M'));

            Program.Main(null);

            DrawMenu = Menu.AddSubMenu("繪製範圍", "yasuoDraw");
            DrawMenu.AddGroupLabel("繪製範圍設定");

            DrawMenu.Add("Draw.Q", new CheckBox("繪製出Q的範圍", false));
            DrawMenu.AddColourItem("Draw.Q.Colour");
            DrawMenu.AddSeparator();

            DrawMenu.Add("Draw.E", new CheckBox("繪製E出的範圍", false));
            DrawMenu.AddColourItem("Draw.E.Colour");
            DrawMenu.AddSeparator();

            DrawMenu.Add("Draw.R", new CheckBox("繪製出R的範圍", false));
            DrawMenu.AddColourItem("Draw.R.Colour");
            DrawMenu.AddSeparator();

            DrawMenu.AddLabel("When Spell is Down Colour = ");
            DrawMenu.AddColourItem("Draw.Down", 7);
            
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            EEvader.Init();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["Draw.Q"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(
                    SpellManager.Q.IsReady() ? DrawMenu.GetColour("Draw.Q.Colour") : DrawMenu.GetColour("Draw.Down"),
                    SpellManager.Q.Range, Player.Instance.Position);
            }
            if (DrawMenu["Draw.R"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(
                    SpellManager.R.IsReady() ? DrawMenu.GetColour("Draw.R.Colour") : DrawMenu.GetColour("Draw.Down"),
                    SpellManager.R.Range, Player.Instance.Position);
            }
            if (DrawMenu["Draw.E"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(
                    SpellManager.E.IsReady() ? DrawMenu.GetColour("Draw.E.Colour") : DrawMenu.GetColour("Draw.Down"),
                    SpellManager.E.Range, Player.Instance.Position);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (_cleanUpTime < Environment.TickCount)
            {
                GC.Collect();
                _cleanUpTime = Environment.TickCount + 1000000;
            }
            StateManager.KillSteal();
            if (MiscSettings["Auto.Active"].Cast<KeyBind>().CurrentValue)
            {
                StateManager.AutoQ();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                StateManager.Flee();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateManager.Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateManager.Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                StateManager.LastHit();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateManager.WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                StateManager.Jungle();
            }
        }
    }
}