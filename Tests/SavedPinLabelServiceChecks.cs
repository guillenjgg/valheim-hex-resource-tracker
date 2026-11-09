// Standalone behavioral checks using lightweight game API doubles.
using System;
using System.Collections.Generic;
using System.Reflection;
using HexResourceTracker.Core.PinManagers;
namespace HarmonyLib { public static class AccessTools {
 public delegate ref F FieldRef<T,F>(T instance);
 public static FieldInfo Field(Type t,string n) { return t.GetField(n); }
 public static MethodInfo Method(Type t,string n) { return t.GetMethod(n,BindingFlags.Instance|BindingFlags.NonPublic); }
 private static ref List<Minimap.PinData> Pins(Minimap map) { return ref map.m_pins; }
 public static FieldRef<T,F> FieldRefAccess<T,F>(string name) { FieldRef<Minimap,List<Minimap.PinData>> value=Pins; return (FieldRef<T,F>)(object)value; }
} }
public class Room { public enum Theme { ForestCrypt,SunkenCrypt,Cave,DvergerTown,MorkHalla,Hole } }
public class Minimap {
 public static Minimap instance; public bool m_pinUpdateRequired;
 public List<PinData> m_pins=new List<PinData>();
 public enum PinType { Icon1,Icon2,Icon3 }
 public class Text { public string text; }
 public class PinData { public string m_name; public bool m_save=true,m_checked; public PinType m_type=PinType.Icon2; public PinNameData m_NamePinData; }
 public class PinNameData { public Text PinNameText=new Text(); public bool Destroyed;
  public PinNameData(PinData p) { PinNameText.text=p.m_name; }
  private void DestroyMapMarker() { Destroyed=true; }
 }
}
namespace HexResourceTracker {
 internal class Entry<T> { internal T Value; public Entry(T value) { Value=value; } }
 internal static class PluginConfig {
  internal static Entry<bool> IsModEnabled=new Entry<bool>(true);
  internal static bool HideDungeon=true,HideLocation=true;
  internal static Dictionary<Room.Theme,Entry<string>> DungeonLabelConfigs=new Dictionary<Room.Theme,Entry<string>>();
  internal static Dictionary<string,Entry<string>> LocationLabelConfigs=new Dictionary<string,Entry<string>>();
  internal static string GetDungeonLabel(Room.Theme theme) { return HideDungeon ? "" : DungeonLabelConfigs[theme].Value; }
  internal static string GetLocationLabel(string prefab) { return HideLocation ? "" : LocationLabelConfigs[prefab].Value; }
 }
}
class Checks {
 static void Check(bool value,string name) { if(!value) throw new Exception(name); }
 static Minimap.PinData Pin(string name,Minimap.PinType type=Minimap.PinType.Icon2,bool save=true) {
  var pin=new Minimap.PinData { m_name=name,m_type=type,m_save=save,m_checked=true };
  pin.m_NamePinData=new Minimap.PinNameData(pin); Minimap.instance.m_pins.Add(pin); return pin;
 }
 static void Main() {
  foreach(var d in HexResourceTracker.Core.TrackedDungeons.AllTrackedDungeons)
   HexResourceTracker.PluginConfig.DungeonLabelConfigs[d.Theme]=new HexResourceTracker.Entry<string>(d.DefaultLabel);
  foreach(var d in HexResourceTracker.Core.TrackedLocations.All)
   HexResourceTracker.PluginConfig.LocationLabelConfigs[d.Key]=new HexResourceTracker.Entry<string>(d.Value);
  Minimap.instance=new Minimap();
  var dungeon=Pin("Sunken Crypt");var location=Pin("Charred Fortress");var memorial=Pin("Memorial Site",Minimap.PinType.Icon3);
  var boss=Pin("Bonemass");var home=Pin("Home");var wrongIcon=Pin("Sunken Crypt",Minimap.PinType.Icon1);var transient=Pin("Sunken Crypt",Minimap.PinType.Icon2,false);
  SavedPinLabelService.Reconcile(Minimap.instance);
  Check(dungeon.m_name=="" && location.m_name=="" && memorial.m_name=="","hide preexisting distant pins without loaded world objects");
  Check(dungeon.m_NamePinData==null && dungeon.m_checked && location.m_type==Minimap.PinType.Icon2,"hide cleans marker and preserves checked state/icon");
  Check(boss.m_name=="Bonemass" && home.m_name=="Home" && wrongIcon.m_name=="Sunken Crypt" && transient.m_name=="Sunken Crypt","unrelated pins untouched");
  HexResourceTracker.PluginConfig.HideDungeon=false;HexResourceTracker.PluginConfig.HideLocation=false;
  HexResourceTracker.PluginConfig.LocationLabelConfigs["CharredFortress"].Value="My Fortress";
  SavedPinLabelService.Reconcile(Minimap.instance);
  Check(dungeon.m_name=="Sunken Crypt" && location.m_name=="My Fortress" && memorial.m_name=="Memorial Site","restore/customize all cached saved pins");
  HexResourceTracker.PluginConfig.HideDungeon=true;HexResourceTracker.PluginConfig.HideLocation=true;
  SavedPinLabelService.Reconcile(Minimap.instance);
  var state=SavedPinLabelService.PrepareForSave(Minimap.instance);
  Check(dungeon.m_name=="Sunken Crypt" && location.m_name=="Charred Fortress","serialize stable identity while hidden");
  var serializedDungeon=dungeon.m_name;var serializedLocation=location.m_name;
  SavedPinLabelService.RestoreAfterSave(state);
  Check(dungeon.m_name=="" && location.m_name=="","restore hidden live state after serialization");
  Minimap.instance=new Minimap();var nextDungeon=Pin(serializedDungeon);var nextLocation=Pin(serializedLocation);
  SavedPinLabelService.Reconcile(Minimap.instance);
  Check(nextDungeon.m_name=="" && nextLocation.m_name=="","next session honors hide setting");
  HexResourceTracker.PluginConfig.HideLocation=false;
  SavedPinLabelService.Reconcile(Minimap.instance);
  Check(nextLocation.m_name=="My Fortress","next session restores configured name from stable identity");
  Minimap.instance.m_pins.Remove(nextLocation);SavedPinLabelService.Reconcile(Minimap.instance);
  Check(Minimap.instance.m_pins.Count==1,"deleted pins are not recreated");
  Console.WriteLine("PASS: restored distant pins, safe matching, hide/show/custom names, serialization, next session, deletion");
 }
}
namespace HexResourceTracker.Core.PinManagers { internal static class LocationPinIconService { internal static void Apply(Minimap.PinData pin, string prefabName) { } } }
