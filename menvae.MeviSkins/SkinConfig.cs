using fluXis.Plugins;
using osu.Framework.Platform;

namespace menvae.MeviSkins;

public class SkinPluginConfig : PluginConfigManager<SkinPluginSetting>
{
    protected override string ID => "menvae_skin_converter";

    public SkinPluginConfig(Storage storage)
        : base(storage)
    {
    }

    protected override void InitialiseDefaults()
    {
    }
}

// I was going to add auto import support but I realized this is going to be a pain since we are already doing this in a roundabout way
public enum SkinPluginSetting
{
    SkinLocations,
}
