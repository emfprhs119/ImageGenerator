namespace ImageGenerator
{
    partial class ResizeablePanel
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.폰트ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.제거ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.폰트ToolStripMenuItem,
            this.제거ToolStripMenuItem,
            this.toolStripTextBox1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(161, 73);
            // 
            // 폰트ToolStripMenuItem
            // 
            this.폰트ToolStripMenuItem.Name = "폰트ToolStripMenuItem";
            this.폰트ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.폰트ToolStripMenuItem.Text = "폰트";
            // 
            // 제거ToolStripMenuItem
            // 
            this.제거ToolStripMenuItem.Name = "제거ToolStripMenuItem";
            this.제거ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.제거ToolStripMenuItem.Text = "제거";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox1.Text = "RegEx";
            // 
            // ResizeablePanel
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 폰트ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 제거ToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
    }
}
