using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenCvSharp;

namespace Slither.ML.Logic
{
    public class GameState
    {
        private const string loss_file_path = "./objects/loss.csv";
        private const string actions_file_path = "./objects/actions.csv";
        private const string q_value_file_path = "./objects/q_values.csv";
        private const string scores_file_path = "./objects/scores.csv";

        private readonly IDictionary<string, List<double>> _logs = new Dictionary<string, List<double>> {
            {loss_file_path, new List<double>()},
            {actions_file_path, new List<double>()},
            {q_value_file_path, new List<double>()},
            {scores_file_path, new List<double>()},
        };

        private readonly DinoAgent _agent;
        private readonly Game _game;
        public List<double> Loss => _logs[loss_file_path];
        public List<double> Scores => _logs[scores_file_path];
        public List<double> Actions => _logs[actions_file_path];
        public List<double> QValues => _logs[q_value_file_path];
        public GameState(DinoAgent agent, Game game)
        {
            _agent = agent ?? throw new ArgumentNullException(nameof(agent));
            _game = game ?? throw new ArgumentNullException(nameof(game));
        }
        public (double[] image, double reward, bool isOver) GetState(double[] actions, int width, int height)
        {
            Actions.Add(actions[1]); //storing actions in a dataframe
            var score = _game.Score;
            var reward = .1;
            var isOver = false;
            if (actions[1] == 1.0)
            {
                _agent.Jump();
            }
            var image = _game.GrabScreen(new Size(width, height));
            if (_agent.IsCrashed)
            {
                Scores.Add(score);
                _game.Restart();
                reward = -1;
                isOver = true;
            }
            return (image, reward, isOver);
        }

        public void InitLogs()
        {
            foreach (var (file, list) in _logs)
            {
                list.Clear();
                if (File.Exists(file))
                {
                    var vals = File.ReadAllLines(file).Select(_ => double.Parse(_));
                    list.AddRange(vals);
                }
            }
        }

        public void InitCache()
        {

        }
    }
}