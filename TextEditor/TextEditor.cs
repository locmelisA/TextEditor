using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class TextEditor : Form
    {
        public TextEditor()
        {
            InitializeComponent();
            this.richTextBox1.AllowDrop = true;
            List_Tab_Index.Add(tabControl1.SelectedIndex.ToString());
            List_File_Name.Add("T");
            List_File_Path.Add("T");
        }

        List<string> List_Tab_Index = new List<string>();
        List<string> List_File_Name = new List<string>();
        List<string> List_File_Path = new List<string>();
        public bool list_debug = false;

        public void Add_to_the_List(string Index, string Filename, string Filepath)
        {
            List_Tab_Index.Add(Index);
            List_File_Name.Add(Filename);
            List_File_Path.Add(Filepath);
        }

        public void Simplify_List_Tab_Index()
        {

            for (int i = 0; i < List_Tab_Index.Count; ++i)
            {
                if (i < int.Parse(List_Tab_Index[i]))
                {
                    List_Tab_Index[i] = (int.Parse(List_Tab_Index[i]) - 1).ToString();
                }
            }
        }

        public void Remove_from_the_List(int Index)
        {
            List_Tab_Index.RemoveAt(Index);
            List_File_Name.RemoveAt(Index);
            List_File_Path.RemoveAt(Index);
        }

        public void Modify_the_List(string Index, string Filename, string Filepath)
        {

            for (int i = 0; i < List_Tab_Index.Count; ++i)
            {
                if (i == int.Parse(Index))
                {
                    List_File_Name[i] = Filename;
                    List_File_Path[i] = Filepath;
                }
            }
        }

        public void List_Message_Boxes()
        {
            string result = "List of Tab Indexes\n";
            foreach (string tabindex in List_Tab_Index)
            {
                result = result + tabindex + "\n";
            }
            result = result + "\nList of File Names\n";
            foreach (string filename in List_File_Name)
            {
                result = result + filename + "\n";
            }
            result = result + "\nList of File Paths\n";
            foreach (string filepath in List_File_Path)
            {
                result = result + filepath + "\n";
            }
            if (list_debug == true)
            {
                MessageBox.Show(result);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.tabControl1.SelectedTab = tabPage1;


            string title = "dok" + (tabControl1.TabCount + 1).ToString() + ".txt";
            TabPage myTabPage = new TabPage(title);

            /*if (tabControl1.TabPages.Contains(myTabPage))
            {
                title = "dok" + (tabControl1.TabCount -1).ToString() + ".txt";
                myTabPage = new TabPage(title);
                tabControl1.TabPages.Add(myTabPage);

            }
            else
            {
                tabControl1.TabPages.Add(myTabPage);
            }*/
            tabControl1.TabPages.Add(myTabPage);
            RichTextBox textBox = new RichTextBox();
            textBox.Size = new Size(600, 329);//800, 400
            textBox.WordWrap = false;
            textBox.TextChanged += new EventHandler(textBox_TextChanged);
            textBox.Dock = DockStyle.Fill;

            myTabPage.Controls.Add(textBox);
            this.tabControl1.SelectedTab = myTabPage;
            textBox.AllowDrop = true;
            textBox.DragDrop += new System.Windows.Forms.DragEventHandler(Drag_and_Drop);
            Add_to_the_List(tabControl1.SelectedIndex.ToString(), "T", "T");
            List_Message_Boxes();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text.StartsWith("*"))
            {
                DialogResult dialogResult = MessageBox.Show("You have some unsaved changes\nDo you want to save the file?", "Warning", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                {
                    Remove_from_the_List(tabControl1.SelectedIndex);
                    tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                    Simplify_List_Tab_Index();
                    List_Message_Boxes();

                }
                else if (dialogResult == DialogResult.No)
                {

                    Remove_from_the_List(tabControl1.SelectedIndex);
                    tabControl1.TabPages.Remove(tabControl1.SelectedTab);// Removes the selected tab:
                    Simplify_List_Tab_Index();
                    List_Message_Boxes();
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    //
                }
            }
            else
            {
                Remove_from_the_List(tabControl1.SelectedIndex);
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                Simplify_List_Tab_Index();
                List_Message_Boxes();
            }
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt";
            ofd.Title = "Open a txt File";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string file_stream = File.ReadAllText(ofd.FileName);

                string title_of_the_file = Path.GetFileName(ofd.FileName);
                TabPage myTabPage = new TabPage(title_of_the_file);
                tabControl1.TabPages.Add(myTabPage);

                RichTextBox textBox = new RichTextBox();
                textBox.Size = new Size(600, 329);//800, 400
                textBox.Text = file_stream;
                textBox.WordWrap = false;
                textBox.TextChanged += new EventHandler(textBox_TextChanged);
                textBox.Dock = DockStyle.Fill;
                Update_Labels(textBox);

                myTabPage.Controls.Add(textBox);
                this.tabControl1.SelectedTab = myTabPage;
                textBox.AllowDrop = true;
                textBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.Drag_and_Drop);

                var onlyFileName = Path.GetFileName(ofd.FileName);
                Add_to_the_List(tabControl1.SelectedIndex.ToString(), onlyFileName, ofd.FileName);
                List_Message_Boxes();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save_As_Function();
        }

        public void Save_As_Function()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt";
            saveFileDialog1.Title = "Save a Text to the txt File";
            //saveFileDialog1.ShowDialog();


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (tabControl1.SelectedTab.Text.StartsWith("*"))
                {
                    string title = tabControl1.SelectedTab.Text.ToString().Substring(1);
                    tabControl1.SelectedTab.Text = title;
                }

                string path = saveFileDialog1.FileName;
                StreamWriter bw = new StreamWriter(File.Create(path));

                //https://www.daniweb.com/programming/software-development/threads/466582/retrieving-text-from-a-dynamically-created-tab-and-text-box
                foreach (RichTextBox textBox in tabControl1.TabPages[tabControl1.SelectedIndex].Controls)
                {
                    bw.Write(textBox.Text);
                    tabControl1.SelectedTab.Text = Path.GetFileName(saveFileDialog1.FileName);
                }
                bw.Close();

                var onlyFileName = Path.GetFileName(saveFileDialog1.FileName);
                Modify_the_List(tabControl1.SelectedIndex.ToString(), onlyFileName, saveFileDialog1.FileName);
                List_Message_Boxes();

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (RichTextBox richtext in tabControl1.TabPages[tabControl1.SelectedIndex].Controls)
            {
                        if (List_File_Path.ElementAt(tabControl1.SelectedIndex) == "T")
                        {
                            Save_As_Function();
                        }
                        else
                        {
                            Save_Function();
                        }
            }
        }



        public void Save_Function()
        {
            foreach (RichTextBox richtext in tabControl1.TabPages[tabControl1.SelectedIndex].Controls)
            {
                foreach (string tabindex in List_Tab_Index)
                {
                    if (tabindex == tabControl1.SelectedIndex.ToString())
                    {
                        //MessageBox.Show(List_File_Name.ElementAt(int.Parse(tabindex)));
                        //MessageBox.Show(List_File_Path.ElementAt(int.Parse(tabindex)));
                        string path = List_File_Path.ElementAt(int.Parse(tabindex));
                        StreamWriter bw = new StreamWriter(File.Create(path));

                        bw.Write(richtext.Text);
                        tabControl1.SelectedTab.Text = Path.GetFileName(path);
                        bw.Close();
                    }
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!tabControl1.SelectedTab.Text.StartsWith("*"))
            { 
                string title = "*" + tabControl1.SelectedTab.Text.ToString();
                tabControl1.SelectedTab.Text = title;
            }
            foreach (RichTextBox richtext in tabControl1.TabPages[tabControl1.SelectedIndex].Controls)
            {
                Update_Labels(richtext);
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            try
            {
                //MessageBox.Show(tabControl1.SelectedTab.Text.ToString()); //debug purpose
                foreach (RichTextBox richtext in tabControl1.TabPages[tabControl1.SelectedIndex].Controls)
                {
                    //MessageBox.Show(richtext.Text); //debug purpose
                    Update_Labels(richtext);
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                //
            }

        }

        //for the custom textbox text changed
        public void textBox_TextChanged(object sender, EventArgs e)
        {
            if (!tabControl1.SelectedTab.Text.StartsWith("*"))
            {
                string title = "*" + tabControl1.SelectedTab.Text.ToString();
                tabControl1.SelectedTab.Text = title;
            }
            //MessageBox.Show("LOL"); //debug purpose
            foreach (RichTextBox richtext in tabControl1.TabPages[tabControl1.SelectedIndex].Controls)
            {
                Update_Labels(richtext);
            }
        }

        public void Update_Labels(RichTextBox richtext)
        {
            label2.Text = richtext.TextLength.ToString();
            string userInput = richtext.Text;
            userInput = userInput.Trim();
            string[] wordCount = userInput.Split();
            int words = wordCount.Length;
            int charCount = 0;
            foreach (var word in wordCount)
                charCount += word.Length;

            label4.Text = charCount.ToString();
            //label6.Text = wordCount.Length.ToString();
            label6.Text = words.ToString();

            //Row Count is here. It counts rows as soon as User presses Enter
            var lineCount = richtext.Lines.Length;
            label8.Text = lineCount.ToString();

            /* bug fix for word count that showed 1 word when there were none */
            if (label2.Text == "0" && label4.Text == "0" && label8.Text == "0")
            {
                label6.Text = "0";
            }
        }
        public void Drag_and_Drop(object sender, DragEventArgs e)
        {
            //shift add the text where your mouse is pointing in selected textbox
            //ctrl add the text in the end of the selected textbox
            //none button - add the text in the new tab


            // If the data is text, copy the data to the RichTextBox control.
            if (e.Data.GetDataPresent("Text"))
                e.Effect = DragDropEffects.Copy;

            if ((e.KeyState & 4) == 4)
            {
                //SHIFT KeyState
                //MessageBox.Show("SHIFT Worked"); //debug purpose
                string[] filepatch = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                string file_stream = File.ReadAllText(filepatch[0]);
                foreach (RichTextBox richtext in tabControl1.TabPages[tabControl1.SelectedIndex].Controls)
                {
                    richtext.SelectedText += file_stream;
                    Update_Labels(richtext);
                }
            }
            else if ((e.KeyState & 8) == 8)
            {
                //CTRL KeyState
                //MessageBox.Show("CTRL Worked"); //debug purpose
                string[] filepatch = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                string file_stream = File.ReadAllText(filepatch[0]);
                foreach (RichTextBox richtext in tabControl1.TabPages[tabControl1.SelectedIndex].Controls)
                {
                    richtext.Text = richtext.Text + file_stream;
                    Update_Labels(richtext);
                }
            }
            else
            {
                string[] filepatch = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                string file_stream = File.ReadAllText(filepatch[0]);
                string title_of_the_file = Path.GetFileName(filepatch[0]);
                TabPage myTabPage = new TabPage(title_of_the_file);
                tabControl1.TabPages.Add(myTabPage);

                RichTextBox textBox = new RichTextBox();
                textBox.Size = new Size(600, 329);//800, 400
                // Paste the text into the RichTextBox where at selection location.
                textBox.Text = file_stream;
                textBox.WordWrap = false;
                textBox.TextChanged += new EventHandler(textBox_TextChanged);
                Update_Labels(textBox);

                myTabPage.Controls.Add(textBox);
                this.tabControl1.SelectedTab = myTabPage;
                textBox.AllowDrop = true;
                textBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.Drag_and_Drop);
                //richTextBox1.SelectedText = filepatch[0];
                Add_to_the_List(tabControl1.SelectedIndex.ToString(), title_of_the_file, filepatch[0]);
                List_Message_Boxes();
            }
        }

        private void Warn_before_closing(object sender, FormClosingEventArgs e)
        {
            var unsaved_tabs = 0;
            foreach (TabPage tabpage in tabControl1.TabPages)
            {
                if (tabpage.Text.StartsWith("*"))
                {
                    unsaved_tabs++;
                }

            }

            if(unsaved_tabs > 0)
            {
                DialogResult dialogResult = new DialogResult();
                if (unsaved_tabs == 1)
                {
                    dialogResult = MessageBox.Show($"You have {unsaved_tabs} unsaved changes\nDo you want to save the file?", "Warning", MessageBoxButtons.YesNoCancel);
                }
                else
                {
                    dialogResult = MessageBox.Show($"You have {unsaved_tabs} unsaved changes\nDo you want to save the files?", "Warning", MessageBoxButtons.YesNoCancel);
                }
                if (dialogResult == DialogResult.Yes)
                {

                    //here must be handled all possible changes.. but how???? EDIT: I think i did it!?
                    do
                    {
                        if (tabControl1.SelectedTab.Text.StartsWith("*"))
                        {
                                if (List_File_Path.ElementAt(tabControl1.SelectedIndex) == "T")
                                {
                                    MessageBox.Show($"Now you going to save {tabControl1.SelectedTab.Text}\nYou may still choose to cancel this individual save!");
                                    Save_As_Function();
                                }
                                else
                                {
                                    Save_Function();
                                }
                                Remove_from_the_List(tabControl1.SelectedIndex);
                                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                                Simplify_List_Tab_Index();
                                List_Message_Boxes();
                        }
                        else
                        {
                            Remove_from_the_List(tabControl1.SelectedIndex);
                            tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                            Simplify_List_Tab_Index();
                            List_Message_Boxes();
                        }
                    }
                    while (tabControl1.SelectedIndex >= 0);
                }
                else if (dialogResult == DialogResult.No)
                {
                    //
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

    }
}
