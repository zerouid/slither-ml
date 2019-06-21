using System;

namespace Slither.ML.Logic
{
    public class DinoAgent
    {
        private readonly Game _game;
        public DinoAgent(Game game)
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