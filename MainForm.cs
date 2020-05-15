using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NotesFomrs;

namespace NotesForms
{
    public partial class MainForm : Form
    {
        private MP3_Player notifySound;

        public List<NoteShow> noteForms;
        public List<NoteForm> notes;

        // Stored notes are the ones, which are showed, when time comes or user misses them
        public List<NoteForm> storedNotes;

        public MainForm()
        {
            InitializeComponent();

            timerNoteNotify.Start();

            notifySound = new MP3_Player();

            noteForms = new List<NoteShow>();
            notes = new List<NoteForm>();
            storedNotes = new List<NoteForm>();

            LoadData();

            notifyIconNotes.ShowBalloonTip(1000, "Simple Note Editor", "Double-click to open or hide note editor", ToolTipIcon.Info);
        }

        // Shows stored notes
        public void ShowNotify()
        {
            timerNoteNotify.Stop();
            for (int i = 0; i < storedNotes.Count; i++)
            {
                NoteCheck();
                NoteShow newForm = new NoteShow(storedNotes[i]);
                newForm.Name = storedNotes[i].Name;

                if (File.Exists(storedNotes[i].Music))
                {
                    notifySound.open(storedNotes[i].Music);
                    notifySound.play();
                }

                storedNotes[i].IsRelevant = false;

                AttentionForm check = new AttentionForm();
                check.ShowDialog();

                notifySound.stop();

                if (!noteForms.Exists(f => f.Name == newForm.Name))
                {
                    noteForms.Add(newForm);
                    newForm.Show();
                }
                else
                {
                    noteForms.Find((f => f.Name == newForm.Name)).BringToFront();
                }
            }

            RelevantCheck();
            timerNoteNotify.Start();
            storedNotes.Clear();
        }

        // Check relevant notify
        public void RelevantCheck()
        {
            bool timeCheck;

            foreach (ListViewItem lvw in listViewNotes.Items)
            {
                timeCheck = true;

                if (notes.Find(n => n.Name == lvw.Text).ToNotify < DateTime.Now)
                    timeCheck = false;
                if (notes.Find(n => n.Name == lvw.Text).ToNotify.Hour < DateTime.Now.Hour
                    && notes.Find(n => n.Name == lvw.Text).ToNotify.Minute < DateTime.Now.Minute
                    && notes.Find(n => n.Name == lvw.Text).ToNotify.Date < DateTime.Now.Date)
                    timeCheck = false;

                if (notes.Find(n => n.Name == lvw.Text).IsRelevant == false || timeCheck == false)
                    lvw.BackColor = Color.Gray;
            }
        }

        // Checks if form was already opened
        public void NoteCheck()
        {
            bool isExist;

            for (int i = 0; i < noteForms.Count; i++)
            {
                isExist = false;
                for (int j = 0; j < Application.OpenForms.Count; j++)
                    if (noteForms[i] == Application.OpenForms[j])
                    {
                        isExist = true;
                        break;
                    }
                if (isExist == false)
                    noteForms.Remove(noteForms[i]);
            }
        }

        // Shows items
        public void ItemsShow(List<NoteForm> itemsToShow)
        {
            for (int i = 0; i < itemsToShow.Count; i++)
            {
                listViewNotes.Items.Insert(0, itemsToShow[i].Name);
                listViewNotes.Items[0].SubItems.Add(itemsToShow[i].ToNotify.ToShortDateString() + " | " + itemsToShow[i].ToNotify.ToShortTimeString());
            }
        }

        // Button, which allows us to make new note 
        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            timerNoteNotify.Stop();

            NoteForm toAdd = CreateNote.CreateNewNote(notes);

            if (toAdd != default)
            {
                notes.Add(toAdd);
                listViewNotes.Items.Insert(0,toAdd.Name);
                listViewNotes.Items[0].SubItems.Add(toAdd.ToNotify.ToShortDateString() + " | " + toAdd.ToNotify.ToShortTimeString());
            }

            timerNoteNotify.Start();

            RelevantCheck();
        }

