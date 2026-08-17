using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Utils;
namespace SkillBuilderPro.WinForms.Controls;
public sealed class HomePageControl : UserControl
{
 private readonly Image? approvedBackground;
 public HomePageControl(User user,bool isDemoMode)
 {
  Dock=DockStyle.Fill;DoubleBuffered=true;BackColor=Color.FromArgb(10,14,20);
  var path=Path.Combine(AppContext.BaseDirectory,"Resources","home_background_approved.png");if(File.Exists(path)){using var source=Image.FromFile(path);approvedBackground=new Bitmap(source);}
  var panel=new Panel{BackColor=Color.FromArgb(82,10,18,28),Size=new Size(470,250),Anchor=AnchorStyles.Top|AnchorStyles.Left};
  panel.Controls.Add(new Label{Text="SKILL BUILDER PRO",ForeColor=Color.White,Font=new Font("Segoe UI",24,FontStyle.Bold),AutoSize=true,Location=new Point(24,22)});
  panel.Controls.Add(new Label{Text="WHERE BETTER IS BUILT.",ForeColor=Color.FromArgb(22,140,255),Font=new Font("Segoe UI",15,FontStyle.Bold),AutoSize=true,Location=new Point(26,65)});
  panel.Controls.Add(new Label{Text=$"Welcome, {user.FullName}",ForeColor=Color.White,Font=new Font("Segoe UI",17,FontStyle.Bold),AutoSize=true,Location=new Point(26,112)});
  panel.Controls.Add(new Label{Text=$"{user.Sport}  •  {user.TargetArea}\n{(isDemoMode?"Demo workspace":"Your live training workspace")}",ForeColor=Color.FromArgb(210,225,238),Font=new Font("Segoe UI",11),AutoSize=true,Location=new Point(28,154)});
  Controls.Add(panel);Resize+=(s,e)=>panel.Location=new Point(Math.Max(32,(ClientSize.Width-panel.Width)/12),Math.Max(32,(ClientSize.Height-panel.Height)/3));
 }
 protected override void OnPaintBackground(PaintEventArgs e){base.OnPaintBackground(e);if(approvedBackground is not null)BackgroundRenderHelper.DrawAspectFill(e.Graphics,approvedBackground,ClientRectangle);}
 protected override void Dispose(bool disposing){if(disposing)approvedBackground?.Dispose();base.Dispose(disposing);}
}
