using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    class CineFrame
    {
        private int age;
        private Bitmap bitmap;
        
        public CineFrame(int lifespan, Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.age = lifespan;
        }

        public int getAge()
        {
            return age;
        }

        public void older() { age--; }

        public Bitmap draw()
        {
            return bitmap;
        }
    }
}
