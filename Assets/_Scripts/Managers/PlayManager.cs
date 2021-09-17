using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    #region Fields, Properties
    [SerializeField] private float _changeDuration;

    [SerializeField] private float _changeTargetTime;

    [SerializeField] private TextMeshProUGUI[] _titleTexts = new TextMeshProUGUI[3];

    [SerializeField] private GameObject _debugPanel = null;

    [SerializeField] private TextMeshProUGUI _debugDisplay = null;

    [SerializeField] private CanvasScaler _mainCanvasScaler = null;
    public CanvasScaler MainCanvasScaler { get { return _mainCanvasScaler; } }

    public bool IsActive { get; set; }
    private int _index = 0;

    private Color _originalColor;
    private int _debugHitCount = 0;
    #endregion Fields, Properties (end)

    #region Methods
    // Use this for initialization
    void Start()
    {
        _originalColor = _titleTexts[0].fontMaterial.GetColor(ShaderUtilities.ID_GlowColor);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
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

    public void ShowDebugPanel()
    {
        if(_debugHitCount > 10)
        {
            _debugPanel.SetActive(true);
            ShowScreenResolution();
        }
        else
        {
            _debugHitCount++;
        }
    }

    public void CloseDebugPanel()
    {
        _debugPanel.SetActive(false);
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


    private void ShowScreenResolution()
    {
        var ratio = _mainCanvasScaler.referenceResolution.x / _mainCanvasScaler.referenceResolution.y;
        _debugDisplay.text = $"Screen Resolution: X - {_mainCanvasScaler.referenceResolution.x}, Y - {_mainCanvasScaler.referenceResolution.y} \r\n";
        _debugDisplay.text += $"Ratio: {ratio} \r\n";
        _debugDisplay.text += $"Camera Aspect: {Camera.main.aspect} \r\n";
        _debugDisplay.text += $"Grid Size: {Utility.GetGridSize()} \r\n";
    }
    #endregion Methods (end)
}
