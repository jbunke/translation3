using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TRANSLATION3.Properties;

namespace TRANSLATION3
{
    public partial class main : Form
    {
        private Bitmap canvas = new Bitmap(1280, 720);

        private Mode mode = Mode.GAME;
        private GameSettings gameSettings = GameSettings.defaultSettings();
        private int playerc = 1;

        // CINEMATIC
        private Cinematic cinematic;

        // PAUSE
        private String pauseScreen;
        private MenuFrame pauseFrame;

        // MENU
        private String menuScreen;
        private MenuFrame menuFrame;

        // EDITOR
        private EditorLevel editor;

        // GAME
        private String levelId;
        private Level level;
        private List<String> campaign;
        private int campaignIndex = 0;

        public main()
        {
            InitializeComponent();
        }

        /* EVENTS */

        private void main_Load(object sender, EventArgs e)
        {
            // TECHNICALS
            this.Location = new Point(10, 10);
            this.Cursor = new Cursor(Resources.mousepointer.Handle);

            // INITIAL MENU
            menuScreen = "main";

            // LEVEL EDITOR
            editor = EditorLevel.newEditor(this);

            // CINEMATIC
            mode = Mode.CINEMATIC;
            cinematic = Cinematic.fromString("startup", this);

            // APPLY SETTINGS
            gameSettings = GameSettings.defaultSettings();
            applySettings();

            // CAMPAIGN TEST
            campaign = new List<String>();

            //campaign.Add("main20");
            //campaign.Add("main34");
            //campaign.Add("main35");
            //campaign.Add("main36");
            //campaign.Add("main37");
            //campaign.Add("main38");
            //campaign.Add("main50");
            //campaign.Add("main75");
            //campaign.Add("main76");
            //campaign.Add("main77");
            //campaign.Add("main74");
            for (int i = 0; i < 16; i++)
            {
                campaign.Add("main" + i);
                //campaign.Add("classic" + i);
                //campaign.Add("classic" + (i + 8));
            }
            //campaign.Add("behemoth");
            generateLevel(true);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            switch (mode)
            {
                case Mode.PAUSE:
                    pauseFrame.actionHandler();
                    break;
                case Mode.MENU:
                    menuFrame.actionHandler();
                    break;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            switch (mode)
            {
                case Mode.PAUSE:
                    if (pauseFrame.update(MenuFrame.Cause.MOUSE_MOVE, e, null))
                    {
                        canvas = pauseFrame.render();
                    }
                    break;
                case Mode.MENU:
                    if (menuFrame.update(MenuFrame.Cause.MOUSE_MOVE, e, null))
                    {
                        canvas = menuFrame.render();
                    }
                    break;
            }
            pictureBox1.Image = canvas;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = new Cursor(Resources.mousepointer.Handle);
        }

        private void main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (mode)
            {
                case Mode.GAME:
                    level.keyHandler(e, true);
                    break;
                case Mode.EDITOR:
                    editor.keyHandler(e, true);
                    break;
            }
        }

        private void main_KeyUp(object sender, KeyEventArgs e)
        {
            switch (mode)
            {
                case Mode.GAME:
                    level.keyHandler(e, false);
                    break;
                case Mode.EDITOR:
                    editor.keyHandler(e, false);
                    break;
                case Mode.PAUSE:
                    if (e.KeyCode == gameSettings.getControls()[8])
                    {
                        unpause();
                    }
                    else if (e.KeyCode == gameSettings.getControls()[6] ||
                        e.KeyCode == Keys.Enter)
                    {
                        pauseFrame.actionHandler();
                    }
                    else
                    {
                        if (pauseFrame.update(MenuFrame.Cause.KEY_PRESS, null, e))
                        {
                            canvas = pauseFrame.render();
                            pictureBox1.Image = canvas;
                        }
                        break;
                    }
                    break;
                case Mode.MENU:
                    if (e.KeyCode == gameSettings.getControls()[6] ||
                        e.KeyCode == Keys.Enter)
                    {
                        menuFrame.actionHandler();
                    }
                    else
                    {
                        if (menuFrame.update(MenuFrame.Cause.KEY_PRESS, null, e))
                        {
                            canvas = menuFrame.render();
                            pictureBox1.Image = canvas;
                        }
                        break;
                    }
                    break;
            }
        }

