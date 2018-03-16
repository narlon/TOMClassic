﻿using System;
using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Tools;
using TaleofMonsters.Core.Loader;

namespace TaleofMonsters.Tools
{
    internal static class ImageManager
    {
        internal class ImageItem
        {
            public Image Image;
            public int Time;

            public override string ToString()
            {
                return string.Format("{0}x{1} {2}", Image != null ? Image.Width : 0, Image != null ? Image.Height : 0, Time);
            }
        }

        private static Image nullImage;
        private static Dictionary<string, ImageItem> images = new Dictionary<string, ImageItem>();
        private static int lastCompressTime;
        private static int count;

        static ImageManager()
        {
            nullImage = PicLoader.Read("System", "Null.JPG");
        }

        public static bool HasImage(string path)
        {
            if (TimeTool.DateTimeToUnixTime(DateTime.Now) > lastCompressTime + 60 && count > 100)
            {
                Compress();
                lastCompressTime = TimeTool.DateTimeToUnixTime(DateTime.Now);
            }

            return images.ContainsKey(path) && images[path].Image != null;
        }

        public static Image GetImage(string path)
        {
            images[path].Time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            return images[path].Image ?? nullImage;
        }

        public static void AddImage(string path, Image img)
        {
            ImageItem item = new ImageItem();
            item.Image = img;
            item.Time = TimeTool.DateTimeToUnixTime(DateTime.Now);
            if (images.ContainsKey(path))
            {
                images[path] = item;
            }
            else
            {
                images.Add(path, item);     
            }
            count++;
        }

        public static void Compress()
        {
            int now = TimeTool.DateTimeToUnixTime(DateTime.Now);
            foreach (ImageItem item in images.Values)
            {
                if (item.Image!=null)
                {
                    int size = item.Image.Width*item.Image.Height;
                    int time = 60*10000/size;
                    if (item.Time < now - time)
                    {
                        item.Image.Dispose();
                        item.Image = null;
                        count--;
                    }
                }
            }
        }
    }
}