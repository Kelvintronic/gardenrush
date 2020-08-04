using System;
using System.Collections.Generic;

namespace gardenrush.lib.Data
{
    public partial class Game
    {
        public Game()
        {
            History = new HashSet<History>();
            Piece = new HashSet<Piece>();
            Player = new HashSet<Player>();
        }

        public int GameId { get; set; }
        public int NGameStatus { get; set; }

        public virtual ICollection<History> History { get; set; }
        public virtual ICollection<Piece> Piece { get; set; }
        public virtual ICollection<Player> Player { get; set; }
    }
}
