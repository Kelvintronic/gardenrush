using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gardenrush.lib.Data;


namespace gardenrush.lib.services
{
    public interface IGardenRepository
    {
        Task<Game> GetGame(int GameId);
        Task<IEnumerable<Player>> GetPlayers(int GameId);
        Task<IEnumerable<Piece>> GetPieces(int GameId);
        Task<IEnumerable<Piece>> GetTruckPieces(int GameId);
        Task<IEnumerable<Piece>> GetHistoryPieces(int HistoryId);

        void DeleteGame(int GameId);
        Task<IEnumerable<Game>> GetGameList();
        Task<GameActionResult> SubmitAction(GameAction action);
        Task<History> GetLastMove(int GameId);

    }
}
