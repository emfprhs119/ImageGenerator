﻿using OpenCvSharp;
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
using Point = System.Drawing.Point;

namespace ImageProcessor
{
    public partial class Form1 : Form
    {
        ArrayList files;
        public Form1()
        {
            files = new ArrayList();
            InitializeComponent();
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = PointToScreen(e.Location);
            p.Offset(((Button)sender).Location);
            contextMenuStrip1.Show(p);
        }

        

        private void button6_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ((Button)sender).Text = dialog.SelectedPath;
                string[] tmpfiles = Directory.GetFiles(dialog.SelectedPath);
                string fileName = null;
                foreach (string tmpfileName in tmpfiles)
                {
                    if (tmpfileName.Substring(tmpfileName.Length - 3).Equals("jpg"))
                    {
                        files.Add(tmpfileName);
                        fileName = tmpfileName;
                    }
                }
                if (fileName != null)
                {
                    origin_Image = Image.FromFile(fileName);
                    pictureBox1.Image = Image.FromFile(fileName);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            funcListBox.Items.Remove(funcListBox.SelectedItem);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (funcListBox.SelectedIndex > 0)
            {
                funcListBox.Items.Insert(funcListBox.SelectedIndex-1, funcListBox.SelectedItem);
                funcListBox.Items.RemoveAt(funcListBox.SelectedIndex);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (funcListBox.SelectedIndex < funcListBox.Items.Count-1)
            {
                funcListBox.Items.Insert(funcListBox.SelectedIndex + 2, funcListBox.SelectedItem);
                funcListBox.Items.RemoveAt(funcListBox.SelectedIndex);
            }
        }
        Image origin_Image;
        private void Test_Click(object sender, EventArgs e)
        {
            if (origin_Image == null)
                return;
            Image image = (Image)origin_Image.Clone();
            IEnumerable<ImgFunc> funcs = funcListBox.Items.Cast<ImgFunc>();
            foreach (ImgFunc func in funcs)
            {
                image = func.Apply(image);
            }
            pictureBox1.Image = image;
        }
        private void Process_Click(object sender, EventArgs e)
        {
            if (files != null)
            {
                Regex regex = new Regex("\\\\[^\\\\]+jpg");
                int splitIndex = regex.Match((string)files[0]).Index;
                Directory.CreateDirectory(((string)files[0]).Substring(0, splitIndex) +"/process");
                foreach (string fileName in files)
                {
                    splitIndex = regex.Match(fileName).Index;
                    var dir = (fileName).Substring(0, splitIndex);
                    var fileOnly = (fileName).Substring(splitIndex+1);
                    Image image = Image.FromFile(fileName);
                    IEnumerable<ImgFunc> funcs = funcListBox.Items.Cast<ImgFunc>();
                    foreach (ImgFunc func in funcs)
                    {
                        image = func.Apply(image);
                    }

                    image.Save(dir+ "/process/"+fileOnly);
                }

                MyDialog.ShowMessageBox("이미지 변환 완료", "이미지 변환을 완료하였습니다.");
                return;
            }
            MyDialog.ShowMessageBox("이미지를 찾을 수 없습니다.", "이미지를 찾을 수 없습니다.");
        }

        private void skewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            funcListBox.Items.Add(new skewFunc());
        }

