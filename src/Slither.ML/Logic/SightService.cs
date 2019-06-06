using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Robot.NET;
using Slither.ML.Interfaces;

namespace Slither.ML.Logic
{
    public class SightService : ISightService
    {
        private readonly ILogger _logger;
        public SightService(ILogger<SightService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var screenSize = Screen.GetScreenSize();
            capture_width = screenSize.Width;
            capture_height = screenSize.Height;
        }
        private Mat currentFrame;
        private int capture_x = 0;
        private int capture_y = 0;
        private int capture_width = -1;
        private int capture_height = -1;
        private double imageToScreenRatio = 0.0;
        public Point FocusOrigin { get; private set; } = new Point(0, 0);

        public async Task<byte[]> GetScreenCaptureImage(string ext)
        {
            var current = currentFrame ?? await GrabScreen();
            Cv2.ImEncode(ext, currentFrame, out byte[] image);
            return image;
        }

        public async Task<Mat> GrabScreen()
        {
            return await Task.Run(() =>
            {
                using (var bmp = Screen.CaptureScreen(capture_x, capture_y, capture_width, capture_height))
                {
                    if (imageToScreenRatio == 0.0)
                    {
                        imageToScreenRatio = (double)capture_width / bmp.Width;
                    }
                    using (var mat = bmp.ToMat())
                    {
                        var graymat = mat.CvtColor(ColorConversionCodes.BGR2GRAY);
                        Interlocked.Exchange(ref currentFrame, graymat)?.Dispose();
                        return currentFrame;
                    }
                }
            });
        }

        public async Task<Point?> Find(Mat template, double threshold)
        {
            var current = currentFrame ?? await GrabScreen();
            using (var match = current.MatchTemplate(template, TemplateMatchModes.CCoeffNormed))
            {
                match.MinMaxLoc(out double minval, out double maxval, out Point minloc, out Point maxloc);
                _logger.LogDebug($"MinMaxLoc: minval = {minval}, maxval = {maxval}, minloc = {minloc}, maxloc={maxloc}");
                if (maxval > threshold)
                {
                    _logger.LogInformation($"Found template at: maxloc={maxloc}");
                    return maxloc * imageToScreenRatio;
                }
                return null;
            }
        }

        public async Task<bool> IsVisible(Mat template, double threshold)
        {
            var current = currentFrame ?? await GrabScreen();
            using (var match = current.MatchTemplate(template, TemplateMatchModes.CCoeffNormed))
            {
                match.MinMaxLoc(out double minval, out double maxval, out Point minloc, out Point maxloc);
                _logger.LogDebug($"MinMaxLoc: minval = {minval}, maxval = {maxval}, minloc = {minloc}, maxloc={maxloc}");
                var visible = maxval > threshold;
                _logger.LogInformation($"Template is {(visible ? "" : "NOT")}visible.");
                return visible;
            }
        }

        public async Task<IEnumerable<string>> OCR(IDictionary<string, Mat> symbolMap)
        {
            var current = currentFrame ?? await GrabScreen();
            return await OCR(current, symbolMap);
        }

        public Task<IEnumerable<string>> OCR(Mat image, IDictionary<string, Mat> symbolMap)
        {
            var recognized = new List<(string str, int xpos)>();

            foreach (var (symbol, template) in symbolMap)
            {

                using (var m = image.MatchTemplate(template, TemplateMatchModes.CCoeffNormed))
                using (var mt = m.Threshold(0.9, 1.0, ThresholdTypes.Tozero))
                {

                    while (true)
                    {
                        mt.MinMaxLoc(out double minVal, out double maxVal, out Point minloc, out Point maxloc);
                        if (maxVal >= 0.9)
                        {
                            _logger.LogDebug($"Found \'{symbol}\' at {maxloc} -> {maxVal}");
                            recognized.Add((symbol, maxloc.X));
                            mt.FloodFill(maxloc, new Scalar(0), out Rect r, new Scalar(.1), new Scalar(1.0));
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
            return Task.FromResult(recognized.OrderBy(_ => _.xpos).Select(_ => _.str));
        }

        public async Task FocusOn(Rect rect)
        {
            Interlocked.Exchange(ref capture_x, rect.X);
            Interlocked.Exchange(ref capture_y, rect.Y);
            Interlocked.Exchange(ref capture_width, rect.Width);
            Interlocked.Exchange(ref capture_height, rect.Height);
            await GrabScreen();
            FocusOrigin = rect.Location;
        }

        public async Task<Mat> LookAt(Rect rect)
        {
            var current = currentFrame ?? await GrabScreen();
            return new Mat(current, rect);
        }
    }
}