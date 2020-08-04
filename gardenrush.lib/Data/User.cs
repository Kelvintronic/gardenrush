using System;
using System.Collections.Generic;

namespace gardenrush.lib.Data
{
    public partial class User
    {
        public User()
        {
            GameId1 = 0;
            GameId2 = 0;
            GameId3 = 0;
            GameId4 = 0;
        }
        public int UserId { get; set; }
        public string Identity { get; set; }
        public int? GameId1 { get; set; }
        public int? GameId2 { get; set; }
        public int? GameId3 { get; set; }
        public int? GameId4 { get; set; }
    }
}
