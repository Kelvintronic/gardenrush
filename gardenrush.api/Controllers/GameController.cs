using gardenrush.lib.Data;
using gardenrush.lib.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gardenrush.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGardenRepository gnomeRepository;

        public GameController(IGardenRepository gnomeRepository)
        {
            this.gnomeRepository = gnomeRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetGameList()
        {

            try
            {
                return Ok(await gnomeRepository.GetGameList());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving games from the database");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetGame(int id)
        {

            try
            {
                var result = await gnomeRepository.GetGame(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);  
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving game from the database");
            }
        }

        [HttpGet("{id:int}/players")]
        public async Task<ActionResult> GetPlayers(int id)
        {

            try
            {
                var result = await gnomeRepository.GetPlayers(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving players from the database");
            }
        }

        [HttpGet("{id:int}/pieces")]
        public async Task<ActionResult> GetPieces(int id)
        {

            try
            {
                var result = await gnomeRepository.GetPieces(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving pieces from the database");
            }
        }

        [HttpGet("{id:int}/history")]
        public async Task<ActionResult> GetHistory(int id)
        {
            try
            {
                var result = await gnomeRepository.GetLastMove(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving history from the database");
            }
        }

        [HttpPost("submit")]
        public async Task<ActionResult> SubmitAction(GameAction action)
        {

            try
            {
                return Ok(await gnomeRepository.SubmitAction(action));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error processing action");
            }
        }

    }
}
