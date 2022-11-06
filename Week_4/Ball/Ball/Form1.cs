using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ball
{
    public partial class Form1 : Form
    {
        private Engine engine;
        public Form1()
        {
            InitializeComponent();
            InitPictureBox();
        }

        private void InitPictureBox()
        {
            engine = new Engine(this.pictureBox1); 
        }
    }
}
