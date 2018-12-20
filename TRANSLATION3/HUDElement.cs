using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    public class HUDElement
    {
        public readonly Point location;
        public readonly Alignment alignment;
        private int age; // -1 for permanent
        private Bitmap bitmap;

        public enum Alignment
        {
            LEFT,
            RIGHT,
            CENTER
        }

        public HUDElement(Point location, Alignment alignment, int lifespan, Bitmap bitmap)
        {
            this.location = location;
            this.alignment = alignment;
            this.age = lifespan;
            this.bitmap = bitmap;
        }

        public HUDElement(Point location, Alignment alignment, int lifespan,
            String text, Font font, int size, Color color)
        {
            this.location = location;
            this.alignment = alignment;
            this.age = lifespan;
            this.bitmap = font.print(text, size, color);
        }

        public void older()
        {
            age--;
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
