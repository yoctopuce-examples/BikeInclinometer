namespace BikeInclinometer
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.loadButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.reset = new System.Windows.Forms.Button();
            this.UpdateListTimer = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.selectFile = new System.Windows.Forms.Button();
            this.inputFileTextbox = new System.Windows.Forms.TextBox();
            this.outFileTextBox = new System.Windows.Forms.TextBox();
            this.selectOutFileButton = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(511, 186);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(117, 23);
            this.loadButton.TabIndex = 0;
            this.loadButton.Text = "Generate Video";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(493, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // reset
            // 
            this.reset.Location = new System.Drawing.Point(511, 12);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(117, 23);
            this.reset.TabIndex = 2;
            this.reset.Text = "Configure Yocto-3D";
            this.reset.UseVisualStyleBackColor = true;
            this.reset.Click += new System.EventHandler(this.button2_Click);
            // 
            // UpdateListTimer
            // 
            this.UpdateListTimer.Tick += new System.EventHandler(this.UpdateListTimer_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Enabled = false;
            this.progressBar1.Location = new System.Drawing.Point(12, 186);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(493, 23);
            this.progressBar1.TabIndex = 11;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // selectFile
            // 
            this.selectFile.Location = new System.Drawing.Point(511, 70);
            this.selectFile.Name = "selectFile";
            this.selectFile.Size = new System.Drawing.Size(117, 23);
            this.selectFile.TabIndex = 12;
            this.selectFile.Text = "Select File";
            this.selectFile.UseVisualStyleBackColor = true;
            this.selectFile.Click += new System.EventHandler(this.selectFile_Click);
            // 
            // inputFileTextbox
            // 
            this.inputFileTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.inputFileTextbox.Location = new System.Drawing.Point(12, 72);
            this.inputFileTextbox.Name = "inputFileTextbox";
            this.inputFileTextbox.Size = new System.Drawing.Size(493, 20);
            this.inputFileTextbox.TabIndex = 13;
            this.inputFileTextbox.Text = "C:\\TMP\\GOPR0217.MP4";
            // 
            // outFileTextBox
            // 
            this.outFileTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.outFileTextBox.Location = new System.Drawing.Point(12, 126);
            this.outFileTextBox.Name = "outFileTextBox";
            this.outFileTextBox.Size = new System.Drawing.Size(493, 20);
            this.outFileTextBox.TabIndex = 14;
            this.outFileTextBox.Text = "C:\\TMP\\OUT.AVI";
            // 
            // selectOutFileButton
            // 
            this.selectOutFileButton.Location = new System.Drawing.Point(511, 124);
            this.selectOutFileButton.Name = "selectOutFileButton";
            this.selectOutFileButton.Size = new System.Drawing.Size(117, 23);
            this.selectOutFileButton.TabIndex = 15;
            this.selectOutFileButton.Text = "Select File";
            this.selectOutFileButton.UseVisualStyleBackColor = true;
            this.selectOutFileButton.Click += new System.EventHandler(this.selectOutFileButton_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Input video file:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Output video file:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 228);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.selectOutFileButton);
            this.Controls.Add(this.outFileTextBox);
            this.Controls.Add(this.inputFileTextbox);
            this.Controls.Add(this.selectFile);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.reset);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.loadButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button reset;
        private System.Windows.Forms.Timer UpdateListTimer;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button selectFile;
        private System.Windows.Forms.TextBox inputFileTextbox;
        private System.Windows.Forms.TextBox outFileTextBox;
        private System.Windows.Forms.Button selectOutFileButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

