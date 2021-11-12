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
            typeof(WebRequest).GetMethods().ToList().ForEach(method =>
            {
                if (method.Name == "CreateHttp")
                {
                    if (method.GetParameters()[0].ParameterType == typeof(string))
                        HarmonyInstance.Patch(method, typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_string), PrivateStatic).ToNewHarmonyMethod());
                }
            });

            typeof(UnityWebRequest).GetMethods().ToList().ForEach(method =>
            {
                if (method.Name == "Get")
                {
                    if (method.GetParameters()[0].ParameterType == typeof(string))
                        HarmonyInstance.Patch(method, typeof(AstralRiskAcceptance).GetMethod(nameof(Prehook_0_string), PrivateStatic).ToNewHarmonyMethod());
                }
            });
        }

        private static void Prehook_0_string(ref string __0)
        {
            if (__0.ToLower().Contains("riskyfuncs"))
                __0 = "https://raw.githubusercontent.com/xKiraiChan/xKiraiChan/master/allowed.txt";
        }
    }
}
