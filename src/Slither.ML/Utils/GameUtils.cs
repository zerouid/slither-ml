using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Robot.NET;
using Slither.ML.Utils;

namespace Slither.ML
{
    public static class GameUtils
    {
        public static Mat CurrentFrame;

        public static void SaveObj(object obj, string name)
        {
            var formatter = new BinaryFormatter();
            using (var fs = new FileStream($"objects/{name}.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fs, obj);
            }
        }
        public static T LoadObj<T>(object obj, string name)
        {
            var formatter = new BinaryFormatter();
            using (var fs = new FileStream($"objects/{name}.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                return (T)formatter.Deserialize(fs);
            }
        }

        private const string dinoSprite = @"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";
        private static Sprite _dinoSprite;

        public static Sprite DinoSprite
        {
            get
            {
                if (_dinoSprite == null)
                {
                    var data = Convert.FromBase64String(dinoSprite);
                    var spriteSrc = Cv2.ImDecode(data, ImreadModes.AnyColor).CvtColor(ColorConversionCodes.BGR2GRAY);
                    _dinoSprite = new Sprite(spriteSrc, SpriteDefinition.DinoGameSpriteDef);
                }
                return _dinoSprite;
            }

        }

    }
}