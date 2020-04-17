namespace Stl2Blueprint
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
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
        private void InitializeComponent ()
        {
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblFile = new System.Windows.Forms.Label();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtBlueprintName = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblTris = new System.Windows.Forms.Label();
            this.txtSizeX = new System.Windows.Forms.TextBox();
            this.txtSizeY = new System.Windows.Forms.TextBox();
            this.txtSizeZ = new System.Windows.Forms.TextBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.lblBlockCount = new System.Windows.Forms.Label();
            this.txtResolution = new System.Windows.Forms.TextBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.btnColor = new System.Windows.Forms.Button();
            this.lblColor = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.lblName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(27, 24);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.OnOpenFileClicked);
            // 
            // fileDialog
            // 
            this.fileDialog.Filter = "Model (*.stl)|*.stl";
            this.fileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OnFileOpened);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 415);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(776, 23);
            this.progressBar.TabIndex = 1;
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(109, 33);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(59, 13);
            this.lblFile.TabIndex = 2;
            this.lblFile.Text = "example.stl";
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(27, 54);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFolder.TabIndex = 3;
            this.btnOpenFolder.Text = "Blueprints";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.OnOpenFolderClicked);
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(109, 63);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(36, 13);
            this.lblOutput.TabIndex = 4;
            this.lblOutput.Text = "Folder";
            // 
            // txtBlueprintName
            // 
            this.txtBlueprintName.ForeColor = System.Drawing.Color.Gray;
            this.txtBlueprintName.Location = new System.Drawing.Point(27, 109);
            this.txtBlueprintName.Name = "txtBlueprintName";
            this.txtBlueprintName.Size = new System.Drawing.Size(100, 20);
            this.txtBlueprintName.TabIndex = 5;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 386);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.OnStartClicked);
            // 
            // lblTris
            // 
            this.lblTris.AutoSize = true;
            this.lblTris.Location = new System.Drawing.Point(109, 16);
            this.lblTris.Name = "lblTris";
            this.lblTris.Size = new System.Drawing.Size(13, 13);
            this.lblTris.TabIndex = 11;
            this.lblTris.Text = "0";
            // 
            // txtSizeX
            // 
            this.txtSizeX.Enabled = false;
            this.txtSizeX.Location = new System.Drawing.Point(27, 161);
            this.txtSizeX.MaxLength = 9;
            this.txtSizeX.Name = "txtSizeX";
            this.txtSizeX.Size = new System.Drawing.Size(100, 20);
            this.txtSizeX.TabIndex = 12;
            this.txtSizeX.TextChanged += new System.EventHandler(this.OnSizeXChanged);
            this.txtSizeX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // txtSizeY
            // 
            this.txtSizeY.Enabled = false;
            this.txtSizeY.Location = new System.Drawing.Point(27, 188);
            this.txtSizeY.MaxLength = 9;
            this.txtSizeY.Name = "txtSizeY";
            this.txtSizeY.Size = new System.Drawing.Size(100, 20);
            this.txtSizeY.TabIndex = 13;
            this.txtSizeY.TextChanged += new System.EventHandler(this.OnSizeYChanged);
            this.txtSizeY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // txtSizeZ
            // 
            this.txtSizeZ.Enabled = false;
            this.txtSizeZ.Location = new System.Drawing.Point(27, 215);
            this.txtSizeZ.MaxLength = 9;
            this.txtSizeZ.Name = "txtSizeZ";
            this.txtSizeZ.Size = new System.Drawing.Size(100, 20);
            this.txtSizeZ.TabIndex = 14;
            this.txtSizeZ.TextChanged += new System.EventHandler(this.OnSizeZChanged);
            this.txtSizeZ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(27, 145);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(27, 13);
            this.lblSize.TabIndex = 15;
            this.lblSize.Text = "Size";
            // 
            // lblBlockCount
            // 
            this.lblBlockCount.AutoSize = true;
            this.lblBlockCount.Location = new System.Drawing.Point(94, 395);
            this.lblBlockCount.Name = "lblBlockCount";
            this.lblBlockCount.Size = new System.Drawing.Size(51, 13);
            this.lblBlockCount.TabIndex = 16;
            this.lblBlockCount.Text = "Blocks: 0";
            // 
            // txtResolution
            // 
            this.txtResolution.Enabled = false;
            this.txtResolution.Location = new System.Drawing.Point(162, 161);
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.Size = new System.Drawing.Size(100, 20);
            this.txtResolution.TabIndex = 17;
            this.txtResolution.TextChanged += new System.EventHandler(this.OnResolutionChanged);
            this.txtResolution.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumberKeyFilter);
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Location = new System.Drawing.Point(159, 145);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(34, 13);
            this.lblResolution.TabIndex = 18;
            this.lblResolution.Text = "Scale";
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.Color = System.Drawing.Color.White;
            this.colorDialog.FullOpen = true;
            // 
            // btnColor
            // 
            this.btnColor.BackColor = System.Drawing.Color.White;
            this.btnColor.Location = new System.Drawing.Point(27, 270);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(75, 23);
            this.btnColor.TabIndex = 19;
            this.btnColor.UseVisualStyleBackColor = false;
            this.btnColor.Click += new System.EventHandler(this.OnColorClicked);
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(27, 258);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(31, 13);
            this.lblColor.TabIndex = 20;
            this.lblColor.Text = "Color";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(27, 93);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 21;
            this.lblName.Text = "Name";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.btnColor);
            this.Controls.Add(this.lblResolution);
            this.Controls.Add(this.txtResolution);
            this.Controls.Add(this.lblBlockCount);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.txtSizeZ);
            this.Controls.Add(this.txtSizeY);
            this.Controls.Add(this.txtSizeX);
            this.Controls.Add(this.lblTris);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtBlueprintName);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnOpenFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Main";
            this.Text = "STL to Blueprint";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtBlueprintName;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblTris;
        private System.Windows.Forms.TextBox txtSizeX;
        private System.Windows.Forms.TextBox txtSizeY;
        private System.Windows.Forms.TextBox txtSizeZ;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label lblBlockCount;
        private System.Windows.Forms.TextBox txtResolution;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label lblName;
    }
}

