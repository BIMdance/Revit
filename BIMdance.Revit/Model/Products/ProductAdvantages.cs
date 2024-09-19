namespace BIMdance.Revit.Model.Products;

public class ProductAdvantages
{
    public ProductAdvantages(string title)
    {
        Title = title;
        Descriptions = new List<string>();
    }

    public ProductAdvantages(string title, string commonDescription)
    {
        Title = title;
        CommonDescription = commonDescription;
        Descriptions = new List<string>();
    }

    public ProductAdvantages(string title, List<string> descriptions)
    {
        Title = title;
        Descriptions = descriptions;
    }

    public ProductAdvantages(string title, string commonDescription, List<string> descriptions)
    {
        Title = title;
        CommonDescription = commonDescription;
        Descriptions = descriptions;
    }

    public string Title { get; }
    public string CommonDescription { get; }
    public List<string> Descriptions { get; }
}