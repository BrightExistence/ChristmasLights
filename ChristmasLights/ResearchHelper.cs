using System;
using System.Collections.Generic;
using Pipliz.APIProvider.Science;
using Server.Science;

namespace BrightExistence.SimpleTools
{
    public static class ResearchHelper
    {

    }

    public class SimpleResearchable : IResearchable
    {
        protected string NAMESPACE;
        public string Name = "New Researchable";
        public string Replaces { get; set; }
        public string Icon = "";
        public int IterationCount = 1;
        public List<string> Dependencies = new List<string>();
        protected List<InventoryItem> Requirements = new List<InventoryItem>();
        public List<ItemShell> IterationRequirements = new List<ItemShell>();
        public List<Unlock> Unlocks = new List<Unlock>();
        public bool enabled = true;

        public SimpleResearchable(string strName, string strNAMESPACE = Variables.NAMESPACE)
        {
            if (strName != null && strName.Length > 0) Name = strName;
            NAMESPACE = strNAMESPACE;
            Variables.Researchables.Add(this);
            Pipliz.Log.Write("{0}: Initialized Researchable {1} (it is not yet registered.)", Variables.NAMESPACE, this.Name);
        }

        public void Register ()
        {
            if (enabled)
            {
                // Convert shell items to real items.
                foreach (ItemShell I in IterationRequirements)
                {
                    if (Variables.itemsMaster == null)
                    {
                        Pipliz.Log.WriteError("{0} CRITICAL ERROR: SimpleResearchable {1} cannot register properly because 'Variables.itemsMaster' is still null.", Variables.NAMESPACE, this.Name);
                    }
                    else
                    {
                        Pipliz.Log.Write("{0}: Converting shell references in researchable {1} to InventoryItem objects.", Variables.NAMESPACE, this.Name);
                        if (Variables.itemsMaster.ContainsKey(I.strItemkey))
                        {
                            Requirements.Add(new InventoryItem(I.strItemkey, I.intAmount));
                        }
                        else
                        {
                            Pipliz.Log.Write("{0} Researchable {1} was given an item key '{2}' as an iteration requirement which was not found by the server.", Variables.NAMESPACE, this.Name, I.strItemkey);
                        }
                    }
                }

                ScienceManager.RegisterResearchable(this);
                Pipliz.Log.Write("{0}: Researchable {1} has been registered with the ScienceManager.", Variables.NAMESPACE, this.Name);
            }
            else
            {
                Pipliz.Log.Write("{0}: Research {1} has been disabled, and will NOT be registered.", Variables.NAMESPACE, this.Name);
            }
        }

        public void addRequirement(string itemKey, int amount = 1)
        {
            if (itemKey == null || itemKey.Length < 1)
            {
                Pipliz.Log.Write("{0}: Research {1} was given a null or invalid item key.", Variables.NAMESPACE, this.Name);
            }
            else
            {
                IterationRequirements.Add(new ItemShell(itemKey, amount));
            }
        }

        public void addRequirement(SimpleItem requiredItem, int amount = 1)
        {
            if (requiredItem == null || requiredItem.Name.Length < 1)
            {
                Pipliz.Log.Write("{0}: Research {1} was given a null or invalid SimpleItem object.", Variables.NAMESPACE, this.Name);
            }
            else
            {
                IterationRequirements.Add(new ItemShell(requiredItem.ID, amount));
                if (!requiredItem.enabled) this.enabled = false;
            }
        }

        public string GetKey()
        {
            if (Replaces == null) return NAMESPACE + ".Research." + Name;
            else return Replaces;
        }

        public string GetIcon()
        {
            return Icon;
        }

        public IList<string> GetDependencies()
        {
            return Dependencies;
        }

        public IList<InventoryItem> GetScienceRequirements()
        {
            return Requirements;
        }

        public int GetResearchIterationCount()
        {
            return IterationCount;
        }

        public void OnResearchComplete(ScienceManagerPlayer manager, EResearchCompletionReason reason)
        {
            foreach (Unlock U in Unlocks)
            {
                if (U.limitType != null)
                {
                    RecipeStorage.GetPlayerStorage(manager.Player).SetRecipeAvailability(U.recipeName, true, U.limitType);
                }
                if (U.uservariant != null)
                {
                    RecipePlayer.UnlockOptionalRecipe(manager.Player, U.uservariant);
                }
            }
        }

        public class Unlock
        {
            public string recipeName = "";
            public string uservariant = null;
            public string limitType = null;
            public bool enabled = true;

            public Unlock(string strRecipeName, string strLimitType)
            {
                if (strRecipeName != null) recipeName = strRecipeName;
                limitType = strLimitType;
            }

            public Unlock(SimpleRecipe unlockMe)
            {
                if (unlockMe.limitType != null) recipeName = unlockMe.fullName;
                if (unlockMe.userCraftable) uservariant = "player." + unlockMe.Name;
                if (!unlockMe.enabled) this.enabled = false;
            }
        }
    }
}
