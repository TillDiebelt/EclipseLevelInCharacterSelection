using BepInEx;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EclipseLevelInCharacterSelection
{	
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public class EclipseLevelInCharacterSelection : BaseUnityPlugin
	{
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "depression_church";
        public const string PluginName = "EclipseLevelInCharacterSelection";
        public const string PluginVersion = "1.1.4";

        private static HashSet<string> changedSurvivorIcon = new HashSet<string>();

        //need to change this to work with modded survivors, dont hardcode it but i am to lazy to see how to do it better
        private static Dictionary<string, int> survivorDic = new Dictionary<string, int>() {
            { "Captain", 1 },{ "Merc", 9 },{ "Huntress", 6 },{ "Bandit2", 0 },{ "Loader", 7 },{ "Engi", 4 },{ "Commando", 2 },
            { "Toolbot", 10},{ "Mage", 8},{ "Treebot", 11},{ "Croco", 3},{ "Railgunner", 12},{ "VoidSurvivor", 13},{ "Seeker", 14},{ "FalseSon", 15},{ "Chef", 16} };
        private static Dictionary<int, string> survivorDicIntToString = new Dictionary<int, string>() {
            { 1, "Captain" },{ 9, "Merc" },{ 6, "Huntress" },{ 0, "Bandit2" },{ 7, "Loader"},{ 4, "Engi"},{ 2, "Commando"},
            { 10, "Toolbot"},{ 8, "Mage"},{ 11, "Treebot"},{ 3, "Croco"},{ 12, "Railgunner"},{ 13, "VoidSurvivor"} ,{ 14, "Seeker"} ,{ 15, "FalseSon"} ,{ 16, "Chef"} };
        public static Dictionary<string, int> characterLevels;
        
        //would be cool if mod only shows in eclipse runs, but oh well
        public static bool isEclipseRun;

        public void Awake()
        {
            Log.Init(Logger);

            On.RoR2.UI.CharacterSelectController.Awake += AddEclipseLevels;

            On.RoR2.UI.SurvivorIconController.Update += UpdateUiIcons;

            Log.LogInfo(nameof(Awake) + " done.");
        }

        private void UpdateUiIcons(On.RoR2.UI.SurvivorIconController.orig_Update orig, RoR2.UI.SurvivorIconController self)
        {
            orig(self);
            if (!changedSurvivorIcon.Contains(self.survivorIndex.ToString())) //isEclipseRun && 
            {
                if (survivorDicIntToString.ContainsKey(Int32.Parse(self.survivorIndex.ToString())) && characterLevels.ContainsKey(survivorDicIntToString[Int32.Parse(self.survivorIndex.ToString())]))
                {
                    Texture2D tex_orig = duplicateTexture(ToTexture2D(self.survivorIcon.texture));
                    int eclipseLevel = characterLevels[survivorDicIntToString[Int32.Parse(self.survivorIndex.ToString())]];
                    //i dont know if this icon can be smaller then 128 pixels, so safety first
                    if (tex_orig.width > 120)
                    {
                        tex_orig.AddBigNumberToTexture(eclipseLevel);
                    }
                    else
                    {
                        tex_orig.AddNumberToTexture(eclipseLevel);
                    }

                    tex_orig.Apply();

                    self.survivorIcon.texture = (Texture)tex_orig;
                }
                changedSurvivorIcon.Add(self.survivorIndex.ToString());
            }
        }

        //helper function
        public static Texture2D ToTexture2D(Texture texture)
        {
            return Texture2D.CreateExternalTexture(
                texture.width,
                texture.height,
                TextureFormat.RGB24,
                false, false,
                texture.GetNativeTexturePtr());
        }

        //helper function
        Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height,TextureFormat.RGBA32,mipChain: false);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        private void OnDestroy()
        {
            //i should do stuff here but idk
        }

        private void AddEclipseLevels(On.RoR2.UI.CharacterSelectController.orig_Awake orig, RoR2.UI.CharacterSelectController self)
        {
            try
            {
                //does not work
                isEclipseRun = PreGameController.instance && PreGameController.instance.gameModeIndex == GameModeCatalog.FindGameModeIndex("EclipseRun");
            
                var localUser = LocalUserManager.GetFirstLocalUser().userProfile;
                var xml = UserProfile.ToXml(localUser);
                //find eclipse levels of characters in xml
                var characters = xml.Descendants("unlock").Where(x => x.Value.Contains("Eclipse."));
                characterLevels = new Dictionary<string, int>();
                foreach (var character in characters)
                {
                    var parse = character.Value.Split('.');
                    if (characterLevels.ContainsKey(parse[1]))
                    {
                        characterLevels[parse[1]] = Math.Min(Math.Max(characterLevels[parse[1]], Int32.Parse(parse[2])),8);
                    }
                    else
                    {
                        characterLevels.Add(parse[1], Int32.Parse(parse[2]));
                    }
                }
            }
            catch (Exception e)
            {
                //i dont know why i try catch here but i parse things so why not
                Log.LogError("Error while parsing character eclipse levels: " + e.Message);
            }
            changedSurvivorIcon = new HashSet<string>();
            
            orig(self);
        }
    }
}
