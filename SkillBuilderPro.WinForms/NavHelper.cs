using System;
using System.Drawing;
using System.Windows.Forms;

namespace SkillBuilderPro.WinForms
{
    public static class NavHelper
    {
        /// <summary>
        /// Adds a BACK button to the top-left of a container and wires Esc to it.
        /// onBack runs when clicked; default is close the form.
        /// </summary>
        public static Button AddBackButton(Form form, Control host, Action onBack = null)
        {
            Button back = new Button
            {
                Text = "← BACK",
                Size = new Size(110, 36),
                Location = new Point(20, 18),
                BackColor = Brand.Graphite,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            back.FlatAppearance.BorderSize = 0;
            back.FlatAppearance.MouseOverBackColor = Brand.Raised;

            Action go = onBack ?? (() => form.Close());
            back.Click += (s, e) => go();

            form.KeyPreview = true;
            form.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) { e.Handled = true; go(); }
            };

            host.Controls.Add(back);
            back.BringToFront();
            return back;
        }

        /// <summary>Adds a mute toggle to the top-right area of a header.</summary>
        public static Button AddMuteButton(Control host, int xFromRight = 180)
        {
            Button mute = new Button
            {
                Text = MusicPlayer.IsMuted ? "♪ OFF" : "♪ ON",
                Size = new Size(70, 36),
                BackColor = Brand.Graphite,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false,
                Visible = MusicPlayer.IsLoaded
            };
            mute.FlatAppearance.BorderSize = 0;
            mute.FlatAppearance.MouseOverBackColor = Brand.Raised;
            mute.Click += (s, e) =>
            {
                MusicPlayer.ToggleMute();
                mute.Text = MusicPlayer.IsMuted ? "♪ OFF" : "♪ ON";
            };

            void Position() => mute.Location =
                new Point(host.ClientSize.Width - xFromRight - mute.Width, 27);
            host.Resize += (s, e) => Position();
            Position();

            host.Controls.Add(mute);
            mute.BringToFront();
            return mute;
        }
    }
}