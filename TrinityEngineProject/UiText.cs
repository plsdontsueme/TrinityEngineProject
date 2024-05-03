using OpenTK.Graphics.OpenGL4;
using StbTrueTypeSharp;
using System.Globalization;

namespace TrinityEngineProject
{
    internal class UiText : ElementRenderer
    {
        public string Text;
        Font font;

        int VertexArrayObject;
        int VertexBufferObject;
        public UiText(Font font, string text) : base(font)
        {
            Text = text;
            this.font = font;
            float[] vertices = {
            0, 0, 0, 0, 0, //Bottom-left vertex
            0, 0, 0, 1, 0, //Bottom-right vertex
            0, 0, 0, 0, 1,  //Top left
            0, 0, 0, 1, 1  //Top right
            };
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, 80, vertices, BufferUsageHint.StreamDraw); //80 = vertices.Length * sizeof(float)
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        public override void RenderElement()
        {
            base.RenderElement();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            string[] lines = Text.Split(Environment.NewLine);
            float y = 0;
            foreach (string line in lines)
            {
                int x = 0;
                foreach (char c in line)
                {
                    var g = font.glyphs[c];

                    float gy = y - g.YOffset;
                    float[] vertices = {
                    x,          gy - g.Height, 0, g.X0, g.Y0, //Bottom-left vertex
                    x+g.Width,  gy - g.Height, 0, g.X1, g.Y0, //Bottom-right vertex
                    x,          gy,            0, g.X0, g.Y1, //Top left
                    x+g.Width,  gy,            0, g.X1, g.Y1  //Top right
                    };
                    x += g.XAdvance - g.XOffset;

                    GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertices.Length * sizeof(float), vertices);
                    GL.BindVertexArray(VertexArrayObject);
                    GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
                }
                y -= font.lineHeight;
            }
        }
    }
}
