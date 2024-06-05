﻿using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    #region Fields, Properties
    [SerializeField]
    private CanvasGroup _canvas = null;
   
    #endregion Fields, Properties (end)

    #region Methods
    public void Setup()
    {
        SetCanvasState(true);        
    }
    public void CloseClicked()
    {
        SetCanvasState(false);
    }

    private void SetCanvasState(bool enabled)
    {
        _canvas.alpha = enabled ? 1 : 0;
        _canvas.blocksRaycasts = enabled;
        _canvas.interactable = enabled;
    }
    #endregion Methods (end)
}