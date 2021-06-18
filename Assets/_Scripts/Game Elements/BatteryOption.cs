using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BatteryOption : MonoBehaviour
{
    [SerializeField]
    private List<ColorType> _batteryOptionColorTypes = null;
    public List<ColorType> BatteryOptionColorTypes { get { return _batteryOptionColorTypes ?? (_batteryOptionColorTypes = new List<ColorType>()); } }

    [SerializeField]
    private List<Image> _batteryColors = null;
    
    [SerializeField]
    private Image _lockedImage = null;

    [SerializeField]
    private bool _isClickable = true;
    public bool IsClickable { get { return _isClickable; } }

    [SerializeField]
    private bool _isVisible = true;
    public bool IsVisible { get { return _isVisible; } }

    public delegate void BatteryOptionEvent(BatteryOption batteryOption);
    public event BatteryOptionEvent OnClick;

    public void BatteryOptionClicked()
    {
        if (!IsClickable)
            return;
        OnClick?.Invoke(this);
    }

    public void Setup(List<ColorType> colorTypes)
    {
        _batteryOptionColorTypes = colorTypes;
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
        _batteryColors[0].gameObject.SetActive(BatteryOptionColorTypes.Contains(ColorType.Red));
        _batteryColors[1].gameObject.SetActive(BatteryOptionColorTypes.Contains(ColorType.Green));
        _batteryColors[2].gameObject.SetActive(BatteryOptionColorTypes.Contains(ColorType.Blue));

    }
}
