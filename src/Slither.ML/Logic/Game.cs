using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using Robot.NET;
using Slither.ML.Interfaces;

namespace Slither.ML.Logic
{
    public class Game
    {
        private static readonly Point scoreAreaAdjustment = new Point(195, -3);
        private static readonly Rect playAreaRectangle = new Rect(100, 50, 500, 100);
        private static readonly Size scoreAreaSize = new Size(115, 30);
        private readonly object _lock = new object();
        private readonly ISightService _sightService;
        private readonly ILogger _logger;
        public Rect scoreArea = Rect.Empty;
        public Game(ISightService sightService, ILogger<Game> logger)
        {
            _sightService = sightService ?? throw new ArgumentNullException(nameof(sightService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public bool GameOver => _sightService.IsVisible(GameUtils.DinoSprite["RESTART"]);
        public bool Playing => _sightService.IsVisible(GameUtils.DinoSprite.Sprites["TEXT_SPRITE"]["H"]) && !GameOver;
        public int Score
        {
            get
            {
                if (Playing || GameOver)
                {
                    using (var scoreImage = _sightService.LookAt(getScoreRectangle()))
                    {
                        var ocrResult = _sightService.OCR(scoreImage, GameUtils.DinoSprite.Sprites["TEXT_SPRITE"].Sprites.ToDictionary(_ => _.Key, _ => _.Value.Image));
                        var str = string.Join("", ocrResult);
                        return int.Parse(str);
                    }
                }
                else
                {
                    return -1;
                }
            }
        }
        public void Prepare()
        {
            _sightService.ResetFocus();
            _logger.LogDebug("Grabbed screen.");
            var dinoLoc = _sightService.FindScreenPosition(GameUtils.DinoSprite["TREX"], .7);
            if (!dinoLoc.HasValue)
            {
                throw new Exception("Cannot find Dino :-(.");
            }
            _logger.LogDebug($"Found Dino at: {dinoLoc.Value}");
            var focus = new Rect(dinoLoc.Value.X - 10, dinoLoc.Value.Y - 97, 600, 150);
            _sightService.FocusOn(focus);
            _logger.LogDebug($"Focused on {focus}.");
            Mouse.MoveMouseSmooth(dinoLoc.Value.X, dinoLoc.Value.Y);
            _logger.LogDebug($"Moved mouse over Dino.");
            Mouse.MouseClick();
            _logger.LogDebug($"Clicked on Dino.");
        }

        public void PressUp()
        {
            Keyboard.KeyTap(Keys.Up);
        }
        public void PressDown()
        {
            Keyboard.KeyTap(Keys.Down);
        }

        public void Restart()
        {
            Keyboard.KeyTap(Keys.Space);
        }

        public double[] GrabScreen(Size resultSize = default(Size))
        {
            _sightService.GrabScreen();
            using (var playArea = _sightService.LookAt(playAreaRectangle)) //Crop Region of Interest(ROI)
            //using (var playArea = playAreaRaw.CvtColor(ColorConversionCodes.BGR2GRAY)) //RGB to Grey Scale
            {
                using (var playAreaOfDoubles = new MatOfDouble(playArea))
                {
                    double[] result;
                    if (resultSize != default(Size))
                    {
                        using (var playAreaResized = playArea.Resize(resultSize))
                        {
                            result = playAreaResized.GetArray(0, 0);
                        }
                    }
                    else
                    {
                        result = playAreaOfDoubles.GetArray(0, 0);
                    }
                    Cv2.ImWrite($"{Guid.NewGuid()}.png", playArea);
                    return result;
                }
            }
        }

        private Rect getScoreRectangle()
        {
            if (scoreArea == Rect.Empty)
            {
                lock (_lock)
                {
                    if (scoreArea == Rect.Empty)
                    {
                        var p = _sightService.Find(GameUtils.DinoSprite.Sprites["TEXT_SPRITE"]["H"]);
                        if (!p.HasValue)
                            throw new Exception("Unable to find score area.");
                        scoreArea = new Rect(p.Value + scoreAreaAdjustment, scoreAreaSize);
                    }
                }
            }
            return scoreArea;
        }
    }
}