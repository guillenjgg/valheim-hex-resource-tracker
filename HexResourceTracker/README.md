# HexResourceTracker

Automatically adds minimap pins for supported gatherable resources, dungeon entrances and tracked creatures.

Includes a draggable **Map Tracking** overlay for enabling or disabling individual resources and dungeons in-game.

## Features

- Tracks supported gatherable resources, dungeon entrances and certain creatures.
- Removes resource pins when resources are harvested or otherwise depleted.
- Restores resource pins when resources respawn.
- Tracks **Onion Seeds found inside Mountain chests** and removes the pin when the seeds are taken.
- Supports two tracking modes:
  - **Zone Based** - Tracks supported objects as Valheim loads the surrounding world.
  - **Range Scanner** - Only displays supported resources and dungeons within a configurable distance of the player.
- Configurable **Range Scanner** distance from 50 to 2000 meters.
- Independently enable or disable each supported resource and dungeon.
- Draggable in-game **Map Tracking** overlay.
- Includes support for Deep North resources and dungeons.
- Resource pins are clustered to reduce map clutter in dense areas.
- Selected resource icons use larger map pins for improved visibility.
- Onion Seed pins use a gold border to make them easier to spot on the map.

![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_1.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_2.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_3.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_4.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_5.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_6.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_7.png)

## Tracking Modes

### Zone Based
* Zone Based - Tracks supported objects as Valheim loads surrounding zones, without a configured distance limit.

* Range Scanner - Automatically adds and removes pins based on the player's configured tracking range. The tracking range is based on the players current position.

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
- Onion Seeds found in Mountain chests

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
- Oat Seeds
- Poteitr Seeds

## Tracked Creatures

### Creatures
- Deer
- Boars
- Hares

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

* Tracking mode
* Range Scanner distance
* Individual resource tracking
* Individual dungeon tracking
* Individual creature tracking

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

- Valheim must load an object before the mod can discover and track it.
- Onion Seeds are tracked through naturally generated Mountain chests, not dropped or player-planted Onion Seed items.
- Pins do not persist across game sessions.

## Feedback & Support

Report bugs, request features, or provide feedback through Discord:

https://discord.gg/wU2FXD94v4

## Source Code

https://github.com/guillenjgg/valheim-hex-resource-tracker