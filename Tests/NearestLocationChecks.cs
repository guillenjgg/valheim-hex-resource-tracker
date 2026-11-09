// Standalone search behavior checks using lightweight game API doubles.
using System;
using System.Collections.Generic;
using System.Reflection;
using HexResourceTracker.Core.PinManagers;
using HexResourceTracker.Core.Tracking.Monoscripts;
namespace HarmonyLib { public static class AccessTools {
 public static FieldInfo Field(Type t,string n) { return t.GetField(n); }
 public static MethodInfo Method(Type t,string n) { return t.GetMethod(n,BindingFlags.Instance|BindingFlags.NonPublic); }
} }
namespace UnityEngine {
 public static class Mathf { public static float Sqrt(float value) { return (float)Math.Sqrt(value); } }
 public struct Vector3 { public float x,z; public Vector3(float a,float b) { x=a;z=b; } }
 public struct Vector2 { float x,z; public Vector2(float a,float b) { x=a;z=b; } public float sqrMagnitude { get { return x*x+z*z; } } }
 public class Transform { public Vector3 position; }
}
public class Minimap {
 public static Minimap instance;
 public bool m_pinUpdateRequired; public UnityEngine.Vector3 Center; public void ShowPointOnMap(UnityEngine.Vector3 pos) { Center=pos; }
 public readonly List<PinData> Pins=new List<PinData>();
 public enum PinType { Icon3 }
 public class Text { public string text; }
 public class PinData { public UnityEngine.Vector3 m_pos; public UnityEngine.Vector3 Position { get { return m_pos; } set { m_pos=value; } } public string Icon; public bool Saved; public string m_name; public bool m_checked; public PinNameData m_NamePinData; }
 public class PinNameData {
  public Text PinNameText=new Text(); public bool Destroyed;
  public PinNameData(PinData p) { PinNameText.text=p.m_name; }
  private void DestroyMapMarker() { Destroyed=true; }
 }
 public PinData AddPin(UnityEngine.Vector3 pos,PinType type,string label,bool save,bool check) {
  var pin=new PinData { Position=pos,Saved=save,m_name=label,m_checked=check };
  if(label!="") pin.m_NamePinData=new PinNameData(pin);
  Pins.Add(pin);return pin;
 }
 public void RemovePin(PinData p) { Pins.Remove(p); }
}
namespace HexResourceTracker {
 internal class Entry<T> { internal T Value; }
 internal static class PluginConfig {
  internal static Entry<bool> IsModEnabled=new Entry<bool> { Value=true };
  internal static string Label="Memorial Site"; internal static bool Hide,Enabled=true;
  internal static bool IsLocationTrackingEnabled(string name) { return Enabled; }
  internal static string GetLocationLabel(string name) { return Hide ? "" : Label; }
 }
}
namespace HexResourceTracker.Core.Tracking {
 internal static class TrackingRangeService { internal static bool InRange=true;
 internal static bool IsWithinTrackingRange(UnityEngine.Vector3 pos) { return InRange; } }
}
namespace HexResourceTracker.Core.Tracking.Monoscripts {
 internal class TrackedLocation {
  internal static HashSet<TrackedLocation> Loaded=new HashSet<TrackedLocation>();
  internal string PrefabName="NorthMemorialPlace";
  internal UnityEngine.Transform transform=new UnityEngine.Transform();
 }
}

