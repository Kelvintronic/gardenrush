using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gardenrush.Services
{
    public class TurnService
    {
        private List<GameItem> statusList;

        public TurnService()
        {
            statusList = new List<GameItem>();
        }
        public int GetGameStatus(int id)
        {
            GameItem game = statusList.Find(g => g.gameId == id);
            if (game == null)
                return 0;
            return game.gameStatus;
        }
        public bool SetGameStatus(int id, int status)
        {
            GameItem game = statusList.Find(g => g.gameId == id);
            if (game == null)
            {
                game = new GameItem() { gameId = id, gameStatus = status };
                statusList.Add(game);
            }
            else
            {
                game.gameStatus = status;
            }

            return true;
        }
    }

    public class GameItem
    {
        public int gameId;
        public int gameStatus;
    }
}
