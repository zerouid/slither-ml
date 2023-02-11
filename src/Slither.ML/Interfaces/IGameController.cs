using OpenCvSharp;

namespace Slither.ML.Interfaces
{
    public interface IGameController
    {
        void Prepare();
        void Restart();
        double[] GrabScreen(Size resultSize = default(Size));
        void PressUp();
        void PressDown();
        int Score { get; }
        bool GameOver { get; }
        bool Playing { get; }

    }
}