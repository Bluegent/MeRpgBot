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

        private View view;

        private Texture2D texture;

        private SpriteBatch spriteBatch;
        public ClientWindow() : base(800,600, new GraphicsMode(32, 24, 8, 8))
        {
            Title = "My Client Window";
            view=new View(Width,Height);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.Texture2D);

            /*spriteBatch=new SpriteBatch();
            texture = ContentPipe.LoadInternalTexture("player.png");
            BezierSpline spline=new BezierSpline(new Vector2[]{new Vector2(0,0), new Vector2(Width, 0), new Vector2(Width, Height),new Vector2(0,Height/2)});
            animation=new Animation(texture,spline,10000);*/


        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            view.Update(Width,Height);
            view.Apply();
            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"FPS: {RenderFrequency.ToString("0")}/{TargetRenderFrequency}";
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            view.Apply();
            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}
