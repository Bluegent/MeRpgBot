using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace RPGClient
{
    using System.Drawing;
    using System.Linq.Expressions;

    using OpenTK.Graphics;

    class ClientWindow : GameWindow
    {
        private IShapeRenderer shapeRenderer;

        private View view;
        public ClientWindow() : base(800,600, new GraphicsMode(32, 24, 8, 8))
        {
            Title = "My Client Window";
            shapeRenderer=new ShapeRenderer2D();
            view=new View(Width,Height);
            
            
        }

        protected override void OnResize(EventArgs e)
        {
            view.Update(Width,Height);
            view.Apply();
            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {

            GL.ClearColor(1f, 1f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            view.Apply();
            shapeRenderer.DrawLine(new Vector2(0,0),new Vector2(Width,Height), Color.Black);
            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}
