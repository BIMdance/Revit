namespace BIMdance.Revit.Utils.Revit.Parameters;

public static class ParameterUtils
{
    public static bool IsValidParameter(this Parameter parameter) =>
        parameter is { HasValue: true };

    public static bool IsValidParameters(IEnumerable<Parameter> parameters) =>
        parameters.All(parameter => parameter.IsValidParameter());

    public static Parameter GetParameter(this Element element, Guid guid)
    {
        return element switch
        {
            FamilyInstance familyInstance => GetParameter(familyInstance, guid),
            _ => element.get_Parameter(guid)
        };
    }

    public static Parameter GetParameter(this FamilyInstance familyInstance, Guid guid)
    {
        var familyInstanceParameter = familyInstance.get_Parameter(guid);
        var symbolParameter = familyInstance.Symbol.get_Parameter(guid);

        if (symbolParameter == null)
            return familyInstanceParameter;

        if (symbolParameter.IsReadOnly)
            return familyInstanceParameter ?? symbolParameter;

        return symbolParameter;
    }

    public static Parameter GetInstanceParameter(FamilyInstance familyInstance, Guid guid, string alternativeName = null)
    {
        if (familyInstance == null)
            return null;

        return familyInstance.get_Parameter(guid) ??
               (string.IsNullOrEmpty(alternativeName) == false
                   ? familyInstance.LookupParameter(alternativeName)
                   : null);
    }

    public static Parameter GetSymbolOrInstanceParameter(FamilyInstance familyInstance, Guid guid, string alternativeName = null)
    {
        if (familyInstance == null)
            return null;

        var familySymbol = familyInstance.Symbol;

        return familySymbol.get_Parameter(guid) ??
               familyInstance.get_Parameter(guid) ??
               (string.IsNullOrEmpty(alternativeName) == false
                   ? familySymbol.LookupParameter(alternativeName) ??
                     familyInstance.LookupParameter(alternativeName)
                   : null);
    }

    public static Parameter GetSymbolOrInstanceParameter(FamilyInstance familyInstance, string parameterName)
    {
        if (familyInstance == null)
            return null;

        var familySymbol = familyInstance.Symbol;

        return familySymbol.LookupParameter(parameterName) ??
               familyInstance.LookupParameter(parameterName);
    }

    public static double? GetValueAsDouble(this Parameter parameter)
    {
        if (parameter is not { HasValue: true })
            return null;

        return parameter.StorageType switch
        {
            StorageType.Double => parameter.AsDouble(),
            StorageType.Integer => parameter.AsInteger(),
            StorageType.ElementId => parameter.AsInteger(),
            StorageType.String => parameter.AsString().FirstDouble(),
            _ => throw new ArgumentOutOfRangeException($"{parameter.StorageType}")
        };
    }

    public static int? GetValueAsInt(this Parameter parameter)
    {
        if (parameter is not { HasValue: true })
            return null;

        return parameter.StorageType switch
        {
            StorageType.Integer => parameter.AsInteger(),
            StorageType.ElementId => parameter.AsInteger(),
            StorageType.String => parameter.AsString().FirstInt(),
            StorageType.Double => (int)Math.Round(parameter.AsDouble()),
            _ => throw new ArgumentOutOfRangeException($"{parameter.StorageType}")
        };
    }

    public static string GetValueAsString(this Parameter parameter)
    {
        if (parameter is not { HasValue: true })
            return null;

        return parameter.StorageType switch
        {
            StorageType.Double => parameter.AsValueString(),
            StorageType.Integer => parameter.AsInteger().ToString(),
            StorageType.ElementId => parameter.AsElementId().ToString(),
            StorageType.String => parameter.AsString(),
            _ => throw new ArgumentOutOfRangeException($"{parameter.StorageType}")
        };
    }

    public static ElementId GetValueAsElementId(this Parameter parameter)
    {
        if (parameter is not { HasValue: true })
            return ElementId.InvalidElementId;

        return parameter.StorageType switch
        {
            StorageType.Integer => RevitVersionResolver.NewElementId(parameter.AsInteger()),
            StorageType.ElementId => parameter.AsElementId(),
            StorageType.String => RevitVersionResolver.NewElementId(parameter.AsString().FirstInt(defaultValue: 0)),
            StorageType.Double => RevitVersionResolver.NewElementId((int)Math.Round(parameter.AsDouble())),
            _ => throw new ArgumentOutOfRangeException($"{parameter.StorageType}")
        };
    }

