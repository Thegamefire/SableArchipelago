# Archipelago Mod for Sable

This is a mod and apworld for playing Sable with the multiworld randomizer [Archipelago](https://github.com/ArchipelagoMW/Archipelago)

## Installation Instructions

TODO

## Build & Dev Instructions

### World

- Clone the repo
- For development on the APWorld I suggest making a symlink in the worlds folder of a clone of the [archipelago repo](https://github.com/ArchipelagoMW/Archipelago) targetting the World folder of this repo.

Build by creating a zip file of the World folder.

### Mod

- Clone the repo
- Edit the `.csproj` file so the references point to the correct gamefiles

Build with:
```sh
dotnet build
```
