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
        public const string PluginVersion = "1.2.0";

        private static HashSet<string> changedSurvivorIcon = new HashSet<string>();

        public static Dictionary<string, int> characterLevels;

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

            if(self == null || self.survivorDef == null || self.survivorDef.cachedName == null)
            {
                Log.LogWarning("EclipseLevelInCharacterSelection: SurvivorIconController or one of its used children was null");
                return;
            }

            //This should only load in eclipse lobbies:
            // && PreGameController.GameModeConVar.instance.GetString() == "EclipseRun"
            //seems to be bugged for non hosts and only load sometimes
            if (!changedSurvivorIcon.Contains(self.survivorIndex.ToString())) 
            {
                //if it was not parsed I guess it is 1
                int eclipseLevel = 1;
                if (characterLevels.ContainsKey(self.survivorDef.cachedName))
                {
                    eclipseLevel =  characterLevels[self.survivorDef.cachedName];
                }
                Texture2D tex_orig = duplicateTexture(ToTexture2D(self.survivorIcon.texture));
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

                //avoid duplicate loading, this will break with scrolling survivor lists
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
        private Texture2D duplicateTexture(Texture2D source)
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
                var localUser = LocalUserManager.GetFirstLocalUser().userProfile;
                var xml = UserProfile.ToXml(localUser);
                //find eclipse levels of characters in xml
                var characters = xml.Descendants("unlock").Where(x => x.Value.Contains("Eclipse."));
                characterLevels = new Dictionary<string, int>();
                foreach (var character in characters)
                {
                    var parse = character.Value.Split('.');
                    string characterName = parse[1];
                    int characterEclipseLevel = Int32.Parse(parse[2]);
                    if (characterLevels.ContainsKey(characterName))
                    {
                        characterLevels[characterName] = Math.Max(characterLevels[characterName], characterEclipseLevel);
                    }
                    else
                    {
                        characterLevels.Add(characterName, characterEclipseLevel);
                    }
                }
            }
            catch (Exception e)
            {
                //i dont know why i try catch here but i parse things so why not
                Log.LogError("EclipseLevelInCharacterSelection: unexpected error while parsing character eclipse levels: " + e.Message);
            }
            changedSurvivorIcon = new HashSet<string>();
            
            orig(self);
        }
    }
}
