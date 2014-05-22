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
    public class TestProvider : IEnvelopeProvider
    {   
        public byte [] render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {
            using (Bitmap img = new Bitmap(tileWidth, tileHeight))
            {
                using (Brush background = new SolidBrush(Color.White))
                using (Pen border = new Pen(new SolidBrush(Color.FromArgb(75, 255, 0, 0)), 50))
                using (Font text = new Font("arial", 10))
                using (Brush textColor = new SolidBrush(Color.Black))
                using (var g = Graphics.FromImage(img))
                {
                    var rect = new Rectangle(0, 0, tileWidth, tileHeight);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillRectangle(background, rect);
                    g.DrawRectangle(border, rect);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("minx: " + envelope.minx.ToString());
                    sb.AppendLine("miny: " + envelope.miny.ToString());
                    sb.AppendLine("maxx: " + envelope.maxx.ToString());
                    sb.AppendLine("maxy: " + envelope.maxy.ToString());
                    g.DrawString(sb.ToString(),text ,textColor , new PointF(30, 30));
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        public List<string> getFormats()
        {
            return new List<string> {"png"};
        }
    }
}
