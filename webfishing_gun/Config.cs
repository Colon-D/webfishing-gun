using System.Text.Json.Serialization;

namespace webfishing_gun;

public class Config {
    [JsonInclude] public bool DebugAttackSelf = false;
    [JsonInclude] public bool RespawnOnAttackedWithGun = true;
}
