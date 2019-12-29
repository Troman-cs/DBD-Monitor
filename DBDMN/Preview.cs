using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBDMN
{
    public partial class Preview : Form
    {
        private static Form1 mainForm = null;

        public Preview(Form1 mainForm)
        {
            InitializeComponent();

            Preview.mainForm = mainForm;
        }

        private void Preview_Load(object sender, EventArgs e)
        {
            picPreview.Size = new Size(picPreview.Width, picPreview.Height);
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;

            setOpTop(true);
        }

        private void timerPreview_Tick(object sender, EventArgs e)
        {
            if(ScreenCapture.debugImageFromFile != null)
            {
                picPreview.Image = ScreenCapture.getScreenShot();
                return;
            }

            if (!ScreenCapture.haveGameHwnd() || !ScreenCapture.haveScreenShot())
                return;

            picPreview.Image = ScreenCapture.getScreenShot();
        }

        private void Preview_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Prevent this form from poping up again on its own
            Preview.mainForm.untickPreview();

            this.Hide();
            e.Cancel = true;
        }

        private void picPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Left = e.X + this.Left - MouseDownLocation.X;
                this.Top = e.Y + this.Top - MouseDownLocation.Y;
            }
        }

        int oldWidth = 0;
        int oldHeight = 0;
        



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkBox1.Checked;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void picPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            

            setOpTop(!toolStripMenuItem1.Checked);
            
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Preview.mainForm.untickPreview();
        }

        private Point MouseDownLocation;
        private void picPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
            }
        }

        private void Preview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
            }
        }

        private void Preview_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Left = e.X + this.Left - MouseDownLocation.X;
                this.Top = e.Y + this.Top - MouseDownLocation.Y;
            }
        }

        public void setOpTop(bool b)
        {
            this.TopMost = b;
            toolStripMenuItem1.Checked = b;
        }

        private void picPreview_MouseEnter( object sender, EventArgs e )
        {
        }

        private void picPreview_MouseLeave( object sender, EventArgs e )
        {
        }

        private void Preview_MouseEnter( object sender, EventArgs e )
        {
        }

        private void Preview_MouseLeave( object sender, EventArgs e )
        {
        }

        private void timer1_Tick( object sender, EventArgs e )
        {
        }

        private void Preview_Activated( object sender, EventArgs e )
        {
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
        }

        private void Preview_Enter( object sender, EventArgs e )
        {
        }

        private void Preview_Deactivate( object sender, EventArgs e )
        {
            this.FormBorderStyle = FormBorderStyle.None;
        }
    }
}
