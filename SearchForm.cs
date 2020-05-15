using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using NotesForms;

namespace NotesFomrs
{
    public partial class SearchForm : Form
    {
        private int selectedCount;

        private ListView update;
        private List<NoteForm> workWith;

        public SearchForm(ListView update, List<NoteForm> workWith)
        {
            InitializeComponent();
            selectedCount = 0;

            this.update = update;
            this.workWith = workWith;

            buttonExit.Click += (s, e) => { Close(); };
        }

        // CheckBox analyze
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;

            _ = check.Checked == true ? selectedCount++ : selectedCount--;

            if (selectedCount > 0)
                buttonSearch.Enabled = true;
            else
                buttonSearch.Enabled = false; 

            switch (check.Text)
            {
                case "Name":
                    _ = check.Checked == true ? textBoxName.Enabled = true : textBoxName.Enabled = false;
                    break;
                case "Message":
                    _ = check.Checked == true ? textBoxMessage.Enabled = true : textBoxMessage.Enabled = false;
                    break;
                case "Date":
                    _ = check.Checked == true ? dateTimePickerToFind.Enabled = true : dateTimePickerToFind.Enabled = false;
                    break;
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
             if (checkBoxName.Checked == true && string.IsNullOrEmpty(textBoxName.Text)
                || checkBoxMessage.Checked == true && string.IsNullOrEmpty(textBoxMessage.Text))
            {
                MessageBox.Show(this, "There must be something in the filters fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            update.Items.Clear();

             // If all data will match with the one, you have chosen in the filters, the item will be added to the list
            int verificationCount;
             for (int i = 0; i < workWith.Count; i++)
            {
                verificationCount = 0;
                if (checkBoxName.Checked == true)
                    if (workWith[i].Name.ToLower().Contains(textBoxName.Text.ToLower()))
                        verificationCount++;
                if (checkBoxMessage.Checked == true)
                    if (workWith[i].Message.ToLower().Contains(textBoxMessage.Text.ToLower()))
                        verificationCount++;
                if (checkBoxDate.Checked == true)
                    if (dateTimePickerToFind.Value.Date == workWith[i].ToNotify.Date)
                        verificationCount++;

                if (verificationCount == selectedCount)
                {
                    update.Items.Insert(0, workWith[i].Name);
                    update.Items[0].SubItems.Add(workWith[i].ToNotify.ToShortDateString() + " | " + workWith[i].ToNotify.ToShortTimeString());
                }
            }
        }
    }
}
