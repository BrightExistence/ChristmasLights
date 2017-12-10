using Pipliz.JSON;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using BrightExistence.SimpleTools;

namespace BrightExistence.ChristmasLights
{
    [ModLoader.ModManager]
    public static class Main
    {
        // GENERAL CLASS MEMBERS
        

        // TEXTURES
        static SimpleTexture ChristmasLeaves = new SimpleTexture("christmasleaves", Variables.NAMESPACE);

        // ITEMS
        static SimpleItem LeavesLit = new SimpleItem("leavesLit");

        // RECIPES
        static SimpleRecipe ChristmasLightsRecipe = new SimpleRecipe(LeavesLit, "pipliz.crafter");

        /// <summary>
        /// OnAssemblyLoaded callback entrypoint. Used for mod configuration / setup.
        /// </summary>
        /// <param name="path">The starting point of mod file structure.</param>
        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnAssemblyLoaded, Variables.NAMESPACE + ".OnAssemblyLoaded")]
		public static void OnAssemblyLoaded (string path)
		{
            // Announce ourselves.
            Pipliz.Log.Write("Mod {0} loading.", Variables.NAMESPACE);

            // Get a properly formatted version of our mod directory and subdirectories.
            Variables.ModGamedataDirectory = Path.GetDirectoryName(path).Replace("\\", "/");
            Variables.JobsPath = Path.Combine(Variables.ModGamedataDirectory, "jobs").Replace("\\", "/");
            Variables.IconPath = Path.Combine(Variables.ModGamedataDirectory, "icons").Replace("\\", "/");
            Variables.TexturePath = Path.Combine(Variables.ModGamedataDirectory, "Textures").Replace("\\", "/");
            Variables.MeshPath = Path.Combine(Variables.ModGamedataDirectory, "meshes").Replace("\\", "/");
            Variables.ResearchablesPath = Path.Combine(Variables.ModGamedataDirectory, "researchables").Replace("\\", "/");
        }

        /// <summary>
        /// AfterSelectedWorld callback entry point. Used for adding textures.
        /// </summary>
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterSelectedWorld, Variables.NAMESPACE + ".afterSelectedWorld"), ModLoader.ModCallbackProvidesFor("pipliz.server.registertexturemappingtextures")]
        public static void afterSelectedWorld()
        {
            // ---------------POPULATE TEXTURES HERE---------------
            ChristmasLeaves.AlbedoPath = Variables.TexturePath + "/albedo/leaveschristmas.png";
            ChristmasLeaves.EmissivePath = Variables.TexturePath + "/emissive/leaveschristmas.png";

            // ---------------AUTOMATED TEXTURE REGISTRATION---------------
            foreach (SimpleTexture thisTexture in Variables.Textures) thisTexture.registerTexture();
            Pipliz.Log.Write("{0}: Texture loading complete.", Variables.NAMESPACE);
        }

        /// <summary>
        /// The afterAddingBaseTypes entrypoint. Used for adding blocks.
        /// </summary>
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterAddingBaseTypes, Variables.NAMESPACE + ".afterAddingBaseTypes")]
        public static void afterAddingBaseTypes(Dictionary<string, ItemTypesServer.ItemTypeRaw> items)
        {
            Variables.itemsMaster = items;
            // ---------------POPULATE ITEM OBJECTS HERE---------------

            // Christmas leaves
            LeavesLit.Icon = getLocalIcon("leaveschristmas");
            LeavesLit.isSolid = true;
            LeavesLit.isDestructible = true;
            LeavesLit.isPlaceable = true;
            LeavesLit.sideAll = ChristmasLeaves.ID;
            //LeavesLit.lightSource = new SimpleItem.Light();
            //LeavesLit.lightSource.intensity = 1f;
            //LeavesLit.lightSource.range = 50;

            // ---------------(AUTOMATED BLOCK REGISTRATION)---------------
            foreach (SimpleItem Item in Variables.Items) Item.registerItem(items);
            Pipliz.Log.Write("{0}: Block and Item loading complete.", Variables.NAMESPACE);
        }

        /// <summary>
        /// The afterItemType callback entrypoint. Used for registering jobs and recipes.
        /// </summary>
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, Variables.NAMESPACE + ".AfterItemTypesDefined")]
        public static void AfterItemTypesDefined()
        {
            //---------------POPULATE RECIPES HERE---------------

            // Christmas Leaves
            ChristmasLightsRecipe.addRequirement("leavestemperate");
            ChristmasLightsRecipe.userCraftable = true;
            ChristmasLightsRecipe.defaultLimit = 15;
            ChristmasLightsRecipe.defaultPriority = -100;


            //---------------AUTOMATED RECIPE REGISTRATION---------------
            foreach (SimpleRecipe Rec in Variables.Recipes) Rec.addRecipeToLimitType();
            Pipliz.Log.Write("{0}: Recipe and Job loading complete.", Variables.NAMESPACE);

            //---------------AUTOMATED INVENTORY BLOCK REGISTRATION---------------
            foreach (SimpleItem Item in Variables.Items) Item.registerAsCrate();
            Pipliz.Log.Write("{0}: Crate registration complete.", Variables.NAMESPACE);
        }

        /// <summary>
        /// AfterDefiningNPCTypes callback. Used for registering jobs.
        /// </summary>
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, Variables.NAMESPACE + ".AfterDefiningNPCTypes")]
        [ModLoader.ModCallbackProvidesFor("pipliz.apiprovider.jobs.resolvetypes")]
        public static void AfterDefiningNPCTypes()
        {
            // ---------------REGISTER JOBS HERE---------------

            //Bowyer.registerJob<Jobs.BowyerJob>();

            Pipliz.Log.Write("{0}: Job loading complete.", Variables.NAMESPACE);
        }

        /// <summary>
        /// OnLoadingPlayer callback, called each time a player is loaded.
        /// </summary>
        /// <param name="n">Player's data as JSON.</param>
        /// <param name="p">Player object.</param>
        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnPlayerConnectedLate, Variables.NAMESPACE + ".OnPlayerConnectedLate")]
        public static void OnPlayerConnectedLate(Players.Player p)
        {
            
        }

        /// <summary>
        /// AfterWorldLoad callback entry point. Used for localization routines.
        /// </summary>
        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterWorldLoad, Variables.NAMESPACE + ".AfterWorldLoad")]
        [ModLoader.ModCallbackDependsOn("pipliz.server.localization.waitforloading")]
        [ModLoader.ModCallbackProvidesFor("pipliz.server.localization.convert")]
        public static void AfterWorldLoad ()
        {
            try
            {
                string[] array = new string[]
                {
                    "translation.json"
                };
                for (int i = 0; i < array.Length; i++)
                {
                    string text = array[i];
                    string[] files = Directory.GetFiles(Path.Combine(Variables.ModGamedataDirectory,"localization"), text, SearchOption.AllDirectories);
                    string[] array2 = files;
                    for (int j = 0; j < array2.Length; j++)
                    {
                        string text2 = array2[j];
                        try
                        {
                            JSONNode jsonFromMod;
                            if (JSON.Deserialize(text2, out jsonFromMod, false))
                            {
                                string name = Directory.GetParent(text2).Name;

                                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
                                {
                                    Pipliz.Log.Write("{0}: Found mod localization file for '{1}' localization", Variables.NAMESPACE, name);
                                    Localize(name, text, jsonFromMod);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Pipliz.Log.Write("{0}: Exception reading localization from {1}; {2}", Variables.NAMESPACE, text2, ex.Message);
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                Pipliz.Log.Write("{0}: Localization directory not found at {1}", Variables.NAMESPACE, Path.Combine(Variables.ModGamedataDirectory,"localization"));
            }
        }


        public static void Localize(string locName, string locFilename, JSONNode jsonFromMod)
        {
            try
            {
                if (Server.Localization.Localization.LoadedTranslation == null)
                {
                    Pipliz.Log.Write("{0} :Unable to localize. Server.Localization.Localization.LoadedTranslation is null.", Variables.NAMESPACE);
                }
                else
                {
                    if (Server.Localization.Localization.LoadedTranslation.TryGetValue(locName, out JSONNode jsn))
                    {
                        if (jsn != null)
                        {
                            foreach (KeyValuePair<string, JSONNode> modNode in jsonFromMod.LoopObject())
                            {
                                Pipliz.Log.Write("{0} : Adding localization for '{1}' from '{2}'.", Variables.NAMESPACE, modNode.Key, Path.Combine(locName, locFilename));
                                AddRecursive(jsn, modNode);
                            }
                        }
                        else
                            Pipliz.Log.Write("{0}: Unable to localize. Localization '{01 not found and is null.", Variables.NAMESPACE, locName);
                    }
                    else
                        Pipliz.Log.Write("{0}: Localization '{1}' not supported", Variables.NAMESPACE, locName);
                }

                Pipliz.Log.Write("{0}: Patched mod localization file '{1}/{2}'", Variables.NAMESPACE, locName, locFilename);

            }
            catch (Exception ex)
            {
                Pipliz.Log.WriteError(ex.ToString(), "{0}: Exception while localizing {1}", Variables.NAMESPACE, Path.Combine(locName, locFilename));
            }
        }

        private static void AddRecursive(JSONNode gameJson, KeyValuePair<string, JSONNode> modNode)
        {
            int childCount = 0;

            try
            {
                childCount = modNode.Value.ChildCount;
            }
            catch { }

            if (childCount != 0)
            {
                if (gameJson.HasChild(modNode.Key))
                {
                    foreach (var child in modNode.Value.LoopObject())
                        AddRecursive(gameJson[modNode.Key], child);
                }
                else
                {
                    gameJson[modNode.Key] = modNode.Value;
                }
            }
            else
            {
                gameJson[modNode.Key] = modNode.Value;
            }
        }

        /// <summary>
        /// Converts the name of an item to its in-game ID by prefixing NAMESPACE.
        /// </summary>
        /// <param name="itemName">Name of item.</param>
        /// <returns>Game ID of item. (NAMESPACE + Name)</returns>
        public static string getLocalID (string itemName)
        {
            return Variables.NAMESPACE + "." + itemName;
        }

        /// <summary>
        /// Converts the name of an icon to a full path.
        /// </summary>
        /// <param name="iconName">Name of the icon. (NOT filename: no extension.)</param>
        /// <returns>Full path of icon file with extension.</returns>
        public static string getLocalIcon (string iconName)
        {
            return Path.Combine(Variables.IconPath, iconName + ".png");
        }
    }

    public static class MultiPath
    {
        public static string Combine(params string[] pathParts)
        {
            StringBuilder result = new StringBuilder();
            foreach (string part in pathParts)
            {
                result.Append(part.TrimEnd('/', '\\')).Append(Path.DirectorySeparatorChar);
            }
            return result.ToString().TrimEnd(Path.DirectorySeparatorChar);
        }
    }
}
