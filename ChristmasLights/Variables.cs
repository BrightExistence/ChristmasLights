using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrightExistence.SimpleTools
{
    public class Variables
    {
        public const string NAMESPACE = "BrightExistence.ChristmasLights";

        public static string ModGamedataDirectory;
        public static string JobsPath;
        public static string IconPath;
        public static string TexturePath;
        public static string MeshPath;
        public static string ResearchablesPath;
        public static Dictionary<string, ItemTypesServer.ItemTypeRaw> itemsMaster;
        protected const int simpleToolsMajor = 0;
        protected const int simpleToolsMinor = 0;
        protected const int simpleToolsBuild = 3;
        public static string toolkitVersion
        {
            get
            {
                return Convert.ToString(simpleToolsMajor) + "." + Convert.ToString(simpleToolsMinor) + "." + Convert.ToString(simpleToolsBuild);
            }
        }

        // AUTO-REGISTERED TEXTURES
        public static List<SimpleTexture> Textures = new List<SimpleTexture>();

        // AUTO-REGISTERED ITEMS
        public static List<SimpleItem> Items = new List<SimpleItem>();

        // AUTO-REGISTERED RECIPES
        public static List<SimpleRecipe> Recipes = new List<SimpleRecipe>();

        // AUTO-REGISTERED RESEARCHABLES
        public static List<SimpleResearchable> Researchables = new List<SimpleResearchable>();
    }
}
