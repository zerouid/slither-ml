using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Slither.ML.Utils;

using ConvNetSharp.Volume.Double;
using ConvNetSharp.Volume;
using ConvNetSharp.Core.Training.Double;
using ConvNetSharp.Core;
using ConvNetSharp.Core.Serialization;
using ConvNetSharp.Core.Fluent;
using System.Collections.Generic;

namespace Slither.ML.Logic
{
    public class ModelTrainer
    {
        private static Random _random = new Random();
        private const string modelPath = "./objects/model.json";
        private readonly ILogger _logger;
        public ModelTrainer(ILogger<ModelTrainer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        private INet<double> BuildModel(GameModelConfig cfg)
        {
            _logger.LogInformation("Building the model.");
            var model = FluentNet<double>.Create(cfg.InputWidth, cfg.InputHeight, cfg.ImageChannels)
                .Conv(8, 8, 32).Stride(4).Pad(2)
                .Pool(2, 2)
                .Relu()
                .Conv(4, 4, 64).Stride(2).Pad(2)
                .Pool(2, 2)
                .Relu()
                .Conv(3, 3, 64).Stride(1).Pad(2)
                .Pool(2, 2)
                .Relu()
                .FullyConn(512)
                .Relu()
                .FullyConn(cfg.Actions)
                .Softmax(cfg.Actions)
                .Build();

            //create model file if not present
            if (!File.Exists(modelPath))
            {
                File.WriteAllText(modelPath, model.ToJson());
            }
            _logger.LogInformation("Finished bilding the model.");
            return model;
        }

        /// <summary>
        /// main training module
        /// </summary>
        /// <param name="cfg">Model configurations</param>
        /// <param name="gameState">Game State module with access to game environment and dino</param>
        /// <param name="observe">Flag to indicate wherther the model is to be trained(weight updates), else just play</param>
        public void TrainModel(GameModelConfig cfg, GameState gameState, bool justPlay = false)
        {
            CacheUtils.InitCache(
                ("epsilon", cfg.InitialEpsilon),
                ("time", 0),
                ("D", new Queue<(Volume<double>, int, double, Volume<double>, bool)>())); //initial variable caching, done only once

            var model = BuildModel(cfg);
            var lastTime = DateTime.Now;
            //store the previous observations in replay memory
            var D = CacheUtils.LoadObj<Queue<(Volume<double>, int, double, Volume<double>, bool)>>("D"); //load from file system
            // get the first state by doing nothing
            var do_nothing = new double[cfg.Actions];
            do_nothing[0] = 1; //0 => do nothing,
                               //1 => jump
            var (x_t, r_0, terminal) = gameState.GetState(do_nothing, cfg.InputWidth, cfg.InputHeight); //get next step after performing the action
            var s_t = BuilderInstance.Volume.From(x_t.Repeat(4), new Shape(cfg.InputWidth, cfg.InputHeight, cfg.ImageChannels));
            //s_t.ReShape(1, cfg.ImageRows, cfg.ImageCols, cfg.ImageChannels);
            var initial_state = x_t;
            double observe;
            double epsilon;


            model = SerializationExtensions.FromJson<double>(File.ReadAllText(modelPath));

            var trainer = new AdamTrainer(model) { LearningRate = cfg.LearningRate };

            if (justPlay)
            {
                observe = 999999999; //We keep observe, never train
                epsilon = cfg.FinalEpsilon;
            }
            else //We go to training mode
            {
                observe = cfg.Observation;
                epsilon = CacheUtils.LoadObj<double>("epsilon");
            }

            int t = CacheUtils.LoadObj<int>("time"); // resume from the previous time step stored in file system

            while (true) //endless running
            {
                double loss = 0;
                double Q_sa = 0;
                int action_index = 0;
                double r_t = 0; //reward at 4
                var a_t = new double[cfg.Actions]; //action at t

                //choose an action epsilon greedy
                if (t % cfg.FramePerAction == 0) //parameter to skip frames for actions
                {
                    if (_random.NextDouble() <= epsilon) //randomly explore an action
                    {
                        _logger.LogInformation("----------Random Action----------");
                        action_index = _random.Next(cfg.Actions);
                        a_t[action_index] = 1;
                    }
                    else //predict the output
                    {
                        model.Forward(s_t); //input a stack of 4 images, get the prediction
                        var q = model.GetPrediction();
                        action_index = q[0]; //chosing index with maximum q value
                        a_t[action_index] = 1; //o=> do nothing, 1=> jump
                    }
                }

                //We reduced the epsilon (exploration parameter) gradually
                if (epsilon > cfg.FinalEpsilon && t > observe)
                {
                    epsilon -= (cfg.InitialEpsilon - cfg.FinalEpsilon) / cfg.Explore;
                }

                //run the selected action and observed next state and reward
                double[] x_t1;
                (x_t1, r_t, terminal) = gameState.GetState(a_t, cfg.InputWidth, cfg.InputHeight);
                _logger.LogInformation($"fps: { 1 / (DateTime.Now - lastTime).TotalSeconds }"); //helpful for measuring frame rate
                lastTime = DateTime.Now;
                var s_t1 = BuilderInstance.Volume.From(s_t.ToArray().StackAndShift(x_t1), s_t.Shape); //append the new image to input stack and remove the first one


                //store the transition in D
                D.Enqueue((s_t, action_index, r_t, s_t1, terminal));
                if (D.Count > cfg.ReplayMemory)
                {
                    D.Dequeue();
                }

                //only train if done observing
                if (t > observe)
                {
                    //var minibatch 
                }
            }
        }
    }
}