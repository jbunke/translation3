using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRANSLATION3
{
    public class EditorLevel : Level
    {
        private EditorMover mover;
        private Keys[] controls;

        private Color crossHairColor = Color.FromArgb(155, 155, 155);
        private HasLocation selected = null;
        private HasLocation selectable = null;
        private String selectionContext = "";
        private String[] selectionStats;
        private SelectionMode selectionMode = SelectionMode.NONE;

        // KEY STATUS
        private bool is4Down;
        private bool is5Down;
        private static readonly int UP = 0;
        private static readonly int DOWN = 1;
        private static readonly int LEFT = 2;
        private static readonly int RIGHT = 3;
        private bool[] arrowDown = new bool[] { false, false, false, false };

        private enum SelectionMode
        {
            SENTRY,
            REG_PLATFORM,
            STARTING_PLATFORM,
            CAN_ADD,
            NONE
        }

        public EditorLevel(main main) : base(main)
        {
            setCamera(Camera.FollowMode.GLUED);
            mover = new EditorMover(main, this);
            camera.setTarget(mover);
            controls = main.getSettings().getControls();
        }

        public static EditorLevel newEditor(main main)
        {
            return new EditorLevel(main);
        }

        // Public helpers
        public void addToName(String s)
        {
            this.name += s;
        }

        public void addToNote(String s) {
            this.note += s;
        }

        public void backspaceName() {
            if (name.Length > 0)
                name = name.Substring(0, name.Length - 1);
        }

        public void backspaceNote() {
            if (note.Length > 0)
                note = note.Substring(0, note.Length - 1);
        }

        private void hovering()
        {
            Point l = mover.getLocation();
            bool found = false;
            HasLocation hovered = null;

            // Player check
            if (Math.Abs(l.X - players.ElementAt(0).getLocation().X) <= 10 &&
                Math.Abs(l.Y - players.ElementAt(0).getLocation().Y) <= 10)
            {
                found = true;
                hovered = players.ElementAt(0);
            }

            // Platform check
            for (int i = 0; i < platforms.Count && !found; i++)
            {
                Point p = platforms.ElementAt(i).getLocation();
                int w = platforms.ElementAt(i).getWidth() / 2;

                if (Math.Abs(l.X - p.X) < w && Math.Abs(l.Y - p.Y) <= 10)
                {
                    hovered = platforms.ElementAt(i);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                foreach (Sentry s in sentries)
                {
                    if (Math.Abs(l.X - s.getLocation().X) <= 10 &&
                        Math.Abs(l.Y - s.getLocation().Y) <= 10)
                    {
                        hovered = s;
                        found = true;
                        break;
                    }
                }
            }

            if (found)
            {
                if (hovered is Player)
                {
                    crossHairColor = Color.FromArgb(255, 0, 0);
                    updateSelectionContext(
                        "<Player spawn is linked to starting platform>", 
                        new string[0]);
                } else
                {
                    crossHairColor = Color.FromArgb(0, 255, 0);
                    selectable = hovered;

                    if (hovered is Sentry)
                    {
                        Sentry s = (Sentry)hovered;
                        updateSelectionContext(
                            "<" + controls[6] + "> to select - SENTRY " +
                            (sentries.IndexOf(s) + 1), 
                            new string[] { "TYPE: " + Sentry.read(s.getType()),
                                "SPEED: " + s.getSpeed(),
                                "DIRECTION: " + s.interpretDirection(),
                                "SECONDARY TYPE: " +
                                Sentry.read(s.getSecondary()) });
                    }

                    if (hovered is Platform)
                    {
                        Platform p = (Platform)hovered;
                        int pIndex = platforms.IndexOf(p);

                        switch (pIndex)
                        {
                            case 0:
                                updateSelectionContext(
                                    "<" + controls[6] + 
                                    "> to select - STARTING PLATFORM", 
                                    new string[] { "WIDTH: " + p.getWidth() });
                                break;
                            default:
                                updateSelectionContext(
                                    "<" + controls[6] + 
                                    "> to select - PLATFORM " + (pIndex + 1),
                                    new string[] { "WIDTH: " + p.getWidth() });
                                break;
                        }
                    }
                }
            } else
            {
                selectable = null;
                updateSelectionContext(" ", new string[0]);
                crossHairColor = Color.FromArgb(155, 155, 155);
            }
        }

        private void updateSelectionContext(String message,
            String[] stats)
        {
            if (selected == null)
            {
                selectionContext = message;
                selectionStats = stats;
            }
        }

        private void generateHUDElements()
        {
            // SELECTION CONTEXT
            addHUDElement(new HUDElement(new Point(640, 50),
                HUDElement.Alignment.CENTER, 1, selectionContext,
                Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));

            // STATS
            for (int i = 0; i < selectionStats.Length; i++)
            {
                addHUDElement(new HUDElement(
                    new Point(1270, 380 + (i * 20)),
                    HUDElement.Alignment.RIGHT, 1, selectionStats[i],
                    Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
            }

            // ACTION ITEMS
            switch (selectionMode)
            {
                case SelectionMode.STARTING_PLATFORM:
                    addHUDElement(new HUDElement(
                        new Point(10, 380),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[4] + "> WIDEN",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 400),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[5] + "> COMPRESS",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 420),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[6] + "> DESELECT",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    break;
                case SelectionMode.REG_PLATFORM:
                    addHUDElement(new HUDElement(
                        new Point(10, 380),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[4] + "> WIDEN",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 400),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[5] + "> COMPRESS",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 420),
                        HUDElement.Alignment.LEFT, 1,
                        "<Arrows> MOVE",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 440),
                        HUDElement.Alignment.LEFT, 1,
                        "<X> ADD SENTRY",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 460),
                        HUDElement.Alignment.LEFT, 1,
                        "<Backspace> DELETE",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 480),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[6] + "> DESELECT",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    break;
                case SelectionMode.SENTRY:
                    addHUDElement(new HUDElement(
                        new Point(10, 380),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[4] + "> TYPE",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 400),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[5] + "> SECONDARY TYPE",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 420),
                        HUDElement.Alignment.LEFT, 1,
                        "<Left/Right> DIRECTION",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 440),
                        HUDElement.Alignment.LEFT, 1,
                        "<Up/Down> SPEED",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 460),
                        HUDElement.Alignment.LEFT, 1,
                        "<Backspace> DELETE",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    addHUDElement(new HUDElement(
                        new Point(10, 480),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[6] + "> DESELECT",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    break;
                case SelectionMode.CAN_ADD:
                    addHUDElement(new HUDElement(
                        new Point(10, 380),
                        HUDElement.Alignment.LEFT, 1,
                        "<" + controls[4] + "> ADD PLATFORM",
                        Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
                    break;
            }

            // TEST
            addHUDElement(new HUDElement(new Point(640, 670),
                HUDElement.Alignment.CENTER, 1, "<T> TEST",
                Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));

            // RESET
            addHUDElement(new HUDElement(new Point(960, 670),
                HUDElement.Alignment.CENTER, 1, "<R> RESET",
                Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));

            // Finish
            addHUDElement(new HUDElement(new Point(320, 670),
                HUDElement.Alignment.CENTER, 1, "<F> FINISH",
                Font.VIGILANT, 2, Color.FromArgb(255, 0, 0), false));
        }

        public new void update()
        {
            mover.movement();
            camera.follow();
            hovering();
            keyBehaviour();

            if (selected == null && crossHairColor == Color.FromArgb(155, 155, 155))
                selectionMode = SelectionMode.CAN_ADD;

            ageHUDElements();
            generateHUDElements();
        }

        private void keyBehaviour()
        {
            switch (selectionMode)
            {
                case SelectionMode.STARTING_PLATFORM:
                case SelectionMode.REG_PLATFORM:
                    Platform p = (Platform)selected;
                    if (is4Down && p.getWidth() < 500)
                        p.changeWidth(5);
                    if (is5Down && p.getWidth() > 25)
                        p.changeWidth(-5);

                    if (selectionMode == SelectionMode.REG_PLATFORM)
                    {
                        // Extras for regular platforms
                        if (MathExt.Distance(
                            players.ElementAt(0).getLocation(), 
                            p.getLocation()) < 20000)
                        {
                            if (arrowDown[UP])
                            {
                                p.moveY(-5);
                            }
                            if (arrowDown[DOWN])
                            {
                                p.moveY(5);
                            }
                            if (arrowDown[LEFT])
                            {
                                p.moveX(-5);
                            }
                            if (arrowDown[RIGHT])
                            {
                                p.moveX(5);
                            }

                            foreach (Sentry s in sentries)
                            {
                                s.editorAdjust();
                            }
                        }
                    }
                    break;
            }
            updateSelectionStats();
        }

        public new Bitmap render()
        {
            Bitmap render = new Bitmap(1280, 720);

            using (Graphics g = Graphics.FromImage(render))
            {
                // Background
                g.DrawImage(background, 0, 0);

                // SETUP
                Point c = camera.getLocation();
                Point o = new Point(640 - c.X, 360 - c.Y);
                bool z = camera.isZoomedOut();
                int d = 1;
                if (z)
                    d = 2;

                // Platforms
                foreach (Platform platform in platforms)
                {
                    if (platform == selected)
                    {
                        g.FillRectangle(new SolidBrush(
                            Color.FromArgb(0, 255, 0)),
                        640 + (((o.X + platform.getLocation().X -
                        (int)(platform.getWidth() / 2)) - 640) / d) - 5,
                        360 + (((o.Y + platform.getLocation().Y - 10) - 360) / d) - 5,
                        (platform.getWidth() / d) + 10, (20 / d) + 10);
                    }

                    g.DrawImage(Render.platform(platform),
                        640 + (((o.X + platform.getLocation().X - (int)(platform.getWidth() / 2)) - 640) / d),
                        360 + (((o.Y + platform.getLocation().Y - 10) - 360) / d),
                        platform.getWidth() / d, 20 / d);
                }

                // Sentries
                foreach (Sentry sentry in sentries)
                {
                    Point s = sentry.getLocation();

                    if (sentry == selected)
                    {
                        g.FillRectangle(new SolidBrush(
                            Color.FromArgb(0, 255, 0)),
                        640 + (((o.X + s.X - 10) - 640) / d) - 5,
                        360 + (((o.Y + s.Y - 10) - 360) / d) - 5, 
                        (20 / d) + 10, (20 / d) + 10);
                    }

                    g.DrawImage(Render.sentry(sentry),
                        640 + (((o.X + s.X - 10) - 640) / d),
                        360 + (((o.Y + s.Y - 10) - 360) / d), 20 / d, 20 / d);
                }

                // Player(s)
                foreach (Player player in players)
                {
                    Point l = player.getLocation();
                    
                    // location
                    g.DrawImage(Render.player(),
                        640 + (((o.X + l.X - 10) - 640) / d),
                        360 + (((o.Y + l.Y - 10) - 360) / d), 20 / d, 20 / d);
                }

                // Crosshair
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    639, 0, 2, 720);
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    0, 359, 1280, 2);
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    539, 340, 2, 40);
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    439, 340, 2, 40);
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    739, 340, 2, 40);
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    839, 340, 2, 40);
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    620, 159, 40, 2);
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    620, 259, 40, 2);
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    620, 459, 40, 2);
                g.FillRectangle(new SolidBrush(
                    crossHairColor),
                    620, 559, 40, 2);

                // HUD Elements
                foreach (HUDElement element in elements)
                {
                    Bitmap h = element.draw();
                    Point hSpot = element.getLocation();

                    switch (element.alignment)
                    {
                        case HUDElement.Alignment.CENTER:
                            switch (element.cameraDep)
                            {
                                case false:
                                    g.DrawImage(h, hSpot.X - (h.Width / 2),
                                hSpot.Y - (h.Height / 2));
                                    break;
                                case true:
                                default:
                                    int x = 640 + ((o.X + hSpot.X -
                                        (h.Width / 2) - 640) / d);
                                    int y = 360 + ((o.Y + hSpot.Y -
                                        (h.Height / 2) - 360) / d);
                                    g.DrawImage(h, x, y);
                                    break;
                            }
                            break;
                        case HUDElement.Alignment.LEFT:
                            g.DrawImage(h, hSpot.X,
                                hSpot.Y - (h.Height / 2));
                            break;
                        case HUDElement.Alignment.RIGHT:
                            g.DrawImage(h, hSpot.X - h.Width,
                                hSpot.Y - (h.Height / 2));
                            break;
                        case HUDElement.Alignment.LEFT_TOP:
                            g.DrawImage(h, hSpot.X, hSpot.Y);
                            break;
                        case HUDElement.Alignment.RIGHT_TOP:
                            g.DrawImage(h, hSpot.X - h.Width, hSpot.Y);
                            break;
                    }
                }
            }

            return render;
        }

        public new void keyHandler(KeyEventArgs e, bool down)
        {
            // Key status
            if (e.KeyCode == controls[4])
            {
                is4Down = down;
            } else if (e.KeyCode == controls[5])
            {
                is5Down = down;
            } else if (e.KeyCode == Keys.Up)
            {
                arrowDown[UP] = down;
            } else if (e.KeyCode == Keys.Down)
            {
                arrowDown[DOWN] = down;
            } else if (e.KeyCode == Keys.Left)
            {
                arrowDown[LEFT] = down;
            } else if (e.KeyCode == Keys.Right)
            {
                arrowDown[RIGHT] = down;
            }

            // TODO
            if (!down)
            {
                // Test
                if (e.KeyCode == Keys.T)
                {
                    main.playEditorLevel();
                } else if (e.KeyCode == Keys.R)
                {
                    main.resetEditor();
                } else if (e.KeyCode == Keys.F)
                {
                    main.setMode(Mode.MENU);
                    main.setMenuFrame("editor-level-finish");
                }

                switch (selectionMode)
                {
                    case SelectionMode.CAN_ADD:
                        if (e.KeyCode == controls[4])
                            platforms.Add(new Platform(
                                mover.getLocation(), 200));
                        break;
                    case SelectionMode.REG_PLATFORM:
                        Platform p = (Platform)selected;
                        if (e.KeyCode == Keys.Back)
                        {
                            // Delete platform
                            platforms.Remove(p);

                            for (int i = 0; i < sentries.Count; i++)
                            {
                                Sentry sentry = sentries.ElementAt(i);
                                if (sentry.getPlatform() == p)
                                {
                                    sentries.Remove(sentry);
                                    i--;
                                }
                            }

                            selected = null;
                            selectionMode = SelectionMode.NONE;
                        } else if (e.KeyCode == Keys.X)
                        {
                            // Add sentry
                            Sentry sentry = new Sentry(Sentry.Type.RANDOM,
                                6, Sentry.Type.RANDOM);
                            sentries.Add(sentry);
                            sentry.setPlatform(p);
                        }
                        break;
                    case SelectionMode.SENTRY:
                        Sentry s = (Sentry)selected;
                        if (e.KeyCode == controls[4])
                        {
                            // Next type - cycles through the enum
                            s.nextType();
                        } else if (e.KeyCode == controls[5])
                        {
                            // Next secondary
                            s.nextSecondary();
                        } else if (e.KeyCode == Keys.Left)
                        {
                            s.setDirection(-1);
                        } else if (e.KeyCode == Keys.Right)
                        {
                            s.setDirection(1);
                        } else if (e.KeyCode == Keys.Up && s.getSpeed() < 14)
                        {
                            s.changeSpeed(2);
                        } else if (e.KeyCode == Keys.Down && s.getSpeed() > 2)
                        {
                            s.changeSpeed(-2);
                        } else if (e.KeyCode == Keys.Back)
                        {
                            sentries.Remove(s);

                            selected = null;
                            selectionMode = SelectionMode.NONE;
                        }
                        break;
                }
            }

            if (down)
            {
                if (e.KeyCode == controls[6])
                {
                    if (selectable != null)
                    {
                        selected = selectable;
                        selectable = null;

                        if (selected is Sentry)
                        {
                            selectionMode = SelectionMode.SENTRY;
                            selectionContext = "SENTRY " +
                                (sentries.IndexOf((Sentry)selected) + 1);
                        } else if (selected is Platform)
                        {
                            Platform p = (Platform)selected;

                            if (platforms.IndexOf(p) == 0)
                            {
                                selectionMode =
                                    SelectionMode.STARTING_PLATFORM;
                                selectionContext = "STARTING PLATFORM";
                            } else
                            {
                                selectionMode = SelectionMode.REG_PLATFORM;
                                selectionContext = "PLATFORM " +
                                    (platforms.IndexOf(p) + 1);
                            }
                        }
                    } else if (selected != null)
                    {
                        selected = null;
                        selectionMode = SelectionMode.NONE;
                    }
                }
            }

            if (selected != null)
                updateSelectionStats();

            mover.keyHandler(e, down);
        }

        private void updateSelectionStats()
        {
            if (selected is Sentry)
            {
                Sentry s = (Sentry)selected;

                selectionStats = new string[] { "TYPE: " + Sentry.read(s.getType()),
                    "SPEED: " + s.getSpeed(),
                    "DIRECTION: " + s.interpretDirection(),
                    "SECONDARY TYPE: " + Sentry.read(s.getSecondary()) };
            } else if (selected is Platform)
            {
                Platform p = (Platform)selected;

                selectionStats = new string[] { "WIDTH: " + p.getWidth() };
            }
        }
    }
}
