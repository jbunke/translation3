using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRANSLATION3
{
    public class Player
    {
        private Level level;

        private Point location;
        private int direction = 1;
        private Point lastLocation;
        private Point saveLocation;

        public Keys[] controls = new Keys[] {Keys.W, Keys.S, Keys.A,
            Keys.D, Keys.Q, Keys.E, Keys.Space, Keys.Z, Keys.Escape};
        // {0 JUMP, 1 DROP, 2 LEFT, 3 RIGHT, 4 SAVE, 5 LOAD, 6 TELEPORT, 7 ZOOM, 8 PAUSE}

        private bool isLeft;
        private bool isRight;
        private bool isTele;

        private int speed = 8;
        private int telePhase = 0;
        private int gAcceleration = 0;

        public Player() { }

        public Player(GameSettings.ControlMode controlMode)
        {
            setControls(controlMode);
        }

        public void setControls(GameSettings.ControlMode controlMode)
        {
            switch (controlMode)
            {
                case GameSettings.ControlMode.NUMPAD:
                    controls = new Keys[] {Keys.NumPad8, Keys.NumPad5,
                        Keys.NumPad4, Keys.NumPad6, Keys.NumPad7, Keys.NumPad9,
                        Keys.NumPad0, Keys.NumPad1, Keys.Escape};
                    break;
                default:
                    controls = new Keys[] {Keys.W, Keys.S, Keys.A,
                        Keys.D, Keys.Q, Keys.E,
                        Keys.Space, Keys.Z, Keys.Escape};
                    break;
            }
        }

        public void keyHandler(KeyEventArgs e, bool down)
        {
            if (down)
            {
                // KEY-DOWN
                if (e.KeyCode == controls[0])
                {
                    // JUMP
                    if (isSupported())
                    {
                        gAcceleration = 30;
                        level.addAnimation(new Animation(Animation.Permanence.TEMPORARY,
                            Color.FromArgb(255, 0, 0), location, 0));
                    }
                }

                if (e.KeyCode == controls[1])
                {
                    // DROP
                    if (isSupported())
                    {
                        location.Y += 21;
                    }
                    else
                    {
                        gAcceleration -= 30;
                        level.addAnimation(new Animation(Animation.Permanence.TEMPORARY,
                            Color.FromArgb(255, 0, 0), location, 0));
                    }
                }

                if (e.KeyCode == controls[2])
                {
                    // LEFT
                    isLeft = true;
                    direction = -1;
                }

                if (e.KeyCode == controls[3])
                {
                    // RIGHT
                    isRight = true;
                    direction = 1;
                }

                if (e.KeyCode == controls[6])
                {
                    // TELEPORT
                    isTele = true;
                }
            } else
            {
                // KEY-UP
                if (e.KeyCode == controls[2])
                {
                    // LEFT
                    isLeft = false;
                }

                if (e.KeyCode == controls[3])
                {
                    // RIGHT
                    isRight = false;
                }

                if (e.KeyCode == controls[6])
                {
                    // TELEPORT
                    isTele = false;
                    teleport();
                    level.addAnimation(new Animation(Animation.Permanence.TEMPORARY,
                        Color.FromArgb(255, 0, 0), location, 0));
                }

                if (e.KeyCode == controls[4])
                {
                    // SAVE
                    saveLocation = location;
                }

                if (e.KeyCode == controls[5])
                {
                    // LOAD
                    location = saveLocation;
                    level.addAnimation(new Animation(Animation.Permanence.TEMPORARY,
                        Color.FromArgb(255, 0, 0), location, 0));
                }

                if (e.KeyCode == controls[7])
                {
                    // ZOOM
                    level.getCamera().switchZoom();
                }

                if (e.KeyCode == controls[8])
                {
                    // PAUSE
                    level.pause();
                }
            }
        }

        public void movement()
        {
            lastLocation = location;

            if (isLeft)
                location.X -= speed;

            if (isRight)
                location.X += speed;

            if (isTele && telePhase < 10)
                telePhase++; // lateral teleportation is capped

            gravity();
            checkCrush();
            
            // TODO: wrapping?? and reset
        }
        
        public void setLevel(Level level)
        {
            this.level = level;
            Point reference = level.startingPlatform().getLocation();
            this.location = new Point(reference.X, reference.Y - 50);
            this.saveLocation = location;
        }

        public void moveX(int inc)
        {
            location.X += inc;
        }

        public void moveY(int inc)
        {
            location.Y += inc;
        }

        public void moveSY(int inc)
        {
            saveLocation.Y += inc;
        }

        public void changeGAcceleration(int inc)
        {
            gAcceleration += inc;
        }

        public void mulGAcceleration(int mult)
        {
            gAcceleration *= mult;
        }

        public void setY(int y)
        {
            location.Y = y;
        }

        public void setSY(int y)
        {
            saveLocation.Y = y;
        }

        public Point getLocation()
        {
            return location;
        }

        public Point getSaveLocation()
        {
            return saveLocation;
        }

        public int getTelePhase()
        {
            return telePhase;
        }

        public int getDirection()
        {
            return direction;
        }

        public int getSpeed()
        {
            return speed;
        }

        private void gravity()
        {
            location.Y -= gAcceleration;

            if (gAcceleration > 0 || !isSupported())
                gAcceleration -= 2;
        }

        private void checkCrush()
        {
            List<Sentry> sentries = level.getSentries();

            foreach (Sentry sentry in sentries)
            {
                Point s = sentry.getLocation();
                if (lastLocation.Y < s.Y && location.Y > s.Y - 20 &&
                    Math.Abs(location.X - s.X) < 20 && sentry.isAlive())
                {
                    sentry.crush();
                    level.addAnimation(new Animation(Animation.Permanence.PERMANENT,
                        Render.sentryColor(sentry), sentry.getLocation(),
                        gAcceleration * -2));
                    level.addAnimation(new Animation(Animation.Permanence.TEMPORARY,
                        Render.sentryColor(sentry), sentry.getLocation(), 0));
                }
            }
        }

        private bool isSupported()
        {
            List<Platform> platforms = level.getPlatforms();
            foreach (Platform platform in platforms)
            {
                if (lastLocation.Y <= platform.getLocation().Y &&
                    location.Y > platform.getLocation().Y - 21 &&
                    Math.Abs(location.X - platform.getLocation().X) < (platform.getWidth() / 2) + 10)
                {
                    location.Y = platform.getLocation().Y - 20;
                    gAcceleration = 0;
                    return true;
                }
            }
            return false;
        }

        private void teleport()
        {
            lastLocation = location;
            location.X += (5 * direction * telePhase * speed);
            telePhase = 0;
        }
    }
}