        private void overlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Image image = Image.FromFile(fileDialog.FileName);
                funcListBox.Items.Add(new overlayFunc(image));
            }
        }
        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Image image = Image.FromFile(fileDialog.FileName);
                funcListBox.Items.Add(new moveFunc(image));
            }
        }
        private void BlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            funcListBox.Items.Add(new blurFunc());
        }

        private void PerspectiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            funcListBox.Items.Add(new PerspectiveFunc());
        }
    }
    interface ImgFunc
    {
        Image Apply(Image image);
    }
    class PerspectiveFunc :ImgFunc
    {
        private Random random;
        int range;
        public PerspectiveFunc()
        {
            string rangeStr = "";
            MyDialog.ShowInputBox("Perspective range", "Perspective 픽셀범위 입력", ref rangeStr);
            range = int.Parse(rangeStr);
            this.random = new Random();
        }
        public Image Apply(Image image)
        {
            int mx = random.Next(range);
            int my = random.Next(range);
            Mat img = OpenCvSharp.Extensions.BitmapConverter.ToMat((Bitmap)image);
            Mat outImg = new Mat();

            Point2f[] srcPoint = new Point2f[4];
            Point2f[] dstPoint = new Point2f[4];

            srcPoint[0] = new Point2f(0.0f, 0.0f);
            srcPoint[1] = new Point2f(0.0f, image.Height);
            srcPoint[2] = new Point2f(image.Width, 0.0f);
            srcPoint[3] = new Point2f(image.Width, image.Height);
            //top
            dstPoint[0] = new Point2f(0.0f+mx, 0.0f+my);
            dstPoint[1] = new Point2f(0.0f, image.Height);
            dstPoint[2] = new Point2f(image.Width-mx, 0.0f+my);
            dstPoint[3] = new Point2f(image.Width, image.Height);


            using (Mat m = Cv2.GetPerspectiveTransform(srcPoint, dstPoint))
            {
                Cv2.WarpPerspective(img, outImg, m, new OpenCvSharp.Size(image.Width, image.Height));
            }
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(outImg);
        }
    }
    class blurFunc : ImgFunc
    {

        private Random random;
        int range;
        public blurFunc()
        {
            string rangeStr = "";
            MyDialog.ShowInputBox("Blur Kernal Size", "Kernal Size Range 입력 (3 이상)", ref rangeStr);
            range = int.Parse(rangeStr);
            this.random = new Random();
        }
        public Image Apply(Image image)
        {
            int blurSize = random.Next(range);
            blurSize = blurSize < 3 ? 3 : blurSize;
            blurSize = blurSize / 2 * 2 + 1;
            Mat outImg = new Mat();
            Mat img = OpenCvSharp.Extensions.BitmapConverter.ToMat((Bitmap)image);
            Cv2.Blur(img, outImg, new OpenCvSharp.Size(blurSize, blurSize));
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(outImg);
        }
    }
    class skewFunc : ImgFunc
    {
        
        private Random random;
        int range;
        public skewFunc()
        {
            string rangeStr = "";
            MyDialog.ShowInputBox("Skew 각도 입력", "각도", ref rangeStr);
            range =int.Parse(rangeStr);
            this.random = new Random();
        }
        public Image Mirror(Image image)
        {
            var destImage = (Image)new Bitmap(image.Width, image.Height);
            Point[] pointsOrigin = { new Point(image.Width, 0), new Point(0, 0), new Point(image.Width, image.Height) };
            var graphics = Graphics.FromImage(destImage);
            graphics.DrawImage(image, pointsOrigin);
            return destImage;
        }
        public Image Apply(Image image)
        {
            double rot = random.NextDouble()*range;
            bool rev = random.Next(2) == 1 ? true : false;
            PointF center = new PointF(0,0);
            PointF rt = RotatePoint(new PointF(image.Width,0), center, rot);
            PointF lb = RotatePoint(new PointF(0, image.Height), center, rot);
            PointF rb = RotatePoint(new PointF(image.Width, image.Height), center, rot);
            
            center = new PointF(center .X- lb.X, center.Y+0);
            rb = new PointF(rb.X - lb.X, rb.Y + 0);
            rt = new PointF(rt.X - lb.X, rt.Y + 0);
            PointF[] pointsOrigin = { new PointF(image.Width, 0), new PointF(0,0), new PointF(image.Width, image.Height) };
            //PointF[] pointsMinusOrigin = { new PointF(rt.X - lb.X, 0), new PointF(0, 0), new PointF(rt.X - lb.X, rb.Y) };

            PointF[] points = { rt,center, rb };

            Graphics graphics;
            if (rev)
            {
                graphics = Graphics.FromImage(image);
                graphics.DrawImage(image, pointsOrigin);
            }
            var destImage = (Image)new Bitmap((int)(rt.X-lb.X), (int)(rb.Y));
            //var one = (Image)new Bitmap((int)(rt.X - lb.X), (int)(rb.Y));
            graphics = Graphics.FromImage(destImage);
            graphics.DrawImage(image, points);
            if (rev)
            {
                return destImage;
            }else
            return Mirror(destImage);
        }
        static PointF RotatePoint(PointF pointToRotate, PointF centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new PointF
            {
                X =
                    (float)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (float)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
    class overlayFunc : ImgFunc
    {
        private Image image;
        private Random random;
        public overlayFunc(Image image)
        {
            this.image = image;
            this.random = new Random();
        }

        public Image Apply(Image image)
        {
            int x = random.Next(this.image.Width + image.Width)- this.image.Width;
            int y = random.Next(this.image.Height + image.Height)- this.image.Height;
            var destImage = (Image)image.Clone();
            var graphics = Graphics.FromImage(destImage);
            graphics.DrawImage(this.image, new Rectangle(x, y, this.image.Width, this.image.Height), 0, 0, this.image.Width, this.image.Height, GraphicsUnit.Pixel); ;
            return destImage;
        }
    }
    class moveFunc : ImgFunc
    {
        private Image image;
        private Random random;
        public moveFunc(Image image)
        {
            this.image = image;
            this.random = new Random();
        }

        public Image Apply(Image image)
        {
            if (this.image.Width > image.Width && this.image.Height > image.Height) {
                int x = random.Next(this.image.Width - image.Width);
                int y = random.Next(this.image.Height - image.Height);
                var destImage = (Image)this.image.Clone();
                var graphics = Graphics.FromImage(destImage);
                graphics.DrawImage(image, new Rectangle(x, y, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel); ;
                return destImage;
            }else
                return image;
        }
    }


}
