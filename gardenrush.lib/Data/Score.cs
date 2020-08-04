using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace gardenrush.lib.Data
{
    public class Score
    {
        private List<CoOrdinate> piecesToScore;
        private ePieceType Type;
        public Score(List<Piece> pieces)
        {
            if (pieces == null)
                return;
            piecesToScore = new List<CoOrdinate>();
            foreach(Piece piece in pieces)
            {
                var pieceCoOrd = new CoOrdinate(piece.NPosition);
                piecesToScore.Add(pieceCoOrd);
            }
            Type = (ePieceType)pieces[0].NPieceType;
        }

        public Score(List<CoOrdinate> pieces,ePieceType type)
        {
            Type = type;
            piecesToScore = pieces;
        }

        private static int[,] corn5 =     { { 0, 0 }, { 1, 0 }, { 1, 1 }, { 0, 1 } };
        private static int[,] corn12 =    { { 0, 0 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { 2, 0 }, { 2, 1 } };
        private static int[,] cabbage6 =  { { 0, 0 }, { 0, 1 }, { 1, 2 }, { 2, 2 } };
        private static int[,] cabbage9 =  { { 0, 0 }, { 0, 1 }, { 0, 2 }, { 1, 2 }, { 2, 2 }};
        private static int[,] turnip3 =  { { 0, 0 }, { 1, 1 }, { 2, 0 } };
        private static int[,] turnip10 = { { 0, 0 }, { 1, 1 }, { 2, 0 }, { 3, 1 }, { 4, 0 }};
        private static int[,] tomato3 =   { { 0, 0 }, { 1, 0 }, { 2, 0 } };
        private static int[,] tomato10 =  { { 0, 0 }, { 0, 1 }, { 1, 1 }, { 2, 1 }, { 2, 0 } };
        private static int[,] eggplant2 = { { 0, 0 }, { 1, 1 } };
        private static int[,] eggplant11 ={ { 0, 0 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 0 } };
        private static int[,] potato4 =   { { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 3 } };
        private static int[,] potato7 =   { { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 } };
        private static int[,] peas3 =     { { 0, 0 }, { 2, 0 } };
        private static int[,] peas8 =     { { 0, 0 }, { 1, 1 }, { 2, 0 }, {-1, 1 } };
        private static int[,] pepper5 =   { { 0, 0 }, { 0, 1 }, { 1, 1 } };

        public int Calculate()
        {
            if(piecesToScore==null||piecesToScore.Count==0)
                return 0;

            switch(Type)
            {
                case ePieceType.corn:
                    if (FindMatch(corn12, 6))
                        return 12;
                    if (FindMatch(corn5, 4))
                        return 5;
                    break;
                case ePieceType.cabbage:
                    if (FindMatch(cabbage9, 5))
                        return 9;
                    if (FindMatch(cabbage6, 4))
                        return 6;
                    break;
                case ePieceType.turnip:
                    if (FindMatch(turnip10, 5))
                        return 10;
                    if (FindMatch(turnip3, 3))
                        return 3;
                    break;
                case ePieceType.tomato:
                    if (FindMatch(tomato10, 5))
                        return 10;
                    if (FindMatch(tomato3, 3))
                        return 3;
                    break;
                case ePieceType.aubergine:
                    if (FindMatch(eggplant11, 5))
                        return 11;
                    if (FindMatch(eggplant2, 2))
                        return 2;
                    break;
                case ePieceType.potato:
                    if (FindMatch(potato7, 4))
                        return 7;
                    if (FindMatch(potato4, 3))
                        return 4;
                    break;
                case ePieceType.pea:
                    if (FindMatch(peas8, 4))
                        return 8;
                    if (FindMatch(peas3, 2))
                        return 3;
                    break;
                case ePieceType.pepper_green:
                case ePieceType.pepper_red:
                case ePieceType.pepper_yellow:
                    if (FindMatch(pepper5, 3))
                        return 5;
                    break;
                default:
                    return 0;

            }

            return 0;
        }

        private bool FindMatch(int[,] matrix,int matrixMax)
        {
            for (int rotate = 0; rotate < 4; rotate++)
            {
                for (int i = 0; i < piecesToScore.Count(); i++)
                {
                    int matrixIndex = 0;
                    while (matrixIndex < matrixMax)
                    {
                        var test = new CoOrdinate() { x = piecesToScore[i].x + matrix[matrixIndex, 0], y = piecesToScore[i].y + matrix[matrixIndex, 1] };
                        if (!piecesToScore.Contains(test))
                            break;
                        matrixIndex++;
                    }

                    if (matrixIndex == matrixMax)
                        return true;
                }

                // rotate all values by 90°
                foreach (var piece in piecesToScore)
                    piece.Rotate90();
            }
            return false;
        }

    }
}
