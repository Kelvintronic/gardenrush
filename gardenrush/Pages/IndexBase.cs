using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Threading;
using gardenrush.lib.Data;
using gardenrush.lib.services;
using gardenrush.Services;
using gardenrush.models;
using System.Text.Json;

namespace gardenrush.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        protected NavigationManager navManager { get; set; }
        [Inject]
        protected IGardenRepository gameService { get; set; }

        [Inject]
        protected TurnService turnService { get; set; }

        [Inject]
        protected AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        public IndexBase()
        {

        }


        public int gameId { get; set; }
        public Player OtherPlayer;
        public Player Player;
        public List<Piece> GamePieces;
        public Game game;
        public string TurnMessage;

        protected bool bGameLoaded { get; set; }
        protected bool bVerified { get; set; }

        protected bool bPollServer;

        // children 
        protected PlayerBoard playerBoard;
        protected Truck truck;
        protected PlayerBoard otherBoard;

        bool bFrom;
        int iFrom;
        Piece pieceFrom;

        protected async void TestFunction(MouseEventArgs eventArgs)
        {
            var fromItems = new List<string>();
            fromItems.Add("5");
            fromItems.Add("10");
            fromItems.Add("15");
            fromItems.Add("20");
            await JSRuntime.InvokeVoidAsync("doAnimateArray", (object)fromItems);

            await Task.Delay(1000); // time it takes for animation to complete.
        }

        protected async Task RefillTruck(int nPosition)
        {
            List<Piece> TruckPieces = (List<Piece>)await gameService.GetTruckPieces(gameId);

            Piece newPiece=null;
            foreach(var piece in TruckPieces)
                if (piece.NPosition == nPosition)
                {
                    newPiece = piece;
                    break;
                }

            if(newPiece!=null)
            {
                await JSRuntime.InvokeVoidAsync("tileAnimateImage", "bag", newPiece.NPosition, newPiece.GetImageFileName());
                await Task.Delay(600); // time it takes for animation to complete.
                truck.AddVeg(newPiece);
            }
        }

        protected void UpdateScores(int score1, int score2)
        {
            if (Player.NPlayer == 1)
            {
                Player.NScore = score1;
                OtherPlayer.NScore = score2;
            }
            else
            {
                Player.NScore = score2;
                OtherPlayer.NScore = score1;
            }
        }
        public async void StatusChangeHandler(Status status)
        {
            Console.WriteLine("StatusChangedHandler");

            if (status.nCaller==0)
            {
                bFrom = true;
                iFrom = status.ButtonId;
                pieceFrom = status.piece;
                StateHasChanged();
            }
            if (status.nCaller!=0&&status.bHarvest)
            {

                var action = new GameAction(gameId, playerBoard.companionBonus, Player.Identity, playerBoard.harvestList);
                var result = await gameService.SubmitAction(action);

                if (result.nResponseCode == 1)
                {
                    await JSRuntime.InvokeVoidAsync("doAnimateArray", (object)playerBoard.harvestIdList);

                    await Task.Delay(100); // wait for javascript to grab the images

                    // updating components before animation completes
                    // so the pieces dissappear from the board                                    
                    game.NGameStatus = result.nGameStatus;
                    playerBoard.ConfirmHarvest();
                    playerBoard.Update(game.NGameStatus);
                    otherBoard.Update(game.NGameStatus);

                    await Task.Delay(900); // rest of time it takes for animation to complete.

                    UpdateScores(result.nScore1, result.nScore2);

                    bPollServer = true;
                    turnService.SetGameStatus(game.GameId, game.NGameStatus);

                    TurnMessage = "Please wait";

                    StateHasChanged();
                }
            }
            if (status.nCaller!=0&& bFrom)
            {
                bFrom = false;
                truck.ClearHighlighting();
                int sourcePosition = pieceFrom.NPosition;   // save source position so we can refill it later
                var action = new GameAction(1, status.ButtonId - (status.nCaller - 1) * 25 - 5, 
                                                    pieceFrom.PieceId, game.GameId, Player.Identity);
                var result = await gameService.SubmitAction(action);
                if (result.nResponseCode == 1)
                {

                    // animate
                    // this call returns immediately but the javascript method sets up an
                    // animation that takes 1000ms to complete
                    await JSRuntime.InvokeVoidAsync("tileAnimate", iFrom, status.ButtonId);

                    await Task.Delay(100); // wait for javascript to grab the image

                    // remove old piece from truck list
                    // so the piece dissappears from the board                                    
                    truck.Update(game.NGameStatus, pieceFrom);

                    await Task.Delay(900); // rest of time it takes for animation to complete.

                    // flip piece if required
                    if (action.nActionArguement % 5 != pieceFrom.NPosition)
                        pieceFrom.NPieceStatus = (pieceFrom.NPieceStatus | 2); // set bit 2 - star side down 

                    // set pieceFrom new position and owner
                    pieceFrom.NPosition = action.nActionArguement;
                    pieceFrom.NOwner = Player.NPlayer;

                    // updating components after animation completes
                    // so the piece reappears at the destination
                    game.NGameStatus = result.nGameStatus;
                    playerBoard.Update(game.NGameStatus, pieceFrom);
                    otherBoard.Update(game.NGameStatus);

                    // first check if the game is over
                    if ((game.NGameStatus & 2) == 0)
                    {
                        TurnMessage = "Game Over";
                    }
                    else
                    {
                        bPollServer = true;
                        TurnMessage = "Please wait";
                    }

                    turnService.SetGameStatus(game.GameId, game.NGameStatus);

                    // add new piece
                    await RefillTruck(sourcePosition);

                    StateHasChanged();
                }
            }

        }
        private void RefreshGame()
        {
            Console.WriteLine("Index:SetParametersAsync nPlayer={0}", Player.NPlayer);

            string redirectUrl;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                redirectUrl = "/gardenrush/?game=" + gameId;
            else
                redirectUrl = "/?game=" + gameId;

            navManager.NavigateTo(redirectUrl, true);
        }
        public override Task SetParametersAsync(ParameterView parameters)
        {
            Console.WriteLine("Index:SetParametersAsync");

            var uri = navManager.ToAbsoluteUri(navManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("game", out var param))
            {
                var gameParam = param.First();
                try
                {
                    gameId = Int32.Parse(gameParam);
                }
                catch (FormatException)
                {
                    gameId = 0;
                    Console.WriteLine("Index:SetParametersAsync: Fail Arguement Parse");
                    return base.SetParametersAsync(parameters);
                }
            }
            else
            {
                gameId = 0;
                Console.WriteLine("Index:SetParametersAsync: Fail - No query string");
                return base.SetParametersAsync(parameters);
            }
            Console.WriteLine("Index:SetParametersAsync: Success");
            return base.SetParametersAsync(parameters);
        }

        protected override async Task OnInitializedAsync()
        {
            bPollServer = false;
            Console.WriteLine("Index:OnInitializedAsync");
            Console.WriteLine("VerifyGame");

            if (!bGameLoaded)
            {
                Console.WriteLine("VerifyGame: Loading game...");
                if (gameId == 0)
                {
                    Console.WriteLine("VerifyGame: Load Fail - game==0");
                    return;
                }
                game = await gameService.GetGame(gameId);
                if (game != null)
                {
                    game.Player = (ICollection<Player>)await gameService.GetPlayers(gameId);
                    game.Piece = (ICollection<Piece>)await gameService.GetPieces(gameId);
                }

                if (game.Piece != null && game.Player != null)
                {
                    bGameLoaded = true;
                    GamePieces = game.Piece.ToList();
                    Console.WriteLine("VerifyGame: Load Success - game=={0}", gameId);
                }
                else
                {
                    Console.WriteLine("VerifyGame: Load Fail - unknow error");
                    return;
                }
            }

            // check if game setup is complete
            if ((game.NGameStatus & 1) == 0)
                return;

            // get user
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            foreach (Player player in game.Player)
            {
                if (player.Identity == user.Identity.Name)
                {
                    Player = player;
                    OtherPlayer = game.Player.Where(p => p.Identity != user.Identity.Name).FirstOrDefault();
                    Console.WriteLine("VerifyGame: Yes");
                    bVerified = true;

                    // is it our turn?

                    // poll server only if game is still active AND it is not our turn
                    if((game.NGameStatus&2)!=0)
                    {
                        if ((game.NGameStatus & 4) != 0 && Player.NPlayer == 1 ||
                            (game.NGameStatus & 4) == 0 && Player.NPlayer == 2)
                        {
                            bPollServer = true;
                            TurnMessage = "Please wait";
                        }
                        else
                            TurnMessage = "Your turn";
                    }
                    return;
                }
            }

            Console.WriteLine("VerifyGame: No - Bad player");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // simple polling of the server
            if(bPollServer)
            {
                await Task.Delay(1000);
                int nGameStatus =  turnService.GetGameStatus(gameId);
                if(nGameStatus!=0)
                {
                    // first check if the game is over
                    if ((nGameStatus & 2) == 0)
                    {
                        TurnMessage = "Game Over";
                        game.NGameStatus = nGameStatus;
                        bPollServer = false;    // if so stop polling
                    }

                    if (nGameStatus != game.NGameStatus)
                    {
                        TurnMessage = "Your turn";
                        bPollServer = false;
                        game.NGameStatus = nGameStatus;
                        var history = await gameService.GetLastMove(gameId);
                        if(history!=null)
                        {
                            List<Piece> pieces = (List<Piece>)await gameService.GetHistoryPieces(history.HistoryId);

                            if (history.NActionType==1)
                            {

                                if(pieces.Count()!=0)
                                {
                                    // animate
                                    // this call returns immediately but the javascript method sets up an
                                    // animation that takes 1000ms to complete
                                    await JSRuntime.InvokeVoidAsync("tileAnimate", history.NSourcePos, history.NDestPos + (OtherPlayer.NPlayer - 1) * 25 + 5);

                                    await Task.Delay(100); // wait for javascript to grab the images

                                    // updating components before animation completes
                                    // so the pieces dissappear from the board                                    
                                    playerBoard.Update(game.NGameStatus, null);
                                    truck.Update(game.NGameStatus, pieces[0]);      // remove

                                    await Task.Delay(900); // rest of time it takes for animation to complete.

                                    otherBoard.Update(game.NGameStatus, pieces[0]); // add

                                    await RefillTruck(history.NSourcePos);
                                }


                            }
                            else if(history.NActionType==2)
                            {
                                var harvestIdList = new List<int>();
                                foreach(var piece in pieces)
                                    harvestIdList.Add(piece.NPosition + (OtherPlayer.NPlayer - 1) * 25 + 5);
                                await JSRuntime.InvokeVoidAsync("doAnimateArray", (object)harvestIdList);

                                await Task.Delay(100); // wait for javascript to grab the images

                                // updating components before animation completes
                                // so the pieces dissappear from the board                                    
                                otherBoard.ConfirmHarvest(pieces);
                                playerBoard.Update(game.NGameStatus);
                                otherBoard.Update(game.NGameStatus);
                                truck.Update(game.NGameStatus);      // remove

                                await Task.Delay(900); // rest of time it takes for animation to complete.

                                UpdateScores(history.NScore1, history.NScore2);

                            }
                        }

                    }
                }
                StateHasChanged();
            }
        }
    }
}
