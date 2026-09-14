using System.Collections.ObjectModel;

namespace ModbusExtendedTools.Extensions.Collections;

public static class CollectionExtensions
{
    public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> items)
    {
        return new ObservableCollection<T>(items);
    }
    
    public static IEnumerable<T> ToEnumerable<T>(this ObservableCollection<T> collection)
    {
        return collection;
    }

    public static void ReplaceWith<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        collection.Clear();
        foreach (var item in items)
            collection.Add(item);
    }
}