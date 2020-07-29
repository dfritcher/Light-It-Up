using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Demo script to toggle between automatic and manual lightning
    /// </summary>
    public class DemoScriptManualAutomatic : MonoBehaviour
    {
        /// <summary>
        /// Lightning bolt prefab
        /// </summary>
        public GameObject LightningPrefab;

        /// <summary>
        /// Automatic toggle checkbox
        /// </summary>
        public UnityEngine.UI.Toggle AutomaticToggle;

        /// <summary>
        /// First transform
        /// </summary>
        public Transform a;

        /// <summary>
        /// Second transform
        /// </summary>
        public Transform b;

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0.0f;
                // LightningPrefab.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScriptBase>().Trigger(null, worldPos);
                LightningPrefab.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScriptBase>().Trigger(a.position, b.position);
            }
        }

        /// <summary>
        /// Automatic checkbox toggled
        /// </summary>
        public void AutomaticToggled()
        {
            LightningPrefab.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScriptBase>().ManualMode = !AutomaticToggle.isOn;
        }

        /// <summary>
        /// Manual trigger checkbox toggled
        /// </summary>
        public void ManualTriggerClicked()
        {
            LightningPrefab.GetComponent<DigitalRuby.ThunderAndLightning.LightningBoltPrefabScriptBase>().Trigger();
        }
    }
}