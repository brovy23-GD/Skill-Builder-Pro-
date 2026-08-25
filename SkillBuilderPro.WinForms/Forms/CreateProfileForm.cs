using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Services;
using SkillBuilderPro.WinForms.Utils;

namespace SkillBuilderPro.WinForms;

public partial class CreateProfileForm : Form
{
    const float ArtWidth = 1672f, ArtHeight = 941f;
    readonly Panel formHost = new() { BackColor = Color.Transparent };
    readonly TextBox name = Field(), team = Field(), feet = Field(), inches = Field(), weight = Field(), jersey = Field(), age = Field();
    readonly TextBox bio = Field(multiline: true);
    readonly ComboBox sport = Picker(), position = Picker(), dominant = Picker();
    readonly PictureBox photo = new() { BackColor = Color.FromArgb(220, 16, 23, 32), SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
    readonly Button upload = Action("UPLOAD PHOTO"), clear = Action("CLEAR ALL"), continueButton = Action("CONTINUE  ›", true), signIn = Action("ALREADY HAVE AN ACCOUNT?  SIGN IN");
    readonly Label validation = new() { ForeColor = Color.FromArgb(255, 179, 167), BackColor = Color.FromArgb(190, 8, 13, 19), AutoSize = false, TextAlign = ContentAlignment.MiddleLeft };
    readonly Label photoLabel = Caption("PROFILE PHOTO"), nameLabel = Caption("ATHLETE NAME"), teamLabel = Caption("TEAM"), heightLabel = Caption("HEIGHT"), bioLabel = Caption("ABOUT YOU"), sportLabel = Caption("PRIMARY SPORT"), positionLabel = Caption("POSITION"), weightLabel = Caption("WEIGHT"), jerseyLabel = Caption("JERSEY NUMBER"), ageLabel = Caption("AGE"), dominantLabel = Caption("DOMINANT HAND / SIDE"), photoHelp = Caption("JPG, PNG  •  MAX 5MB");
    string photoPath = string.Empty;

    static readonly Dictionary<string, string[]> Positions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Baseball"] = ["Pitcher", "Catcher", "Infield", "Outfield", "Utility"], ["Softball"] = ["Pitcher", "Catcher", "Infield", "Outfield", "Utility"],
        ["Basketball"] = ["Guard", "Wing", "Forward", "Center"], ["Football"] = ["Quarterback", "Running Back", "Receiver", "Line", "Linebacker", "Defensive Back"],
        ["Soccer"] = ["Goalkeeper", "Defender", "Midfielder", "Forward"], ["Hockey"] = ["Goalie", "Defense", "Center", "Wing"]
    };

    public User CreatedUser { get; private set; } = null!;

    public CreateProfileForm()
    {
        InitializeComponent();
        Text = "Create Athlete Profile - Skill Builder Pro"; WindowState = FormWindowState.Maximized; MinimumSize = new Size(1000, 650);
        var createProfilePath = Path.Combine(AppContext.BaseDirectory, "Resources", "create_profile_desktop.png");
        using var approvedBackground = Image.FromFile(createProfilePath);
        BackgroundImage = new Bitmap(approvedBackground);
        BackgroundImageLayout = ImageLayout.Zoom; BackColor = Color.Black; DoubleBuffered = true;
        Controls.Add(formHost);
        foreach (var control in new Control[] { photoLabel, photo, upload, photoHelp, nameLabel, name, teamLabel, team, heightLabel, feet, inches, bioLabel, bio, sportLabel, sport, positionLabel, position, weightLabel, weight, jerseyLabel, jersey, ageLabel, age, dominantLabel, dominant, clear, continueButton, signIn, validation }) formHost.Controls.Add(control);
        sport.Items.AddRange(Positions.Keys.OrderBy(x => x).Cast<object>().ToArray()); dominant.Items.AddRange(["Left", "Right", "Both"]);
        sport.SelectedIndexChanged += (_, _) => { position.Items.Clear(); if (sport.SelectedItem is string s) position.Items.AddRange(Positions[s]); position.SelectedIndex = -1; };
        upload.Click += UploadClicked; clear.Click += (_, _) => ClearAll(); continueButton.Click += ContinueClicked; signIn.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Resize += (_, _) => LayoutAgainstBackground(); Shown += (_, _) => LayoutAgainstBackground();
        AcceptButton = continueButton; TabOrder();
    }

