using BepInEx;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EclipseLevelInCharacterSelection
{	
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class EclipseLevelInCharacterSelection : BaseUnityPlugin
	{
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "depression_church";
        public const string PluginName = "EclipseLevelInCharacterSelection";
        public const string PluginVersion = "2.1.0";

        private static Texture _GoldE8Icon;
        private static Texture GoldE8Icon => _GoldE8Icon ??= UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Texture>("RoR2/Base/EclipseRun/texDifficultyEclipse8IconGold.png").WaitForCompletion();

        internal static new Config Config { get; private set; }

        private void Awake()
        {
            Log.Init(Logger);
            Config = new Config(base.Config);

            On.RoR2.UI.SurvivorIconController.Rebuild += SurvivorIconController_Rebuild;

            Log.LogInfo(nameof(Awake) + " done.");
        }

        private void OnDestroy()
        {
            On.RoR2.UI.SurvivorIconController.Rebuild -= SurvivorIconController_Rebuild;
        }

        private void SurvivorIconController_Rebuild(On.RoR2.UI.SurvivorIconController.orig_Rebuild orig, RoR2.UI.SurvivorIconController self)
        {
            orig(self);

            try {
                string activeSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
#if DEBUG
                // On clients (non-hosts), PreGameController.GameModeConVar is "ClassicRun" instead of "EclipseRun" (i.e. not synced?); it also doesn't update in pre-lobby menus
                Log.LogWarning($"{PreGameController.GameModeConVar.instance.GetString()} | {activeSceneName} | {PreGameController.instance?.gameModeIndex} | {(PreGameController.instance ? GameModeCatalog.indexToName[(int)PreGameController.instance.gameModeIndex] : "null")}");
#endif
                bool isEclipseMenu = (activeSceneName == "eclipseworld" || (PreGameController.instance != null && PreGameController.instance.gameModeIndex == GameModeCatalog.FindGameModeIndex("EclipseRun")));
                if (Config.OnlyShowInEclipseLobby && !isEclipseMenu) return;
                if (activeSceneName == "infinitetowerworld") return; // Never show in Simulacrum pre-lobby menu (for some reason the eclipse icon size become massive)

                // DifficultyDef logic from RoR2.UI.EclipseRunScreenController.UpdateDisplayedSurvivor()
                int completedLevel = EclipseRun.GetLocalUserSurvivorCompletedEclipseLevel(self.GetLocalUser(), self.survivorDef);
                if (Config.ShowUpcomingLevel) completedLevel++;

                if (completedLevel < EclipseRun.minEclipseLevel) return; // Don't show eclipse icon for survivors that have not beaten any eclipse level

                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(EclipseRun.GetEclipseDifficultyIndex(Mathf.Clamp(completedLevel, EclipseRun.minEclipseLevel, EclipseRun.maxEclipseLevel)));

                if (difficultyDef == null) {
                    Log.LogWarning($"Failed to get {nameof(difficultyDef)} for {self.survivorDef.cachedName}");
                }
                else {
                    RawImage eclipseIcon = GetOrAddEclipseIcon(self.survivorIcon);
                    eclipseIcon.texture = completedLevel > EclipseRun.maxEclipseLevel ? GoldE8Icon : difficultyDef.GetIconSprite().texture;
                    eclipseIcon.gameObject.SetActive(self.survivorIcon.color != Color.black); // Don't show icons for unavailable (silhouetted) characters
                }
            }
            catch (Exception e) {
                Log.LogError(e);
            }
        }

        private RawImage GetOrAddEclipseIcon(RawImage survivorIcon)
        {
            RawImage[] components = survivorIcon.GetComponentsInChildren<RawImage>();
            for (int i = 0; i < components.Length; i++) {
                if (components[i] != survivorIcon) return components[i];
            }

#if DEBUG
            Log.LogDebug($"Adding child \"EclipseIcon\" under \"{survivorIcon.name}\"");
#endif
            GameObject obj = new GameObject("EclipseIcon", typeof(RectTransform));
            obj.transform.SetParent(survivorIcon.transform);
            obj.layer = survivorIcon.gameObject.layer;
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            // bottom-left (//todo: could try make configurable)
            rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;
            rect.sizeDelta = Vector2.one * survivorIcon.rectTransform.rect.width * Config.IconSizePercentage;

            return obj.AddComponent<RawImage>();
        }
    }
}
