using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using Service.Exploit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class VulnButton : MonoBehaviour, IPointerClickHandler
    {
        public GameObject cellPrefab;

        public void Start()
        {
            Debug.Log ("I'm Attached to " + gameObject);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            Transform parent = transform.parent;
            string vulnName = parent.Find("Name").GetComponent<Text>().text;
            string[] ipPort = parent.Find("IP").GetComponent<Text>().text.Split(':');
            
            if (vulnName.Contains("SQL")) DisplayPanel(ipPort[0], ipPort[1], AccessPointType.SQLi);
            if (vulnName.Contains("Insecure_Authentication")) DisplayPanel(ipPort[0], ipPort[1], AccessPointType.Insecure_Authentication);
            if (vulnName.Contains("Wordpress"))
            {
                DisplayPanel(ipPort[0],ipPort[1], AccessPointType.Wordpress);
            }
            if (vulnName.Contains("RCE")) DisplayPanel(ipPort[0], ipPort[1],AccessPointType.RCE);
        }

        /// <summary>
        /// Display 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="vulnName"></param>
        public void DisplayPanel(string ip, string port, AccessPointType vulnName)
        {
            string dir = Path.Combine("Results", ip, port, "dump");
            if (!Directory.Exists(dir)) return; // We don't display anything
            string[] tables = Directory.GetFiles(dir, "*.csv")
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();

            Transform root = transform.root;
            GameObject sqlPanel = root.Find("SQLPanel").gameObject;
            GameObject tableNameDropdown = sqlPanel.transform.Find("Title/TableNamesDropdown").gameObject;
            
            sqlPanel.SetActive(true);
            TMP_Dropdown dropdown = tableNameDropdown.GetComponent<TMP_Dropdown>();
            dropdown.ClearOptions();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                switch (go.name)
                {
                    case "BaseDeDonnée":
                    case "TableNamesDropdown":
                        go.SetActive(true);
                        break;
                    case "RCEInputField":
                    case "RCEButton":
                        go.SetActive(false);
                        break;
                }
            }
            
            foreach (string s in tables)
            {   
                dropdown.options.Add(new TMP_Dropdown.OptionData(s));
            }
            if (vulnName == AccessPointType.SQLi)
            {
                DisplaySQLResults(ip, port, dropdown);
                dropdown.onValueChanged.AddListener(delegate { DisplaySQLResults(ip, port, dropdown); });
            }
            if (vulnName == AccessPointType.Insecure_Authentication)
            {
                DisplayWeakPassword(ip,port);
            }

            if (vulnName == AccessPointType.Wordpress)
            {
                DisplayWorpress(ip,port);
            }
            
            if (vulnName == AccessPointType.RCE)
                DisplayReverseShell(ip, port);
        }

        private void DisplayWorpress(string ip, string port)
        {
            GameObject sqlResultGroup = GameObject.Find("SQLResult");
            GridLayoutGroup glg = sqlResultGroup.GetComponent<GridLayoutGroup>();
            foreach (Transform child in glg.transform) {
                Destroy(child.gameObject);
            }
            GameObject cell = Instantiate(cellPrefab, glg.transform, false);
            string res = File.ReadAllText(Path.Combine("Results", ip,port,"Wpscanres"));
            cell.GetComponent<TMP_Text>().text = res;
        }
        private void DisplayWeakPassword(string ip,string port)
        {
            GameObject sqlResultGroup = GameObject.Find("SQLResult");
            GridLayoutGroup glg = sqlResultGroup.GetComponent<GridLayoutGroup>();
            foreach (Transform child in glg.transform) {
                Destroy(child.gameObject);
            }
            GameObject cell = Instantiate(cellPrefab, glg.transform, false);
            ServiceResult r=Service.Service.GetServiceResult(ip,port);
            string poc = r.AccessPoints.Find(a => a.Type == AccessPointType.Insecure_Authentication).POC;
            cell.GetComponent<TMP_Text>().text = poc;
        }

        /// <summary>
        /// Display a table dumped by sqlmap. 
        /// </summary>
        /// <param name="ip">IP of the machine hosting the database</param>
        /// <param name="port">Port of the service</param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="dropdown">Value of dropdown</param>
        /// <exception cref="ArgumentException">The table is an invalid table name.</exception>
        public void DisplaySQLResults(string ip, string port, TMP_Dropdown dropdown)
        {
            string tableName = dropdown.options[dropdown.value].text;
            
            string filename = Path.Combine("Results", ip, port, "dump", tableName + ".csv");
            
            if (!File.Exists(filename))
                throw new FileNotFoundException($"DisplaySQLResults: {filename} does not exists");
            
            GameObject sqlResultGroup = GameObject.Find("SQLResult");
            GridLayoutGroup glg = sqlResultGroup.GetComponent<GridLayoutGroup>();
            
            using StreamReader sr = new StreamReader(filename);
            string csvText = sr.ReadToEnd();

            string[][] values = csvText
                .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(',')).ToArray();

            glg.constraintCount = values[0].Length;
            
            // We clear the glg
            foreach (Transform child in glg.transform) {
                Destroy(child.gameObject);
            }

            foreach (string[] line in values)
            {
                foreach (string value in line)
                {
                    GameObject cell = Instantiate(cellPrefab, glg.transform, false);
                    cell.GetComponent<TMP_Text>().text = value;
                    glg.GetComponent<RectTransform>().transform.SetAsLastSibling();
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(glg.GetComponent<RectTransform>()); 
        }

        public void DisplayReverseShell(string ip, string port)
        {
            // We clear the glg
            GameObject sqlResultGroup = GameObject.Find("SQLResult");
            GridLayoutGroup glg = sqlResultGroup.GetComponent<GridLayoutGroup>();
            foreach (Transform child in glg.transform) {
                Destroy(child.gameObject);
            }
            
            WebShellInterface webShellInterface = WebShellInterface.AttachWebShell(ip, port);
            
            foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                switch (go.name)
                {
                    case "BaseDeDonnée":
                    case "TableNamesDropdown":
                        go.SetActive(false);
                        break;
                    case "RCEInputField":
                    case "RCEButton":
                        go.SetActive(true);
                        break;
                }
            }
            
            GameObject.Find("RCEButton").GetComponent<Button>().onClick.AddListener(async delegate
            {
                await RCE_OnButtonClick(webShellInterface);
            });
        }

        public async Task RCE_OnButtonClick(WebShellInterface webShellInterface)
        {
            string cmd = GameObject.Find("CommandText").GetComponent<TMP_Text>().text;
            Debug.Log(cmd);
            string result = await webShellInterface.SendCommand(cmd);
            Debug.Log(result);
            GameObject sqlResultGroup = GameObject.Find("SQLResult");
            GridLayoutGroup glg = sqlResultGroup.GetComponent<GridLayoutGroup>();
            foreach (Transform child in glg.transform) {
                Destroy(child.gameObject);
            }
            
            GameObject cell = Instantiate(cellPrefab, glg.transform, false);
            cell.GetComponent<TMP_Text>().text = result;
        }
    }
}