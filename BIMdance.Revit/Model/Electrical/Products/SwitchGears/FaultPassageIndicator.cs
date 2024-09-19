namespace BIMdance.Revit.Model.Electrical.Products.SwitchGears;

public class FaultPassageIndicator : Manufactured, ICompatibleComponents<CurrentTransformer>, IPrototype<FaultPassageIndicator>
{
    public FaultPassageIndicator(Guid guid) : base(guid)
    {
        CompatibleComponents = new UniqueCollection<CurrentTransformer>();
        CurrentTransformers = new CurrentTransformer[3];
    }
        
    public CurrentTransformer[] CurrentTransformers { get; }

    public UniqueCollection<CurrentTransformer> CompatibleComponents { get; set; }

    public bool IsCompatible(CurrentTransformer currentTransformer) =>
        CompatibleComponents.Contains(currentTransformer);

    public CurrentTransformer GetDefault(Type type = null) =>
        type == typeof(CurrentTransformerZeroSequence)
            ? CompatibleComponents.OfType<CurrentTransformerZeroSequence>().FirstOrDefault() 
            : CompatibleComponents.FirstOrDefault();

    public void SetupA()
    {
        CurrentTransformers[0] = GetDefault(typeof(CurrentTransformer));
        CurrentTransformers[1] = GetDefault(typeof(CurrentTransformer));
        CurrentTransformers[2] = GetDefault(typeof(CurrentTransformer));
    }
    public void SetupB()
    {
        CurrentTransformers[0] = GetDefault(typeof(CurrentTransformer));
        CurrentTransformers[1] = GetDefault(typeof(CurrentTransformer));
        CurrentTransformers[2] = GetDefault(typeof(CurrentTransformerZeroSequence));
    }

    public FaultPassageIndicator Clone()
    {
        var clone =  new FaultPassageIndicator(this.Product.Guid)
        {
            Product = this.Product,
            // SwitchGearSeries = this.SwitchGearSeries,
            CompatibleComponents = this.CompatibleComponents,
            CurrentTransformers =
            {
                [0] = CurrentTransformers[0],
                [1] = CurrentTransformers[1],
                [2] = CurrentTransformers[2]
            }
        };

        return clone;
    }
}