public class MessageHud { public enum MessageType { Center } }
public class Player {
 public static Player m_localPlayer; public UnityEngine.Transform transform=new UnityEngine.Transform(); public string LastMessage;
 public void Message(MessageHud.MessageType type,string text) { LastMessage=text; }
}
public class ZNet { public static ZNet instance; public bool Server=true; public bool IsServer() { return Server; } }
public class ZoneSystem {
 public static ZoneSystem instance;
 public class LocationInstance { public string Name; public UnityEngine.Vector3 m_position; }
 public List<LocationInstance> Locations=new List<LocationInstance>();
 public bool FindClosestLocation(string name,UnityEngine.Vector3 from,out LocationInstance result) {
  result=null;float best=float.MaxValue;
  foreach(var loc in Locations) { float dx=loc.m_position.x-from.x,dz=loc.m_position.z-from.z;float d=dx*dx+dz*dz;
   if(loc.Name==name && d<best) { best=d;result=loc; }
  }return result!=null;
 }
}
namespace HexResourceTracker.Core { internal static class TrackedLocations { internal static Minimap.PinType GetPinType(string name) { return Minimap.PinType.Icon3; } } }
namespace HexResourceTracker.Core.PinManagers {
 internal static class SavedPinLabelService { internal static void Reconcile(Minimap map) {} internal static void RegisterLocation(Minimap.PinData pin,string name) { LocationPinManager.RegisterSavedLocation(pin,name); } }
 internal static class LocationPinIconService { internal static void Apply(Minimap.PinData pin,string name) { pin.Icon=name; } }
}
class Checks {
 static void Check(bool ok,string name) { if(!ok) throw new Exception(name); }
 static void Main() {
  Minimap.instance=new Minimap();Player.m_localPlayer=new Player();ZNet.instance=new ZNet();ZoneSystem.instance=new ZoneSystem();
  ZoneSystem.instance.Locations.Add(new ZoneSystem.LocationInstance { Name="NorthMemorialPlace",m_position=new UnityEngine.Vector3(1000,0) });
  ZoneSystem.instance.Locations.Add(new ZoneSystem.LocationInstance { Name="NorthMemorialPlace",m_position=new UnityEngine.Vector3(2000,0) });
  ZoneSystem.instance.Locations.Add(new ZoneSystem.LocationInstance { Name="CharredFortress",m_position=new UnityEngine.Vector3(5,0) });
  HexResourceTracker.Core.Tracking.TrackingRangeService.InRange=false;
  HexResourceTracker.PluginConfig.Hide=true;
  HexResourceTracker.Core.LocationSearchService.FindNearestMemorialSite();
  Check(Minimap.instance.Pins.Count==1 && Minimap.instance.Pins[0].Position.x==1000 && Minimap.instance.Center.x==1000,"nearest memorial revealed and centered without loaded objects or scanner range");
  Check(Minimap.instance.Pins[0].m_name=="" && Minimap.instance.Pins[0].Icon=="NorthMemorialPlace" && Minimap.instance.Pins[0].Saved,"search respects label, icon and saved-map persistence");
  HexResourceTracker.Core.LocationSearchService.FindNearestMemorialSite();
  Check(Minimap.instance.Pins.Count==1,"repeated search does not duplicate pins");
  Player.m_localPlayer.transform.position=new UnityEngine.Vector3(1900,0);
  HexResourceTracker.Core.LocationSearchService.FindNearestMemorialSite();
  Check(Minimap.instance.Pins.Count==1 && Minimap.instance.Pins[0].Position.x==2000,"search uses character position and replaces old out-of-range result");
  var loaded=new TrackedLocation();loaded.transform.position=new UnityEngine.Vector3(2000,0);TrackedLocation.Loaded.Add(loaded);
  LocationPinManager.ReconcilePins();
  Check(Minimap.instance.Pins.Count==1,"later object discovery reuses search result");
  HexResourceTracker.PluginConfig.Enabled=false;LocationPinManager.ReconcilePins();
  Check(Minimap.instance.Pins.Count==0,"tracking toggle removes search result");
  HexResourceTracker.Core.LocationSearchService.FindNearestMemorialSite();
  Check(Minimap.instance.Pins.Count==0 && Player.m_localPlayer.LastMessage.Contains("Enable"),"disabled tracking gives feedback");
  HexResourceTracker.PluginConfig.Enabled=true;ZNet.instance.Server=false;
  HexResourceTracker.Core.LocationSearchService.FindNearestMemorialSite();
  Check(Minimap.instance.Pins.Count==0 && Player.m_localPlayer.LastMessage.Contains("host"),"remote client gets accurate limitation");
  ZNet.instance.Server=true;ZoneSystem.instance.Locations.Clear();
  HexResourceTracker.Core.LocationSearchService.FindNearestMemorialSite();
  Check(Minimap.instance.Pins.Count==0 && Player.m_localPlayer.LastMessage.Contains("No Memorial"),"not-found feedback creates no bogus pin");
  TrackedLocation.Loaded.Clear();
  Minimap.instance=new Minimap();
  var restored=Minimap.instance.AddPin(new UnityEngine.Vector3(2000,0),Minimap.PinType.Icon3,"Memorial Site",true,true);
  LocationPinManager.RegisterSavedLocation(restored,"NorthMemorialPlace");
  HexResourceTracker.Core.Tracking.TrackingRangeService.InRange=true;
  LocationPinManager.ReconcilePins();
  Check(Minimap.instance.Pins.Count==1 && Minimap.instance.Pins[0]==restored && restored.m_checked,"restored saved pin adopted without objects or duplicates");
  LocationPinManager.RevealLocation("NorthMemorialPlace",new UnityEngine.Vector3(2000,0));
  Check(Minimap.instance.Pins.Count==1 && Minimap.instance.Pins[0]==restored,"search reuses restored saved pin");
  HexResourceTracker.PluginConfig.Hide=false;HexResourceTracker.PluginConfig.Label="Remembered Site";
  LocationPinManager.UpdateLocationLabels();
  Check(restored.m_name=="Remembered Site","label settings apply after map reload");
  Console.WriteLine("PASS: saved pin restore/adoption and nearest search, unseen target, range bypass, centering, label/icon settings, deduplication, toggles, remote/not-found feedback");
 }
}
