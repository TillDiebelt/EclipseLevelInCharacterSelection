using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EclipseLevelInCharacterSelection
{
    [BepInDependency(R2API.R2API.PluginGUID)]
	
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
	
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI))]
	
	public class EclipseLevelInCharacterSelection : BaseUnityPlugin
	{
        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "depression_church";
        public const string PluginName = "EclipseLevelInCharacterSelection";
        public const string PluginVersion = "1.0.2";

        private static Dictionary<string, bool> changedSurvivorIcon = new Dictionary<string, bool>();
        private static Dictionary<string, int> survivorDic = new Dictionary<string, int>() {
            { "Captain", 1 },{ "Merc", 9 },{ "Huntress", 6 },{ "Bandit2", 0 },{ "Loader", 7 },{ "Engi", 4 },{ "Commando", 2 },
            { "Toolbot", 10},{ "Mage", 8},{ "Treebot", 11},{ "Croco", 3},{ "Railgunner", 12},{ "VoidSurvivor", 13} };
        private static Dictionary<int, string> survivorDicIntToString = new Dictionary<int, string>() {
            { 1, "Captain" },{ 9, "Merc" },{ 6, "Huntress" },{ 0, "Bandit2" },{ 7, "Loader"},{ 4, "Engi"},{ 2, "Commando"},
            { 10, "Toolbot"},{ 8, "Mage"},{ 11, "Treebot"},{ 3, "Croco"},{ 12, "Railgunner"},{ 13, "VoidSurvivor"} };
        public static Dictionary<string, int> characterLevels;
        
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
            if (!changedSurvivorIcon.ContainsKey(self.survivorIndex.ToString())) //isEclipseRun && 
            {
                if (survivorDicIntToString.ContainsKey(Int32.Parse(self.survivorIndex.ToString())) && characterLevels.ContainsKey(survivorDicIntToString[Int32.Parse(self.survivorIndex.ToString())]))
                {
                    Texture2D tex_orig = duplicateTexture(ToTexture2D(self.survivorIcon.texture));
                    int eclipseLevel = characterLevels[survivorDicIntToString[Int32.Parse(self.survivorIndex.ToString())]];
                    //why 120 and why blablabla, idk
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
                changedSurvivorIcon.Add(self.survivorIndex.ToString(), true);

            }
        }

        //argh
        public static Texture2D ToTexture2D(Texture texture)
        {
            return Texture2D.CreateExternalTexture(
                texture.width,
                texture.height,
                TextureFormat.RGB24,
                false, false,
                texture.GetNativeTexturePtr());
        }

        //argh2
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
            Texture2D readableText = new Texture2D(source.width, source.height);
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
            //does not work
            isEclipseRun = PreGameController.instance && PreGameController.instance.gameModeIndex == GameModeCatalog.FindGameModeIndex("EclipseRun");
            
            Log.LogInfo("loading eclipse data");
            var localUser = LocalUserManager.localUsersList[0]._userProfile;
            var xml = UserProfile.ToXml(localUser);
            //find eclipse levels of characters in xml
            var characters = xml.Descendants("unlock").Where(x => x.Value.Contains("Eclipse."));
            characterLevels = new Dictionary<string, int>();
            try
            {
                foreach (var character in characters)
                {
                    var parse = character.Value.Split('.');
                    if (characterLevels.ContainsKey(parse[1]))
                    {
                        characterLevels[parse[1]] = Math.Max(characterLevels[parse[1]], Int32.Parse(parse[2]));
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
            changedSurvivorIcon = new Dictionary<string, bool>();
            
            orig(self);
        }
    }
}
