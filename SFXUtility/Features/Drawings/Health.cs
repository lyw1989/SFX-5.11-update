﻿#region License

/*
 Copyright 2014 - 2015 Nikita Bernthaler
 Health.cs is part of SFXUtility.

 SFXUtility is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 SFXUtility is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with SFXUtility. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion License

#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SFXLibrary;
using SFXLibrary.Extensions.NET;
using SFXLibrary.Extensions.SharpDX;
using SFXLibrary.Logger;
using SFXUtility.Classes;
using SharpDX;
using SharpDX.Direct3D9;

#endregion

namespace SFXUtility.Features.Drawings
{
    internal class Health : Child<Drawings>
    {
        private List<Obj_BarracksDampener> _inhibs;
        private Font _text;
        private List<Obj_AI_Turret> _turrets;
        public Health(SFXUtility sfx) : base(sfx) {}

        public override string Name
        {
            get { return Global.Lang.Get("F_Health"); }
        }

        protected override void OnEnable()
        {
            Drawing.OnEndScene += OnDrawingEndScene;

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Drawing.OnEndScene -= OnDrawingEndScene;

            base.OnDisable();
        }

        private void OnDrawingEndScene(EventArgs args)
        {
            try
            {
                if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                {
                    return;
                }

                var percent = Menu.Item(Name + "DrawingPercent").GetValue<bool>();
                if (Menu.Item(Name + "Turret").GetValue<bool>())
                {
                    foreach (
                        var turret in _turrets.Where(t => t != null && t.IsValid && !t.IsDead && t.HealthPercent <= 75))
                    {
                        _text.DrawTextCentered(
                            ((int) (percent ? (int) turret.HealthPercent : turret.Health)).ToStringLookUp(),
                            Drawing.WorldToMinimap(turret.Position), Color.White);
                    }
                }
                if (Menu.Item(Name + "Inhibitor").GetValue<bool>())
                {
                    foreach (var inhib in
                        _inhibs.Where(
                            i => i != null && i.IsValid && !i.IsDead && i.Health > 1f && i.HealthPercent <= 75))
                    {
                        _text.DrawTextCentered(
                            ((int) (percent ? (int) inhib.HealthPercent : inhib.Health)).ToStringLookUp(),
                            Drawing.WorldToMinimap(inhib.Position), Color.White);
                    }
                }
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        protected override void OnLoad()
        {
            try
            {
                Menu = new Menu(Name, Name);
                var drawingMenu = new Menu(Global.Lang.Get("G_Drawing"), Name + "Drawing");
                drawingMenu.AddItem(
                    new MenuItem(drawingMenu.Name + "Percent", Global.Lang.Get("G_Percent")).SetValue(false));
                drawingMenu.AddItem(
                    new MenuItem(drawingMenu.Name + "FontSize", Global.Lang.Get("G_FontSize")).SetValue(
                        new Slider(13, 3, 30)));

                Menu.AddSubMenu(drawingMenu);

                Menu.AddItem(new MenuItem(Name + "Turret", Global.Lang.Get("G_Turret")).SetValue(false));
                Menu.AddItem(new MenuItem(Name + "Inhibitor", Global.Lang.Get("G_Inhibitor")).SetValue(false));

                Menu.AddItem(new MenuItem(Name + "Enabled", Global.Lang.Get("G_Enabled")).SetValue(false));

                Parent.Menu.AddSubMenu(Menu);
            }
            catch (Exception ex)
            {
                Global.Logger.AddItem(new LogItem(ex));
            }
        }

        protected override void OnInitialize()
        {
            _inhibs = new List<Obj_BarracksDampener>();
            _turrets = new List<Obj_AI_Turret>();

            _turrets.AddRange(
                GameObjects.Turrets.Where(t => t.IsValid && !t.IsDead && t.Health > 1f && t.Health < 9999f));
            _inhibs.AddRange(GameObjects.Inhibitors);

            if (!_turrets.Any() || !_inhibs.Any())
            {
                OnUnload(null, new UnloadEventArgs(true));
                return;
            }

            _text = MDrawing.GetFont(Menu.Item(Name + "DrawingFontSize").GetValue<Slider>().Value);

            base.OnInitialize();
        }
    }
}