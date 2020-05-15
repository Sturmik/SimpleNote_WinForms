using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotesForms
{
    public partial class CreateNote : Form
    {
        List<string> allAvaibleMusic;

        DateTime toNotify = new DateTime();
        List<NoteForm> toCheck;
        NoteForm workWith;

        public CreateNote(NoteForm workWith)
        {
            InitializeComponent();

            allAvaibleMusic = new List<string>();

            this.workWith = workWith;

            workWith.FontType = Font;
            workWith.FontColor = Color.Black;
            workWith.BackGroundColor = Color.White;

            textBoxFont.Text = Font.Name;

            if (!Directory.Exists("Music"))
                Directory.CreateDirectory("Music");

            allAvaibleMusic = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Music").ToList();

            string[] fileToAdd;

            for (int i = 0; i < allAvaibleMusic.Count; i++)
            {
                fileToAdd = allAvaibleMusic[i].Split('\\');
                comboBoxMusic.Items.Add(fileToAdd[fileToAdd.Length - 1]);
            }

            numericUpDownHours.Value = DateTime.Now.Hour;
            numericUpDownMinutes.Value = DateTime.Now.Minute;
            dateTimePickerToNotify.MinDate = DateTime.Now;

            textBoxName.MouseHover += (s, e) => { toolTipShow.SetToolTip(textBoxName, "Name field"); };
            dateTimePickerToNotify.MouseHover += (s, e) => { toolTipShow.SetToolTip(dateTimePickerToNotify, "Data to notify"); };
            textBoxFont.MouseHover += (s, e) => { toolTipShow.SetToolTip(textBoxFont, "Font. Click to change its type"); };
            comboBoxMusic.MouseHover += (s, e) => { toolTipShow.SetToolTip(comboBoxMusic, "Music\n Click: choose existing one\n Press button on the right to import new one"); };

            buttonCancel.Click += (s, e) => { workWith.Name = null; Close(); };
        }

        static public NoteForm CreateNewNote(List<NoteForm> toCheck)
        {
            NoteForm toCreate = new NoteForm();
            CreateNote notes = new CreateNote(toCreate);
            notes.toCheck = toCheck;

            notes.ShowDialog();

            if (toCreate.Name != null)
                return toCreate;
            return default;
        }

        /// <summary>
        /// Adds new note
        /// </summary>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxName.Text))
            {
                if (toCheck.Find(note => note.Name.ToLower() == textBoxName.Text.ToLower()) != null)
                {
                    MessageBox.Show(this, "This name already exist", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string minutes = numericUpDownMinutes.Value.ToString();
                if ((int)numericUpDownMinutes.Value < 10)
                    minutes = minutes.Insert(0, "0");
                string hours = numericUpDownHours.Value.ToString();
                if ((int)numericUpDownHours.Value < 10)
                    hours = hours.Insert(0, "0");

                if (DateTime.TryParse(dateTimePickerToNotify.Value.ToShortDateString() + " " + hours + ":" + minutes, out toNotify) == true)
                    if (toNotify.TimeOfDay < DateTime.Now.TimeOfDay && toNotify.Date == DateTime.Now.Date)
                        MessageBox.Show(this, "This note time is in the past", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        workWith.Name = textBoxName.Text;
                        workWith.ToNotify = toNotify;
                        workWith.Message = textBoxMessage.Text;

                        workWith.IsRelevant = true;

                        if (File.Exists(Directory.GetCurrentDirectory() + "\\Music" + "\\" + comboBoxMusic.Text))
                            workWith.Music = Directory.GetCurrentDirectory() + "\\Music" + "\\" + comboBoxMusic.Text;
                        Close();
                    }
            }
            else
                MessageBox.Show(this, "Name field must be filled", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// Font choice
        /// </summary>
        private void textBoxFont_MouseClick(object sender, MouseEventArgs e)
        {
            if (fontDialogToChoose.ShowDialog() == DialogResult.OK)
            {
                textBoxFont.Text = fontDialogToChoose.Font.Name;
                workWith.FontType = fontDialogToChoose.Font;
                textBoxMessage.Font = fontDialogToChoose.Font;
            }
        }
        /// <summary>
        /// Fore color
        /// </summary>
        private void pictureBoxForeColor_Click(object sender, EventArgs e)
        {
            if (colorDialogToChoose.ShowDialog() == DialogResult.OK)
            {
                pictureBoxForeColor.BackColor = colorDialogToChoose.Color;
                workWith.FontColor = colorDialogToChoose.Color;
                textBoxMessage.ForeColor = colorDialogToChoose.Color;
            }
        }
        /// <summary>
        /// Back color
        /// </summary>
        private void pictureBoxBackColor_Click(object sender, EventArgs e)
        {
            if (colorDialogToChoose.ShowDialog() == DialogResult.OK)
            {
                pictureBoxBackColor.BackColor = colorDialogToChoose.Color;
                workWith.BackGroundColor = colorDialogToChoose.Color;
                textBoxMessage.BackColor = colorDialogToChoose.Color;
            }
        }

        /// <summary>
        /// Changes music
        /// </summary>
        private void comboBoxMusic_SelectedValueChanged(object sender, EventArgs e)
        {
            workWith.Music = comboBoxMusic.SelectedItem.ToString();
        }
        /// <summary>
        /// Adds new music
        /// </summary>
        private void buttonAddMusic_Click(object sender, EventArgs e)
        {
            if (openFileDialogMusic.ShowDialog() == DialogResult.OK)
            {
                string[] check = openFileDialogMusic.FileName.Split('\\');

                if (!File.Exists(Directory.GetCurrentDirectory() + "\\Music" + "\\" + check[check.Length - 1]))
                {
                    File.Copy(openFileDialogMusic.FileName, Directory.GetCurrentDirectory() + "\\Music" + "\\" + check[check.Length - 1]);
                    comboBoxMusic.Items.Add(check[check.Length - 1]);
                    comboBoxMusic.Text = check[check.Length - 1];
                }
                else
                    comboBoxMusic.Text = check[check.Length - 1];
            }
        }
        /// <summary>
        /// Exits the application
        /// </summary>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
