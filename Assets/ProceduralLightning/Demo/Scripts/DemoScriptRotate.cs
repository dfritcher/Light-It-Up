using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Demo script to rotate an object
    /// </summary>
    public class DemoScriptRotate : MonoBehaviour
    {
        /// <summary>
        /// Rotation
        /// </summary>
        public Vector3 Rotation;

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            gameObject.transform.Rotate(Rotation * LightningBoltScript.DeltaTime);
        }
    }
}