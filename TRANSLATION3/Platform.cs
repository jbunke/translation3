using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    [Serializable] public class Platform : HasLocation
    {
        private int width;

        public Platform(Point location, int width)
        {
            this.location = location;
            this.width = width;
        }
        
        public int getWidth()
        {
            return width;
        }

        public void moveX(int inc)
        {
            location.X += inc;
        }

        public void moveY(int inc)
        {
            location.Y += inc;
        }

        public void changeWidth(int inc)
        {
            width += inc;
        }

        public void setY(int y)
        {
            location.Y = y;
        }
    }
}
