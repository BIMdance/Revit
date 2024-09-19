namespace BIMdance.Revit.Model.Electrical;

public interface IBus : IEnumerable
{

}

public class Bus<TEquipment> : ElementProxy, IBus, IList<TEquipment>
    where TEquipment : ElectricalEquipmentProxy
{
    private readonly List<TEquipment> _equipments;

    public Bus(TEquipment input)
    {
        Input = input;
        Input.Bus = this;
        _equipments = new List<TEquipment> {Input};
    }

    public TEquipment Input { get; set; }
    public int Count => _equipments.Count;
    public bool IsReadOnly => false;

    IEnumerator<TEquipment> IEnumerable<TEquipment>.GetEnumerator() => _equipments.GetEnumerator();
    public IEnumerator GetEnumerator() => _equipments.GetEnumerator();

    public TEquipment this[int index]
    {
        get => _equipments[index];
        set => _equipments[index] = value;
    }

    public void Add(TEquipment item)
    {
        if (item == null)
            throw new NullReferenceException(nameof(item));

        _equipments.Add(item);
        item.Bus = this;
    }

    public void Clear() => _equipments.Clear();
    public bool Contains(TEquipment item) => _equipments.Contains(item);
    public void CopyTo(TEquipment[] array, int arrayIndex) => _equipments.CopyTo(array,arrayIndex);
    public int IndexOf(TEquipment item) => _equipments.IndexOf(item);
    public int InputIndex() => _equipments.IndexOf(Input);
    public void Insert(int index, TEquipment item)
    {
        if (item == null)
            throw new NullReferenceException(nameof(item));

        _equipments.Insert(index, item);
        item.Bus = this;
    }
    public bool Remove(TEquipment item)
    {
        if (item == null)
            throw new NullReferenceException(nameof(item));

        item.Bus = null;
        return _equipments.Remove(item);
    }

    public void RemoveAt(int index)
    {
        var item = this[index];
        item.Bus = null;
        _equipments.RemoveAt(index);
    }
}