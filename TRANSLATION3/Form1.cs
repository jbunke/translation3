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
        private Font font = new Font("VIGILANT",
            Resources.uppercase_vigilant_2_boldface,
            Resources.lowercase_vigilant_2_boldface,
            Resources.numbers_vigilant_2_boldface,
            Resources.symbols_vigilant_2_boldface);

        private Mode mode = Mode.GAME;
        private String levelId = "behemoth";
        private int playerc = 1;
        private Level level;

        private List<String> campaign =
            new List<String>() { "classic0", "classic1", "classic2",
                "classic3", "classic4", "classic5", "Take Flight" };
        private int campaignIndex = 0;

        public main()
        {
            InitializeComponent();
        }

        /* EVENTS */

        private void main_Load(object sender, EventArgs e)
        {
            this.Location = new Point(10, 10);

            // CAMPAIGN TEST
            campaign = new List<String>();

            for (int i = 0; i < 16; i++)
            {
                campaign.Add("classic" + i);
            }
            campaign.Add("main50");

            generateLevel();
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

        public void generateLevel()
        {
            levelId = campaign.ElementAt(campaignIndex);
            level = SavedLevels.fetchLevel(levelId, playerc, this);
        }

        public void levelComplete()
        {
            if (campaignIndex + 1 < campaign.Count)
                campaignIndex++;
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
    }
}
