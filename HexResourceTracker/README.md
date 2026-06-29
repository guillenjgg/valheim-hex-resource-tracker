# HexResourceTracker

Automatically adds minimap resource pins for berries, mushrooms, thistle, seeds, and other gatherables. Includes a custom draggable map overlay that lets players toggle resource tracking on and off without leaving the game.

## Features

- Automatically adds minimap pins for tracked resources.
- Pins are removed when resources are harvested.
- Pins are automatically restored when resources respawn.
- Configurable resource tracking.
- Draggable map overlay for managing tracked resources in-game. Click and drag the overlay title bar to move it around.

![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_1.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_2.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_3.png)
![Tracking Overlay](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcetracker_4.png)

## Tracked Resources

### Meadows
- Mushrooms
- Raspberries
- Dandelions

### Black Forest
- Blueberries
- Thistle
- Carrot Seeds

### Swamp
- Turnip Seeds

### Plains
- Wild Flax
- Wild Barley
- Cloudberries

### Mistlands
- Jotun Puffs
- Magecaps

## Configuration

The following resources can be individually enabled or disabled:

- Mushrooms
- Raspberries
- Blueberries
- Thistle
- Carrot Seeds
- Turnip Seeds
- Wild Flax
- Wild Barley
- Jotun Puffs
- Dandelions

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
- Resources must be loaded by Valheim before pins can be displayed.
- Pins do not persist across game sessions.
- Pins are clustered to reduce map clutter in dense resource areas.

## Feedback & Support

Report bugs, request features, or provide feedback through Discord:

https://discord.gg/wU2FXD94v4

## Source Code

https://github.com/guillenjgg/valheim-hex-resource-tracker