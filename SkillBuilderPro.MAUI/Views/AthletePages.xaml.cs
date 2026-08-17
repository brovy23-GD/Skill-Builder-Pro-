using SkillBuilderPro.MAUI.ViewModels;
using SkillBuilderPro.MAUI.Models;

namespace SkillBuilderPro.MAUI.Views;
public partial class AthleteDashboardPage:ContentPage{public AthleteDashboardPage(DashboardViewModel vm){InitializeComponent();BindingContext=vm;}protected override void OnAppearing(){base.OnAppearing();((DashboardViewModel)BindingContext).LoadCommand.Execute(null);}async void BellClicked(object? s,EventArgs e)=>await Shell.Current.GoToAsync(nameof(NotificationsPage));async void QuickActionClicked(object? s,EventArgs e){if(s is Button{CommandParameter:string route})await Shell.Current.GoToAsync($"//{route}");}}
public partial class GoalsPage:ContentPage{public GoalsPage(GoalsViewModel vm){InitializeComponent();BindingContext=vm;}protected override void OnAppearing(){base.OnAppearing();((GoalsViewModel)BindingContext).LoadCommand.Execute(null);}async void GoalsNavigationClicked(object? s,EventArgs e){if(s is Button{CommandParameter:string route})await Shell.Current.GoToAsync($"//{route}");}async void BackClicked(object? s,EventArgs e)=>await GoBackAsync();async void ExitClicked(object? s,EventArgs e)=>await Shell.Current.GoToAsync("//Home");async Task GoBackAsync(){if(Shell.Current.Navigation.NavigationStack.Count>1)await Shell.Current.GoToAsync("..");else await Shell.Current.GoToAsync("//Home");}}
public partial class TrophyRoomPage:ContentPage
{
 public TrophyRoomPage(TrophyViewModel vm){InitializeComponent();BindingContext=vm;}
 void TrophyStageSizeChanged(object? s,EventArgs e){if(TrophyStage.Width<=0)return;var phone=TrophyStage.Width<700;var rank=TrophySummaryGrid.Children[0];var spacer=TrophySummaryGrid.Children[1];var achievements=TrophySummaryGrid.Children[2];TrophySummaryGrid.ColumnDefinitions.Clear();TrophySummaryGrid.RowDefinitions.Clear();if(phone){TrophySummaryGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));TrophySummaryGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));TrophySummaryGrid.RowDefinitions.Add(new RowDefinition(new GridLength(180)));TrophySummaryGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));TrophySummaryGrid.SetColumn(rank,0);TrophySummaryGrid.SetRow(rank,0);TrophySummaryGrid.SetColumn(spacer,0);TrophySummaryGrid.SetRow(spacer,1);TrophySummaryGrid.SetColumn(achievements,0);TrophySummaryGrid.SetRow(achievements,2);}else{TrophySummaryGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(.9,GridUnitType.Star)));TrophySummaryGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1.2,GridUnitType.Star)));TrophySummaryGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(.9,GridUnitType.Star)));TrophySummaryGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));TrophySummaryGrid.SetColumn(rank,0);TrophySummaryGrid.SetRow(rank,0);TrophySummaryGrid.SetColumn(spacer,1);TrophySummaryGrid.SetRow(spacer,0);TrophySummaryGrid.SetColumn(achievements,2);TrophySummaryGrid.SetRow(achievements,0);}}
 protected override void OnAppearing(){base.OnAppearing();((TrophyViewModel)BindingContext).LoadCommand.Execute(null);}
 async void BackClicked(object? s,EventArgs e){if(Shell.Current.Navigation.NavigationStack.Count>1)await Shell.Current.GoToAsync("..");else await Shell.Current.GoToAsync("//Goals");}
 async void ExitClicked(object? s,EventArgs e)=>await Shell.Current.GoToAsync("//Home");
}
public partial class TrainingPage:ContentPage,IQueryAttributable
{
 public TrainingPage(TrainingViewModel vm){InitializeComponent();BindingContext=vm;}
 void TrainingStageSizeChanged(object? s,EventArgs e){var phone=TrainingStage.Width>0&&TrainingStage.Width<700;TrainingContent.Padding=phone?new Thickness(16,18,16,28):new Thickness(22);TrainingBuilderGrid.ColumnDefinitions.Clear();TrainingBuilderGrid.RowDefinitions.Clear();if(phone){TrainingBuilderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));TrainingBuilderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));TrainingBuilderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));TrainingBuilderGrid.SetColumn(DrillCard,0);TrainingBuilderGrid.SetRow(DrillCard,1);}else{TrainingBuilderGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(.85,GridUnitType.Star)));TrainingBuilderGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1.15,GridUnitType.Star)));TrainingBuilderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));TrainingBuilderGrid.SetColumn(DrillCard,1);TrainingBuilderGrid.SetRow(DrillCard,0);}}
 public void ApplyQueryAttributes(IDictionary<string,object> query){if(query.TryGetValue("sport",out var sport))((TrainingViewModel)BindingContext).SetRequestedSport(Uri.UnescapeDataString(sport?.ToString()??string.Empty));}
 protected override void OnAppearing(){base.OnAppearing();((TrainingViewModel)BindingContext).LoadCommand.Execute(null);}
 async void RequestsClicked(object? s,EventArgs e)=>await Shell.Current.GoToAsync(nameof(TrainingRequestsPage));
 async void BackClicked(object? s,EventArgs e){if(Shell.Current.Navigation.NavigationStack.Count>1)await Shell.Current.GoToAsync("..");else await Shell.Current.GoToAsync("//Home");}
 async void ExitClicked(object? s,EventArgs e)=>await Shell.Current.GoToAsync("//Home");
 async void VideoClicked(object? s,EventArgs e){var drill=((TrainingViewModel)BindingContext).SelectedDrill;if(drill is null){await DisplayAlert("Select a drill","Choose a drill before opening the Drill Library.","OK");return;}await Shell.Current.GoToAsync($"{nameof(DrillLibraryPage)}?drillId={drill.Id}&fromTraining=true");}
}
public partial class TrainingRequestsPage:ContentPage{public TrainingRequestsPage(RequestsViewModel vm){InitializeComponent();BindingContext=vm;}protected override void OnAppearing(){base.OnAppearing();((RequestsViewModel)BindingContext).LoadCommand.Execute(null);}async void BackClicked(object? sender,EventArgs e){if(Shell.Current.Navigation.NavigationStack.Count>1)await Shell.Current.GoToAsync("..");else await Shell.Current.GoToAsync("//Training");}async void ExitClicked(object? sender,EventArgs e)=>await Shell.Current.GoToAsync("//Home");}
public partial class NotificationsPage:ContentPage
{
 public NotificationsPage(NotificationsViewModel vm){InitializeComponent();BindingContext=vm;}
 void NotificationsStageSizeChanged(object? s,EventArgs e){var phone=NotificationsStage.Width>0&&NotificationsStage.Width<620;NotificationsContent.Padding=phone?new Thickness(16,18,16,24):new Thickness(20);NotificationsHeader.ColumnDefinitions.Clear();NotificationsHeader.RowDefinitions.Clear();if(phone){NotificationsHeader.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));NotificationsHeader.RowDefinitions.Add(new RowDefinition(GridLength.Auto));NotificationsHeader.RowDefinitions.Add(new RowDefinition(GridLength.Auto));NotificationsHeader.SetColumn(NotificationActions,0);NotificationsHeader.SetRow(NotificationActions,1);NotificationActions.HorizontalOptions=LayoutOptions.Start;}else{NotificationsHeader.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));NotificationsHeader.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));NotificationsHeader.RowDefinitions.Add(new RowDefinition(GridLength.Auto));NotificationsHeader.SetColumn(NotificationActions,1);NotificationsHeader.SetRow(NotificationActions,0);NotificationActions.HorizontalOptions=LayoutOptions.End;}}
 protected override void OnAppearing(){base.OnAppearing();OnPropertyChanged(nameof(BindingContext));((NotificationsViewModel)BindingContext).LoadCommand.Execute(null);}
 async void BackClicked(object? sender,EventArgs e){if(Shell.Current.Navigation.NavigationStack.Count>1)await Shell.Current.GoToAsync("..");else await Shell.Current.GoToAsync("//Home");}
 async void HomeClicked(object? sender,EventArgs e)=>await Shell.Current.GoToAsync("//Home");
 async void NotificationSelected(object? sender,SelectionChangedEventArgs e){if(e.CurrentSelection.FirstOrDefault() is not NotificationItem item)return;((CollectionView)sender!).SelectedItem=null;var vm=(NotificationsViewModel)BindingContext;vm.ReadCommand.Execute(item);if(item.ActionRoute?.Contains("Training",StringComparison.OrdinalIgnoreCase)==true){var sport=await vm.ResolveSportAsync(item);await Shell.Current.GoToAsync($"//Training?sport={Uri.EscapeDataString(sport??string.Empty)}");}else if(!string.IsNullOrWhiteSpace(item.ActionRoute))await Shell.Current.GoToAsync(item.ActionRoute);}
}
public partial class ProfilePage:ContentPage
{
 const double LockerSourceWidth=1086;
 const double LockerSourceHeight=1448;
 // Coordinates are measured against the 1086 x 1448 approved locker-door artwork.
 // Keeping both labels in these image-relative bounds makes them move and scale
 // with the door as one visual object.
 static readonly Rect NamePlateBounds=new(402,226,282,58);
 static readonly Rect NumberPlateBounds=new(389,758,308,300);
 bool open;
 public ProfilePage(ProfileViewModel vm){InitializeComponent();BindingContext=vm;SizeChanged+=ProfileSizeChanged;}
 void ProfileSizeChanged(object? sender,EventArgs e)
 {
  var phone=Width>0&&Width<600;
  LockerDoorContainer.WidthRequest=phone?Math.Max(270,Math.Min(Width-32,390)):408;
  LockerDoorContainer.HeightRequest=LockerDoorContainer.WidthRequest*LockerSourceHeight/LockerSourceWidth;
  PlayerCardGrid.ColumnDefinitions.Clear();PlayerCardGrid.RowDefinitions.Clear();
  if(phone){PlayerCardGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));PlayerCardGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));PlayerCardGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));var details=PlayerCardGrid.Children[1];PlayerCardGrid.SetRow(details,1);PlayerCardGrid.SetColumn(details,0);ContactBioGrid.ColumnDefinitions.Clear();ContactBioGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));ContactBioGrid.RowDefinitions.Clear();ContactBioGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));ContactBioGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));ContactBioGrid.SetColumn(BioCard,0);ContactBioGrid.SetRow(BioCard,1);}
  else{PlayerCardGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(220)));PlayerCardGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));PlayerCardGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));var details=PlayerCardGrid.Children[1];PlayerCardGrid.SetRow(details,0);PlayerCardGrid.SetColumn(details,1);ContactBioGrid.ColumnDefinitions.Clear();ContactBioGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));ContactBioGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));ContactBioGrid.RowDefinitions.Clear();ContactBioGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));ContactBioGrid.SetColumn(BioCard,1);ContactBioGrid.SetRow(BioCard,0);}
 }
 protected override void OnAppearing(){base.OnAppearing();((ProfileViewModel)BindingContext).LoadCommand.Execute(null);}
 async void LockerTapped(object? s,TappedEventArgs e){if(open)return;open=true;ProfilePanel.Opacity=0;ProfilePanel.IsVisible=true;await LockerDoorContainer.TranslateToAsync(-LockerDoorContainer.Width,0,420,Easing.CubicInOut);LockerDoorContainer.IsVisible=false;await ProfilePanel.FadeToAsync(1,260,Easing.CubicOut);}
 void LockerDoorSizeChanged(object? sender,EventArgs e)
 {
  if(LockerDoorContainer.Width<=0||LockerDoorContainer.Height<=0)return;
  var scale=Math.Min(LockerDoorContainer.Width/LockerSourceWidth,LockerDoorContainer.Height/LockerSourceHeight);
  var renderedWidth=LockerSourceWidth*scale;var renderedHeight=LockerSourceHeight*scale;
  var left=(LockerDoorContainer.Width-renderedWidth)/2;var top=(LockerDoorContainer.Height-renderedHeight)/2;
  AbsoluteLayout.SetLayoutBounds(LockerNameLabel,new Rect(left+NamePlateBounds.X*scale,top+NamePlateBounds.Y*scale,NamePlateBounds.Width*scale,NamePlateBounds.Height*scale));
  AbsoluteLayout.SetLayoutBounds(LockerNumberLabel,new Rect(left+NumberPlateBounds.X*scale,top+NumberPlateBounds.Y*scale,NumberPlateBounds.Width*scale,NumberPlateBounds.Height*scale));
  var nameLength=((ProfileViewModel)BindingContext).DisplayName.Length;
  LockerNameLabel.FontSize=Math.Max(8,Math.Min(15,NamePlateBounds.Height*scale*.38-(Math.Max(0,nameLength-18)*.16)));
  LockerNumberLabel.FontSize=Math.Max(38,Math.Min(82,NumberPlateBounds.Height*scale*.42));
 }
 async void ProfileBackClicked(object? s,EventArgs e){if(!open)return;await ProfilePanel.FadeToAsync(0,180);ProfilePanel.IsVisible=false;LockerDoorContainer.TranslationX=0;LockerDoorContainer.IsVisible=true;open=false;}
 async void ProfileExitClicked(object? s,EventArgs e)=>await Shell.Current.GoToAsync("//Home");
 async void ChangePhotoClicked(object? s,EventArgs e){var file=await FilePicker.Default.PickAsync(new PickOptions{PickerTitle="Choose athlete photo",FileTypes=FilePickerFileType.Images});if(file is null)return;AthletePhoto.Source=ImageSource.FromFile(file.FullPath);PhotoPlaceholder.IsVisible=false;}
}
