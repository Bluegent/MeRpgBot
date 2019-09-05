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

        public void DrawRectangle(float x, float y, float width, float height, Color color, float lineWidth = 1)
        {
            Vector2 point1 = new Vector2(x,y);
            Vector2 point2 = new Vector2(x+width,y);
            Vector2 point3 = new Vector2(x+width,y+height);
            Vector2 point4 = new Vector2(x,y+height);
            GL.PushMatrix();
            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(color);
            GL.LineWidth(lineWidth);
            GL.Vertex2(point1);
            GL.Vertex2(point2);
            GL.Vertex2(point3);
            GL.Vertex2(point4);
            GL.End();
            GL.PopMatrix();
        }

        public void FillRectangle(float x, float y, float width, float height, Color color)
        {
            Vector2 point1 = new Vector2(x, y);
            Vector2 point2 = new Vector2(x + width, y);
            Vector2 point3 = new Vector2(x + width, y + height);
            Vector2 point4 = new Vector2(x, y + height);
            GL.PushMatrix();
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(color);
            GL.Vertex2(point1);
            GL.Vertex2(point2);
            GL.Vertex2(point3);
            GL.Vertex2(point4);
            GL.End();
            GL.PopMatrix();
        }

        public void DrawSpline(BezierSpline spline, Color color, float lineWidth = 1, bool debug = false)
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
                DrawLine(spline.ResultCurve[i - 1], spline.ResultCurve[i], color, lineWidth);
            }
        }
    }
}