using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UIController
{
    public class UIController : MonoBehaviour
    {
        public static void UpdateProfile(TextMeshProUGUI nametxt, TextMeshProUGUI winningratetxt, string name, string winnigrate)
        {
            nametxt.text = name;
            winningratetxt.text = winnigrate;
        }
    }
}
