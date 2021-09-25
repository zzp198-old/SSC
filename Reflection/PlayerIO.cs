using System;
using System.Reflection;

namespace SSC.Reflection
{
    public static class PlayerIO
    {
        private static Type GetPlayerIO() => Assembly.Load("tModLoader").GetType("Terraria.ModLoader.IO.PlayerIO");
        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        private static object Invoke(string name, object obj, object[] parameters)
        {
            var method = GetPlayerIO().GetMethod(name, BindingAttr);
            return method?.Invoke(obj, parameters);
        }

        private static object Invoke(string name, object[] parameters) => Invoke(name, null, parameters);
    }
}