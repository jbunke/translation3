using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    public class Camera
    {
        private HasLocation target;
        private Point location;
        private bool zoomedOut;
        private FollowMode followMode = FollowMode.STEADY;

        public enum FollowMode
        {
            GLUED,
            STEADY,
            FIXED
        }

        public Camera(FollowMode followMode)
        {
            this.followMode = followMode;
        }

        public Camera(FollowMode followMode, HasLocation target)
        {
            this.followMode = followMode;
            setTarget(target);
        }

        public void follow()
        {
            switch (followMode)
            {
                case FollowMode.GLUED:
                    location = target.getLocation();
                    break;
                case FollowMode.FIXED:
                    break;
                case FollowMode.STEADY:
                    Point t = target.getLocation();

                    location.X += Math.Sign(t.X - location.X) * (int)Math.Ceiling(Math.Sqrt(Math.Abs(location.X - t.X)));
                    location.Y += Math.Sign(t.Y - location.Y) * (int)Math.Ceiling(Math.Sqrt(Math.Abs(location.Y - t.Y)));

                    break;
            }
        }

        public void setTarget(HasLocation target)
        {
            this.target = target;
            this.location = new Point(target.getLocation().X, target.getLocation().Y);
        }

        public HasLocation getTarget()
        {
            return target;
        }

        public Point getLocation()
        {
            return location;
        }

        public Point getTargetLocation()
        {
            return target.getLocation();
        }

        public bool isZoomedOut()
        {
            return zoomedOut;
        }

        public void switchZoom()
        {
            zoomedOut = !zoomedOut;
        }
    }
}
