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
                    new MenuObject(false, "CAMERA FOLLOW MODE", 4, new Point(640, 180),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, stgs.getFollowMode().ToString(), 4, 
                        new Point(640, 220), MenuObject.Task.NULL, null, main),
                    new MenuObject(true, "<", 4,
                        new Point(520, 220), MenuObject.Task.SWITCH_FOLLOW, "-1", main),
                    new MenuObject(true, ">", 4,
                        new Point(760, 220), MenuObject.Task.SWITCH_FOLLOW, "1", main),
                    new MenuObject(false, "WINDOW MODE", 4, new Point(640, 300),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, stgs.getWindowMode().ToString(),
                        4, new Point(640, 340), MenuObject.Task.SET_PAUSE, null, main),
                    new MenuObject(true, "<", 4, new Point(480, 340),
                        MenuObject.Task.SWITCH_WINDOW, null, main),
                    new MenuObject(true, ">", 4, new Point(800, 340),
                        MenuObject.Task.SWITCH_WINDOW, null, main),
                    new MenuObject(false, "CONTROL MODE", 4, new Point(640, 420),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(true, stgs.getControlMode().ToString(),
                        4, new Point(640, 460), MenuObject.Task.SET_PAUSE,
                        "controls-pause", main),
                    new MenuObject(true, "<", 4, new Point(520, 460),
                        MenuObject.Task.SWITCH_CONTROLS, null, main),
                    new MenuObject(true, ">", 4, new Point(760, 460),
                        MenuObject.Task.SWITCH_CONTROLS, null, main),
                    new MenuObject(false, "GAME SPEED", 4, new Point(640, 540),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, stgs.gameSpeed(), 4,
                        new Point(640, 580), MenuObject.Task.NULL, null, main),
                    new MenuObject(true, "<", 4, new Point(520, 580),
                        MenuObject.Task.SWITCH_PERIOD, "1", main),
                    new MenuObject(true, ">", 4, new Point(760, 580),
                        MenuObject.Task.SWITCH_PERIOD, "-1", main) };
                    return new MenuFrame(pauseObjs, main);
                case "controls-pause":
                    Keys[] controls = main.getSettings().getControls();
                    int lc = 480;
                    int rc = 800;
                    pauseObjs = new MenuObject[] {
                    new MenuObject(false, "CONTROLS", 8, new Point(640, 100),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(true, "BACK", 4, new Point(640, 660),
                        MenuObject.Task.SET_PAUSE, "settings-pause", main),
                    new MenuObject(false, "JUMP", 4, new Point(lc, 200),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, controls[0].ToString(), 4,
                        new Point(rc, 200), MenuObject.Task.NULL, null, main),
                    new MenuObject(false, "DROP", 4, new Point(lc, 240),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, controls[1].ToString(), 4,
                        new Point(rc, 240), MenuObject.Task.NULL, null, main),
                    new MenuObject(false, "LEFT", 4, new Point(lc, 280),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, controls[2].ToString(), 4,
                        new Point(rc, 280), MenuObject.Task.NULL, null, main),
                    new MenuObject(false, "RIGHT", 4, new Point(lc, 320),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, controls[3].ToString(), 4,
                        new Point(rc, 320), MenuObject.Task.NULL, null, main),
                    new MenuObject(false, "SAVE LOCATION", 4, new Point(lc, 360),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, controls[4].ToString(), 4,
                        new Point(rc, 360), MenuObject.Task.NULL, null, main),
                    new MenuObject(false, "LOAD LOCATION", 4, new Point(lc, 400),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, controls[5].ToString(), 4,
                        new Point(rc, 400), MenuObject.Task.NULL, null, main),
                    new MenuObject(false, "TELEPORT", 4, new Point(lc, 440),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, controls[6].ToString(), 4,
                        new Point(rc, 440), MenuObject.Task.NULL, null, main),
                    new MenuObject(false, "CAMERA ZOOM", 4, new Point(lc, 480),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, controls[7].ToString(), 4,
                        new Point(rc, 480), MenuObject.Task.NULL, null, main),
                    new MenuObject(false, "PAUSE", 4, new Point(lc, 520),
                        MenuObject.Task.NULL, null, main),
                    new MenuObject(false, controls[8].ToString(), 4,
                        new Point(rc, 520), MenuObject.Task.NULL, null, main) };
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
                        int x = (int)(m.X * (1280 / (float)main.getSize().Width));
                        int y = (int)(m.Y * (720 / (float)main.getSize().Height));
                        bool val = objects[i].isHovering(new Point(x, y));
                        
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
