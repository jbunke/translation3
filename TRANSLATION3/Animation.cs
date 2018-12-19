using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    public class Animation
    {
        private Permanence permanence;
        private Color color;
        private Point location;
        private int age;

        private Bitmap bitmap;

        public enum Permanence
        {
            PERMANENT,
            TEMPORARY
        }

        public Animation(Permanence permanence, Color color, Point location)
        {
            this.permanence = permanence;
            this.color = color;
            this.location = location;
            this.age = 0;

            if (permanence == Permanence.PERMANENT)
                generateBitmap();
        }

        public int getAge() { return age; }

        public Color getColor() { return color; }

        public Permanence getPermanence() { return permanence; }

        public Point getLocation() { return location; }

        public Bitmap getBitmap() { return bitmap; }

        public void older() { age++; }

        public void generateBitmap()
        {
            bitmap = new Bitmap(80, 80);
            for (int x = 0; x < 80; x += 4)
            {
                for (int y = 0; y < 80; y += 4)
                {
                    Random random = new Random(Guid.NewGuid().GetHashCode());
                    int seed = random.Next(0, 40);
                    bool draw = MathExt.Distance(new Point(x, y),
                        new Point(40, 40)) < seed;
                    if (draw)
                    {
                        Random col = new Random(Guid.NewGuid().GetHashCode());
                        int a = col.Next(50, 100);
                        int slide = col.Next(-50, 50);
                        int r = Math.Min(255, Math.Max(0, color.R + slide));
                        slide = col.Next(-50, 50);
                        int g = Math.Min(255, Math.Max(0, color.G + slide));
                        slide = col.Next(-50, 50);
                        int b = Math.Min(255, Math.Max(0, color.B + slide));

                        Graphics.FromImage(bitmap).FillRectangle(
                            new SolidBrush(Color.FromArgb(a, r, g, b)),
                            x, y, 4, 4);
                    }
                }
            }
        }

        public void setY(int y)
        {
            location.Y = y;
        }

        public void moveY(int inc)
        {
            location.Y += inc;
        }
    }
}
