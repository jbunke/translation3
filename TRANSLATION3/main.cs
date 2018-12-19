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

namespace TRANSLATION3
{
    public partial class main : Form
    {
        private Bitmap canvas = new Bitmap(1280, 720);

        private Mode mode = Mode.GAME;
        private GameSettings gameSettings = GameSettings.defaultSettings();
        private int playerc = 1;

        // TODO: pause
        private String pauseScreen;
        private MenuFrame pauseFrame;

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
            this.Location = new Point(10, 10);

            // PAUSE CONTEXT
            pauseScreen = "pause";
            pauseFrame = MenuFrame.fromString(pauseScreen, this);

            // APPLY SETTINGS
            gameSettings = GameSettings.defaultSettings();
            applySettings();

            // CAMPAIGN TEST
            campaign = new List<String>();

            campaign.Add("test");
            for (int i = 0; i < 8; i++)
            {
                campaign.Add("main" + i);
                campaign.Add("classic" + i);
                campaign.Add("classic" + (i + 8));
            }
            campaign.Add("main20");
            campaign.Add("main34");
            campaign.Add("main35");
            campaign.Add("main36");
            campaign.Add("main37");
            campaign.Add("main38");
            campaign.Add("main50");

            generateLevel();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            switch (mode)
            {
                case Mode.PAUSE:
                    pauseFrame.actionHandler();
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
            }
            pictureBox1.Image = canvas;
        }

        private void main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (mode)
            {
                case Mode.GAME:
                    Debug.Assert(level != null);
                    level.keyHandler(e, true);
                    break;
            }
        }

        private void main_KeyUp(object sender, KeyEventArgs e)
        {
            switch (mode)
            {
                case Mode.GAME:
                    Debug.Assert(level != null);
                    level.keyHandler(e, false);
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
            }
        }

        private void tmr_Tick(object sender, EventArgs e)
        {
            switch (mode)
            {
                case Mode.GAME:
                    update();
                    canvas = render();
                    pictureBox1.Image = canvas;
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

        public void generateLevel()
        {
            levelId = campaign.ElementAt(campaignIndex);
            level = SavedLevels.fetchLevel(levelId, playerc,
                gameSettings.getFollowMode(), this);
        }

        public void levelComplete()
        {
            if (campaignIndex + 1 < campaign.Count)
                campaignIndex++;
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

        private void update()
        {
            level.update();
        }

        private Bitmap render()
        {
            // render() is for GAME mode only
            return level.render();
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
