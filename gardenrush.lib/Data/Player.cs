using System;
using System.Collections.Generic;

namespace gardenrush.lib.Data
{
    public partial class Player
    {
        public int PlayerId { get; set; }
        public int NPlayer { get; set; }
        public string Identity { get; set; }
        public int NPlayerStatus { get; set; }
        public int NScore { get; set; }
        public int GameId { get; set; }

        public virtual Game Game { get; set; }
    }
}
