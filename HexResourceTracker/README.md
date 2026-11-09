# HexResourceTracker

Automatically adds map pins for resources, dungeons, locations, and creatures in Valheim.

## Using the mod

Open the map to use the draggable, collapsible **Map Tracking** panel. Toggle individual resources, dungeons, locations, and creatures on or off.

In the BepInEx configuration, you can:

- Choose **Zone Based** tracking for objects as they load, or **Range Scanner** for objects within **50–2000 meters** of your character.
- Customize or hide dungeon, deposit, and location labels. Changes apply immediately, including to cached pins for unloadeddg locations.

Resource pins disappear when harvested or depleted and return when resources respawn. Nearby resources are grouped to reduce clutter. Dungeon and location pins save with your character’s map; resource and creature pins are temporary.

### Find a Memorial Site

Enable Memorial Site tracking, then click **Find Nearest Memorial Site** under **Locations**. It finds the nearest site, including unvisited ones, and centers the map on its pin.

## What it tracks

### Resources

| Biome | Resources |
| --- | --- |
| Meadows | Mushrooms, raspberries, dandelions |
| Black Forest | Blueberries, thistle, carrot seeds, copper |
| Swamp | Turnip seeds |
| Mountains | Silver, dragon eggs, onion seeds in Mountain chests |
| Plains | Wild flax, wild barley, cloudberries |
| Mistlands | Jotun puffs, magecaps, giant skulls (soft tissue) |
| Ashlands | Vineberries, fiddleheads, smoke puffs, flametal |
| Deep North | Lingonberries, kale seeds, oat seeds, Poteitr seeds |

### Dungeons

| Biome | Dungeons |
| --- | --- |
| Black Forest | Burial Chambers |
| Swamp | Sunken Crypts |
| Mountains | Frost Caves |
| Mistlands | Infested Mines |
| Deep North | Morkhalla, Winding Tunnels |

### Locations

| Biome | Locations | Map icon |
| --- | --- | --- |
| Ashlands | Charred Fortresses | Castle |
| Deep North | Memorial Sites | Runestone |

### Creatures

| Biome | Creatures |
| --- | --- |
| Meadows | Deer, boars |
| Mistlands | Hares |

## Installation

Install through Thunderstore/r2modman, or install BepInEx and copy the mod into `BepInEx/plugins`.

## Notes

- Client-side mod. Multiplayer and dedicated-server compatibility has not been fully tested.
- Automatic discovery requires objects to load nearby. The Memorial Site search is the exception.

## Screenshots

<details>
<summary>View screenshots</summary>

![Map Tracking 1](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_1.png)

![Map Tracking 2](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_2.png)

![Map Tracking 3](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_3.png)

![Map Tracking 4](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_4.png)

![Map Tracking 5](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_5.png)

![Map Tracking 6](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_6.png)

![Map Tracking 7](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_7.png)

![Map Tracking 8](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_8.png)

![Map Tracking 9](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexresourcetracker/hexresourcedgtracker_9.png)

</details>

## Links and credits

- [Support and feedback](https://discord.gg/wU2FXD94v4)
- [Source code](https://github.com/guillenjgg/valheim-hex-resource-tracker)
- Castle and rune stone icons by Lorc, from Game-icons.net, under CC BY 3.0. [Icon credits and license](THIRD_PARTY_NOTICES.md).