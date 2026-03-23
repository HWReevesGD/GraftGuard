using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafter
{
    public static class GeometryHelper
    {
        public static PointF ControlToImage(PictureBox pb, Point mouseLoc)
        {
            if (pb.Image == null) return PointF.Empty;

            // Calculate the scale and offset created by Zoom mode
            float scale = Math.Min((float)pb.Width / pb.Image.Width, (float)pb.Height / pb.Image.Height);
            float ox = (pb.Width - pb.Image.Width * scale) / 2;
            float oy = (pb.Height - pb.Image.Height * scale) / 2;

            // Convert mouse position to relative 0.0 - 1.0 coordinates
            float rx = (mouseLoc.X - ox) / (pb.Image.Width * scale);
            float ry = (mouseLoc.Y - oy) / (pb.Image.Height * scale);

            return new PointF(Math.Clamp(rx, 0, 1), Math.Clamp(ry, 0, 1));
        }

        public static Point ImageToControl(PictureBox pb, float rx, float ry)
        {
            if (pb.Image == null) return Point.Empty;

            float scale = Math.Min((float)pb.Width / pb.Image.Width, (float)pb.Height / pb.Image.Height);
            float ox = (pb.Width - pb.Image.Width * scale) / 2;
            float oy = (pb.Height - pb.Image.Height * scale) / 2;

            return new Point(
                (int)(ox + rx * pb.Image.Width * scale),
                (int)(oy + ry * pb.Image.Height * scale)
            );
        }
    }
}
