using MelonLoader;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using VRC.Core;

[assembly: MelonInfo(typeof(Astrum.AstralRiskAcceptance), "AstralRiskAcceptance", "0.5.1", downloadLink: "github.com/Astrum-Project/AstralRiskAcceptance")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]

namespace Astrum
{
    public class AstralRiskAcceptance : MelonMod
    {
        const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        public override void OnApplicationStart()
        {
            TryHook("WebRequest.CreateHttp",
                typeof(WebRequest).GetMethod(nameof(WebRequest.CreateHttp), new Type[1] { typeof(Uri) }),
                typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_Uri), PrivateStatic).ToNewHarmonyMethod()
            );

            TryHook("WebClient.DownloadString",
                typeof(WebClient).GetMethod(nameof(WebClient.DownloadString), new Type[1] { typeof(string) }),
                typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_string), PrivateStatic).ToNewHarmonyMethod()
            );
            
            TryHook("ApiWorld.tags",
                typeof(ApiWorld).GetProperty(nameof(ApiWorld.tags)).GetSetMethod(), 
                typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_Tags), PrivateStatic).ToNewHarmonyMethod()
            );
        }

        public override void OnSceneWasLoaded(int _, string __)
        {
            if (UnityEngine.GameObject.Find("eVRCRiskFuncEnable") == null)
                UnityEngine.Object.DontDestroyOnLoad(new UnityEngine.GameObject("eVRCRiskFuncEnable"));

            if (UnityEngine.GameObject.Find("UniversalRiskyFuncEnable") == null)
                UnityEngine.Object.DontDestroyOnLoad(new UnityEngine.GameObject("UniversalRiskyFuncEnable"));

            UnityEngine.GameObject disabler;
            if ((disabler = UnityEngine.GameObject.Find("eVRCRiskFuncDisable")) != null)
                UnityEngine.Object.Destroy(disabler);

            if ((disabler = UnityEngine.GameObject.Find("UniversalRiskyFuncDisable")) != null)
                UnityEngine.Object.Destroy(disabler);
        }

        private void TryHook(string name, MethodBase method, HarmonyLib.HarmonyMethod pre, HarmonyLib.HarmonyMethod post = null)
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

        private static void Prehook_0_Tags(ref List<string> __0) => __0 = new List<string>();
    }
}
