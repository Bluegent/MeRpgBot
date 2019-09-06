using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGClient
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

    public class ObjectRenderer : Renderable
    {
        private readonly int _texture;
        public ObjectRenderer(Vertex[] vertices, int program, string filename = null)
            : base(program, vertices.Length)
        {
            GL.NamedBufferStorage(
                Buffer,
                Vertex.Size * vertices.Length,
                vertices,
                BufferStorageFlags.MapWriteBit);

            GL.VertexArrayAttribBinding(VertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 0);
            GL.VertexArrayAttribFormat(
                VertexArray,
                0,
                4,
                VertexAttribType.Float,
                false,
                0);

            GL.VertexArrayAttribBinding(VertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 1);
            GL.VertexArrayAttribFormat(
                VertexArray,
                1,
                4,
                VertexAttribType.Float,
                false,
                2 * Vector2.SizeInBytes + Vector4.SizeInBytes);

            BindTexture();
            GL.VertexArrayVertexBuffer(VertexArray, 0, Buffer, IntPtr.Zero, Vertex.Size);


            _texture = -1;
            if (!string.IsNullOrEmpty(filename))
            {
                _texture = ContentPipe.LoadInternalTexture("default.png").Id;
                //Load texture
                SetFiltering(All.Linear);
            }
        }

        private void BindTexture()
        {
            GL.VertexArrayAttribBinding(VertexArray, 2, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 2);
            GL.VertexArrayAttribFormat(
                VertexArray,
                2,
                2,
                VertexAttribType.Float,
                false,
                 Vector4.SizeInBytes);
            GL.VertexArrayAttribBinding(VertexArray, 3, 0);
            GL.EnableVertexArrayAttrib(VertexArray, 3);
            GL.VertexArrayAttribFormat(
                VertexArray,
                3,
                2,
                VertexAttribType.Float,
                false,
                 Vector2.SizeInBytes + Vector4.SizeInBytes);
        }
        private void SetFiltering(All filter)
        {
            var textureMinFilter = (int)filter;
            GL.TextureParameterI(_texture, TextureParameterName.TextureMinFilter, ref textureMinFilter);
            var textureMagFilter = (int)filter;
            GL.TextureParameterI(_texture, TextureParameterName.TextureMagFilter, ref textureMagFilter);
        }

        private int LoadTexture(string filename)
        {
            int width, height;
            int texture = -1;
            using (var bmp = (Bitmap)Image.FromFile($"Content/{filename}"))
            {
                width = bmp.Width;
                height = bmp.Height;
                BitmapData data = null;
               
                try
                {
                    data = bmp.LockBits(
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    GL.CreateTextures(TextureTarget.Texture2D, 1, out texture);
                    GL.TextureStorage2D(
                        texture,
                        1,                           // levels of mipmapping
                        SizedInternalFormat.Rgba32f, // format of texture
                        width,
                        height);
                    IntPtr srcBmpPtr = data.Scan0;
                    int bitsPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat);
                    int srcIMGBytesSize = data.Stride * data.Height;
                    byte[] byteSrcImage2DData = new byte[srcIMGBytesSize];
                    Marshal.Copy(srcBmpPtr, byteSrcImage2DData, 0, srcIMGBytesSize);
                    float[] srcImage2DData = new float[srcIMGBytesSize];
                    Array.Copy(byteSrcImage2DData, srcImage2DData, srcIMGBytesSize);

                    GL.BindTexture(TextureTarget.Texture2D, texture);
                    GL.TextureSubImage2D(texture,
                        0,                  // this is level 0
                        0,                  // x offset
                        0,                  // y offset
                        width,
                        height,
                        PixelFormat.Rgba,
                        PixelType.Float,
                        srcImage2DData);
                }
                finally
                {
                    bmp.UnlockBits(data);

                }
            }


            return texture;
            // data not needed from here on, OpenGL has the data
        }

        public override void Bind()
        {
            base.Bind();
            if (_texture != -1)
                GL.BindTexture(TextureTarget.Texture2D, _texture);
        }
    }
}
