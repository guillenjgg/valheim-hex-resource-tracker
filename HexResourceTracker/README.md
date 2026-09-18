# HexResourceTracker

Automatically adds minimap pins for supported gatherable resources and dungeon entrances.

Includes a draggable **Map Tracking** overlay for enabling or disabling individual resources and dungeons in-game.

## Features

- Tracks supported gatherable resources and dungeon entrances.
- Removes resource pins when resources are harvested.
- Restores resource pins when resources respawn.
- Supports two tracking modes:
  - **Zone Based** - Tracks supported objects as Valheim loads the surrounding world.
  - **Range Scanner** - Only displays supported resources and dungeons within a configurable distance of the player.
- Configurable **Range Scanner** distance from 50 to 2000 meters.
- Independently enable or disable each supported resource and dungeon.
- Draggable in-game **Map Tracking** overlay.
- Includes support for Deep North resources and dungeons.
- Resource pins are clustered to reduce map clutter in dense areas.

![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_1.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_2.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_3.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_4.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_5.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_6.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_7.png)

## Tracking Modes

### Zone Based

Tracks supported resources and dungeons as their surrounding zones are loaded by Valheim.

There is no configured distance limit in this mode.

### Range Scanner

Only displays supported resources and dungeons within the configured **Tracking Range** of the player.

Pins are automatically added and removed as the player moves through the world.

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
- Dragon Eggs

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

### Deep North
- Lingonberries
- Kale Seeds

## Tracked Dungeons

- Burial Chambers
- Sunken Crypts
- Frost Caves
- Infested Mines
- Morkhalla
- Winding Tunnels

## Configuration

Tracking can be configured through the BepInEx configuration file or the in-game **Map Tracking** overlay.

Configuration options include:

- Tracking mode
- Range Scanner distance
- Individual resource tracking
- Individual dungeon tracking

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

- Only supported resources and dungeon entrances are tracked.
- Valheim must load an object before the mod can discover and track it.
- Pins do not persist across game sessions.

## Feedback & Support

Report bugs, request features, or provide feedback through Discord:

https://discord.gg/wU2FXD94v4

## Source Code

https://github.com/guillenjgg/valheim-hex-resource-tracker