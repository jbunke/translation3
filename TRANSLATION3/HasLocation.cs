using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    public abstract class HasLocation
    {
        protected Point location;

        public Point getLocation()
        {
            return location;
        }
    }
}
