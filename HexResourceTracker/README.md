# HexResourceTracker

Automatically adds minimap pins for gatherable resources such as berries, mushrooms, thistle, seeds, ore deposits, and other harvestable materials.

Automatically adds minimap pins for supported dungeon entrances.

Includes a custom draggable **Map Tracking** overlay that lets players enable or disable resource and dungeon tracking without leaving the game.

## Features

- Automatically adds minimap pins for supported gatherable resources and dungeon entrances.
- Resource pins are removed when resources are harvested.
- Resource pins are automatically restored when resources respawn.
- Independently configure which resources and dungeons are tracked.
- Draggable in-game **Map Tracking** overlay for enabling or disabling resource and dungeon tracking without opening the configuration file.

![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_1.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_2.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_3.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_4.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_5.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_6.png)

## Tracked Resources

### Meadows
- Mushrooms
- Raspberries
- Dandelions

### Black Forest
- Blueberries
- Thistle
- Carrot Seeds
- Copper

### Swamp
- Turnip Seeds

### Mountains
- Silver Veins

### Plains
- Wild Flax
- Wild Barley
- Cloudberries

### Mistlands
- Jotun Puffs
- Magecaps
- Giant Skulls (Soft Tissue)

### Ashlands
- Vineberries
- Fiddleheads
- Smoke Puffs
- Flametal Deposits

## Tracked Dungeons

- Burial Chambers
- Sunken Crypts
- Frost Caves
- Infested Mines

## Configuration

Tracking can be configured through either:

- BepInEx configuration file
- In-game **Map Tracking** overlay

## Installation

### Manual Installation

1. Install BepInEx for Valheim.
2. Extract the mod into your Valheim `BepInEx/plugins` folder.
3. Launch the game.

### Thunderstore / r2modman

1. Install using Thunderstore Mod Manager or r2modman.
2. Launch the game.

## Compatibility

- Client-side only.
- Has not been tested in multiplayer or on dedicated servers.

## Known Limitations

- Only currently supported resources and dungeon entrances are tracked.
- Pins appear as the surrounding world is loaded by the game.
- Pins do not persist across game sessions.
- Resource pins are clustered to reduce map clutter in dense areas.

## Feedback & Support

Report bugs, request features, or provide feedback through Discord:

https://discord.gg/wU2FXD94v4

## Source Code

https://github.com/guillenjgg/valheim-hex-resource-tracker