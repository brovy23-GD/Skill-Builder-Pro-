using SkillBuilderPro.MAUI.Services;
using SkillBuilderPro.MAUI.ViewModels;
using System.ComponentModel;
using System.Diagnostics;

namespace SkillBuilderPro.MAUI.Views;

public partial class TrainingBuilderPage : ContentPage, IQueryAttributable
{
    private TrainingBuilderLayoutState? currentLayoutState;

    public TrainingBuilderPage(TrainingBuilderViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        viewModel.PropertyChanged += ViewModelPropertyChanged;

        ApplyBackground(viewModel.Background);

        Loaded += (_, _) =>
        {
            ApplyResponsiveLayout(force: true);
        };

        BuilderBackground.HandlerChanged += (_, _) =>
        {
            ApplyBackground(viewModel.Background);
        };

        SizeChanged += (_, _) =>
        {
            ApplyResponsiveLayout();
        };
    }

    private void ViewModelPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (BindingContext is not TrainingBuilderViewModel viewModel)
            return;

        if (e.PropertyName == nameof(TrainingBuilderViewModel.Background))
        {
            ApplyBackground(viewModel.Background);
        }

        if (e.PropertyName is
            nameof(TrainingBuilderViewModel.SelectedSport) or
            nameof(TrainingBuilderViewModel.CanAddSelectedDrill))
        {
            ApplyResponsiveLayout(force: true);
        }
    }

    private void ApplyBackground(string assetName)
    {
#if DEBUG
        Debug.WriteLine(
            $"[TrainingBuilder] applying-background={assetName} " +
            $"handler-ready={BuilderBackground.Handler is not null}");
#endif

        if (string.IsNullOrWhiteSpace(assetName))
            return;

        BuilderBackground.Source = ImageSource.FromFile(assetName);
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.TryGetValue("sport", out var sport) &&
            BindingContext is TrainingBuilderViewModel viewModel)
        {
            viewModel.SetRequestedSport(
                Uri.UnescapeDataString(
                    sport?.ToString() ?? string.Empty));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is not TrainingBuilderViewModel viewModel)
            return;

        viewModel.UpdateViewport(Width, Height);

        ApplyResponsiveLayout(force: true);

        ApplyBackground(viewModel.Background);

        if (viewModel.Sports.Count == 0)
        {
            viewModel.LoadCommand.Execute(null);
        }

        Dispatcher.Dispatch(() =>
        {
            ApplyBackground(viewModel.Background);
        });
    }

    private void ApplyResponsiveLayout(bool force = false)
    {
        if (Width <= 0 ||
            Height <= 0 ||
            BindingContext is not TrainingBuilderViewModel viewModel)
        {
            return;
        }

        viewModel.UpdateViewport(Width, Height);

        var (deviceClass, orientation) =
            SportVisualService.ClassifyViewport(
                Width,
                Height);

        var state =
            ToLayoutState(
                deviceClass,
                orientation);

        if (!force &&
            state == currentLayoutState)
        {
            return;
        }

        currentLayoutState = state;

        ResetLayout();

        ApplySurfaceMaterial(
            viewModel.SelectedSport,
            state);

        ApplyFilterLayout(state);

        switch (state)
        {
            case TrainingBuilderLayoutState.PhonePortrait:

                ConfigurePhonePortrait(viewModel);

                break;

            case TrainingBuilderLayoutState.PhoneLandscape:

                ConfigurePhoneLandscape(viewModel);

                break;

            case TrainingBuilderLayoutState.TabletPortrait:

                ConfigureTabletPortrait();

                break;

            case TrainingBuilderLayoutState.TabletLandscape:

                ConfigureTabletLandscape(
                    viewModel.SelectedSport);

                break;

            default:

                ConfigureDesktop(
                    viewModel.SelectedSport);

                break;
        }

#if DEBUG
        Debug.WriteLine(
            $"[TrainingBuilder] layout={state} " +
            $"viewport={Width:0}x{Height:0} " +
            $"sport={viewModel.SelectedSport} " +
            $"asset={viewModel.Background}");
#endif
    }

    /*
     * IMPORTANT:
     *
     * This method changes only layout definitions.
     *
     * It never removes bound controls from their parent.
     *
     * ErrorCard, WorkoutCard, FindDrillsCard,
     * SessionCard and SummaryCard remain permanent
     * children of CardsGrid.
     */
    private void ResetLayout()
    {
        HeaderGrid.ColumnDefinitions.Clear();
        HeaderGrid.RowDefinitions.Clear();
        HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        HeaderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        HeaderGrid.SetColumn(HeaderActions, 1);
        HeaderGrid.SetRow(HeaderActions, 0);
        HeaderActions.HorizontalOptions = LayoutOptions.End;
        BuilderTitle.FontSize = 28;

        OverlayLayout.RowDefinitions.Clear();
        OverlayLayout.ColumnDefinitions.Clear();

        CardsGrid.RowDefinitions.Clear();
        CardsGrid.ColumnDefinitions.Clear();

        BuilderContentScroll.IsVisible = true;

        StickyActionBar.IsVisible = false;

        HeaderCard.MaximumWidthRequest =
            double.PositiveInfinity;

        BuilderContentScroll.MaximumWidthRequest =
            double.PositiveInfinity;

        CardsGrid.MaximumWidthRequest =
            double.PositiveInfinity;

        /*
         * Reset attached Grid state so no responsive
         * configuration inherits row/column spans
         * from the previous orientation.
         */
        ResetPlacement(HeaderCard);
        ResetPlacement(BuilderContentScroll);
        ResetPlacement(StickyActionBar);

        ResetPlacement(ErrorCard);
        ResetPlacement(WorkoutCard);
        ResetPlacement(FindDrillsCard);
        ResetPlacement(SessionCard);
        ResetPlacement(SummaryCard);

        var side =
            Math.Max(
                12,
                Width *
                (currentLayoutState ==
                    TrainingBuilderLayoutState.PhonePortrait
                    ? .04
                    : .03));

        var vertical =
            Math.Max(
                10,
                Height * .02);

        OverlayLayout.Margin =
            new Thickness(
                side,
                vertical,
                side,
                currentLayoutState == TrainingBuilderLayoutState.PhonePortrait ? 84 : Math.Max(vertical, 12));
    }

    /*
     * PHONE PORTRAIT
     *
     * HEADER
     * ----------------
     * Error
     * Workout
     * Find Drills
     * Session
     * Summary
     * ----------------
     * Sticky ADD action
     */
    private void ConfigurePhonePortrait(
        TrainingBuilderViewModel viewModel)
    {
        HeaderGrid.ColumnDefinitions.Clear();
        HeaderGrid.RowDefinitions.Clear();
        HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        HeaderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        HeaderGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        HeaderGrid.SetColumn(HeaderActions, 0);
        HeaderGrid.SetRow(HeaderActions, 1);
        HeaderActions.HorizontalOptions = LayoutOptions.Start;
        BuilderTitle.FontSize = 23;

        AddOverlayRows(
            GridLength.Auto,
            GridLength.Star,
            GridLength.Auto);

        AddOverlayColumns(
            GridLength.Star);

        AddCardRows(
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto);

        AddCardColumns(
            GridLength.Star);

        Place(
            HeaderCard,
            row: 0,
            column: 0);

        Place(
            BuilderContentScroll,
            row: 1,
            column: 0);

        Place(
            StickyActionBar,
            row: 2,
            column: 0);

        PlaceCard(
            ErrorCard,
            row: 0,
            column: 0);

        PlaceCard(
            WorkoutCard,
            row: 1,
            column: 0);

        PlaceCard(
            FindDrillsCard,
            row: 2,
            column: 0);

        PlaceCard(
            SessionCard,
            row: 3,
            column: 0);

        PlaceCard(
            SummaryCard,
            row: 4,
            column: 0);

        StickyActionBar.IsVisible =
            viewModel.CanAddSelectedDrill;

        DrillResults.HeightRequest =
            Math.Clamp(
                Height * .25,
                210,
                300);

        SessionItemsList.HeightRequest =
            Math.Clamp(
                Height * .25,
                220,
                320);
    }

    /*
     * PHONE LANDSCAPE
     *
     * Preserve the original SBP layout intent:
     * workflow lives in roughly the left 40%,
     * while the environment remains visible
     * across the right side.
     */
    private void ConfigurePhoneLandscape(
        TrainingBuilderViewModel viewModel)
    {
        AddOverlayRows(
            GridLength.Auto,
            GridLength.Star,
            GridLength.Auto);

        AddOverlayColumns(
            new GridLength(
                .40,
                GridUnitType.Star),

            new GridLength(
                .60,
                GridUnitType.Star));

        AddCardRows(
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto);

        AddCardColumns(
            GridLength.Star);

        Place(
            HeaderCard,
            row: 0,
            column: 0);

        Place(
            BuilderContentScroll,
            row: 1,
            column: 0);

        Place(
            StickyActionBar,
            row: 2,
            column: 0);

        PlaceCard(
            ErrorCard,
            row: 0,
            column: 0);

        PlaceCard(
            WorkoutCard,
            row: 1,
            column: 0);

        PlaceCard(
            FindDrillsCard,
            row: 2,
            column: 0);

        PlaceCard(
            SessionCard,
            row: 3,
            column: 0);

        PlaceCard(
            SummaryCard,
            row: 4,
            column: 0);

        StickyActionBar.IsVisible =
            viewModel.CanAddSelectedDrill;

        DrillResults.HeightRequest =
            Math.Clamp(
                Height * .29,
                180,
                300);

        SessionItemsList.HeightRequest =
            Math.Clamp(
                Height * .34,
                210,
                330);
    }

    /*
     * TABLET PORTRAIT
     *
     * Workflow column on left.
     * Session column on right.
     *
     * All cards remain permanent children
     * of CardsGrid.
     */
    private void ConfigureTabletPortrait()
    {
        AddOverlayRows(
            GridLength.Auto,
            GridLength.Star);

        AddOverlayColumns(
            GridLength.Star);

        AddCardRows(
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto);

        AddCardColumns(
            new GridLength(
                .47,
                GridUnitType.Star),

            new GridLength(
                .53,
                GridUnitType.Star));

        Place(
            HeaderCard,
            row: 0,
            column: 0);

        Place(
            BuilderContentScroll,
            row: 1,
            column: 0);

        /*
         * LEFT WORKFLOW
         */
        PlaceCard(
            ErrorCard,
            row: 0,
            column: 0);

        PlaceCard(
            WorkoutCard,
            row: 1,
            column: 0);

        PlaceCard(
            FindDrillsCard,
            row: 2,
            column: 0);

        /*
         * RIGHT SESSION
         */
        PlaceCard(
            SessionCard,
            row: 0,
            column: 1,
            rowSpan: 2);

        PlaceCard(
            SummaryCard,
            row: 2,
            column: 1);

        DrillResults.HeightRequest =
            Math.Clamp(
                Height * .29,
                260,
                460);

        SessionItemsList.HeightRequest =
            Math.Clamp(
                Height * .21,
                240,
                390);
    }

    /*
     * TABLET LANDSCAPE
     *
     * Workflow rail | environment | session rail
     */
    private void ConfigureTabletLandscape(
        string? sport)
    {
        var left =
            string.Equals(
                sport,
                "Soccer",
                StringComparison.OrdinalIgnoreCase)
                ? .38
                : .42;

        const double center = .29;

        var right =
            1 - left - center;

        AddOverlayRows(
            GridLength.Auto,
            GridLength.Star);

        AddOverlayColumns(
            GridLength.Star);

        AddCardRows(
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto);

        AddCardColumns(
            new GridLength(
                left,
                GridUnitType.Star),

            new GridLength(
                center,
                GridUnitType.Star),

            new GridLength(
                right,
                GridUnitType.Star));

        Place(
            HeaderCard,
            row: 0,
            column: 0);

        Place(
            BuilderContentScroll,
            row: 1,
            column: 0);

        /*
         * LEFT WORKFLOW RAIL
         */
        PlaceCard(
            ErrorCard,
            row: 0,
            column: 0);

        PlaceCard(
            WorkoutCard,
            row: 1,
            column: 0);

        PlaceCard(
            FindDrillsCard,
            row: 2,
            column: 0);

        /*
         * CENTER COLUMN INTENTIONALLY EMPTY
         *
         * This preserves the background
         * field/court/rink hero zone.
         */

        /*
         * RIGHT SESSION RAIL
         */
        PlaceCard(
            SessionCard,
            row: 0,
            column: 2,
            rowSpan: 2);

        PlaceCard(
            SummaryCard,
            row: 2,
            column: 2);

        DrillResults.HeightRequest =
            Math.Clamp(
                Height * .36,
                260,
                430);

        SessionItemsList.HeightRequest =
            Math.Clamp(
                Height * .35,
                250,
                410);
    }

    /*
     * DESKTOP / WINDOWS
     *
     * Workflow rail | protected hero zone | session rail
     */
    private void ConfigureDesktop(
        string? sport)
    {
        var left =
            string.Equals(
                sport,
                "Soccer",
                StringComparison.OrdinalIgnoreCase)
                ? .24
                : .27;

        var right =
            string.Equals(
                sport,
                "Basketball",
                StringComparison.OrdinalIgnoreCase)
                ? .23
                : .27;

        var center =
            1 - left - right;

        AddOverlayRows(
            GridLength.Auto,
            GridLength.Star);

        AddOverlayColumns(
            GridLength.Star);

        AddCardRows(
            GridLength.Auto,
            GridLength.Auto,
            GridLength.Auto);

        AddCardColumns(
            new GridLength(
                left,
                GridUnitType.Star),

            new GridLength(
                center,
                GridUnitType.Star),

            new GridLength(
                right,
                GridUnitType.Star));

        Place(
            HeaderCard,
            row: 0,
            column: 0);

        Place(
            BuilderContentScroll,
            row: 1,
            column: 0);

        /*
         * LEFT WORKFLOW RAIL
         */
        PlaceCard(
            ErrorCard,
            row: 0,
            column: 0);

        PlaceCard(
            WorkoutCard,
            row: 1,
            column: 0);

        PlaceCard(
            FindDrillsCard,
            row: 2,
            column: 0);

        /*
         * CENTER COLUMN STAYS EMPTY
         * so the approved sport environment
         * remains the visual hero.
         */

        /*
         * RIGHT SESSION RAIL
         */
        PlaceCard(
            SessionCard,
            row: 0,
            column: 2,
            rowSpan: 2);

        PlaceCard(
            SummaryCard,
            row: 2,
            column: 2);

        HeaderCard.MaximumWidthRequest =
            520;

        DrillResults.HeightRequest =
            Math.Clamp(
                Height * .30,
                250,
                390);

        SessionItemsList.HeightRequest =
            Math.Clamp(
                Height * .48,
                300,
                470);
    }

    /*
     * Filter controls remain permanently
     * inside FilterGrid.
     *
     * Only their Grid positions change.
     */
    private void ApplyFilterLayout(
        TrainingBuilderLayoutState state)
    {
        FilterGrid.ColumnDefinitions.Clear();
        FilterGrid.RowDefinitions.Clear();

        if (state is
            TrainingBuilderLayoutState.PhonePortrait or
            TrainingBuilderLayoutState.PhoneLandscape)
        {
            FilterGrid.ColumnDefinitions.Add(
                new ColumnDefinition(
                    GridLength.Star));

            for (var row = 0; row < 3; row++)
            {
                FilterGrid.RowDefinitions.Add(
                    new RowDefinition(
                        GridLength.Auto));
            }

            PlaceFilter(
                SportFilter,
                0,
                0);

            PlaceFilter(
                CategoryFilter,
                1,
                0);

            PlaceFilter(
                SkillFilter,
                2,
                0);
        }
        else if (
            state ==
            TrainingBuilderLayoutState.TabletPortrait)
        {
            FilterGrid.ColumnDefinitions.Add(
                new ColumnDefinition(
                    GridLength.Star));

            FilterGrid.ColumnDefinitions.Add(
                new ColumnDefinition(
                    GridLength.Star));

            FilterGrid.RowDefinitions.Add(
                new RowDefinition(
                    GridLength.Auto));

            FilterGrid.RowDefinitions.Add(
                new RowDefinition(
                    GridLength.Auto));

            PlaceFilter(
                SportFilter,
                0,
                0);

            PlaceFilter(
                CategoryFilter,
                0,
                1);

            PlaceFilter(
                SkillFilter,
                1,
                0,
                2);
        }
        else
        {
            FilterGrid.ColumnDefinitions.Add(
                new ColumnDefinition(
                    GridLength.Star));

            for (var row = 0; row < 3; row++)
            {
                FilterGrid.RowDefinitions.Add(
                    new RowDefinition(
                        GridLength.Auto));
            }

            PlaceFilter(
                SportFilter,
                0,
                0);

            PlaceFilter(
                CategoryFilter,
                1,
                0);

            PlaceFilter(
                SkillFilter,
                2,
                0);
        }
    }

    private void ApplySurfaceMaterial(
        string? sport,
        TrainingBuilderLayoutState state)
    {
        var alpha = 150;

        if (string.Equals(
                sport,
                "Hockey",
                StringComparison.OrdinalIgnoreCase))
        {
            alpha = 185;
        }
        else if (
            string.Equals(
                sport,
                "Football",
                StringComparison.OrdinalIgnoreCase) &&
            state ==
            TrainingBuilderLayoutState.PhonePortrait)
        {
            alpha = 170;
        }

        var surface =
            Color.FromRgba(
                8,
                16,
                24,
                alpha);

        var header =
            Color.FromRgba(
                6,
                13,
                21,
                Math.Min(
                    230,
                    alpha + 10));

        HeaderCard.BackgroundColor =
            header;

        ErrorCard.BackgroundColor =
            surface;

        WorkoutCard.BackgroundColor =
            surface;

        FindDrillsCard.BackgroundColor =
            surface;

        SessionCard.BackgroundColor =
            surface;

        SummaryCard.BackgroundColor =
            surface;

        StickyActionBar.BackgroundColor =
            surface;

        StickyActionBar.Stroke =
            new SolidColorBrush(
                Color.FromRgba(
                    15,
                    124,
                    255,
                    150));

        var compact =
            string.Equals(
                sport,
                "Softball",
                StringComparison.OrdinalIgnoreCase) &&
            state ==
            TrainingBuilderLayoutState.PhonePortrait;

        /*
         * CardsGrid replaces PhoneFlowStack.
         */
        CardsGrid.RowSpacing =
            compact
                ? 8
                : 12;

        CardsGrid.ColumnSpacing =
            compact
                ? 8
                : 12;

        WorkoutCard.Padding =
            compact
                ? 10
                : 14;

        FindDrillsCard.Padding =
            compact
                ? 10
                : 14;
    }

    private static TrainingBuilderLayoutState ToLayoutState(
        VisualDeviceClass deviceClass,
        VisualOrientation orientation)
    {
        return deviceClass switch
        {
            VisualDeviceClass.Desktop =>
                TrainingBuilderLayoutState.Desktop,

            VisualDeviceClass.Tablet
                when orientation ==
                     VisualOrientation.Portrait =>
                TrainingBuilderLayoutState.TabletPortrait,

            VisualDeviceClass.Tablet =>
                TrainingBuilderLayoutState.TabletLandscape,

            VisualDeviceClass.Phone
                when orientation ==
                     VisualOrientation.Portrait =>
                TrainingBuilderLayoutState.PhonePortrait,

            _ =>
                TrainingBuilderLayoutState.PhoneLandscape
        };
    }

    private void AddOverlayRows(
        params GridLength[] rows)
    {
        foreach (var row in rows)
        {
            OverlayLayout.RowDefinitions.Add(
                new RowDefinition(row));
        }
    }

    private void AddOverlayColumns(
        params GridLength[] columns)
    {
        foreach (var column in columns)
        {
            OverlayLayout.ColumnDefinitions.Add(
                new ColumnDefinition(column));
        }
    }

    private void AddCardRows(
        params GridLength[] rows)
    {
        foreach (var row in rows)
        {
            CardsGrid.RowDefinitions.Add(
                new RowDefinition(row));
        }
    }

    private void AddCardColumns(
        params GridLength[] columns)
    {
        foreach (var column in columns)
        {
            CardsGrid.ColumnDefinitions.Add(
                new ColumnDefinition(column));
        }
    }

    private static void Place(
        View view,
        int row,
        int column,
        int rowSpan = 1,
        int columnSpan = 1)
    {
        Grid.SetRow(
            view,
            row);

        Grid.SetColumn(
            view,
            column);

        Grid.SetRowSpan(
            view,
            rowSpan);

        Grid.SetColumnSpan(
            view,
            columnSpan);
    }

    private static void PlaceCard(
        View view,
        int row,
        int column,
        int rowSpan = 1,
        int columnSpan = 1)
    {
        Grid.SetRow(
            view,
            row);

        Grid.SetColumn(
            view,
            column);

        Grid.SetRowSpan(
            view,
            rowSpan);

        Grid.SetColumnSpan(
            view,
            columnSpan);
    }

    private static void PlaceFilter(
        View view,
        int row,
        int column,
        int columnSpan = 1)
    {
        Grid.SetRow(
            view,
            row);

        Grid.SetColumn(
            view,
            column);

        Grid.SetRowSpan(
            view,
            1);

        Grid.SetColumnSpan(
            view,
            columnSpan);
    }

    private static void ResetPlacement(
        View view)
    {
        Grid.SetRow(
            view,
            0);

        Grid.SetColumn(
            view,
            0);

        Grid.SetRowSpan(
            view,
            1);

        Grid.SetColumnSpan(
            view,
            1);
    }

    private async void BackClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private enum TrainingBuilderLayoutState
    {
        PhonePortrait,
        PhoneLandscape,
        TabletPortrait,
        TabletLandscape,
        Desktop
    }
}
