using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;

namespace DoNotEatItems.Patches
{
    [HarmonyPatch]
    internal class DropItemOnDeathPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayerServerRpc))]
        private static void DropItems(PlayerControllerB __instance)
        {
            NetworkManager networkManager = __instance.NetworkManager;
            if (networkManager == null || !networkManager.IsListening)
                return;
            if (__instance.__rpc_exec_stage != NetworkBehaviour.__RpcExecStage.Server || !networkManager.IsServer && !networkManager.IsHost)
                return;
            __instance.DropAllHeldItemsServerRpc();
        }
    }
}