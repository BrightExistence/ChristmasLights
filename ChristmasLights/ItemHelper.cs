using System;
using System.Collections.Generic;
using Pipliz.APIProvider.Jobs;
using Pipliz.JSON;

namespace BrightExistence.SimpleTools
{
    public static class ItemHelper
    {
        /// <summary>
        /// Attempts to remove an item from the server's database.
        /// </summary>
        /// <param name="itemName">string: Item's Key.</param>
        /// <returns>True if item was removed. False if it was not for any reason.</returns>
        public static bool tryRemoveItem (string itemName)
        {
            if (itemName == null || itemName.Length < 1)
            {
                Pipliz.Log.WriteError("{0}: tryRemoveItem has been called but was not given a valid item identifier.", Variables.NAMESPACE);
                return false;
            }
            else
            {
                if (Variables.itemsMaster == null)
                {
                    Pipliz.Log.WriteError("{0}: tryRemoveItem was called on {1} before Items master dictionary has been obtained. Cannot complete action.", Variables.NAMESPACE, itemName);
                    return false;
                }
                else
                {
                    if (!Variables.itemsMaster.ContainsKey(itemName))
                    {
                        Pipliz.Log.WriteError("{0}: tryRemoveItem was called on key {1} that was not found.", Variables.NAMESPACE, itemName);
                        return false;
                    }
                    else
                    {
                        Pipliz.Log.Write("{0}: Item key {1} found, attempting removal", Variables.NAMESPACE, itemName);
                        Variables.itemsMaster.Remove(itemName);

                        if (!Variables.itemsMaster.ContainsKey(itemName))
                        {
                            Pipliz.Log.Write("{0}: Item {1} successfully removed.", Variables.NAMESPACE, itemName);
                            return true;
                        }
                        else
                        {
                            Pipliz.Log.Write("{0}: Item {1} removal was not successful for an unknown reason.", Variables.NAMESPACE, itemName);
                            return false;
                        }
                    }
                }
            }
        }
    }

    public class ItemShell
    {
        public string strItemkey = "[unknown]";
        public int intAmount;

        public ItemShell(string Key, int amount = 1)
        {
            if (Key != null) strItemkey = Key;
            intAmount = amount;
        }
    }

    public class SimpleItem
    {
        /// <summary>
        /// Stores the mod's namespace.
        /// </summary>
        public string NAMESPACE { get; protected set; }

        /// <summary>
        /// Name of Item, excluding prefix. Ex: myItem instead of myHandle.myMod.myItem
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path to icon .png file Ex: gamedata/textures/icons/vanillaIconName.png or getLocalIcon("myIconFile.png")
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Can the item be placed in the world; is it a block?
        /// </summary>
        public bool? isPlaceable { get; set; }

        /// <summary>
        /// When a player attempts to remove the block, is it actually removed?
        /// </summary>
        public bool? isDestructible { get; set; }

        /// <summary>
        /// Is it a solid block, or can players and NPCs walk through it?
        /// </summary>
        public bool? isSolid { get; set; }

        /// <summary>
        /// The name of the texture that will be displayed on all sides of the block unless otherwise specified.
        /// </summary>
        public string sideAll { get; set; }

        /// <summary>
        /// The name of the texture that will be displayed on the top (y+) side of the block only.
        /// </summary>
        public string sideTop { get; set; }

        /// <summary>
        /// The name of the texture that will be displayed on the bottom (y-) side of the block only.
        /// </summary>
        public string sideBottom { get; set; }

        /// <summary>
        /// The name of the texture which will be displayed on the front (z+) side of the block only.
        /// </summary>
        public string sideFront { get; set; }

        /// <summary>
        /// The name of the texture which will be displayed on the back (z-) side of the block only.
        /// </summary>
        public string sideBack { get; set; }

        /// <summary>
        /// The name of the texture which will be displayed on the left (x-) side of the block only.
        /// </summary>
        public string sideLeft { get; set; }

        /// <summary>
        /// The name of the texture which will be displayed on the right (x+) side of the block only.
        /// </summary>
        public string sideRight { get; set; }

        /// <summary>
        /// The location of a file that is the mesh for this item. If omitted, the item will be a perfect cube.
        /// </summary>
        public string mesh { get; set; }

