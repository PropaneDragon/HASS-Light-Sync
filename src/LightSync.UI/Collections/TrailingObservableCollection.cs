using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LightSync.UI.Collections
{
    internal class TrailingObservableCollection<T> : IList, INotifyCollectionChanged
    {
        private readonly ObservableCollection<T> _source;
        private readonly object _trailingObject;

        public object? this[int index]
        {
            get => index < _source.Count ? _source[index] : _trailingObject;
            set => throw new NotSupportedException();
        }

        public bool IsFixedSize => false;
        public bool IsReadOnly => true;
        public bool IsSynchronized => false;

        public int Count => _source.Count + 1;

        public object SyncRoot => this;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public TrailingObservableCollection(ObservableCollection<T> source, object trailingObject)
        {
            _source = source;
            _trailingObject = trailingObject;

            _source.CollectionChanged += (_, e) => CollectionChanged?.Invoke(this, e);
        }

        public void Clear() => throw new NotSupportedException();
        public void Insert(int index, object? value) => throw new NotSupportedException();
        public void Remove(object? value) => throw new NotSupportedException();
        public void RemoveAt(int index) => throw new NotSupportedException();

        public void CopyTo(Array array, int index)
        {
            foreach (var item in this)
                array.SetValue(item, index++);
        }

        public bool Contains(object? value) => IndexOf(value) >= 0;

        public int Add(object? value) => throw new NotSupportedException();
        public int IndexOf(object? value) => value == _trailingObject ? _source.Count : _source.IndexOf((T)value!);

        public IEnumerator GetEnumerator()
        {
            foreach (var item in _source) yield return item!;
            yield return _trailingObject;
        }
    }
}
