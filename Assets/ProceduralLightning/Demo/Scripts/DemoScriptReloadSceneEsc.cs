using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Press ESC to reload a scene
    /// </summary>
    public class DemoScriptReloadSceneEsc : MonoBehaviour
    {
        private void Start()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
    }
}