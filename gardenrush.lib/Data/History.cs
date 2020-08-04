using System;
using System.Collections.Generic;

namespace gardenrush.lib.Data
{
    public partial class History
    {
        public int HistoryId { get; set; }
        public int NActionType { get; set; }
        public int NPlayer { get; set; }
        public int NSourcePos { get; set; }
        public int NDestPos { get; set; }
        public int NScore1 { get; set; }
        public int NScore2 { get; set; }
        public int GameId { get; set; }
        public bool BReloadTruck { get; set; }
        public virtual Game Game { get; set; }
        public virtual ICollection<HistoryPiece> HistoryPiece { get; set; }

    }
}
