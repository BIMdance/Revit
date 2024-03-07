using AutodeskRibbonPanel = Autodesk.Windows.RibbonPanel;
using Color = System.Windows.Media.Color;
using RevitRibbonItem = Autodesk.Revit.UI.RibbonItem;
using RevitRibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace BIMdance.Revit.Ribbon;

public class RibbonFactory
{
    private readonly UIControlledApplication _uiControlledApplication;
    private readonly RibbonItemFactory _ribbonItemFactory;
    private readonly RibbonVisibleUtils _ribbonVisibleUtils;
    private RevitRibbonPanel _currentRevitRibbonPanel;
    private string _ribbonName;

    public RibbonFactory(UIControlledApplication uiControlledApplication, RibbonVisibleUtils ribbonVisibleUtils)
    {
        _uiControlledApplication = uiControlledApplication;
        _ribbonItemFactory = new RibbonItemFactory(AssemblyUtils.GetExecutingAssemblyPath());
        _ribbonVisibleUtils = ribbonVisibleUtils;
    }

    public AutodeskRibbonPanel CurrentRibbonPanel { private set; get; }
    public RevitRibbonPanel CurrentRevitRibbonPanel
    {
        private set
        {
            _currentRevitRibbonPanel = value;

            var ribbon = ComponentManager.Ribbon;
            var ribbonTab = ribbon.Tabs.FirstOrDefault(n => n.Name == _ribbonName);

            CurrentRibbonPanel = ribbonTab?.Panels.Last();
        }
        get => _currentRevitRibbonPanel;
    }

    public RibbonTab CreateRibbonTab(string ribbonName, RibbonVisible ribbonVisible)
    {
        _ribbonName = ribbonName;
        _uiControlledApplication.CreateRibbonTab(_ribbonName);
        var ribbonTab = ComponentManager.Ribbon.Tabs.FirstOrDefault(x => x.Name == ribbonName);
        _ribbonVisibleUtils.SetVisibleCondition(ribbonTab, ribbonVisible);
        return ribbonTab;
    }

    public RevitRibbonPanel CreateRibbonPanel(string panelName, RibbonVisible ribbonVisible, Func<bool> visibleFunc = null)
    {
        var ribbonPanel = _uiControlledApplication.CreateRibbonPanel(_ribbonName, panelName);
        _ribbonVisibleUtils.SetVisibleCondition(ribbonPanel, ribbonVisible, visibleFunc);
        return CurrentRevitRibbonPanel = ribbonPanel;
    }

    public RevitRibbonItem AddPushButton(ExternalCommandDefinition externalCommand, RibbonVisible ribbonVisible = RibbonVisible.All, Func<bool> visibleFunc = null)
    {
        if (CurrentRevitRibbonPanel == null) throw new NullReferenceException($"{nameof(CurrentRevitRibbonPanel)}: {typeof(RevitRibbonPanel)}");
        var ribbonButton = _ribbonItemFactory.CreatePushButton(CurrentRevitRibbonPanel, externalCommand);
        _ribbonVisibleUtils.SetVisibleCondition(ribbonButton, ribbonVisible, visibleFunc);
        return ribbonButton;
    }

    public RevitRibbonItem AddPushButton(ExternalCommandDefinition externalCommand, Func<View, bool> visibleFunc)
    {
        if (CurrentRevitRibbonPanel == null) throw new NullReferenceException($"{nameof(CurrentRevitRibbonPanel)}: {typeof(RevitRibbonPanel)}");
        var ribbonButton = _ribbonItemFactory.CreatePushButton(CurrentRevitRibbonPanel, externalCommand);
        _ribbonVisibleUtils.SetVisibleCondition(ribbonButton, visibleFunc);
        return ribbonButton;
    }

    public RevitRibbonItem AddPushButton(ExternalCommandDefinition externalCommand, ViewType checkViewType, Func<View, bool> visibleFunc = null)
    {
        if (CurrentRevitRibbonPanel == null) throw new NullReferenceException($"{nameof(CurrentRevitRibbonPanel)}: {typeof(RevitRibbonPanel)}");
        var ribbonButton = _ribbonItemFactory.CreatePushButton(CurrentRevitRibbonPanel, externalCommand);
        _ribbonVisibleUtils.SetVisibleCondition(ribbonButton, checkViewType, visibleFunc);
        return ribbonButton;
    }

    public PulldownButton AddPulldownButton(List<ExternalCommandDefinition> externalCommands, ExternalCommandDefinition pulldownButtonDefinition = null, RibbonVisible ribbonVisible = RibbonVisible.All)
    {
        pulldownButtonDefinition ??= externalCommands.FirstOrDefault() ?? throw new ArgumentNullException($"{nameof(pulldownButtonDefinition)}");
        var pulldownButton = _ribbonItemFactory.CreatePulldownButton(CurrentRevitRibbonPanel, pulldownButtonDefinition);
        foreach (var pushButtonData in externalCommands.Select(_ribbonItemFactory.CreatePushButtonData))
        {
            pulldownButton.AddPushButton(pushButtonData);
        }
        _ribbonVisibleUtils.SetVisibleCondition(pulldownButton, ribbonVisible);
        return pulldownButton;
    }

