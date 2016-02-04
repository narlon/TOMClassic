﻿using System;
using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms.Items
{
    internal class DeckCardRegion
    {
        internal delegate void InvalidateRegion();

        private const int cardWidth = 68;
        private const int cardHeight = 88;
        private int xCount, yCount;
        public int CardCount { get; private set; }
        public int CardTotalCount { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public InvalidateRegion Invalidate;

        private Bitmap tempImage;
        private bool isDirty = true;
        private Dictionary<int, string> cardAttr = new Dictionary<int, string>();
        public int Page { get; private set; }
        private int tar = -1;
        private DeckCard[] dcards;

        public DeckCardRegion(int x, int y, int width, int height)
        {
            xCount = width / cardWidth;
            yCount = height / cardHeight;
            CardCount = xCount * yCount;

            X = x;
            Y = y;
            Width = width;
            Height = height;

            tempImage = new Bitmap(width, height);
        }

        private void RefreshDict()
        {
            cardAttr.Clear();
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                AddCardAttr(UserProfile.InfoCard.SelectedDeck.GetCardAt(i), "D");
            }
            foreach (int cid in UserProfile.InfoCard.Newcards)
            {
                AddCardAttr(cid, "N");
            }
        }

        private void AddCardAttr(int cardid, string mark)
        {
            if (cardAttr.ContainsKey(cardid))
            {
                cardAttr[cardid] += mark;
            }
            else
            {
                cardAttr.Add(cardid, mark);
            }
        }

        private string GetCardAttr(int cardid)
        {
            if (!cardAttr.ContainsKey(cardid))
                return "";

            return cardAttr[cardid];
        }

        public void ChangeDeck(DeckCard[] decks)
        {
            Page = 0;
            tar = -1;
            isDirty = true;
            dcards = decks;
            Array.Sort(dcards, new DeckSelectCardRegion.CompareDeckCardByStar());
            CardTotalCount = decks.Length;
        }

        public DeckCard GetTargetCard()
        {
            if (tar >= 0 && tar < dcards.Length)
            {
                return dcards[tar];
            }
            return null;
        }

        public void ClearCard()
        {
            dcards[tar] = new DeckCard(0, 0, 0);
        }

        public void MenuRefresh()
        {
            isDirty = true;
        }

        public bool PrePage()
        {
            Page--;
            if (Page < 0)
            {
                Page++;
                return false;
            }
            tar = Page*CardCount;
            isDirty = true;
            Invalidate();
            return true;
        }

        public bool NextPage()
        {
            Page++;
            if (Page > (dcards.Length - 1) / CardCount)
            {
                Page--;
                return false;
            }
            tar = Page * CardCount;
            isDirty = true;
            Invalidate();
            return true;
        }

        private void DrawOnDeckView(Graphics g, DeckCard card, int x, int y, bool isSelected, string attr)
        {
            CardAssistant.DrawBase(g, card.BaseId, x, y, cardWidth, cardHeight);
            if (isSelected)
            {
                var brushes = new SolidBrush(Color.FromArgb(130, Color.Yellow));
                g.FillRectangle(brushes, x, y, cardWidth, cardHeight);
                brushes.Dispose();
            }

            if (card.BaseId <= 0)
            {
                return;
            }

            var cardConfigData = CardConfigManager.GetCardConfig(card.BaseId);
            Font font = new Font("宋体", 5*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString(("★★★★★★★★★★").Substring(10 - cardConfigData.Star), font, Brushes.Yellow, x + 3, y + 3);
            font.Dispose();

            var quality = cardConfigData.Quality + 1;
            g.DrawImage(HSIcons.GetIconsByEName("gem" + quality), x + cardWidth / 2 - 8, y + cardHeight - 20, 16, 16);


            //  g.FillPie(Brushes.Gray, x + 5, y + cardHeight - 30, 20, 20, 0, 360);
            //  g.FillPie(Brushes.GreenYellow, x + 5, y + cardHeight - 30, 20, 20, 0, card.Exp * 360 / ExpTree.GetNextRequiredCard(card.Level));

            if (attr.Contains("D"))
            {
                Image mark = PicLoader.Read("System", "MarkSelect.PNG");
                g.DrawImage(mark, x, y, cardWidth, cardHeight);
                mark.Dispose();
            }
            if (attr.Contains("N"))
            {
                Image mark = PicLoader.Read("System", "MarkNew.PNG");
                g.DrawImage(mark, x, y, 30, 30);
                mark.Dispose();
            }

            g.FillEllipse(Brushes.Black, x + 3, y + 4 + cardHeight - 26,16,16);
            Font fontsong = new Font("宋体", 9*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            string text = string.Format("{0:00}", card.Level);
            g.DrawString(text, fontsong, Brushes.Yellow, x + 4, y + 4 + cardHeight-23);
            fontsong.Dispose();
        }

        public void CheckMouseMove(int mousex, int mousey)
        {
            int truex = mousex - X;
            int truey = mousey - Y;
            if (truex > 0 && truex < xCount * cardWidth && truey > 0 && truey < yCount * cardHeight)
            {
                int temp = truex / cardWidth + truey / cardHeight * xCount + CardCount * Page;
                if (temp != tar)
                {
                    if (temp < dcards.Length)
                    {
                        tar = temp;
                    }
                    else
                    {
                        tar = -1;
                    }
                    if (Invalidate != null)
                    {
                        Invalidate();
                    }
                }
            }
            else
            {
                if (tar != -1)
                {
                    tar = -1;
                    if (Invalidate != null)
                    {
                        Invalidate();
                    }
                }
            }
        }

        public void Draw(Graphics eg)
        {
            int pages = dcards.Length / CardCount + 1;
            int cardLimit = (Page != pages - 1) ? CardCount : (dcards.Length % CardCount);
            int former = CardCount * Page + 1;

            if (isDirty)
            {
                RefreshDict();
                tempImage.Dispose();
                tempImage = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(tempImage);
                for (int i = former - 1; i < former + cardLimit - 1; i++)
                {
                    int ri = i % (xCount * yCount);
                    int x = (ri % xCount) * cardWidth;
                    int y = (ri / xCount) * cardHeight;
                    DrawOnDeckView(g, dcards[i], x, y, false, GetCardAttr(dcards[i].BaseId));
                }
                g.Dispose();

                isDirty = false;
            }
            eg.DrawImage(tempImage, X, Y);
            if (tar != -1)
            {
                int ri = tar % (xCount * yCount);
                int x = (ri % xCount) * cardWidth + X;
                int y = (ri / xCount) * cardHeight + Y;
                DrawOnDeckView(eg, dcards[tar], x, y, true, GetCardAttr(dcards[tar].BaseId));
            }
        }
    }
}