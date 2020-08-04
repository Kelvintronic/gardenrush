using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gardenrush.lib.Data;

namespace gardenrush.lib
{
    public interface IGardenRushLaunchService
    {
        Task<GameActionResult> SubmitAction(int nActionType, int nActionArguement, int gameId, string strIdentity);
        Task<IEnumerable<Game>> GetGamesList();
        Task<IEnumerable<Player>> GetPlayers(int gameId);

    }
}