        private void tmr_Tick(object sender, EventArgs e)
        {
            switch (mode)
            {
                case Mode.GAME:
                    level.update();
                    canvas = level.render();
                    pictureBox1.Image = canvas;
                    break;
                case Mode.EDITOR:
                    editor.update();
                    canvas = editor.render();
                    pictureBox1.Image = canvas;
                    break;
                case Mode.CINEMATIC:
                    if (cinematic.update())
                    {
                        canvas = cinematic.draw();
                        pictureBox1.Image = canvas;
                    } else
                    {
                        cinematic.link();
                    }
                    break;
            }
        }

        public void applySettings()
        {
            // TIMER PERIOD / UPDATE FREQUENCY
            tmr.Interval = gameSettings.getPeriod();

            // WINDOW MODE
            if (gameSettings.getWindowMode() == GameSettings.WindowMode.FULLSCREEN)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.MinimumSize = new Size(0, 0);
                this.MaximumSize = new Size(0, 0);
                this.Location = new Point(0, 0);
                this.Bounds = Screen.PrimaryScreen.Bounds;

                pictureBox1.Size = Screen.PrimaryScreen.Bounds.Size;
            } else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.MinimumSize = new Size(1296, 759);
                this.MaximumSize = new Size(1296, 759);
                this.Location = new Point(10, 10);

                pictureBox1.Size = new Size(1280, 720);
            }
        }

        public void playEditorLevel()
        {
            campaignIndex = 0;
            campaign = new List<string> { "editor" };
            levelId = campaign.ElementAt(campaignIndex);
            level = Level.fromEditor(editor, this);
            level.startUpHUD();
            setMode(Mode.GAME);
        }

        public void generateLevel(bool startup)
        {
            if (levelId == "editor")
            {
                playEditorLevel();
            } else
            {
                levelId = campaign.ElementAt(campaignIndex);
                level = SavedLevels.fetchLevel(levelId, playerc,
                    gameSettings.getFollowMode(), this);

                if (startup)
                {
                    level.startUpHUD();
                }
            }
        }

        public void levelComplete()
        {
            if (campaignIndex + 1 < campaign.Count)
                campaignIndex++;
            // TODO: go right to next level for now; until transitions / cinematics
            if (levelId == "editor")
            {
                setMode(Mode.EDITOR);
            } else
            {
                generateLevel(true);
            }
        }

        public void pause()
        {
            this.mode = Mode.PAUSE;

            setPauseFrame("pause");
        }

        public void unpause()
        {
            this.mode = Mode.GAME;
        }
        
        public Level getLevel()
        {
            return level;
        }

        public GameSettings getSettings()
        {
            return gameSettings;
        }

        public Size getSize()
        {
            return pictureBox1.Size;
        }

        public Mode getMode()
        {
            return mode;
        }

        public void setMode(Mode mode)
        {
            this.mode = mode;

            switch (mode)
            {
                case Mode.MENU:
                    setMenuFrame(menuScreen);
                    break;
                case Mode.PAUSE:
                    setPauseFrame(pauseScreen);
                    break;
            }
        }

        public void setMenuFrame(String s)
        {
            menuScreen = s;
            refreshMenuFrame();
        }

        public void refreshMenuFrame()
        {
            menuFrame = MenuFrame.fromString(menuScreen, this);

            canvas = menuFrame.render();
            pictureBox1.Image = canvas;
        }

        public void setPauseFrame(String s)
        {
            pauseScreen = s;
            refreshPauseFrame();
        }

        public void refreshPauseFrame()
        {
            pauseFrame = MenuFrame.fromString(pauseScreen, this);

            canvas = pauseFrame.render();
            pictureBox1.Image = canvas;
        }
    }
}
