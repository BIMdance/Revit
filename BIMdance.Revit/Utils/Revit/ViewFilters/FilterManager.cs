using Color = Autodesk.Revit.DB.Color;

namespace BIMdance.Revit.Utils.Revit.ViewFilters;

internal class FilterManager
{
    private readonly Document _document;
    internal FilterManager(Document document) => _document = document;

    internal void ClearFilters(string filterKey, out Dictionary<ElementId, OverrideGraphicSettings> existFilterGraphicSettings)
    {
        var view = _document.ActiveView;
        
        RemoveFilters(view, filterKey);

        existFilterGraphicSettings = view.GetFilters().ToDictionary(n => n, view.GetFilterOverrides);

        foreach (var elementId in existFilterGraphicSettings.Keys)
            view.RemoveFilter(elementId);
    }

    internal void RemoveFilters(View view, params string[] filterKeys)
    {
        var filterIds = _document.GetElementsOfClass<ParameterFilterElement>()
            .Where(filter => filter.Name.StartsWith(Constants.AppPrefix) && filterKeys.Any(key => filter.Name.EndsWith(key)))
            .Select(filter => filter.Id);

        foreach (var elementId in filterIds.Where(view.IsFilterApplied))
            view.RemoveFilter(elementId);
    }

    internal void RemoveFilters(params string[] filterKeys)
    {
        var filterIds = _document.GetElementsOfClass<ParameterFilterElement>()
            .Where(filter => filter.Name.StartsWith(Constants.AppPrefix) && filterKeys.Any(key => filter.Name.EndsWith(key)))
            .Select(filter => filter.Id)
            .ToList();

        _document.Delete(filterIds);
    }

    internal void AddExistFilters(Dictionary<ElementId, OverrideGraphicSettings> existFilterGraphicSettings)
    {
        if (existFilterGraphicSettings == null)
            return;

        var view = _document.ActiveView;
        
        foreach (var filterGraphicSetting in existFilterGraphicSettings)
        {
            var idFilter = filterGraphicSetting.Key;
            view.AddFilter(idFilter);
            view.SetFilterOverrides(idFilter, filterGraphicSetting.Value);
        }
    }

    public void SetFilters(
        string name, string filterKey,
        IReadOnlyCollection<FilterRuleParameter> filterRuleParameters,
        FilterGraphicSettings filterGraphicSettings,
        IEnumerable<BuiltInCategory> categories)
    {
        var categoryIds = categories.Select(n => RevitVersionResolver.NewElementId((int)n)).ToList();

        if (!SetParameterIds(filterRuleParameters, categoryIds))
            return;

        var filterName = $"{Constants.AppPrefix}_{name}_{filterKey}";
        var rules = filterRuleParameters.Where(x => x.ParameterId != null).Select(x => x.CreateFilterRule()).ToList();

        SetFilter(filterName, categoryIds, rules, filterGraphicSettings);
    }

    private bool SetParameterIds(IReadOnlyCollection<FilterRuleParameter> filterRuleParameters, ICollection<ElementId> categoryIds)
    {
        var guids = filterRuleParameters.Select(n => n.SharedParameterGuid);
        var parameterIds = _document.GetElementsOfClass<SharedParameterElement>()
            .Where(n => guids.Contains(n.GuidValue))
            .ToDictionary(n => n.GuidValue, n => n.Id);
        var parametersOfCategories = ParameterFilterUtilities
            .GetFilterableParametersInCommon(_document, categoryIds);

        if (!parameterIds.Any() ||
            !parametersOfCategories.Intersect(parameterIds.Values).Any())
            return false;

        foreach (var filterRuleParameter in filterRuleParameters)
        {
            var guid = filterRuleParameter.SharedParameterGuid;
            if (parameterIds.TryGetValue(guid, out var id))
                filterRuleParameter.ParameterId = id;
        }

        return true;
    }
    
    private void SetFilter(
        string name,
        IList<ElementId> categoryIds,
        IList<FilterRule> rules,
        FilterGraphicSettings filterGraphicSettings)
    {
        try
        {
            var filter = GetFilter(name, categoryIds, rules);
            var graphicSettings = new OverrideGraphicSettings();
            SetGraphicSettings(graphicSettings, filterGraphicSettings);

            _document.ActiveView.SetFilterOverrides(filter.Id, graphicSettings);
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            Logger.Error($"Categories:\n\t{string.Join("\n\t", categoryIds.Select(n => (BuiltInCategory)n.GetValue()).ToList())}");
        }
    }
    
    private ParameterFilterElement GetFilter(string name, IList<ElementId> categoryIds, IList<FilterRule> rules)
    {
        return _document.GetElementsOfClass<ParameterFilterElement>().FirstOrDefault(n => n.Name == name) ??
               RevitVersionResolver.Filters.CreateParameterFilterElement(_document, name, categoryIds, rules);
    }

    private void SetGraphicSettings(
        OverrideGraphicSettings graphicSettings,
        FilterGraphicSettings filterGraphicSettings)
    {
        var solidPattern = _document.GetElementsOfClass<FillPatternElement>().FirstOrDefault(n => n?.GetFillPattern()?.IsSolidFill ?? false);

        switch (filterGraphicSettings)
        {
            case FilterGraphicSettings.Default:
                break;

            case FilterGraphicSettings.Halftone:
                graphicSettings.SetHalftone(true);
                break;

            case FilterGraphicSettings.Invisible:
                RevitVersionResolver.Graphics.SetSurfaceForegroundPatternVisible(graphicSettings, false);
                break;

            case FilterGraphicSettings.SolidBlueLight:
                graphicSettings.SetProjectionFill(solidPattern, new Color(050, 200, 255));
                break;

            case FilterGraphicSettings.SolidGreen:
                graphicSettings.SetProjectionFill(solidPattern, new Color(000, 255, 000));
                break;

            case FilterGraphicSettings.SolidGreenDark:
                graphicSettings.SetProjectionFill(solidPattern, new Color(000, 128, 000));
                break;

            case FilterGraphicSettings.SolidGreenLight:
                graphicSettings.SetProjectionFill(solidPattern, new Color(192, 255, 192));
                break;

            case FilterGraphicSettings.SolidOrange:
                graphicSettings.SetProjectionFill(solidPattern, new Color(255, 128, 000));
                break;

            case FilterGraphicSettings.SolidRed:
                graphicSettings.SetProjectionFill(solidPattern, new Color(255, 000, 000));
                break;

            case FilterGraphicSettings.SolidRedDark:
                graphicSettings.SetProjectionFill(solidPattern, new Color(128, 000, 000));
                break;

            case FilterGraphicSettings.SolidWhite:
                graphicSettings.SetProjectionFill(solidPattern, new Color(255, 255, 255));
                break;

            case FilterGraphicSettings.SolidYellow:
                graphicSettings.SetProjectionFill(solidPattern, new Color(255, 255, 000));
                break;

            case FilterGraphicSettings.LinesRed:
                graphicSettings.SetProjectionLineColor(new Color(255, 0, 0));
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(filterGraphicSettings), filterGraphicSettings, null);
        }
    }
}