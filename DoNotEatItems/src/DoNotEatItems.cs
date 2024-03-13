using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace DoNotEatItems
{
    [BepInPlugin(GUID, NAME, VERSION)]
    internal class DoNotEatItems : BaseUnityPlugin
    {
        public const string GUID = "mattymatty.DoNotEatItems";
        public const string NAME = "DoNotEatItems";
        public const string VERSION = "2.0.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            try
            {
                Log.LogInfo("Patching Methods");
                var harmony = new Harmony(GUID);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Log.LogInfo(NAME + " v" + VERSION + " Loaded!");
            }
            catch (Exception ex)
            {
                Log.LogError("Exception while initializing: \n" + ex);
            }
        }

    }
}