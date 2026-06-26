# Exclaim Everything!

A RitsuLib-based Slay the Spire 2 mod that makes displayed text more enthusiastic!

## Features!

- Replaces English sentence periods with exclamation marks!
- Replaces Chinese full stops with Chinese exclamation marks!
- Preserves decimal points such as `1.5` and `0.4.38`!
- Preserves BBCode image tags so inline icons keep rendering!
- Optionally appends a missing final exclamation mark at line breaks or text endings!
- Registers a RitsuLib settings page for the optional missing-mark behavior!

## Requirements!

- Slay the Spire 2 `0.107.1` or newer!
- RitsuLib `0.4.38` or newer!

## Build!

```powershell
dotnet build .\STS2-ExclaimEverything.csproj
```

The project is DLL-only! The build target copies the DLL and `mod_manifest.json` into the local Slay the Spire 2 `mods` directory configured in the project file!

## License!

This project is licensed under the GNU Affero General Public License v3.0 or later! See [LICENSE](LICENSE)!
