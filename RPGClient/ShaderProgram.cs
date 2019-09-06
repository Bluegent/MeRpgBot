using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGClient
{
    using System.IO;

    using OpenTK.Graphics.OpenGL;

    public class ShaderProgram : IDisposable
    {
        public int Id => _program;
        private readonly int _program;
        private readonly List<int> _shaders = new List<int>();

        public ShaderProgram()
        {
            _program = GL.CreateProgram();
        }

        public void Link()
        {
            foreach (var shader in _shaders)
                GL.AttachShader(_program, shader);
            GL.LinkProgram(_program);
            var info = GL.GetProgramInfoLog(_program);
            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine($"GL.LinkProgram had info log: {info}");

            foreach (var shader in _shaders)
            {
                GL.DetachShader(_program, shader);
                GL.DeleteShader(shader);
            }
        }

        public void AddShader(ShaderType type, string shaderSource)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, shaderSource);
            GL.CompileShader(shader);
            string info = GL.GetShaderInfoLog(shader);
            if (!string.IsNullOrEmpty(info))
            {
                Console.WriteLine($"GL.CompileShader [{type}]:{info}");
            }
            _shaders.Add(shader);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GL.DeleteProgram(_program);
            }
        }

        public static string GetDefaultColorVertexShader()
        {
            string shader = "";
            using (System.IO.StreamReader sr = new StreamReader("Content/Shaders/Default/color.vert"))
            {
                shader = sr.ReadToEnd();
                sr.Close();
            }
            return shader;
        }

        public static string GetDefaultColorFragmentShader()
        {
            string shader = "";
            using (System.IO.StreamReader sr = new StreamReader("Content/Shaders/Default/color.frag"))
            {
                shader = sr.ReadToEnd();
                sr.Close();
            }
            return shader;
        }

        public static string GetDefaultTextureVertexShader()
        {
            string shader = "";
            using (System.IO.StreamReader sr = new StreamReader("Content/Shaders/Default/texture.vert"))
            {
                shader = sr.ReadToEnd();
                sr.Close();
            }
            return shader;
        }

        public static string GetDefaultTextureFragmentShader()
        {
            string shader = "";
            using (System.IO.StreamReader sr = new StreamReader("Content/Shaders/Default/texture.frag"))
            {
                shader = sr.ReadToEnd();
                sr.Close();
            }
            return shader;
        }

    }
}
