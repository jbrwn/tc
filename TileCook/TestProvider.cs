using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace TileCook
{
    [DataContract]
    public class TestProvider : IProvider
    {
        private byte[] imgBytes;
        
        public byte [] render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            if (imgBytes == null)
            {
                using (Bitmap img = new Bitmap(tileWidth, tileHeight))
                {
                    using (Brush background = new SolidBrush(Color.White))
                    using (Pen border = new Pen(new SolidBrush(Color.FromArgb(75, 255, 0, 0)), 50))
                    using (var g = Graphics.FromImage(img))
                    {
                        var rect = new Rectangle(0, 0, tileWidth, tileHeight);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.FillRectangle(background, rect);
                        g.DrawRectangle(border, rect);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, ImageFormat.Png);
                        this.imgBytes = ms.ToArray();
                    }
                }
            }
            return this.imgBytes;
        }

        public List<string> getFormats()
        {
            return new List<string> {"png"};
        }
    }
}
