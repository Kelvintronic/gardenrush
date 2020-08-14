using System;
using System.Collections.Generic;

namespace gardenrush.lib.Data
{
    public partial class Piece
    {
        public Piece()
        {
            NPieceType = (int)ePieceType.none;
            NPieceStatus = 0;
        }
        public int PieceId { get; set; }
        public int NPieceType { get; set; }
        public int NPieceStatus { get; set; }
        public int NOwner { get; set; }
        public int NPosition { get; set; }
        public int GameId { get; set; }
        public int HashIndex { get; set; }

        public virtual Game Game { get; set; }
        public virtual ICollection<HistoryPiece> HistoryPiece { get; set; }

        private static readonly string[] ImageFileSide1 = new[]
{
            "Corn.png", "Cabbage.png", "Turnip.png", "Tomato.png", "Eggplant.png", "Potato.png", "Peas.png",
            "Pepper-Green.png","Pepper-Red.png","Pepper-Yellow.png"
        };

        private static readonly string[] ImageFileSide2 = new[]
        {
            "Corn-Star.png", "Cabbage-Star.png", "Turnip-Star.png", "Tomato-Star.png", "Eggplant-Star.png", "Potato-Star.png",
            "Peas-Star.png", "Pepper-Green-Star.png","Pepper-Red-Star.png","Pepper-Yellow-Star.png"
        };

        // corn, cabbage, turnip, tomato, aubergine, potato, pea, pepper_green, pepper_red, pepper_yellow, none

        private static ePieceType[] CompanionTypes = { ePieceType.pea, ePieceType.potato, ePieceType.tomato, ePieceType.turnip,
                                                        ePieceType.pepper_green, ePieceType.cabbage, ePieceType.corn, ePieceType.aubergine,
                                                        ePieceType.aubergine, ePieceType.aubergine, ePieceType.none };
        public string GetImageFileName()
        {
            if (NPieceType == (int)ePieceType.none)
                return "";

            string imageFilePath;
            // correct file path
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                imageFilePath = "/gardenrush/images/veg/";
            else
                imageFilePath = "/images/veg/";

            // determine side to show
            if ((NPieceStatus & 2) == 2) // if side bit is set
                return imageFilePath + ImageFileSide1[NPieceType]; // set to star down
            else
                return imageFilePath + ImageFileSide2[NPieceType]; // else set to star up
        }
        public void SetPieceType(ePieceType type)
        {
            NPieceType = (int)type;
        }

        public ePieceType GetPieceType()
        {
            return (ePieceType)NPieceType;
        }

        public ePieceType GetCompanionType()
        {
            return CompanionTypes[NPieceType];
        }

        public bool IsSideOne()
        {
            return (NPieceStatus & 2) != 0;
        }

        public static bool operator ==(Piece left, Piece right)
        {
            if (object.ReferenceEquals(left, right)) return true;

            if ((object)right == null)
                return false;

            return left.PieceId == right.PieceId && left.HashIndex==right.HashIndex;
        }
        public static bool operator !=(Piece left, Piece right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return this == (Piece)obj;
        }

        public override int GetHashCode()
        {
            return PieceId.GetHashCode() ^ HashIndex.GetHashCode();
        }
    }

    public enum eOwner
    {
        bag, player1, player2, mainboard, discard
    }
    public enum ePieceType
    {
        corn, cabbage, turnip, tomato, aubergine, potato, pea, pepper_green, pepper_red, pepper_yellow,
        none
    }
}
