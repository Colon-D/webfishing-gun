using GDWeave;

namespace webfishing_gun;

public class Mod : IMod {
    public Config Config;

    public Mod(IModInterface modInterface) {
        Config = modInterface.ReadConfig<Config>();

        if (Config.DebugAttackSelf) {
			modInterface.RegisterScriptMod(new DebugAttackSelfPatch());
		}
		if (Config.RespawnOnAttackedWithGun) {
		    modInterface.RegisterScriptMod(new RespawnOnAttackedWithGunPatch());
        }
	}

    public void Dispose() {
    }
}
