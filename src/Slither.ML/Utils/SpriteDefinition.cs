using System.Collections.Generic;

namespace Slither.ML.Utils
{
    public class SpriteDefinition
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IEnumerable<SpriteDefinition> Sprites { get; set; }

        /*
            CACTUS_LARGE: {x: 652, y: 2},
            CACTUS_SMALL: {x: 446, y: 2},
            CLOUD: {x: 166, y: 2},
            HORIZON: {x: 2, y: 104},
            MOON: {x: 954, y: 2},
            PTERODACTYL: {x: 260, y: 2},
            RESTART: {x: 2, y: 2},
            TEXT_SPRITE: {x: 1294, y: 2},
            TREX: {x: 1678, y: 2},
            STAR: {x: 1276, y: 2}
             */
        public static readonly SpriteDefinition DinoGameSpriteDef =
        new SpriteDefinition
        {
            Name = "MAIN",
            X = 0,
            Y = 0,
            Width = -1,
            Height = -1,
            Sprites = new[]{
                new SpriteDefinition { Name = "CACTUS_LARGE", X= 652, Y=2, Width=50, Height=100 },
                new SpriteDefinition { Name = "CACTUS_SMALL", X= 446, Y=2, Width=34, Height=70 },
                new SpriteDefinition { Name = "CLOUD", X= 166, Y=2, Width=92, Height=28 },
                new SpriteDefinition { Name = "HORIZON", X= 2, Y=104, Width=1200, Height=24 },
                new SpriteDefinition { Name = "MOON", X= 954, Y=2, Width=40, Height=80 },
                new SpriteDefinition { Name = "PTERODACTYL", X= 260, Y=2, Width=92, Height=80 },
                new SpriteDefinition { Name = "RESTART", X= 2, Y=2, Width=72, Height=64 },
                new SpriteDefinition { Name = "TREX", X= 1678, Y=2, Width=88, Height=94 },
                new SpriteDefinition { Name = "STAR", X= 1276, Y=2, Width=18, Height=18 },
                new SpriteDefinition { Name = "TEXT_SPRITE", X= 1294, Y=2, Width=382, Height=48,
                    Sprites = new []{
                        new SpriteDefinition { Name = "0", X= 0, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "1", X= 20, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "2", X= 40, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "3", X= 60, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "4", X= 80, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "5", X= 100, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "6", X= 120, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "7", X= 140, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "8", X= 160, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "9", X= 180, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "H", X= 200, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "I", X= 220, Y=0, Width=20, Height=26 },
                        new SpriteDefinition { Name = "GAMEOVER", X= 0, Y=26, Width=382, Height=22 }
                    }
                },
            }
        };
    }
}