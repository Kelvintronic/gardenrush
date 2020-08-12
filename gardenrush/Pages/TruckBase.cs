using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using gardenrush.lib.Data;
using gardenrush.lib.services;
using gardenrush.models;
using Microsoft.JSInterop;

namespace gardenrush.Pages
{
    public class TruckBase : ComponentBase  //, IDisposable
    {
        //protected MainBoard mainBoard;

        [Inject]
        protected IGardenRepository gameService { get; set; }

        public IEnumerable<Piece> Pieces { get; set; }

        public List<Piece> OurPieces { get; set; } = new List<Piece>();

        public TruckBase()
        {

        }

        [CascadingParameter]
        List<Piece> AllPieces { get; set; }

        [Parameter]
        public int gameId { get; set; }
        [Parameter]
        public int nGameStatus { get; set; }
        [Parameter]
        public int nSessionPlayer { get; set; }

        [Parameter]
        public string strIdentity { get; set; }

        [Parameter] 
        public EventCallback<Status> OnChange { get; set; }

        private void NotifyChange(Status status) => OnChange.InvokeAsync(status);

        // Status variables calculated from nGameStatus in OnParametersSet()
        private bool bOurTurn;
        private bool[] bHighlightState = new bool[5];
        private bool bInitialized = false;

        public void Update(int gameStatus, Piece piece=null)
        {
            if (piece != null)
                OurPieces.Remove(piece);

            if (gameStatus != -1)
            {
                int nPlayerTurn;
                if ((gameStatus & 4) != 0)
                    nPlayerTurn = 2;
                else
                    nPlayerTurn = 1;
                if (nSessionPlayer == nPlayerTurn)
                    bOurTurn = true;
                else
                    bOurTurn = false;
            }
            StateHasChanged();
        }

        public void Empty()
        {
            OurPieces.Clear();
        }
        public void AddVeg(Piece piece)
        {
            OurPieces.Add(piece);
            StateHasChanged();
        }


        protected async void TruckGridClick(MouseEventArgs eventArgs, int buttonId)
        {
            // is it the session player's turn?
            if (!bOurTurn)
                return;

            // is there a vege in the plot?
            Piece piece = OurPieces.Find(p => p.NPosition == buttonId);
            if (piece == null)
                return;

            ClearHighlighting();

            bHighlightState[buttonId] = true;

            NotifyChange(new Status(0, buttonId,piece));
        }
        protected string GetImageFilePath(int id)
        {
            var result = OurPieces.Find(p => p.NPosition == id);
            if (result == null)
                return "";
            return result.GetImageFileName();
        }
        protected string GetBorderStyle(int id)
        {
            if (bHighlightState[id])
                return "solid";

            return "none";
        }
        public void ClearHighlighting()
        {
            for (int i = 0; i < 5; i++)
                bHighlightState[i] = false;
        }


        protected override void OnParametersSet()
        {
            if(!bInitialized)
            {
                Console.WriteLine("Pieces assigned to CentralBoard");

                // get the list of pieces assigned to mainboard
                OurPieces = AllPieces.Where(p => p.NOwner == (int)eOwner.mainboard).ToList();

                // update self
                Update(nGameStatus);

                bInitialized = true;
            }

        }
    }
}
