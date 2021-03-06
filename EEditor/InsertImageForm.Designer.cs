﻿namespace EEditor
{
    partial class InsertImageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.loadImageButton = new System.Windows.Forms.Button();
            this.checkBoxBlocks = new System.Windows.Forms.CheckBox();
            this.checkBoxBackground = new System.Windows.Forms.CheckBox();
            this.CreateImagegroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ActionBlockscheckBox = new System.Windows.Forms.CheckBox();
            this.MorphablecheckBox = new System.Windows.Forms.CheckBox();
            this.ConvertergroupBox = new System.Windows.Forms.GroupBox();
            this.EEditorRadiobutton = new System.Windows.Forms.RadioButton();
            this.EEArtistRadioButton = new System.Windows.Forms.RadioButton();
            this.CreateImagegroupBox.SuspendLayout();
            this.ConvertergroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadImageButton
            // 
            this.loadImageButton.Location = new System.Drawing.Point(12, 231);
            this.loadImageButton.Name = "loadImageButton";
            this.loadImageButton.Size = new System.Drawing.Size(134, 27);
            this.loadImageButton.TabIndex = 1;
            this.loadImageButton.Text = "Select image file";
            this.loadImageButton.UseVisualStyleBackColor = true;
            this.loadImageButton.Click += new System.EventHandler(this.loadImageButton_Click);
            // 
            // checkBoxBlocks
            // 
            this.checkBoxBlocks.AutoSize = true;
            this.checkBoxBlocks.Checked = true;
            this.checkBoxBlocks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBlocks.Location = new System.Drawing.Point(6, 42);
            this.checkBoxBlocks.Name = "checkBoxBlocks";
            this.checkBoxBlocks.Size = new System.Drawing.Size(58, 17);
            this.checkBoxBlocks.TabIndex = 5;
            this.checkBoxBlocks.Text = "Blocks";
            this.checkBoxBlocks.UseVisualStyleBackColor = true;
            this.checkBoxBlocks.CheckedChanged += new System.EventHandler(this.checkBoxBlocks_CheckedChanged);
            // 
            // checkBoxBackground
            // 
            this.checkBoxBackground.AutoSize = true;
            this.checkBoxBackground.Checked = true;
            this.checkBoxBackground.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBackground.Location = new System.Drawing.Point(6, 19);
            this.checkBoxBackground.Name = "checkBoxBackground";
            this.checkBoxBackground.Size = new System.Drawing.Size(84, 17);
            this.checkBoxBackground.TabIndex = 6;
            this.checkBoxBackground.Text = "Background";
            this.checkBoxBackground.UseVisualStyleBackColor = true;
            this.checkBoxBackground.CheckedChanged += new System.EventHandler(this.checkBoxBackground_CheckedChanged);
            // 
            // CreateImagegroupBox
            // 
            this.CreateImagegroupBox.Controls.Add(this.label1);
            this.CreateImagegroupBox.Controls.Add(this.ActionBlockscheckBox);
            this.CreateImagegroupBox.Controls.Add(this.MorphablecheckBox);
            this.CreateImagegroupBox.Controls.Add(this.checkBoxBackground);
            this.CreateImagegroupBox.Controls.Add(this.checkBoxBlocks);
            this.CreateImagegroupBox.Location = new System.Drawing.Point(12, 84);
            this.CreateImagegroupBox.Name = "CreateImagegroupBox";
            this.CreateImagegroupBox.Size = new System.Drawing.Size(134, 141);
            this.CreateImagegroupBox.TabIndex = 7;
            this.CreateImagegroupBox.TabStop = false;
            this.CreateImagegroupBox.Text = "Create image using";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Special Blocks";
            // 
            // ActionBlockscheckBox
            // 
            this.ActionBlockscheckBox.AutoSize = true;
            this.ActionBlockscheckBox.Location = new System.Drawing.Point(6, 111);
            this.ActionBlockscheckBox.Name = "ActionBlockscheckBox";
            this.ActionBlockscheckBox.Size = new System.Drawing.Size(91, 17);
            this.ActionBlockscheckBox.TabIndex = 10;
            this.ActionBlockscheckBox.Text = "Action Blocks";
            this.ActionBlockscheckBox.UseVisualStyleBackColor = true;
            this.ActionBlockscheckBox.CheckedChanged += new System.EventHandler(this.ActionBlockscheckBox_CheckedChanged);
            // 
            // MorphablecheckBox
            // 
            this.MorphablecheckBox.AutoSize = true;
            this.MorphablecheckBox.Location = new System.Drawing.Point(6, 88);
            this.MorphablecheckBox.Name = "MorphablecheckBox";
            this.MorphablecheckBox.Size = new System.Drawing.Size(76, 17);
            this.MorphablecheckBox.TabIndex = 9;
            this.MorphablecheckBox.Text = "Morphable";
            this.MorphablecheckBox.UseVisualStyleBackColor = true;
            this.MorphablecheckBox.CheckedChanged += new System.EventHandler(this.MorphablecheckBox_CheckedChanged);
            // 
            // ConvertergroupBox
            // 
            this.ConvertergroupBox.Controls.Add(this.EEArtistRadioButton);
            this.ConvertergroupBox.Controls.Add(this.EEditorRadiobutton);
            this.ConvertergroupBox.Location = new System.Drawing.Point(12, 12);
            this.ConvertergroupBox.Name = "ConvertergroupBox";
            this.ConvertergroupBox.Size = new System.Drawing.Size(134, 66);
            this.ConvertergroupBox.TabIndex = 8;
            this.ConvertergroupBox.TabStop = false;
            this.ConvertergroupBox.Text = "Choose Converter";
            // 
            // EEditorRadiobutton
            // 
            this.EEditorRadiobutton.AutoSize = true;
            this.EEditorRadiobutton.Checked = true;
            this.EEditorRadiobutton.Location = new System.Drawing.Point(6, 19);
            this.EEditorRadiobutton.Name = "EEditorRadiobutton";
            this.EEditorRadiobutton.Size = new System.Drawing.Size(59, 17);
            this.EEditorRadiobutton.TabIndex = 0;
            this.EEditorRadiobutton.TabStop = true;
            this.EEditorRadiobutton.Text = "EEditor";
            this.EEditorRadiobutton.UseVisualStyleBackColor = true;
            // 
            // EEArtistRadioButton
            // 
            this.EEArtistRadioButton.AutoSize = true;
            this.EEArtistRadioButton.Location = new System.Drawing.Point(6, 36);
            this.EEArtistRadioButton.Name = "EEArtistRadioButton";
            this.EEArtistRadioButton.Size = new System.Drawing.Size(62, 17);
            this.EEArtistRadioButton.TabIndex = 1;
            this.EEArtistRadioButton.Text = "EEArtist";
            this.EEArtistRadioButton.UseVisualStyleBackColor = true;
            // 
            // InsertImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(164, 270);
            this.Controls.Add(this.ConvertergroupBox);
            this.Controls.Add(this.CreateImagegroupBox);
            this.Controls.Add(this.loadImageButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "InsertImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Insert image";
            this.Load += new System.EventHandler(this.InsertImageForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InsertImageForm_KeyDown);
            this.CreateImagegroupBox.ResumeLayout(false);
            this.CreateImagegroupBox.PerformLayout();
            this.ConvertergroupBox.ResumeLayout(false);
            this.ConvertergroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button loadImageButton;
        private System.Windows.Forms.CheckBox checkBoxBlocks;
        private System.Windows.Forms.CheckBox checkBoxBackground;
        private System.Windows.Forms.GroupBox CreateImagegroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ActionBlockscheckBox;
        private System.Windows.Forms.CheckBox MorphablecheckBox;
        private System.Windows.Forms.GroupBox ConvertergroupBox;
        private System.Windows.Forms.RadioButton EEArtistRadioButton;
        private System.Windows.Forms.RadioButton EEditorRadiobutton;
    }
}