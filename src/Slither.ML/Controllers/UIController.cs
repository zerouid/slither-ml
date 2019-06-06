using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using Slither.ML.Interfaces;

namespace Slither.ML.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UIController : ControllerBase
    {
        private readonly ISightService _sightService;
        private readonly ILogger _logger;
        public UIController(ISightService sightService, ILogger<UIController> logger)
        {
            _sightService = sightService ?? throw new ArgumentNullException(nameof(sightService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("spectate")]
        public async Task<IActionResult> GetVideoStream(CancellationToken cancellationToken)
        {
            await _sightService.GrabScreen();
            var loc = await _sightService.Find(GameUtils.DinoSprite["TREX"]);
            if (loc.HasValue)
            {
                _logger.LogDebug($"Found Dino at: {loc.Value}");
                var focus = new Rect(loc.Value.X - 10, loc.Value.Y - 97, 600, 150);
                await _sightService.FocusOn(focus);
                _logger.LogDebug($"FocusOn: {loc}");
                // await _sightService.GetScore();
                // _logger.LogDebug("Grabbed the score.");
                byte[] png = await _sightService.GetScreenCaptureImage(".png");
                return File(png, "image/png");
            }
            else
            {
                _logger.LogInformation("Cannot find Dino :-(.");
                return NotFound();
            }
        }


    }
}