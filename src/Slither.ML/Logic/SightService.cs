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
            screenSize = Screen.GetScreenSize();
            capture_width = screenSize.Width;
            capture_height = screenSize.Height;
            currentFrame = new Mat(new Size(capture_width, capture_height), MatType.CV_8U, Scalar.Black);
        }
        private readonly System.Drawing.Size screenSize;
        private Mat currentFrame;
        private int capture_x = 0;
        private int capture_y = 0;
        private int capture_width = -1;
        private int capture_height = -1;
        private double imageToScreenRatio = 0.0;
        public Point FocusOrigin { get; private set; } = new Point(0, 0);

        public byte[] GetScreenCaptureImage(string ext)
        {
            Cv2.ImEncode(ext, currentFrame, out byte[] image);
            return image;
        }

        public void GrabScreen()
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
                }
            }
        }

        public Point? FindScreenPosition(Mat template, double threshold)
        {
            return Find(template, threshold) * imageToScreenRatio;
        }
        public Point? Find(Mat template, double threshold)
        {
            using (var match = currentFrame.MatchTemplate(template, TemplateMatchModes.CCoeffNormed))
            {
                match.MinMaxLoc(out double minval, out double maxval, out Point minloc, out Point maxloc);
                _logger.LogDebug($"MinMaxLoc: minval = {minval}, maxval = {maxval}, minloc = {minloc}, maxloc={maxloc}");
                if (maxval > threshold)
                {
                    _logger.LogInformation($"Found template at: maxloc={maxloc}");
                    return maxloc;
                }
                return null;
            }
        }

        public bool IsVisible(Mat template, double threshold)
        {
            using (var match = currentFrame.MatchTemplate(template, TemplateMatchModes.CCoeffNormed))
            {
                match.MinMaxLoc(out double minval, out double maxval, out Point minloc, out Point maxloc);
                _logger.LogDebug($"MinMaxLoc: minval = {minval}, maxval = {maxval}, minloc = {minloc}, maxloc={maxloc}");
                var visible = maxval > threshold;
                _logger.LogInformation($"Template is {(visible ? "" : "NOT")}visible.");
                return visible;
            }
        }

        public IEnumerable<string> OCR(IDictionary<string, Mat> symbolMap)
        {
            return OCR(currentFrame, symbolMap);
        }

        public IEnumerable<string> OCR(Mat image, IDictionary<string, Mat> symbolMap)
        {
            var recognized = new List<(string str, int xpos)>();

            foreach (var (symbol, template) in symbolMap)
            {
                //obviously not a match
                if (template.Size().Width >= image.Size().Width || template.Size().Height >= image.Size().Height)
                {
                    continue;
                }

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
            return recognized.OrderBy(_ => _.xpos).Select(_ => _.str);
        }

        public void FocusOn(Rect rect)
        {
            Interlocked.Exchange(ref capture_x, rect.X);
            Interlocked.Exchange(ref capture_y, rect.Y);
            Interlocked.Exchange(ref capture_width, rect.Width);
            Interlocked.Exchange(ref capture_height, rect.Height);
            GrabScreen();
            FocusOrigin = rect.Location;
        }

        public void ResetFocus()
        {
            FocusOn(new Rect(0, 0, screenSize.Width, screenSize.Height));
        }

        public Mat LookAt(Rect rect)
        {
            return new Mat(currentFrame, rect);
        }
    }
}