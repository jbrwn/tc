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
        public byte [] render(Envelope envelope, string format, int tileWidth, int tileHeight)
        {

            byte[] imgBytes;
            using (Image img = new Bitmap(tileWidth, tileHeight))
            {
                using (var font = new Font("Arial", 10))
                using (var stringFormat = new StringFormat())
                using (Brush fill = new SolidBrush(Color.LightBlue))
                using (var g = Graphics.FromImage(img))
                {
                    

                    var rect = new RectangleF(2, 2, tileWidth-4, tileHeight-4);
                    g.FillRectangle(fill, rect);
                    
                    //ul.x
                    stringFormat.LineAlignment = StringAlignment.Near;
                    stringFormat.Alignment = StringAlignment.Near;
                    g.DrawString(envelope.minx.ToString(), new Font("Arial", 10), SystemBrushes.WindowText, rect, stringFormat);
                   
                    //lr.y
                    stringFormat.LineAlignment = StringAlignment.Far;
                    stringFormat.Alignment = StringAlignment.Far;
                    g.DrawString(envelope.miny.ToString(), new Font("Arial", 10), SystemBrushes.WindowText,rect, stringFormat);

                    rect = new RectangleF(2, 20, tileWidth - 4, tileHeight - 40);
                    
                    //ul.y
                    stringFormat.LineAlignment = StringAlignment.Near;
                    stringFormat.Alignment = StringAlignment.Near;
                    g.DrawString(envelope.maxy.ToString(), new Font("Arial", 10), SystemBrushes.WindowText,rect, stringFormat);

                    //lr.x
                    stringFormat.LineAlignment = StringAlignment.Far;
                    stringFormat.Alignment = StringAlignment.Far;
                    g.DrawString(envelope.maxx.ToString(), new Font("Arial", 10), SystemBrushes.WindowText,rect, stringFormat);


                }
                
                
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    imgBytes = ms.ToArray();
                }

            }

            return imgBytes;

        }

        public List<string> getFormats()
        {
            return new List<string> {"png"};
        }

    }
}
