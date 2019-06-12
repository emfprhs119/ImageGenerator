using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessor
{
    class MyDialog
    {
        public static DialogResult ShowMessageBox(string title, string content)
        {
            Form form = new Form();
            Label label = new Label();
            Button buttonOk = new Button();

            form.ClientSize = new Size(300, 100);
            form.Controls.AddRange(new Control[] { label, buttonOk });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.AcceptButton = buttonOk;
            form.Text = title;
            label.Text = content;
            buttonOk.Text = "확인";

            buttonOk.DialogResult = DialogResult.OK;

            label.SetBounds(17, 17, 300, 20);
            buttonOk.SetBounds(135, 70, 70, 20);

            DialogResult dialogResult = form.ShowDialog();

            return dialogResult;
        }
        public static DialogResult ShowInputBox(string title, string content, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.ClientSize = new Size(300, 100);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            form.Text = title;
            label.Text = content;
            textBox.Text = value;
            buttonOk.Text = "확인";
            buttonCancel.Text = "취소";

            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(17, 17, 100, 20);
            textBox.SetBounds(17, 40, 220, 20);
            buttonOk.SetBounds(135, 70, 70, 20);
            buttonCancel.SetBounds(215, 70, 70, 20);

            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;
            return dialogResult;
        }
        public static void ShowImageBox(Image image)
        {
            if (image == null)
                return;
            Form form = new Form();
            PictureBox pictureBox = new PictureBox();
            form.ClientSize = image.Size;
            pictureBox.Size = image.Size;
            pictureBox.Image = image;
            form.Controls.Add(pictureBox);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.ShowDialog();
        }
    }
}
