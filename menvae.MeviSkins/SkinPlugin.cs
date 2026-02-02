using System;
using System.Collections.Generic;
using fluXis.Import;
using fluXis.Overlay.Settings.UI;
using fluXis.Plugins;
using menvae.MeviSkins.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Platform;

namespace menvae.MeviSkins;

public class SkinPlugin : Plugin
{
    public override string Name => "Mevi Skins";
    public override string Author => "menvae";
    public override Version Version => new(0, 0, 2);

    private SkinPluginConfig config;

    protected override MapImporter CreateImporter() => new SkinImporter(config);
    public override void CreateConfig(Storage storage) => config = new SkinPluginConfig(storage);
    public override List<SettingsItem> CreateSettings() => new()
    {
        new TheShiggyWiggy() { Padding = new MarginPadding { Bottom = 10 } },
    };
}