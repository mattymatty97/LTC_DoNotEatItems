using System.Runtime.CompilerServices;
using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;

namespace DoNotEatItems.Dependency
{
    public static class LobbyCompatibilityChecker
    {
        public static bool Enabled { get { return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("BMX.LobbyCompatibility"); } }
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void Init()
        {
            PluginHelper.RegisterPlugin(DoNotEatItems.GUID, System.Version.Parse(DoNotEatItems.VERSION), CompatibilityLevel.ServerOnly, VersionStrictness.Minor);
        }
        
    }
}