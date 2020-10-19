using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Settings", menuName = "LightItUp/Settings")]
public class Settings : ScriptableObject
{
    /// <summary>
    /// Variable to hold whether the user has turned music on / off 
    /// </summary>
    [SerializeField]
    private bool _isMusicOn = false;
    public bool IsMusicOn { get { return _isMusicOn; } }


    /// <summary>
    /// Variable to hold whether the user has turned Sound Effects on / off
    /// </summary>
    [SerializeField]
    private bool _isSfxOn = false;
    public bool IsSfxOn { get { return _isSfxOn; } }

}
