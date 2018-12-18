using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    class Render
    {
        public static Bitmap initialBackground()
        {
            Bitmap bitmap = new Bitmap(1280, 720);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(20, 20, 20)), 0, 0, 1280, 720);
            }
            return bitmap;
        }

        public static Bitmap menuBackground()
        {
            Bitmap bitmap = new Bitmap(1280, 720);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(20, 20, 20)), 0, 0, 1280, 720);
            }
            return bitmap;
        }

        public static Bitmap player()
        {
            Bitmap player = new Bitmap(20, 20);
            using (Graphics g = Graphics.FromImage(player))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), 0, 0, 20, 20);
                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 0)), 3, 3, 14, 14);
            }
            return player;
        }

        public static Color sentryColor(Sentry sentry)
        {
            switch (sentry.getType())
            {
                case Sentry.Type.PUSH:
                     return Color.FromArgb(0, 0, 255);
                case Sentry.Type.SHOVE:
                    return Color.FromArgb(0, 0, 127);
                case Sentry.Type.PULL:
                    return Color.FromArgb(100, 255, 255);
                case Sentry.Type.DROP:
                    return Color.FromArgb(120, 0, 0);
                case Sentry.Type.HORZ_MAGNET:
                    return Color.FromArgb(255, 100, 255);
                case Sentry.Type.GRAV_FLIP:
                    return Color.FromArgb(127, 255, 127);
                case Sentry.Type.GRAV_DOUBLE:
                    return Color.FromArgb(255, 127, 127);
                case Sentry.Type.MOVE:
                    return Color.FromArgb(0, 255, 0);
                case Sentry.Type.DECAY:
                    return Color.FromArgb(255, 255, 0);
                case Sentry.Type.FLEE:
                    return Color.FromArgb(255, 255, 255);
                case Sentry.Type.SPREAD:
                    return Color.FromArgb(0, 151, 151);
                case Sentry.Type.SPAWN:
                    return Color.FromArgb(150, 100, 0);
                case Sentry.Type.GRAV_RED:
                    return Color.FromArgb(100, 100, 255);
                case Sentry.Type.GRAV_INC:
                    return Color.FromArgb(255, 255, 127);
                case Sentry.Type.GOD:
                    return Color.FromArgb(150, 0, 150);
                case Sentry.Type.RANDOM:
                    return Color.FromArgb(20, 20, 20);
                default:
                     return Color.FromArgb(0, 0, 0);
            }
        }

        public static Bitmap sentry(Sentry sentry)
        {
            Color color = sentryColor(sentry);

            Bitmap bitmap = new Bitmap(20, 20);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (sentry.isAlive())
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), 0, 0, 20, 20);
                    g.FillRectangle(new SolidBrush(color), 3, 3, 14, 14);
                } else
                {
                    g.FillRectangle(new SolidBrush(color), 0, 16, 20, 4);
                }
            }
            return bitmap;
        }

        public static Bitmap platform(Platform platform)
        {
            int w = platform.getWidth();
            Bitmap bitmap = new Bitmap(w, 20);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), 0, 0, w, 20);
                g.FillRectangle(new SolidBrush(Color.FromArgb(100, 100, 100)), 3, 3, w - 6, 14);
            }
            return bitmap;
        }
    }
}