    Rectangle GetBackgroundRenderBounds()
        => BackgroundRenderHelper.AspectFit(
            new Size((int)ArtWidth, (int)ArtHeight),
            ClientRectangle);

    void LayoutAgainstBackground()
    {
        var art = GetBackgroundRenderBounds();
        formHost.Bounds = new Rectangle(art.Left + Px(16, art.Width), art.Top + Py(400, art.Height), Px(1640, art.Width), Py(530, art.Height));
        Set(photoLabel, 62, 34, 250, 24); Set(photo, 62, 65, 250, 220); Set(upload, 62, 305, 250, 48); Set(photoHelp, 62, 361, 250, 22);
        Set(nameLabel, 373, 29, 397, 22); Set(teamLabel, 373, 134, 397, 22); Set(heightLabel, 503, 202, 266, 22); Set(bioLabel, 373, 301, 397, 22);
        Set(name, 373, 57, 397, 46); Set(team, 373, 162, 397, 45); Set(feet, 503, 226, 148, 46); Set(inches, 660, 226, 109, 46); Set(bio, 373, 328, 397, 74);
        Set(sportLabel, 861, 29, 361, 22); Set(positionLabel, 861, 134, 361, 22); Set(weightLabel, 861, 249, 361, 22);
        Set(sport, 861, 57, 361, 46); Set(position, 861, 161, 361, 46); Set(weight, 861, 276, 361, 46);
        Set(jerseyLabel, 1299, 28, 311, 22); Set(ageLabel, 1299, 131, 311, 22); Set(dominantLabel, 1299, 242, 311, 22);
        Set(jersey, 1299, 56, 311, 46); Set(age, 1299, 158, 311, 46); Set(dominant, 1299, 269, 311, 46);
        Set(clear, 156, 471, 283, 52); Set(continueButton, 564, 471, 512, 54); Set(signIn, 1137, 471, 345, 52); Set(validation, 861, 328, 749, 62);
        var scale = Math.Max(.62f, art.Width / ArtWidth); Font = new Font("Segoe UI", Math.Max(8, 11 * scale));
    }

    void Set(Control control, int x, int y, int w, int h) => control.Bounds = new Rectangle(Px(x, formHost.Width, 1640), Py(y, formHost.Height, 530), Px(w, formHost.Width, 1640), Py(h, formHost.Height, 530));
    static int Px(float value, int size, float basis = ArtWidth) => (int)Math.Round(size * value / basis);
    static int Py(float value, int size, float basis = ArtHeight) => (int)Math.Round(size * value / basis);

    async void UploadClicked(object? sender, EventArgs e)
    {
        using var picker = new OpenFileDialog { Filter = "Image files|*.jpg;*.jpeg;*.png", Title = "Choose athlete photo" };
        if (picker.ShowDialog(this) != DialogResult.OK) return;
        photoPath = picker.FileName;
        await using var stream = File.OpenRead(photoPath); photo.Image = Image.FromStream(stream);
    }

    void ClearAll()
    {
        foreach (var box in new[] { name, team, feet, inches, weight, jersey, age, bio }) box.Clear();
        sport.SelectedIndex = position.SelectedIndex = dominant.SelectedIndex = -1; photo.Image = null; photoPath = string.Empty; validation.Text = string.Empty;
    }

    void ContinueClicked(object? sender, EventArgs e)
    {
        var issues = new List<string>();
        if (string.IsNullOrWhiteSpace(name.Text)) issues.Add("Athlete name is required.");
        if (sport.SelectedItem is null) issues.Add("Primary sport is required.");
        if (position.SelectedItem is null) issues.Add("Position is required.");
        if (!OptionalInt(jersey.Text, 0, 999)) issues.Add("Jersey number must be 0–999.");
        if (!OptionalInt(age.Text, 1, 120)) issues.Add("Age must be 1–120.");
        if (!OptionalInt(feet.Text, 1, 8) || !OptionalInt(inches.Text, 0, 11)) issues.Add("Height must use valid feet and inches.");
        if (!OptionalDouble(weight.Text, 1, 999)) issues.Add("Weight must be valid.");
        if (issues.Count > 0) { validation.Text = string.Join("  ", issues); return; }
        if (!TryGetCredentials(out var email, out var password)) return;

        var selectedSport = sport.SelectedItem!.ToString()!; var selectedPosition = position.SelectedItem!.ToString()!;
        var result = new AuthenticationService().SignUp(email, password, name.Text.Trim(), "Athlete", selectedSport, selectedPosition, "Beginner", "", ParseInt(jersey.Text), "Build consistent training habits");
        if (!result.success) { validation.Text = result.message; return; }
        CreatedUser = new User { FullName = name.Text.Trim(), Email = email, Password = password, Sport = selectedSport, TargetArea = selectedPosition, ExperienceLevel = "Beginner", Role = "Athlete", IsActive = true, PhotoPath = photoPath, Team = team.Text.Trim(), Bio = bio.Text.Trim(), JerseyNumber = ParseInt(jersey.Text), Age = ParseInt(age.Text), Height = ParseInt(feet.Text) + ParseInt(inches.Text) / 12d, Weight = double.TryParse(weight.Text, out var pounds) ? pounds : 0, Goal = "Build consistent training habits" };
        DialogResult = DialogResult.OK; Close();
    }

