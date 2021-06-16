using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class TableNameDropdown : MonoBehaviour
    {
        public TMP_Dropdown m_Dropdown;

        void Start()
        {
            //Fetch the Dropdown GameObject
            m_Dropdown = GetComponent<TMP_Dropdown>();
            //Add listener for when the value of the Dropdown changes, to take action
        }
        
        void DropdownValueChanged(TMP_Dropdown change)
        {
            
        }
    }
}