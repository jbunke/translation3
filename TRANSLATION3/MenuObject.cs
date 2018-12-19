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
            CLOSE,
            SWITCH_FOLLOW,
            SWITCH_CONTROLS,
            SWITCH_WINDOW,
            SWITCH_PERIOD,
            NULL
        }

        public MenuObject(bool canSelect, String text, int size,
            Point location, Task task, String set, main main)
        {
            this.canSelect = canSelect;
            this.isSelected = false;
            this.location = location;
            this.main = main;

            image = Font.VIGILANT.print(text, size);

            if (canSelect)
            {
                selectedImage = Font.VIGILANT.print(
                    text, size, Color.FromArgb(255, 0, 0));
                this.task = task;
                this.set = set;
            } 
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
                case Task.SET_PAUSE:
                    main.setPauseFrame(set);
                    break;
                case Task.SWITCH_CONTROLS:
                    main.getSettings().switchControlMode();
                    List<Player> players = main.getLevel().getPlayers();
                    players[0].setControls(
                        main.getSettings().getControlMode());

                    if (players.Count > 1)
                    {
                        players[1].setControls(1 - 
                        main.getSettings().getControlMode());
                    }

                    main.refreshPauseFrame();
                    break;
                case Task.SWITCH_FOLLOW:
                    main.getSettings().switchFollowMode(Int32.Parse(set));
                    main.getLevel().setCamera(
                        main.getSettings().getFollowMode());
                    main.refreshPauseFrame();
                    break;
                case Task.SWITCH_PERIOD:
                    main.getSettings().switchPeriod(Int32.Parse(set));
                    main.applySettings();
                    main.refreshPauseFrame();
                    break;
                case Task.SWITCH_WINDOW:
                    main.getSettings().switchWindowMode();
                    main.applySettings();
                    main.refreshPauseFrame();
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
