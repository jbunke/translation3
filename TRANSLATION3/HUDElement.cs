using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    public class HUDElement : HasLocation
    {
        private Point frameMove;
        public readonly Alignment alignment;
        public readonly bool cameraDep;
        private int age; // -1 for permanent
        private Bitmap bitmap;

        public enum Alignment
        {
            LEFT,
            LEFT_TOP,
            RIGHT,
            RIGHT_TOP,
            CENTER
        }

        public HUDElement(Point location, Alignment alignment, int lifespan, Bitmap bitmap, 
            bool cameraDep)
        {
            this.location = location;
            this.alignment = alignment;
            this.age = lifespan;
            this.bitmap = bitmap;
            this.frameMove = new Point(0, 0);
            this.cameraDep = cameraDep;
        }

        public HUDElement(Point location, Alignment alignment, int lifespan,
            String text, Font font, int size, Color color, bool cameraDep)
        {
            this.location = location;
            this.alignment = alignment;
            this.age = lifespan;
            this.bitmap = font.print(text, size, color);
            this.frameMove = new Point(0, 0);
            this.cameraDep = cameraDep;
        }

        public HUDElement(Point location, Alignment alignment, int lifespan,
            String text, Font font, int size, Color color, Point frameMove, 
            bool cameraDep)
        {
            this.location = location;
            this.alignment = alignment;
            this.age = lifespan;
            this.bitmap = font.print(text, size, color);
            this.frameMove = frameMove;
            this.cameraDep = cameraDep;
        }

        public void older()
        {
            age--;

            location.X += frameMove.X;
            location.Y += frameMove.Y;
        }

        public int getAge()
        {
            return age;
        }
        
        public Bitmap draw()
        {
            return bitmap;
        }
    }
}
