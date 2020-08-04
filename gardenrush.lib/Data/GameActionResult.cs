using System;
using System.Collections.Generic;
using System.Text;

namespace gardenrush.lib.Data
{
    public class GameActionResult
    {
        public GameActionResult(int nActionType, int gameId = 0, int nResponseCode = 0)
        {
            this.nActionType = nActionType;
            this.nResponseCode = nResponseCode;
            this.gameId = gameId;
            bReloadTruck = false;
        }
        public GameActionResult()
        {
            bReloadTruck = false;
        }
        public int nActionType { get; set; }
        public int nResponseCode { get; set; }
        public int gameId { get; set; }
        public int nGameStatus { get; set; }
        public int nScore1 { get; set; }
        public int nScore2 { get; set; }
        public bool bReloadTruck { get; set; }
    }
}
