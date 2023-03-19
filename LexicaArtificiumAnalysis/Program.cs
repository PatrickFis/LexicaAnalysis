using CsvHelper;
using System.Globalization;
using System.Reflection;

namespace LexicaArtificiumAnalysis
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IDictionary<string, string> creatureTypeToCraftingSkillDictionary = new Dictionary<string, string>
            {
                { "Aberration", "Arcana" },
                { "Beast", "Survival" },
                { "Celestial", "Religion" },
                { "Construct", "Investigation" },
                { "Dragon", "Survival" },
                { "Elemental", "Arcana" },
                { "Fey", "Arcana" },
                { "Fiend", "Religion" },
                { "Giant", "Medicine" },
                { "Humanoid", "Medicine" },
                { "Monstrosity", "Survival" },
                { "Ooze", "Nature" },
                { "Plant", "Nature" },
                { "Undead", "Medicine" },
                { "Target creature", "Various" },
                { "Material", "Various" },
                { "Celestial (Evil), Humanoid (Neutral) or Fiend (Good) skin", "Various" }
            };

            // Store off crafting components
            using var creatureTypeReader = new StreamReader(@"Resources\CreatureTypesToComponentsList.csv");
            using var creatureTypeCsvReader = new CsvReader(creatureTypeReader, CultureInfo.InvariantCulture);
            List<CraftingComponent> craftingComponents = new();
            await foreach(var component in creatureTypeCsvReader.GetRecordsAsync<CraftingComponent>())
            {
                craftingComponents.Add(component);
            }

            // Store off all the items and the skills needed to craft them
            using var reader = new StreamReader(@"Resources\Communal Campaign Inventory Spreadsheet - Lexica Artificium.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            List<Item> items = new();
            await foreach (var item in csv.GetRecordsAsync<Item>())
            {
                item.CraftingSkill = creatureTypeToCraftingSkillDictionary[item.CreatureType];
                items.Add(item);
            }

            // Log all the creature types along with their components, crafting skills, and the amount of times they're used
            Console.WriteLine("Crafting components ordered by creature type and amount");
            var query = from item in items
                        group item by new { item.CreatureType, item.Component, item.CraftingSkill }
                        into grp
                        select new
                        {
                            CraftingComponent = new CraftingComponent()
                            {
                                CreatureType = grp.Key.CreatureType, 
                                Component = grp.Key.Component
                            },
                            grp.Key.CraftingSkill,
                            Amount = grp.Count()
                        }
                        into grp
                        orderby grp.CraftingComponent.CreatureType ascending, grp.Amount descending
                        select grp;

            foreach (var result in query)
            {
                Console.WriteLine($"{result.CraftingComponent.CreatureType}, {result.CraftingComponent.Component}, {result.CraftingSkill}, {result.Amount}");
            }
            Console.WriteLine();

            // Log the amount of times each crafting skill is used
            Console.WriteLine("Skills ordered by usage");
            var totalsBySkillQuery = from item in items
                                     group item by new { item.CraftingSkill }
                                     into grp
                                     orderby grp.Count() descending
                                     select new
                                     {
                                         grp.Key.CraftingSkill,
                                         Amount = grp.Count()
                                     };

            foreach (var result in totalsBySkillQuery)
            {
                Console.WriteLine($"The skill {result.CraftingSkill} is used {result.Amount} times");
            }
            Console.WriteLine();

            // Log the items that are used in recipes while not being listed as available
            List<CraftingComponent> usedComponents = query.ToList().Select(c => c.CraftingComponent).ToList();
            List<string> ignoredCreatureTypes = new() { "Target creature", "Material", "Celestial (Evil), Humanoid (Neutral) or Fiend (Good) skin" };
            Console.WriteLine("The following items are used in crafting recipes while not being listed as available items (special cases removed [target creature, material, and the various skins used for the Robe of the Archmagi])");
            foreach(var component in usedComponents)
            {
                if(craftingComponents.Contains(component) || ignoredCreatureTypes.Contains(component.CreatureType))
                {
                    continue;
                }
                Console.WriteLine($"{component.CreatureType} {component.Component}");
            }
            Console.WriteLine();

            // Log the crafting components that aren't used
            Console.WriteLine("The following items are not used in any crafting recipes");
            foreach (var component in usedComponents)
            {
                if (craftingComponents.Contains(component) || ignoredCreatureTypes.Contains(component.CreatureType))
                {
                    craftingComponents.Remove(component);
                    continue;
                }
            }
            craftingComponents.ForEach(c => Console.WriteLine($"{c.CreatureType} {c.Component}"));
            Console.WriteLine();
        }
    }
}