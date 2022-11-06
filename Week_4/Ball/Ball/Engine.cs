using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ball
{
    internal class Engine
    {
        private PictureBox pictureBox;
        private Bitmap bitmap;
        private Ball ball;
        private Timer timer;


        public Engine(PictureBox pb)
        {
            pictureBox = pb;
            var size = pictureBox.Size;
            bitmap = new Bitmap(size.Width, size.Height);
            ball = new Ball(50, 50, 30);
            InitPb();
            timer = new Timer();
            timer.Interval = 15;
            timer.Tick += OnTimerTick;
            timer.Start();
        }

        private void InitPb()
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    bitmap.SetPixel(i, j, Color.Coral);
                }
            }
            pictureBox.Image = bitmap;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            ball.Y += 1;
            DrawBall();
        }

        private void DrawBall()
        {
            var startX = ball.X - ball.Radius;
            var startY = ball.Y - ball.Radius;
            for (int i = startX; i < ball.X + ball.Radius; i++)
            {
                for (int j = startY; j < ball.Y + ball.Radius; j++)
                {
                    if (i < 0 || i >= bitmap.Width || j < 0 || j >= bitmap.Height)
                        continue;
                    //bitmap.SetPixel(i, j, Color.Black);
                    var x = i - startX - ball.Radius; var y = j - startY - ball.Radius;
                    if (x * x + y * y < ball.Radius * ball.Radius)
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                }
            }

            pictureBox.Image = bitmap;
        }


    }
}
