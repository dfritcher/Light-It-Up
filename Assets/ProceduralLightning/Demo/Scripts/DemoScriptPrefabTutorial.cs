using UnityEngine;
using System.Collections;

using DigitalRuby.ThunderAndLightning;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Trigger lightning with spacebar
    /// </summary>
    public class DemoScriptPrefabTutorial : MonoBehaviour
    {
        /// <summary>
        /// Lightning script
        /// </summary>
        public LightningBoltPrefabScript LightningScript;

        private void Start()
        {

        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                LightningScript.Trigger();
            }
        }
    }
}
