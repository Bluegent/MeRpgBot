namespace RPGClient
{
    using System.Drawing;

    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class ShapeRenderer2D : IShapeRenderer
    {
        public void DrawLine(Vector2 p1, Vector2 p2, Color color, float lineWidth = 1)
        {
            GL.PushMatrix();
            GL.Begin(PrimitiveType.Lines);
            GL.Color4(color);
            GL.LineWidth(lineWidth);
            GL.Vertex2(p1);
            GL.Vertex2(p2);
            GL.End();
            GL.PopMatrix();
        }

        public void DrawSpline(BezierSpline spline, bool debug = false)
        {
            if (debug)
            {
                for (int i = 1; i < spline.control.Length; ++i)
                {
                    DrawLine(spline.control[i - 1], spline.control[i], Color.Red);
                }
            }

            for (int i = 1; i < spline.ResultCurve.Count; ++i)
            {
                DrawLine(spline.ResultCurve[i-1],spline.ResultCurve[i],Color.Black);
            }

        }
    }
}