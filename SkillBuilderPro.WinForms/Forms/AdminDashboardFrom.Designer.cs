namespace SkillBuilderPro.WinForms.AdminScreens
{
    partial class AdminDashboardForm
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
        /// Designer method — intentionally minimal.
        /// All UI is now built manually in AdminDashboardForm.cs.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ================================
            // FORM SETTINGS
            // ================================
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Text = "Admin Dashboard";

            // IMPORTANT:
            // No sidebar
            // No search bar
            // No navigation buttons
            // Background tile buttons are created manually in AdminDashboardForm.cs
            // Page builders and admin controls are also created manually.
        }

        #endregion
    }
}
