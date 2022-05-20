using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PaletteApplier
{
    public partial class Form1 : Form
    {
        public const int SymbLimit = 60;
        public string ImgPath = null;
        public string PalPath = null;
        PaletteApplier pa = new PaletteApplier();
        Drawer dr;
        public Form1()
        {
            InitializeComponent();
            Clear();
            label1.TextChanged += LimitText;
            label2.TextChanged += LimitText;
        }

        public void Clear()
        {
            pa = new PaletteApplier();
            pictureBox1.CreateGraphics().Clear(this.BackColor);
            dr = new Drawer(pictureBox1.CreateGraphics());
            ImgPath = null;
            PalPath = null;
            label1.Text = "";
            label2.Text = "";
            button3.Enabled = false;
            button4.Enabled = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e) //Opening texidx
        {
            string path = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Choose a base image";
                openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF,*.png)|*.BMP;*.JPG;*.GIF;*.png|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
            }
            if (path == "")
            {
                return;
            }
            Image img = Image.FromFile(path);
            int d;
            if (checkBox1.Checked)
            {
                d = pa.SetColorfulImage(img);
                if (d == 1)
                {
                    MessageBox.Show("Error: There are too much\ncolors on this picture.");
                    return;
                }
            } else
            {
                d = pa.SetColorlessImage(img); //error if this is not Alpha8
                if (d == 1)
                {
                    MessageBox.Show("Error: Some of the pixels\nare not white.");
                    return;
                }
            }
            checkBox2.Checked = true;
            label1.Text = path;
            ImgPath = path;
            BothReady();
            DrawKnown();
        }

        void BothReady()
        {
            if (checkBox2.Checked && checkBox3.Checked)
            {
                button3.Enabled = true;
                button4.Enabled = false;
            }
        }

        void DrawKnown() //Drawing all loaded textures
        {
            pictureBox1.CreateGraphics().Clear(this.BackColor);
            dr = new Drawer(pictureBox1.CreateGraphics());
            if (checkBox2.Checked)
            {
                dr.DrawImage(Image.FromFile(ImgPath), 0);
            }
            if (checkBox3.Checked)
            {
                dr.DrawImage(Image.FromFile(PalPath), 1);
            }
            if (button4.Enabled)
            {
                dr.DrawImage((checkBox1.Checked) ? pa.GetColorlessImage() : pa.GetColorfulImage(), 2);
            }
        }

        private void button2_Click(object sender, EventArgs e) //Opening a palette image
        {
            string path = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Choose a palette image";
                openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF,*.png)|*.BMP;*.JPG;*.GIF;*.png|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
            }
            if (path == "")
            {
                return;
            }
            Image img = Image.FromFile(path);
            int d = pa.SetPalette(img);
            if (d == 1)
            {
                MessageBox.Show("Error: An invalid\npalette image.");
                return;
            }
            checkBox3.Checked = true;
            label2.Text = path;
            PalPath = path;
            BothReady();
            DrawKnown();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Clear();
        }

        private void LimitText(object sender, EventArgs e) //Cut the text for label
        {
            Label lb = (Label)sender;
            if (lb.Text.Length > SymbLimit)
            {
                lb.Text = lb.Text.Substring(0, SymbLimit - 3) + "...";
            }
        }

        private void button3_Click(object sender, EventArgs e) //Calculating an image
        {
            int d = checkBox1.Checked ? pa.CalculateColorlessImage(true) : pa.CalculateColorfulImage();
            if (d == 1)
            {
                MessageBox.Show("Error: The palette and/or\nbase image was not chosen.");
                return;
            }
            else if (d == 2)
            {
                MessageBox.Show("Error: Base image contains colors,\nthat do not exist in this palette.");
                return;
            }
            Image img = checkBox1.Checked ? pa.GetColorlessImage() : pa.GetColorfulImage();
            dr.DrawImage(img, 2);
            button4.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Image img = checkBox1.Checked ? pa.GetColorlessImage() : pa.GetColorfulImage();
            string path = "";
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.RestoreDirectory = true;
                sfd.Title = "Save Image As";
                sfd.Filter = "Image File (*.png)|*.png|No extention (*.*)|*.*";
                sfd.FilterIndex = 1;
                DialogResult dr = sfd.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    path = sfd.FileName;
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }
            img.Save(path, ImageFormat.Png);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DrawKnown();
        }
    }
}
