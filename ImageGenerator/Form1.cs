using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ImageGenerator
{
    public partial class Form1 : Form
    {
        private Stack<ResizeablePanel> initFieldStack;
        private LinkedList<ResizeablePanel> fieldList;
        Point tempPoint;
        public Form1()
        {
            initFieldStack = new Stack<ResizeablePanel>();
            fieldList = new LinkedList<ResizeablePanel>();
            
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                ResizeablePanel resizeablePanel = new ResizeablePanel(initFieldStack, fieldList);
                initFieldStack.Push(resizeablePanel);
                this.pictureBox1.Controls.Add(resizeablePanel);
            }
            foreach (ResizeablePanel penal in initFieldStack){
                penal.setPictureBox1(pictureBox1);
            }
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "jpg|*.jpg";
            openFileDialog1.DefaultExt = ".jpg";
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private Tuple<Image,string> GetGenerateImage()
        {
            //Image image = (Image)pictureBox1.Image.Clone();
            Bitmap b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(b);
            //g.FillRectangle(Brushes.Red, 0, 0, 200, 10);
            Image image = pictureBox1.Image;

            g.DrawImage(ResizeImageFixRatio(image, pictureBox1.Width, pictureBox1.Height),new Point(0,0));
            string rt = "";
            string tmp;
            foreach (ResizeablePanel field in fieldList) {
                tmp = field.rxrdg.Next();
                g.DrawString(tmp, field.Font, Brushes.Black, field.Location);
                rt += "\t" + tmp;
            }
            return new Tuple<Image, string>(b,rt);

        }
        public static Bitmap ResizeImageFixRatio(Image image, int width, int height)
        {
            float ratio = 1;
            if (image.Height/(float)image.Width < height / (float)width)
            {
                ratio = width / (float)image.Width;
            }
            else
            {
                ratio = height / (float)image.Height;
            }
            var destRect = new Rectangle((width- (int)(image.Width * ratio))/2, (height - (int)(image.Height * ratio)) / 2, (int)(image.Width* ratio), (int)(image.Height*ratio));
            var destImage = new Bitmap(width, height);

            //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.Clamp);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {

                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                pictureBox1.Size = pictureBox1.Image.Size;
                ResizePicture();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            
            if (result == DialogResult.OK)
                textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tempPoint = e.Location;
                contextMenuStrip1.Show(PointToScreen(e.Location));
            };
        }

        private void 필드추가ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (initFieldStack.Count > 0)
            {
                ResizeablePanel panel = initFieldStack.Pop();
                panel.Visible = true;
                fieldList.AddLast(panel);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MyDialog.ShowImageBox(GetGenerateImage().Item1);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizePicture();
        }
        private void ResizePicture()
        {
            if (pictureBox1.Image != null)
            {
                float ratio = 1;
                if (pictureBox1.Image.Height / (float)pictureBox1.Image.Width < panel3.Size.Height / (float)panel3.Size.Width)
                {
                    ratio = (float)panel3.Size.Width / pictureBox1.Image.Width;
                }
                else
                {
                    ratio = (float)panel3.Size.Height / pictureBox1.Image.Height;
                }
                pictureBox1.Size = new Size((int)(pictureBox1.Image.Size.Width * ratio), (int)(pictureBox1.Image.Size.Height * ratio));
            }
            else
            {
                pictureBox1.Size = panel3.Size;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || pictureBox1.Image == null)
            {
                return;
            }
            string str = "";
            DialogResult result = MyDialog.ShowInputBox("이미지 수", "생성할 이미지 수", ref str);
            if (result.Equals(DialogResult.OK))
            {
                Regex regex = new Regex("[0-9]{1,}");
                try
                {
                    int count = Int32.Parse(regex.Match(str).Value);

                    using (StreamWriter outputFile = new StreamWriter(textBox1.Text + "/label.csv"))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Tuple<Image, string> tup = GetGenerateImage();
                            Image image = tup.Item1;
                            string imgName = textBox1.Text + "/image_" + String.Format("{0:000000}", i) + ".jpg";
                            image.Save(imgName);

                            outputFile.WriteLine(imgName + tup.Item2);
                        }
                    }
                    MyDialog.ShowMessageBox("완료", "(" + count + ") 이미지 생성 완료");
                }catch (System.FormatException)
                {

                }
            }
        }
    }
}
