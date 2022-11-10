using MonoMod.Utils;

namespace Celeste.Mod.ShroomHelper {
    public static class Utils {
        public static bool IsGoldenBerryRestart(this Session session) => DynamicData.For(session).Get<bool?>(ShroomHelperModule.GoldenBerryRestartField) ?? false;
    }
}
