using GameNetcodeStuff;
using Unity.Netcode;

namespace TooManySuits.Helper;

public class LocalPlayer
{
    public static PlayerControllerB localPlayer = null!;
    
    public static bool isActive()
    {
        return localPlayer != null;
    }
    
    public static void PlayerControllerStart(PlayerControllerB __instance)
    {
        if (NetworkManager.Singleton.LocalClientId != __instance.playerClientId) return;
        localPlayer = __instance;
    }
}