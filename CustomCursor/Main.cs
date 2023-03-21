using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomCursor
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string ModName = nameof(CustomCursor);
        public const string ModAuthor  = "Mayster911";
        public const string ModGUID = "com.mayster911.customcursor";
        public const string ModVersion = "1.0.0";

        internal static string LoadedDllLocation = null;
        internal Harmony Harmony;
        internal void Awake()
        {
            LoadedDllLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Harmony = new Harmony(ModGUID);

            Harmony.PatchAll();
            Logger.LogInfo($"{ModName} successfully loaded! Made by {ModAuthor}");
        }
    }

    [HarmonyPatch(nameof(LevelController), nameof(LevelController.Start))]
    public static class AddCustomCursorPatch
    {
        private static bool HasLoaded = false;

        public static void Postfix()
        {
            if (HasLoaded)
                return;

            try
            {
                var texture = BuildCrosshairTexture();
                Cursor.SetCursor(texture, GetHotSpot(texture), CursorMode.Auto);
                HasLoaded = true;
            }
            catch
            {
                // Probably shouldn't cause the game to crash with an unhandled exception
            }
        }

        private static Texture2D BuildCrosshairTexture()
        {
            var bytes = File.ReadAllBytes(Path.Combine(Main.LoadedDllLocation, "cursor.png"));
            var texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);

            return texture;
        }

        private static Vector2 GetHotSpot(Texture2D texture2D)
        {
            return new Vector2(
                texture2D.height / 2,
                texture2D.width / 2
            );
        }
    }
}
