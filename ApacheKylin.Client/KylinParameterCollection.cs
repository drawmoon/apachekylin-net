using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace ApacheKylin.Client
{
    public class KylinParameterCollection : DbParameterCollection
    {
        private readonly List<KylinParameter> _parameters = new List<KylinParameter>();
        
        public override int Add(object value)
        {
            var kylinParameter = (KylinParameter) value;
            _parameters.Add(kylinParameter);
            return _parameters.IndexOf(kylinParameter);
        }

        public override void Clear() => _parameters.Clear();

        public override bool Contains(object value) => IndexOf(value) != -1;

        public override int IndexOf(object value) => _parameters.IndexOf((KylinParameter) value);

        public override void Insert(int index, object value) => _parameters.Insert(index, (KylinParameter) value);

        public override void Remove(object value) => _parameters.Remove((KylinParameter) value);

        public override void RemoveAt(int index) => _parameters.RemoveAt(index);

        public override void RemoveAt(string parameterName) => _parameters.RemoveAt(IndexOf(parameterName));

        protected override void SetParameter(int index, DbParameter value) => _parameters[index] = (KylinParameter) value;

        protected override void SetParameter(string parameterName, DbParameter value) =>
            _parameters[IndexOf(parameterName)] = (KylinParameter) value;

        public override int Count => _parameters.Count;
        public override object? SyncRoot { get; }

        public override int IndexOf(string parameterName)
        {
            var kylinParameter = _parameters.FirstOrDefault(p => p.ParameterName == parameterName);

            if (kylinParameter == null)
            {
                return -1;
            }

            return IndexOf(kylinParameter);
        }

        public override bool Contains(string value) => IndexOf(value) != -1;

        public override void CopyTo(Array array, int index) =>
            ((ICollection) _parameters).CopyTo(array ?? throw new ArgumentNullException(nameof(array)), index);

        public override IEnumerator GetEnumerator() => _parameters.GetEnumerator();

        protected override DbParameter GetParameter(int index) => _parameters[index];

        protected override DbParameter GetParameter(string parameterName) => _parameters.FirstOrDefault(p => p.ParameterName == parameterName);

        public override void AddRange(Array values) => _parameters.AddRange(values.Cast<KylinParameter>());
    }
}