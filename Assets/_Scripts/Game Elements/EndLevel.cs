using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndLevel : MonoBehaviour
{
    #region Fields, Properties
    [SerializeField]
    private TextMeshProUGUI _message = null;

    [SerializeField]
    private string _androidStoreText = string.Empty;
    [SerializeField]
    private string _iosStoreText = string.Empty;
    #endregion Fields, Properties (end)

    #region Methods
    private void Awake()
    {
#if UNITY_ANDROID
        _message.text = $"Thank you for playing Light 'Em Up. If you enjoyed this game please check out my other game at: {_androidStoreText}";
#endif

#if UNITY_IOS
        _message.text = $"Thank you for playing Light 'Em Up. If you enjoyed this game please check out my other game at: {_iosStoreText}";
#endif
    }
    #endregion Methods (end)
}
