using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGClient
{
    using System.Drawing;

    using OpenTK;

    interface IShapeRenderer
    {
        void DrawLine(Vector2 p1, Vector2 p2, Color color, float lineWidth = 1);
    }
}
