﻿#region License

/*
 Copyright 2014 - 2015 Nikita Bernthaler
 MenuExtensions.cs is part of SFXLibrary.

 SFXLibrary is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 SFXLibrary is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with SFXLibrary. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion License

#region

using System.Collections.Generic;
using LeagueSharp.Common;

#endregion

namespace SFXLibrary.Extensions.LeagueSharp.Common
{
    public static class MenuExtensions
    {
        public static void AddItems(this Menu menu, List<MenuItem> items)
        {
            foreach (var item in items)
            {
                menu.AddItem(item);
            }
        }
    }
}