using System;
using Slither.ML.Interfaces;

namespace Slither.ML.Logic
{
    public class DinoAgent
    {
        private readonly IGameController _game;
        public DinoAgent(IGameController game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
        }

        public bool IsRunning => _game.Playing;
        public bool IsCrashed => _game.GameOver;

        public void Jump()
        {
            _game.PressUp();
        }
        public void Duck()
        {
            _game.PressDown();
        }
    }
}