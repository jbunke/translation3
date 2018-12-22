using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRANSLATION3
{
    public class EditorMover : HasLocation
    {
        private main main;
        private Keys[] controls;
        private EditorLevel editor;

        bool isLeft;
        bool isRight;
        bool isUp;
        bool isDown;

        public EditorMover(main main, EditorLevel editor)
        {
            this.main = main;
            this.location = new Point(1000, 950);
            this.controls = main.getSettings().getControls();
            this.editor = editor;
        }

        public void movement()
        {
            if (isLeft)
            {
                location.X -= 8;
            }

            if (isRight)
            {
                location.X += 8;
            }

            if (isDown)
            {
                location.Y += 8;
            }

            if (isUp)
            {
                location.Y -= 8;
            }
        }

        public void keyHandler(KeyEventArgs e, bool down)
        {
            if (e.KeyCode == controls[0])
            {
                isUp = down;
            }
            else if (e.KeyCode == controls[1])
            {
                isDown = down;
            }
            else if (e.KeyCode == controls[2])
            {
                isLeft = down;
            }
            else if (e.KeyCode == controls[3])
            {
                isRight = down;
            }

            if (!down)
            {
                // Zooming
                if (e.KeyCode == controls[7])
                {
                    editor.getCamera().switchZoom();
                } else if (e.KeyCode == controls[8])
                {
                    // TODO - temp fix
                    main.setMode(Mode.MENU);
                }
            }
        }
    }
}
