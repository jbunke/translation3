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
            MenuObject[] pauseObjs;
            switch (s.ToLower())
            {
                case "pause":
                    pauseObjs = new MenuObject[] {
                    new MenuObject(false, "PAUSED", 8, new Point(640, 100),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(true, "RESUME", 4, new Point(640, 260),
                        MenuObject.Task.UNPAUSE, null, main),
                    new MenuObject(true, "SETTINGS", 4, new Point(640, 360),
                        MenuObject.Task.SET_PAUSE, "settings-pause", main),
                    new MenuObject(true, "QUIT", 4, new Point(640, 460),
                        MenuObject.Task.SET_PAUSE, "are-you-sure", main) };
                    return new MenuFrame(pauseObjs, main);
                case "are-you-sure":
                    pauseObjs = new MenuObject[] {
                    new MenuObject(false, "Are you sure you want to quit?",
                        4, new Point(640, 310),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(true, "BACK", 4, new Point(540, 410),
                        MenuObject.Task.SET_PAUSE, "pause", main),
                    new MenuObject(true, "YES", 4, new Point(740, 410),
                        MenuObject.Task.CLOSE, null, main) };
                    return new MenuFrame(pauseObjs, main);
                case "settings-pause":
                    GameSettings stgs = main.getSettings();
                    pauseObjs = new MenuObject[] {
                    new MenuObject(false, "SETTINGS", 8, new Point(640, 100),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(true, "BACK", 4, new Point(640, 660),
                        MenuObject.Task.SET_PAUSE, "pause", main),
                    new MenuObject(false, "CAMERA FOLLOW MODE", 4, new Point(640, 240),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, stgs.getFollowMode().ToString(), 4, 
                        new Point(640, 280), MenuObject.Task.NULL, null, main),
                    new MenuObject(true, "<", 4,
                        new Point(480, 280), MenuObject.Task.SWITCH_FOLLOW, "-1", main),
                    new MenuObject(true, ">", 4,
                        new Point(800, 280), MenuObject.Task.SWITCH_FOLLOW, "1", main),
                    new MenuObject(false, "CONTROL MODE", 4, new Point(640, 360),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, stgs.getControlMode().ToString(), 4,
                        new Point(640, 400), MenuObject.Task.NULL, null, main),
                    new MenuObject(true, "<", 4, new Point(480, 400),
                        MenuObject.Task.SWITCH_CONTROLS, null, main),
                    new MenuObject(true, ">", 4, new Point(800, 400),
                        MenuObject.Task.SWITCH_CONTROLS, null, main) };
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
                        if (i != selected && objects[i].canBeSelected())
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
