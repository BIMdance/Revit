﻿using BIMdance.Revit.Logic.CableRouting.ViewFilters;
using Color = Autodesk.Revit.DB.Color;

namespace BIMdance.Revit.Logic.CableRouting;

public class NetworkDrawer
{
    private const string EdCableTraceBinding = "ED_Cable Trace Binding";
    private const string EdDash1 = "ED_Dash_1";
    private const string RouteFilterKey = "2D97D11D";
    private const string CableTrayFillingFilterKey = "CBE7E5AE";

    private readonly Document _document;
    private readonly ElectricalSystemConverter _electricalSystemConverter;
    private readonly NetworkElements _networkElements;
    private readonly GraphicsStyle _graphicStyle;

    private readonly List<BuiltInCategory> _cableTrays = new() {
        BuiltInCategory.OST_CableTray,
        BuiltInCategory.OST_CableTrayFitting,
    };

    private readonly List<BuiltInCategory> _cableTrayConduits = new() {
        BuiltInCategory.OST_CableTray,
        BuiltInCategory.OST_CableTrayFitting,
        BuiltInCategory.OST_Conduit,
        BuiltInCategory.OST_ConduitFitting,
    };
    
    private readonly List<BuiltInCategory> _electricalCategories = new() {
        BuiltInCategory.OST_CableTray,
        BuiltInCategory.OST_CableTrayFitting,
        BuiltInCategory.OST_Conduit,
        BuiltInCategory.OST_ConduitFitting,
        BuiltInCategory.OST_ElectricalEquipment,
        BuiltInCategory.OST_ElectricalFixtures,
        BuiltInCategory.OST_CommunicationDevices,
        BuiltInCategory.OST_DataDevices,
        BuiltInCategory.OST_FireAlarmDevices,
        BuiltInCategory.OST_GenericModel,
        BuiltInCategory.OST_LightingDevices,
        BuiltInCategory.OST_LightingFixtures,
        BuiltInCategory.OST_MechanicalEquipment,
        BuiltInCategory.OST_NurseCallDevices,
        BuiltInCategory.OST_SecurityDevices,
        BuiltInCategory.OST_TelephoneDevices,
    };

    public NetworkDrawer(Document document)
    {
        _document = document;
        var building = new BuildingConverter(document).Convert();
        var networkConverter = new NetworkConverter(document, building);
        _electricalSystemConverter = new ElectricalSystemConverter(document, building);
        _networkElements = new NetworkElements(networkConverter);
        _graphicStyle = GetGraphicsStyle();
    }

    private GraphicsStyle GetGraphicsStyle()
    {
        var style = GetByName<GraphicsStyle>(EdCableTraceBinding);

        if (style != null)
            return style;

        var lineCategory = _document.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
        var linePatternElement = GetLinePatternElement();

        _document.Transaction(_ =>
        {
            var lineStyleCategory = _document.Settings.Categories.NewSubcategory(lineCategory, EdCableTraceBinding);
            lineStyleCategory.LineColor = new Color(255, 0, 0);
            RevitVersionResolver.Graphics.SetProjectionLinePatternId(lineStyleCategory, linePatternElement.Id);
            lineStyleCategory.SetLineWeight(5, GraphicsStyleType.Projection);
        }, TransactionNames.LineStyles);

        style = GetByName<GraphicsStyle>(EdCableTraceBinding);

        return style;
    }

    private T GetByName<T>(string name) where T : Element
    {
        return new FilteredElementCollector(_document)
            .OfClass(typeof(T))
            .OfType<T>()
            .FirstOrDefault(x => x.Name == name);
    }

    private LinePatternElement GetLinePatternElement()
    {
        var linePatternElement = new FilteredElementCollector(_document)
            .OfClass(typeof(LinePatternElement))
            .OfType<LinePatternElement>()
            .FirstOrDefault(x => x.Name == EdDash1);

        if (linePatternElement != null)
            return linePatternElement;

        var linePattern = new LinePattern(EdDash1);

        linePattern.SetSegments(new List<LinePatternSegment>
        {
            new(LinePatternSegmentType.Dash, 1.0.MillimetersToInternal()),
            new(LinePatternSegmentType.Space, 1.0.MillimetersToInternal()),
        });

        _document.Transaction(_ => { linePatternElement = LinePatternElement.Create(_document, linePattern); },
            TransactionNames.LinePatterns);

        return linePatternElement;
    }

