using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;


[CustomEditor(typeof(Battery)), CanEditMultipleObjects]
public class CustomBatteryDrawer : Editor
{
    GameObject _red = null;
    GameObject _green = null;
    GameObject _blue = null;

    Image _lockedIcon = null;
    Button _increasePower = null;
    Button _decreasePower = null;
    
    bool _hasVariablePower = false;
    bool _isClickable = false;
    
    SerializedProperty _colorTypesProperty = null;
    ColorType[] _colorTypes = new ColorType[4];
    ColorType[] ColorTypes { get { return _colorTypes; } }
    
    private void OnEnable()
    {
#if UNITY_EDITOR
        _colorTypesProperty = serializedObject.FindProperty("_originalColorTypes");
        _red = serializedObject.FindProperty("_redSection").objectReferenceValue as GameObject;
        _green = serializedObject.FindProperty("_greenSection").objectReferenceValue as GameObject;
        _blue = serializedObject.FindProperty("_blueSection").objectReferenceValue as GameObject;

        _lockedIcon = serializedObject.FindProperty("_lockedIcon").objectReferenceValue as Image;
        _increasePower = serializedObject.FindProperty("_increasePowerButton").objectReferenceValue as Button;
        _decreasePower = serializedObject.FindProperty("_decreasePowerButton").objectReferenceValue as Button;
        
        _isClickable = serializedObject.FindProperty("_isClickable").boolValue;
        _hasVariablePower = serializedObject.FindProperty("_hasVariablePower").boolValue;
        if (_colorTypesProperty.isArray)
        {
            for (var i = 0; i < _colorTypesProperty.arraySize; i++)
            {
                var color = (ColorType)_colorTypesProperty.GetArrayElementAtIndex(i).enumValueIndex;
                _colorTypes[i] = color;
            }
        }        
#endif
    }

    public override void OnInspectorGUI()
    {
#if UNITY_EDITOR
        base.OnInspectorGUI();
        UpdateEditorDisplay();
#endif
    }


    private void UpdateEditorDisplay()
    {
        if (Application.isPlaying)
            return;
        _lockedIcon.gameObject?.SetActive(!_isClickable);
        _increasePower?.gameObject?.SetActive(_hasVariablePower);
        _decreasePower?.gameObject?.SetActive(_hasVariablePower);

        _red?.SetActive(ColorTypes.Contains(ColorType.Red));
        _green?.SetActive(ColorTypes.Contains(ColorType.Green));
        _blue?.SetActive(ColorTypes.Contains(ColorType.Blue));
        //_powerDisplay.text = _power.Amount.ToString();
    }
}
