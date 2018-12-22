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

        public Animation(Permanence permanence, Color color, Point location, int flag)
        {
            this.permanence = permanence;
            this.color = color;
            this.location = location;
            this.age = 0;

            if (permanence == Permanence.PERMANENT)
                generateBitmap(flag);
        }

        public int getAge() { return age; }

        public Color getColor() { return color; }

        public Permanence getPermanence() { return permanence; }

        public Point getLocation() { return location; }

        public Bitmap getBitmap() { return bitmap; }

        public void older() { age++; }

        public void generateBitmap(int flag)
        {
            int s = Math.Max(40, flag);
            bitmap = new Bitmap(s * 2, s * 2);
            for (int x = 0; x < s * 2; x += 4)
            {
                for (int y = 0; y < s + 20; y += 4)
                {
                    Random random = new Random(Guid.NewGuid().GetHashCode());
                    int seed = random.Next(0, s);
                    bool draw = MathExt.Distance(new Point(x, y),
                        new Point(s, s)) < seed;
                    if (draw)
                    {
                        Random col = new Random(Guid.NewGuid().GetHashCode());
                        int a = col.Next(50, 100);
                        int slide = col.Next(-50, 50);
                        int r = MathExt.Bounded(0, color.R + slide, 255);
                        slide = col.Next(-50, 50);
                        int g = MathExt.Bounded(0, color.G + slide, 255);
                        slide = col.Next(-50, 50);
                        int b = MathExt.Bounded(0, color.B + slide, 255);

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
