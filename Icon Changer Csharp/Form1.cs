using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CISCO_ICON_CHANGER.Properties;
using System.Diagnostics;
using System.Net;
using System.Collections.Specialized;
using DSStdInstallerCompiler;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string OpenFileDialogsPath(bool Exe)
        {
            Settings.Default.Reload();
            using (OpenFileDialog O1 = new OpenFileDialog())
            {
                if (Exe == true)
                {
                    O1.Filter = "Exe Files (*.exe)|*.exe";
                    if (File.Exists(Settings.Default.PathExe))
                    O1.InitialDirectory = Path.GetDirectoryName(Settings.Default.PathExe);

                }
                else
                {
                    O1.Filter = "Icon Files (*.ico)|*.ico";
                    if(File.Exists(Settings.Default.PathIcon))
                    O1.InitialDirectory = Path.GetDirectoryName(Settings.Default.PathIcon);

                }
               
                if (O1.ShowDialog() == DialogResult.OK)
                {
                    if (Exe == true)
                        Settings.Default.PathExe = O1.FileName;
                    else
                        Settings.Default.PathIcon = O1.FileName;

                    Settings.Default.Save();
                    return O1.FileName;
                }
                else
                {
                    if (Exe == false)
                        return "Choose icon...";
                    else
                        return "Choose Exe...";
                }
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
          
        }
        private void textBox2_Click(object sender, EventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            if (textBox1.Text == "Choose Exe..."||textBox1.Text=="")
            {
                MessageBox.Show("Please Choose Exe File ...", "DSG ICON CHANGER", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                if (textBox2.Text == "Choose icon..." || textBox2.Text == "")
                {
                    MessageBox.Show("Please Choose Icon  ...", "DSG ICON CHANGER", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Because Method Object Want A Progressbar i add an empty
                    // progressBar you can make your own!
                      ProgressBar prg = new ProgressBar();
                     IconInjector.InjectIcon(textBox1.Text, textBox2.Text,prg);
                    label2.Text = "CHANGED [ " + Path.GetFileName(textBox1.Text) + " ] SeccussFully! ";
                 
                }
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            textBox1.Text = OpenFileDialogsPath(true);
          
        }

        private void button4_Click(object sender, EventArgs e)
        {
      
            textBox2.Text = OpenFileDialogsPath(false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
           
          
        }
    }
}