    public void DrawBinding(ElectricalSystem electricalSystem)
    {
        if (electricalSystem == null) throw new NullReferenceException(nameof(electricalSystem));
        
        DrawBinding(_electricalSystemConverter.Convert(electricalSystem));
    }

    public void DrawBinding(ElectricalSystemProxy electricalSystemProxy)
    {
        if (_networkElements is null)
            return;
        
        if (electricalSystemProxy.BaseEquipment is { TraceBinding: null })
            _networkElements.SetTraceBinding(electricalSystemProxy.BaseEquipment);
        
        if (electricalSystemProxy.Elements.Any(x => x.TraceBinding == null))
            _networkElements.SetTraceBinding(electricalSystemProxy);

        DrawBinding(electricalSystemProxy.BaseEquipment);

        foreach (var element in electricalSystemProxy.Elements)
            DrawBinding(element);
    }

    public void DrawBinding(ElectricalElementProxy element)
    {
        if (element == null)
        {
            Debug.WriteLine($"Element is NULL.", GetType().Namespace);
            return;
        }

        if (new UIDocument(_document).ActiveView is not ViewPlan viewPlan ||
            viewPlan.GenLevel == null)
        {
            Debug.WriteLine($"ActiveView must be a ViewPlan.", GetType().Namespace);
            return;
        }

        if (element.TraceBinding.ConnectorsCount is 0)
        {
            var revitTraceBinding = _document.GetElement(RevitVersionResolver.NewElementId(element.TraceBinding.RevitId));

            if (revitTraceBinding is { IsValidObject: true })
            {
                element.SetTraceBinding(revitTraceBinding switch
                {
                    CableTray cableTray => RevitMapper.Map<CableTray, CableTrayProxy>(cableTray),
                    Conduit conduit => RevitMapper.Map<Conduit, CableTrayProxy>(conduit),
                    _ => null
                });
            }
            
            foreach (var electricalSystem in element.ElectricalSystems)
            {
                _networkElements.SetTraceBinding(electricalSystem);
            }
        }

        if (element.TraceBinding is CableTrayConduitBaseProxy cableTrayConduit)
        {
            var cableTrayConduitSystemIds = _document.GetElement(RevitVersionResolver.NewElementId(cableTrayConduit.RevitId))?
                .get_Parameter(SharedParameters.ElectricalSystemIds)?.AsString()?
                .Trim('#').Split('#')
                .Select(x => long.TryParse(x, out var i) ? i : -1)
                .Where(x => x > 0) ?? Array.Empty<long>();
            
            if (!cableTrayConduitSystemIds.Intersect(element.ElectricalSystems.Select(x => x.RevitId)).Any())
            {
                return;
            }
        }
        
        var startPointProxy = new XYZProxy(element.LocationPoint);
        var endPointProxy = GetEndPoint(element);

        if (endPointProxy == null)
        {
            Debug.WriteLine(new NullReferenceException(nameof(endPointProxy)));
            return;
        }

        var level = viewPlan.GenLevel;
        startPointProxy.Z = level.Elevation;
        endPointProxy.Z = level.Elevation;

        var length = startPointProxy.DistanceTo(endPointProxy);
        var startPoint = RevitMapper.Map<XYZProxy, XYZ>(startPointProxy);

        if (length > 0.02)
        {
            if (Math.Abs(startPointProxy.X - endPointProxy.X) < 1e-3 ||
                Math.Abs(startPointProxy.Y - endPointProxy.Y) < 1e-3)
            {
                CreateDetailCurves(viewPlan, Line.CreateBound(startPoint, RevitMapper.Map<XYZProxy, XYZ>(endPointProxy)));
            }
            else
            {
                GetSegments(startPointProxy, endPointProxy, level, out var curve1, out var curve2);
                CreateDetailCurves(viewPlan, curve1, curve2);
            }
        }
        else
        {
            CreateDetailCurves(viewPlan, Arc.Create(startPoint, 0.1, 0, 2 * Math.PI, XYZ.BasisX, XYZ.BasisY));
        }
    }

    private void GetSegments(XYZProxy startPointProxy, XYZProxy endPointProxy, Level level,
        out Line curve1, out Line curve2)
    {
        var maxX = Math.Max(startPointProxy.X, endPointProxy.X);
        var middlePoint = startPointProxy.X > endPointProxy.X && startPointProxy.Y > endPointProxy.Y ||
                          endPointProxy.X > startPointProxy.X && endPointProxy.Y > startPointProxy.Y
            ? new XYZ(maxX, Math.Min(startPointProxy.Y, endPointProxy.Y), level.Elevation)
            : new XYZ(maxX, Math.Max(startPointProxy.Y, endPointProxy.Y), level.Elevation);
        var startPoint = RevitMapper.Map<XYZProxy, XYZ>(startPointProxy);
        curve1 = Line.CreateBound(startPoint, middlePoint);
        curve2 = Line.CreateBound(middlePoint, RevitMapper.Map<XYZProxy, XYZ>(endPointProxy));
    }

