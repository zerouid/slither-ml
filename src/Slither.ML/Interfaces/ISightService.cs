using System.Collections.Generic;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Slither.ML.Interfaces
{
    public interface ISightService
    {
        Task<Mat> GrabScreen();
        Task<byte[]> GetScreenCaptureImage(string ext);
        Task<IEnumerable<string>> OCR(IDictionary<string, Mat> symbolMap);
        Task<IEnumerable<string>> OCR(Mat image, IDictionary<string, Mat> symbolMap);
        Task FocusOn(Rect rect);
        Task<Mat> LookAt(Rect rect);
        Point FocusOrigin { get; }
        Task<bool> IsVisible(Mat template, double threshold = 0.8);
        Task<Point?> Find(Mat template, double threshold = 0.8);
    }
}