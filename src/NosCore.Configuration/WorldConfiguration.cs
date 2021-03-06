﻿//  __  _  __    __   ___ __  ___ ___  
// |  \| |/__\ /' _/ / _//__\| _ \ __| 
// | | ' | \/ |`._`.| \_| \/ | v / _|  
// |_|\__|\__/ |___/ \__/\__/|_|_\___| 
// 
// Copyright (C) 2018 - NosCore
// 
// NosCore is a free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;

namespace NosCore.Configuration
{
    public class WorldConfiguration : WebApiConfiguration
    {
        public SqlConnectionConfiguration Database { get; set; }

        public short ConnectedAccountLimit { get; set; }

        public byte ServerGroup { get; set; }

        public bool WorldInformation { get; set; }
        public bool SceneOnCreate { get; set; }
        public string ServerName { get; set; }
        public Dictionary<FeatureFlag, bool> FeatureFlags { get; set; } = new Dictionary<FeatureFlag, bool>();
        public short MaxItemAmount { get; set; }
        public byte BackpackSize { get; set; }
        public long MaxGoldAmount { get; set; }
    }
}