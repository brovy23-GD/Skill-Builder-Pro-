using Microsoft.VisualBasic.ApplicationServices;
using SkillBuilderPro.WinForms.AdminScreens;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MusicPlayer.Start(); 
            
            while (true)
            {
                // 1. Role screen
                var roleSelect = new RoleSelectForm();
                if (roleSelect.ShowDialog() != DialogResult.OK)
                    break;

                // 2. Login
                var login = new LoginForm(roleSelect.SelectedRole);
                if (login.ShowDialog() != DialogResult.OK)
                    continue;   // back to role screen

                SkillBuilderPro.WinForms.Models.User current = login.LoggedInUser;

                bool demo = login.IsDemoMode;

                // 3. Dashboard loop — stays here while athletes switch users
                while (current != null)
                {
                    Form dashboard;

                    switch (current.Role)
                    {
                        case "Coach":
                            dashboard = new CoachDashboard(current);
                            break;

                        case "Parent":
                            dashboard = new ParentDashboard(current);
                            break;

                        case "Admin":
                            dashboard = new AdminDashboardForm(current);


                            break;

                        case "Athlete":
                        default:
                            dashboard = new MainForm(current, demo);
                            break;
                    }

                    Application.Run(dashboard);

                    // Athlete switching users → rebuild dashboard, no re-login
                    current = (dashboard is MainForm mf) ? mf.NextUser : null;
                }

               
                
                // Dashboard closed without a switch → back to role screen
            }

            MusicPlayer.Stop();

        }





    }
}