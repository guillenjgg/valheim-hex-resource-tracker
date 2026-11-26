// Standalone checks for resource variants: other prefab names resolve to one definition, one toggle.
using System;
using System.Linq;
using HexResourceTracker.Core;
using HexResourceTracker.Models;
namespace HexResourceTracker.Core { internal static class TarPitResourcePinService { internal const string ResourcePrefabName="TarLiquid"; } internal static class YggdrasilRootPinService { internal const string ResourcePrefabName="YggdrasilRoot"; } }
static class Checks {
 static int failures;
 static void Check(bool ok,string name) { Console.WriteLine((ok?"PASS ":"FAIL ")+name); if(!ok) failures++; }
 static int Main() {
  TrackedResourceDefinitionModel mud;
  Check(TrackedResources.TryGetByPrefabName("mudpile",out mud)&&mud.DisplayName=="Muddy Scrap Pile"
   &&mud.ResourceType==TrackedResourceTypeEnum.Deposit&&mud.IconItemPrefabName=="IronScrap","muddy scrap pile is a deposit");
  foreach(string variant in new[] { "mudpile2","mudpile_beacon" }) {
   TrackedResourceDefinitionModel found;
   Check(TrackedResources.TryGetByPrefabName(variant,out found)&&ReferenceEquals(found,mud),variant+" resolves to the same definition");
  }
  Check(TrackedResources.AllTrackedResources.Count(r=>r.DisplayName=="Muddy Scrap Pile")==1,"one definition, so one toggle and one setting");
  TrackedResourceDefinitionModel obsidian;
  Check(TrackedResources.TryGetByPrefabName("MineRock_Obsidian",out obsidian)&&obsidian.DisplayName=="Obsidian"
   &&obsidian.ResourceType==TrackedResourceTypeEnum.Deposit&&obsidian.IconItemPrefabName=="Obsidian","obsidian is a deposit");
  TrackedResourceDefinitionModel tin;
  Check(TrackedResources.TryGetByPrefabName("MineRock_Tin",out tin)&&tin.AlsoPrefabNames.Length==0,"definitions without variants unchanged");
  Check(TrackedResources.AllTrackedResources.Select(r=>r.ResourcePrefabName)
   .Concat(TrackedResources.AllTrackedResources.SelectMany(r=>r.AlsoPrefabNames)).GroupBy(n=>n).All(g=>g.Count()==1),"no prefab name claimed twice");
  Console.WriteLine(failures==0?"All resource variant checks passed.":failures+" check(s) failed.");
  return failures==0?0:1;
 }
}
