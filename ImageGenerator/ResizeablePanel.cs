using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageGenerator
{
    public partial class ResizeablePanel : Label
    {
        ResizeablePanel target;
        private const int cGripSize = 15;
        private bool mResizeDragging;
        private bool mMoveDragging;
        private Point beforePoint;
        private Point mDragPos;
        private Point mMovePos;
        Stack<ResizeablePanel> initFieldStack;
        LinkedList<ResizeablePanel> fieldList;
        public PictureBox pictureBox1;
        string regex;
        public RegularExpressionDataGenerator.RegExpDataGenerator rxrdg;
        public ResizeablePanel( Stack<ResizeablePanel> initFieldStack, LinkedList<ResizeablePanel> fieldList)
        {
            this.initFieldStack = initFieldStack;
            this.fieldList = fieldList;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.BackColor = Color.Transparent;

            this.BorderStyle = BorderStyle.FixedSingle;
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(RegexToolStripMenuItem_Click);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(ClickEvent);

            contextMenuStrip2 = new ContextMenuStrip();
            폰트ToolStripMenuItem = new ToolStripMenuItem();
            폰트ToolStripMenuItem.Text = "폰트";
            ToolStripMenuItem 컬러ToolStripMenuItem = new ToolStripMenuItem();
            컬러ToolStripMenuItem.Text = "컬러";
            제거ToolStripMenuItem = new ToolStripMenuItem();
            제거ToolStripMenuItem.Text = "제거";
            ToolStripMenuItem RegexToolStripMenuItem = new ToolStripMenuItem();
            RegexToolStripMenuItem.Text = "Regex";
            폰트ToolStripMenuItem.Click += 폰트ToolStripMenuItem_Click;
            컬러ToolStripMenuItem.Click += 컬러ToolStripMenuItem_Click;
            RegexToolStripMenuItem.Click += RegexToolStripMenuItem_Click;
            제거ToolStripMenuItem.Click += 제거ToolStripMenuItem_Click;
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.폰트ToolStripMenuItem,컬러ToolStripMenuItem,RegexToolStripMenuItem,this.제거ToolStripMenuItem});
            initialize();
        }

        private void 컬러ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            DialogResult result = colorDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.ForeColor = colorDialog.Color;
            }
        }

        internal Brush getColor()
        {
            return new SolidBrush(ForeColor);
        }

        public void initialize()
        {
            this.Size = new System.Drawing.Size(100, 30);
            this.Font = new Font(FontFamily.GenericSerif, (int)(this.Size.Height * 0.5));
            this.Text = "Text";
            this.Location = new Point(1, 1);
            this.Visible = false;
            regex = "Text";
            rxrdg = new RegularExpressionDataGenerator.RegExpDataGenerator(regex);
            BringToFront();
        }
        private void 폰트ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            DialogResult result = fontDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.Font = fontDialog.Font;
                this.Font = new Font(this.Font.FontFamily, (int)(this.Size.Height * 0.5));
            }
        }

        private void RegexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyDialog.ShowInputBox("정규표현식", "Regular_expression",ref regex);
            rxrdg = new RegularExpressionDataGenerator.RegExpDataGenerator(regex);
            this.Text = rxrdg.Next();
        }

        public void setPictureBox1(PictureBox pictureBox1)
        {
            this.pictureBox1 = pictureBox1;
        }
        
        private void 제거ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initFieldStack.Push(target);

            fieldList.Remove(target);
            target.initialize();
        }
        private void ClickEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                target = (ResizeablePanel)sender;
                contextMenuStrip2.Show(PointToScreen(e.Location));
            }
        }
        private bool IsOnGrip(Point pos)
        {
            return pos.X >= this.ClientSize.Width - cGripSize &&
                   pos.Y >= this.ClientSize.Height - cGripSize;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            mResizeDragging = IsOnGrip(e.Location);
            mMoveDragging = !IsOnGrip(e.Location);
            beforePoint = PointToScreen(e.Location);
            beforePoint.Offset(-this.Location.X,-this.Location.Y);
            mDragPos = e.Location;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mResizeDragging = false;
            mMoveDragging = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mResizeDragging)
            {
                int tmpW = this.Width + e.X - mDragPos.X;
                int tmpH = this.Height + e.Y - mDragPos.Y;
                tmpW = tmpW < 20 ? 20 : tmpW;
                tmpH = tmpH < 20 ? 20 : tmpH;
                mDragPos = new Point(tmpW - this.Width + mDragPos.X, tmpH - this.Height + mDragPos.Y);
                this.Size = new Size(tmpW,tmpH);
                this.Font = new Font(this.Font.FontFamily, (int)(this.Size.Height*0.5));
            }else if (mMoveDragging)
            {
                mMovePos = PointToScreen(e.Location);
                mMovePos.Offset(-beforePoint.X, -beforePoint.Y);
                if (mMovePos.X < 0)
                    mMovePos.X = 0;
                if (mMovePos.X > pictureBox1.Width - Width/2)
                    mMovePos.X = pictureBox1.Width - Width/2;
                if (mMovePos.Y > pictureBox1.Height - Height/2)
                    mMovePos.Y = pictureBox1.Height - Height/2;
                if (mMovePos.Y < 0)
                    mMovePos.Y = 0;
                this.Location = mMovePos;
            }

            else if (IsOnGrip(e.Location)) this.Cursor = Cursors.SizeNWSE;
            else this.Cursor = Cursors.Default;
            base.OnMouseMove(e);
        }
    }
}
