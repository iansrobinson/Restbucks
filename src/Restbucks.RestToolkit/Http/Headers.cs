using System;
using System.Collections;
using System.Collections.Generic;

namespace Restbucks.RestToolkit.Http
{
    public class Headers : IDictionary<string, IEnumerable<string>>
    {
        private readonly IDictionary<string, IEnumerable<string>> headers;

        public Headers(IDictionary<string, IEnumerable<string>> headers)
        {
            this.headers = headers;
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, IEnumerable<string>> item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(KeyValuePair<string, IEnumerable<string>> item)
        {
            return headers.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, IEnumerable<string>>[] array, int arrayIndex)
        {
            headers.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, IEnumerable<string>> item)
        {
            throw new InvalidOperationException();
        }

        public int Count
        {
            get { return headers.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool ContainsKey(string key)
        {
            return headers.ContainsKey(key);
        }

        public void Add(string key, IEnumerable<string> value)
        {
            throw new InvalidOperationException();
        }

        public bool Remove(string key)
        {
            throw new InvalidOperationException();
        }

        public bool TryGetValue(string key, out IEnumerable<string> value)
        {
            return headers.TryGetValue(key, out value);
        }

        public IEnumerable<string> this[string key]
        {
            get { return headers[key]; }
            set { throw new InvalidOperationException(); }
        }

        public ICollection<string> Keys
        {
            get { return headers.Keys; }
        }

        public ICollection<IEnumerable<string>> Values
        {
            get { return headers.Values; }
        }
    }
}