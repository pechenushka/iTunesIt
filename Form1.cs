using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iTunesIt
{
    public partial class MainForm : Form
    {
        private itunesit itunesit;

        public MainForm()
        {
            InitializeComponent();
            this.itunesit = itunesit.instance();
            this.init_buttons();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string label_text = "";
            label_text = this.itunesit.get_errors();
            toolStripStatusLabel1.Text = label_text;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите выйти ?", "Внимание !", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void оПрограммеToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AboutBox1 about_box = new AboutBox1();
            about_box.Show();
        }

        private void синхронизироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.itunesit.synchronize_my_library();
        }

        private void init_buttons()
        {
            if (this.itunesit.get_local_itunes_library_path() == false)
            {
                this.button1.Enabled = false;
            }
            else
            {
                this.button1.Enabled = true;
            }

            if (this.itunesit.get_local_files_returned() == true)
            {
                this.button2.Enabled = false;
            }
            else
            {
                this.button2.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
             * Заменяем нашими файлами
             */
            this.itunesit.replace_itunes_library();

            /*
             * Перерисовывем кнопки
             */
            this.init_buttons();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
             * Возвращаем локальную библиотеку на место
             */
            this.itunesit.return_local_library();
            this.init_buttons();
        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
