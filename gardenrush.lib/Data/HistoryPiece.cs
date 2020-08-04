using System;
using System.Collections.Generic;

namespace gardenrush.lib.Data
{
    public partial class HistoryPiece
    {
        public int HistoryId { get; set; }
        public int PieceId { get; set; }

        public virtual History History { get; set; }
        public virtual Piece Piece { get; set; }
    }
}
