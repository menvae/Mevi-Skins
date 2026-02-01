use std::fs;
use std::path::Path;

fn main() {
    csbindgen::Builder::default()
        .input_extern_file("src/lib.rs")
        .csharp_dll_name("rgskin")
        .csharp_namespace("RGSkin.Bindings")
        .csharp_class_accessibility("public")
        .generate_csharp_file("../RGSkin.Bindings/Convert.g.cs")
        .unwrap();

    let csproj_content = r#"<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>RGSkin.Bindings</RootNamespace>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>"#;

    let csproj_path = Path::new("../RGSKin.Bindings/RGSkin.Bindings.csproj");
    fs::write(csproj_path, csproj_content)
        .expect("Failed to write .csproj file");

    println!("cargo:rerun-if-changed=src/lib.rs");
}