﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlayerIOClient;
using System.Threading;

namespace EEditor
{
    public partial class NewDialogForm : Form
    {
        public int SizeWidth { get; private set; }
        public int SizeHeight { get; private set; }
        public Frame MapFrame { get; private set; }
        public bool NeedsInit { get; private set; }
        public bool RealTime { get; private set; }
        public bool notsaved { get; set; }
        public Connection Connection { get; set; }
        public MainForm MainForm { get; set; }
        private Client client;
        private string worldOwner = "Anonymous";
        private string owner = null;
        private Dictionary<string, string> data = new Dictionary<string, string>();
        private Semaphore s = new Semaphore(0, 1);
        private int messages = 0;
        //private bool errors = false;
        public NewDialogForm(MainForm mainForm)
        {
            InitializeComponent();
            levelTextBox.Text = MainForm.userdata.level ;
            //levelPassTextBox.Text = EEditor.Properties.Settings.Default.LevelPass;
            MainForm = mainForm;
            CheckForIllegalCrossThreadCalls = false;
            listBox1.SelectedIndex = 0;
            notsaved = false;
            messages = 0;
        }

        //Enable-disable level combobox accordingly
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == 0 || listBox1.SelectedIndex == 15 || listBox1.SelectedIndex == 16)
            {
                groupBox3.Visible = true;
                groupBox2.Visible = false;
            }
            if (listBox1.SelectedIndex == 17)
            {
                groupBox2.Visible = false;
            }
            if (listBox1.SelectedIndex >= 1 && listBox1.SelectedIndex <= 14)
            {
                groupBox3.Visible = false;
                groupBox2.Visible = false;
            }
        }

        private void levelTextBox_Enter(object sender, EventArgs e)
        {
            //rLoadLevel.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.SetPenTool();
            MainForm.userdata.thisColor = Color.Transparent;
            MainForm.userdata.useColor = false;
            ToolPen.undolist.Clear();
            ToolPen.redolist.Clear();
            ToolPen.rotation.Clear();
            messages = 0;
            //Clipboard.Clear();
            MainForm.tsc.Items.Clear();
            MainForm.tsc.Items.Add("Background");
            MainForm.tsc.Text = "Background";
            NeedsInit = true;
            MainForm.Text = "EEditor " + MainForm.ProductVersion;
            MainForm.userdata.openWorld = false;
            MainForm.userdata.openCodeWorld = false;
            #region Listbox selection
            if (listBox1.SelectedIndex == 0)
            {
                if (!string.IsNullOrEmpty(levelTextBox.Text))
                {
                    var level = levelTextBox.Text.Trim();
                    if (level.Contains('/')) level = level.Substring(level.LastIndexOf('/') + 1).Trim();
                    MainForm.userdata.level = level;
                    LoadFromLevel(level, 2);

                }
                return;
            }
            else if (listBox1.SelectedIndex == 1)
            {
                SizeWidth = 25;
                SizeHeight = 25;
            }
            else if (listBox1.SelectedIndex == 2)
            {
                SizeWidth = 40;
                SizeHeight = 30;
            }
            else if (listBox1.SelectedIndex == 3)
            {
                SizeWidth = 50;
                SizeHeight = 50;
            }
            else if (listBox1.SelectedIndex == 4)
            {
                SizeWidth = 100;
                SizeHeight = 100;
            }
            else if (listBox1.SelectedIndex == 5)
            {
                SizeWidth = 100;
                SizeHeight = 400;
            }
            else if (listBox1.SelectedIndex == 6)
            {
                SizeWidth = 110;
                SizeHeight = 110;
            }
            else if (listBox1.SelectedIndex == 7)
            {
                SizeWidth = 200;
                SizeHeight = 200;
            }
            else if (listBox1.SelectedIndex == 8)
            {
                SizeWidth = 200;
                SizeHeight = 400;
            }
            else if (listBox1.SelectedIndex == 9)
            {
                SizeWidth = 300;
                SizeHeight = 300;
            }
            else if (listBox1.SelectedIndex == 10)
            {
                SizeWidth = 400;
                SizeHeight = 50;
            }
            else if (listBox1.SelectedIndex == 11)
            {
                SizeWidth = 400;
                SizeHeight = 200;
            }
            else if (listBox1.SelectedIndex == 12)
            {
                SizeWidth = 636;
                SizeHeight = 50;
            }
            else if (listBox1.SelectedIndex == 13)
            {
                SizeWidth = 200;
                SizeHeight = 200;
                MainForm.userdata.openWorld = true;
                // restrictions here
            }
            else if (listBox1.SelectedIndex == 14)
            {
                SizeWidth = 200;
                SizeHeight = 200;
                MainForm.userdata.openWorld = true;
                MainForm.userdata.openCodeWorld = true;
                // restrictions here
            }
            else if (listBox1.SelectedIndex == 15)
            {
                if (!string.IsNullOrEmpty(levelTextBox.Text))
                {
                    var level = levelTextBox.Text.Trim();
                    if (level.Contains('/')) level = level.Substring(level.LastIndexOf('/') + 1).Trim();
                    MainForm.userdata.level = level;
                    LoadFromLevel(level, 0);

                }
                return;
            }
            else if (listBox1.SelectedIndex == 16)
            {
                if (!string.IsNullOrEmpty(levelTextBox.Text))
                {
                    var level = levelTextBox.Text.Trim();
                    if (level.Contains('/')) level = level.Substring(level.LastIndexOf('/') + 1).Trim();
                    MainForm.userdata.level = level;

                    LoadFromLevel(level, 1);
                }
                return;
            }
            else if (listBox1.SelectedIndex == 17)
            {
                /*if (!string.IsNullOrEmpty(levelTextBox.Text))
                {
                    var level = levelTextBox.Text.Trim();
                    if (level.Contains('/')) level = level.Substring(level.LastIndexOf('/') + 1).Trim();
                    Properties.Settings.Default.Level = level;
                    Properties.Settings.Default.loadx = (int)numericUpDown1.Value;
                    Properties.Settings.Default.loady = (int)numericUpDown2.Value;
                    Properties.Settings.Default.loadw = (int)numericUpDown3.Value;
                    Properties.Settings.Default.loadh = (int)numericUpDown4.Value;
                    Properties.Settings.Default.Save();

                    LoadFromLevel(level, 3);
                }*/
                return;
            }
            #endregion
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog dg = new ColorDialog();
            if (dg.ShowDialog() == DialogResult.OK)
            {
                MainForm.userdata.useColor = true;
                MainForm.userdata.thisColor = dg.Color;
            }
            else
            {
                MainForm.userdata.useColor = false;
                MainForm.userdata.thisColor = Color.Transparent;
            }
            Graphics g = Graphics.FromImage(MainForm.editArea.Back);
            for (int y = 0; y < MainForm.editArea.Frames[0].Height; y++)
            {

                for (int x = 0; x < MainForm.editArea.Frames[0].Width; x++)
                {
                    if (x == 0 || y == 0 || x == MainForm.editArea.Frames[0].Width - 1 || y == MainForm.editArea.Frames[0].Height - 1)
                    {
                        MainForm.editArea.Draw(x, y, g, Color.Transparent);
                    }
                    else
                    {
                        MainForm.editArea.Draw(x, y, g, MainForm.userdata.thisColor);
                    }
                }
            }
            MainForm.editArea.Invalidate();
            //Properties.Settings.Default.usecolor = false;
            //Properties.Settings.Default.Save();
        }

        public void LoadFromLevel(string level, int datas)
        {

                //errors = false;
                //EEditor.Properties.Settings.Default.LevelPass = levelPassTextBox.Text;

                try
                {
                    if (MainForm.accs[MainForm.selectedAcc].loginMethod == 0 && MainForm.accs.ContainsKey(MainForm.selectedAcc))
                    {
                        client = PlayerIO.QuickConnect.SimpleConnect("everybody-edits-su9rn58o40itdbnw69plyw", MainForm.accs[MainForm.selectedAcc].login, MainForm.accs[MainForm.selectedAcc].password, null);
                    }
                    else if (MainForm.accs[MainForm.selectedAcc].loginMethod == 1 && MainForm.accs.ContainsKey(MainForm.selectedAcc))
                    {
                        client = PlayerIO.QuickConnect.FacebookOAuthConnect("everybody-edits-su9rn58o40itdbnw69plyw", MainForm.accs[MainForm.selectedAcc].login, null, null);
                    }
                    else if (MainForm.accs[MainForm.selectedAcc].loginMethod == 2 && MainForm.accs.ContainsKey(MainForm.selectedAcc))
                    {
                        client = PlayerIO.QuickConnect.KongregateConnect("everybody-edits-su9rn58o40itdbnw69plyw", MainForm.accs[MainForm.selectedAcc].login, MainForm.accs[MainForm.selectedAcc].password, null);
                    }
                    else if (MainForm.accs[MainForm.selectedAcc].loginMethod == 3 && MainForm.accs.ContainsKey(MainForm.selectedAcc))
                    {
                        client = PlayerIO.Authenticate("everybody-edits-su9rn58o40itdbnw69plyw", "secure", new Dictionary<string, string> { { "userId", MainForm.accs[MainForm.selectedAcc].login }, { "authToken", MainForm.accs[MainForm.selectedAcc].password } }, null);
                    }

                    if (datas == 0)
                    {
                        Connection = client.Multiplayer.CreateJoinRoom(MainForm.userdata.level, MainForm.userdata.level.StartsWith("BW") ? "Beta" : "Everybodyedits" + client.BigDB.Load("config", "config")["version"], true, null, null);
                        Connection.OnMessage += OnMessage;
                        Connection.Send("init");
                        NeedsInit = false;
                        s.WaitOne();
                    }


                    else if (datas == 1)
                    {
                        int w = 0;
                        int h = 0;
                        DatabaseObject dbo = client.BigDB.Load("Worlds", MainForm.userdata.level);
                        var name = dbo.Contains("name") ? dbo["name"].ToString() : "Untitled World";
                        owner = dbo.Contains("owner") ? dbo["owner"].ToString() : null;
                        if (dbo.Contains("width") && dbo.Contains("height") && dbo.Contains("worlddata"))
                        {
                            uid2name(owner, name, Convert.ToInt32(dbo["width"]), Convert.ToInt32(dbo["height"]));
                            MapFrame = new Frame(Convert.ToInt32(dbo["width"]), Convert.ToInt32(dbo["height"]));
                        }
                        else
                        {
                            if (dbo.Contains("type"))
                            {
                                switch ((int)dbo["type"])
                                {
                                    case 1:
                                        w = 50;
                                        h = 50;
                                        break;
                                    case 2:
                                        w = 100;
                                        h = 100;
                                        break;
                                    default:
                                    case 3:
                                        w = 200;
                                        h = 200;
                                        break;
                                    case 4:
                                        w = 400;
                                        h = 50;
                                        break;
                                    case 5:
                                        w = 400;
                                        h = 200;
                                        break;
                                    case 6:
                                        w = 100;
                                        h = 400;
                                        break;
                                    case 7:
                                        w = 636;
                                        h = 50;
                                        break;
                                    case 8:
                                        w = 110;
                                        h = 110;
                                        break;
                                    case 11:
                                        w = 300;
                                        h = 300;
                                        break;
                                    case 12:
                                        w = 250;
                                        h = 150;
                                        break;
                                    case 13:
                                        w = 200;
                                        h = 200;
                                        break;
                                }
                                if (dbo.Contains("worlddata"))
                                {
                                    MapFrame = new Frame(w, h);
                                    uid2name(owner, name, w, h);
                                }
                            }
                            else
                            {
                                uid2name(owner, name, 200, 200);
                                MapFrame = new Frame(200, 200);
                            }
                        }


                        if (dbo.Contains("worlddata"))
                        {
                            MapFrame = Frame.FromMessage2(dbo);
                            SizeWidth = MapFrame.Width;
                            SizeHeight = MapFrame.Height;
                            NeedsInit = false;
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                        }
                        else
                        {
                            notsaved = true;
                            DialogResult = System.Windows.Forms.DialogResult.Cancel;
                        }
                        Close();
                    }
                    else if (datas == 2)
                    {
                        int w = 0;
                        int h = 0;
                        DatabaseObject dbo = client.BigDB.Load("Worlds", MainForm.userdata.level);
                        var name = dbo.Contains("name") ? dbo["name"].ToString() : "Untitled World";
                        owner = dbo.Contains("owner") ? dbo["owner"].ToString() : null;
                        if (dbo.Contains("width") && dbo.Contains("height") && dbo.Contains("worlddata"))
                        {
                            uid2name(owner, name, Convert.ToInt32(dbo["width"]), Convert.ToInt32(dbo["height"]));
                            MapFrame = new Frame(Convert.ToInt32(dbo["width"]), Convert.ToInt32(dbo["height"]));
                        }
                        else
                        {
                            if (dbo.Contains("type"))
                            {
                                switch ((int)dbo["type"])
                                {
                                    case 1:
                                        w = 50;
                                        h = 50;
                                        break;
                                    case 2:
                                        w = 100;
                                        h = 100;
                                        break;
                                    default:
                                    case 3:
                                        w = 200;
                                        h = 200;
                                        break;
                                    case 4:
                                        w = 400;
                                        h = 50;
                                        break;
                                    case 5:
                                        w = 400;
                                        h = 200;
                                        break;
                                    case 6:
                                        w = 100;
                                        h = 400;
                                        break;
                                    case 7:
                                        w = 636;
                                        h = 50;
                                        break;
                                    case 8:
                                        w = 110;
                                        h = 110;
                                        break;
                                    case 11:
                                        w = 300;
                                        h = 300;
                                        break;
                                    case 12:
                                        w = 250;
                                        h = 150;
                                        break;
                                    case 13:
                                        w = 200;
                                        h = 200;
                                        break;
                                }
                                MapFrame = new Frame(w, h);
                                uid2name(owner, name, w, h);
                            }
                            else
                            {
                                uid2name(owner, name, 200, 200);
                                MapFrame = new Frame(200, 200);
                            }



                        }
                        MapFrame.Reset(false);
                        SizeWidth = MapFrame.Width;
                        SizeHeight = MapFrame.Height;
                        NeedsInit = false;
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        Close();

                    }
                }
                catch (PlayerIOError error)
                {
                    MessageBox.Show("An error occurred:" + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            
        }

        void OnMessage(object sender, PlayerIOClient.Message e)
        {
            if (e.Type == "init")
            {
                MapFrame = Frame.FromMessage(e, false);
                if (MapFrame != null)
                {
                    var owner = e.GetString(0) == "" ? "Unknown" : e.GetString(0);
                    MainForm.Text = e[1] + " by " + owner + " (" + e[18] + "x" + e[19] + ") - EEditor " + this.ProductVersion;
                    SizeWidth = MapFrame.Width;
                    SizeHeight = MapFrame.Height;
                    Connection.Disconnect();
                    Connection.OnMessage -= OnMessage;
                    s.Release();
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("World's width and height aren't integers. Please report this to our bug tracker.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    s.Release();
                    DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    Close();
                }
            }
            else if (e.Type == "upgrade")
            {
                MessageBox.Show("Game got updated. Please report this to our bug tracker.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                s.Release();
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();

            }
            else
            {
                messages += 1;
                if (messages == 1)
                {
                    MessageBox.Show(e.GetString(1), e.GetString(0), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    s.Release();
                    DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    Close();
                }

                //if (e.Type != "b" && e.Type != "m" && e.Type != "hide" && e.Type != "show")Console.WriteLine(e.ToString());
            }
        }
        private void uid2name(string uid, string title, int width, int height)
        {
            worldOwner = "Anonymous";
            if (uid != null)
            {
                PlayerIO.QuickConnect.SimpleConnect("everybody-edits-su9rn58o40itdbnw69plyw", "guest", "guest", null,
                    delegate (Client c)
                    {
                        c.BigDB.Load("PlayerObjects", uid,
                            delegate (DatabaseObject dbo)
                            {
                                worldOwner = dbo.Contains("name") ? dbo.GetString("name") : "Anonymous";
                                MainForm.Text = title + " by " + worldOwner + " (" + width + "x" + height + ") - EEditor " + this.ProductVersion;
                            },
                            delegate (PlayerIOError error)
                            {
                            });
                    },
                       delegate (PlayerIOError error)
                       {
                       });
            }
            else
            {
                MainForm.Text = title + " by anonymous owner " + " (" + width + "x" + height + ") - EEditor " + this.ProductVersion;
            }
        }

        private void NewDialogForm_Load(object sender, EventArgs e)
        {
            listBox1.Items.RemoveAt(16);

        }

        private void NewDialogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.userdata.level = levelTextBox.Text;
        }
    }
}
