using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Core.Interface;

namespace TaleofMonsters.Forms.Items.Regions
{
    internal class ImageRegion : SubVirtualRegion
    {
        protected ImageRegionCellType type;
        private readonly Image image;

        public ImageRegion(int id, int x, int y, int width, int height, ImageRegionCellType t, Image img)
            : base(id, x, y, width, height)
        {
            type = t;
            image = img;
            Scale = 1;
        }

        public override void Draw(Graphics g)
        {
            if (image != null)
            {
                if (Scale == 1)
                {
                    g.DrawImage(image, X, Y, Width, Height);
                }
                else
                {
                    int realWidth = (int)(Width*Scale);
                    int realHeight = (int)(Height * Scale);
                    g.DrawImage(image, X + (Width- realWidth)/2, Y + (Height- realHeight)/2, realWidth, realHeight);
                }
            }

            foreach (IRegionDecorator decorator in decorators)
            {
                decorator.Draw(g, X, Y, Width, Height);
            }
        }
        public ImageRegionCellType GetVType()
        {
            return type;
        }

        public float Scale { get; set; } //����ͼƬ����


        public override void ShowTip(ImageToolTip tooltip, Control form, int x, int y)
        {
            var regionType = GetVType();
            if (regionType == ImageRegionCellType.Gold)
            {
                string resStr = string.Format("�ƽ�:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Lumber)
            {
                string resStr = string.Format("ľ��:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Stone)
            {
                string resStr = string.Format("ʯ��:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Mercury)
            {
                string resStr = string.Format("ˮ��:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Carbuncle)
            {
                string resStr = string.Format("�챦ʯ:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Sulfur)
            {
                string resStr = string.Format("���:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Gem)
            {
                string resStr = string.Format("ˮ��:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Food)
            {
                string resStr = string.Format("ʳ��:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Health)
            {
                string resStr = string.Format("����:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Mental)
            {
                string resStr = string.Format("����:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Exp)
            {
                string resStr = string.Format("����ֵ:{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 100), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Str)
            {
                string resStr = string.Format("�������������ԣ�+{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 120), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Agi)
            {
                string resStr = string.Format("���ݣ��������ԣ�+{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 120), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Intl)
            {
                string resStr = string.Format("�ǻۣ��������ԣ�+{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 120), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Perc)
            {
                string resStr = string.Format("��֪���������ԣ�+{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 120), form, x, y);
            }
            else if (regionType == ImageRegionCellType.Endu)
            {
                string resStr = string.Format("�������������ԣ�+{0}", Parm);
                tooltip.Show(DrawTool.GetImageByString(resStr, 120), form, x, y);
            }
        }
    }

    internal enum ImageRegionCellType
    {
        None,
        Gold,
        Exp,
        Food,
        Health,
        Mental,
        Lumber,
        Stone,
        Mercury,
        Carbuncle,
        Sulfur,
        Gem,
        Str,
        Agi,
        Intl,
        Perc,
        Endu
    }
}
