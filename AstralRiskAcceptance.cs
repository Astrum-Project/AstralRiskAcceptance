using MelonLoader;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine.Networking;

[assembly: MelonInfo(typeof(Astrum.AstralRiskAcceptance), "AstralRiskAcceptance", "0.1.0", downloadLink: "github.com/Astrum-Project/AstralRiskAcceptance")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum
{
    public class AstralRiskAcceptance : MelonMod
    {
        const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        public override void OnApplicationStart()
        {
            TryHook("WebRequest::CreateHttp (Uri)",
                typeof(WebRequest).GetMethod(nameof(WebRequest.CreateHttp), new Type[1] { typeof(Uri) }),
                typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_Uri), PrivateStatic).ToNewHarmonyMethod()
            );

            TryHook("UnityWebRequest::Get (String)",
                typeof(UnityWebRequest).GetMethod(nameof(UnityWebRequest.Get), new Type[] { typeof(string) }),
                typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_string), PrivateStatic).ToNewHarmonyMethod()
            );

            TryHook("VRChatUtilityKit::EndEmmCheck",
                AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(x => x.GetName().Name == "VRChatUtilityKit")
                    .GetTypes()
                    .FirstOrDefault(x => x.Name == "VRCUtils")
                    .GetProperty("AreRiskyFunctionsAllowed")
                    .GetSetMethod(true),
                typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_bool), PrivateStatic).ToNewHarmonyMethod()
            );
        }

        private void TryHook(string name, MethodInfo method, HarmonyLib.HarmonyMethod pre, HarmonyLib.HarmonyMethod post = null)
        {
            try
            {
                if (method is null)
                {
                    MelonLogger.Msg("Skipping " + name);
                    return;
                }

                HarmonyInstance.Patch(method, pre, post);
                MelonLogger.Msg("Hooked " + name);
            } 
            catch { MelonLogger.Warning("Failed to hook " + name); }
        }

        private static void Prehook_0_string(ref string __0)
        {
            if (__0.ToLower().Contains("riskyfuncs"))
                __0 = "https://raw.githubusercontent.com/xKiraiChan/xKiraiChan/master/allowed.txt";
        }

        private static void Prehook_0_Uri(ref Uri __0)
        {
            if (__0.AbsoluteUri.ToLower().Contains("riskyfuncs"))
                __0 = new Uri("https://raw.githubusercontent.com/xKiraiChan/xKiraiChan/master/allowed.txt");
        }

        private static void Prehook_0_bool(ref bool __0) => __0 = true;
    }
}
