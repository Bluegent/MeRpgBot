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
    using System.Drawing.Drawing2D;
    using System.Linq.Expressions;

    using OpenTK.Graphics;

    class ClientWindow : GameWindow
    {
        private Dictionary<string, Renderable> renderables = new Dictionary<string, Renderable>();
        private readonly Color4 _backColor = new Color4(0.1f, 0.1f, 0.3f, 1.0f);
        private Matrix4 _projectionMatrix;
        private float _fov = 45f;

        private ShaderProgram program;

        private ShaderProgram textureProgram;
        public ClientWindow() : base(800, 600, new GraphicsMode(32, 24, 8, 8))
        {
            Title = "My Client Window";
           

            /*spriteBatch=new SpriteBatch();
            texture = ContentPipe.LoadInternalTexture("player.png");
            BezierSpline spline=new BezierSpline(new Vector2[]{new Vector2(0,0), new Vector2(Width, 0), new Vector2(Width, Height),new Vector2(0,Height/2)});
            animation=new Animation(texture,spline,10000);*/
            Console.WriteLine($"Vector4 {Vector4.SizeInBytes}, Vector2 {Vector2.SizeInBytes}");
        }
        private void CreateProjection()
        {
            var aspectRatio = (float)Width / Height;
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                _fov * ((float)Math.PI / 180f), // field of view angle, in radians
                aspectRatio,                // current window aspect ratio
                0.001f,                       // near plane
                4000f);                     // far plane
        }

        protected override void OnLoad(EventArgs e)
        {
            Console.WriteLine("OnLoad");
            VSync = VSyncMode.Off;
            CreateProjection();

            program = new ShaderProgram();
            program.AddShader(ShaderType.VertexShader, ShaderProgram.GetDefaultColorVertexShader());
            program.AddShader(ShaderType.FragmentShader, ShaderProgram.GetDefaultColorFragmentShader());
            program.Link();

            textureProgram = new ShaderProgram();
            textureProgram.AddShader(ShaderType.VertexShader, ShaderProgram.GetDefaultTextureVertexShader());
            textureProgram.AddShader(ShaderType.FragmentShader, ShaderProgram.GetDefaultTextureFragmentShader());
            textureProgram.Link();
            Renderable renderable = new ObjectRenderer(RenderableFactory.CreateSolidCube(1, Color.Red), program.Id);
            Renderable renderableTextured = new ObjectRenderer(RenderableFactory.CreateTexturedCube(1,1,1), textureProgram.Id, "default.png");
            //renderables.Add("renderable", renderable);
            renderables.Add("renderable2", renderableTextured);

            CursorVisible = true;

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.PointSize(3);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            base.OnLoad(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            CreateProjection();
            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"FPS: {RenderFrequency.ToString("0")}/{TargetRenderFrequency}";
            GL.ClearColor(_backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            int lastProgram = -1;
            foreach (var obj in renderables.Values)
            {
                lastProgram = RenderOneElement(obj, lastProgram);
            }

            SwapBuffers();
            base.OnRenderFrame(e);
        }
        private int RenderOneElement(Renderable obj, int lastProgram)
        {
            Vector3 position;
            position.X = 0;
            position.Y = 0;
            position.Z = 0;
            Matrix4 LookAtMatrix;
            LookAtMatrix = Matrix4.LookAt(position, -Vector3.UnitZ, Vector3.UnitY);
            Matrix4 _modelView;
            var prog = obj.Program;
            if (lastProgram != prog)
                GL.UniformMatrix4(20, false, ref _projectionMatrix);
            lastProgram = obj.Program;
            obj.Bind();
            var t2 = Matrix4.CreateTranslation(0, 0, -2.7f);
            var r1 = Matrix4.CreateRotationX(0);
            var r2 = Matrix4.CreateRotationY(45);
            var r3 = Matrix4.CreateRotationZ(0);
            var s = Matrix4.CreateScale(new Vector3(1f, 1f, 1f));
            _modelView = r1 * r2 * r3 * s * t2;//LookAtMatrix;
            GL.UniformMatrix4(21, false, ref _modelView);
            obj.Render();
            return lastProgram;
        }
    }
}
