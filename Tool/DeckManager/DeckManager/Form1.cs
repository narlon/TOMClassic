using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using ConfigDatas;

namespace DeckManager
{
    public partial class Form1 : Form
    {
        private CardDeck deck;
        private string path;

        private List<int> cards = new List<int>();
        private bool isDirty = true;
        private Bitmap cacheImage;

        private int leftSelectIndex = -1;

        public Form1(string path)
        {
            AllowDrop = true;
            InitializeComponent();
            this.path = path;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string version = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.ExecutablePath).FileVersion;
            Text = string.Format("��Ƭ��֯�� v{0}", version);

            if (path != "null")
                LoadFromFile(path);

            ConfigDatas.ConfigData.LoadData();
            comboBoxCatalog.SelectedIndex = 0;
            ChangeCards();
        }

        private void LoadFromFile(string txt)
        {
            path = txt;
            try
            {
                deck = new CardDeck();
                deck.Load(txt);
            }
            catch (Exception e)
            {
                MessageBox.Show("������ļ���ʽ"+e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var wid = panel1.Width/3;
            var het = panel1.Height / 10;
            Font ft = new Font("����", 9);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (deck != null)
                    {
                        var cardData = deck.GetCardId(j * 3 + i);
                        if (cardData.Id > 0)
                        {
                            DrawCardImg(e.Graphics, cardData.Id, ft, i * wid, j * het, wid, het);
                        }
                        else
                        {
                            e.Graphics.DrawString(cardData.Type, ft, Brushes.White, i * wid, j * het);
                        }
                    }
                }
            }
            if (leftSelectIndex >= 0)
            {
                int nx = leftSelectIndex%3;
                int ny = leftSelectIndex/3;
                Pen p = new Pen(Color.LightGreen, 5);
                e.Graphics.DrawRectangle(p, nx*wid, ny*het, wid, het);
                p.Dispose();
            }
            ft.Dispose();
        }

