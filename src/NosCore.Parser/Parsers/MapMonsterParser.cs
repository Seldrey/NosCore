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

using System;
using System.Collections.Generic;
using System.Linq;
using NosCore.Data.AliveEntities;
using NosCore.DAL;
using NosCore.Shared.I18N;

namespace NosCore.Parser.Parsers
{
    internal class MapMonsterParser
    {
        public void InsertMapMonster(List<string[]> packetList)
        {
            var monsterCounter = 0;
            short map = 0;
            var mobMvPacketsList = new List<int>();
            var monsters = new List<MapMonsterDto>();

            foreach (var currentPacket in packetList.Where(o => o[0].Equals("mv") && o[1].Equals("3")))
            {
                if (!mobMvPacketsList.Contains(Convert.ToInt32(currentPacket[2])))
                {
                    mobMvPacketsList.Add(Convert.ToInt32(currentPacket[2]));
                }
            }

            foreach (var currentPacket in packetList.Where(o => o[0].Equals("in") || o[0].Equals("at")))
            {
                if (currentPacket.Length > 5 && currentPacket[0] == "at")
                {
                    map = short.Parse(currentPacket[2]);
                    continue;
                }

                if (currentPacket.Length <= 7 || currentPacket[0] != "in" || currentPacket[1] != "3")
                {
                    continue;
                }

                var monster = new MapMonsterDto
                {
                    MapId = map,
                    VNum = short.Parse(currentPacket[2]),
                    MapMonsterId = int.Parse(currentPacket[3]),
                    MapX = short.Parse(currentPacket[4]),
                    MapY = short.Parse(currentPacket[5]),
                    Direction = (byte) (currentPacket[6] == string.Empty ? 0 : byte.Parse(currentPacket[6])),
                    IsDisabled = false
                };
                monster.IsMoving = mobMvPacketsList.Contains(monster.MapMonsterId);

                if (DaoFactory.NpcMonsterDao.FirstOrDefault(s => s.NpcMonsterVNum.Equals(monster.VNum)) == null
                    || DaoFactory.MapMonsterDao.FirstOrDefault(s => s.MapMonsterId.Equals(monster.MapMonsterId)) != null
                    || monsters.Count(i => i.MapMonsterId == monster.MapMonsterId) != 0)
                {
                    continue;
                }

                monsters.Add(monster);
                monsterCounter++;
            }


            IEnumerable<MapMonsterDto> mapMonsterDtos = monsters;
            DaoFactory.MapMonsterDao.InsertOrUpdate(mapMonsterDtos);
            Logger.Log.Info(string.Format(LogLanguage.Instance.GetMessageFromKey(LanguageKey.MONSTERS_PARSED),
                monsterCounter));
        }
    }
}