﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Avery16282Generator.AeonsEnd
{
    public static class DataAccess
    {
        public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static IEnumerable<Divider> GetDividers()
        {
            //Gotten from https://www.actionphasegames.com/pages/aerandomizer
            return File.ReadAllLines(Path.Combine(CurrentPath, "AeonsEnd", "CardList.txt"))
                .Select(ConvertLineToDivider)
                .ToList();
        }

        private static Divider ConvertLineToDivider(string line)
        {
            var tokens = line.Split(',');
            var allExpansionsByFriendlyText = Enum.GetValues(typeof(Expansion))
                .Cast<Expansion>()
                .ToDictionary(expansion => expansion.GetFriendlyName(), expansion => expansion);
            return new Divider
            {
                Name = tokens[0],
                Type = tokens[1],
                Cost = string.IsNullOrWhiteSpace(tokens[2]) ? (int?)null : int.Parse(tokens[2]),
                Expansion = allExpansionsByFriendlyText[tokens[3]]
            };
        }
    }
}
