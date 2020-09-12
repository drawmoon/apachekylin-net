using System;
using System.Collections.Generic;
using System.Text;

namespace ApacheKylin.Client
{
    internal class KylinMetaColumn
    {
        public KylinMetaColumn(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }

        public Type Type { get; set; }
    }
}
