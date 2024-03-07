// ReSharper disable InconsistentNaming

namespace BIMdance.Revit;

public static class UIControlledApplicationExtension
{
    public static ExternalApplicationBuilder StartBuilding(
        this UIControlledApplication uiControlledApplication,
        string applicationName) =>
        new(applicationName, uiControlledApplication);
}