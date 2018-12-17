using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    class SavedLevels
    {
        public static Level fetchLevel(String id, int playerc, main main)
        {
            Debug.Assert(playerc == 1 || playerc == 2);

            Player[] players;
            Platform[] platforms = new Platform[] { new Platform(new Point(1000, 1000), 200) };
            Sentry[] sentries = new Sentry[] { };
            int[] key = new int[] { };

            if (playerc == 2)
            {
                players = new Player[] { new Player(), new Player("NUMPAD") };
            } else
            {
                players = new Player[] { new Player() };
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
            }

            return new Level(players, platforms, sentries, key, Camera.FollowMode.STEADY, main);
        }
    }
}
