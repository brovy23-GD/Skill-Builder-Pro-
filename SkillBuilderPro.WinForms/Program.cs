using System;
using System.Windows.Forms;

namespace SkillBuilderPro.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            LoginForm login = new LoginForm();
            var result = login.ShowDialog();

            if (login.IsDemoMode && login.LoggedInUser != null)
            {
                Application.Run(new MainForm(login.LoggedInUser));
                return;
            }

            if (result == DialogResult.OK && login.LoggedInUser != null)
            {
                Application.Run(new MainForm(login.LoggedInUser));
                return;
            }

            Application.Exit();
        }
    }
}

