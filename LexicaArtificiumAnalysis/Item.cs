using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicaArtificiumAnalysis
{
    public class Item
    {
        public string Name { get; set; }

        public string Type { get; set; }

        [Name("Suggested Off the shelf price (gp)")]
        public string Price { get; set; }
        
        public string Rarity { get; set; }
        
        public string Attunement { get; set; }

        [Name("Creature Type")]
        public string CreatureType { get; set; }
        
        public string Subtype { get; set; }
        
        public string Component { get; set; }

        [Ignore]
        public string CraftingSkill { get; set; }
    }
}
