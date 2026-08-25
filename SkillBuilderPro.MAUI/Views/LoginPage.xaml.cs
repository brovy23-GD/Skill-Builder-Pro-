using SkillBuilderPro.MAUI.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;
namespace SkillBuilderPro.MAUI.Views;

public partial class LoginPage : ContentPage
{
	readonly IAthleteApiService api;
	readonly string role;
	public LoginPage(IAthleteApiService api):this(api,api.SelectedRole??"Athlete"){}
	public LoginPage(IAthleteApiService api,string role)
	{
		InitializeComponent();
		this.api=api;this.role=role;RoleLabel.Text=$"{role.ToUpperInvariant()} ACCESS";SignInButton.Text=role switch{"Athlete"=>"ENTER LOCKER ROOM","Parent"=>"ENTER PARENT HUB","Coach"=>"ENTER COACH'S OFFICE","Administrator"=>"ENTER ADMIN CENTER",_=>"SIGN IN"};CreateButton.IsVisible=role is "Athlete" or "Parent";DemoButton.IsVisible=role=="Athlete";
	}
	async void SignInClicked(object? sender,EventArgs e){SignInButton.IsEnabled=false;ErrorLabel.IsVisible=false;var result=await api.LoginAsync(EmailEntry.Text??"",PasswordEntry.Text??"",role);if(result.Ok)Application.Current!.Windows[0].Page=ShellFactory.Create(api);else{ErrorLabel.Text=result.Error;ErrorLabel.IsVisible=true;}SignInButton.IsEnabled=true;}
	async void CreateClicked(object? sender,EventArgs e)=>await Navigation.PushAsync(new RegisterPage(api,role));
	void DemoClicked(object? sender,EventArgs e){api.EnterDemoMode();Application.Current!.Windows[0].Page=new AppShell();}
	void PasswordVisibilityClicked(object? sender,EventArgs e){PasswordEntry.IsPassword=!PasswordEntry.IsPassword;PasswordVisibilityButton.Source=PasswordEntry.IsPassword?"eye.svg":"eye_off.svg";SemanticProperties.SetDescription(PasswordVisibilityButton,PasswordEntry.IsPassword?"Show password":"Hide password");PasswordEntry.Focus();}
	async void PasteEmailClicked(object? sender,EventArgs e)=>await PasteAsync(EmailEntry,false);
	async void PastePasswordClicked(object? sender,EventArgs e)=>await PasteAsync(PasswordEntry,true);
	async Task PasteAsync(Entry target,bool password){try{var text=await Clipboard.Default.GetTextAsync();if(string.IsNullOrEmpty(text)){ShowInlineMessage("Clipboard is empty.");return;}target.Text=text;if(password){PasswordEntry.IsPassword=true;PasswordVisibilityButton.Source="eye.svg";SemanticProperties.SetDescription(PasswordVisibilityButton,"Show password");}ErrorLabel.IsVisible=false;target.Focus();}catch(Exception){ShowInlineMessage("Clipboard is unavailable. Copy the text and try PASTE again.");}}
	void ShowInlineMessage(string message){ErrorLabel.Text=message;ErrorLabel.IsVisible=true;}
	protected override void OnDisappearing(){base.OnDisappearing();PasswordEntry.IsPassword=true;PasswordVisibilityButton.Source="eye.svg";}
}
