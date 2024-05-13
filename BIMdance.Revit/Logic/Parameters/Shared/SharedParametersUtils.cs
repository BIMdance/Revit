using Binding = Autodesk.Revit.DB.Binding;

namespace BIMdance.Revit.Logic.Parameters.Shared;

public class SharedParametersUtils : IDisposable
{
    private readonly Document _document;
    private readonly string _applicationSharedParametersFile;
    private readonly string _temporarySharedParametersFile;
    private readonly DefinitionFileUtils _definitionFileUtils;

    public SharedParametersUtils(Document document)
    {
        _document = document;
        var definitionFile = RevitSourceFiles.DefinitionFile;
        _applicationSharedParametersFile = _document.Application.SharedParametersFilename;
        _temporarySharedParametersFile = Path.ChangeExtension(Path.GetTempFileName(), "txt");
        _definitionFileUtils = new DefinitionFileUtils(_temporarySharedParametersFile, definitionFile);
    }

    public void AddSharedParametersToProject(List<ProjectSharedParameter> sharedParameters)
    {
        _document.Transaction(_ =>
        {
            var documentSharedParameterGuids = GetSharedParameterGuids();
            var createdParameters = sharedParameters.Where(n => documentSharedParameterGuids.Contains(n.Guid)).ToList();
            var notCreatedParameters = sharedParameters.Where(n => false == documentSharedParameterGuids.Contains(n.Guid)).ToList();

            CheckSharedParameterCategories(createdParameters);

            notCreatedParameters.ForEach(AddSharedParameterToProject);

        }, TransactionNames.Parameters_Add);
    }

    public List<Guid> GetSharedParameterGuids()
    {
        return _document.IsFamilyDocument
            ? GetFamilySharedParameterGuids()
            : GetProjectSharedParameterGuids();
    }

    private List<Guid> GetFamilySharedParameterGuids() =>
        _document.FamilyManager.GetParameters().Where(n => n.IsShared)
            .Select(n => n.GUID).ToList();

    private List<Guid> GetProjectSharedParameterGuids() =>
        GetProjectSharedParameters()
            .Select(n => n.GuidValue).ToList();

    public void AddSharedParameterToProject(ProjectSharedParameter projectSharedParameter)
    {
        try
        {
            var bindingMap = _document.ParameterBindings;
            var categorySet = projectSharedParameter.GetCategorySet(_document);
            var sharedParameterDefinition = projectSharedParameter.BaseSharedParameterDefinition;
            var definition = GetExternalDefinition(sharedParameterDefinition);

            if (false == bindingMap.Contains(definition))
            {
                var binding = projectSharedParameter.IsInstance
                    ? (Binding) _document.Application.Create.NewInstanceBinding(categorySet)
                    : _document.Application.Create.NewTypeBinding(categorySet);

                bindingMap.Insert(definition, binding, (BuiltInParameterGroup)sharedParameterDefinition.ParameterGroup);

                Logger.Info($"Add shared parameter: {sharedParameterDefinition}");
            }
            else
            {
                var guid = sharedParameterDefinition.Guid;
                var mapIterator = bindingMap.ForwardIterator();

                mapIterator.Reset();

                while (mapIterator.MoveNext())
                {
                    var iteratorKey = mapIterator.Key;

                    if ((iteratorKey as ExternalDefinition)?.GUID != guid)
                        continue;

                    var binding = mapIterator.Current as ElementBinding;

                    projectSharedParameter.Categories.ForEach(builtInCategory =>
                    {
                        var category = Category.GetCategory(_document, builtInCategory);

                        if (binding != null && !binding.Categories.Contains(category))
                            binding.Categories.Insert(category);
                    });

                    break;
                }
            }
        }
        catch (Exception exception)
        {
            Logger.Error(projectSharedParameter.ToString());
            Logger.Error(exception);
        }
    }

    private void CheckSharedParameterCategories(IEnumerable<ProjectSharedParameter> projectSharedParameters)
    {
        var parameterBindings = GetProjectParameterBindings();
        var sharedParameterElements = GetProjectSharedParameters();
        var projectParameterDefinitions = parameterBindings.Keys.OfType<InternalDefinition>().ToList();

        foreach (var projectSharedParameter in projectSharedParameters)
        {
            try
            {
                var giud = projectSharedParameter.Guid;
                var sharedParameter = sharedParameterElements.FirstOrDefault(n => n.GuidValue == giud);
                var internalDefinition = sharedParameter?.GetDefinition();
                var projectDefinition = projectParameterDefinitions.FirstOrDefault(n => n.Name == internalDefinition?.Name);

                if (projectDefinition == null)
                    continue;

                if (parameterBindings.TryGetValue(projectDefinition, out var binding))
                    CheckCategorySetInProject(projectSharedParameter, internalDefinition, binding);
            }
            catch (Exception exception)
            {
                Logger.Error(projectSharedParameter.ToString());
                Logger.Error(exception);
            }
        }
    }

    private  Dictionary<Definition, ElementBinding> GetProjectParameterBindings()
    {
        var definitionBinding = new Dictionary<Definition, ElementBinding>();
        var bindingMap = _document.ParameterBindings;
        var mapIterator = bindingMap.ForwardIterator();

        mapIterator.Reset();

        while (mapIterator.MoveNext())
        {
            if (false == definitionBinding.ContainsKey(mapIterator.Key))
                definitionBinding.Add(mapIterator.Key, mapIterator.Current as ElementBinding);
        }

        return definitionBinding;
    }

