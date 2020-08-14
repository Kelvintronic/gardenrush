using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Update.Internal;
using gardenrush.lib.Data;
using gardenrush.lib.services;
using gardenrush.models;

namespace gardenrush.Pages
{
    public class PlayerBoardBase : ComponentBase
    {

        [Inject]
        protected IGardenRepository gameService { get; set; }

        public IEnumerable<Piece> Pieces { get; set; }

        public List<Piece> OurPieces { get; set; } = new List<Piece>();

        [CascadingParameter]
        List<Piece> AllPieces { get; set; }

        // Player name, number and gameId passed by parent
        [Parameter]
        public string playerName { get; set; }

        [Parameter]
        public int nPlayer { get; set; }

        [Parameter]
        public bool bHost { get; set; }

        [Parameter]
        public int nGameStatus { get; set; }

        [Parameter]
        public int gameId { get; set; }

        [Parameter]
        public EventCallback<Status> OnChange { get; set; }


        private bool bHarvest;

        // Status variables calculated from nGameStatus in OnParametersSet()
        private bool bOurTurn;
        private bool bInitialized = false;

        // Harvest variables
        private bool[] bHighlightState = new bool[25];
        public List<Piece> harvestList = new List<Piece>();
        public List<int> harvestIdList = new List<int>();
        public int companionBonus;

        public PlayerBoardBase()
        {

        }

        private void NotifyChange(Status status) => OnChange.InvokeAsync(status);

        public void Update(int gameStatus, Piece piece = null)
        {
            // if a piece has been placed on our board
            if(piece!=null)
                OurPieces.Add(piece);

            // is it this player's turn?
            int nPlayerTurn;
            if ((gameStatus & 4) != 0)
                nPlayerTurn = 2;
            else
                nPlayerTurn = 1;
            if (nPlayer == nPlayerTurn)
                bOurTurn = true;
            else
                bOurTurn = false;

            StateHasChanged();

        }

        public void ConfirmHarvest(List<Piece> pieces=null)
        {
            if (pieces == null)
                pieces = harvestList;

            foreach (Piece piece in pieces)
                OurPieces.Remove(piece);
        }

        // companion position matrix
        private static int[] companion = { -5, -1, 1, 5 };

        protected async void HarvestClick(MouseEventArgs eventArgs)
        {
            if (!bOurTurn)
                return;
            if (bHarvest)
            {
                bHarvest = false;
                if(harvestList.Count()>0)
                {
                    var harvestCompanionList = new List<int>();
                    foreach(Piece piece in harvestList)
                    {
                        bHighlightState[piece.NPosition] = false;
                        harvestIdList.Add(piece.NPosition + (nPlayer-1)*25+5);

                        // check for companions
                        for (int i = 0; i < 4; i++)
                        {
                            int pos = piece.NPosition + companion[i];
                            if (pos >= 0 && pos < 25)
                            {
                                Piece companionPiece = OurPieces.Find(p => p.NPosition == pos);
                                if (companionPiece != null)
                                {
                                    ePieceType companionType = companionPiece.GetPieceType();
                                    if (companionPiece.GetPieceType() == ePieceType.pepper_red ||
                                        companionPiece.GetPieceType() == ePieceType.pepper_yellow)
                                        companionType = ePieceType.pepper_green;
                                    if (companionType == piece.GetCompanionType())
                                    {
                                        harvestCompanionList.Add(pos);
                                    }
                                }
                            }
                        }
                    }

                    // filter companion list for distinct items
                    var distinctCompanionSet = harvestCompanionList.Distinct();

                    // set bonus points for companions
                    companionBonus = distinctCompanionSet.Count();

                    NotifyChange(new Status(nPlayer, true));
                }
            }
            else
            {
                harvestList.Clear();
                harvestIdList.Clear();
                bHarvest = true;
            }
            StateHasChanged();
        }

        protected async void PlayerGridClick(MouseEventArgs eventArgs, int buttonId)
        {
            // Are we the host
            if (!bHost)
                return;

            // is it our turn?
            if (!bOurTurn)
                return;

            // Get position from buttonId
            int position = buttonId - (nPlayer - 1) * 25 - 5;

            // is there a vege in the plot?
            Piece piece = OurPieces.Find(p => p.NPosition == position);

            if(bHarvest && piece!=null)
            {
                // only allow same piece types to be harvested
                if(harvestList.Count()>0)
                {
                    if(harvestList[0].NPieceType==(int)ePieceType.pepper_green||
                        harvestList[0].NPieceType == (int)ePieceType.pepper_red||
                        harvestList[0].NPieceType == (int)ePieceType.pepper_yellow)
                    {
                        if (piece.NPieceType != (int)ePieceType.pepper_green &&
                            piece.NPieceType != (int)ePieceType.pepper_red &&
                            piece.NPieceType != (int)ePieceType.pepper_yellow)
                            return;
                    }
                    else
                    if (piece.NPieceType != harvestList[0].NPieceType)
                        return;
                }

                // remove the piece if clicked twice
                if (harvestList.Contains(piece))
                {
                    harvestList.Remove(piece);
                    bHighlightState[position] = false;
                }
                else
                {
                    harvestList.Add(piece);
                    bHighlightState[position] = true;
                }
                StateHasChanged();
            }

            if (piece != null)
                return; // yes


            NotifyChange(new Status(nPlayer, buttonId));
        }

        
        protected string GetHarvestButtonColour()
        {
            if(bHarvest)
                return "red";
            return "none";
        }

        protected string GetBorderStyle(int id)
        {
            // 7 = number of items on the truck
            // 35 = number of items in a player's garden
            id = id - (nPlayer - 1) * 25 - 5;
            if(bHighlightState[id])
                return "solid";

            return "none";
        }
        protected string GetImageFilePath(int id)
        {
            // 7 = number of items on the truck
            // 35 = number of items in a player's garden
            id = id - (nPlayer - 1) * 25 - 5;

            // Get piece image so long as it's status is not "in hand"
            var result = OurPieces.Find(p => p.NPosition == id && (p.NPieceStatus & 1) == 0);
            if (result == null)
                return "";

            return result.GetImageFileName();
        }
        protected override void OnParametersSet()
        {
            if(!bInitialized)
            { 
                Console.WriteLine("GetPieces by PlayerBoard {0}", nPlayer);
                OurPieces = AllPieces.Where(p => p.NOwner == nPlayer).ToList();

                Update(nGameStatus);

                bInitialized = true;
            }

        }
    }
}
