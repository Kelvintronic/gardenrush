using System;
using System.Collections;
using System.Collections.Generic;

namespace gardenrush.lib.Data
{
    public class PiecesBag
    {
        public PiecesBag()
        {
            Reset();
        }

        // full set of vege pieces
        public ePieceType[] vegeBag = new ePieceType[90];

        private int nVegeLeft;

        public void Reset()
        {
            // 98 vege pieces in total
            nVegeLeft = 0;
            
            AddVege(ePieceType.corn, 14);
            AddVege(ePieceType.cabbage, 13);
            AddVege(ePieceType.turnip, 12);
            AddVege(ePieceType.tomato, 12);
            AddVege(ePieceType.aubergine, 11);
            AddVege(ePieceType.potato, 10);
            AddVege(ePieceType.pea, 9);
            AddVege(ePieceType.pepper_green, 3);
            AddVege(ePieceType.pepper_red, 3);
            AddVege(ePieceType.pepper_yellow, 3);
            Shuffle();
        }

        void AddVege(ePieceType pieceType, int count)
        {
            while(count>0)
            {
                if (nVegeLeft == 90)
                    return;
                vegeBag[nVegeLeft]=pieceType;
                count--;
                nVegeLeft++;
            }
        }

        void Shuffle()
        {
            int Pos1;
            int Pos2;
            ePieceType pieceBuff;
            var rnd = new Random();

            // shuffle veg
            for(int i=0; i<500; i++)
            {
                Pos1 = rnd.Next(90);
                pieceBuff = vegeBag[Pos1];
                Pos2 = rnd.Next(90);
                vegeBag[Pos1] = vegeBag[Pos2];
                vegeBag[Pos2] = pieceBuff;
            }
        }

        public ePieceType GetNextPiece()
        {
            if(nVegeLeft==0)
                return ePieceType.none;
            nVegeLeft--;
            return vegeBag[nVegeLeft];
        }
    }
}
