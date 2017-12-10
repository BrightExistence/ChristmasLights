using System;
using BrightExistence.SimpleTools;

namespace BrightExistence
{
    public static class TextureHelper
    {
    }

    /// <summary>
    /// Front-end for built-in ItemTypesServer.TextureMapping class to enable auto-registration.
    /// </summary>
    public class SimpleTexture
    {
        /// <summary>
        /// Name of texture, excluding any prefixes. Ex: myTexture NOT myHandle.myMod.myTexture
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Prefix used to generate ID. Ex: myHandle.myMod
        /// </summary>
        public string NAMESPACE { get; protected set; }

        public string AlbedoPath;

        public string NormalPath;

        public string EmissivePath;

        public string HeightPath;

        /// <summary>
        /// The string by which this texture will be referenced.
        /// </summary>
        public string ID
        {
            get
            {
                return NAMESPACE + "." + Name;
            }
        }

        /// <summary>
        /// Constructor for SimpleTexture.
        /// </summary>
        /// <param name="strName">Name of texture, excluding any prefixes. Ex: myTexture NOT myHandle.myMod.myTexture</param>
        /// <param name="strNAMESPACE">Prefix used to generate ID. Ex: myHandle.myMod</param>
        public SimpleTexture(string strName, string strNAMESPACE)
        {
            Name = (strName == null || strName.Length < 1) ? "NewTexture" : strName;
            NAMESPACE = strNAMESPACE == null ? "" : strNAMESPACE;
            Pipliz.Log.Write("{0}: Initializing texture {1}, it is not yet registered.", Variables.NAMESPACE, this.Name);
            try
            {
                if (!Variables.Textures.Contains(this)) Variables.Textures.Add(this);
            }
            catch (Exception)
            {
                Pipliz.Log.Write("{0} : WARNING : Texture {1} could not be automatically added to auto-load list. Make sure you explicityly added it.", Variables.NAMESPACE, this.Name);
            }
        }

        /// <summary>
        /// Returns this item as a ItemTypeServer.TextureMapping struct. (Note this will strip name, ID, and namespace properties.)
        /// </summary>
        /// <returns>ItemTypeServer.TextureMapping struct</returns>
        protected ItemTypesServer.TextureMapping asTextureMapping ()
        {
            ItemTypesServer.TextureMapping thisMapping = new ItemTypesServer.TextureMapping(new Pipliz.JSON.JSONNode());
            if (this.AlbedoPath != null) thisMapping.AlbedoPath = this.AlbedoPath;
            if (this.NormalPath != null) thisMapping.NormalPath = this.NormalPath;
            if (this.EmissivePath != null) thisMapping.EmissivePath = this.EmissivePath;
            if (this.HeightPath != null) thisMapping.HeightPath = this.HeightPath;
            return thisMapping;
        }

        /// <summary>
        /// Registers this texture in the server database. Should be called during the afterSelectedWorld callback method.
        /// </summary>
        public void registerTexture ()
        {
            Pipliz.Log.Write("Registering texture as "+ this.ID + " using file: " + this.AlbedoPath);
            if (System.IO.File.Exists(this.AlbedoPath))
            {
                Pipliz.Log.Write("{0}: Looks good, file exists.", Variables.NAMESPACE);
            }
            else
            {
                Pipliz.Log.WriteError("{0}: ERROR! Registering texture to a file which does not exist!", Variables.NAMESPACE);
            }
            ItemTypesServer.SetTextureMapping(this.ID, this.asTextureMapping());
            Pipliz.Log.Write("Texture registered: "+ this.Name);
        }
    }
}
