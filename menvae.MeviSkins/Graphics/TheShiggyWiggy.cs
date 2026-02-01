using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Textures;
using osuTK;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace menvae.MeviSkins.Graphics;

public partial class TheShiggyWiggy : SettingsItem
{
    private TextureAnimation animation;

    protected override Drawable CreateContent()
    {
        return animation = new TextureAnimation
        {
            Size = new Vector2(100),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };
    }

    [BackgroundDependencyLoader]
    private void load(IRenderer renderer)
    {
        // idk a better way of doing this

        var assembly = typeof(TheShiggyWiggy).Assembly;

        using var stream = assembly.GetManifestResourceStream("menvae.MeviSkins.shiggy.gif");
        
        using var image = Image.Load<Rgba32>(stream);

        for (var i = 0; i < image.Frames.Count; i++)
        {
            var frame = image.Frames[i];
            var meta = frame.Metadata.GetGifMetadata();

            var clone = image.Frames.CloneFrame(i);
            var texUpload = new TextureUpload(clone);

            var texture = renderer.CreateTexture(texUpload.Width, texUpload.Height);
            texture.SetData(texUpload);

            animation.AddFrame(texture, meta.FrameDelay * 10);
        }
    }

    protected override void Reset() { }
}