        // Deletes note
        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (listViewNotes.SelectedItems.Count > 0)
            {
                notes.Remove(notes.Find(not => not.Name == listViewNotes.SelectedItems[0].Text));

                for (int j = 0; j < Application.OpenForms.Count; j++)
                    if (Application.OpenForms[j].Name == listViewNotes.SelectedItems[0].Text)
                        Application.OpenForms[j].Close();

               noteForms.Remove(noteForms.Find(not => not.Name == listViewNotes.SelectedItems[0].Text));
               listViewNotes.Items.Remove(listViewNotes.SelectedItems[0]);
            }
        }

        // Refreshes the list
        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            listViewNotes.Items.Clear();
            ItemsShow(notes);
        }

        // Finds notes by specific data
        private void toolStripButtonLookFor_Click(object sender, EventArgs e)
        {
            SearchForm find = new SearchForm(listViewNotes, notes);
            find.ShowDialog(this);
        }

        // Opens note
        private void listViewNotes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewNotes.SelectedItems.Count > 0)
            {
                NoteCheck();

                NoteShow newForm = new NoteShow(notes.Find(n => n.Name == listViewNotes.SelectedItems[0].Text));
                newForm.Name = listViewNotes.SelectedItems[0].Text;
                if (!noteForms.Exists(f => f.Name == newForm.Name))
                {
                    noteForms.Add(newForm);
                    newForm.Show();
                }
                else
                    noteForms.Find((f=> f.Name == newForm.Name)).BringToFront();
            }
        }

        // Delets all irrelevant notes
        private void toolStripButtonClearOldNotes_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].BackColor == Color.Gray)
                {
                    for (int j = 0; j < Application.OpenForms.Count; j++)
                        if (Application.OpenForms[j].Name == listViewNotes.Items[i].Text)
                            Application.OpenForms[j].Close();

                    listViewNotes.Items.Remove(listViewNotes.Items[i]); i = -1;
                }
            }

            for (int i = 0; i < notes.Count; i++)
                if (notes[i].IsRelevant == false)
                {
                    notes.Remove(notes[i]);
                    i = -1;
                }
        }

        // Icon for opening or closing the main form
        private void notifyIconNotes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState != FormWindowState.Normal)
            {
                Show();
                WindowState = FormWindowState.Normal;
                BringToFront();
            }
            else
                WindowState = FormWindowState.Minimized;
        }

        // Notify check
        private void timerNoteNotify_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < notes.Count; i++)
                if (notes[i].ToNotify < DateTime.Now &&
                    notes[i].IsRelevant == true
                    || 
                    notes[i].ToNotify.Date < DateTime.Now.Date && 
                    notes[i].ToNotify.Hour < DateTime.Now.Hour && 
                    notes[i].ToNotify.Minute < DateTime.Now.Minute && 
                    notes[i].IsRelevant == true)
                    storedNotes.Add(notes[i]);

            ShowNotify();
        }

        // Saves data
        private void SaveData()
        {
            FileStream FS = new FileStream("DataNotes.dat", FileMode.Create, FileAccess.Write);
            BinaryFormatter BF = new BinaryFormatter();
            BF.Serialize(FS, notes);
            FS.Close();
        }
       
        // Loads data
        private void LoadData()
        {
            if (File.Exists("DataNotes.dat"))
            {
                FileStream FS = new FileStream("DataNotes.dat", FileMode.Open, FileAccess.Read);
                BinaryFormatter BF = new BinaryFormatter();
                notes = (List<NoteForm>)BF.Deserialize(FS);
                FS.Close();
            }

            ItemsShow(notes);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveData();
        }
    }

    /// <summary>
    /// NoteForm class for making
    /// </summary>
    [Serializable]
    public class NoteForm
    {
        /// <summary>
        /// Checks if the note is still relevant
        /// </summary>
        public bool IsRelevant;

        /// <summary>
        /// Checks if you can change the text
        /// </summary>
        public bool IsReadOnly;

        /// <summary>
        /// Note name
        /// </summary>
        public string Name;
        /// <summary>
        /// Time to remind about the note
        /// </summary>
        public DateTime ToNotify;
        /// <summary>
        /// Message, which note stores
        /// </summary>
        public string Message;
        /// <summary>
        /// Path to music, which will play during notify time
        /// </summary>
        public string Music;

        // Font color
        public Font FontType;
        public Color FontColor;
        public Color BackGroundColor;

        public NoteForm() { }
        public NoteForm(string name, DateTime toNotify, string message)
        {
            Name = name;
            ToNotify = toNotify;
            Message = message;
        }
    }

    /// <summary>
    /// MP3 
    /// </summary>
    public class MP3_Player
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, int hwdCallBack);
        public void open(string File)
        {
            string command = "close MediaFile";
            mciSendString(command, null, 0, 0);
            string Format = @"open ""{0}"" type MPEGVideo alias MediaFile";
            command = string.Format(Format, File);
            mciSendString(command, null, 0, 0);
        }
        public void play()
        {
            string command = "play MediaFile";
            mciSendString(command, null, 0, 0);
        }
        public void stop()
        {
            string command = "stop MediaFile";
            mciSendString(command, null, 0, 0);
        }
    }
}
