﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace EEditor
{
    public partial class Replacer : Form
    {
        public NumericUpDown NU1 { get { return numericUpDown1; } set { numericUpDown1 = value; } }
        public NumericUpDown NU2 { get { return numericUpDown2; } set { numericUpDown2 = value; } }
        MainForm MainForm { get; set; }
        private int[,] replaced;
        private int[,] replaced1;
        Bitmap bmp = new Bitmap(1, 1);
        Bitmap img1;
        public Replacer(MainForm mainForm)
        {
            this.MainForm = mainForm;
            InitializeComponent();
        }

        private void Replacer_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(MainForm.editArea.Frames[0].Height, MainForm.editArea.Frames[0].Width);
            replaced = new int[MainForm.editArea.Frames[0].Height, MainForm.editArea.Frames[0].Width];
            replaced1 = new int[MainForm.editArea.Frames[0].Height, MainForm.editArea.Frames[0].Width];
            MainForm.editArea.Back1 = MainForm.editArea.Back;
            numericUpDown1.Value = MainForm.editArea.Tool.PenID;
            numericUpDown2.Value = 0;
            //if (bdata.getRotation(numericUpDown1.Value, MainForm.editArea.Frames[0].BlockData[yy, xx]) != null) MainForm.editArea.Back = bdata.getRotation(MainForm.editArea.Frames[0].Foreground[yy, xx], MainForm.editArea.Frames[0].BlockData[yy, xx]);
            Bitmap img2 = MainForm.foregroundBMD.Clone(new Rectangle(0 * 16, 0, 16, 16), MainForm.foregroundBMD.PixelFormat);
            pictureBox2.Image = img2;

            ToolTip tp = new ToolTip();
            tp.SetToolTip(button4, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around it");
            tp.SetToolTip(button3, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around them");
            tp.SetToolTip(button5, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd replaces it with block ID " + numericUpDown2.Value);
            tp.SetToolTip(button1, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd replaces them with block ID " + numericUpDown2.Value);
            tp.SetToolTip(button8, "Removes all red rectangles around blocks");
            tp.SetToolTip(button9, "Finds all blocks you don't own and replaces them with block ID " + numericUpDown2.Value);
            tp.SetToolTip(toolStripContainer1.ContentPanel, "Left click: insert ID to find box\nRight click: insert ID to replace box"); // Would be more useful if the tooltip was added to single blocks
            this.Text = MainForm.debug ? "Find & replace - Debug Activated" : "Find & replace";
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ToolTip tp = new ToolTip();
            tp.SetToolTip(button4, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around it");
            tp.SetToolTip(button3, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around them");
            tp.SetToolTip(button5, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd replaces it with block ID " + numericUpDown2.Value);
            tp.SetToolTip(button1, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd replaces them with block ID " + numericUpDown2.Value);
            if (numericUpDown1.Value < 500 || numericUpDown1.Value >= 1001 && numericUpDown1.Value < 3000)
            {
                if (MainForm.decosBMI[(int)numericUpDown1.Value] != 0)
                {
                    Bitmap img1 = MainForm.decosBMD.Clone(new Rectangle(MainForm.decosBMI[Convert.ToInt32(numericUpDown1.Value)] * 16, 0, 16, 16), MainForm.decosBMD.PixelFormat);
                    pictureBox1.Image = img1;
                    button4.Enabled = true;
                    button3.Enabled = true;
                }
                else if (MainForm.miscBMI[(int)numericUpDown1.Value] != 0 || (int)numericUpDown1.Value == 119)
                {

                    Bitmap img1 = MainForm.miscBMD.Clone(new Rectangle(MainForm.miscBMI[Convert.ToInt32(numericUpDown1.Value)] * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                    pictureBox1.Image = img1;
                    button4.Enabled = true;
                    button3.Enabled = true;
                }
                else if (MainForm.foregroundBMI[(int)numericUpDown1.Value] != 0 || (int)numericUpDown1.Value == 0)
                {
                    Bitmap img1 = MainForm.foregroundBMD.Clone(new Rectangle(MainForm.foregroundBMI[Convert.ToInt32(numericUpDown1.Value)] * 16, 0, 16, 16), MainForm.foregroundBMD.PixelFormat);
                    pictureBox1.Image = img1;
                    numericUpDown1.ForeColor = SystemColors.ControlText;
                    button4.Enabled = true;
                    button3.Enabled = true;
                }
                else
                {
                    Bitmap temp = new Bitmap(16, 16);
                    Graphics gr = Graphics.FromImage(temp);
                    gr.Clear(Color.Black);
                    gr.DrawImage(Properties.Resources.bullets.Clone(new Rectangle(5 * 16, 0, 16, 16), Properties.Resources.bullets.PixelFormat), 0, 0);
                    pictureBox1.Image = temp;
                }
            }
            else if (numericUpDown1.Value >= 500 && numericUpDown1.Value <= 999)
            {
                if (MainForm.backgroundBMI[(int)numericUpDown1.Value] != 0 || (int)numericUpDown1.Value == 500)
                {
                    Bitmap img7 = MainForm.backgroundBMD.Clone(new Rectangle(MainForm.backgroundBMI[Convert.ToInt32(numericUpDown1.Value)] * 16, 0, 16, 16), MainForm.backgroundBMD.PixelFormat);
                    pictureBox1.Image = img7;
                    numericUpDown1.ForeColor = SystemColors.ControlText;
                    button4.Enabled = true;
                    button3.Enabled = true;
                }
                else
                {
                    Bitmap temp = new Bitmap(16, 16);
                    Graphics gr = Graphics.FromImage(temp);
                    gr.Clear(Color.Black);
                    gr.DrawImage(Properties.Resources.bullets.Clone(new Rectangle(5 * 16, 0, 16, 16), Properties.Resources.bullets.PixelFormat), 0, 0);
                    pictureBox1.Image = temp;

                }
            }

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            ToolTip tp = new ToolTip();
            tp.SetToolTip(button4, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around it");
            tp.SetToolTip(button3, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around them");
            tp.SetToolTip(button5, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd replaces it with block ID " + numericUpDown2.Value);
            tp.SetToolTip(button1, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd replaces them with block ID " + numericUpDown2.Value);
            if (numericUpDown2.Value < 500 || numericUpDown2.Value >= 1001 && numericUpDown2.Value < 3000)
            {
                if (MainForm.decosBMI[(int)numericUpDown2.Value] != 0)
                {
                    if (MainForm.editArea.ownedBid((int)numericUpDown2.Value, 2))
                    {
                        Bitmap img1 = MainForm.decosBMD.Clone(new Rectangle(MainForm.decosBMI[Convert.ToInt32(numericUpDown2.Value)] * 16, 0, 16, 16), MainForm.decosBMD.PixelFormat);
                        pictureBox2.Image = img1;
                        numericUpDown2.ForeColor = SystemColors.ControlText;
                        label4.Text = null;
                        button5.Enabled = true;
                        button1.Enabled = true;
                    }
                    else
                    {
                        pictureBox2.Image = Properties.Resources.cross;
                        numericUpDown2.ForeColor = Color.Red;
                        button5.Enabled = false;
                        button1.Enabled = false;
                        label4.Text = "Can't replace block ID " + numericUpDown1.Value + " with ID " + numericUpDown2.Value + ", you don't own it.";
                    }
                }

                else if (MainForm.miscBMI[(int)numericUpDown2.Value] != 0 || (int)numericUpDown2.Value == 119)
                {
                    if (MainForm.editArea.ownedBid((int)numericUpDown2.Value, 1))
                    {
                        Bitmap img1 = MainForm.miscBMD.Clone(new Rectangle(MainForm.miscBMI[Convert.ToInt32(numericUpDown2.Value)] * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                        pictureBox2.Image = img1;
                        numericUpDown2.ForeColor = SystemColors.ControlText;
                        label4.Text = null;
                        button5.Enabled = true;
                        button1.Enabled = true;
                    }
                    else
                    {
                        pictureBox2.Image = Properties.Resources.cross;
                        numericUpDown2.ForeColor = Color.Red;
                        button5.Enabled = false;
                        button1.Enabled = false;
                        label4.Text = "Can't replace block ID " + numericUpDown1.Value + " with ID " + numericUpDown2.Value + ", you don't own it.";
                    }
                }
                else
                {
                    if (MainForm.editArea.ownedBid((int)numericUpDown2.Value, 0))
                    {
                        Bitmap img1 = MainForm.foregroundBMD.Clone(new Rectangle(MainForm.foregroundBMI[Convert.ToInt32(numericUpDown2.Value)] * 16, 0, 16, 16), MainForm.foregroundBMD.PixelFormat);
                        pictureBox2.Image = img1;
                        numericUpDown2.ForeColor = SystemColors.ControlText;
                        label4.Text = null;
                        button5.Enabled = true;
                        button1.Enabled = true;
                    }
                    else
                    {
                        pictureBox2.Image = Properties.Resources.cross;
                        numericUpDown2.ForeColor = Color.Red;
                        button5.Enabled = false;
                        button1.Enabled = false;
                        label4.Text = "Can't replace block ID " + numericUpDown1.Value + " with ID " + numericUpDown2.Value + ", you don't own it.";
                    }

                }
            }

            else if (numericUpDown2.Value >= 500 && numericUpDown2.Value <= 999)
            {
                if (MainForm.editArea.ownedBid((int)numericUpDown2.Value, 3))
                {
                    Bitmap img6 = MainForm.backgroundBMD.Clone(new Rectangle(MainForm.backgroundBMI[Convert.ToInt32(numericUpDown2.Value)] * 16, 0, 16, 16), MainForm.backgroundBMD.PixelFormat);
                    pictureBox2.Image = img6;
                    numericUpDown2.ForeColor = SystemColors.ControlText;
                    label4.Text = null;
                    button5.Enabled = true;
                    button1.Enabled = true;
                }
                else
                {
                    pictureBox2.Image = Properties.Resources.cross;
                    numericUpDown2.ForeColor = Color.Red;
                    button5.Enabled = false;
                    button1.Enabled = false;
                    label4.Text = "Can't replace block ID " + numericUpDown1.Value + " with ID " + numericUpDown2.Value + ", you don't own it.";
                }
            }
        }

        private void numericUpDown2_KeyUp(object sender, KeyEventArgs e)
        {
            ToolTip tp = new ToolTip();
            tp.SetToolTip(button4, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around it");
            tp.SetToolTip(button3, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around them");
            tp.SetToolTip(button5, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd replaces it with block ID " + numericUpDown2.Value);
            tp.SetToolTip(button1, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd replaces them with block ID " + numericUpDown2.Value);
            if (numericUpDown2.Value < 500 || numericUpDown2.Value >= 1001 && numericUpDown2.Value < 3000)
            {
                if (MainForm.decosBMI[(int)numericUpDown2.Value] != 0)
                {
                    if (MainForm.editArea.ownedBid((int)numericUpDown2.Value, 2))
                    {
                        Bitmap img1 = MainForm.decosBMD.Clone(new Rectangle(MainForm.decosBMI[Convert.ToInt32(numericUpDown2.Value)] * 16, 0, 16, 16), MainForm.decosBMD.PixelFormat);
                        pictureBox2.Image = img1;
                        button5.Enabled = true;
                        button1.Enabled = true;
                        label4.Text = null;
                        numericUpDown2.ForeColor = SystemColors.ControlText;
                    }
                    else
                    {
                        pictureBox2.Image = Properties.Resources.cross;
                        numericUpDown2.ForeColor = Color.Red;
                        button5.Enabled = false;
                        button1.Enabled = false;
                        label4.Text = "Can't replace block ID " + numericUpDown1.Value + " with ID " + numericUpDown2.Value + ", you don't own it.";
                    }
                }
                else if (MainForm.miscBMI[(int)numericUpDown2.Value] != 0 || numericUpDown2.Value == 119)
                {
                    if (MainForm.editArea.ownedBid((int)numericUpDown2.Value, 1))
                    {
                        Bitmap img1 = MainForm.miscBMD.Clone(new Rectangle(MainForm.miscBMI[Convert.ToInt32(numericUpDown2.Value)] * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                        pictureBox2.Image = img1;
                        button5.Enabled = true;
                        button1.Enabled = true;
                        label4.Text = null;
                        numericUpDown2.ForeColor = SystemColors.ControlText;
                    }
                    else
                    {
                        pictureBox2.Image = Properties.Resources.cross;
                        numericUpDown2.ForeColor = Color.Red;
                        button5.Enabled = false;
                        button1.Enabled = false;
                        label4.Text = "Can't replace block ID " + numericUpDown1.Value + " with ID " + numericUpDown2.Value + ", you don't own it.";
                    }
                }
                else
                {
                    if (MainForm.editArea.ownedBid((int)numericUpDown2.Value, 0))
                    {
                        Bitmap img1 = MainForm.foregroundBMD.Clone(new Rectangle(MainForm.foregroundBMI[Convert.ToInt32(numericUpDown2.Value)] * 16, 0, 16, 16), MainForm.foregroundBMD.PixelFormat);
                        pictureBox2.Image = img1;
                        button5.Enabled = true;
                        button1.Enabled = true;
                        label4.Text = null;
                        numericUpDown2.ForeColor = SystemColors.ControlText;
                    }
                    else
                    {
                        pictureBox2.Image = Properties.Resources.cross;
                        numericUpDown2.ForeColor = Color.Red;
                        button5.Enabled = false;
                        button1.Enabled = false;
                        label4.Text = "Can't replace block ID " + numericUpDown1.Value + " with ID " + numericUpDown2.Value + ", you don't own it.";
                    }
                }
            }
            else if (numericUpDown2.Value >= 500 && numericUpDown2.Value <= 999)
            {
                if (MainForm.editArea.ownedBid((int)numericUpDown2.Value, 3))
                {
                    Bitmap img2 = MainForm.backgroundBMD.Clone(new Rectangle(MainForm.backgroundBMI[Convert.ToInt32(numericUpDown2.Value)] * 16, 0, 16, 16), MainForm.backgroundBMD.PixelFormat);
                    pictureBox2.Image = img2;
                    button5.Enabled = true;
                    button1.Enabled = true;
                    label4.Text = null;
                    numericUpDown2.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    pictureBox2.Image = Properties.Resources.cross;
                    numericUpDown2.ForeColor = Color.Red;
                    button5.Enabled = false;
                    button1.Enabled = false;
                    label4.Text = null;
                    label4.Text = "Can't replace block ID " + numericUpDown1.Value + " with ID " + numericUpDown2.Value + ", you don't own it.";
                }
            }
        }

        private void numericUpDown1_KeyUp(object sender, KeyEventArgs e)
        {
            ToolTip tp = new ToolTip();
            tp.SetToolTip(button4, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around it");
            tp.SetToolTip(button3, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around them");
            tp.SetToolTip(button5, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd replaces it with block ID " + numericUpDown2.Value);
            tp.SetToolTip(button1, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd replaces them with block ID " + numericUpDown2.Value);
            if (numericUpDown1.Value < 500 || numericUpDown1.Value >= 1001 && numericUpDown1.Value < 3000)
            {
                if (MainForm.decosBMI[(int)numericUpDown1.Value] != 0)
                {
                    Bitmap img1 = MainForm.decosBMD.Clone(new Rectangle(MainForm.decosBMI[Convert.ToInt32(numericUpDown1.Value)] * 16, 0, 16, 16), MainForm.decosBMD.PixelFormat);
                    pictureBox1.Image = img1;
                }
                else if (MainForm.miscBMI[(int)numericUpDown1.Value] != 0 || (int)numericUpDown1.Value == 119)
                {
                    Bitmap img1 = MainForm.miscBMD.Clone(new Rectangle(MainForm.miscBMI[Convert.ToInt32(numericUpDown1.Value)] * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                    pictureBox1.Image = img1;
                }
                else if (MainForm.foregroundBMI[(int)numericUpDown1.Value] != 0 || numericUpDown1.Value == 0)
                {
                    Bitmap img1 = MainForm.foregroundBMD.Clone(new Rectangle(MainForm.foregroundBMI[Convert.ToInt32(numericUpDown1.Value)] * 16, 0, 16, 16), MainForm.foregroundBMD.PixelFormat);
                    pictureBox1.Image = img1;
                    numericUpDown1.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    Bitmap temp = new Bitmap(16, 16);
                    Graphics gr = Graphics.FromImage(temp);
                    gr.Clear(Color.Black);
                    gr.DrawImage(Properties.Resources.bullets.Clone(new Rectangle(5 * 16, 0, 16, 16), Properties.Resources.bullets.PixelFormat), 0, 0);
                    pictureBox1.Image = temp;
                }
                int total = 0;
                for (int x = 0; x < MainForm.editArea.CurFrame.Width; x++)
                {
                    for (int y = 0; y < MainForm.editArea.CurFrame.Height; y++)
                    {
                        if (MainForm.editArea.CurFrame.Foreground[y, x] == numericUpDown1.Value)
                        {
                            total += 1;
                        }
                    }
                }
                label4.Text = "The world has " + total + " blocks of ID " + numericUpDown1.Value + ".";
            }
            else if (numericUpDown1.Value >= 500 && numericUpDown1.Value <= 999)
            {
                if (MainForm.backgroundBMI[Convert.ToInt32(numericUpDown1.Value)] != 0 || Convert.ToInt32(numericUpDown1.Value) == 500)
                {
                    Bitmap img4 = MainForm.backgroundBMD.Clone(new Rectangle(MainForm.backgroundBMI[Convert.ToInt32(numericUpDown1.Value)] * 16, 0, 16, 16), MainForm.backgroundBMD.PixelFormat);
                    pictureBox1.Image = img4;
                    int total = 0;
                    for (int x = 0; x < MainForm.editArea.CurFrame.Width; x++)
                    {
                        for (int y = 0; y < MainForm.editArea.CurFrame.Height; y++)
                        {
                            if (MainForm.editArea.CurFrame.Background[x, y] == numericUpDown1.Value)
                            {
                                total += 1;
                            }
                        }
                    }
                    label4.Text = "The world has " + total + " backgrounds of ID " + numericUpDown1.Value + ".";
                }
                else
                {
                    Bitmap temp = new Bitmap(16, 16);
                    Graphics gr = Graphics.FromImage(temp);
                    gr.Clear(Color.Black);
                    gr.DrawImage(Properties.Resources.bullets.Clone(new Rectangle(5 * 16, 0, 16, 16), Properties.Resources.bullets.PixelFormat), 0, 0);
                    pictureBox1.Image = temp;
                    int total = 0;
                    for (int x = 0; x < MainForm.editArea.CurFrame.Width; x++)
                    {
                        for (int y = 0; y < MainForm.editArea.CurFrame.Height; y++)
                        {
                            if (MainForm.editArea.CurFrame.Background[x, y] == numericUpDown1.Value)
                            {
                                total += 1;
                            }
                        }
                    }
                    label4.Text = "The world has " + total + " backgrounds of ID " + numericUpDown1.Value + ".";

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            toolStrip1.Items.Clear();
            toolStripContainer1.ContentPanel.Controls.Clear();
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {

                for (int i = 0; i < MainForm.ownedb.Count; i++)
                {
                    if (MainForm.ownedb[i].name.ToLower().Contains(textBox1.Text))
                    {
                        int[] blocks = MainForm.ownedb[i].blocks;
                        ToolStrip strip = new ToolStrip();
                        strip.Name = MainForm.ownedb[i].name + i;
                        strip.GripStyle = ToolStripGripStyle.Hidden;
                        toolStripContainer1.ContentPanel.Controls.Add(strip);
                        for (int a = 0; a < blocks.Length; a++)
                        {
                            int bid = blocks[a];
                            if (bid < 500 || bid >= 1001)
                            {
                                if (MainForm.decosBMI[bid] != 0)
                                {
                                    img1 = MainForm.decosBMD.Clone(new Rectangle(MainForm.decosBMI[bid] * 16, 0, 16, 16), MainForm.decosBMD.PixelFormat);
                                }
                                else if (MainForm.miscBMI[bid] != 0 || bid == 119)
                                {
                                    img1 = MainForm.miscBMD.Clone(new Rectangle(MainForm.miscBMI[bid] * 16, 0, 16, 16), MainForm.miscBMD.PixelFormat);
                                }
                                else
                                {
                                    img1 = MainForm.foregroundBMD.Clone(new Rectangle(MainForm.foregroundBMI[bid] * 16, 0, 16, 16), MainForm.foregroundBMD.PixelFormat);
                                }
                            }
                            else if (bid >= 500 && bid <= 999)
                            {
                                if (MainForm.backgroundBMI[bid] != 0 || bid == 500)
                                {
                                    img1 = MainForm.backgroundBMD.Clone(new Rectangle(MainForm.backgroundBMI[bid] * 16, 0, 16, 16), MainForm.backgroundBMD.PixelFormat);
                                }
                            }
                            ToolStripButton bt = new ToolStripButton();
                            bt.Image = img1;
                            bt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
                            bt.Name = blocks[a].ToString();
                            bt.MouseDown += Bt_MouseDown;
                            bt.ToolTipText = "Left click: insert ID to find box\nRight click: insert ID to replace box";
                            strip.Items.Add(bt);
                        }
                    }
                }
            }
        }

        private void Bt_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ToolStripButton der = (ToolStripButton)sender;
                numericUpDown2.Value = Convert.ToInt32(der.Name);
            }
            else if (e.Button == MouseButtons.Left)
            {
                ToolStripButton der = (ToolStripButton)sender;
                numericUpDown1.Value = Convert.ToInt32(der.Name);
            }
            ToolTip tp = new ToolTip();
            tp.SetToolTip(button4, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around it");
            tp.SetToolTip(button3, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd displays a red rectangle around them");
            tp.SetToolTip(button5, "Finds the next block with ID " + numericUpDown1.Value + "\nAnd replaces it with block ID " + numericUpDown2.Value);
            tp.SetToolTip(button1, "Finds all blocks with ID " + numericUpDown1.Value + "\nAnd replaces them with block ID " + numericUpDown2.Value);
        }


        private void button5_Click(object sender, EventArgs e)
        {
            int first = 0;
            for (int y = 0; y < MainForm.editArea.Frames[0].Height; y++)
            {
                for (int x = 0; x < MainForm.editArea.Frames[0].Width; x++)
                {
                    if (MainForm.editArea.Frames[0].Foreground[y, x] == numericUpDown1.Value && MainForm.editArea.Tool.IsPaintable(x, y, (int)numericUpDown2.Value, true))
                    {
                        first += 1;
                        if (first == 1)
                        {
                            Point p = new Point(x * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.X), y * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.Y));
                            MainForm.editArea.Frames[0].Foreground[y, x] = (int)numericUpDown2.Value;
                            MainForm.editArea.Draw(x, y, Graphics.FromImage(MainForm.editArea.Back), MainForm.userdata.thisColor);
                            MainForm.editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                            label4.Text = "Finished replacing block ID " + numericUpDown1.Value + " with " + numericUpDown2.Value + "."; //Checks for last row, not last block unfortunately
                            break;
                        }
                        else
                        {
                            label4.Text = "Replaced block ID " + numericUpDown1.Value + " with " + numericUpDown2.Value + ".";
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ThreadPool.QueueUserWorkItem(delegate (object param0)
            //{
            int incr = 0;
            int total = MainForm.editArea.CurFrame.Height - 1;

            if (numericUpDown1.Value >= 500 && numericUpDown1.Value <= 999 || numericUpDown1.Value == 0 && numericUpDown2.Value >= 500 && numericUpDown2.Value <= 999 || numericUpDown2.Value == 0)
            {

                for (int yy = 0; yy < MainForm.editArea.Frames[0].Height; yy++)
                {
                    for (int xx = 0; xx < MainForm.editArea.Frames[0].Width; xx++)
                    {
                        if (MainForm.editArea.Frames[0].Background[yy, xx] == numericUpDown1.Value)
                        {

                            if (MainForm.editArea.Tool.IsPaintable(xx, yy, (int)numericUpDown2.Value, true) && MainForm.editArea.Tool.IsPaintable(xx, yy, (int)numericUpDown2.Value, false))
                            {
                                MainForm.editArea.Frames[0].Background[yy, xx] = (int)numericUpDown2.Value;
                                //incfg += (int)rp.NU2.Value + ":" + editArea.CurFrame.Background[yy, xx] + ":" + xx + ":" + yy + ":";
                                Point p = new Point(xx * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.X), yy * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.Y));
                                if (MainForm.editArea.InvokeRequired)
                                {
                                    MainForm.editArea.Invoke((MethodInvoker)delegate
                                    {
                                        MainForm.editArea.Draw(xx, yy, Graphics.FromImage(MainForm.editArea.Back), MainForm.userdata.thisColor);
                                    });
                                }
                                else
                                {
                                    MainForm.editArea.Draw(xx, yy, Graphics.FromImage(MainForm.editArea.Back), MainForm.userdata.thisColor);

                                }
                                MainForm.editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                            }


                        }
                        if (progressBar1.InvokeRequired)
                        {
                            progressBar1.Invoke((MethodInvoker)delegate
                            {
                                double db = ((double)incr / total) * 100;
                                int value = Convert.ToInt32(db) > 100 ? 100 : Convert.ToInt32(db);
                                progressBar1.Value = value;
                            });
                        }
                        if (!progressBar1.InvokeRequired)
                        {
                            double db = ((double)incr / total) * 100;
                            int value = Convert.ToInt32(db) > 100 ? 100 : Convert.ToInt32(db);
                            progressBar1.Value = value;
                        }
                        incr += 1;
                    }

                }
            }
            if ((numericUpDown1.Value < 500 || numericUpDown1.Value >= 1001) && (numericUpDown2.Value < 500 || numericUpDown2.Value >= 1001))
            {
                for (int yy = 0; yy < MainForm.editArea.Frames[0].Height; yy++)
                {
                    for (int xx = 0; xx < MainForm.editArea.Frames[0].Width; xx++)
                    {
                        if (MainForm.editArea.Frames[0].Foreground[yy, xx] == numericUpDown1.Value)
                        {

                            if (MainForm.editArea.Tool.IsPaintable(xx, yy, (int)numericUpDown2.Value, true) && MainForm.editArea.Tool.IsPaintable(xx, yy, (int)numericUpDown2.Value, false))
                            {
                                //incfg += (int)rp.NU2.Value + ":" + editArea.CurFrame.Foreground[yy, xx] + ":" + xx + ":" + yy + ":";
                                MainForm.editArea.Frames[0].Foreground[yy, xx] = (int)numericUpDown2.Value;
                                Point p = new Point(xx * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.X), yy * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.Y));
                                if (MainForm.editArea.InvokeRequired)
                                {
                                    MainForm.editArea.Invoke((MethodInvoker)delegate
                                    {
                                        MainForm.editArea.Draw(xx, yy, Graphics.FromImage(MainForm.editArea.Back), MainForm.userdata.thisColor);
                                    });
                                }
                                else
                                {
                                    MainForm.editArea.Draw(xx, yy, Graphics.FromImage(MainForm.editArea.Back), MainForm.userdata.thisColor);

                                }
                                
                            }

                        }



                    }
                    if (progressBar1.InvokeRequired)
                    {
                        progressBar1.Invoke((MethodInvoker)delegate
                        {
                            double db = ((double)incr / total) * 100;
                            int value = Convert.ToInt32(db) > 100 ? 100 : Convert.ToInt32(db);
                            progressBar1.Value = value;
                        });
                    }
                    if (!progressBar1.InvokeRequired)
                    {
                        double db = ((double)incr / total) * 100;
                        int value = Convert.ToInt32(db) > 100 ? 100 : Convert.ToInt32(db);
                        progressBar1.Value = value;
                    }
                    incr += 1;

                }
            }
            MainForm.editArea.Invalidate();
            //});
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int first = 0;
            //int first1 = 0;
            for (int y = 0; y < MainForm.editArea.Frames[0].Height; y++)
            {
                for (int x = 0; x < MainForm.editArea.Frames[0].Width; x++)
                {
                    if (MainForm.editArea.Frames[0].Foreground[y, x] == numericUpDown1.Value && replaced[y, x] == 0)
                    {
                        first += 1;
                        if (first == 1)
                        {


                            Point p = new Point(x * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.X), y * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.Y));
                            Graphics gr = Graphics.FromImage(MainForm.editArea.Back1);
                            gr.DrawRectangle(new Pen(Color.Red), new Rectangle(x * 16, y * 16, 15, 15));
                            replaced[y, x] = 1;


                            MainForm.editArea.AutoScrollPosition = new Point(x * 16, y * 16);
                            break;
                        }
                    }
                    if (replaced[y, x] == 1)
                    {
                        Bitmap img1 = MainForm.foregroundBMD.Clone(new Rectangle(MainForm.foregroundBMI[MainForm.editArea.Frames[0].Foreground[y, x]] * 16, 0, 16, 16), MainForm.foregroundBMD.PixelFormat);
                        Graphics gr = Graphics.FromImage(MainForm.editArea.Back1);
                        gr.DrawImage(img1, new Point(x * 16, y * 16));
                        //gr.DrawRectangle(new Pen(Color.FromArgb(255, 0, 0, 0)), new Rectangle(x * 16, y * 16, 16, 16));
                        //gr.FillRectangle(new SolidBrush(Color.Transparent), new Rectangle(x * 16, y * 16, 16, 16));
                    }
                }
            }
            MainForm.editArea.Invalidate();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MainForm.editArea.Back1 = null;
            for (int y = 0; y < MainForm.editArea.Frames[0].Height; y++)
            {
                for (int x = 0; x < MainForm.editArea.Frames[0].Width; x++)
                {

                    replaced[y, x] = 0;
                    replaced1[y, x] = 0;
                    MainForm.editArea.Draw(x, y, Graphics.FromImage(MainForm.editArea.Back), MainForm.userdata.thisColor);
                    MainForm.editArea.Invalidate();
                }
            }
            MainForm.editArea.Back1 = MainForm.editArea.Back;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            for (int y = 0; y < MainForm.editArea.Frames[0].Height; y++)
            {
                for (int x = 0; x < MainForm.editArea.Frames[0].Width; x++)
                {
                    if (MainForm.editArea.Frames[0].Foreground[y, x] == numericUpDown1.Value && replaced1[y, x] == 0)
                    {


                        Point p = new Point(x * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.X), y * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.Y));
                        Graphics gr = Graphics.FromImage(MainForm.editArea.Back1);
                        gr.DrawRectangle(new Pen(Color.Red), new Rectangle(x * 16, y * 16, 15, 15));
                        replaced1[y, x] = 1;
                    }
                }
            }
            MainForm.editArea.Invalidate();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Thread thr = new Thread(replaceNotOwnedBlocks);
            thr.Start();
        }
        private void replaceNotOwnedBlocks()
        {
            if (!MainForm.debug)
            {
                int total1 = MainForm.editArea.Frames[0].Height - 1;
                int incr = 0;
                int totalReplaced = 0;
                if (label4.InvokeRequired)
                {
                    label4.Invoke((MethodInvoker)delegate
                    {
                        label4.Text = "Replacing blocks you don't own.";
                    });
                }
                for (int yy = 0; yy < MainForm.editArea.Frames[0].Height; yy++)
                {

                    for (int xx = 0; xx < MainForm.editArea.Frames[0].Width; xx++)
                    {

                        if (MainForm.decosBMI[MainForm.editArea.Frames[0].Foreground[yy, xx]] != 0)
                        {


                            if (!MainForm.editArea.ownedBid(MainForm.editArea.Frames[0].Foreground[yy, xx], 2))
                            {
                                MainForm.editArea.Frames[0].Foreground[yy, xx] = (int)numericUpDown2.Value;
                                if (bdata.getRotation(MainForm.editArea.Frames[0].Foreground[yy, xx], MainForm.editArea.Frames[0].BlockData[yy, xx]) != null)
                                {

                                    MainForm.editArea.Frames[0].BlockData[yy, xx] = (int)numericUpDown2.Value;
                                }
                                //incfg += (int)rp.NU2.Value + ":" + editArea.CurFrame.Background[yy, xx] + ":" + xx + ":" + yy + ":";
                                totalReplaced += 1;

                            }
                        }
                        if (MainForm.miscBMI[MainForm.editArea.Frames[0].Foreground[yy, xx]] != 0)
                        {
                            if (MainForm.editArea.Frames[0].Foreground[yy, xx] == 385)
                            {
                                if (!MainForm.accs[MainForm.userdata.username].payvault.Contains("goldmember"))
                                {
                                    MainForm.editArea.Frames[0].BlockData[yy, xx] = 0;
                                    totalReplaced += 1;
                                }
                            }
                            if (!MainForm.editArea.ownedBid(MainForm.editArea.Frames[0].Foreground[yy, xx], 1))
                            {

                                MainForm.editArea.Frames[0].Foreground[yy, xx] = (int)numericUpDown2.Value;
                                MainForm.editArea.Frames[0].BlockData[yy, xx] = 0;
                                totalReplaced += 1;
                                //incfg += (int)rp.NU2.Value + ":" + editArea.CurFrame.Background[yy, xx] + ":" + xx + ":" + yy + ":";
                            }
                        }

                        if (MainForm.foregroundBMI[MainForm.editArea.Frames[0].Foreground[yy, xx]] != 0)
                        {

                            if (!MainForm.editArea.ownedBid(MainForm.editArea.Frames[0].Foreground[yy, xx], 0))
                            {

                                MainForm.editArea.Frames[0].Foreground[yy, xx] = (int)numericUpDown2.Value;
                                MainForm.editArea.Frames[0].BlockData[yy, xx] = 0;
                                totalReplaced += 1;
                                //incfg += (int)rp.NU2.Value + ":" + editArea.CurFrame.Background[yy, xx] + ":" + xx + ":" + yy + ":";


                            }
                        }
                        if (MainForm.backgroundBMI[MainForm.editArea.Frames[0].Background[yy, xx]] != 0)
                        {

                            if (!MainForm.editArea.ownedBid(MainForm.editArea.Frames[0].Background[yy, xx], 3))
                            {

                                MainForm.editArea.Frames[0].Background[yy, xx] = (int)numericUpDown2.Value;
                                totalReplaced += 1;
                                //incfg += (int)rp.NU2.Value + ":" + editArea.CurFrame.Background[yy, xx] + ":" + xx + ":" + yy + ":";


                            }
                        }
                        if (MainForm.foregroundBMI[MainForm.editArea.Frames[0].Foreground[yy, xx]] == 0)
                        {

                            if (MainForm.editArea.Frames[0].Foreground[yy, xx] != 0)
                            {

                                MainForm.editArea.Frames[0].Foreground[yy, xx] = (int)numericUpDown2.Value;
                                totalReplaced += 1;
                                //incfg += (int)rp.NU2.Value + ":" + editArea.CurFrame.Background[yy, xx] + ":" + xx + ":" + yy + ":";


                            }
                        }
                        if (MainForm.backgroundBMI[MainForm.editArea.Frames[0].Background[yy, xx]] == 0)
                        {
                            if (MainForm.editArea.Frames[0].Background[yy, xx] != 0)
                            {
                                MainForm.editArea.Frames[0].Background[yy, xx] = (int)numericUpDown2.Value;
                                totalReplaced += 1;
                            }
                        }
                        Point p = new Point(xx * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.X), yy * 16 - Math.Abs(MainForm.editArea.AutoScrollPosition.Y));


                        if (MainForm.editArea.InvokeRequired)
                        {
                            MainForm.editArea.Invoke((MethodInvoker)delegate
                            {
                                MainForm.editArea.Draw(xx, yy, Graphics.FromImage(MainForm.editArea.Back), MainForm.userdata.thisColor);
                                MainForm.editArea.Invalidate(new Rectangle(p, new Size(16, 16)));

                            });
                        }
                        else
                        {
                            MainForm.editArea.Draw(xx, yy, Graphics.FromImage(MainForm.editArea.Back), MainForm.userdata.thisColor);
                            MainForm.editArea.Invalidate(new Rectangle(p, new Size(16, 16)));
                        }

                    }
                    if (progressBar1.InvokeRequired)
                    {
                        progressBar1.Invoke((MethodInvoker)delegate
                        {
                            double db = ((double)incr / total1) * 100;
                            progressBar1.Value = Convert.ToInt32(db);
                        });
                    }
                    if (!progressBar1.InvokeRequired)
                    {
                        double db = ((double)incr / total1) * 100;
                        progressBar1.Value = Convert.ToInt32(db);
                    }
                    incr += 1;
                }
                if (label4.InvokeRequired)
                {
                    label4.Invoke((MethodInvoker)delegate
                    {
                        label4.Text = "Replaced " + totalReplaced + " blocks you don't own.";
                    });
                }

            }

            //MessageBox.Show("Unowned blocks have been replaced.","Unowned blocks replaced",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button2.PerformClick();
            }
        }

        private void Replacer_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.editArea.Back1 = null;
            for (int y = 0; y < MainForm.editArea.Frames[0].Height; y++)
            {
                for (int x = 0; x < MainForm.editArea.Frames[0].Width; x++)
                {
                    replaced[y, x] = 0;
                    replaced1[y, x] = 0;
                    MainForm.editArea.Draw(x, y, Graphics.FromImage(MainForm.editArea.Back), MainForm.userdata.thisColor);
                    MainForm.editArea.Invalidate();

                }
            }
            MainForm.editArea.Back1 = MainForm.editArea.Back;
        }
    }
}
