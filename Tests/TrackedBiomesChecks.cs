// Standalone checks for biomes: every tracked item belongs to at least one biome, and the panel shows only the current biome's items.
using System;
using System.Linq;
using HexResourceTracker.Core;
using HexResourceTracker.Models;
public class Heightmap { [Flags] public enum Biome { None = 0, Meadows = 1, Swamp = 2, Mountain = 4, BlackForest = 8, Plains = 16, AshLands = 32, DeepNorth = 64, Ocean = 256, Mistlands = 512 } }
public class Room { public enum Theme { ForestCrypt, SunkenCrypt, Cave, DvergerTown, MorkHalla, Hole } }
public class Minimap { public enum PinType { Icon2, Icon3 } }
namespace HexResourceTracker.Core { internal static class TarPitResourcePinService { internal const string ResourcePrefabName="TarLiquid"; } internal static class YggdrasilRootPinService { internal const string ResourcePrefabName="YggdrasilRoot"; } }
static class Checks {
 static int failures;
 static void Check(bool ok,string name) { Console.WriteLine((ok?"PASS ":"FAIL ")+name); if(!ok) failures++; }
 static int Main() {
  Check(TrackedResources.AllTrackedResources.All(r=>TrackedBiomes.ForResource(r)!=Heightmap.Biome.None),"every resource has a biome");
  Check(TrackedDungeons.AllTrackedDungeons.All(d=>TrackedBiomes.ForDungeon(d.Theme)!=Heightmap.Biome.None),"every dungeon has a biome");
  Check(TrackedCreatures.AllTrackedCreatures.All(c=>TrackedBiomes.ForCreature(c.PrefabName)!=Heightmap.Biome.None),"every creature has a biome");
  Check(TrackedLocations.All.Keys.All(p=>TrackedBiomes.ForLocations(new[] { p })!=Heightmap.Biome.None),"every location has a biome");

  var turnip=TrackedResources.AllTrackedResources.Single(r=>r.ResourcePrefabName=="Pickable_SeedTurnip");
  Check(TrackedBiomes.ForResource(turnip)==Heightmap.Biome.Swamp,"a pickable's biome comes from its biome name");
  var thistle=TrackedBiomes.ForResource(TrackedResources.AllTrackedResources.Single(r=>r.ResourcePrefabName=="Pickable_Thistle"));
  Check(TrackedBiomes.IsShown(thistle,Heightmap.Biome.BlackForest)&&TrackedBiomes.IsShown(thistle,Heightmap.Biome.Swamp),"Thistle shows in the Black Forest and the Swamp");
  var mushrooms=TrackedBiomes.ForResource(TrackedResources.AllTrackedResources.Single(r=>r.ResourcePrefabName=="Pickable_Mushroom"));
  Check(TrackedBiomes.IsShown(mushrooms,Heightmap.Biome.Meadows)&&TrackedBiomes.IsShown(mushrooms,Heightmap.Biome.Swamp),"Mushrooms show in the Meadows and the Swamp");
  var root=TrackedResources.AllTrackedResources.Single(r=>r.ResourcePrefabName=="YggdrasilRoot");
  Check(root.ResourceType==TrackedResourceTypeEnum.Deposit&&root.IconItemPrefabName=="Sap"&&TrackedBiomes.ForResource(root)==Heightmap.Biome.Mistlands,"Yggdrasil roots are a Mistlands deposit with the Sap icon");
  var moulds=TrackedResources.AllTrackedResources.Single(r=>r.ResourcePrefabName==TrackedResources.MouldsKey);
  Check(moulds.ResourceType==TrackedResourceTypeEnum.Container&&TrackedBiomes.ForResource(moulds)==Heightmap.Biome.DeepNorth,"moulds are a Deep North container find");
  Check(TrackedResources.ContainerKeyForItem("SomeMouldPrefab","$item_mold_axe")==TrackedResources.MouldsKey,"any mould in a container counts, by its name token");
  Check(TrackedResources.ContainerKeyForItem("OatSeeds","$item_oat_seeds")=="OatSeeds","other container items still match by prefab name");
  Check(TrackedResources.ContainerKeyForItem("Wood","$item_wood")==null,"an untracked item matches nothing");
  string[] chestOnly={"AxeHead1","AxeHead2","AxeWood","KnifeWood","MaceWood","SpearWood","SledgeWood","BattleaxeWood","AtgeirWood","THSwordWood"};
  Check(chestOnly.All(p=>TrackedResources.TryGetByPrefabName(p,out var d)&&d.ResourceType==TrackedResourceTypeEnum.Container),"the axe heads and wooden weapons are chest finds");
  var spear=TrackedBiomes.ForResource(TrackedResources.AllTrackedResources.Single(r=>r.ResourcePrefabName=="SpearWood"));
  Check(TrackedBiomes.IsShown(spear,Heightmap.Biome.BlackForest)&&TrackedBiomes.IsShown(spear,Heightmap.Biome.Swamp),"the Wooden Spear shows in the Black Forest and the Swamp");
  Check(TrackedBiomes.ForResource(TrackedResources.AllTrackedResources.Single(r=>r.ResourcePrefabName=="THSwordWood"))==Heightmap.Biome.Mistlands,"the Wooden Greatsword shows in the Mistlands");
  Check(TrackedResources.IsWorldContainer(isPlayerBuilt:false,isGrave:false),"a world chest or barrel is looked inside");
  Check(!TrackedResources.IsWorldContainer(isPlayerBuilt:true,isGrave:false),"a player's own chest is never pinned");
  Check(!TrackedResources.IsWorldContainer(isPlayerBuilt:false,isGrave:true),"a grave is never pinned");
  var copper=TrackedResources.AllTrackedResources.Single(r=>r.ResourcePrefabName=="rock4_copper");
  Check(TrackedBiomes.ForResource(copper)==Heightmap.Biome.BlackForest,"a deposit's biome comes from the table");

  var trader=TrackedLocations.Groups().Single(g=>g.Key=="Trader").Value;
  Check(TrackedBiomes.ForLocations(trader)==(Heightmap.Biome.Meadows|Heightmap.Biome.BlackForest|Heightmap.Biome.Swamp),"a shared toggle covers every biome its locations are in");
  var altar=TrackedBiomes.ForLocations(TrackedLocations.Groups().Single(g=>g.Key=="Boss Altar").Value);
  Check(TrackedBiomes.IsShown(altar,Heightmap.Biome.Meadows)&&TrackedBiomes.IsShown(altar,Heightmap.Biome.AshLands),"boss altars show in each boss's biome");
  Check(TrackedLocations.All.ContainsKey("AncientUpgradeStation")&&TrackedBiomes.ForLocations(new[] { "AncientUpgradeStation" })==Heightmap.Biome.Mountain,"the Forge of Potential is a Mountains location");
  Check(TrackedLocations.All["DN_Bossroom"]=="Boss Altar"&&TrackedBiomes.IsShown(altar,Heightmap.Biome.DeepNorth),"the Aesir passage (DN_Bossroom) is the Deep North's boss altar");

  var deer=TrackedBiomes.ForCreature("Deer");
  Check(TrackedBiomes.IsShown(deer,Heightmap.Biome.Meadows)&&TrackedBiomes.IsShown(deer,Heightmap.Biome.BlackForest),"an item in several biomes shows in each");
  Check(!TrackedBiomes.IsShown(deer,Heightmap.Biome.Swamp),"an item is hidden outside its biomes");
  Check(!TrackedBiomes.IsShown(deer,Heightmap.Biome.None),"nothing shows before the biome is known");

  Check(TrackedBiomes.IsShown(TrackedBiomes.ForCreature("Serpent"),Heightmap.Biome.Ocean),"the Serpent shows on the Ocean");
  var leviathan=TrackedResources.AllTrackedResources.Single(r=>r.ResourcePrefabName=="Leviathan");
  Check(TrackedBiomes.IsShown(TrackedBiomes.ForResource(leviathan),Heightmap.Biome.Ocean),"the Leviathan shows on the Ocean");
  var bonemaw=TrackedBiomes.ForCreature("BonemawSerpent");
  Check(bonemaw==Heightmap.Biome.AshLands,"the Bonemaw shows in the Ashlands only (its water, not the open Ocean)");

  Check(Enumerable.Range(1,12).Where(n=>n!=4).All(n=>TrackedCreatures.TryGetDefinition("Fish"+n,out _)),"eleven fish are tracked");
  var puffer=TrackedBiomes.ForCreature("Fish12");
  Check(TrackedBiomes.IsShown(puffer,Heightmap.Biome.Ocean)&&TrackedBiomes.IsShown(puffer,Heightmap.Biome.Mistlands),"the Pufferfish shows on the Ocean and in the Mistlands");
  Check(!TrackedCreatures.TryGetDefinition("Fish4_cave",out _)&&!TrackedCreatures.TryGetDefinition("Bat",out _),"Frost Cave only creatures (Tetra, Bat) are not tracked");
  Check(TrackedBiomes.ForCreature("Leech")==Heightmap.Biome.Swamp,"Leeches show in the Swamp only");
  Check(TrackedCreatures.TryGetDefinition("Draugr_Ranged_sleeping",out var archer)&&archer.PrefabName=="Draugr","a sleeping Draugr archer counts as a Draugr");
  Check(TrackedCreatures.TryGetDefinition("Greydwarf_Frozen",out var frozen)&&frozen.PrefabName=="Greydwarf"
   &&TrackedBiomes.IsShown(TrackedBiomes.ForCreature("Greydwarf"),Heightmap.Biome.DeepNorth),"a frozen Greydwarf counts as a Greydwarf, in the Deep North too");
  Check(TrackedCreatures.TryGetDefinition("JotunWarriorDualWield",out var krigen)&&krigen.DisplayName=="Krigen","both Krigen share one toggle");
  Check(TrackedCreatures.TryGetDefinition("Seagal",out var gull)&&TrackedBiomes.IsShown(TrackedBiomes.ForCreature("Seagal"),Heightmap.Biome.Ocean),"gulls are tracked, on the Ocean too");
  Check(TrackedCreatures.TryGetDefinition("Gjall",out _)&&TrackedBiomes.ForCreature("Gjall")==Heightmap.Biome.Mistlands,"the Gjall is tracked in the Mistlands");
  string[] authorCreatures={"Deer","Boar","Hare","Lox"};
  Check(TrackedCreatures.AllTrackedCreatures.Where(c=>authorCreatures.Contains(c.PrefabName)).All(c=>c.EnabledByDefault),"the author's creatures stay on by default");
  Check(TrackedCreatures.AllTrackedCreatures.Where(c=>!authorCreatures.Contains(c.PrefabName)).All(c=>!c.EnabledByDefault),"every new creature is off by default");
  Check(TrackedCreatures.TryGetDefinition("Bjorn_sleeping",out var sleepingBear)&&sleepingBear.PrefabName=="Bjorn","a sleeping bear counts as a bear");
  Check(TrackedCreatures.TryGetDefinition("Morgen_NonSleeping",out var wakingMorgen)&&wakingMorgen.PrefabName=="Morgen","a waking Morgen counts as a Morgen");
  Check(TrackedCreatures.AllTrackedCreatures.Select(c=>c.PrefabName).Distinct().Count()==TrackedCreatures.AllTrackedCreatures.Count,"no creature is listed twice");

  var near=new[]{ new float[]{0,0}, new float[]{10,0}, new float[]{25,5}, new float[]{200,200} };
  var packs=CreaturePacks.Group(near,p=>p[0],p=>p[1],30f);
  Check(packs.Count==2&&packs[0].Count==3&&packs[1].Count==1,"creatures close together share one pack, a far one stands alone");
  Check(CreaturePacks.Group(near,p=>p[0],p=>p[1],0f).Count==4,"a radius of zero leaves every creature on its own");
  Check(CreaturePacks.Group(new float[0][],p=>p[0],p=>p[1],30f).Count==0,"no creatures, no packs");

  Check(TrackedBiomes.IsShown(TrackedBiomes.MemorialSiteSearch,Heightmap.Biome.DeepNorth),"the memorial site search shows in the Deep North");
  Check(TrackedBiomes.DisplayName(Heightmap.Biome.BlackForest)=="Black Forest"&&TrackedBiomes.DisplayName(Heightmap.Biome.Mountain)=="Mountains"
   &&TrackedBiomes.DisplayName(Heightmap.Biome.AshLands)=="Ashlands"&&TrackedBiomes.DisplayName(Heightmap.Biome.DeepNorth)=="Deep North"
   &&TrackedBiomes.DisplayName(Heightmap.Biome.Ocean)=="Ocean","biome display names");
  Check(TrackedBiomes.DisplayName(Heightmap.Biome.None)=="","no name before the biome is known");
  Console.WriteLine(failures==0?"All biome checks passed.":failures+" check(s) failed.");
  return failures==0?0:1;
 }
}
