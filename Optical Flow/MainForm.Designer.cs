namespace Optical_Flow
{
    partial class MainForm
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
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.b_AddVideo = new System.Windows.Forms.Button();
            this.b_StartStop = new System.Windows.Forms.Button();
            this.p_totalProgress = new System.Windows.Forms.ProgressBar();
            this.b_outputFileLocation = new System.Windows.Forms.Button();
            this.b_Settings = new System.Windows.Forms.Button();
            this.t_OutputFileLocation = new System.Windows.Forms.TextBox();
            this.t_Files = new System.Windows.Forms.DataGridView();
            this.File = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Progress = new System.Windows.Forms.DataGridViewImageColumn();
            this.Remove = new System.Windows.Forms.DataGridViewButtonColumn();
            this.l_Progress = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.t_Files)).BeginInit();
            this.SuspendLayout();
            // 
            // b_AddVideo
            // 
            this.b_AddVideo.Location = new System.Drawing.Point(93, 12);
            this.b_AddVideo.Name = "b_AddVideo";
            this.b_AddVideo.Size = new System.Drawing.Size(75, 23);
            this.b_AddVideo.TabIndex = 1;
            this.b_AddVideo.Text = "Add Video";
            this.b_AddVideo.UseVisualStyleBackColor = true;
            this.b_AddVideo.Click += new System.EventHandler(this.OpenFile);
            // 
            // b_StartStop
            // 
            this.b_StartStop.Location = new System.Drawing.Point(12, 12);
            this.b_StartStop.Name = "b_StartStop";
            this.b_StartStop.Size = new System.Drawing.Size(75, 23);
            this.b_StartStop.TabIndex = 2;
            this.b_StartStop.Text = "Start/Stop";
            this.b_StartStop.UseMnemonic = false;
            this.b_StartStop.UseVisualStyleBackColor = true;
            // 
            // p_totalProgress
            // 
            this.p_totalProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.p_totalProgress.Location = new System.Drawing.Point(12, 464);
            this.p_totalProgress.Name = "p_totalProgress";
            this.p_totalProgress.Size = new System.Drawing.Size(776, 23);
            this.p_totalProgress.TabIndex = 3;
            // 
            // b_outputFileLocation
            // 
            this.b_outputFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.b_outputFileLocation.Location = new System.Drawing.Point(13, 435);
            this.b_outputFileLocation.Name = "b_outputFileLocation";
            this.b_outputFileLocation.Size = new System.Drawing.Size(75, 23);
            this.b_outputFileLocation.TabIndex = 4;
            this.b_outputFileLocation.Text = "Data Output";
            this.b_outputFileLocation.UseMnemonic = false;
            this.b_outputFileLocation.UseVisualStyleBackColor = true;
            // 
            // b_Settings
            // 
            this.b_Settings.Location = new System.Drawing.Point(175, 12);
            this.b_Settings.Name = "b_Settings";
            this.b_Settings.Size = new System.Drawing.Size(75, 23);
            this.b_Settings.TabIndex = 5;
            this.b_Settings.Text = "Settings";
            this.b_Settings.UseMnemonic = false;
            this.b_Settings.UseVisualStyleBackColor = true;
            // 
            // t_OutputFileLocation
            // 
            this.t_OutputFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.t_OutputFileLocation.Location = new System.Drawing.Point(93, 435);
            this.t_OutputFileLocation.Name = "t_OutputFileLocation";
            this.t_OutputFileLocation.ReadOnly = true;
            this.t_OutputFileLocation.Size = new System.Drawing.Size(695, 20);
            this.t_OutputFileLocation.TabIndex = 6;
            // 
            // t_Files
            // 
            this.t_Files.AllowUserToAddRows = false;
            this.t_Files.AllowUserToDeleteRows = false;
            this.t_Files.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.t_Files.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.t_Files.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.t_Files.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.File,
            this.Progress,
            this.Remove});
            this.t_Files.Location = new System.Drawing.Point(13, 41);
            this.t_Files.Name = "t_Files";
            this.t_Files.ReadOnly = true;
            this.t_Files.RowHeadersVisible = false;
            this.t_Files.Size = new System.Drawing.Size(775, 388);
            this.t_Files.TabIndex = 7;
            // 
            // File
            // 
            this.File.HeaderText = "File";
            this.File.Name = "File";
            this.File.ReadOnly = true;
            // 
            // Progress
            // 
            this.Progress.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Progress.HeaderText = "Progress";
            this.Progress.Name = "Progress";
            this.Progress.ReadOnly = true;
            this.Progress.Width = 54;
            // 
            // Remove
            // 
            this.Remove.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Remove.HeaderText = "Remove";
            this.Remove.Name = "Remove";
            this.Remove.ReadOnly = true;
            this.Remove.Width = 53;
            // 
            // l_Progress
            // 
            this.l_Progress.AutoSize = true;
            this.l_Progress.Location = new System.Drawing.Point(12, 469);
            this.l_Progress.Name = "l_Progress";
            this.l_Progress.Size = new System.Drawing.Size(48, 13);
            this.l_Progress.TabIndex = 8;
            this.l_Progress.Text = "Progress";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 499);
            this.Controls.Add(this.l_Progress);
            this.Controls.Add(this.t_Files);
            this.Controls.Add(this.t_OutputFileLocation);
            this.Controls.Add(this.b_Settings);
            this.Controls.Add(this.b_outputFileLocation);
            this.Controls.Add(this.p_totalProgress);
            this.Controls.Add(this.b_StartStop);
            this.Controls.Add(this.b_AddVideo);
            this.Name = "MainForm";
            this.Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.t_Files)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button b_AddVideo;
        private System.Windows.Forms.Button b_StartStop;
        private System.Windows.Forms.ProgressBar p_totalProgress;
        private System.Windows.Forms.Button b_outputFileLocation;
        private System.Windows.Forms.Button b_Settings;
        private System.Windows.Forms.TextBox t_OutputFileLocation;
        private System.Windows.Forms.DataGridView t_Files;
        private System.Windows.Forms.DataGridViewTextBoxColumn File;
        private System.Windows.Forms.DataGridViewImageColumn Progress;
        private System.Windows.Forms.DataGridViewButtonColumn Remove;
        private System.Windows.Forms.Label l_Progress;
    }
}