using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCMushroomSim
{
    public partial class Form1 : Form
    {
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;

        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        { 
            GL.ClearColor(new Color4(0.192f, 0.192f, 0.192f, 1));

            //making array of points
            float[] vertices = new float[]
            {
                0.0f, 0.5f, 0f,     //vertex0
                0.5f, -0.5f, 0f,    //vertex1
                -0.5f, -0.5f, 0f    //vertex2
            };

            //giving them to gpu
            this.vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            this.vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            string vertexShaderCode = 
                @"
                #version 330 core

                layout (location = 0) in vec3 aPosition;

                void main()
                {
                    gl_Position = vec4(aPosition, 1f);
                }
                ";

            string fragmentShaderCode =
                @"
                #version 330 core

                out vec4 pixelColor;

                void main()
                {
                    pixelColor = vec4(1, 1, 1, 1f);
                }
                ";

            //compiling vertex shader
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);

            //compiling fragment(pixel) shader
            int fragmentShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);

            this.shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(this.shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(this.shaderProgramHandle, fragmentShaderHandle);

            GL.LinkProgram(this.shaderProgramHandle);

            GL.DetachShader(this.shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(this.shaderProgramHandle, fragmentShaderHandle);

            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            glControl1.MakeCurrent();

            GL.Viewport(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height);

            GL.Clear(ClearBufferMask.ColorBufferBit);


            GL.UseProgram(this.shaderProgramHandle);
            GL.BindVertexArray(this.vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            glControl1.SwapBuffers();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //clears buffers before closing
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(this.vertexBufferHandle);
            GL.UseProgram(0);
            GL.DeleteProgram(this.shaderProgramHandle);
        }
    }
}
