using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Utility;
internal class RotatingArray<T> : IEnumerable
{
    private T[] _array;
    private int _firstIndex;
    private long _version;
    public int Length { get; private set; }
    public RotatingArray(int length, T fill)
    {
        Length = length;
        _array = new T[length];
        _array = _array.Select((_) => fill).ToArray();
        _firstIndex = 0;
        _version = 0;
    }

    public void Add(T value)
    {
        _array[_firstIndex] = value;
        _firstIndex++;
        _firstIndex %= Length;
        _version++;
    }

    public IEnumerator GetEnumerator()
    {
        return new RotatingArrayEnumerator(this);
    }

    public T Oldest => _array[_firstIndex];
    public T Newest => _array[(_firstIndex - 1) % Length];

    public T this[int index]
    {
        get
        {
            index %= Length;
            index = Length - index - 1;
            index += _firstIndex;
            index %= Length;
            return _array[index];
        }
        set
        {
            index %= Length;
            index = Length - index - 1;
            index += _firstIndex;
            index %= Length;
            _version++;
            _array[index] = value;
        }
    }

    private class RotatingArrayEnumerator : IEnumerator, IDisposable
    {
        private RotatingArray<T> _array;
        private int _index;
        private long _version;
        public RotatingArrayEnumerator(RotatingArray<T> array)
        {
            _array = array;
            _index = 0;
            _version = array._version;
        }

        public object Current
        {
            get
            {
                if (_index < 0)
                {
                    throw new InvalidOperationException("Enumerator Ended");
                }
                return _array[_index];
            }
        }

        public bool MoveNext()
        {
            if (_version != _array._version)
            {
                throw new InvalidOperationException("Modification of an enumerating RotatingList is not supported");
            }
            _index++;
            if (_index > _array.Length)
            {
                return false;
            }
            return true;
        }

        public void Reset()
        {
            if (_version != _array._version)
            {
                throw new InvalidOperationException("Modification of an enumerating RotatingList is not supported");
            }
            _index = 0;
        }

        public void Dispose()
        {
            _index = -1;
        }
    }
}
