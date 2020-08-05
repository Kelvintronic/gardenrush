using System;
using System.Collections.Generic;
using System.Text;
using gardenrush.lib.Data;

namespace gardenrush.models
{
    public class Status
    {
        public Status(int nCaller, int id, Piece piece=null)
        {
            this.piece = piece;
            ButtonId = id;
            this.nCaller = nCaller;
            bHarvest = false;
        }

        public Status(int nCaller, bool bHarvest)
        {
            piece = null;
            this.bHarvest = bHarvest;
            this.nCaller = nCaller;
        }

        public bool bHarvest;
        public int ButtonId;
        public int nCaller;
        public Piece piece { get; set; }

    }
}
