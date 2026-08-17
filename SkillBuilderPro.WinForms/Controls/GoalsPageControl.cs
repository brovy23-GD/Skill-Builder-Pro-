using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Controls;

public sealed class GoalsPageControl : UserControl
{
    private static readonly Color Graphite = Color.FromArgb(96, 17, 23, 32);
    private static readonly Color Surface = Color.FromArgb(104, 24, 32, 43);
    private static readonly Color Silver = Color.FromArgb(210, 220, 230);
    private static readonly Color Muted = Color.FromArgb(153, 170, 190);
    private static readonly Color PerformanceBlue = Color.FromArgb(77, 155, 232);

    public GoalsPageControl(User user, bool demoMode)
    {
        Dock = DockStyle.Fill;
        BackColor = Color.Transparent;
        AutoScroll = true;

        var content = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.Transparent,
            Padding = new Padding(28, 24, 28, 36),
            Width = 980
        };
        Controls.Add(content);
        Resize += (_, _) => content.Left = Math.Max((ClientSize.Width - content.Width) / 2, 0);

        content.Controls.Add(Label("ATHLETE DEVELOPMENT", 9, PerformanceBlue, FontStyle.Bold, 900, 20));
        content.Controls.Add(Label("GOALS & PROGRESS", 24, Color.White, FontStyle.Bold, 900, 38));
        content.Controls.Add(Label("SET IT. CHASE IT. ACHIEVE IT.", 10, Muted, FontStyle.Regular, 900, 25));

        var metrics = new FlowLayoutPanel { Width = 900, Height = 88, BackColor = Color.Transparent, Margin = new Padding(0, 14, 0, 14) };
        metrics.Controls.Add(Metric("CURRENT RANK", demoMode ? "Competitor" : user.ExperienceLevel));
        metrics.Controls.Add(Metric("STREAK", demoMode ? "6 DAYS" : "—"));
        metrics.Controls.Add(Metric("ACTIVE GOALS", demoMode ? "3" : string.IsNullOrWhiteSpace(user.Goal) ? "0" : "1"));
        metrics.Controls.Add(Metric("PROGRESS", demoMode ? "68%" : "—"));
        content.Controls.Add(metrics);
        content.Controls.Add(Label("ACTIVE GOALS", 13, Silver, FontStyle.Bold, 900, 28));

        if (demoMode)
        {
            content.Controls.Add(GoalCard("Improve Batting Contact", "Current 72%  •  Target 85%", 60));
            content.Controls.Add(GoalCard("Complete 3 Hitting Sessions", "2 of 3 sessions complete", 67));
            content.Controls.Add(GoalCard("Fielding Repetition Goal", "160 of 200 quality reps", 80));
        }
        else if (!string.IsNullOrWhiteSpace(user.Goal))
        {
            content.Controls.Add(GoalCard(user.Goal, "Athlete goal", 0));
        }
        else
        {
            var empty = Panel(900, 86);
            empty.Controls.Add(Label("NO ACTIVE GOALS", 12, Silver, FontStyle.Bold, 840, 24, 20, 14));
            empty.Controls.Add(Label("Build your next target and start tracking progress.", 10, Muted, FontStyle.Regular, 840, 22, 20, 42));
            content.Controls.Add(empty);
        }
    }

    private static Panel Metric(string caption, string value)
    {
        var panel = Panel(210, 72); panel.Margin = new Padding(0, 0, 12, 0);
        panel.Controls.Add(Label(caption, 8, Muted, FontStyle.Bold, 180, 18, 14, 10));
        panel.Controls.Add(Label(value, 15, Color.White, FontStyle.Bold, 180, 28, 14, 30));
        return panel;
    }

    private static Panel GoalCard(string title, string detail, int progress)
    {
        var panel = Panel(900, 92); panel.Margin = new Padding(0, 0, 0, 10);
        panel.Controls.Add(Label(title, 12, Color.White, FontStyle.Bold, 700, 24, 16, 12));
        panel.Controls.Add(Label(detail, 9, Muted, FontStyle.Regular, 700, 20, 16, 39));
        panel.Controls.Add(Label($"{progress}%", 10, PerformanceBlue, FontStyle.Bold, 100, 22, 780, 14));
        var track = new Panel { BackColor = Color.FromArgb(50, 62, 76), Bounds = new Rectangle(16, 68, 852, 5) };
        track.Controls.Add(new Panel { BackColor = PerformanceBlue, Bounds = new Rectangle(0, 0, 852 * Math.Clamp(progress, 0, 100) / 100, 5) });
        panel.Controls.Add(track);
        return panel;
    }

    private static Panel Panel(int width, int height) => new() { Size = new Size(width, height), BackColor = Surface };
    private static Label Label(string text, float size, Color color, FontStyle style, int width, int height, int x = 0, int y = 0) => new()
    {
        Text = text, ForeColor = color, BackColor = Color.Transparent, Font = new Font("Segoe UI", size, style),
        AutoSize = false, Size = new Size(width, height), Location = new Point(x, y), TextAlign = ContentAlignment.MiddleLeft
    };
}
