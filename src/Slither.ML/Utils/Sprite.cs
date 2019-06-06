using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace Slither.ML.Utils
{
    public class Sprite : IDisposable
    {
        public Mat Image { get; private set; }
        public string Name { get; private set; }
        public IDictionary<string, Sprite> Sprites { get; private set; }
        public Mat this[string name]
        {
            get
            {
                if (Sprites != null && Sprites.TryGetValue(name, out Sprite sp))
                {
                    return sp.Image;
                }
                else
                {
                    return null;
                }
            }
        }
        public Sprite(Mat source, SpriteDefinition def)
        {
            Sprites = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
            var width = def.Width > 0 ? def.Width : source.Width - def.X;
            var height = def.Height > 0 ? def.Height : source.Height - def.Y;
            Image = new Mat(source, new Rect(def.X, def.Y, width, height));
            Name = def.Name;
            if (def.Sprites != null)
            {
                Sprites = def.Sprites.ToDictionary(_ => _.Name, _ => new Sprite(Image, _));
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Sprites != null)
                    {
                        foreach (var s in Sprites.Values)
                            s?.Dispose();
                    }
                    Image?.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}