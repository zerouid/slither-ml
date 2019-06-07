using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using Slither.ML.Interfaces;
using Slither.ML.Logic;

namespace Slither.ML.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UIController : ControllerBase
    {
        private readonly ISightService _sightService;
        private readonly ILogger _logger;
        private readonly Game _game;
        public UIController(ISightService sightService, ILogger<UIController> logger, Game game)
        {
            _sightService = sightService ?? throw new ArgumentNullException(nameof(sightService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _game = game ?? throw new ArgumentNullException(nameof(game));
        }

        [HttpGet("spectate")]
        public IActionResult GetVideoStream(CancellationToken cancellationToken)
        {
            // await _sightService.GrabScreen();
            // var loc = await _sightService.Find(GameUtils.DinoSprite["TREX"]);
            // if (loc.HasValue)
            // {
            // _logger.LogDebug($"Found Dino at: {loc.Value}");
            // var focus = new Rect(loc.Value.X - 10, loc.Value.Y - 97, 600, 150);
            // await _sightService.FocusOn(focus);
            // _logger.LogDebug($"FocusOn: {loc}");
            // await _sightService.GetScore();
            // _logger.LogDebug("Grabbed the score.");
            _sightService.GrabScreen();
            byte[] png = _sightService.GetScreenCaptureImage(".png");
            return File(png, "image/png");
            // }
            // else
            // {
            //     _logger.LogInformation("Cannot find Dino :-(.");
            //     return NotFound();
            // }
        }

        [HttpPost("start")]
        public IActionResult StartPlaying(CancellationToken cancellationToken)
        {
            _game.Prepare();
            _game.Restart();
            return Ok();
        }

        [HttpGet("score")]
        public IActionResult GetScore(CancellationToken cancellationToken)
        {
            return Ok(_game.Score);
        }
    }
}