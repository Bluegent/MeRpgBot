using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
namespace RPGClient
{
    public class View
    {
        private float _width;

        private float _height;
        public View(float width, float height)
        {
            _width = width;
            _height = height;
        }

        public void Update(float width, float height)
        {
            _width = width;
            _height = height;
        }

        public void Apply()
        {
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.Ortho(0, _width, _height, 0, 1, -1);
            GL.Viewport(0,0,(int)_width,(int)_height);
        }
    }
}
