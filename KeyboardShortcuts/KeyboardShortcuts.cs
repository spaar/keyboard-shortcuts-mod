using System;
using spaar.ModLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace spaar.Mods.KeyboardShortcuts
{
  public class KeyboardShortcuts : SingleInstance<KeyboardShortcuts>
  {
    public override string Name { get; } = "Keyboard Shortcuts";

    private Key pipette, openSettings, nextTab, previousTab;
    private Key[] blockKeys, tabKeys;

    private int[] tabIndices; // Mapping of Besiege-internal indices to the order
                              // in which they are actually displayed
    private int currentTab;
    private BlockTabController tabController;
    private HudInputControl hudInputControl;

    private int[][] blockIndices; // Basically same as with tabIndices

    private void Start()
    {
      pipette = Keybindings.Get("Pipette");
      openSettings = Keybindings.Get("Open Block Settings");
      nextTab = Keybindings.Get("Next Tab");
      previousTab = Keybindings.Get("Previous Tab");

      blockKeys = new Key[9];
      for (int i = 0; i < 9; i++)
      {
        blockKeys[i] = Keybindings.Get("Block " + (i + 1));
      }

      tabKeys = new Key[7];
      for (int i = 0; i < 7; i++)
      {
        tabKeys[i] = Keybindings.Get("Tab " + (i + 1));
      }

      OnSceneLoaded(default(Scene), default(LoadSceneMode));
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      if (!Game.AddPiece) return;

      tabController = FindObjectOfType<BlockTabController>();
      hudInputControl = FindObjectOfType<HudInputControl>();

      tabIndices = new int[7]
      {
        4, 0, 6, 1, 2, 3, 5
      };
      currentTab = 0;

      blockIndices = new int[7][]
       {
        new int[9] {  0,  1,  2,  4,  3, -1, -1, -1, -1 }, // Fundamentals
        new int[9] {  0,  1,  2,  3,  4,  5,  6,  7,  8 }, // Blocks
        new int[9] {  9, 10,  0,  6,  4,  5,  2,  7,  8 }, // Locomotion
        new int[9] {  1,  2,  4,  3,  5,  7,  6,  0, -1 }, // Mechanical
        new int[9] {  0,  1,  4, 12,  5,  2, 13,  6, 10 }, // Weaponry
        new int[9] {  0,  1,  6,  2,  3,  4,  5, -1, -1 }, // Flight
        new int[9] {  0,  1,  2,  3,  4,  5, -1, -1, -1 }, // Armour
      };
    }

    private void Update()
    {
      if (!Game.AddPiece) return;
      if (Game.IsSimulating) return;

      ////Debug code helpful for filling out blockIndices table
      //if (Input.GetKeyDown(KeyCode.N))
      //{
      //  var buttons = tabController.tabs[tabController.activeTab]
      //    .GetComponent<BlockMenuControl>().buttons;
      //  for (int i = 0; i < buttons.Length; i++)
      //  {
      //    Debug.Log("buttons[" + i + "] = " + buttons[i]);
      //  }
      //}

      if (pipette.Pressed())
      {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
          var myName = hit.transform.GetComponent<MyBlockInfo>().blockName;
          for (int i = 0; i < PrefabMaster.BlockPrefabs.Count; i++)
          {
            var type = PrefabMaster.BlockPrefabs[i].gameObject;
            if ((type.GetComponent<MyBlockInfo>()
              && type.GetComponent<MyBlockInfo>().blockName == myName)
              || (type.GetComponentInChildren<MyBlockInfo>()
              && type.GetComponentInChildren<MyBlockInfo>().blockName == myName))
            {
              Game.AddPiece.SetBlockType(i);
              break;
            }
          }
        }
      }

      if (openSettings.Pressed())
      {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
          var block = hit.transform.gameObject.GetComponent<BlockBehaviour>();
          FindObjectOfType<KeyMapModeButton>().KeyMapOn();
          BlockSelect(block);
          BlockMapper.Open(block);
        }
      }

      if (nextTab.Pressed())
      {
        currentTab = InternalToOrderedTabIndex(tabController.activeTab);
        tabController.OpenTab(tabIndices[mod(++currentTab, tabIndices.Length)]);
      }
      if (previousTab.Pressed())
      {
        currentTab = InternalToOrderedTabIndex(tabController.activeTab);
        tabController.OpenTab(tabIndices[mod(--currentTab, tabIndices.Length)]);
      }

      // Don't react to block shortcuts when block settings are open to prevent
      // typing a slider value changing what block is selected
      if (BlockMapper.CurrentInstance == null)
      {
        for (int i = 0; i < 9; i++)
        {
          if (blockKeys[i].Pressed())
          {
            var index = blockIndices[
              InternalToOrderedTabIndex(tabController.activeTab)][i];
            if (index != -1)
            {
              tabController.tabs[tabController.activeTab]
                .GetComponent<BlockMenuControl>()
                .buttons[index].Set();
            }
          }
        }
      }

      for (int i = 0; i < 7; i++)
      {
        if (tabKeys[i].Pressed())
        {
          tabController.OpenTab(tabIndices[i]);
        }
      }
    }

    private void BlockSelect(BlockBehaviour block)
    {
      if (block != null)
      {
        if (block != AddPiece.SelectedBlock)
        {
          BlockVisualController componentInParent = null;
          if (AddPiece.SelectedBlock != null)
          {
            componentInParent = AddPiece.SelectedBlock.GetComponentInParent<BlockVisualController>();
            if (componentInParent != null)
            {
              componentInParent.SetNormal();
            }
          }
          componentInParent = block.GetComponentInParent<BlockVisualController>();
          if (componentInParent != null)
          {
            componentInParent.SetSelected();
          }
          AddPiece.SelectedBlock = (GenericBlock)block;
        }
      }
    }

    private int InternalToOrderedTabIndex(int internalIndex)
    {
      for (int i = 0; i < tabIndices.Length; i++)
      {
        if (tabIndices[i] == internalIndex) return i;
      }

      return -1;
    }

    // Our own modulo function that deals with negative numbers correctly
    private int mod(int x, int m)
    {
      return ((x % m) + m) % m;
    }
  }
}
