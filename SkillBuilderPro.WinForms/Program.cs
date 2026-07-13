using SkillBuilderPro.WinForms.Models;
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

            while (true)
            {
                LoginForm login = new LoginForm();
                var result = login.ShowDialog();

                if (result != DialogResult.OK || login.LoggedInUser == null)
                {
                    Application.Exit();
                    return;
                }

                User current = login.LoggedInUser;// Get the logged-in user
                bool demo = login.IsDemoMode;// Get the demo mode status
                bool returnToLogin = false;//       Flag to determine if we should return to the login form

                while (current != null)// Loop to keep the user in the dashboard until they log out or switch users
                {
                    MainForm dashboard = new MainForm(current, demo);// Create the dashboard form with the current user and demo mode status
                    Application.Run(dashboard);// Run the dashboard form

                    if (dashboard.NextUser != null)//   If the user has chosen to switch users, set the current user to the next user and continue the loop
                    {
                        current = dashboard.NextUser;// Set the current user to the next user
                        demo = true;// Set demo mode to true for the next user
                    }
                    else
                    {
                        returnToLogin = true;
                        break;
                    }
                }

                if (!returnToLogin)
                    break;
            }
        }
    }
}