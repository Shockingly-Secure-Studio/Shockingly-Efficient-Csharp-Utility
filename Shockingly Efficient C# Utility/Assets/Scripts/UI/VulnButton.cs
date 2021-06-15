using System;
using System.IO;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class VulnButton : MonoBehaviour, IPointerClickHandler
    {
        public GameObject sqlPanel;
        public GameObject tableNameDropdown;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            Transform parent = transform.parent;
            string vulnName = parent.Find("Name").GetComponent<Text>().text;
            string[] ipPort = parent.Find("IP").GetComponent<Text>().text.Split(':');
            
            if (vulnName.Contains("SQL")) DisplaySQLResults(ipPort[0], ipPort[1]);
            
        }

        /// <summary>
        /// Display 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void DisplaySQLResults(string ip, string port)
        {
            string dir = Path.Combine("Results", ip, port, "dump");
            if (!Directory.Exists(dir)) return; // We don't display anything
            string[] tables = Directory.GetFiles(dir)
                .Select(filename => filename.Replace(".csv", ""))
                .ToArray();

            sqlPanel.SetActive(true);
            Dropdown dropdown = tableNameDropdown.GetComponent<Dropdown>();
            dropdown.ClearOptions();
            
            foreach (string s in tables)
            {
                dropdown.options.Add(new Dropdown.OptionData(s));
            }
        }

        /// <summary>
        /// Display a table dumped by sqlmap. 
        /// </summary>
        /// <param name="ip">IP of the machine hosting the database</param>
        /// <param name="port">Port of the service</param>
        /// <param name="tableName">Name of the table</param>
        /// <exception cref="ArgumentException">The table is an invalid table name.</exception>
        public void DisplaySQLResults(string ip, string port, string tableName)
        {
            string filename = Path.Combine("Results", ip, port, "dump", tableName + ".csv");
            
            if (!File.Exists(filename))
                throw new FileNotFoundException($"DisplaySQLResults: {filename} does not exists");
            
            GameObject sqlResultGroup = GameObject.Find("SQLResult");
            GridLayoutGroup glg = sqlResultGroup.GetComponent<GridLayoutGroup>();
            
            using StreamReader sr = new StreamReader(filename);
            string csvText = sr.ReadToEnd();

            int nbColumns;
        }
    }
}