using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    public class Sentry : HasLocation
    {
        private static int NUM_TYPES = 18;

        private Type type;
        private Platform platform;
        private Level level;
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
            NECROMANCER, // 0, 0, 0
            EXPAND, // 255, 100, 0
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
            this.count = 0;

            if (type == Type.SPAWN || type == Type.NECROMANCER)
            {
                this.secondary = Type.RANDOM;
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

        public static String read(Sentry.Type s)
        {
            switch (s)
            {
                case Type.DECAY:
                    return "PLATFORM DECAYING";
                case Type.DROP:
                    return "DROPPING";
                case Type.EXPAND:
                    return "STAGE EXPANDING";
                case Type.FLEE:
                    return "FLEEING";
                case Type.GOD:
                    return "STAGE FLIPPING";
                case Type.GRAV_DOUBLE:
                    return "GRAVITY DOUBLING";
                case Type.GRAV_FLIP:
                    return "GRAVITY FLIPPING";
                case Type.GRAV_INC:
                    return "GRAVITY INCREASING";
                case Type.GRAV_RED:
                    return "GRAVITY REDUCING";
                case Type.HORZ_MAGNET:
                    return "HORIZONTAL MAGNET";
                case Type.MOVE:
                    return "PLATFORM MOVING";
                case Type.NECROMANCER:
                    return "NECROMANCER";
                case Type.PULL:
                    return "PULLING";
                case Type.PUSH:
                    return "PUSHING";
                case Type.SHOVE:
                    return "SHOVING";
                case Type.SPAWN:
                    return "SPAWNER";
                case Type.SPREAD:
                    return "PLATFORM SPREADING";
                default:
                    return s.ToString();
            }
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
        
        public Platform getPlatform()
        {
            return platform;
        }

        public int getSpeed()
        {
            return speed;
        }

        public void changeSpeed(int inc)
        {
            speed = MathExt.Bounded(2, speed + inc, 14);
        }

        public String interpretDirection()
        {
            if (direction == -1)
                return "LEFT";

            return "RIGHT";
        }

        public int getDirection()
        {
            return direction;
        }

        public void setDirection(int direction)
        {
            this.direction = direction;
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

        public void nextType()
        {
            type++;
            if ((int)type >= NUM_TYPES)
                type = 0;
        }

        public void nextSecondary()
        {
            secondary++;
            if ((int)secondary >= NUM_TYPES)
                secondary = 0;
        }

        public int getCount()
        {
            return count;
        }

        public void crush()
        {
            alive = false;
            if (type == Type.NECROMANCER)
            {
                foreach (Sentry child in children)
                {
                    if (child.alive)
                    {
                        child.crush();
                        level.addAnimation(new Animation(
                            Animation.Permanence.TEMPORARY,
                            Render.sentryColor(child), child.getLocation(), 0));
                    }
                }
            }
        }

        public bool sightDependent()
        {
            switch (type)
            {
                case Type.GRAV_INC:
                case Type.GRAV_RED:
                case Type.HORZ_MAGNET:
                case Type.SPAWN:
                case Type.NECROMANCER:
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

            fix();
        }

        private void fix()
        {
            // fixes the Y pos of the sentry to above its platform
            location.Y = platform.getLocation().Y - 20;

            if (location.X < platform.getLocation().X - (platform.getWidth() / 2))
                location.X = platform.getLocation().X - (platform.getWidth() / 2) + 10;

            if (location.X > platform.getLocation().X + (platform.getWidth() / 2))
                location.X = platform.getLocation().X + (platform.getWidth() / 2) - 10;
        }

        public void editorAdjust()
        {
            location.Y = platform.getLocation().Y - 20;
            location.X = platform.getLocation().X;
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
                            {
                                platform.changeWidth(-speed);
                                platform.moveX(direction * (-speed / 2));
                            }
                            break;
                        case Type.SPREAD:
                            if (platform.getWidth() < 500)
                                platform.changeWidth(speed);
                            break;
                        case Type.EXPAND:
                            foreach (Platform p in level.getPlatforms())
                            {
                                if (p != platform)
                                {
                                    p.moveX(speed * Math.Sign(
                                        p.getLocation().X - platform.getLocation().X));
                                    p.moveY((speed / 2) * Math.Sign(
                                        p.getLocation().Y - platform.getLocation().Y));
                                }
                            }
                            foreach (Sentry s in level.getSentries())
                            {
                                if (s != this)
                                {
                                    s.fix();
                                }
                            }
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
                            List<Animation> animations = level.getAnimations();
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

                            foreach (Animation a in animations)
                            {
                                a.setY(-1 * a.getLocation().Y);
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

                            foreach (Animation a in animations)
                            {
                                a.moveY(o);
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
                    case Type.NECROMANCER:
                        count++;
                        count %= 100;

                        // REANIMATION LOGIC
                        if (count == 0)
                        {
                            for (int i = 0; i < level.getSentries().Count; i++)
                            {
                                Sentry s = level.getSentries().ElementAt(i);
                                if (!s.alive && s != this)
                                {
                                    s.alive = true;
                                    children.Add(s);
                                    break;
                                }
                            }
                        }
                        break;
                    case Type.RANDOM:
                        // RESOLVE to another type
                        while (type == Type.RANDOM)
                        {
                            Random random = new Random(Guid.NewGuid().GetHashCode());
                            type = (Type)random.Next(0, NUM_TYPES);
                        }

                        if (type == Type.SPAWN || type == Type.NECROMANCER)
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
