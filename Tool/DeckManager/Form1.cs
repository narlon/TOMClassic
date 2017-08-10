using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace DeckManager
{
    public partial class Form1 : Form
    {
        CardDescript[] cd = new CardDescript[30];
        Image[] images = new Image[30];
        private string path;

        public Form1(string path)
        {
            AllowDrop = true;
            InitializeComponent();

            if (path != "null")
                LoadFromFile(path);
        }

        private void LoadFromFile(String txt)
        {
            path = txt;
            try
            {
                cd = CardDescript.MakeLoad(txt, 1);
                UpdateImages();
            }
            catch (Exception e)
            {
                MessageBox.Show("������ļ���ʽ"+e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateImages()
        {
            for (int i = 0; i < 30; i++)
            {
                int cardId = cd[i].Id;
                if (cardId == 0)
                {
                    images[i] = null;
                    continue;
                }
                string pathParent = "../../PicResource/";
                if (cardId < 52000000)
                    images[i] = Image.FromFile(String.Format("{0}Monsters/{1}.JPG", pathParent, cd[i].Id%1000000));
                else if (cardId < 53000000)
                    images[i] = Image.FromFile(String.Format("{0}Weapon/{1}.JPG", pathParent, cd[i].Id % 1000000));
                else
                    images[i] = Image.FromFile(String.Format("{0}Spell/{1}.JPG", pathParent, cd[i].Id % 1000000));
            }
            panel1.Invalidate();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (images[j * 6 + i] != null)
                    {
                        e.Graphics.DrawImage(images[j * 6 + i], i * 100, j * 100, 100, 100);
                    }
                    if (cd != null && !string.IsNullOrEmpty(cd[j * 6 + i].Tip))
                    {
                        Font ft = new Font("����", 9);
                        e.Graphics.DrawString(cd[j * 6 + i].Tip, ft, Brushes.White, i * 100, j * 100);
                        ft.Dispose();
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String version = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.ExecutablePath).FileVersion;
            Text = String.Format("��Ƭ��֯�� v{0}", version);
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", path);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            LoadFromFile(path);
        }
    }
}