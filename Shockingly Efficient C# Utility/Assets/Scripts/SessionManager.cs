using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SessionManager : MonoBehaviour
    {
        public Text inputSaveName;
        public GameObject InvalidNameBox;
        public GameObject InfoBox;
        private bool nameIsValid = true;
        public void SpawnBox(bool active){
            InvalidNameBox.SetActive(active);
        }
        public void SaveSession()
        {
            if (nameIsValid)
            {
                bool result = SessionSave.SaveSession(inputSaveName.text);
                if (!result)
                {
                    SpawnBox2(true);
                }
            }
        }

        public void OverrideSave()
        {
            SessionSave.SaveSession(inputSaveName.text,true);
        }
        public void SpawnBox2(bool active)
        {
            InfoBox.SetActive(active);
        }

        public void LoadSession()
        {
            if (nameIsValid)
            {
                bool result=SessionSave.LoadSession(inputSaveName.text);
                if(result)
                    SceneManager.LoadScene("ResultScan");
                else
                    SpawnBox(true);
            }
            
        }

        public void Input()
        {
            string saveName = inputSaveName.text;
            nameIsValid=CheckName(saveName);

        }
        public bool CheckName(string _name)
        {
            string regex = "^[A-Za-z0-9-_]+";//@"^[\w,-]+";
            Regex rgx = new Regex(regex);
            if(!rgx.IsMatch(_name)||_name=="")
            {
                SpawnBox(true);
                return false;
            }
            return true;
        }
    }
}