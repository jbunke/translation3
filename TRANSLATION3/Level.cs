using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRANSLATION3
{
    class Level
    {
        private List<Platform> platforms = new List<Platform>();
        private List<Player> players = new List<Player>();
        private List<Sentry> sentries = new List<Sentry>();
        private Camera camera;
        private Bitmap background;

        private main main;
        private bool finished = false;
        private int countdown = 10;

        public Level(Player[] players, Platform[] platforms, 
            Sentry[] sentries, int[] key, Camera.FollowMode followMode, 
            main main)
        {
            Debug.Assert(sentries.Length == key.Length);

            // use arrays arg for cleaner constructor calls
            foreach (Platform platform in platforms)
            {
                this.platforms.Add(platform);
            }

            foreach (Player player in players)
            {
                player.setLevel(this);
                this.players.Add(player);
            }

            for (int i = 0; i < sentries.Length; i++)
            {
                Debug.Assert(key[i] != 0); // 0 (starting platform) reserved for player
                sentries[i].setPlatform(platforms[key[i]]);
                sentries[i].setLevel(this);
                this.sentries.Add(sentries[i]);
            }

            this.camera = new Camera(followMode);
            camera.setTarget(players[0]);

            this.main = main;

            this.background = Render.initialBackground();
        }

        public void update()
        {
            foreach (Player player in players)
            {
                player.movement();
            }

            for (int i = 0; i < sentries.Count; i++)
            {
                if (sentries.ElementAt(i).isAlive())
                {
                    sentries.ElementAt(i).patrol();
                    sentries.ElementAt(i).behave();
                }
            }

            camera.follow();

            if (outOfWorld())
            {
                main.generateLevel();
            }

            if (!finished)
            {
                finished = hasFinished();
            } else
            {
                countdown--;
                if (countdown == 0)
                {
                    main.levelComplete();
                    main.generateLevel();
                }
            }
        }

        public Bitmap render()
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
                    g.DrawImage(Render.platform(platform),
                        640 + (((o.X + platform.getLocation().X - (int)(platform.getWidth() / 2)) - 640) / d),
                        360 + (((o.Y + platform.getLocation().Y - 10) - 360) / d),
                        platform.getWidth() / d, 20 / d);
                }

                // Sentries
                foreach (Sentry sentry in sentries)
                {
                    Point s = sentry.getLocation();

                    if (sentry.isAlive() && sentry.sightDependent())
                    {
                        int a = 50;

                        if (sentry.seesPlayer() != null)
                            a += 100;

                        if (sentry.getDirection() == -1)
                        {
                            g.FillRectangle(new SolidBrush(
                                Color.FromArgb(a,
                                Render.sentryColor(sentry))),
                                0, 360 + (((o.Y + s.Y - 7) - 360) / d), 
                                640 + (((o.X + s.X - 10) - 640) / d), 14 / d);
                        } else
                        {
                            g.FillRectangle(new SolidBrush(
                                Color.FromArgb(a,
                                Render.sentryColor(sentry))),
                                640 + (((o.X + s.X + 10) - 640) / d),
                                360 + (((o.Y + s.Y - 7) - 360) / d),
                                1280 - (640 + (((o.X + s.X - 10) - 640) / d)), 14 / d);
                        }
                    } else if (sentry.isAlive())
                    {
                        g.FillEllipse(new SolidBrush(
                            Color.FromArgb(100,
                            Render.sentryColor(sentry))),
                            640 + (((o.X + s.X - 20) - 640) / d),
                            360 + (((o.Y + s.Y - 20) - 360) / d), 40 / d, 40 / d);
                    }

                    g.DrawImage(Render.sentry(sentry),
                        640 + (((o.X + s.X - 10) - 640) / d),
                        360 + (((o.Y + s.Y - 10) - 360) / d), 20 / d, 20 / d);

                    if (sentry.getType() == Sentry.Type.SPAWN && sentry.isAlive())
                    {
                        if (sentry.getCount() % 10 >= 7)
                        {
                            Sentry pretend = new Sentry(sentry.getSecondary(), 0);
                            g.DrawImage(Render.sentry(pretend),
                            640 + (((o.X + s.X - 10) - 640) / d),
                            360 + (((o.Y + s.Y - 10) - 360) / d));
                        }
                    }
                }

                // Player(s)
                foreach (Player player in players)
                {
                    Point l = player.getLocation();
                    Point sl = player.getSaveLocation();

                    // saved location
                    g.FillRectangle(new SolidBrush(Color.FromArgb(100, 155, 0, 0)),
                        640 + (((o.X + sl.X - 5) - 640) / d),
                        360 + (((o.Y + sl.Y - 5) - 360) / d), 10 / d, 10 / d);

                    // teleportation shadow
                    if (player.getTelePhase() > 0)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(100, 255, 0, 0)),
                            640 + (((o.X + l.X - 5 + (5 * player.getDirection() *
                                player.getTelePhase() * player.getSpeed())) - 640) / d),
                            360 + (((o.Y + l.Y - 5) - 360) / d), 10 / d, 10 / d);
                    }

                    // location
                    g.DrawImage(Render.player(),
                        640 + (((o.X + l.X - 10) - 640) / d),
                        360 + (((o.Y + l.Y - 10) - 360) / d), 20 / d, 20 / d);
                }
            }

            return render;
        }

        private bool hasFinished()
        {
            bool finished = true;
            foreach (Sentry sentry in sentries)
            {
                finished &= !sentry.isAlive();
            }

            return finished;
        }

        private bool outOfWorld()
        {
            int u = Int32.MinValue;
            int l = Int32.MaxValue;

            foreach (Platform platform in platforms)
            {
                u = Math.Max(platform.getLocation().Y, u);
                l = Math.Min(platform.getLocation().Y, l);
            }

            foreach (Player player in players)
            {
                int p = player.getLocation().Y;

                if (p < u + 1000 && p > l - 1000)
                    return false;
            }
            return true;
        }

        public void keyHandler(KeyEventArgs e, bool down)
        {
            foreach (Player player in players)
            {
                player.keyHandler(e, down);
            }
        }

        public Platform startingPlatform()
        {
            Debug.Assert(platforms.Count >= 1);
            return platforms.ElementAt(0);
        }

        public List<Platform> getPlatforms()
        {
            return platforms;
        }

        public List<Sentry> getSentries()
        {
            return sentries;
        }

        public void addSentry(Sentry sentry)
        {
            sentries.Add(sentry);
            sentry.setLevel(this);
        }

        public List<Player> getPlayers()
        {
            return players;
        }

        public Camera getCamera()
        {
            return camera;
        }
    }
}
