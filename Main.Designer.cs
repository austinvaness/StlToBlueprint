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
            this.components = new System.ComponentModel.Container();
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
            this.lblInfo = new System.Windows.Forms.Label();
            this.txtResolution = new System.Windows.Forms.TextBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.btnColor = new System.Windows.Forms.Button();
            this.lblColor = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.lblName = new System.Windows.Forms.Label();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.comboSkin = new System.Windows.Forms.ComboBox();
            this.lblSkin = new System.Windows.Forms.Label();
            this.background = new System.ComponentModel.BackgroundWorker();
            this.chkHollow = new System.Windows.Forms.CheckBox();
            this.chkSlopes = new System.Windows.Forms.CheckBox();
            this.lblSizeMeters = new System.Windows.Forms.Label();
            this.chkAccuracy = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
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
            this.toolTip.SetToolTip(this.txtSizeX, "Size of the model x-axis in blocks.");
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
            this.toolTip.SetToolTip(this.txtSizeY, "Size of the model y-axis in blocks.");
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
            this.toolTip.SetToolTip(this.txtSizeZ, "Size of the model z-axis in blocks.");
            this.txtSizeZ.TextChanged += new System.EventHandler(this.OnSizeZChanged);
            this.txtSizeZ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(27, 145);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(57, 13);
            this.lblSize.TabIndex = 15;
            this.lblSize.Text = "Block Size";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(94, 395);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(51, 13);
            this.lblInfo.TabIndex = 16;
            this.lblInfo.Text = "Blocks: 0";
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
            this.lblColor.Location = new System.Drawing.Point(27, 254);
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
            // comboType
            // 
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.FormattingEnabled = true;
            this.comboType.Items.AddRange(new object[] {
            "Large",
            "Small"});
            this.comboType.Location = new System.Drawing.Point(27, 328);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(121, 21);
            this.comboType.TabIndex = 22;
            this.comboType.SelectedIndexChanged += new System.EventHandler(this.OnCubeSizeChanged);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(27, 312);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(31, 13);
            this.lblType.TabIndex = 23;
            this.lblType.Text = "Type";
            // 
            // comboSkin
            // 
            this.comboSkin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSkin.FormattingEnabled = true;
            this.comboSkin.Items.AddRange(new object[] {
            "Default",
            "DigitalCamouflage",
            "CarbonFibre",
            "Clean",
            "Golden",
            "Silver",
            "Glamour",
            "Disco",
            "Wood",
            "Mossy",
            "Battered",
            "CowMooFlage",
            "RustNonColorable",
            "Rusty",
            "Frozen"});
            this.comboSkin.Location = new System.Drawing.Point(162, 328);
            this.comboSkin.Name = "comboSkin";
            this.comboSkin.Size = new System.Drawing.Size(121, 21);
            this.comboSkin.TabIndex = 24;
            this.comboSkin.SelectedIndexChanged += new System.EventHandler(this.OnSkinTypeChanged);
            // 
            // lblSkin
            // 
            this.lblSkin.AutoSize = true;
            this.lblSkin.Location = new System.Drawing.Point(162, 311);
            this.lblSkin.Name = "lblSkin";
            this.lblSkin.Size = new System.Drawing.Size(28, 13);
            this.lblSkin.TabIndex = 25;
            this.lblSkin.Text = "Skin";
            // 
            // background
            // 
            this.background.WorkerReportsProgress = true;
            this.background.WorkerSupportsCancellation = true;
            // 
            // chkHollow
            // 
            this.chkHollow.AutoSize = true;
            this.chkHollow.Checked = true;
            this.chkHollow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHollow.Location = new System.Drawing.Point(15, 367);
            this.chkHollow.Name = "chkHollow";
            this.chkHollow.Size = new System.Drawing.Size(58, 17);
            this.chkHollow.TabIndex = 26;
            this.chkHollow.Text = "Hollow";
            this.toolTip.SetToolTip(this.chkHollow, "Makes the blueprint hollow.");
            this.chkHollow.UseVisualStyleBackColor = true;
            this.chkHollow.CheckedChanged += new System.EventHandler(this.OnHollowChanged);
            // 
            // chkSlopes
            // 
            this.chkSlopes.AutoSize = true;
            this.chkSlopes.Location = new System.Drawing.Point(80, 367);
            this.chkSlopes.Name = "chkSlopes";
            this.chkSlopes.Size = new System.Drawing.Size(58, 17);
            this.chkSlopes.TabIndex = 27;
            this.chkSlopes.Text = "Slopes";
            this.toolTip.SetToolTip(this.chkSlopes, "Adds slopes to the blueprint.\r\nThis feature is not yet finished.");
            this.chkSlopes.UseVisualStyleBackColor = true;
            this.chkSlopes.CheckedChanged += new System.EventHandler(this.OnUseSlopesChanged);
            // 
            // lblSizeMeters
            // 
            this.lblSizeMeters.AutoSize = true;
            this.lblSizeMeters.Location = new System.Drawing.Point(162, 188);
            this.lblSizeMeters.Name = "lblSizeMeters";
            this.lblSizeMeters.Size = new System.Drawing.Size(0, 13);
            this.lblSizeMeters.TabIndex = 29;
            // 
            // chkAccuracy
            // 
            this.chkAccuracy.AutoSize = true;
            this.chkAccuracy.Location = new System.Drawing.Point(145, 367);
            this.chkAccuracy.Name = "chkAccuracy";
            this.chkAccuracy.Size = new System.Drawing.Size(96, 17);
            this.chkAccuracy.TabIndex = 30;
            this.chkAccuracy.Text = "Less Accuracy";
            this.toolTip.SetToolTip(this.chkAccuracy, "Speeds up processing time at the cost of less accurate blueprints.");
            this.chkAccuracy.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chkAccuracy);
            this.Controls.Add(this.lblSizeMeters);
            this.Controls.Add(this.chkSlopes);
            this.Controls.Add(this.chkHollow);
            this.Controls.Add(this.lblSkin);
            this.Controls.Add(this.comboSkin);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.comboType);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.btnColor);
            this.Controls.Add(this.lblResolution);
            this.Controls.Add(this.txtResolution);
            this.Controls.Add(this.lblInfo);
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
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.TextBox txtResolution;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox comboSkin;
        private System.Windows.Forms.Label lblSkin;
        private System.ComponentModel.BackgroundWorker background;
        private System.Windows.Forms.CheckBox chkHollow;
        private System.Windows.Forms.CheckBox chkSlopes;
        private System.Windows.Forms.Label lblSizeMeters;
        private System.Windows.Forms.CheckBox chkAccuracy;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

