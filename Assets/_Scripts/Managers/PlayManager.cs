using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class PlayManager : MonoBehaviour
{
    #region Fields, Properties
    [SerializeField]
    private float _changeDuration;

    [SerializeField]
    private float _changeTargetTime;

    [SerializeField]
    private TextMeshProUGUI[] _titleTexts = new TextMeshProUGUI[3];

    public bool IsActive { get; set; }
    private int _index = 0;

    private Color _originalColor;
    #endregion Fields, Properties (end)

    #region Methods
    // Use this for initialization
    void Start()
    {
        _originalColor = _titleTexts[0].fontMaterial.GetColor(ShaderUtilities.ID_GlowColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsActive)
            return;
        if(_changeDuration > _changeTargetTime)
        {
            SwitchHighlightedText();
            _changeDuration = 0;
        }
        else
        {
            _changeDuration += Time.deltaTime;
        }
    }

    private void SwitchHighlightedText()
    {
        for(int i = 0; i< _titleTexts.Length; i++)
        {
            if(i == _index)
                _titleTexts[i].fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, _originalColor);            
            else
                _titleTexts[i].fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, new Color(0, 0, 0, 255));
        }
        _index++;
        if (_index > 2)
            _index = 0;
    }
    #endregion Methods (end)
}
