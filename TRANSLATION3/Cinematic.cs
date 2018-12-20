using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRANSLATION3.Properties;

namespace TRANSLATION3
{
    class Cinematic
    {
        private Stack<CineFrame> frames;
        public readonly Mode linkTo;
        private main main;
        
        public Cinematic(CineFrame[] frames, Mode linkTo, main main)
        {
            this.main = main;
            this.linkTo = linkTo;
            this.frames = new Stack<CineFrame>();

            // Could do in order and reverse,
            // but this way conveys LIFO nature of Stack
            for (int i = frames.Length - 1; i >= 0; i--)
            {
                this.frames.Push(frames[i]);
            }
        }

        public bool update()
        {
            frames.Peek().older();
            if (frames.Peek().getAge() == 0)
            {
                frames.Pop();
            }
            return (frames.Count > 0);
        }

        public Bitmap draw()
        {
            return frames.Peek().draw();
        }

        public void link()
        {
            main.setMode(linkTo);
        }

        public static Cinematic fromString(String s, main main)
        {
            switch (s)
            {
                case "startup":
                default:
                    Bitmap black = new Bitmap(1280, 720);

                    Graphics.FromImage(black).FillRectangle(
                        new SolidBrush(Color.FromArgb(0, 0, 0)),
                        0, 0, 1280, 720);

                    Bitmap rsgScreen = new Bitmap(1280, 720);

                    using (Graphics g = Graphics.FromImage(rsgScreen))
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0)),
                            0, 0, 1280, 720);
                        g.DrawImage(Resources._redsquaregames, 390, 110);
                    }

                    Bitmap logoScreen = new Bitmap(1280, 720);

                    using (Graphics g = Graphics.FromImage(logoScreen))
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0)),
                            0, 0, 1280, 720);
                        g.DrawImage(Resources.t3logo_664_164, 308, 278);
                    }

                    return new Cinematic(new CineFrame[] {
                        new CineFrame(10, black),
                        new CineFrame(100, rsgScreen),
                        new CineFrame(100, logoScreen) },
                        Mode.GAME, main);
            }
        }
    }
}
