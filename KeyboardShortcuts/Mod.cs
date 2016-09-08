using System;
using spaar.ModLoader;
using UnityEngine;

namespace spaar.Mods.KeyboardShortcuts
{
  public class KeyboardShortcutsMod : ModLoader.Mod
  {
    public override string Name { get; } = "keyboardShortcuts";
    public override string DisplayName { get; } = "Keyboard Shortcuts";
    public override string Author { get; } = "spaar";

    public override Version Version { get; } = new Version(1, 2, 1);
    public override string VersionExtra { get; } = "";
    public override string BesiegeVersion { get; } = "v0.35";

    public override bool CanBeUnloaded { get; } = true;
    public override bool Preload { get; } = false;


    public override void OnLoad()
    {
      Keybindings.AddKeybinding("Pipette",
        new Key(KeyCode.LeftControl, KeyCode.Mouse1));
      Keybindings.AddKeybinding("Open Block Settings",
        new Key(KeyCode.LeftAlt, KeyCode.Mouse1));

      Keybindings.AddKeybinding("Next Tab",
        new Key(KeyCode.LeftControl, KeyCode.Tab));
      Keybindings.AddKeybinding("Previous Tab",
        new Key(KeyCode.LeftShift, KeyCode.Tab));

      Keybindings.AddKeybinding("Increase time scale",
        new Key(KeyCode.RightControl, KeyCode.RightArrow));
      Keybindings.AddKeybinding("Decrease time scale",
        new Key(KeyCode.RightControl, KeyCode.LeftArrow));
      Keybindings.AddKeybinding("Set time scale to 100%",
        new Key(KeyCode.RightShift, KeyCode.RightArrow));
      Keybindings.AddKeybinding("Set time scale to 0%",
        new Key(KeyCode.RightShift, KeyCode.LeftArrow));

      for (int i = 1; i < 10; i++)
      {
        Keybindings.AddKeybinding("Block " + i,
          new Key(KeyCode.None,
          (KeyCode)Enum.Parse(typeof(KeyCode), "Alpha" + i)));
      }

      for (int i = 1; i < 8; i++)
      {
        Keybindings.AddKeybinding("Tab " + i,
          new Key(KeyCode.LeftControl,
          (KeyCode)Enum.Parse(typeof(KeyCode), "Alpha" + i)));
      }

      UnityEngine.Object.DontDestroyOnLoad(KeyboardShortcuts.Instance);
    }

    public override void OnUnload()
    {
      UnityEngine.Object.Destroy(KeyboardShortcuts.Instance);
    }
  }
}
