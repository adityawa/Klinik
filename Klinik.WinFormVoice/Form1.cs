using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace Klinik.WinFormVoice
{
   
    public partial class Form1 : Form
    {
        private SpeechSynthesizer ss = new SpeechSynthesizer();
        public Form1()
        {
            InitializeComponent();
            ss.Volume = 80;
            ss.Rate = 2;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ss.SpeakAsync(richTextBox1.Text);
        }
    }
}
