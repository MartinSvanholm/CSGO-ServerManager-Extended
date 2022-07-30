﻿using CSGOServerInterface.Server.DTO;
using CSGOServerInterface.Server.DathostServer;

namespace CSGOServerInterface.Mappers
{
    public static class DatHostCsgoServerMapper
    {
        public static DatHostCsgoServer Map(DatHostServerDTO dto)
        {
            return new DatHostCsgoServer(
                id: -1,
                name: dto.Name,
                ip: dto.RawIp,
                password: dto.CsgoSettings.Password,
                rconPassword: dto.CsgoSettings.Rcon,
                gamePort: dto.Ports.Game,
                gOTVPort: dto.Ports.Gotv,
                isOn: dto.On,
                booting: dto.Booting,
                isFavourite: false,
                datHostID: dto.Id
                );
        }

        public static List<DatHostCsgoServer> MapList(List<DatHostServerDTO> dTOs)
        {
            List<DatHostCsgoServer> result = new();

            foreach (var dto in dTOs)
            {
                result.Add(Map(dto));
            }

            return result;
        }
    }
}