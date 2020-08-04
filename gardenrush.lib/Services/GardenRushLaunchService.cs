using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using gardenrush.lib.Data;



namespace gardenrush.lib
{
    public class GardenRushLaunchService : IGardenRushLaunchService
    {
        private readonly HttpClient httpClient;
        public GardenRushLaunchService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<GameActionResult> SubmitAction(int nActionType, int nActionArguement, int gameId, string strIdentity)
        {
            GameActionResult result = new GameActionResult(nActionType, gameId);
            GameAction action = new GameAction(nActionType,nActionArguement,gameId,strIdentity);
            
            // post action to game database
            string request = "api/game/submit";

            var httpResponse = await httpClient.PostAsJsonAsync(request, action);

            if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
            {

                var contentStream = await httpResponse.Content.ReadAsStreamAsync();

                try
                {
                    return await JsonSerializer.DeserializeAsync<GameActionResult>(contentStream,
                        new JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });
                }
                catch (JsonException) // Invalid JSON
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }
            else
            {
                Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");
            }

            return result; // Unknown error by default

        }

        public async Task<IEnumerable<Game>> GetGamesList()
        {
            IEnumerable<Game> games = new List<Game>();
            string request = "api/game";

            var httpResponse = await httpClient.GetAsync(request, HttpCompletionOption.ResponseHeadersRead);

            try
            {
                httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299
            }
            catch(Exception)
            {
                Console.WriteLine("HttpGet failed on GetGamesList()");
            }

            if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
            {
                var contentStream = await httpResponse.Content.ReadAsStreamAsync();

                try
                {
                    games = await JsonSerializer.DeserializeAsync<List<Game>>(contentStream,
                        new JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });
                    // games = games.Where(g => g.NGameStatus == 2); // only return games marked as need player + incomplete
                    foreach (Game game in games)
                    {
                        game.Player = (ICollection<Player>)await GetPlayers(game.GameId);
                    }
                    return games;
                }
                catch (JsonException) // Invalid JSON
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }
            else
            {
                Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");
            }

            return null;
        }
        public async Task<IEnumerable<Player>> GetPlayers(int gameId)
        {
            // query game database with gameId to get player list
            string request = "api/game/" + gameId + "/players";
            
            var httpResponse = await httpClient.GetAsync(request, HttpCompletionOption.ResponseHeadersRead);

            try
            {
                httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299
            }
            catch (Exception)
            {
                Console.WriteLine("HttpGet failed on GetPlayers()");
            }

            if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
            {
                var contentStream = await httpResponse.Content.ReadAsStreamAsync();

                try
                {
                    return await JsonSerializer.DeserializeAsync<List<Player>>(contentStream,
                        new JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });
                }
                catch (JsonException) // Invalid JSON
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }
            else
            {
                Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");
            }

            return null;
        }
    }
}
