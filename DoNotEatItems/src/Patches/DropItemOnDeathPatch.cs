using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

namespace DoNotEatItems.Patches
{
    [HarmonyPatch]
    internal class DropItemOnDeathPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayerServerRpc))]
        private static IEnumerable<CodeInstruction> KillPlayerServerRpcPatch(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            var methodInfo = typeof(PlayerControllerB).GetMethod(nameof(PlayerControllerB.KillPlayerClientRpc), BindingFlags.NonPublic | BindingFlags.Instance);
            
            var startIndex = -1;
            for (var i = 0; i < codes.Count; i++)
            {
                var curr = codes[i];
                if (curr.IsLdarg(2))
                {
                    var next = codes[i + 1];
                    if (next.Branches(out _))
                    {
                        startIndex = i + 2;
                    }
                }

                if (startIndex != -1 && curr.Calls(methodInfo))
                {
                    var labelIndex = i - 6;

                    if (codes[labelIndex].labels.Count > 0)
                    {
                        var label = codes[labelIndex].labels[0];
                        codes.Insert(startIndex, new CodeInstruction(OpCodes.Br, label)
                        {
                            labels = codes[startIndex].labels,
                            blocks = codes[startIndex].blocks
                        });
                        DoNotEatItems.Log.LogDebug("KillPlayerServerRpc Patched!");
                        break;
                    }
                }
            }
            return codes;
        }
        
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
        private static IEnumerable<CodeInstruction> KillPlayerPatch(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            var methodInfo = typeof(PlayerControllerB).GetMethod(nameof(PlayerControllerB.DropAllHeldItems));
            var replacementMethodInfo = typeof(PlayerControllerB).GetMethod(nameof(PlayerControllerB.DropAllHeldItemsAndSync));

            for (var i = 0; i < codes.Count; i++)
            {
                var curr = codes[i];
                if (curr.Calls(methodInfo))
                {
                    codes[i - 2] = new CodeInstruction(OpCodes.Nop)
                    {
                        labels = codes[i - 2].labels,
                        blocks = codes[i - 2].blocks
                    };
                    codes[i - 1] = new CodeInstruction(OpCodes.Nop)
                    {
                        labels = codes[i - 1].labels,
                        blocks = codes[i - 1].blocks
                    };
                    codes[i] = new CodeInstruction(OpCodes.Call, replacementMethodInfo)
                    {
                        labels = codes[i].labels,
                        blocks = codes[i].blocks
                    };
                    DoNotEatItems.Log.LogDebug("KillPlayer Patched!");
                    break;
                }
            }

            return codes;
        }
    }
}