        private static void DrawCardImg(Graphics g, int cardId, Font ft, int x, int y, int wid, int het)
        {
            var img = ImageCache.GetImage(cardId);
            g.DrawImage(img, x, y, wid, het);
            var cardConfig = CardConfigManager.GetCardConfig(cardId);
            var brush = new SolidBrush(Color.FromName(HSTypes.I2QualityColor((int) cardConfig.Quality)));
            g.DrawString(cardConfig.Name, ft, Brushes.LightBlue, x + 4, y + het - 16);
            g.DrawString(cardConfig.Name, ft, brush, x+3, y + het - 17);
            brush.Dispose();

            Font ft2 = new Font("����", 6);
            g.DrawString(("�����������").Substring(10 - cardConfig.Star), ft2, Brushes.Yellow, x + 1, y + 3);
            ft2.Dispose();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            int index = 0;
            int wid = 60;
            int heg = 60;
            int xCount = panel2.Width / wid;
            if (isDirty)
            {
                panel2.Height = (cards.Count / xCount + 1) * wid;
                cacheImage = new Bitmap(panel2.Width, panel2.Height);

                Graphics g = Graphics.FromImage(cacheImage);
                Font ft = new Font("����", 9);
                foreach (var cardId in cards)
                {
                    DrawCardImg(g, cardId, ft, (index % xCount) * wid, (index / xCount) * heg, wid, heg);
                    index++;
                }
                ft.Dispose();
                isDirty = false;
            }

            if (cacheImage != null)
            {
                e.Graphics.DrawImage(cacheImage, 0, 0, cacheImage.Width, cacheImage.Height);
            }
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
            panel1.Invalidate();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            isDirty = true;
            panel2.Invalidate();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            leftSelectIndex = -1;
            var wid = panel1.Width / 3;
            var het = panel1.Height / 10;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (e.X >= i*wid && e.X <= (i + 1)*wid && e.Y >= het*j && e.Y <= het*(j + 1))
                    {
                        leftSelectIndex = 3*j + i;
                        panel1.Invalidate();
                        break;
                    }
                }
            }
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void panel2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int wid = 60;
            int het = 60;
            int xCount = panel2.Width / wid;
            if (e.X > 0 && e.X < wid*xCount)
            {
                var nowIndex = e.X/wid + e.Y/het*xCount;
                if (leftSelectIndex >= 0 && cards.Count > nowIndex)
                {
                    deck.Replace(leftSelectIndex, cards[nowIndex]);
                    panel1.Invalidate();
                    return;
                }
            }
        }


        private void comboBoxCatalog_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxValue.Items.Clear();
            var type = comboBoxCatalog.SelectedItem.ToString();
            switch (type)
            {
                case "����":
                    comboBoxValue.Items.AddRange(new object[] { "ȫ��", "Yellow|����", "Red|����", "DodgerBlue|����" }); break;
                case "-ϸ��":
                    comboBoxValue.Items.AddRange(new object[] { "ȫ��", "��ħ","��е","����","����","��","��",
                        "����","����","����","����","Ұ��","��","Ԫ��","ֲ��","�ؾ�","ʯ��",
                        "Red|����","Red|����","Red|����", "Red|��Ʒ",
                        "DodgerBlue|���巨��","DodgerBlue|Ⱥ�巨��","DodgerBlue|��������","DodgerBlue|���α仯" }); break;
                case "Ʒ��":
                    comboBoxValue.Items.AddRange(new object[] { "ȫ��", "��ͨ", "Green|����", "DodgerBlue|����", "Violet|ʷʫ", "Orange|��˵" }); break;
                case "�Ǽ�":
                    comboBoxValue.Items.AddRange(new object[] { "ȫ��", "��", "���", "����", "�����", "������", "�������", "Gold|��x7" }); break;
                case "ְҵ":
                    comboBoxValue.Items.Add("ȫ��");
                    foreach (var configData in ConfigData.JobDict.Values)
                    {
                        if (!configData.IsSpecial)
                            comboBoxValue.Items.Add(configData.Color + "|" + configData.Name);
                    }
                    break;
                case "Ԫ��":
                    comboBoxValue.Items.AddRange(new object[] { "ȫ��", "��", "Aqua|ˮ", "Green|��", "Red|��", "Peru|��", "Gold|��", "DimGray|��" }); break;
                case "��ǩ":
                    comboBoxValue.Items.AddRange(new object[] { "ȫ��", "����", "Red|ֱ��", "��Χ", "״̬", "Gold|����", "����", "Aqua|ħ��", "����", "�ٻ�", "����" }); break;
            }
            comboBoxValue.SelectedIndex = 0;
        }

        private int filterLevel = 0;
        private int filterQual = -1;
        private int filterType = -1;
        private string filterTypeSub = "ȫ��";
        private int filterJob = -1;
        private int filterEle = -1;
        private string filterRemark = "ȫ��";
        private void buttonOk_Click(object sender, EventArgs e)
        {
            filterLevel = 0;
            filterQual = -1;
            filterType = -1;
            filterTypeSub = "ȫ��";
            filterJob = -1;
            filterEle = -1;
            filterRemark = "ȫ��";

            var type = comboBoxCatalog.SelectedItem.ToString();
            switch (type)
            {
                case "����":
                    filterType = comboBoxValue.SelectedIndex - 1; break;
                case "-ϸ��":
                    filterTypeSub = comboBoxValue.TargetText; break;
                case "�Ǽ�":
                    filterLevel = comboBoxValue.SelectedIndex; break;
                case "ְҵ":
                    foreach (var configData in ConfigData.JobDict.Values)
                    {
                        if (configData.Name == comboBoxValue.TargetText)
                            filterJob = configData.Id;
                    }
                    break;
                case "��ǩ":
                    filterRemark = comboBoxValue.TargetText; break;
                case "Ʒ��":
                    filterQual = comboBoxValue.SelectedIndex - 1; break;
                case "Ԫ��":
                    filterEle = comboBoxValue.SelectedIndex - 1; break;
            }
            ChangeCards();
        }

        private void ChangeCards()
        {
            cards.Clear();

            List<CardConfigData> configData = new List<CardConfigData>();
            #region ����װ��

            if (filterType == -1 || filterType == 0)
            {
                foreach (var monsterConfig in ConfigData.MonsterDict.Values)
                {
                    if (monsterConfig.IsSpecial > 0)
                        continue;
                    if (filterJob != -1 && monsterConfig.JobId != filterJob)
                        continue;
                    if (filterLevel != 0 && monsterConfig.Star != filterLevel)
                        continue;
                    if (filterQual != -1 && monsterConfig.Quality != filterQual)
                        continue;
                    if (filterEle != -1 && monsterConfig.Attr != filterEle)
                        continue;
                    if (filterRemark != "ȫ��" && (string.IsNullOrEmpty(monsterConfig.Remark) || !monsterConfig.Remark.Contains(filterRemark)))
                        continue;
                    if (filterTypeSub != "ȫ��" && HSTypes.I2CardTypeSub(monsterConfig.Type) != filterTypeSub)
                        continue;
                    var cardData = CardConfigManager.GetCardConfig(monsterConfig.Id);
                    configData.Add(cardData);
                }
            }
            if (filterType == -1 || filterType == 1)
            {
                foreach (var weaponConfig in ConfigData.WeaponDict.Values)
                {
                    if (weaponConfig.IsSpecial > 0)
                        continue;
                    if (filterJob != -1 && weaponConfig.JobId != filterJob)
                        continue;
                    if (filterLevel != 0 && weaponConfig.Star != filterLevel)
                        continue;
                    if (filterQual != -1 && weaponConfig.Quality != filterQual)
                        continue;
                    if (filterEle != -1 && weaponConfig.Attr != filterEle)
                        continue;
                    if (filterRemark != "ȫ��" && (string.IsNullOrEmpty(weaponConfig.Remark) || !weaponConfig.Remark.Contains(filterRemark)))
                        continue;
                    if (filterTypeSub != "ȫ��" && HSTypes.I2CardTypeSub(weaponConfig.Type) != filterTypeSub)
                        continue;
                    var cardData = CardConfigManager.GetCardConfig(weaponConfig.Id);
                    configData.Add(cardData);
                }
            }
            if (filterType == -1 || filterType == 2)
            {
                foreach (var spellConfig in ConfigData.SpellDict.Values)
                {
                    if (spellConfig.IsSpecial > 0)
                        continue;
                    if (filterJob != -1 && spellConfig.JobId != filterJob)
                        continue;
                    if (filterLevel != 0 && spellConfig.Star != filterLevel)
                        continue;
                    if (filterQual != -1 && spellConfig.Quality != filterQual)
                        continue;
                    if (filterEle != -1 && spellConfig.Attr != filterEle)
                        continue;
                    if (filterRemark != "ȫ��" && (string.IsNullOrEmpty(spellConfig.Remark) || !spellConfig.Remark.Contains(filterRemark)))
                        continue;
                    if (filterTypeSub != "ȫ��" && HSTypes.I2CardTypeSub(spellConfig.Type) != filterTypeSub)
                        continue;
                    var cardData = CardConfigManager.GetCardConfig(spellConfig.Id);
                    configData.Add(cardData);
                }
            }

            #endregion

            configData.Sort(new CompareByCard());
            cards = configData.ConvertAll(card => card.Id);

            isDirty = true;
            panel2.Invalidate();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (deck != null)
            {
                deck.Save();
                panel1.Invalidate();
            }
        }
    }

    class CompareByCard : IComparer<CardConfigData>
    {
        #region IComparer<int> ��Ա

        public int Compare(CardConfigData x, CardConfigData y)
        {
            if (x.Star != y.Star)
            {
                return x.Star.CompareTo(y.Star);
            }

            if (x.Quality != y.Quality)
            {
                return x.Quality.CompareTo(y.Quality);
            }
            if (x.Attr != y.Attr)
            {
                return x.Attr.CompareTo(y.Attr);
            }

            return x.Id.CompareTo(y.Id);
        }

        #endregion
    }
}