    private void CreateDetailCurves(ViewPlan viewPlan, params Curve[] curves)
    {
        foreach (var curve in curves)
        {
            var detailCurve = _document.Create.NewDetailCurve(viewPlan, curve);

            if (_graphicStyle != null)
                detailCurve.LineStyle = _graphicStyle;
        }
    }

    private static XYZProxy GetEndPoint(ElectricalElementProxy element)
    {
        var endPoint = element.TraceBinding switch
        {
            CableTrayConduitBaseProxy cableTrayConduit => element.LocationPoint.ProjectToSegment(cableTrayConduit.Point1, cableTrayConduit.Point2),
            ElectricalElementProxy bindingElement => bindingElement.LocationPoint,
            _ => null
        };

        return endPoint is not null ? new XYZProxy(endPoint) : null;
    }

    public void DrawRoute(ElectricalSystem electricalSystem)
    {
        if (electricalSystem == null)
            return;
        
        SetElectricalSystemIdsToElements(electricalSystem);

        var electricalSystemId = electricalSystem.Id.GetValue().ToString();
        var searchId = $"#{electricalSystemId}#";
        var filterManager = new FilterManager(_document);

        filterManager.ClearFilters(RouteFilterKey, out var existFilterGraphicSettings);

        filterManager.SetFilters(
            $"!ElectricalCircuit({electricalSystemId})_Route",
            RouteFilterKey,
            new [] { new FilterRuleParameter(SharedParameters.ElectricalSystemIds, searchId, FilterRuleMode.NotContains) },
            FilterGraphicSettings.Halftone,
            _electricalCategories);
        
        filterManager.SetFilters(
            $"ElectricalCircuit({electricalSystemId})_Route",
            RouteFilterKey,
            new [] { new FilterRuleParameter(SharedParameters.ElectricalSystemIds, searchId, FilterRuleMode.Contains) },
            FilterGraphicSettings.LinesRed,
            _electricalCategories);

        filterManager.SetFilters(
            $"!ElectricalCircuit({electricalSystemId})_Route",
            RouteFilterKey,
            new [] { new FilterRuleParameter(SharedParameters.ElectricalSystemIds, searchId, FilterRuleMode.NotContains) },
            FilterGraphicSettings.Halftone,
            _electricalCategories);
        
        filterManager.AddExistFilters(existFilterGraphicSettings);
    }

    private void SetElectricalSystemIdsToElements(MEPSystem electricalSystem)
    {
        var familyInstances = _document.NewElementCollector()
            .OfClass(typeof(FamilyInstance))
            .OfType<FamilyInstance>();

        foreach (var familyInstance in familyInstances)
        {
            var electricalSystemIdsParameter = familyInstance.get_Parameter(SharedParameters.ElectricalSystemIds);
            
            if (electricalSystemIdsParameter == null)
                continue;
            
            var familyInstanceSystemIds = RevitVersionResolver.Electrical.GetElectricalSystems(familyInstance)
                .Select(x => x.Id.GetValue()).ToList();

            var formatElectricalSystemIds = familyInstanceSystemIds.Contains(electricalSystem.Id.GetValue())
                ? $"#{string.Join("#", familyInstanceSystemIds)}#"
                : string.Empty;
            
            if (string.IsNullOrWhiteSpace(formatElectricalSystemIds) && !electricalSystemIdsParameter.HasValue)
                continue;
                
            electricalSystemIdsParameter.Set(formatElectricalSystemIds);
        }
        
        SetElectricalSystemIds(electricalSystem.BaseEquipment);

        foreach (FamilyInstance familyInstance in electricalSystem.Elements)
            SetElectricalSystemIds(familyInstance);
    }

    private static void SetElectricalSystemIds(FamilyInstance familyInstance)
    {
        var familyInstanceSystems = RevitVersionResolver.Electrical.GetElectricalSystems(familyInstance);
        var electricalSystemIds = $"#{string.Join("#", familyInstanceSystems.Select(x => x.Id.GetValue()))}#";
        familyInstance.get_Parameter(SharedParameters.ElectricalSystemIds)?.Set(electricalSystemIds);
    }

