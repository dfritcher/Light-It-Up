using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Rotates a sprite
    /// </summary>
    public class DemoScript2D : MonoBehaviour
    {
        /// <summary>
        /// Sprite to rotate
        /// </summary>
        public GameObject SpriteToRotate;

        /// <summary>
        /// Lightning bolt base prefab script
        /// </summary>
        public LightningBoltPrefabScriptBase LightningScript;

        private void Start()
        {
        }

        private void Update()
        {
            SpriteToRotate.transform.Rotate(0.0f, 0.0f, LightningBoltScript.DeltaTime * 10.0f);
            if (Input.GetKeyDown(KeyCode.Pause))
            {
                UnityEngine.Time.timeScale = (UnityEngine.Time.timeScale == 0.0f ? 1.0f : 0.0f);
            }
        }
    }
}