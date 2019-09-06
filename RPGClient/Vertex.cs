using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGClient
{
    using System.Drawing;

    using OpenTK;
    using OpenTK.Graphics;

    public struct Vertex
    {
        Vector4 position;
        Vector2 texCoord;
        Vector2 texOffset;
        Color4 color;
        uint useTexture;


        public Vertex(Vector4 pos, Vector2 tex)
        {
            color = Color4.White;
            texOffset = Vector2.Zero;
            position = pos;
            texCoord = tex;
            useTexture = 1;

        }

        public Vertex(Vector4 pos, Color4 col)
        {
            texCoord = Vector2.Zero;
           color = col;
            texOffset = Vector2.Zero;
            position = pos;
            useTexture = 1;

        }
        public Vertex(Vector4 pos, Vector2 tex, Color4 col)
        {
            color = col;
            texOffset = Vector2.Zero;
            position = pos;
            texCoord = tex;
            useTexture = 1;

        }

        public static int Size
        {
            get
            {
                return Vector2.SizeInBytes * 2 + 2 * Vector4.SizeInBytes + sizeof(uint);
            }
        }
        public Color Color
        {
            get
            {
                return Color.FromArgb(
                    (int)(255 * color.A),
                    (int)(255 * color.R),
                    (int)(255 * color.G),
                    (int)(255 * color.B));
            }
            set
            {
                color = new Color4(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
            }
        }

    }
}
