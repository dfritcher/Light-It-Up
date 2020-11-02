using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "GameInfo", menuName = "LightItUp/SaveData")]
public class GameSaveInfo : ScriptableObject
{
    /// <summary>
    /// Variable to hold the highest level the player has achieved.
    /// </summary>
    [SerializeField]
    private int _highestLevelUnlocked = 1;
    public int HighestLevelUnlocked { get { return _highestLevelUnlocked; } set { _highestLevelUnlocked = value; } }

    /// <summary>
    /// Variable to hold whether the user has turned music on / off 
    /// </summary>
    [SerializeField]
    private bool _isMusicOn = false;
    public bool IsMusicOn { get { return _isMusicOn; } set { _isMusicOn = value; } }

    /// <summary>
    /// Variable to hold whether the user has turned Sound Effects on / off
    /// </summary>
    [SerializeField]
    private bool _isSfxOn = false;
    public bool IsSfxOn { get { return _isSfxOn; } set { _isSfxOn = value; } }

}