    public PulldownButton AddPulldownButton(Dictionary<ExternalCommandDefinition, RibbonVisible> ribbonItemsParameters, ExternalCommandDefinition pulldownButtonDefinition = null)
    {
        pulldownButtonDefinition ??= ribbonItemsParameters.Keys.FirstOrDefault() ?? throw new ArgumentNullException($"{nameof(pulldownButtonDefinition)}");
        var pulldownButton = _ribbonItemFactory.CreatePulldownButton(CurrentRevitRibbonPanel, pulldownButtonDefinition);
        foreach (var ribbonItemParameters in ribbonItemsParameters)
        {
            var pushButtonData = _ribbonItemFactory.CreatePushButtonData(ribbonItemParameters.Key);
            var pushButton = pulldownButton.AddPushButton(pushButtonData);
            _ribbonVisibleUtils.SetVisibleCondition(pushButton, ribbonItemParameters.Value);
        }
        return pulldownButton;
    }

    public RibbonCheckBox AddCheckBox(RibbonCheckBoxDefinition ribbonCheckBoxDefinition)
    {
        if (CurrentRibbonPanel == null) throw new NullReferenceException($"{nameof(CurrentRibbonPanel)}: {typeof(AutodeskRibbonPanel)}");
        var ribbonCheckBox = _ribbonItemFactory.CreateRibbonCheckBoxItem(ribbonCheckBoxDefinition);
        CurrentRibbonPanel.Source.Items.Add(new RibbonFlowPanel { Items = { ribbonCheckBox }});
        return ribbonCheckBox;
    }

    public RibbonTextBox AddTextBox(RibbonTextBoxDefinition ribbonTextBoxDefinition)
    {
        if (CurrentRibbonPanel == null) throw new NullReferenceException($"{nameof(CurrentRibbonPanel)}: {typeof(AutodeskRibbonPanel)}");
        var ribbonTextBox = _ribbonItemFactory.CreateRibbonTextBoxItem(ribbonTextBoxDefinition);
        CurrentRibbonPanel.Source.Items.Add(new RibbonFlowPanel { Items = { ribbonTextBox } });
        return ribbonTextBox;
    }

    public RibbonLabel AddLabel(RibbonLabelDefinition ribbonLabelDefinition)
    {
        if (CurrentRibbonPanel == null) throw new NullReferenceException($"{nameof(CurrentRibbonPanel)}: {typeof(AutodeskRibbonPanel)}");
        var ribbonLabel = _ribbonItemFactory.CreateRibbonLabelItem(ribbonLabelDefinition);
        CurrentRibbonPanel.Source.Items.Add(new RibbonFlowPanel { Items = { ribbonLabel } });
        return ribbonLabel;
    }

    public void AddItemsToFlowPanel(List<IRibbonDefinition> ribbonDefinitions)
    {
        if (CurrentRibbonPanel == null) throw new NullReferenceException($"{nameof(CurrentRibbonPanel)}: {typeof(AutodeskRibbonPanel)}");
        var ribbonFlowPanel = new RibbonFlowPanel();
        foreach (var ribbonItem in ribbonDefinitions.Select(_ribbonItemFactory.CreateRibbonItem))
        {
            ribbonFlowPanel.Items.Add(ribbonItem);
        }
        CurrentRibbonPanel.Source.Items.Add(ribbonFlowPanel);
    }

    public void AddSeparator() => CurrentRevitRibbonPanel.AddSeparator();
    public void AddSlideOut() => CurrentRevitRibbonPanel.AddSlideOut();

    public void SetBackgroundImage(Bitmap bitmap)
    {
        var ribbon = ComponentManager.Ribbon;
        var ribbonTab = ribbon.Tabs.FirstOrDefault(n => n.Name == _ribbonName);
        var ribbonPanel = ribbonTab?.Panels.Last();

        if (ribbonPanel == null)
            return;

        ribbonPanel.Source.Items.Add(new RibbonLabel { Height = 70, Width = 192, });

        ribbonPanel.CustomPanelBackground =
            new ImageBrush
            {
                ImageSource = bitmap.ToBitmapSource(),
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                Stretch = Stretch.Uniform,
            };
    }

    public void SetRibbonStyle(Color background, Color titleBarBackground)
    {
        var ribbon = ComponentManager.Ribbon;
        var ribbonTab = ribbon.Tabs.FirstOrDefault(n => n.Name == _ribbonName);

        if (ribbonTab == null)
            return;

        foreach (var ribbonPanel in ribbonTab.Panels)
        {
            ribbonPanel.CustomPanelTitleBarBackground = new SolidColorBrush(titleBarBackground);
            ribbonPanel.CustomPanelBackground ??= new SolidColorBrush(background);
        }
    }
}