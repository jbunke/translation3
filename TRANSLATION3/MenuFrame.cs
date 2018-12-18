using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRANSLATION3
{
    class MenuFrame
    {
        MenuObject[] objects;
        main main;
        bool[] selVector;
        bool[] lastSelVector;

        public enum Cause
        {
            MOUSE_MOVE,
            KEY_PRESS
        }

        public static MenuFrame fromString(String s, main main)
        {
            switch (s.ToLower())
            {
                case "pause":
                    MenuObject[] pauseObjs = new MenuObject[] {
                    new MenuObject(true, "RESUME", 4, new Point(640, 260),
                        MenuObject.Task.UNPAUSE, null, main),
                    new MenuObject(true, "SETTINGS", 4, new Point(640, 360),
                        MenuObject.Task.SET_PAUSE, "settings", main),
                    new MenuObject(true, "QUIT", 4, new Point(640, 460),
                        MenuObject.Task.CLOSE, null, main) };
                    return new MenuFrame(pauseObjs, main);
                default:
                    return new MenuFrame(new MenuObject[] { }, main);
            }
        }
        
        public MenuFrame(MenuObject[] objects, main main)
        {
            this.objects = objects;
            this.main = main;
            selVector = new bool[objects.Length];
            lastSelVector = new bool[objects.Length];

            bool assignedSel = false;

            for (int i = 0; i < objects.Length & !assignedSel; i++)
            {
                if (objects[i].canBeSelected())
                {
                    selVector[i] = true;
                    lastSelVector[i] = true; // just for init
                    objects[i].setSelect(true);
                    assignedSel = true;
                }
            }
        }

        public void actionHandler()
        {
            for (int i = 0; i < selVector.Length; i++)
            {
                if (selVector[i])
                {
                    objects[i].doTask();
                }
            }
        }

        public bool update(Cause cause, MouseEventArgs m, KeyEventArgs k)
        {
            // null m XOR k if the cause isn't relevant
            // TODO: selVector and object selection update for KEY_DOWN
            switch (cause)
            {
                case Cause.MOUSE_MOVE:
                    for (int i = 0; i < objects.Length; i++)
                    {
                        bool val = objects[i].isHovering(new Point(m.X, m.Y));
                        
                        selVector[i] = val;
                        objects[i].setSelect(val);
                    }
                    break;
                case Cause.KEY_PRESS:
                    // SET CONTROLS
                    Keys[] controls = main.getSettings().getControls();
                    Keys[] up = new Keys[] { controls[0], Keys.Up };
                    Keys[] down = new Keys[] { controls[1], Keys.Down };
                    Keys[] left = new Keys[] { controls[2], Keys.Left };
                    Keys[] right = new Keys[] { controls[3], Keys.Right };

                    // FIND SELECTED
                    int selected = -1;
                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (selVector[i])
                        {
                            selected = i;
                            break;
                        }
                    }

                    if (selected == -1)
                    {
                        bool assignedSel = false;

                        for (int i = 0; i < objects.Length; i++)
                        {
                            if (objects[i].canBeSelected() && !assignedSel)
                            {
                                selVector[i] = true;
                                lastSelVector[i] = true;
                                objects[i].setSelect(true);
                                assignedSel = true;
                            } else
                            {
                                selVector[i] = false;
                                lastSelVector[i] = false;
                                objects[i].setSelect(false);
                            }
                        }
                        return true;
                    }

                    int candIndex = selected;
                    double closestDistance = Double.MaxValue;

                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (i != selected)
                        {
                            Point old = objects[selected].getLocation();
                            Point cand = objects[i].getLocation();
                            bool direction = false;
                            // Principal direction
                            if (k.KeyCode == up[0] || k.KeyCode == up[1])
                            {
                                // UP
                                direction = cand.Y < old.Y &&
                                    Math.Abs(cand.Y - old.Y) >
                                    Math.Abs(cand.X - old.X);
                            } else if (k.KeyCode == down[0] ||
                                k.KeyCode == down[1])
                            {
                                // DOWN
                                direction = cand.Y > old.Y &&
                                    Math.Abs(cand.Y - old.Y) >
                                    Math.Abs(cand.X - old.X);
                            }
                            else if (k.KeyCode == left[0] ||
                              k.KeyCode == left[1])
                            {
                                // LEFT
                                direction = cand.X < old.X &&
                                    Math.Abs(cand.Y - old.Y) <
                                    Math.Abs(cand.X - old.X);
                            }
                            else if (k.KeyCode == right[0] ||
                              k.KeyCode == right[1])
                            {
                                // RIGHT
                                direction = cand.X > old.X &&
                                    Math.Abs(cand.Y - old.Y) <
                                    Math.Abs(cand.X - old.X);
                            }

                            if (direction &&
                                MathExt.Distance(old, cand) < closestDistance)
                            {
                                candIndex = i;
                                closestDistance = MathExt.Distance(old, cand);
                            }
                        }
                    }

                    if (candIndex != selected)
                    {
                        for (int i = 0; i < selVector.Length; i++)
                        {
                            selVector[i] = (i == candIndex);
                            objects[i].setSelect(i == candIndex);
                        }
                    }

                    break;
            }

            bool different = false;

            for (int i = 0; i < selVector.Length; i++)
            {
                different |= (selVector[i] != lastSelVector[i]);
                // Safe as index has already been compared
                lastSelVector[i] = selVector[i];
            }
            
            return different;
        }

        public Bitmap render()
        {
            Bitmap bitmap = new Bitmap(1280, 720);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // BACKGROUND
                g.DrawImage(Render.menuBackground(), 0, 0);

                // MENU OBJECTS
                foreach (MenuObject mob in objects)
                {
                    Bitmap oRender = mob.render();
                    Point p = mob.getLocation();

                    g.DrawImage(oRender, p.X - (oRender.Size.Width / 2),
                        p.Y - (oRender.Size.Height / 2));
                }
            }

            return bitmap;
        }
    }
}
