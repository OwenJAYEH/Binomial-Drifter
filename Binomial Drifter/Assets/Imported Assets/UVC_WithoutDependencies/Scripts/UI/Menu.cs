using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace PG 
{
    public class Menu :MonoBehaviour
    {
        public List<ButtonScene> ButtonScenes = new List<ButtonScene>();
        public bool CanHideMainMenu;
        public GameObject MainParent;
        public GameObject HelpTextGO;

        public int LastSelectedGamepadP1
        {
            get
            {
                return PlayerPrefs.GetInt ("GamePadP1");
            }
            set
            {
                PlayerPrefs.SetInt ("GamePadP1", value);
            }
        }

        public int LastSelectedGamepadP2
        {
            get
            {
                return PlayerPrefs.GetInt ("GamePadP2");
            }
            set
            {
                PlayerPrefs.SetInt ("GamePadP2", value);
            }
        }

        private void Awake ()
        {
            if (CanHideMainMenu)
            {
                MainParent.SetActive (false);
            }

            foreach(var bs in ButtonScenes)
            {
                bs.Btn.onClick.AddListener (()=> 
                {
                    SceneManager.LoadScene (bs.Scene.SceneName);
                });
            }

            if (Application.isMobilePlatform)
            {
                HelpTextGO.SetActive (false);
            }
        }

        private void Update ()
        {
            if (CanHideMainMenu && Input.GetKeyDown (KeyCode.Escape))
            {
                MainParent.SetActive(!MainParent.activeSelf);
            }
        }

        [System.Serializable]
        public class ButtonScene
        {
            public Button Btn;
            public SceneField Scene;
        }
    }
}
