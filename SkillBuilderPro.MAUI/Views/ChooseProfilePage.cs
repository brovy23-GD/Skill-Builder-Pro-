using SkillBuilderPro.MAUI.Services;
using Microsoft.Maui.Layouts;
namespace SkillBuilderPro.MAUI.Views;
public sealed class ChooseProfilePage : ContentPage
{
 readonly IAthleteApiService api;
 readonly List<Border> roleCards=[];
 readonly VerticalStackLayout root;
 static readonly (string Role,string Description)[] Roles=[("Athlete","Train. Track. Improve."),("Coach","Lead. Assign. Develop."),("Parent","Follow. Support. Encourage."),("Administrator","Operate. Oversee. Optimize.")];
 public ChooseProfilePage(IAthleteApiService api)
 {
  this.api=api;Title="Choose Your Experience";NavigationPage.SetHasNavigationBar(this,false);BackgroundImageSource="weight_room.png";
  var headerStack=new VerticalStackLayout();
  headerStack.Add(new Label{Text="SKILL BUILDER PRO",FontSize=34,FontAttributes=FontAttributes.Bold,TextColor=Colors.White,HorizontalTextAlignment=TextAlignment.Center});
  headerStack.Add(new Label{Text="CHOOSE YOUR EXPERIENCE",FontSize=21,TextColor=Color.FromArgb("#168CFF"),HorizontalTextAlignment=TextAlignment.Center});
  headerStack.Add(new Label{Text="SELECT HOW YOU'LL ENTER SKILL BUILDER PRO",FontSize=12,TextColor=Color.FromArgb("#D6E2EC"),HorizontalTextAlignment=TextAlignment.Center});
  var header=new Border{Style=(Style)Application.Current!.Resources["GlassHeaderStyle"],Content=headerStack};
  var cards=new FlexLayout{Direction=FlexDirection.Row,Wrap=FlexWrap.Wrap,JustifyContent=FlexJustify.Center,AlignItems=FlexAlignItems.Center};
  foreach(var item in Roles){var button=new Button{Text=item.Role.ToUpperInvariant(),CommandParameter=item.Role,Style=(Style)Application.Current.Resources["GlassActionButtonStyle"]};SemanticProperties.SetDescription(button,$"Continue as {item.Role}");button.Clicked+=RoleClicked;var stack=new VerticalStackLayout{Spacing=8,VerticalOptions=LayoutOptions.Center};stack.Add(button);stack.Add(new Label{Text=item.Description,TextColor=Color.FromArgb("#D6E2EC"),HorizontalTextAlignment=TextAlignment.Center});var card=new Border{WidthRequest=245,HeightRequest=150,Margin=8,Style=(Style)Application.Current.Resources["GlassCardStyle"],Content=stack};roleCards.Add(card);cards.Children.Add(card);}
  var demoButton=new Button{Text="TRY DEMO MODE",Style=(Style)Application.Current.Resources["GlassSecondaryButtonStyle"],MaximumWidthRequest=360,HorizontalOptions=LayoutOptions.Center};
  demoButton.Clicked+=DemoClicked;
  root=new VerticalStackLayout{Padding=24,Spacing=22,MaximumWidthRequest=1050,HorizontalOptions=LayoutOptions.Center,VerticalOptions=LayoutOptions.Center};root.Add(header);root.Add(cards);root.Add(demoButton);Content=new ScrollView{Content=root};
  SizeChanged+=LayoutForSize;
 }
 async void RoleClicked(object? sender,EventArgs e){var role=(string)((Button)sender!).CommandParameter;api.SelectRole(role);await Navigation.PushAsync(new LoginPage(api,role));}
 void DemoClicked(object? sender,EventArgs e){api.EnterDemoMode();Application.Current!.Windows[0].Page=new AppShell();}
 void LayoutForSize(object? sender,EventArgs e){var phone=Width>0&&Width<600;root.Padding=phone?new Thickness(16,24):new Thickness(24);root.Spacing=phone?14:22;foreach(var card in roleCards){card.WidthRequest=phone?Math.Max(260,Width-48):245;card.HeightRequest=phone?112:150;card.Margin=phone?new Thickness(0,5):new Thickness(8);}}
}