    bool TryGetCredentials(out string email, out string password)
    {
        using var dialog = new Form { Text = "Secure your account", StartPosition = FormStartPosition.CenterParent, ClientSize = new Size(420, 190), BackColor = Color.FromArgb(14, 20, 29), FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
        var emailBox = Field(); var passwordBox = Field(); passwordBox.UseSystemPasswordChar = true;
        emailBox.SetBounds(30, 30, 360, 32); passwordBox.SetBounds(30, 82, 318, 32); var ok = Action("CREATE PROFILE", true); ok.SetBounds(220, 132, 170, 40); ok.DialogResult = DialogResult.OK;
        var passwordEye = Action("👁"); passwordEye.AccessibleName = "Show password"; passwordEye.SetBounds(354, 82, 36, 32);
        passwordEye.Click += (_, _) => { var selectionStart = passwordBox.SelectionStart; passwordBox.UseSystemPasswordChar = !passwordBox.UseSystemPasswordChar; passwordEye.Text = passwordBox.UseSystemPasswordChar ? "👁" : "⊘"; passwordEye.AccessibleName = passwordBox.UseSystemPasswordChar ? "Show password" : "Hide password"; passwordBox.Focus(); passwordBox.SelectionStart = Math.Min(selectionStart, passwordBox.TextLength); };
        dialog.Controls.AddRange([emailBox, passwordBox, passwordEye, ok]); dialog.AcceptButton = ok;
        if (dialog.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(emailBox.Text) || passwordBox.Text.Length < 8) { validation.Text = "Email and a password of at least 8 characters are required."; email = password = ""; return false; }
        email = emailBox.Text.Trim(); password = passwordBox.Text; return true;
    }

    void TabOrder() { Control[] order = [photo, upload, name, sport, jersey, team, position, age, feet, inches, weight, dominant, bio, clear, continueButton, signIn]; for (var i = 0; i < order.Length; i++) order[i].TabIndex = i; }
    static bool OptionalInt(string value, int min, int max) => string.IsNullOrWhiteSpace(value) || int.TryParse(value, out var n) && n >= min && n <= max;
    static bool OptionalDouble(string value, double min, double max) => string.IsNullOrWhiteSpace(value) || double.TryParse(value, out var n) && n >= min && n <= max;
    static int ParseInt(string value) => int.TryParse(value, out var n) ? n : 0;
    static TextBox Field(bool multiline = false) => new() { BackColor = Color.FromArgb(16, 23, 32), ForeColor = Color.FromArgb(243, 246, 249), BorderStyle = BorderStyle.FixedSingle, Multiline = multiline };
    static ComboBox Picker() => new() { BackColor = Color.FromArgb(16, 23, 32), ForeColor = Color.FromArgb(243, 246, 249), FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList };
    static Label Caption(string text) => new() { Text = text, ForeColor = Color.FromArgb(190, 204, 218), BackColor = Color.Transparent, Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold), AutoSize = false, TextAlign = ContentAlignment.MiddleLeft };
    static Button Action(string text, bool primary = false) { var b = new Button { Text = text, ForeColor = Color.FromArgb(243, 246, 249), BackColor = primary ? Color.FromArgb(19, 75, 133) : Color.FromArgb(16, 23, 32), FlatStyle = FlatStyle.Flat }; b.FlatAppearance.BorderColor = Color.FromArgb(83, 127, 170); return b; }
}
