using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGClient
{
    using System.Drawing;

    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class SpriteBatch
    {
        public void DrawTexture(Texture2D texture, float x, float y, float width, float height,float scale,float rotation, float pivotX, float pivotY, Color color)
        {
            width *= scale;
            height *= scale;
            Vector2 point1 = new Vector2(x, y);
            Vector2 point2 = new Vector2(x + width, y);
            Vector2 point3 = new Vector2(x + width, y + height);
            Vector2 point4 = new Vector2(x, y + height);
            GL.BindTexture(TextureTarget.Texture2D,texture.Id);
            GL.Color4(color);
            GL.PushMatrix();
            GL.Translate(pivotX,pivotY,0);
            GL.Rotate(rotation, 0, 0, 1);
            GL.Translate(-pivotX, -pivotY, 0);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex2(point1);
            GL.TexCoord2(1, 0);
            GL.Vertex2(point2);
            GL.TexCoord2(1, 1);
            GL.Vertex2(point3);
            GL.TexCoord2(0, 1);
            GL.Vertex2(point4);
            
            GL.End();
            GL.PopMatrix();
            GL.BindTexture(TextureTarget.Texture2D,0);
        }

        public void DrawTexture(Texture2D texture, float x, float y, float width, float height)
        {
            DrawTexture(texture,x,y,width,height,1,0,x+width/2,y+height/2,Color.White);
        }
        public void DrawTexture(Texture2D texture, float x, float y, float width, float height,Color color)
        {
            DrawTexture(texture, x, y, width, height,1, 0, x + width / 2, y + height / 2, color);
        }
        public void DrawTexture(Texture2D texture, float x, float y, float width, float height, float scale,float rotation, Color color)
        {
            DrawTexture(texture, x, y, width, height, scale, rotation, x + width / 2, y + height / 2, color);
        }
        public void DrawTexture(Texture2D texture, float x, float y, float width, float height, float rotation)
        {
            DrawTexture(texture, x, y, width, height,1, rotation, x + width / 2, y + height / 2, Color.White);
        }
        public void DrawTexture(Texture2D texture, float x, float y, float width, float height, float scale, Color color)
        {
            DrawTexture(texture, x, y, width, height,scale, 0, x + width / 2, y + height / 2, color);
        }
        public void DrawTexture(Texture2D texture, float x, float y, float width, float height, float rotation, float pivotX, float pivotY)
        {
            DrawTexture(texture, x, y, width, height,1, rotation, pivotX, pivotY, Color.White);
        }
    }
}
