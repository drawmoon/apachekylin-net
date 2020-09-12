using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ApacheKylin.Client
{
    public class KylinConnectionSetting
    {
        private static readonly Dictionary<string, PropertyInfo> Properties;

        static KylinConnectionSetting()
        {
            Properties = typeof(KylinConnectionSetting).GetProperties().ToDictionary(p => p.Name.ToLower(), p => p);
        }

        private KylinConnectionSetting()
        {
        }

        public KylinConnectionSetting(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var regex = new Regex(@"(?<ful>(?<prcl>http|https):\/\/(?<host>localhost|\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3})):(?<port>\d+)\/?(?<proj>[^,|;]+)?(?<outh>,.*)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var regex2 = new Regex(@",?(?<prop>[^,|;]+)=(?<val>[^,|;]+)", RegexOptions.Compiled);

            if (regex.IsMatch(connectionString))
            {
                var match = regex.Match(connectionString);

                var host = match.Groups["ful"].Value;
                SetValue(nameof(host), host);

                var port = match.Groups["port"].Value;
                SetValue(nameof(port), port);

                var project = match.Groups["proj"].Value;
                SetValue(nameof(project), project);

                var outh = match.Groups["outh"].Value;

                if (regex2.IsMatch(outh))
                {
                    var matches = regex2.Matches(outh);

                    foreach (var (prop, val) in from Match m in matches
                                                let prop = m.Groups["prop"].Value
                                                let val = m.Groups["val"].Value
                                                select (prop, val))
                    {
                        SetValue(prop, val);
                    }
                }
            }
        }

#nullable disable
        public string Host { get; set; }
            
        public ushort Port { get; set; }
            
        public string User { get; set; }
            
        public string Password { get; set; }
            
        public string Project { get; set; }
#nullable enable

        private void SetValue(string name, object? value)
        {
            var property = Properties[name.ToLower()];
            property.SetMethod?.Invoke(this, new[] { Convert.ChangeType(value, property.PropertyType) });
        }

        public override string ToString() => $"{Host}:{Port}/{Project},user={User},password={Password}";
    }
}