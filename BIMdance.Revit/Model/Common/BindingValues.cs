namespace BIMdance.Revit.Model.Common;

public class BindingValues<T1, T2>
{
    private readonly SortedDictionary<T1, HashSet<T2>> _dictionary1;
    private readonly SortedDictionary<T2, HashSet<T1>> _dictionary2;

    public BindingValues()
    {
        _dictionary1 = new SortedDictionary<T1, HashSet<T2>>();
        _dictionary2 = new SortedDictionary<T2, HashSet<T1>>();
    }

    public BindingValues(IEnumerable<(T1, T2)> values) : this()
    {
        foreach (var (item1, item2) in values)
            Add(item1, item2);
    }

    public BindingValues(IEnumerable<(T2, T1)> values) : this()
    {
        foreach (var (item1, item2) in values)
            Add(item1, item2);
    }

    public BindingValues(T1 value1, IEnumerable<T2> values2) : this()
    {
        foreach (var value2 in values2)
            Add(value1, value2);
    }

    public BindingValues(IEnumerable<(T1, List<T2>)> values) : this()
    {
        foreach (var (value1, values2) in values)
        foreach (var value2 in values2)
        {
            Add(value1, value2);
        }
    }

    public BindingValues(IEnumerable<(T2, List<T1>)> values) : this()
    {
        foreach (var (value1, values2) in values)
        foreach (var value2 in values2)
        {
            Add(value1, value2);
        }
    }

    public BindingValues(T2 value1, IEnumerable<T1> values2) : this()
    {
        foreach (var value2 in values2)
            Add(value1, value2);
    }

    public BindingValues(IEnumerable<T1> values1, IEnumerable<T2> values2) : this()
    {
        var values2List = values2.ToList();

        foreach (var value1 in values1)
        foreach (var value2 in values2List)
            Add(value1, value2);
    }

    public BindingValues(IEnumerable<T2> values1, IEnumerable<T1> values2) :
        this(values2, values1)
    { }

    public void Add(T1 value1, T2 value2)
    {
        AddToDictionary(_dictionary1, value1, value2);
        AddToDictionary(_dictionary2, value2, value1);
    }

    public void Add(T2 value1, T1 value2)
    {
        AddToDictionary(_dictionary2, value1, value2);
        AddToDictionary(_dictionary1, value2, value1);
    }

    public bool Contains(T1 value) => _dictionary1.ContainsKey(value);
    public bool Contains(T2 value) => _dictionary2.ContainsKey(value);

    public void Merge(BindingValues<T1, T2> bindingValue)
    {
        foreach (var key in bindingValue._dictionary1.Keys)
        {
            var values = bindingValue._dictionary1[key];

            foreach (var value in values)
            {
                AddToDictionary(_dictionary1, key, value);
                AddToDictionary(_dictionary2, value, key);
            }
        }
    }

    public void Merge(IEnumerable<BindingValues<T1, T2>> bindingValues)
    {
        foreach (var bindingValue in bindingValues)
            Merge(bindingValue);
    }

    private static void AddToDictionary<TT1, TT2>(IDictionary<TT1, HashSet<TT2>> dictionary, TT1 value1, TT2 value2)
    {
        if (dictionary.TryGetValue(value1, out var values2))
            values2.Add(value2);
        else
            dictionary.Add(value1, new HashSet<TT2> { value2 });
    }

    public IEnumerable<T1> GetAllValues1() => _dictionary1.Keys;
    public IEnumerable<T2> GetAllValues2() => _dictionary2.Keys;
    public IEnumerable<T1> GetValues1(T2 value2) => _dictionary2.TryGetValue(value2, out var value1) ? value1 : default;
    public IEnumerable<T2> GetValues2(T1 value1) => _dictionary1.TryGetValue(value1, out var value2) ? value2 : default;
    public IEnumerable<T1> GetValues1(IEnumerable<T2> values2) =>
        _dictionary2.Where(n => values2.Contains(n.Key)).SelectMany(n => n.Value).Distinct();
    public IEnumerable<T2> GetValues2(IEnumerable<T1> values1) =>
        _dictionary1.Where(n => values1.Contains(n.Key)).SelectMany(n => n.Value).Distinct();

    public bool Any() => _dictionary1.Any();
}