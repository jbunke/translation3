using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    class MenuObject
    {
        private bool canSelect;
        private bool isSelected;
        private Bitmap image;
        private Bitmap selectedImage;
        private Point location;
        private Task task;
        private String set;
        private main main;

        public enum Task
        {
            UNPAUSE,
            SET_PAUSE,
            SET_MENU,
            CLOSE
        }

        public MenuObject(bool canSelect, String text, int size,
            Point location, Task task, String set, main main)
        {
            this.canSelect = canSelect;
            this.isSelected = false;
            this.location = location;
            this.task = task;
            this.main = main;

            if (task == Task.SET_MENU || task == Task.SET_PAUSE)
                this.set = set;

            image = Font.VIGILANT().print(text, size);

            if (canSelect)
                selectedImage = Font.VIGILANT().print(
                    text, size, Color.FromArgb(255, 0, 0));
        }
        
        public void doTask()
        {
            switch (task)
            {
                case Task.UNPAUSE:
                    main.unpause();
                    break;
                case Task.CLOSE:
                    main.Close();
                    break;
            }
        }

        public Point getLocation()
        {
            return location;
        }

        public bool canBeSelected()
        {
            return canSelect;
        }

        public void setSelect(bool isSelected)
        {
            this.isSelected = isSelected;
        }

        public bool isHovering(Point mousePos)
        {
            if (!canSelect)
                return false;

            int x = selectedImage.Width / 2;
            int y = selectedImage.Height / 2;

            return Math.Abs(mousePos.X - location.X) <= x &&
                Math.Abs(mousePos.Y - location.Y) <= y;
        }

        public Bitmap render()
        {
            if (canSelect && isSelected)
            {
                return selectedImage;
            } else
            {
                return image;
            }
        }
    }
}
