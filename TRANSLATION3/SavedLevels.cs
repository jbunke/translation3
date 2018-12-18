using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    class SavedLevels
    {
        public static Level fetchLevel(String id, int playerc,
            Camera.FollowMode followMode, main main)
        {
            Debug.Assert(playerc == 1 || playerc == 2);

            Player[] players;
            Platform[] platforms = new Platform[] { new Platform(new Point(1000, 1000), 200) };
            Sentry[] sentries = new Sentry[] { };
            int[] key = new int[] { };

            if (playerc == 2)
            {
                players = new Player[] { new Player(main.getSettings().getControlMode()),
                    new Player((GameSettings.ControlMode)(1 - 
                    (int)main.getSettings().getControlMode())) };
            } else
            {
                players = new Player[] { new Player(main.getSettings().getControlMode()) };
            }
            
            switch (id)
            {
                case "staircase1":
                    platforms = new Platform[] {
                        new Platform(new Point(1000, 1000), 200),
                        new Platform(new Point(800, 900), 200),
                        new Platform(new Point(1200, 800), 200),
                        new Platform(new Point(800, 700), 200),
                        new Platform(new Point(1200, 600), 200),
                        new Platform(new Point(800, 500), 200) };
                    sentries = new Sentry[] {
                        new Sentry(Sentry.Type.PUSH, 10),
                        new Sentry(Sentry.Type.SPAWN, -10),
                        new Sentry(Sentry.Type.RANDOM, 14)};
                    key = new int[] { 1, 2, 4 };
                    break;
                case "behemoth":
                    platforms = new Platform[] {
                        new Platform(new Point(1100, 1250), 100),
                        new Platform(new Point(700, 1150), 200),
                        new Platform(new Point(1150, 1075), 100),
                        new Platform(new Point(1000, 1000), 150),
                        new Platform(new Point(650, 1000), 200),
                        new Platform(new Point(1350, 1000), 150),
                        new Platform(new Point(850, 925), 100),
                        new Platform(new Point(1300, 850), 200),
                        new Platform(new Point(900, 750), 100) };
                    sentries = new Sentry[] {
                        new Sentry(Sentry.Type.GRAV_DOUBLE, 10),
                        new Sentry(Sentry.Type.MOVE, 14),
                        new Sentry(Sentry.Type.GRAV_INC, 10),
                        new Sentry(Sentry.Type.GRAV_RED, -10),
                        new Sentry(Sentry.Type.GRAV_FLIP, -10),
                        new Sentry(Sentry.Type.PULL, 12)};
                    key = new int[] { 1, 3, 4, 5, 7, 8 };
                    break;
                case "Take Flight":
                    platforms = new Platform[] {
                        new Platform(new Point(1100, 1250), 100),
                        new Platform(new Point(700, 1150), 200),
                        new Platform(new Point(2000, 1000), 150) };
                    sentries = new Sentry[] {
                        new Sentry(Sentry.Type.GRAV_DOUBLE, 10),
                        new Sentry(Sentry.Type.DECAY, 14)};
                    key = new int[] { 1, 2 };
                    break;
                default:
                    String file = "../../Resources/" + id + ".txt";
                    return readFromFile(file, players, followMode, main);
            }
            return new Level(players, platforms, sentries, key, followMode, main);
        }

        public static Level readFromFile(String file, Player[] players,
            Camera.FollowMode followMode, main main)
        {
            string[] lines = File.ReadAllLines(file);
            int platformc = Int32.Parse(
                lines[0].Substring(0, lines[0].IndexOf(" ")));
            int sentryc = Int32.Parse(
                lines[0].Substring(lines[0].IndexOf(" ") + 1));
            Platform[] platforms = new Platform[platformc];
            Sentry[] sentries = new Sentry[sentryc];
            int[] key = new int[sentryc];

            int l = 2;

            for (int i = 0; i < platformc; i++)
            {
                string line = lines[l + i];

                int x = Int32.Parse(line.Substring(0, line.IndexOf(" ")));
                line = line.Substring(line.IndexOf(" ") + 1);
                int y = Int32.Parse(line.Substring(0, line.IndexOf(" ")));
                line = line.Substring(line.IndexOf(" ") + 1);
                int w = Int32.Parse(line);
                platforms[i] = new Platform(new Point(x, y), w);
            }

            l += platformc + 1;

            for (int i = 0; i < sentryc; i++)
            {
                string line = lines[l + i];

                Enum.TryParse(line.Substring(0, line.IndexOf(" ")),
                    out Sentry.Type type);
                line = line.Substring(line.IndexOf(" ") + 1);
                int speed = Int32.Parse(line.Substring(0, line.IndexOf(" ")));
                line = line.Substring(line.IndexOf(" ") + 1);
                Enum.TryParse(line.Substring(0, line.IndexOf(" ")),
                    out Sentry.Type secondary);

                if (type == Sentry.Type.SPAWN)
                {
                    sentries[i] = new Sentry(type, speed, secondary);
                } else
                {
                    sentries[i] = new Sentry(type, speed);
                }

                line = line.Substring(line.IndexOf(" ") + 1);
                
                key[i] = Int32.Parse(line);
            }
                
            return new Level(players, platforms, sentries,
                key, followMode, main);
        }
    }
}
