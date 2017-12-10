using System;
using System.Collections.Generic;

namespace BrightExistence.SimpleTools
{
    public static class RecipeHelper
    {
        /// <summary>
        /// Attempts to remove an existing recipe from the server's database.
        /// </summary>
        /// <param name="recName">Name of recipe.</param>
        /// <returns>True if recipe was removed, False if recipe was not found or removal was not successful.</returns>
        public static bool tryRemoveRecipe (string recName)
        {
            try
            {
                if (RecipeStorage.TryGetRecipe(recName, out Recipe Rec))
                {
                    Pipliz.Log.Write("{0}: Recipe {1} found, attempting to remove.", Variables.NAMESPACE, Rec.Name);
                    RecipeStorage.Recipes.Remove(recName);

                    if (!RecipeStorage.TryGetRecipe(recName, out Recipe Rec2))
                    {
                        Pipliz.Log.Write("{0}: Recipe {1} successfully removed", Variables.NAMESPACE, Rec.Name);
                        return true;
                    }
                    else
                    {
                        Pipliz.Log.Write("{0}: Recipe {1} removal failed for unknown reason.", Variables.NAMESPACE, Rec.Name);
                        return false;
                    }
                }
                else
                {
                    Pipliz.Log.Write("{0}: Recipe {1} not found.", Variables.NAMESPACE, recName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Pipliz.Log.Write("{0}: tryRemoveRecipe has reached an exception.");
                return false;
            }
        }
    }

    public class SimpleRecipe
    {
        /// <summary>
        /// Name of Recipe, excluding prefixs. Ex: myRecipe instead of myHandle.myMod.myRecipe
        /// </summary>
        public string Name = "New Recipe";

        /// <summary>
        /// An InventoryItem list containing the items the user recieves when this recipe is completed. May be ignored if the constructor
        /// which takes a SimpleItem object is used.
        /// </summary>
        protected List<InventoryItem> realResults = new List<InventoryItem>();

        /// <summary>
        /// Items references are added in the form of shells so they can be evaluated at the right time into actual InventoryItem objects.
        /// </summary>
        public List<ItemShell> Results = new List<ItemShell>();

        /// <summary>
        /// Items references are added in the form of shells so they can be evaluated at the right time into actual InventoryItem objects.
        /// </summary>
        public List<ItemShell> Requirements = new List<ItemShell>();

        /// <summary>
        /// An InventoryItem list containing the items necessary to complete this recipe.
        /// </summary>
        protected List<InventoryItem> realRequirements = new List<InventoryItem>();

        /// <summary>
        /// The limitType, a.k.a. NPCTypeKey is essentially a group of recipes associated with a block and an NPC. Ex: pipliz.crafter
        /// </summary>
        public string limitType { get; set; }

        /// <summary>
        /// The default limit at which an NPC will stop crafting this recipe.
        /// </summary>
        public int defaultLimit = 1;

        /// <summary>
        /// The default priority of this recipe vs other recipes of the same limitType when crafted by an NPC.
        /// </summary>
        public int defaultPriority = 0;

        /// <summary>
        /// True if this recipe must be researched to be available, otherwise false.
        /// </summary>
        public bool isOptional = false;

        /// <summary>
        /// Set to true if you want addRecipeToLimitType() to create a copy of this recipe and add it to the list of recipes the players
        /// themselves can craft.
        /// </summary>
        public bool userCraftable = false;

        /// <summary>
        /// Names what recipes, if any, this recipe is intended to replace. The named recipes will be deleted from the server's
        /// database before this recipe is added. Use when replacing vanilla recipes.
        /// </summary>
        public List<string> Replaces = new List<string>();

        /// <summary>
        /// Set to false if you want this recipe, and SimpleResearchable items that know about and depend upon it, to NOT be registered.
        /// </summary>
        public bool enabled = true;

        /// <summary>
        /// Automatically generated recipe key with limit type prefix. Ex: 'recipeLimit.myRecipe'. To get the player
        /// crafted recipe key, use "player." + Name
        /// </summary>
        public string fullName
        {
            get
            {
                if (limitType != null) return limitType + "." + Name;
                else return Name;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strName">Name of recipe excluding any prefixes. Ex: myRecipe NOT myHandle.myMod.myRecipe</param>
        /// <param name="strLimitType">The limitType, a.k.a. NPCTypeKey is essentially a group of recipes associated with a block and an NPC. Ex: pipliz.crafter</param>
        public SimpleRecipe(string strName, string strLimitType = null)
        {
            this.Name = strName == null ? Variables.NAMESPACE + "NewRecipe" : strName;
            this.limitType = strLimitType;

            Pipliz.Log.Write("{0}: Initialized Recipe {1} (it is not yet registered.)", Variables.NAMESPACE, this.Name);
            try
            {
                if (!Variables.Recipes.Contains(this)) Variables.Recipes.Add(this);
            }
            catch (Exception)
            {
                Pipliz.Log.Write("{0} : WARNING : Recipe {1} could not be automatically added to auto-load list. Make sure you explicityly added it.", Variables.NAMESPACE, this.Name);
            }

            if (strLimitType == null) userCraftable = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Item">A SimpleItem object holding a type which is the intended result of this recipe.</param>
        /// <param name="strLimitType">The limitType, a.k.a. NPCTypeKey is essentially a group of recipes associated with a block and an NPC. Ex: pipliz.crafter</param>
        public SimpleRecipe(SimpleItem Item, string strLimitType = null)
        {
            if (Item == null || Item.Name == null || Item.Name.Length < 1)
            {
                throw new ArgumentException(Variables.NAMESPACE + ": Simple recipe cannot initialize when given a null Item or an Item with a Name of less than one character.");
            }
            else
            {
                limitType = strLimitType;
                this.Name = Item.Name;
                addResult(Item);

                Pipliz.Log.Write("{0}: Initialized Recipe {1} (it is not yet registered.)", Variables.NAMESPACE, Name);
                try
                {
                    if (!Variables.Recipes.Contains(this)) Variables.Recipes.Add(this);
                }
                catch (Exception)
                {
                    Pipliz.Log.Write("{0} : WARNING : Recipe {1} could not be automatically added to auto-load list. Make sure you explicityly added it.", Variables.NAMESPACE, this.Name);
                }

                if (strLimitType == null) userCraftable = true;
            }
        }

        public void addRequirement (string itemKey, int amount = 1)
        {
            if (itemKey == null || itemKey.Length < 1)
            {
                Pipliz.Log.Write("{0} WARNING: Recipe {1}'s addRequirement() method was called but was given a null or invalid item key.", Variables.NAMESPACE, this.Name);
            }
            else
            {
                Requirements.Add(new ItemShell(itemKey, amount));
            }
        }

        public void addRequirement (SimpleItem requiredItem, int amount = 1)
        {
            if (requiredItem != null && requiredItem.Name.Length > 0)
            {
                Requirements.Add(new ItemShell(requiredItem.ID, amount));
                if (!requiredItem.enabled) this.enabled = false;
            }
            else
            {
                Pipliz.Log.Write("{0} WARNING: Recipe {1}'s addRequirement() method was called but was given a null or invalid SimpleItem object.", Variables.NAMESPACE, this.Name);
            }
        }

        public void addResult (string itemKey, int amount = 1)
        {
            if (itemKey == null || itemKey.Length < 1)
            {
                Pipliz.Log.Write("{0} WARNING: Recipe {1}'s addResult() method was called but was given a null or invalid item key.", Variables.NAMESPACE, this.Name);
            }
            else
            {
                Results.Add(new ItemShell(itemKey, amount));
            }
        }

        public void addResult(SimpleItem resultItem, int amount = 1)
        {
            if (resultItem != null && resultItem.Name.Length > 0)
            {
                Results.Add(new ItemShell(resultItem.ID, amount));
                if (!resultItem.enabled) this.enabled = false;
            }
            else
            {
                Pipliz.Log.Write("{0} WARNING: Recipe {1}'s addResult() method was called but was given a null or invalid SimpleItem object.", Variables.NAMESPACE, this.Name);
            }
        }

        /// <summary>
        /// Does all the work of adding this recipe to the server's database. Should be called in the AfterItemTypesDefined callback.
        /// </summary>
        public void addRecipeToLimitType()
        {
            if (enabled)
            {
                try
                {
                    // First remove any recipes we are replacing.
                    foreach (string deleteMe in Replaces)
                    {
                        Pipliz.Log.Write("{0}: Recipe {1} is marked as replacing {2}, attempting to comply.", Variables.NAMESPACE, this.Name, deleteMe);
                        RecipeHelper.tryRemoveRecipe(deleteMe);
                    }

                    // Convert shell references into actual InventoryItem objects.
                    foreach (ItemShell I in Results)
                    {
                        if (Variables.itemsMaster == null)
                        {
                            Pipliz.Log.WriteError("{0}.SimpleRecipe.addRecipeToLimitType() has reached a critical error: 'Variables.itemsMaster' is not yet available. Recipe: {1}", Variables.NAMESPACE, this.Name);
                        }
                        else
                        {
                            if (Variables.itemsMaster.ContainsKey(I.strItemkey))
                            {
                                realResults.Add(new InventoryItem(I.strItemkey, I.intAmount));
                            }
                            else
                            {
                                Pipliz.Log.WriteError("{0}: A problem occurred adding recipe RESULT {1} to recipe {2}, the item key was not found.", Variables.NAMESPACE, I.strItemkey, this.Name);
                            }
                        }
                    }
                    foreach (ItemShell I in Requirements)
                    {
                        if (Variables.itemsMaster == null)
                        {
                            Pipliz.Log.WriteError("{0}.SimpleRecipe.addRecipeToLimitType() has reached a critical error: 'Variables.itemsMaster' is not yet available. Recipe: {1}", Variables.NAMESPACE, this.Name);
                        }
                        else
                        {
                            if (Variables.itemsMaster.ContainsKey(I.strItemkey))
                            {
                                realRequirements.Add(new InventoryItem(I.strItemkey, I.intAmount));
                            }
                            else
                            {
                                Pipliz.Log.WriteError("{0}: A problem occurred adding recipe REQUIREMENT {1} to recipe {2}, the item key was not found.", Variables.NAMESPACE, I.strItemkey, this.Name);
                            }
                        }
                    }

                    // Build actual Recipe object.
                    Recipe thisRecipe = new Recipe(this.fullName, this.realRequirements, this.realResults, this.defaultLimit, this.isOptional, this.defaultPriority);

                    // Commence registering it.
                    Pipliz.Log.Write("SimpleItem: Attempting to register recipe {0} to limit type (NPCName) {1}", thisRecipe.Name, limitType);
                    if (isOptional)
                    {
                        Pipliz.Log.Write("{0}: Attempting to register optional limit type recipe {1}", Variables.NAMESPACE, thisRecipe.Name);
                        RecipeStorage.AddOptionalLimitTypeRecipe(limitType, thisRecipe);
                    }
                    else
                    {
                        Pipliz.Log.Write("{0}: Attempting to register default limit type recipe {1}", Variables.NAMESPACE, thisRecipe.Name);
                        RecipeStorage.AddDefaultLimitTypeRecipe(limitType, thisRecipe);
                    }

                    if (userCraftable)
                    {
                        Recipe playerRecipe = new Recipe("player." + this.Name, this.realRequirements, this.realResults, this.defaultLimit, this.isOptional);
                        Pipliz.Log.Write("{0}: Attempting to register default player type recipe {1}", Variables.NAMESPACE, playerRecipe.Name);
                        RecipePlayer.AddDefaultRecipe(playerRecipe);
                    }
                }
                catch (Exception ex)
                {
                    Pipliz.Log.WriteError("{0}: Error adding recipe to limit type: {1}", Variables.NAMESPACE, ex.Message);
                }
            }
            else
            {
                Pipliz.Log.Write("{0}: Recipe {1} has been disabled and will NOT be registered.", Variables.NAMESPACE, this.Name);
            }
        }
    }
}
