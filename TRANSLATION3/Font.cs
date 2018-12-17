using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRANSLATION3
{
    class Font
    {
        private Bitmap[] glyphs = new Bitmap[128];
        private String name;

        public Font(String name, Bitmap uppercase, Bitmap lowercase, 
            Bitmap numbers, Bitmap symbols)
        {
            for (int i = 0; i < 128; i++)
            {
                glyphs[i] = new Bitmap(1, 1);
            }

            this.name = name;
            generate(uppercase, 65, 26); // uppercase
            generate(lowercase, 97, 26); // lowercase
            generate(numbers, 48, 10); // numbers
            generate(symbols, 33, 15); // symbols

            // SPACE CHAR
            glyphs[32] = new Bitmap(32, glyphs[65].Height);
        }

        public Bitmap print(String text, int size)
        {
            int width = 0;
            int height = glyphs[65].Height;

            foreach (char c in text)
            {
                width += glyphs[(int)c].Width;
            }

            Bitmap printed = new Bitmap(width, height);
            int xCovered = 0;

            using (Graphics g = Graphics.FromImage(printed))
            {
                foreach (char c in text)
                {
                    g.DrawImage(glyphs[(int)c], xCovered, 0);

                    xCovered += glyphs[(int)c].Width;
                }
            }

            Bitmap resize = new Bitmap((int)(printed.Width * (size / 8f)),
                (int)(printed.Height * (size / 8f)));

            using (Graphics g = Graphics.FromImage(resize))
            {
                g.DrawImage(printed, 0, 0, resize.Width, resize.Height);
            }

            printed = resize;

            return printed;
        }

        private void generate(Bitmap reference, int offset, int duration)
        {
            int vertLogged = 0;

            for (int i = 0; i < duration; i++)
            {
                int height = 0;
                bool foundHeight = false;

                while (!foundHeight)
                {
                    if (reference.GetPixel(0, vertLogged + height) ==
                        Color.FromArgb(255, 0, 0))
                    {
                        foundHeight = true;
                    } else
                    {
                        height += 8;
                    }
                }

                int width = 0;
                bool foundWidth = false;

                while (!foundWidth)
                {
                    if (reference.GetPixel(width, vertLogged + (height / 2)) == 
                        Color.FromArgb(255, 0, 0))
                    {
                        foundWidth = true;
                    } else
                    {
                        width += 8;
                    }
                }

                glyphs[offset + i] = new Bitmap(width, height);

                for (int y = 0; y < height; y += 8)
                {
                    for (int x = 0; x < width; x += 8)
                    {
                        using (Graphics g = Graphics.FromImage(glyphs[offset + i]))
                        {
                            if (reference.GetPixel(x, y + vertLogged) ==
                                Color.FromArgb(0, 0, 0))
                            {
                                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), x, y, 8, 8);
                            }
                        }
                    }
                }

                vertLogged += (8 + height);
            }
        }
    }
}
