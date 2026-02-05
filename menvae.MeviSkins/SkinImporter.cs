using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using fluXis.Import;
using fluXis.Overlay.Notifications;
using fluXis.Skinning;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Framework.Platform;
using RGSkin.Bindings;

namespace menvae.MeviSkins;

// for my own sanity I will not talk about any reflection in this class...

[UsedImplicitly]
public class SkinImporter : MapImporter
{
    public override string[] FileExtensions => [".osk"];
    public override string GameName => "skin not";

    #nullable enable
    private Storage? storage;
    private SkinManager? skinManager = null;
    #nullable disable

    private readonly Bindable<bool> exportLayouts;

    public SkinImporter(SkinPluginConfig config)
    {
        storage = GetStorage();
        skinManager = GetSkinManager(false);
        exportLayouts = config.GetBindable<bool>(SkinPluginSetting.ExportLayouts);
    }

    #nullable enable
    private Storage? GetStorage()
    {
        var storageProperty = typeof(MapImporter).GetProperty("Storage", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        return storageProperty?.GetValue(this) as Storage;
    }

    private SkinManager? GetSkinManager(bool log = true)
    {
        try
        {
            // MapImporter.MapStore (internal)
            var mapStoreProperty = typeof(MapImporter).GetProperty("MapStore", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var mapStore = mapStoreProperty?.GetValue(this);
            
            if (mapStore == null)
            {
                if (log) Logger.Error(null, "MeviSkins: Failed to get MapStore");
                return null;
            }
            
            // MapImporter.MapStore.game (private)
            var mapStoreType = mapStore.GetType();
            var gameField = mapStoreType.GetField("<game>k__BackingField", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var game = gameField?.GetValue(mapStore);
            
            if (game == null)
            {
                if (log) Logger.Error(null, "MeviSkins: Failed to get game from MapStore");
                return null;
            }
            
            // MapImporter.MapStore.game.SkinManager (protected)

            var gameType = game.GetType();
            
            var skinManagerProperty = gameType.GetProperty("SkinManager", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (skinManagerProperty != null)
            {
                return skinManagerProperty.GetValue(game) as SkinManager;
            }
            
            var skinManagerField = gameType.GetField("<SkinManager>k__BackingField", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (skinManagerField != null)
            {
                return skinManagerField.GetValue(game) as SkinManager;
            }
            
            skinManagerField = gameType.GetField("SkinManager", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            return skinManagerField?.GetValue(game) as SkinManager;
        }
        catch (Exception e)
        {
            Logger.Error(e, "MeviSkins: Failed to get SkinManager via reflection");
            return null;
        }
    }
    #nullable disable

    private void ScheduleReloadSkinList(SkinManager manager)
    {
        try
        {
            var scheduleMethod = manager.GetType()
                .GetMethod("Schedule", 
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public,
                    null,
                    [typeof(Action)],
                    null);
            
            if (scheduleMethod == null)
            {
                Logger.Error(null, "MeviSkins: Failed to find Schedule method");
                return;
            }

            Action reloadAction = () => manager.ReloadSkinList();
            
            scheduleMethod.Invoke(manager, [reloadAction]);
        }
        catch (Exception e)
        {
            Logger.Error(e, "MeviSkins: Failed to schedule ReloadSkinList");
        }
    }

    public override void Import(string path)
    {
        if (!File.Exists(path))
            return;

        storage ??= GetStorage();
        skinManager ??= GetSkinManager();

        if (storage == null)
        {
            Logger.Log("MeviSkins: Skin storage is still null vro.");
        }

        if (skinManager == null)
        {
            Logger.Log("MeviSkins: Skin manager is still null vroooooooo.");
        }

        var skinStorage = storage.GetStorageForDirectory("skins");
        var layoutDir = storage.GetStorageForDirectory("layouts").GetFullPath("");

        var notification = CreateNotification();

        try
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var fileExt = Path.GetExtension(path);
            var folder = CreateTempFolder(fileName);

            var archive = ZipFile.OpenRead(path);

            archive.ExtractToDirectory(folder);

            archive.Dispose();

            switch (fileExt)
            {
                case ".osk":
                    Converter.ConvertFromOsu(folder, skinStorage.GetFullPath(""), exportLayouts.Value, layoutDir);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("No Converter found for this skin.");
            }

            ScheduleReloadSkinList(skinManager);

            notification.State = LoadingState.Complete;
            CleanUp(folder);
        }
        catch (Exception e)
        {
            notification.State = LoadingState.Failed;
            Logger.Error(e, "MeviSkins: Error while importing 3rd-party skin.");
        }
    }
}