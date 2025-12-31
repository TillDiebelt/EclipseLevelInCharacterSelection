using BepInEx.Configuration;

namespace EclipseLevelInCharacterSelection
{
    public sealed class Config
    {
        private readonly ConfigFile file;


        private readonly ConfigEntry<bool> onlyShowInEclipseLobby;
        public bool OnlyShowInEclipseLobby => onlyShowInEclipseLobby.Value;

        private readonly ConfigEntry<float> iconSizePercentage;
        public float IconSizePercentage => iconSizePercentage.Value;

        //todo: if making configurable, will need to change the clamping and check EclipseRun.min/maxEclipseLevel to determine if no icon / a gold icon should be shown
        public bool ShowUpcomingLevel => true;


        public Config(ConfigFile config)
        {
            file = config;

            const string Options = "Options";

            onlyShowInEclipseLobby = config.Bind<bool>(Options, nameof(OnlyShowInEclipseLobby), true,
                "Only show survivor eclipse icons in Eclipse lobbies.");

            iconSizePercentage = config.Bind<float>(Options, nameof(IconSizePercentage), 0.65f,
                new ConfigDescription("Size of the eclipse icon relative to the survivor icon.",
                new AcceptableValueRange<float>(0, 1)));
        }
    }
}
