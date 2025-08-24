using System.Collections;
using System.Data;

namespace EdShel.DapperBatcher.Proxies;

internal class ProxyParametersCollection : IDataParameterCollection, IEnumerable<ProxyParameter>
{
    private readonly List<ProxyParameter> parameters = new();

    public object this[string parameterName]
    {
        get => parameters[0];
        set
        {
            if (parameters.Count > 0)
            {
                parameters[0] = EnsureParameterType(value);
            }
            else
            {
                parameters.Add(EnsureParameterType(value));
            }
        }
    }

    public object? this[int index]
    {
        get => parameters[index];
        set => parameters[index] = EnsureParameterType(value);
    }

    public int Count => parameters.Count;

    public bool IsFixedSize => false;

    public bool IsReadOnly => false;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public int Add(object? value)
    {
        parameters.Add(EnsureParameterType(value));
        return parameters.Count - 1;
    }

    public void Clear()
    {
        Console.WriteLine("Clear Parameters Collection DapperDbCommandProxy");
        // parameters.Clear();
    }

    public bool Contains(object? value)
    {
        return parameters.Contains(value);
    }

    public bool Contains(string parameterName)
    {
        return parameters.Exists(p => p.ParameterName == parameterName);
    }

    public void CopyTo(Array array, int index)
    {
        parameters.ToArray().CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
        return parameters.GetEnumerator();
    }

    public int IndexOf(object? value)
    {
        return parameters.IndexOf(EnsureParameterType(value));
    }

    public int IndexOf(string parameterName)
    {
        return parameters.FindIndex(p => p.ParameterName == parameterName);
    }

    public void Insert(int index, object? value)
    {
        parameters.Insert(index, EnsureParameterType(value));
    }

    public void Remove(object? value)
    {
        parameters.Remove(EnsureParameterType(value));
    }

    public void RemoveAt(int index)
    {
        parameters.RemoveAt(index);
    }

    public void RemoveAt(string parameterName)
    {
        int index = IndexOf(parameterName);
        if (index == -1)
        {
            return;
        }
        parameters.RemoveAt(index);
    }

    private static ProxyParameter EnsureParameterType(object? value)
    {
        if (value is not ProxyParameter param)
        {
            throw new ArgumentException($"Value must be of type {nameof(ProxyParameter)}.", nameof(value));
        }
        return param;
    }

    IEnumerator<ProxyParameter> IEnumerable<ProxyParameter>.GetEnumerator()
    {
        return parameters.GetEnumerator();
    }
}
