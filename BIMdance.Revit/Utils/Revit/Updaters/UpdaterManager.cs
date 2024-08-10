namespace BIMdance.Revit.Utils.Revit.Updaters;

public class UpdaterManager
{
    private readonly List<Updater> _updaters;
    private readonly AppSettings _settings;

    public UpdaterManager(
        List<Updater> updaters,
        AppSettings settings)
    {
        _updaters = updaters;
        _settings = settings;
    }

    public void RegisterUpdaters()
    {
        foreach (var updater in _updaters)
        {
            UpdaterRegistry.RegisterUpdater(updater, isOptional: true);
            updater.AddTriggers();
        }
        
        RefreshUpdatersState();
    }

    public void UnregisterAllUpdaters()
    {
        foreach (var updater in _updaters)
            UpdaterRegistry.UnregisterUpdater(updater.Id);
    }

    public void RefreshUpdatersState()
    {
        _settings.Load();
        
        foreach (var updater in _updaters)
            SetUpdaterState(updater.Id, _settings.IsUpdatersEnabled);
    }

    private static void SetUpdaterState(UpdaterId updaterId, bool settingState)
    {
        switch (UpdaterRegistry.IsUpdaterEnabled(updaterId), settingState)
        {
            case (false, true): UpdaterRegistry.EnableUpdater(updaterId);  break;
            case (true, false): UpdaterRegistry.DisableUpdater(updaterId); break;
        }
    }
}