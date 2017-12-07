using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrightExistence
{
    static class Variables
    {
        public static string ModGamedataDirectory;
        public static string JobsPath;
        public static string IconPath;
        public static string TexturePath;
        public static string MeshPath;
        public static string ResearchablesPath;
        public const string NAMESPACE = "BrightExistence.ChristmasLights";
        public static Dictionary<string, ItemTypesServer.ItemTypeRaw> itemsMaster;

        // AUTO-REGISTERED TEXTURES
        public static List<SimpleTexture> Textures = new List<SimpleTexture>();

        // AUTO-REGISTERED ITEMS
        public static List<SimpleItem> Items = new List<SimpleItem>();

        // AUTO-REGISTERED RECIPES
        public static List<SimpleRecipe> Recipes = new List<SimpleRecipe>();
    }
}
