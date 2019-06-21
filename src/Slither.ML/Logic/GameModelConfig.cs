namespace Slither.ML.Logic
{
    public class GameModelConfig
    {
        /// <summary>
        /// Possible actions
        /// </summary>
        /// <value>2 - jump, do nothing</value>
        public int Actions { get; set; } = 2;
        /// <summary>
        /// decay rate of past observations original 0.99
        /// </summary>
        public double Gamma { get; set; } = .99;
        /// <summary>
        /// timesteps to observe before training
        /// </summary>
        public double Observation { get; set; } = 100.0;
        /// <summary>
        /// frames over which to anneal epsilon
        /// </summary>
        /// <value></value>
        public int Explore { get; set; } = 100000;
        /// <summary>
        /// final value of epsilon
        /// </summary>
        public double FinalEpsilon { get; set; } = .0001;
        /// <summary>
        /// starting value of epsilon
        /// </summary>
        /// <value></value>
        public double InitialEpsilon { get; set; } = .1;
        /// <summary>
        /// number of previous transitions to remember
        /// </summary>
        /// <value></value>
        public int ReplayMemory { get; set; } = 50000;
        /// <summary>
        /// size of minibatch
        /// </summary>
        /// <value></value>
        public int Batch { get; set; } = 16;
        public int FramePerAction { get; set; } = 1;
        public double LearningRate { get; set; } = 1e-4;
        public int InputHeight { get; set; } = 80;
        public int InputWidth { get; set; } = 80;
        /// <summary>
        /// Number of frames in the input
        /// </summary>
        /// <value></value>
        public int ImageChannels { get; set; } = 4;
    }
}