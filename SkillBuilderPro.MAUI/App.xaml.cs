using Microsoft.Extensions.DependencyInjection;

namespace SkillBuilderPro.MAUI;

public partial class App : Application
{
	readonly Services.IAthleteApiService api;
	public App(Services.IAthleteApiService api)
	{
		InitializeComponent();
		this.api=api;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window=new Window(new NavigationPage(new Views.ChooseProfilePage(api)));
		_ = RestoreAsync(window);
		return window;
	}
	async Task RestoreAsync(Window window){if(await api.RestoreAsync())MainThread.BeginInvokeOnMainThread(()=>window.Page=ShellFactory.Create(api));}
}
