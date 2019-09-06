using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGClient
{
    using OpenTK;
    using OpenTK.Graphics;

    public class RenderableFactory
    {

        public static Vertex[] CreateTexturedCube(float side, float textureWidth, float textureHeight)
        {
            float h = textureHeight;
            float w = textureWidth;
            side = side / 2f; // half side - and other half

            Vertex[] vertices =
            {
                new Vertex(new Vector4(-side, -side, -side, 1.0f),   new Vector2(0, h)),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    new Vector2(w, h)),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    new Vector2(0, 0)),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    new Vector2(0, 0)),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    new Vector2(w, h)),
                new Vertex(new Vector4(-side, side, side, 1.0f),     new Vector2(w, 0)),

                new Vertex(new Vector4(side, -side, -side, 1.0f),    new Vector2(w, 0)),
                new Vertex(new Vector4(side, side, -side, 1.0f),     new Vector2(0, 0)),
                new Vertex(new Vector4(side, -side, side, 1.0f),     new Vector2(w, h)),
                new Vertex(new Vector4(side, -side, side, 1.0f),     new Vector2(w, h)),
                new Vertex(new Vector4(side, side, -side, 1.0f),     new Vector2(0, 0)),
                new Vertex(new Vector4(side, side, side, 1.0f),      new Vector2(0, h)),

                new Vertex(new Vector4(-side, -side, -side, 1.0f),   new Vector2(w, 0)),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    new Vector2(0, 0)),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    new Vector2(w, h)),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    new Vector2(w, h)),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    new Vector2(0, 0)),
                new Vertex(new Vector4(side, -side, side, 1.0f),     new Vector2(0, h)),

                new Vertex(new Vector4(-side, side, -side, 1.0f),    new Vector2(w, 0)),
                new Vertex(new Vector4(-side, side, side, 1.0f),     new Vector2(0, 0)),
                new Vertex(new Vector4(side, side, -side, 1.0f),     new Vector2(w, h)),
                new Vertex(new Vector4(side, side, -side, 1.0f),     new Vector2(w, h)),
                new Vertex(new Vector4(-side, side, side, 1.0f),     new Vector2(0, 0)),
                new Vertex(new Vector4(side, side, side, 1.0f),      new Vector2(0, h)),

                new Vertex(new Vector4(-side, -side, -side, 1.0f),   new Vector2(0, h)),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    new Vector2(w, h)),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    new Vector2(0, 0)),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    new Vector2(0, 0)),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    new Vector2(w, h)),
                new Vertex(new Vector4(side, side, -side, 1.0f),     new Vector2(w, 0)),

                new Vertex(new Vector4(-side, -side, side, 1.0f),    new Vector2(0, h)),
                new Vertex(new Vector4(side, -side, side, 1.0f),     new Vector2(w, h)),
                new Vertex(new Vector4(-side, side, side, 1.0f),     new Vector2(0, 0)),
                new Vertex(new Vector4(-side, side, side, 1.0f),     new Vector2(0, 0)),
                new Vertex(new Vector4(side, -side, side, 1.0f),     new Vector2(w, h)),
                new Vertex(new Vector4(side, side, side, 1.0f),      new Vector2(w, 0)),
            };
            return vertices;
        }

        public static Vertex[] CreateSolidCube(float side, Color4 color)
        {
            side = side / 2f; // half side - and other half
            Vertex[] vertices =
            {
                new Vertex(new Vector4(-side, -side, -side, 1.0f),   color),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),
                new Vertex(new Vector4(-side, side, side, 1.0f),     color),

                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),
                new Vertex(new Vector4(side, side, -side, 1.0f),     color),
                new Vertex(new Vector4(side, -side, side, 1.0f),     color),
                new Vertex(new Vector4(side, -side, side, 1.0f),     color),
                new Vertex(new Vector4(side, side, -side, 1.0f),     color),
                new Vertex(new Vector4(side, side, side, 1.0f),      color),

                new Vertex(new Vector4(-side, -side, -side, 1.0f),   color),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),
                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),
                new Vertex(new Vector4(side, -side, side, 1.0f),     color),

                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),
                new Vertex(new Vector4(-side, side, side, 1.0f),     color),
                new Vertex(new Vector4(side, side, -side, 1.0f),     color),
                new Vertex(new Vector4(side, side, -side, 1.0f),     color),
                new Vertex(new Vector4(-side, side, side, 1.0f),     color),
                new Vertex(new Vector4(side, side, side, 1.0f),      color),

                new Vertex(new Vector4(-side, -side, -side, 1.0f),   color),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),
                new Vertex(new Vector4(side, -side, -side, 1.0f),    color),
                new Vertex(new Vector4(-side, side, -side, 1.0f),    color),
                new Vertex(new Vector4(side, side, -side, 1.0f),     color),

                new Vertex(new Vector4(-side, -side, side, 1.0f),    color),
                new Vertex(new Vector4(side, -side, side, 1.0f),     color),
                new Vertex(new Vector4(-side, side, side, 1.0f),     color),
                new Vertex(new Vector4(-side, side, side, 1.0f),     color),
                new Vertex(new Vector4(side, -side, side, 1.0f),     color),
                new Vertex(new Vector4(side, side, side, 1.0f),      color),
            };
            return vertices;
        }

    }
}
