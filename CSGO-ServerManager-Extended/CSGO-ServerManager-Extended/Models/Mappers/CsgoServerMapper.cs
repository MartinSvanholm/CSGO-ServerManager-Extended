using CSGO_ServerManager_Extended.Models.DTOs;
using CsgoServerInterface.CsgoServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_ServerManager_Extended.Models.Mappers
{
    public static class CsgoServerMapper
    {
        public static CsgoServer MapToCsgoServer(CsgoServerDTO csgoServerDTO)
        {
            return new CsgoServer(
                new CsgoSettings(csgoServerDTO.Password, csgoServerDTO.RconPassord),
                csgoServerDTO.Ip,
                csgoServerDTO.Name,
                new Ports(csgoServerDTO.GamePort),
                csgoServerDTO.Ip);
        }

        public static CsgoServerDTO MapToCsgoServerDTO(CsgoServer csgoServer)
        {
            return new CsgoServerDTO()
            {
                Name = csgoServer.Name,
                Password = csgoServer.CsgoSettings.Password,
                RconPassord = csgoServer.CsgoSettings.Rcon,
                Ip = csgoServer.Ip,
                GamePort = csgoServer.Ports.Game
            };
        }
    }
}
