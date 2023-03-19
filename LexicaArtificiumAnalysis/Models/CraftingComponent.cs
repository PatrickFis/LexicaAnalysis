using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicaArtificiumAnalysis.Models
{
    public class CraftingComponent : IEquatable<CraftingComponent>
    {
        [Name("Creature Type")]
        public string CreatureType { get; set; }
        public string Component { get; set; }

        public bool Equals(CraftingComponent? other)
        {
            if (other == null)
            {
                return false;
            }

            return CreatureType.Equals(other.CreatureType, StringComparison.OrdinalIgnoreCase) && Component.Equals(other.Component, StringComparison.OrdinalIgnoreCase);
        }
    }
}