        /// <summary>
        /// The amount of time the user must hold down the left mouse button to remove a block of this item.
        /// </summary>
        public int? destructionTime = 500;

        /// <summary>
        /// The name of an audio asset which will be played when a block of this item is placed.
        /// </summary>
        public string onPlaceAudio { get; set; }

        /// <summary>
        /// The name of an audio asset which will be played when a block of this item is removed.
        /// </summary>
        public string onRemoveAudio { get; set; }

        /// <summary>
        /// If true, the registerAsCrate method will register this item as a crate (a type of tracked block) when called during the proper callback.
        /// </summary>
        public bool isCrate = false;

        /// <summary>
        /// A list of DropItem objects describing what types are added to inventory when a block of this type is removed, and by what chance.
        /// </summary>
        public List<DropItem> Drops = new List<DropItem>();

        /// <summary>
        /// Will things grow on it?
        /// </summary>
        public bool? isFertile = false;

        /// <summary>
        /// Can it be mined by NPCs
        /// </summary>
        public bool? minerIsMineable = false;

        /// <summary>
        /// How quickly do NPCs mine it, if they're allowed to?
        /// </summary>
        public int minerMiningTime = 2;

        /// <summary>
        /// Replaces an item of this key in the server's item database.
        /// </summary>
        public string maskItem;

        /// <summary>
        /// Used to make the block glow by using a SimpleItem.Light object.
        /// </summary>
        public SimpleItem.Light lightSource;

        /// <summary>
        /// True to overwrite an existing item by the same ID if it is found, is ignored if masking is used.
        /// </summary>
        public bool overwrite = false;

        /// <summary>
        /// Set to false if you do NOT want this item, and any SimpleRecipe/SimpleResearch objects which know about and depend upon it, registered.
        /// </summary>
        public bool enabled = true;

        /// <summary>
        /// The ID, or name of this item as it will be stored in the server database.
        /// </summary>
        public string ID
        {
            get
            {
                if (maskItem == null)
                {
                    if (NAMESPACE == null || NAMESPACE.Length < 1) return Name;
                    else return NAMESPACE + "." + Name;
                }
                else return maskItem;
            }
        }

