using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassThroughOption : MonoBehaviour
{
    [SerializeField]
    private List<ColorType> _optionColorTypes = null;
    public List<ColorType> OptionColorTypes { get { return _optionColorTypes ?? (_optionColorTypes = new List<ColorType>()); } }

    [SerializeField]
    private List<Image> _optionColors = null;

    [SerializeField]
    private TextMeshProUGUI _optionDisplay = null;
    [SerializeField]
    private Image _lockedImage = null;

    [SerializeField]
    private bool _isClickable = true;
    public bool IsClickable { get { return _isClickable; } }

    [SerializeField]
    private bool _isVisible = true;
    public bool IsVisible { get { return _isVisible; } }

    public delegate void PassThroughOptionEvent(PassThroughOption option);
    public event PassThroughOptionEvent OnClick;

    public void OptionClicked()
    {
        if (!IsClickable)
            return;
        OnClick?.Invoke(this);
    }

    public void Setup(List<ColorType> colorTypes)
    {
        _optionColorTypes = colorTypes;
        gameObject.SetActive(_isVisible);
        UpdateUI();
    }

    private void UpdateUI()
    {
        _lockedImage.gameObject.SetActive(!IsClickable);
        UpdateColorDisplay();
    }

    private void UpdateColorDisplay()
    {
        _optionColors[0].gameObject.SetActive(OptionColorTypes.Contains(ColorType.Red));
        _optionColors[1].gameObject.SetActive(OptionColorTypes.Contains(ColorType.Green));
        _optionColors[2].gameObject.SetActive(OptionColorTypes.Contains(ColorType.Blue));
    }
}
