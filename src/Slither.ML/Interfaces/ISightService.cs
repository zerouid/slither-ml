using System.Collections.Generic;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Slither.ML.Interfaces
{
    public interface ISightService
    {
        Mat GrabScreen();
        byte[] GetScreenCaptureImage(string ext);
        IEnumerable<string> OCR(IDictionary<string, Mat> symbolMap);
        IEnumerable<string> OCR(Mat image, IDictionary<string, Mat> symbolMap);
        void FocusOn(Rect rect);
        void ResetFocus();
        Mat LookAt(Rect rect);
        Point FocusOrigin { get; }
        bool IsVisible(Mat template, double threshold = 0.8);
        Point? FindScreenPosition(Mat template, double threshold = 0.8);
        Point? Find(Mat template, double threshold = 0.8);
    }
}