        protected ItemTypesServer.ItemTypeRaw itrThisItem;
        protected ItemTypesServer.ItemTypeRaw thisItemRaw
        {
            get
            {
                this.itrThisItem = new ItemTypesServer.ItemTypeRaw(this.ID, itemAsJSON());
                return itrThisItem; 
            }
            set
            {
                itrThisItem = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strNamespace">Namespace of mod. Ex: DeveloperHandle.ModName Will be used as a prefix to generate item IDs.</param>
        /// <param name="strName">Name of item excluding any prefixes. Ex: MyItem NOT DeveloperHandle.ModName.MyItem</param>
        public SimpleItem(string strName, bool dropsSelf = true, string strNAMESPACE = Variables.NAMESPACE)
        {
            NAMESPACE = strNAMESPACE == null ? "" : strNAMESPACE;
            Name = (strName == null || strName.Length < 1) ? "NewItem" : strName;
            if (dropsSelf) Drops.Add(new DropItem(this.ID));
            Pipliz.Log.Write("{0}: Initialized Item {1} (it is not yet registered.)", Variables.NAMESPACE, this.Name);
            try
            {
                if (!Variables.Items.Contains(this)) Variables.Items.Add(this);
            }
            catch (Exception)
            {
                Pipliz.Log.Write("{0} : WARNING : Item {1} could not be automatically added to auto-load list. Make sure you explicityly added it.", Variables.NAMESPACE, this.Name);
            }
        }

        /// <summary>
        /// Registers this item in the server's database of items.Should be called during the afterAddingBaseTypes callback.
        /// </summary>
        /// <param name="items">The server's item database (a Dictionary object). Will be passed to the afterAddingBaseTypes callback method.</param>
        public void registerItem(Dictionary<string, ItemTypesServer.ItemTypeRaw> items)
        {
            if (enabled)
            {
                if (maskItem != null)
                {
                    Pipliz.Log.Write("{0}: Attempting to mask item {1} with {2}.", Variables.NAMESPACE, this.ID, this.Name);
                    // Masking is being used, see if there is an existing item to mask.
                    if (items.ContainsKey(this.ID))
                    {
                        // It exists, merge values.
                        ItemTypesServer.ItemTypeRaw originalItem;
                        if (items.TryGetValue(this.ID, out originalItem))
                        {
                            // Successfully retrieved item, overwrite its properties which are explicity specified in this object.
                            itemAsJSON(originalItem.description);
                            Pipliz.Log.Write("{0}: Masking complete.", Variables.NAMESPACE);
                        }
                        else
                        {
                            // Item exists, but we can't retrieve it.
                            Pipliz.Log.Write("{0}: Masking failed, item {1} exists but we could not retrieve it. Overwriting instead.", Variables.NAMESPACE, this.ID);
                            // Remove existing item.
                            ItemHelper.tryRemoveItem(this.ID);
                            // Add this item.
                            items.Add(this.ID, thisItemRaw);
                        }

                    }
                    else
                    {
                        // Item did not already exist, let's add it.
                        Pipliz.Log.Write("{0}: Masking was enabled, but masked item was not found. Adding {1} as a new item with ID {2}", Variables.NAMESPACE, this.Name, this.ID);
                        items.Add(this.ID, thisItemRaw);
                    }
                }
                else
                {
                    Pipliz.Log.Write("{0}: Registering block {1} to ID {2}", Variables.NAMESPACE, this.Name, this.ID);
                    if (items.ContainsKey(this.ID))
                    {
                        // Item already exists, do we overwrite?
                        if (overwrite)
                        {
                            Pipliz.Log.Write("{0}: Item {1} already exists, overwriting item entry.", Variables.NAMESPACE, this.ID);
                            // Remove existing item.
                            ItemHelper.tryRemoveItem(this.ID);
                            // Add this item.
                            items.Add(this.ID, thisItemRaw);
                        }
                        else
                        {
                            // Do nothing, it already exists and we're neither masking nor overwriting.
                            Pipliz.Log.Write("{0}: Item {1} already exists, registration is not necessary and is being aborted.", Variables.NAMESPACE, this.ID);
                        }
                    }
                    else
                    {
                        // Item does not already exist, add it.
                        items.Add(this.ID, thisItemRaw);
                    }
                }

                Pipliz.Log.Write("{0}: Block {1} registration complete.", Variables.NAMESPACE, this.Name);
            }
            else
            {
                Pipliz.Log.Write("{0}: Block {1} has been disabled, and will NOT be registered.", Variables.NAMESPACE, this.Name);
            }
        }

        /// <summary>
        /// Registers this block as a crate if the isCrate property is set to true. Should be called during the AfterItemTypesDefined callback.
        /// </summary>
        public void registerAsCrate()
        {
            if (this.isCrate)
            {
                Pipliz.Log.Write("{0}: Attempting to register {1} as a crate.", Variables.NAMESPACE, this.ID);

                try
                {
                    ItemTypesServer.RegisterOnAdd(this.ID, StockpileBlockTracker.Add);
                    ItemTypesServer.RegisterOnRemove(this.ID, StockpileBlockTracker.Remove);
                }
                catch (Exception ex)
                {
                    Pipliz.Log.Write("{0}: Crate registration error: {1}", Variables.NAMESPACE, ex.Message);
                }
            }
        }

        /// <summary>
        /// Associates a job class to this block.
        /// </summary>
        /// <typeparam name="T">A class which describes the job being associated with the block, must impliment ITrackableBlock,
        /// IBlockJobBase, INPCTypeDefiner, and have a default constructor. Should be called during the AfterDefiningNPCTypes callback.</typeparam>
        public void registerJob<T>() where T : ITrackableBlock, IBlockJobBase, INPCTypeDefiner, new()
        {
            Pipliz.Log.Write("{0}: Attempting to register a job to block {1}", Variables.NAMESPACE, this.ID);
            try
            {
                BlockJobManagerTracker.Register<T>(this.ID);
            }
            catch (Exception ex)
            {
                Pipliz.Log.Write("{0}: Registration error: {1}", Variables.NAMESPACE, ex.Message);
            }
        }

        /// <summary>
        /// Returns this item's properties as a JSONNode.
        /// </summary>
        /// <param name="thisItemJSON">Optional. If provided overwrites the values in provided JSONNode with this object's values and returns provided JSON.</param>
        /// <returns>A JSONNode object containing original data (if provided by thisItemJSON parameter) overwritten by this item's data.</returns>
        protected JSONNode itemAsJSON(JSONNode thisItemJSON = null)
        {
            if (thisItemJSON == null) thisItemJSON = new JSONNode();
            if (Icon != null) thisItemJSON.SetAs("icon", Icon);
            if (isPlaceable != null) thisItemJSON.SetAs("isPlaceable", isPlaceable);
            if (isDestructible != null) thisItemJSON.SetAs("isDestructible", isDestructible);
            if (isSolid != null) thisItemJSON.SetAs("isSolid", isSolid);
            if (this.Drops.Count > 0)
            {
                JSONNode DropsNode = new JSONNode(NodeType.Array);
                foreach (SimpleItem.DropItem thisDrop in Drops)
                {
                    DropsNode.AddToArray(thisDrop.asJSONNode());
                }
            }
            if (sideAll != null) thisItemJSON.SetAs("sideall", sideAll);
            if (destructionTime != null) thisItemJSON.SetAs("destructionTime", destructionTime);
            if (isFertile != null) thisItemJSON.SetAs("isFertile", isFertile);
            if (mesh != null) thisItemJSON.SetAs("mesh", mesh);
            if (minerIsMineable != null || lightSource != null)
            {
                JSONNode customData = new JSONNode();
                if (minerIsMineable != null && minerIsMineable == true)
                {
                    JSONNode MiningData = new JSONNode();
                    customData.SetAs("minerIsMineable", true);
                    customData.SetAs("minerMiningTime", minerMiningTime);
                }
                if (lightSource != null)
                {
                    customData.SetAs("torches", lightSource.asJSONNode());
                }
                thisItemJSON.SetAs("customData", customData);
            }
            if (sideTop != null) thisItemJSON.SetAs("sidey+", sideTop);
            if (sideBottom != null) thisItemJSON.SetAs("sidey-", sideBottom);
            if (sideFront != null) thisItemJSON.SetAs("sidez+", sideFront);
            if (sideBack != null) thisItemJSON.SetAs("sidez-", sideBack);
            if (sideLeft != null) thisItemJSON.SetAs("sidex-", sideLeft);
            if (sideRight != null) thisItemJSON.SetAs("sidex+", sideRight);
            if (onPlaceAudio != null) thisItemJSON.SetAs("onPlaceAudio", onPlaceAudio);
            if (onRemoveAudio != null) thisItemJSON.SetAs("onRemoveAudio", onRemoveAudio);
            /*
            using (StreamWriter toJSON = new StreamWriter(this.Name + ".JSON"))
            {
                thisItemJSON.Serialize(toJSON,0);
            }
            */
            Pipliz.Log.Write("{0}: Created raw item type {1}, not yet registered.", Variables.NAMESPACE, this.Name);
            return thisItemJSON;
        }

        /// <summary>
        /// Helper class used in building ItemTypeRaw JSONs
        /// </summary>
        public struct DropItem
        {
            string type;
            int amount;
            float chance;

            public DropItem(string strType)
            {
                type = strType;
                amount = 1;
                chance = 1f;
            }

            public DropItem(string strType, int intAmount)
            {
                type = strType;
                amount = intAmount;
                chance = 1f;
            }

            public DropItem(string strType, int intAmount, float fltChance)
            {
                type = strType;
                amount = intAmount;
                chance = fltChance;
            }

            public JSONNode asJSONNode ()
            {
                JSONNode returnMe = new JSONNode();
                returnMe.SetAs("type", type);
                returnMe.SetAs("amount", amount);
                returnMe.SetAs("chance", chance);

                return returnMe;
            }
        }

        public class Light
        {
            public float volume = 0.5f;
            public float intensity = 10f;
            public int range = 10;
            public float red = 195f;
            public float green = 135f;
            public float blue = 46f;

            public JSONNode asJSONNode ()
            {
                JSONNode torches = new JSONNode();
                JSONNode a = new JSONNode();
                a.SetAs("volume", volume);
                a.SetAs("intensity", intensity);
                a.SetAs("range", range);
                a.SetAs("red", red);
                a.SetAs("green", green);
                a.SetAs("blue", blue);
                torches.SetAs("a", a);

                return torches;
            }
        }
    }
}
