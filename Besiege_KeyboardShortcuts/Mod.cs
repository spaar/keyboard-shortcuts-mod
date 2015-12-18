using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace meta
{
    public class Mod : MonoBehaviour
    {
        public Mod()
        {
            
        }

        void Update()
        {
            AddPiece addPiece = spaar.ModLoader.AddPiece;
            if (addPiece != null)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    addPiece.undoCommandSystem.MyRedo();
                }
                if (Input.GetKeyDown(KeyCode.U))
                {
                    addPiece.undoCommandSystem.MyUndo();
                }
            }
        }
    }

    public class ModInfo
    {
        public static string MOD_NAME = "KeyboardShortcuts";
    }
}