    public void AddCableTrayFillingFilters()
    {
        var filterManager = new FilterManager(_document);

        _document.Transaction(_ =>
        {
            filterManager.ClearFilters(CableTrayFillingFilterKey, out var existFilterGraphicSettings);
        
            filterManager.SetFilters(
                $"Trace=\"\"",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.ElectricalSystemIds, "", FilterRuleMode.Equals) },
                FilterGraphicSettings.Halftone,
                _cableTrayConduits);
        
            filterManager.SetFilters(
                $"CableTraysFilling=0",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.CableTray_FillingPercent, 0d, FilterRuleMode.Equals) },
                FilterGraphicSettings.SolidWhite,
                _cableTrayConduits);
        
            filterManager.SetFilters(
                $"CableTraysFillingLess10",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.CableTray_FillingPercent, 0.1d, FilterRuleMode.LessOrEqual) },
                FilterGraphicSettings.SolidBlueLight,
                _cableTrayConduits);
        
            filterManager.SetFilters(
                $"CableTraysFillingLess20",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.CableTray_FillingPercent, 0.2d, FilterRuleMode.LessOrEqual) },
                FilterGraphicSettings.SolidGreenLight,
                _cableTrayConduits);
        
            filterManager.SetFilters(
                $"CableTraysFillingLess30",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.CableTray_FillingPercent, 0.3d, FilterRuleMode.LessOrEqual) },
                FilterGraphicSettings.SolidGreen,
                _cableTrayConduits);
        
            filterManager.SetFilters(
                $"CableTraysFillingLess40",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.CableTray_FillingPercent, 0.4d, FilterRuleMode.LessOrEqual) },
                FilterGraphicSettings.SolidGreenDark,
                _cableTrayConduits);
        
            filterManager.SetFilters(
                $"CableTraysFillingLess50",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.CableTray_FillingPercent, 0.5d, FilterRuleMode.LessOrEqual) },
                FilterGraphicSettings.SolidYellow,
                _cableTrayConduits);
        
            filterManager.SetFilters(
                $"CableTraysFillingLess60",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.CableTray_FillingPercent, 0.6d, FilterRuleMode.LessOrEqual) },
                FilterGraphicSettings.SolidOrange,
                _cableTrayConduits);
        
            filterManager.SetFilters(
                $"CableTraysFillingLess60",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.CableTray_FillingPercent, 0.7d, FilterRuleMode.LessOrEqual) },
                FilterGraphicSettings.SolidRed,
                _cableTrayConduits);
        
            filterManager.SetFilters(
                $"CableTraysFillingGreater70",
                CableTrayFillingFilterKey,
                new [] { new FilterRuleParameter(SharedParameters.CableTray_FillingPercent, 0.7d, FilterRuleMode.Greater) },
                FilterGraphicSettings.SolidRedDark,
                _cableTrayConduits);
        
            filterManager.AddExistFilters(existFilterGraphicSettings);
        }, TransactionNames.AddFilters);
    }

    public void ClearView()
    {
        _document.Transaction(_ =>
        {
            ClearBindingLines();
            ClearViewFilters();
        }, TransactionNames.Delete);
    }

    public void ClearBindingLines()
    {
        if (_document.ActiveView is not ViewPlan viewPlan ||
            viewPlan.GenLevel == null)
        {
            Debug.WriteLine($"ActiveView must be a ViewPlan.", GetType().Namespace);
            return;
        }

        var curveElement = _document.NewElementCollector()
            .OfClass(typeof(CurveElement))
            .OfType<CurveElement>()
            .Where(x => x.OwnerViewId == viewPlan.Id && x.LineStyle.Name == EdCableTraceBinding)
            .Select(x => x.Id)
            .ToList();

        _document.Transaction(_ =>
        {
            _document.Delete(curveElement);
        }, TransactionNames.Delete);
    }

    public void ClearViewFilters()
    {
        _document.Transaction(_ =>
        {
            new FilterManager(_document).RemoveFilters(_document.ActiveView, RouteFilterKey, CableTrayFillingFilterKey);
        }, TransactionNames.Delete);
    }

    public void ClearDocumentFilters()
    {
        _document.Transaction(_ =>
        {
            new FilterManager(_document).RemoveFilters(RouteFilterKey, CableTrayFillingFilterKey);
        }, TransactionNames.Delete);
    }
}