using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace NotesForms
{
    public partial class NoteShow : Form
    {
        public NoteForm toChange;

        public NoteShow(NoteForm toShow)
        {
            InitializeComponent();

            Top = (int)SystemParameters.VirtualScreenHeight - (Bottom - Top) - 50;
            Left = (int)SystemParameters.VirtualScreenWidth - (Right - Left);

            toChange = toShow;

            textBoxMessage.ReadOnly = toShow.IsReadOnly;
            readonlyToolStripMenuItem.Checked = toShow.IsReadOnly;

            toolStripStatusLabelName.Text = toShow.Name;
            textBoxMessage.Text = toShow.Message;
            toolStripStatusLabelToNotify.Text = toShow.ToNotify.ToShortDateString() + " | " + toShow.ToNotify.ToShortTimeString();

            textBoxMessage.Font = toShow.FontType;
            textBoxMessage.ForeColor = toShow.FontColor;
            textBoxMessage.BackColor = toShow.BackGroundColor;
        }

        // Form movement
        Point movePoint;
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                movePoint = new Point(e.X, e.Y);
        }
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                Point deltaPos = new Point(e.X - movePoint.X, e.Y - movePoint.Y);

                Location = new Point(Location.X + deltaPos.X, Location.Y + deltaPos.Y);
            }
        }

        // Color change
        private void changeFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialogText.ShowDialog() == DialogResult.OK)
            {
                toChange.FontType = fontDialogText.Font;
                textBoxMessage.Font = fontDialogText.Font;
            }
        }
        private void changeFontColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialogText.ShowDialog() == DialogResult.OK)
            {
                toChange.FontColor = colorDialogText.Color;
                textBoxMessage.ForeColor = colorDialogText.Color;
            }
        }
        private void changeBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialogText.ShowDialog() == DialogResult.OK)
            {
                toChange.BackGroundColor = colorDialogText.Color;
                textBoxMessage.BackColor = colorDialogText.Color;
            }
        }

        // Text changed
        private void textBoxMessage_TextChanged(object sender, EventArgs e)
        {
            toChange.Message = textBoxMessage.Text;  
        }
        // Read-only value
        private void readonlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toChange.IsReadOnly = readonlyToolStripMenuItem.Checked;
            textBoxMessage.ReadOnly = readonlyToolStripMenuItem.Checked;
        }
        // Saves txt file in specific directory
        private void saveTextInFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogTo.ShowDialog() == DialogResult.OK)
                File.WriteAllText(saveFileDialogTo.FileName, textBoxMessage.Text, Encoding.Default);
        }
        // Loads txt file from specific directory
        private void loadTextFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogTo.ShowDialog() == DialogResult.OK)
                textBoxMessage.Text = File.ReadAllText(openFileDialogTo.FileName, Encoding.Default);
        }
        // Closes form
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
