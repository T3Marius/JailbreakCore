namespace LastRequests;

public class LRConfig
{
    public KnifeFight_LR KnifeFight { get; set; } = new();
    public GrenadeWar_LR GrenadeWar { get; set; } = new();
}
public class KnifeFight_LR
{
    public bool Enable { get; set; } = true;
}
public class GrenadeWar_LR
{
    public bool Enable { get; set; } = true;
    public int GrenadeCount { get; set; } = 3;
    public float ThrowDelay { get; set; } = 2.0f;
}
