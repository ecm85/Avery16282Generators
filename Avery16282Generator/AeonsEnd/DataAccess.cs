using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Avery16282Generator.AeonsEnd
{
    public static class DataAccess
    {
        public static IEnumerable<Divider> GetDividers()
        {
            return File.ReadAllLines(@"AeonsEnd\CardList")
                .Select(ConvertLineToDivider)
                .ToList();
        }

        private static Divider ConvertLineToDivider(string line)
        {
            var tokens = line.Split(',');
            return new Divider
            {
                Name = tokens[0],
                Cost = string.IsNullOrWhiteSpace(tokens[1]) ? (int?)null : int.Parse(tokens[1]),
                Expansion = tokens[2],
                Type = tokens[3]
            };
        }
    }
}
