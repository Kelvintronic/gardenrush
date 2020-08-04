using System;
using System.Collections.Generic;
using System.Text;

namespace gardenrush.lib.Data
{
    public class GameAction
    {
        public GameAction(int nActionType, int nActionArguement, int gameId, string strIdentity)
        {
            this.nActionType = nActionType;
            this.nActionArguement = nActionArguement;
            this.gameId = gameId;
            this.strIdentity = strIdentity;
        }
        public GameAction(int nActionType, int nActionArguement, int pieceId, int gameId, string strIdentity)
        {
            this.nActionType = nActionType;
            this.nActionArguement = nActionArguement;
            this.gameId = gameId;
            this.strIdentity = strIdentity;
            this.pieceId = pieceId;
        }

        public GameAction(int gameId, string strIdentity, List<Piece> pieces)
        {
            nActionType = 2; // Harvest
            this.gameId = gameId;
            this.strIdentity = strIdentity;
            Pieces = pieces;
        }
        public GameAction()
        {

        }
        public int nActionType { get; set; }
        public int nActionArguement { get; set; }
        public int pieceId { get; set; }
        public int gameId { get; set; }
        public string strIdentity { get; set; }

        // Pieces is used by GardenRepository::Harvest()
        public List<Piece> Pieces { get; set; }

        // the following properties are populated when the
        // action in validated by GardenRepository::IsActionAllowed()
        public Game game { get; set; }
        public Player player { get; set; }
        public Player otherplayer { get; set; }

    }

}
