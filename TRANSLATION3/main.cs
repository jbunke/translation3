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
            pauseFrame = MenuFrame.fromString("pause", this);

            // APPLY SETTINGS
            gameSettings = GameSettings.defaultSettings();
            applySettings();

            // CAMPAIGN TEST
            campaign = new List<String>();

            for (int i = 0; i < 16; i++)
            {
                campaign.Add("classic" + i);
            }
            // campaign.Add("behemoth");
            campaign.Add("Take Flight");
            campaign.Add("staircase1");
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

        private void applySettings()
        {
            // TIMER PERIOD / UPDATE FREQUENCY
            tmr.Interval = gameSettings.getPeriod();
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

            canvas = pauseFrame.render();
            pictureBox1.Image = canvas;
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

        public GameSettings getSettings()
        {
            return gameSettings;
        }
    }
}
