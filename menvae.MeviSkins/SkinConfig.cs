using fluXis.Plugins;
using osu.Framework.Platform;

namespace menvae.MeviSkins;

public class SkinPluginConfig : PluginConfigManager<SkinPluginSetting>
{
    protected override string ID => "mevi_skins";

    public SkinPluginConfig(Storage storage)
        : base(storage)
    {
    }

    protected override void InitialiseDefaults()
    {
        SetDefault(SkinPluginSetting.ExportLayouts, false);
    }
}

// I was going to add auto import support but I realized this is going to be a pain since we are already doing this in a roundabout way
public enum SkinPluginSetting
{
    ExportLayouts,
    SkinLocations,
}
