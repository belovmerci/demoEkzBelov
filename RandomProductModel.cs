using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoEkzBelov
{
    public static class RandomProductModel
    {
        private static readonly Random _random = new Random();

        private static readonly List<string> _adjectives = new List<string>
        {
            "Golden", "Iron", "Wooden", "Legendary", "Rare", "Common", "Dysfunctional"
        };

        private static readonly List<string> _names = new List<string>
        {
            "Pan", "Table", "Toaster", "Blender", "Pot"
        };

        private static readonly List<string> _descriptions = new List<string>
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
            "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris.",
            "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore.",
            "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia."
        };

        private static readonly List<string> _manufacturers = new List<string>
        {
            "X", "Y", "Z"
        };

        private static readonly List<string> _picturePaths = new List<string>
        {
            // "path1.jpg", "path2.jpg", "path3.jpg", "path4.jpg", "path5.jpg"
                "https://cataas.com/cat",
                "https://placekitten.com/200/300",
                "https://www.randomdoggiegenerator.com/randomdoggie?type=cat",
                "https://www.randomkittengenerator.com/randomkitten",
                "https://www.placecage.com/c/200/300"
        };

        public static ProductModel GenerateRandomProduct()
        {
            return new ProductModel
            {
                Name = (GetRandomItem(_adjectives) + ' ' + GetRandomItem(_names)),
                Description = GetRandomItem(_descriptions),
                Manufacturer = GetRandomItem(_manufacturers),
                Price = (decimal)_random.NextDouble() * 100,
                Available = _random.Next(2) == 0,
                PicturePath = GetRandomItem(_picturePaths)
            };
        }

        private static string GetRandomItem(List<string> list)
        {
            return list[_random.Next(list.Count)];
        }
    }
}