    private List<SharedParameterElement> GetProjectSharedParameters()
    {
        using var collector = new FilteredElementCollector(_document);
        return collector.OfClass(typeof(SharedParameterElement))
            .OfType<SharedParameterElement>()
            .ToList();
    }

    private void CheckCategorySetInProject(ProjectSharedParameter projectSharedParameter, Definition definition, ElementBinding binding)
    {
        try
        {
            var isNotContainsAnyCategory = false;

            projectSharedParameter.Categories.ForEach(n =>
            {
                var category = Category.GetCategory(_document, n);

                if (binding.Categories.Contains(category))
                    return;

                binding.Categories.Insert(category);
                isNotContainsAnyCategory = true;
            });

            if (isNotContainsAnyCategory)
                _document.ParameterBindings.ReInsert(definition, binding);
        }
        catch (Exception exception)
        {
            Logger.Error(projectSharedParameter.ToString());
            Logger.Error(exception);
        }
    }

    public List<FamilyParameter> AddSharedParametersToFamily(IEnumerable<BaseSharedParameterDefinition> sharedParameterDefinitions) =>
        sharedParameterDefinitions.Select(n => AddSharedParameterToFamily(n)).ToList();

    public FamilyParameter AddSharedParameterToFamily(BaseSharedParameterDefinition sharedParameterDefinition, bool? isInstance = null)
    {
        if (false == _document.IsFamilyDocument)
            throw new InvalidOperationException($"Document '{_document.Title}' must be a Family.");
            
        var existParameter = _document.FamilyManager.get_Parameter(sharedParameterDefinition.Guid);

        if (existParameter != null)
            return existParameter;
        
        var externalDefinition = GetExternalDefinition(sharedParameterDefinition);
        isInstance ??= sharedParameterDefinition.IsInstance;
        
        return RevitVersionResolver.Parameters.AddFamilyParameter(_document.FamilyManager, externalDefinition, sharedParameterDefinition.ParameterGroup, (bool)isInstance);
    }

    public FamilyParameter ReplaceSharedParameter(FamilyParameter familyParameter, BaseSharedParameterDefinition newBaseSharedParameter, ParameterGroupProxy? newParameterGroup = null, bool? isInstance = null)
    {
        var newExternalDefinition = GetExternalDefinition(newBaseSharedParameter);
        return RevitVersionResolver.Parameters.ReplaceFamilyParameter(
            _document.FamilyManager,
            familyParameter,
            newExternalDefinition,
            newParameterGroup ?? RevitVersionResolver.Parameters.GetParameterGroup(familyParameter),
            isInstance ?? familyParameter.IsInstance);
    }

    public ExternalDefinition GetExternalDefinition(BaseSharedParameterDefinition sharedParameterDefinition)
    {
        var guid = sharedParameterDefinition.Guid;

        if (sharedParameterDefinition is SharedParameterDefinition s && _definitionFileUtils.GetParameterName(guid) is null)
            _definitionFileUtils.AddSharedParameter(guid, s.Name, ParameterTypeConverter.GetParameterTypeName(s.ParameterType), s.Description, visible: s.IsVisible, userModifiable: s.IsUserModifiable);

        _document.Application.SharedParametersFilename = _temporarySharedParametersFile;
        var groupName = _definitionFileUtils.GetGroupName(guid) ?? throw new KeyNotFoundException($"{nameof(Guid)} [{guid}] not found in the file {_temporarySharedParametersFile}");
        var parameterName = _definitionFileUtils.GetParameterName(guid) ?? throw new KeyNotFoundException($"{nameof(Guid)} [{guid}] not found in the file {_temporarySharedParametersFile}");
        var definitionFile = _document.Application.OpenSharedParameterFile();
        var group = definitionFile.Groups.get_Item(groupName);
        return group.Definitions.get_Item(parameterName) as ExternalDefinition;
    }

    public List<ExternalDefinition> GetExternalDefinitions(List<BaseSharedParameterDefinition> sharedParameterDefinitions)
    {
        _document.Application.SharedParametersFilename = _temporarySharedParametersFile;

        var definitionFile = _document.Application.OpenSharedParameterFile();

        return sharedParameterDefinitions
            .Select(x =>
            {
                var groupName = _definitionFileUtils.GetGroupName(x.Guid) ?? throw new KeyNotFoundException($"{nameof(Guid)} [{x.Guid}] not found in the file {_temporarySharedParametersFile}");
                var parameterName = _definitionFileUtils.GetParameterName(x.Guid) ?? throw new KeyNotFoundException($"{nameof(Guid)} [{x.Guid}] not found in the file {_temporarySharedParametersFile}");
                var group = definitionFile.Groups.get_Item(groupName);
                return group.Definitions.get_Item(parameterName) as ExternalDefinition;
            })
            .Where(x => x != null).ToList();
    }

    public void Dispose()
    {
        if (_applicationSharedParametersFile != null &&
            _applicationSharedParametersFile != _document.Application.SharedParametersFilename)
        {
            _document.Application.SharedParametersFilename = _applicationSharedParametersFile;
        }

        _definitionFileUtils.Dispose();
    }
}