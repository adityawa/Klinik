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
using Klinik.Data;
using Klinik.Data.DataRepository;
using System.Configuration;
using MyMedia = System.Media;
using System.IO;

namespace Klinik.WinFormVoice
{

    public partial class Form1 : Form
    {
        private SpeechSynthesizer ss = new SpeechSynthesizer();
        private bool isSpeak = false;
        public Form1()
        {
            InitializeComponent();
            ss.Volume = 100;
            ss.Rate = 2;
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }
        private void GetWav(string Number)
        {
            using (MyMedia.SoundPlayer mySoundPlayer =
                   new MyMedia.SoundPlayer())
            {
                switch (Number)
                {
                    case "0":
                        mySoundPlayer.SoundLocation = new FileInfo("0.wav").FullName;
                        break;
                    case "1":
                        mySoundPlayer.SoundLocation = new FileInfo("1.wav").FullName;
                        break;
                    case "2":
                        mySoundPlayer.SoundLocation = new FileInfo("2.wav").FullName;
                        break;
                    case "3":
                        mySoundPlayer.SoundLocation = new FileInfo("3.wav").FullName;
                        break;
                    case "4":
                        mySoundPlayer.SoundLocation = new FileInfo("4.wav").FullName;
                        break;
                    case "5":
                        mySoundPlayer.SoundLocation = new FileInfo("5.wav").FullName;
                        break;
                    case "6":
                        mySoundPlayer.SoundLocation = new FileInfo("6.wav").FullName;
                        break;
                    case "7":
                        mySoundPlayer.SoundLocation = new FileInfo("7.wav").FullName;
                        break;
                    case "8":
                        mySoundPlayer.SoundLocation = new FileInfo("8.wav").FullName;
                        break;
                    case "9":
                        mySoundPlayer.SoundLocation = new FileInfo("9.wav").FullName;
                        break;
                    case "B":
                        mySoundPlayer.SoundLocation = new FileInfo("B.wav").FullName;
                        break;
                    case "M":
                        mySoundPlayer.SoundLocation = new FileInfo( "M.wav").FullName;
                        break;
                }
                mySoundPlayer.Play();
                System.Threading.Thread.Sleep(1500);
            }
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            #region "Run Voice"
           
            string _sortNumbCd = string.Empty;
            int _poliId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PoliID"].ToString());
            using (KlinikDBEntities context = new KlinikDBEntities())
            {
                try
                {
                    var qry = context.PanggilanPolis.Where(x => x.PoliID == _poliId).OrderBy(x => x.SortNumber).FirstOrDefault();
                    if (qry != null && isSpeak==false)
                    {
                        isSpeak = true;
                        richTxtNo.Text = qry.QueueCode;

                        
                        char[] arrays = richTxtNo.Text.ToCharArray();
                        foreach (char c in arrays)
                        {
                            if (c != '-')
                            {
                                string strC = c.ToString();
                                GetWav(strC);
                            }

                        }
                       

                        var toBeDel = context.PanggilanPolis.SingleOrDefault(x => x.Id == qry.Id);
                        if (toBeDel != null)
                        {
                            context.PanggilanPolis.Remove(toBeDel);
                            context.SaveChanges();
                        }

                        isSpeak = false;
                      
                    }
                }
                catch (Exception ex)
                {
                    isSpeak = false;
                    throw new Exception(ex.Message);
                }
                #endregion




            }
        }

        private void RichTxtNo_TextChanged(object sender, EventArgs e)
        {
           
        }
    }
}
