using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using gardenrush.lib.Data;


namespace gardenrush.lib.services
{
    public class GardenRepository : IGardenRepository
    {
        private readonly GardenRushDbContext appDbContext;
        public GardenRepository(GardenRushDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async void DeleteGame(int Id)
        {
            var result = await appDbContext.Game.FirstOrDefaultAsync(g => g.GameId == Id);
            if (result != null)
            {
                appDbContext.Game.Remove(result);
                await appDbContext.SaveChangesAsync();
            }
        }

        public async Task<Game> GetGame(int GameId)
        {
            var game = await appDbContext.Game.FindAsync(GameId);
            return game;
        }

        public async Task<IEnumerable<Player>> GetPlayers(int GameId)
        {
            var playerList = await appDbContext.Player.ToListAsync();
            var playerRefinedList = playerList.FindAll(p => p.GameId == GameId);
            return playerRefinedList;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await appDbContext.User.ToListAsync();
        }

        public async Task<User> GetUser(string strIdentity)
        {
            var users = await GetUsers();

            // check if user exists
            User User = null;

            foreach (User user in users)
            {
                if (user.Identity == strIdentity)
                {
                    User = user;
                    break;
                }
            }

            if (User == null)
            {
                // try to add new user
                User = await AddUser(strIdentity);
            }

            return User;    // can be null if all above fail
        }

        public async Task<IEnumerable<Piece>> GetPieces(int GameId)
        {
            var piecesList = await appDbContext.Piece.ToListAsync();
            var piecesRefinedList = piecesList.FindAll(p => p.GameId == GameId);
            return piecesRefinedList;
        }
        public async Task<IEnumerable<Piece>> GetTruckPieces(int GameId)
        {
            var piecesList = await appDbContext.Piece.Where(p=>p.GameId==GameId && p.NOwner==3).ToListAsync();
            // force the context to refresh the selected pieces on next query
            foreach(Piece piece in piecesList)
                appDbContext.Entry(piece).State = EntityState.Detached;
            // get refreshed pieces
            piecesList = await appDbContext.Piece.Where(p => p.GameId == GameId && p.NOwner == 3).ToListAsync();
            return piecesList;
        }

        public async Task<IEnumerable<Piece>> GetHistoryPieces(int HistoryId)
        {
            // get pieces associated with the historyId
            var piecesList = await appDbContext.HistoryPiece
                                        .Where(hp => hp.HistoryId==HistoryId)
                                        .Include(hp => hp.Piece)
                                        .Select(hp => hp.Piece)
                                        .ToListAsync();
            // mark pieces for refresh
            foreach (Piece piece in piecesList)
                appDbContext.Entry(piece).State = EntityState.Detached;

            // get refreshed pieces
            return await appDbContext.HistoryPiece
                                        .Where(hp => hp.HistoryId == HistoryId)
                                        .Include(hp => hp.Piece)
                                        .Select(hp => hp.Piece)
                                        .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetGameList()
        {
            return await appDbContext.Game.ToListAsync();
        }

        public async Task<GameActionResult> SubmitAction(GameAction action)
        {
            GameActionResult result = new GameActionResult(action.nActionType, action.gameId, 4); // action not implemented

            // check action is legit
            if(action.nActionType>0 && action.nActionType<5)
            {
                result = await IsActionAllowed(action);
                if (result.nResponseCode != 1)
                    return result;
            }
            switch (action.nActionType)
            {
                case 1:
                    result = await MovePiece(action);
                    break;
                case 2:
                    result = await Harvest(action);
                    break;
                case 5: // Create game
                    result = await CreateGame(action);
                    break;
                case 6: // Join game
                    result = await JoinGame(action);
                    break;
                case 7: // Add user
                    break;
            }

            return result;
        }

        private async Task<GameActionResult> IsActionAllowed(GameAction action)
        {
            GameActionResult result = new GameActionResult(action.nActionType, action.gameId);

            // get game
            Game game = await GetGame(action.gameId);
            if (game == null)
            {
                result.nResponseCode = 2;
                return result;
            }

            action.game = game;

            // check if game setup is complete
            if ((game.NGameStatus & 1) == 0)
            {
                result.nResponseCode = 11;
                return result;
            }

            // check if game is over
            if ((game.NGameStatus & 2) == 0)
            {
                result.nResponseCode = 10;
                return result;
            }

            // get players
            List<Player> players = (List<Player>)await GetPlayers(action.gameId);

            // confirm player exists in game
            var player = players.Where(p => p.Identity == action.strIdentity).First();
            if (player == null)
            {
                result.nResponseCode = 3;
                return result;
            }

            action.player = player;
            action.otherplayer = players.Where(p => p.Identity != action.strIdentity).FirstOrDefault();


            // check it is this player's turn
            int nPlayerTurn;
            if ((game.NGameStatus & 4) != 0)
                nPlayerTurn = 2;
            else
                nPlayerTurn = 1;
            if (player.NPlayer != nPlayerTurn)
            {
                result.nResponseCode = 10;
                return result;
            }

            result.nResponseCode = 1;
            return result;
        }

        private async Task<GameActionResult> MovePiece(GameAction action)
        {
            GameActionResult result = new GameActionResult(action.nActionType, action.gameId);
            
            Piece piece;
            try
            {
                piece = await appDbContext.Piece.FindAsync(action.pieceId);
            }
            catch(Exception)
            {
                // piece has already been picked up
                result.nResponseCode = 11;
                return result;
            }
            piece.NOwner = action.player.NPlayer;

            // setup history object
            History history = new History();
            history.NPlayer = action.player.NPlayer;
            history.NActionType = action.nActionType;
            history.GameId = piece.GameId;

            // check if placement will require piece to be turned over
            if (action.nActionArguement%5 != piece.NPosition)
                piece.NPieceStatus = (piece.NPieceStatus | 2); // set bit 2 - star side down 

            history.NSourcePos = piece.NPosition;
            history.NDestPos = action.nActionArguement;
            piece.NPosition = action.nActionArguement;
            appDbContext.Piece.Update(piece);

            // Get turn count from nGameStatus (MSByte)
            int turns = (action.game.NGameStatus & 0b1111_1111_0000_0000) / 256;
            turns++;

            // flip player turn bit
            action.game.NGameStatus = (action.game.NGameStatus ^ 4);

            // reform nGameStatus
            action.game.NGameStatus = (action.game.NGameStatus & 0b0000_0000_1111_1111) + turns * 256;
            appDbContext.Game.Update(action.game);
            var dBresult = appDbContext.History.Add(history);
            
            // save required at this point to populate dBresult.HistoryId
            // and to allow the reload truck check to reflect the changes so far
            await appDbContext.SaveChangesAsync();

            // add linking HistoryPiece entry
            HistoryPiece historyPiece = new HistoryPiece
            {
                HistoryId = history.HistoryId,
                PieceId= piece.PieceId
            };

            appDbContext.HistoryPiece.Add(historyPiece);


            var unusedPiece = await appDbContext.Piece.Where(p => p.GameId == action.gameId && p.NOwner == 0).FirstAsync();
            if(unusedPiece!=null) 
            {
                unusedPiece.NOwner = (int)eOwner.mainboard;
                unusedPiece.NPosition = history.NSourcePos;
            }

            await appDbContext.SaveChangesAsync();
            result.nGameStatus = action.game.NGameStatus;
            result.nResponseCode = 1;   // OK
            return result;
        }

        private async Task<GameActionResult> Harvest(GameAction action)
        {
            GameActionResult result = new GameActionResult(action.nActionType, action.gameId);

            var score = new Score(action.Pieces);
            action.player.NScore += score.Calculate();

            // increase score for each star
            foreach(var piece in action.Pieces)
            {
                // if bit 2 has not been set the piece has not been flipped
                if ((piece.NPieceStatus&2)!=2) 
                    action.player.NScore++;
            }

            // setup history object
            History history = new History();
            history.NPlayer = action.player.NPlayer;
            history.NActionType = action.nActionType;
            history.GameId = action.gameId;
            if (action.player.NPlayer == 1)
            {
                history.NScore1 = action.player.NScore;
                history.NScore2 = action.otherplayer.NScore;
                result.nScore1 = action.player.NScore;
                result.nScore2 = action.otherplayer.NScore;
            }
            else
            {
                history.NScore2 = action.player.NScore;
                history.NScore1 = action.otherplayer.NScore;
                result.nScore2 = action.player.NScore;
                result.nScore1 = action.otherplayer.NScore;
            }

            // Get turn count from nGameStatus (MSByte)
            int turns = (action.game.NGameStatus & 0b1111_1111_0000_0000) / 256;
            turns++;

            // flip player turn bit
            action.game.NGameStatus = (action.game.NGameStatus ^ 4);

            // reform nGameStatus
            action.game.NGameStatus = (action.game.NGameStatus & 0b0000_0000_1111_1111) + turns * 256;
            appDbContext.Game.Update(action.game);
            appDbContext.Player.Update(action.player);
            appDbContext.History.Add(history);

            // save required at this point to populate dBresult.HistoryId
            // and to allow the reload truck check to reflect the changes so far
            await appDbContext.SaveChangesAsync();

            // add linking HistoryPiece entries
            foreach(Piece piece in action.Pieces)
            {
                // mark piece as discarded
                piece.NOwner = (int)eOwner.discard;

                HistoryPiece historyPiece = new HistoryPiece
                {
                    HistoryId = history.HistoryId,
                    PieceId = piece.PieceId
                };
                appDbContext.HistoryPiece.Add(historyPiece);
            }

            // save alterations to pieces
            appDbContext.Piece.UpdateRange(action.Pieces);

            await appDbContext.SaveChangesAsync();
            result.nGameStatus = action.game.NGameStatus;
            result.nResponseCode = 1;   // OK
            return result;
        }

        private async Task<GameActionResult> CreateGame(GameAction action)
        {
            GameActionResult result = new GameActionResult(action.nActionType); // unknown error by default

            // confirm action type
            if (action.nActionType != 5)
                return result;

            var User = await GetUser(action.strIdentity);

            if (User == null)
                return result; // unknown error by default

            // check if their game slots are full
            if (User.GameId1 != 0 && User.GameId2 != 0 && User.GameId3 != 0 && User.GameId4 != 0)
            {
                result.nResponseCode = 7;
                return result;
            }

            // create new player opject
            Player player = new Player();
            player.Identity = action.strIdentity;
            player.NPlayer = 1;
            player.NScore = 0;
            player.NPlayerStatus = 1;   // active

            // new game database object
            Game game = new Game();
            game.Player.Add(player);
            /*
             * Bit  | On            | Off
             * 1    | setup complete| need player
             * 2    | Incomplete    | Complete 
             * 4    | Player 2 turn | Player 1 turn
             * 8    | Bonus turn    | Normal turn
             */
            game.NGameStatus = 2;   // Need player (0) + Incomplete (2) + player1 turn (0) + normal turn (0)

            PiecesBag bag = new PiecesBag();

            // give the first 5 to the main board
            for (int i = 0; i < 5; i++)
            {
                Piece piece = new Piece();
                piece.NOwner = (int)eOwner.mainboard;
                piece.NPosition = i;
                piece.NPieceType = (int)bag.GetNextPiece();
                piece.HashIndex = i;
                game.Piece.Add(piece);
            }

            // give the rest to the bag
            for(int i=0; i<85; i++)
            {
                Piece piece = new Piece();
                piece.NOwner = (int)eOwner.bag;
                piece.NPosition = 0;
                piece.NPieceType = (int)bag.GetNextPiece();
                piece.HashIndex = i + 5;
                game.Piece.Add(piece);
            }

            var dBresult = await appDbContext.Game.AddAsync(game);
            await appDbContext.SaveChangesAsync();  // need to initialise GameId

            // assign GameId one of the user's game slots
            if (User.GameId1 == 0)
                User.GameId1 = dBresult.Entity.GameId;
            else if (User.GameId2==0)
                User.GameId2 = dBresult.Entity.GameId;
            else if (User.GameId3 == 0)
                User.GameId3 = dBresult.Entity.GameId;
            else
                User.GameId4 = dBresult.Entity.GameId;

            appDbContext.User.Update(User);
            await appDbContext.SaveChangesAsync();
            result.gameId = dBresult.Entity.GameId;
            result.nResponseCode = 1; // OK
            return result;
        }

        private async Task<GameActionResult> JoinGame(GameAction action)
        {
            GameActionResult result = new GameActionResult(action.nActionType,action.gameId); // unknown error by default

            // confirm action type
            if (action.nActionType != 6)
                return result;

            var User = await GetUser(action.strIdentity);

            if (User == null)
                return result; // unknown error by default

            // check if their game slots are full
            if (User.GameId1 != 0 && User.GameId2 != 0 && User.GameId3 != 0 && User.GameId4 != 0)
            {
                result.nResponseCode = 7;
                return result;
            }

            Player player = new Player();
            player.Identity = action.strIdentity;
            player.NPlayer = 2;
            player.NScore = 0;
            player.NPlayerStatus = 1;   // active

            // get game
            Game game = await GetGame(action.gameId);
            if (game == null)
            {
                result.nResponseCode = 2; // game not exist
                return result;
            }

            // get player(s)
            var playerBulkList = await appDbContext.Player.ToListAsync();
            var playerList = playerBulkList.FindAll(p => p.GameId == action.gameId);

            // can't join a game with more than one player
            if (playerList.Count() > 1)
            {
                result.nResponseCode = 6; // game full
                return result;
            }

            // add player
            playerList.Add(player);

            // assign player list
            game.Player = playerList;

            game.NGameStatus = 1 + 2; // setup complete + game incomplete

            appDbContext.Game.Update(game);

            // assign GameId one of the user's game slots
            if (User.GameId1 == 0)
                User.GameId1 = action.gameId;
            else if (User.GameId2 == 0)
                User.GameId2 = action.gameId;
            else if (User.GameId3 == 0)
                User.GameId3 = action.gameId;
            else
                User.GameId4 = action.gameId;

            appDbContext.User.Update(User);
            await appDbContext.SaveChangesAsync();
            result.nResponseCode = 1; // OK
            return result;
        }

        public async Task<History> GetLastMove(int GameId)
        {
            var historyList = await appDbContext.History.Where(h => h.GameId==GameId).ToListAsync();
            int iHighest = 0;
            History result=new History();
            foreach(var move in historyList)
            {
                if(iHighest<move.HistoryId)
                {
                    result = move;
                    iHighest = move.HistoryId;
                }
            }

            // if there is no history make sure we return null
            if (result.HistoryId == 0)
                return null;
            return result;
        }

        private async Task<User> AddUser(string strIdentity)
        {
            User newUser = new User();

            newUser.Identity = strIdentity;
            var dBresult = await appDbContext.User.AddAsync(newUser);
            await appDbContext.SaveChangesAsync();
            return dBresult.Entity;
        }

        private async Task<GameActionResult> AddUser(GameAction action)
        {
            GameActionResult result = new GameActionResult(action.nActionType); // unknown error by default

            // confirm action type
            if (action.nActionType != 7)
                return result;

            var users = await GetUsers();

            foreach (User user in users)
            {
                if (user.Identity == action.strIdentity)
                {
                    result.nResponseCode = 8;
                    return result;
                }
            }

            // minimum Identity string length = 8
            if (action.strIdentity.Length < 9)
            {
                result.nResponseCode = 5; // bad player name
                return result;
            }

            await AddUser(action.strIdentity);
            result.nResponseCode = 1;
            return result;
        }
    }
}
