using System;
using System.Collections;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет строго типизированный список объектов, доступных по индексу с возможностью возвращать объект по ссылке.
    /// </summary>
    /// <typeparam name="T">Тип элементов в списке.</typeparam>
    public class ValueList<T> : IList<T> where T : struct
    {
        private const int DEFAULT_CAPACITY = 4;
        private T[] items;

        /// <summary>
        /// Возвращает или задает элемент по указанному индексу.
        /// </summary>
        /// <param name="index">Индекс элемента.</param>
        public T this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        public int Count { get; private set; }

        public int Capacity => items.GetLength(0);

        public bool IsReadOnly => false;

        public ValueList()
        {
            items = new T[DEFAULT_CAPACITY];
        }

        public ref T GetByRef(int index)
        {
            return ref items[index];
        }

        public void Add(T item)
        {
            if (Count == Capacity)
            {
                EnsureCapacity();
            }

            items[Count] = item;
            Count++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Count - 1; i++)
            {
                items[i] = items[i + 1];
            }

            items[Count - 1] = default(T);
            Count--;
        }

        private void EnsureCapacity()
        {
            T[] newArray = new T[Capacity * 2];

            for (int i = 0; i < Count; i++)
            {
                newArray[i] = items[i];
            }

            items = newArray;
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (items as IList<T>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
