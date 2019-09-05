using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGClient
{
    using System.Drawing;
    using System.Drawing.Imaging;

    using OpenTK.Graphics.OpenGL;

    using PixelFormat = System.Drawing.Imaging.PixelFormat;

    public class ContentPipe
    {
        public static Texture2D LoadInternalTexture(string fileName)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D,id);
            Bitmap bitmap = new Bitmap($"Content/{fileName}");
            BitmapData bData = bitmap.LockBits(new Rectangle( 0, 0, bitmap.Width, bitmap.Height),ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgba,bData.Width,bData.Height,0,OpenTK.Graphics.OpenGL.PixelFormat.Bgra,PixelType.UnsignedByte,bData.Scan0);
            bitmap.UnlockBits(bData);
            GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapS,(int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapT,(int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,(int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,(int)TextureMagFilter.Linear);
            Texture2D result = new Texture2D() { Id = id, Width = bitmap.Width, Height = bitmap.Height };
            bitmap.Dispose();
            return result;
        }
    }
}
