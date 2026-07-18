namespace SkillBuilderPro.WinForms;

partial class ApiDrillsForm
{
    private System.ComponentModel.IContainer components = null;

    // Instructor pattern (Deepali's Form1): grid named dg*, buttons named btn*.
    private DataGridView dgDrills;
    private Button btnLoad;
    private Button btnWatchVideo;   // ← ADDED HERE
    private ComboBox cboSport;
    private Label lblSport;
    private Label lblStatus;
    
    

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        dgDrills = new DataGridView();
        btnLoad = new Button();
        cboSport = new ComboBox();
        lblSport = new Label();
        lblStatus = new Label();
        ((System.ComponentModel.ISupportInitialize)dgDrills).BeginInit();
        SuspendLayout();

        // lblSport
        lblSport.AutoSize = true;
        lblSport.BackColor = Color.Transparent;
        lblSport.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblSport.ForeColor = Color.White;
        lblSport.Location = new Point(24, 27);
        lblSport.Name = "lblSport";
        lblSport.Size = new Size(58, 21);
        lblSport.Text = "Sport:";

        // cboSport
        // DropDownList (not DropDown) makes it read-only — the user picks, never types.
        // Prevents typos reaching the API as filter values.
        cboSport.BackColor = Color.FromArgb(40, 40, 40);
        cboSport.DropDownStyle = ComboBoxStyle.DropDownList;
        cboSport.Font = new Font("Segoe UI", 11F);
        cboSport.ForeColor = Color.White;
        cboSport.FlatStyle = FlatStyle.Flat;
        cboSport.Location = new Point(92, 24);
        cboSport.Name = "cboSport";
        cboSport.Size = new Size(200, 29);

        // btnLoad
        btnLoad.BackColor = Color.FromArgb(200, 16, 46);
        btnLoad.FlatAppearance.BorderSize = 0;
        btnLoad.FlatStyle = FlatStyle.Flat;
        btnLoad.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnLoad.ForeColor = Color.White;
        btnLoad.Location = new Point(312, 22);
        btnLoad.Name = "btnLoad";
        btnLoad.Size = new Size(125, 38);
        btnLoad.Text = "LOAD DRILLS";
        btnLoad.UseVisualStyleBackColor = false;
        btnLoad.Click += btnLoad_Click;

        // btnWatchVideo  ← ADDED BLOCK
        btnWatchVideo = new Button
        {
            BackColor = Color.FromArgb(200, 16, 46),
            FlatAppearance = { BorderSize = 0 },
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(445, 22),
            Name = "btnWatchVideo",
            Size = new Size(125, 38),
            Text = "WATCH VIDEO",
            UseVisualStyleBackColor = false
        };
        btnWatchVideo.Click += BtnWatchVideo_Click;
        Controls.Add(btnWatchVideo);

        // lblStatus
        lblStatus.AutoSize = true;
        lblStatus.BackColor = Color.Transparent;
        lblStatus.Font = new Font("Segoe UI", 9F);
        lblStatus.ForeColor = Color.Silver;
        lblStatus.Location = new Point(460, 33);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(0, 15);
        lblStatus.Text = "";

        // dgDrills
        // ReadOnly + full-row select + no add-row: this is a display grid, not an editor.
        // BackgroundColor near-black kills the gray dead space below the last row.
        dgDrills.AllowUserToAddRows = false;
        dgDrills.AllowUserToDeleteRows = false;
        dgDrills.BackgroundColor = Color.FromArgb(20, 20, 20);
        dgDrills.BorderStyle = BorderStyle.None;
        dgDrills.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgDrills.EnableHeadersVisualStyles = false;
        dgDrills.GridColor = Color.FromArgb(60, 60, 60);
        dgDrills.Location = new Point(24, 76);
        dgDrills.Name = "dgDrills";
        dgDrills.ReadOnly = true;
        dgDrills.RowHeadersVisible = false;
        dgDrills.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgDrills.Size = new Size(940, 460);
        dgDrills.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        dgDrills.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(200, 16, 46);
        dgDrills.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgDrills.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        dgDrills.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
        dgDrills.DefaultCellStyle.ForeColor = Color.White;
        dgDrills.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 16, 46);
        dgDrills.DefaultCellStyle.SelectionForeColor = Color.White;

        // ApiDrillsForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(18, 18, 18);
        ClientSize = new Size(988, 561);
        Controls.Add(lblStatus);
        Controls.Add(dgDrills);
        Controls.Add(btnLoad);
        Controls.Add(cboSport);
        Controls.Add(lblSport);
        Name = "ApiDrillsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Drills — Live from Web API";
        Load += ApiDrillsForm_Load;

        ((System.ComponentModel.ISupportInitialize)dgDrills).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
