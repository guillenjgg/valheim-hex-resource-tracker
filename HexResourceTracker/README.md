# HexResourceTracker

Automatically adds minimap resource pins for berries, mushrooms, thistle, seeds, ore deposits, and other gatherable resources. Includes a custom draggable map overlay that lets players toggle resource tracking on and off without leaving the game.

## Features

- Automatically adds minimap pins for tracked resources, including pickables and ore deposits.
- Pins are removed when resources are harvested.
- Pins are automatically restored when resources respawn.
- Configurable resource tracking.
- Draggable map overlay for managing tracked resources in-game. Click and drag the overlay title bar to move it around.

![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_1.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_2.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_3.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_4.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_5.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_6.png)

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

## Configuration

Settings can be configured through:

- BepInEx configuration file
- In-game draggable map overlay

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
- Has not been tested in multiplayer, or on a dedicated server.

## Known Limitations

- Only tracks currently supported gatherable resources.
- Resources appear on the map as their zones are loaded by the game.
- Pins do not persist across game sessions.
- Pins are clustered to reduce map clutter in dense resource areas.

## Feedback & Support

Report bugs, request features, or provide feedback through Discord:

https://discord.gg/wU2FXD94v4

## Source Code

https://github.com/guillenjgg/valheim-hex-resource-tracker