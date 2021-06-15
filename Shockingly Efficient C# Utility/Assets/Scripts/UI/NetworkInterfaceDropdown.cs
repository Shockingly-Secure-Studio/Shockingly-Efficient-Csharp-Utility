using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NetworkInterfaceDropdown : MonoBehaviour
{
    public static Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponents<Dropdown>()
            .First(_dropdown => _dropdown.name.Contains("NetworkInterface"));
        UpdateNetworkInterfaceChoice();
    }

    public void UpdateNetworkInterfaceChoice()
    {
        List<string> names = Utils.GetNetworkInterfaces();
        dropdown.options.Clear();

        foreach (string ifName in names)
        {
            dropdown.options.Add(new Dropdown.OptionData(ifName));
        }
        
        dropdown.RefreshShownValue();
    }
}
