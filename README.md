# Mevi SKins

Convert skins from other games to fluXis skins.

## How to Install & Use

1. Go to [Releases](https://github.com/menvae/Mevi-Skins/releases/latest) and download ``fluXis.menvae.MeviSkins.dll``

2. Go to plugins in your fluXis local directory. It can be either in ``%APPDATA%/fluXis/plugins`` if you're on windows or ``~/.local/share/fluXis/plugins`` on linux (may differ)

3. Put ``fluXis.menvae.MeviSkins.dll`` in that directory and you're basically done.

4. Now just drag any .osk file into the game and it'll import!

### Features (so far)

- Convert from Osu! skins

### Todo

- [ ] Convert from Quaver skins

## Building (Plugin)

You have to download the source code by cloning the repository using git:

```shell
git clone https://github.com/menvae/Mevi-Skins
```

Make sure the [fluXis repo](https://github.com/InventiveRhythm/fluXis) is cloned next to where the Mevi Skins repo is.

> Your filesystem should look like this if done correctly:
> ```
> ~/GitHub/InventiveRhythm> ls
> fluXis
>   fluXis/
>   fluXis.Resources/
>   fluXis.sln
>   ...
> Mevi-Skins
>   menvae.MeviSkins/
>   menvae.MeviSkins.NativeLibs/
>   Mevi Skins.sln
>   ...
> ```

and finally building:

```sh
dotnet build --project menvae.MeviSkins
```

It's really important to build this in debug and NOT in Release, reason for that being is that I have no idea why everything goes south when you do..

## Building (Bindings) \[optional]

Not really important since I took the courtesy of doing it.

inside rgskin_wrapper: (yes this is rust)
```sh
cargo build --release
```

this will build the lib at ``target/rgskin.dll`` or ``target/librgskin.so`` for linux;
The Bindings will be generated a directory up in the ``RGSkin.Bindings`` project.
You'll have to manually copy the built binaries to ``menvae.MeviSkins.NativeLibs`` though.

---

Any bugs or behavioural issues (eg. something feels wrong) you encounter regarding conversion will have to be taken to the [rgskin](https://github.com/R2O3/rgskin) repo, Unless the issue is actually related to the plugin itself.