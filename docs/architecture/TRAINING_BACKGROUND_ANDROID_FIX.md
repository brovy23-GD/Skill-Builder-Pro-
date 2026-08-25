# Training Background Android Rendering Fix

## Result

**BUILD PASS**

The controlled change was applied only to `SkillBuilderPro.MAUI/Views/TrainingPage.xaml`: the negative `ZIndex="-2"` was removed from the page background `Image`.

No visual-success claim is made. Android emulator verification is still required.

## Pre-edit inspection

Before editing, line 3 was:

```xml
  <Image x:Name="TrainingBackground" Source="{Binding Background}" Aspect="AspectFill" Opacity="1" ZIndex="-2" HorizontalOptions="Fill" VerticalOptions="Fill" SemanticProperties.Description="Selected sport training environment"/><BoxView Color="#14040910" ZIndex="-1"/>
```

This confirmed:

- The background source remained dynamically bound through `Source="{Binding Background}"`.
- The background `Image` used `ZIndex="-2"`.
- The overlay `BoxView` used `ZIndex="-1"`.

## Applied change

After editing, line 3 is:

```xml
  <Image x:Name="TrainingBackground" Source="{Binding Background}" Aspect="AspectFill" Opacity="1" HorizontalOptions="Fill" VerticalOptions="Fill" SemanticProperties.Description="Selected sport training environment"/><BoxView Color="#14040910" ZIndex="-1"/>
```

Preserved unchanged:

- `Source="{Binding Background}"`
- `Aspect="AspectFill"`
- `Opacity="1"`
- Horizontal and vertical fill layout
- Semantic description
- Overlay `BoxView`, including `ZIndex="-1"`
- All bindings, buttons, text, and unrelated UI
- `SportVisualService` and selected-sport logic

No implementation file other than `TrainingPage.xaml` was modified. This report is the required documentation artifact.

## Exact git diff

```diff
diff --git a/SkillBuilderPro.MAUI/Views/TrainingPage.xaml b/SkillBuilderPro.MAUI/Views/TrainingPage.xaml
index 3e80133..3cb2420 100644
--- a/SkillBuilderPro.MAUI/Views/TrainingPage.xaml
+++ b/SkillBuilderPro.MAUI/Views/TrainingPage.xaml
@@ -1,6 +1,6 @@
 <ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="SkillBuilderPro.MAUI.Views.TrainingPage" Title="Training">
  <Grid x:Name="TrainingStage" SizeChanged="TrainingStageSizeChanged" BackgroundColor="#080C12">
-  <Image x:Name="TrainingBackground" Source="{Binding Background}" Aspect="AspectFill" Opacity="1" ZIndex="-2" HorizontalOptions="Fill" VerticalOptions="Fill" SemanticProperties.Description="Selected sport training environment"/><BoxView Color="#14040910" ZIndex="-1"/>
+  <Image x:Name="TrainingBackground" Source="{Binding Background}" Aspect="AspectFill" Opacity="1" HorizontalOptions="Fill" VerticalOptions="Fill" SemanticProperties.Description="Selected sport training environment"/><BoxView Color="#14040910" ZIndex="-1"/>
   <ScrollView><VerticalStackLayout x:Name="TrainingContent" Padding="22" Spacing="14" MaximumWidthRequest="1120" HorizontalOptions="Center">
    <Border IsVisible="{Binding HasError}" BackgroundColor="#D018202A" Stroke="#5F82A3" StrokeThickness="1" StrokeShape="RoundRectangle 8" Padding="12"><Grid ColumnDefinitions="*,Auto" ColumnSpacing="12"><Label Text="{Binding ErrorMessage}" TextColor="#DCE6F0" VerticalTextAlignment="Center"/><Button Grid.Column="1" Text="RETRY" Command="{Binding LoadCommand}" Style="{StaticResource CompactNavButtonStyle}"/></Grid></Border>
    <Grid ColumnDefinitions="*,Auto"><VerticalStackLayout Spacing="2"><Label Text="ATHLETE TRAINING" Style="{StaticResource PageEyebrowStyle}"/><Label Text="TRAINING BUILDER" Style="{StaticResource PageTitleStyle}"/><Label Text="BUILD THE WORK. MASTER THE DETAILS." Style="{StaticResource PageSubtitleStyle}"/></VerticalStackLayout><HorizontalStackLayout Grid.Column="1" Spacing="8" VerticalOptions="Center"><Button Text="BACK" Clicked="BackClicked" Style="{StaticResource CompactNavButtonStyle}"/><Button Text="EXIT" Clicked="ExitClicked" Style="{StaticResource CompactNavButtonStyle}"/></HorizontalStackLayout></Grid>
```

## Android Debug build

Command:

```powershell
dotnet build SkillBuilderPro.MAUI/SkillBuilderPro.MAUI.csproj -f net10.0-android -c Debug
```

Final build output:

```text
Build succeeded.
    40 Warning(s)
    0 Error(s)
Time Elapsed 00:01:45.70
```

The warnings were existing obsolescence/nullability warnings in other source/generated files; the requested target compiled successfully to:

```text
SkillBuilderPro.MAUI/bin/Debug/net10.0-android/SkillBuilderPro.MAUI.dll
```

## Verification boundary

Compilation verifies that the XAML change is accepted for `net10.0-android` Debug. It does not verify rendering. The Training page must next be opened in an Android emulator and tested across sport selections to determine whether the bound sport-specific backgrounds render correctly.