    public static Parameter GetSimilarParameter(Element element, Parameter sourceParameter)
    {
        if (sourceParameter.IsShared)
        {
            var guid = sourceParameter.GUID;
            return element.get_Parameter(guid);
        }

        var sourceInternalDefinition = (InternalDefinition)sourceParameter.Definition;
        if (sourceInternalDefinition != null &&
            sourceInternalDefinition.BuiltInParameter != BuiltInParameter.INVALID)
        {
            return element.get_Parameter(sourceInternalDefinition.BuiltInParameter);
        }

        var sourceParameterName = sourceParameter.Definition.Name;
        return element.LookupParameter(sourceParameterName);
    }

    public static void SetParameter(this Element element, BuiltInParameter builtInParameter, object value)
    {
        if (element == null) throw new NullReferenceException("Element is NULL.");

        var parameter = element.get_Parameter(builtInParameter) ?? throw new NullReferenceException(
            $"Parameter '{builtInParameter}' is not found in Element '[{element.Id}] {element.Name}'.");

        SetParameterValue(element, parameter, value);
    }

    public static void SetParameter(this Element element, Guid parameterGuid, object value)
    {
        if (element == null) throw new NullReferenceException("Element is NULL.");

        var parameter = element.get_Parameter(parameterGuid) ?? throw new NullReferenceException(
            $"Parameter '{parameterGuid}' is not found in Element '[{element.Id}] {element.Name}'.");

        SetParameterValue(element, parameter, value);
    }

    public static void SetParameter(this Element element, string parameterName, object value)
    {
        if (element == null) throw new NullReferenceException("Element is NULL.");

        var parameter = element.LookupParameter(parameterName) ?? throw new NullReferenceException(
            $"Parameter '{parameterName}' is not found in Element '[{element.Id}] {element.Name}'.");

        SetParameterValue(element, parameter, value);
    }

    private static void SetParameterValue(Element element, Parameter parameter, object value)
    {
        if (parameter.IsReadOnly) throw new InvalidOperationException(
            $"Parameter '[{parameter.Id} / {parameter.GUID}] {parameter.Definition.Name}' of Element '[{element.Id}] {element.Name}' is read only.");

        SetValue(parameter, value);
    }

    public static void SetValue(this Parameter parameter, object value)
    {
        if (parameter.IsReadOnly)
            return;
        
        switch (parameter.StorageType)
        {
            case StorageType.Integer when value is bool @bool:
                parameter.Set(@bool ? 1 : 0);
                return;

            case StorageType.Integer when value is int @int:
                parameter.Set(@int);
                return;

            case StorageType.Integer when value is double @double:
                parameter.Set((int)@double);
                return;

            case StorageType.Integer when value is string @string:
                var intValue = @string.FirstInt(defaultValue: 0);
                parameter.Set(intValue);
                return;

            case StorageType.Double when value is double @double:
                parameter.Set(@double);
                return;

            case StorageType.Double when value is int @int:
                parameter.Set(@int);
                return;

            case StorageType.Double when value is decimal @decimal:
                parameter.Set((double)@decimal);
                return;

            case StorageType.Double when value is string @string && double.TryParse(@string, NumberStyles.Any, CultureInfo.InvariantCulture, out var @double):
                parameter.Set(RevitVersionResolver.Parameters.GetParameterType(parameter) switch
                {
                    ParameterTypeProxy.Length => @double.MillimetersToInternal(),
                    _ => @double
                });
                return;

            case StorageType.String:
                parameter.Set(value?.ToString());
                return;

            case StorageType.ElementId when value is ElementId elementId:
                parameter.Set(elementId);
                return;

            case StorageType.ElementId when value is int @int:
                parameter.Set(RevitVersionResolver.NewElementId(@int));
                return;

            case StorageType.ElementId when value is long @long:
                parameter.Set(RevitVersionResolver.NewElementId(@long));
                return;
        }
    }
}