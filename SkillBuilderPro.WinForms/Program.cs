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

                SkillBuilderPro.WinForms.Models.User current;
                bool demo;
                if (roleSelect.IsDemoMode)
                {
                    // Preserve the existing product-wide Athlete demonstration
                    // identity without invoking authenticated login.
                    current = DummyUsers.GetAllDummyUsers()
                        .First(user => user.FullName == "Aubrey Rovy");
                    current.IsActive = true;
                    current.Role = "Athlete";
                    demo = true;
                }
                else
                {
                    // 2. Authenticated role login
                    var login = new LoginForm(roleSelect.SelectedRole);
                    if (login.ShowDialog() != DialogResult.OK)
                        continue;   // back to role screen

                    current = login.LoggedInUser;
                    demo = login.IsDemoMode;
                }

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
