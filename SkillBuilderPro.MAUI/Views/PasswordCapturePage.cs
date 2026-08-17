namespace SkillBuilderPro.MAUI.Views;

internal sealed class PasswordCapturePage : ContentPage
{
    private readonly Entry password = new() { Placeholder = "Password (8+ characters)", IsPassword = true };
    private readonly Entry confirm = new() { Placeholder = "Confirm password", IsPassword = true };
    private readonly Label error = new() { TextColor = Color.FromArgb("#FFB3A7"), IsVisible = false };
    private readonly TaskCompletionSource<string?> completion = new();

    public PasswordCapturePage()
    {
        Title = "Secure Your Account";
        password.Style = (Style)Application.Current!.Resources["GlassInputStyle"];
        confirm.Style = (Style)Application.Current.Resources["GlassInputStyle"];
        Content = new Grid
        {
            BackgroundColor = Color.FromArgb("#080C12"), Padding = 20,
            Children =
            {
                new Border
                {
                    MaximumWidthRequest = 500, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center,
                    Style = (Style)Application.Current.Resources["GlassPanelStyle"],
                    Content = BuildContent()
                }
            }
        };
    }

    public Task<string?> Result => completion.Task;

    private View BuildContent()
    {
        var save = new Button { Text = "CREATE SECURE PROFILE", Style = (Style)Application.Current!.Resources["GlassActionButtonStyle"] };
        save.Clicked += ConfirmClicked;
        var cancel = new Button { Text = "CANCEL", Style = (Style)Application.Current.Resources["GlassSecondaryButtonStyle"] };
        cancel.Clicked += CancelClicked;
        return new VerticalStackLayout
        {
            Spacing = 14,
            Children =
            {
                new Label { Text = "SECURE YOUR ACCOUNT", Style = (Style)Application.Current.Resources["SectionTitleStyle"] },
                PasswordRow(password), PasswordRow(confirm), error, save, cancel
            }
        };
    }

    private Grid PasswordRow(Entry entry)
    {
        var eye = new ImageButton { Source = "eye.svg", BackgroundColor = Color.FromArgb("#6815202D"), Padding = 13 };
        SemanticProperties.SetDescription(eye, "Show password");
        eye.Clicked += (_, _) =>
        {
            entry.IsPassword = !entry.IsPassword;
            eye.Source = entry.IsPassword ? "eye.svg" : "eye_off.svg";
            SemanticProperties.SetDescription(eye, entry.IsPassword ? "Show password" : "Hide password");
            entry.Focus();
        };
        var grid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(new GridLength(52)) } };
        grid.Add(entry, 0, 0); grid.Add(eye, 1, 0);
        return grid;
    }

    private async void ConfirmClicked(object? sender, EventArgs e)
    {
        if ((password.Text?.Length ?? 0) < 8) { ShowError("Password must contain at least 8 characters."); return; }
        if (!string.Equals(password.Text, confirm.Text, StringComparison.Ordinal)) { ShowError("Passwords do not match."); return; }
        completion.TrySetResult(password.Text);
        await Navigation.PopModalAsync();
    }

    private async void CancelClicked(object? sender, EventArgs e)
    {
        completion.TrySetResult(null);
        await Navigation.PopModalAsync();
    }

    protected override bool OnBackButtonPressed() { completion.TrySetResult(null); return base.OnBackButtonPressed(); }
    protected override void OnDisappearing() { password.IsPassword = true; confirm.IsPassword = true; base.OnDisappearing(); }
    private void ShowError(string message) { error.Text = message; error.IsVisible = true; }
}
