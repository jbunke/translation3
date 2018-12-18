using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    public class Sentry
    {
        private static int NUM_TYPES = 16;

        private Type type;
        private Platform platform;
        private Level level;
        private Point location;
        private int direction;
        private int speed;
        private bool alive;

        // SPECIAL
        private Type secondary;
        private int count;
        private List<Sentry> children;

        public enum Type
        {
            PUSH, // 0, 0, 255
            PULL, // 175, 0, 175
            HORZ_MAGNET, // 255, 100, 255
            GRAV_FLIP, // 127, 255, 127
            GRAV_DOUBLE, // 255, 127, 127
            SHOVE, // 0, 0, 127
            DROP, // 120, 0, 0
            MOVE, // 0, 255, 0
            DECAY, // 255, 255, 0
            FLEE, // 255, 255, 255
            SPREAD, // 0, 151, 0
            SPAWN, // 150, 100, 0
            GRAV_RED,
            GRAV_INC,
            GOD, // 150, 0, 150
            RANDOM // 20, 20, 20
        }

        public Sentry(Type type, int speed)
        {
            this.type = type;
            direction = Math.Sign(speed);
            this.speed = Math.Abs(speed);
            this.alive = true;

            if (type == Type.SPAWN)
            {
                this.secondary = Type.RANDOM;
                this.count = 0;
                this.children = new List<Sentry>();
            }
        }

        // SPAWN CONSTRUCTOR
        public Sentry(Type type, int speed, Type secondary)
        {
            this.type = type;
            direction = Math.Sign(speed);
            this.speed = Math.Abs(speed);
            this.alive = true;

            this.secondary = secondary;
            this.count = 0;
            this.children = new List<Sentry>();
        }

        public void setLevel(Level level)
        {
            this.level = level;
        }

        public void setPlatform(Platform platform)
        {
            this.platform = platform;
            this.location = new Point(platform.getLocation().X,
                platform.getLocation().Y - 20);
        }

        public Point getLocation()
        {
            return location;
        }

        public int getDirection()
        {
            return direction;
        }

        public bool isAlive()
        {
            return alive;
        }

        public Type getType()
        {
            return type;
        }

        public Type getSecondary()
        {
            return secondary;
        }

        public int getCount()
        {
            return count;
        }

        public void crush()
        {
            alive = false;
        }

        public bool sightDependent()
        {
            switch (type)
            {
                case Type.GRAV_INC:
                case Type.GRAV_RED:
                case Type.HORZ_MAGNET:
                case Type.SPAWN:
                case Type.RANDOM:
                    return false;
                default:
                    return true;
            }
        }

        public void patrol()
        {
            Debug.Assert(alive); // must be alive to be patrolling

            if (Math.Abs(location.X - platform.getLocation().X) < (platform.getWidth() / 2) - 10 ||
                ((location.X - platform.getLocation().X) / direction) < 0)
            {
                location.X += (speed * direction);
            } else
            {
                direction = direction * -1;
            }

            location.Y = platform.getLocation().Y - 20;
        }

        public void behave()
        {
            Player sees = null;
            List<Platform> platforms = level.getPlatforms();

            if (sightDependent())
            {
                // SIGHT-DEPENDENT
                sees = seesPlayer();

                if (sees != null)
                {
                    switch (type)
                    {
                        case Type.PUSH:
                            sees.moveX(speed * direction);
                            break;
                        case Type.SHOVE:
                            sees.moveX(4 * speed * direction);
                            break;
                        case Type.PULL:
                            sees.moveX(-speed * direction);
                            break;
                        case Type.DROP:
                            sees.moveY(21);
                            break;
                        case Type.GRAV_FLIP:
                            sees.mulGAcceleration(-1);
                            break;
                        case Type.GRAV_DOUBLE:
                            sees.mulGAcceleration(2);
                            break;
                        case Type.MOVE:
                            platform.moveX(-(speed * direction));
                            break;
                        case Type.DECAY:
                            if (platform.getWidth() > 20)
                                platform.changeWidth(-speed);
                            break;
                        case Type.SPREAD:
                            if (platform.getWidth() < 500)
                                platform.changeWidth(speed);
                            break;
                        case Type.FLEE:
                            int i = 0;
                            while (i == 0 || platforms.ElementAt(i) == platform)
                            {
                                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                                i = rnd.Next(1, platforms.Count);
                            }
                            setPlatform(platforms.ElementAt(i));
                            break;
                        case Type.GOD:
                            int seesY = sees.getLocation().Y;

                            List<Player> players = level.getPlayers();
                            List<Sentry> sentries = level.getSentries();
                            foreach (Player p in players)
                            {
                                p.setY(-1 * p.getLocation().Y);
                                p.setSY(-1 * p.getSaveLocation().Y);
                                p.mulGAcceleration(-1);
                            }

                            foreach (Platform p in platforms)
                            {
                                p.setY(-1 * p.getLocation().Y);
                            }

                            foreach (Sentry s in sentries)
                            {
                                s.location.Y *= -1;
                                s.location.Y -= 40;
                            }

                            int o = seesY - sees.getLocation().Y;

                            foreach (Player p in players)
                            {
                                p.moveY(o);
                                p.moveSY(o);
                            }

                            foreach (Platform p in platforms)
                            {
                                p.moveY(o);
                            }

                            foreach (Sentry s in sentries)
                            {
                                s.location.Y += o;
                            }

                            break;
                        default:
                            // TODO
                            break;
                    }
                }
            } else
            {
                // TODO: NON- SIGHT-DEPENDENT
                List<Player> players = level.getPlayers();

                switch (type)
                {
                    case Type.GRAV_INC:
                        foreach (Player player in players)
                        {
                            player.changeGAcceleration(-3);
                        }
                        break;
                    case Type.GRAV_RED:
                        foreach (Player player in players)
                        {
                            player.changeGAcceleration(3);
                        }
                        break;
                    case Type.HORZ_MAGNET:
                        foreach (Player player in players)
                        {
                            player.moveX(speed * Math.Sign(location.X -
                                player.getLocation().X));
                        }
                        break;
                    case Type.SPAWN:
                        count++;
                        count %= 100;

                        // UPDATE CHILDREN
                        for (int i = 0; i < children.Count; i++)
                        {
                            if (!children.ElementAt(i).isAlive())
                            {
                                children.RemoveAt(i);
                                i--;
                            }
                        }

                        // SPAWN LOGIC
                        if (count == 0 && children.Count < 5)
                        {
                            Random sp = new Random(Guid.NewGuid().GetHashCode());
                            int childSpeed = sp.Next(1, 8) * 2;

                            Sentry child = new Sentry(secondary, childSpeed);
                            children.Add(child);
                            level.addSentry(child);
                            int i = 0;
                            while (i == 0 || platforms.ElementAt(i) == platform)
                            {
                                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                                i = rnd.Next(1, platforms.Count);
                            }
                            child.setPlatform(platforms.ElementAt(i));
                        }
                        break;
                    case Type.RANDOM:
                        // RESOLVE to another type
                        while (type == Type.RANDOM)
                        {
                            Random random = new Random(Guid.NewGuid().GetHashCode());
                            type = (Type)random.Next(0, NUM_TYPES);
                        }

                        if (type == Type.SPAWN)
                        {
                            children = new List<Sentry>();
                            count = 0;
                            secondary = Type.RANDOM;
                        }
                        break;
                    default:
                        // TODO
                        break;
                }
            }
        }

        public Player seesPlayer()
        {
            List<Player> players = level.getPlayers();

            foreach (Player player in players)
            {
                Point p = player.getLocation();
                if (Math.Abs(p.Y - location.Y) < 20 &&
                    (p.X - location.X) / direction > 0)
                {
                    return player;
                }
            }
            return null;
        }
    }
}
