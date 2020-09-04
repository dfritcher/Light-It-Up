using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor (typeof(PassThrough)), CanEditMultipleObjects]
public class CustomPassThroughDrawer : Editor
{
    GameObject _redFull = null;
    GameObject _greenFull = null;
    GameObject _blueFull = null;

    GameObject _redTop = null;
    GameObject _greenTop = null;
    GameObject _greenBottom = null;
    GameObject _blueBottom = null;


    Image _lockedIcon = null;

    bool _isClickable = false;

    SerializedProperty _colorTypesProperty = null;
    ColorType[] _colorTypes = new ColorType[4];
    ColorType[] ColorTypes { get { return _colorTypes; } }

    private bool IsPowered { get { return ColorTypes.Any(c => c != ColorType.None); } }
    private void OnEnable()
    {
        _colorTypesProperty = serializedObject.FindProperty("_originalColorTypes");
        _redFull = serializedObject.FindProperty("_redFullUnLit").objectReferenceValue as GameObject;
        _greenFull = serializedObject.FindProperty("_greenFullUnLit").objectReferenceValue as GameObject;
        _blueFull = serializedObject.FindProperty("_blueFullUnLit").objectReferenceValue as GameObject;
        _redTop = serializedObject.FindProperty("_redTopUnLit").objectReferenceValue as GameObject;
        _greenTop = serializedObject.FindProperty("_greenTopUnLit").objectReferenceValue as GameObject;
        _greenBottom = serializedObject.FindProperty("_greenBottomUnLit").objectReferenceValue as GameObject;
        _blueBottom = serializedObject.FindProperty("_blueBottomUnLit").objectReferenceValue as GameObject;
        _isClickable = serializedObject.FindProperty("_isClickable").boolValue;
        _lockedIcon = serializedObject.FindProperty("_lockedIcon").objectReferenceValue as Image;

        if (_colorTypesProperty.isArray)
        {
            for (var i = 0; i < _colorTypesProperty.arraySize; i++)
            {
                var color = (ColorType)_colorTypesProperty.GetArrayElementAtIndex(i).enumValueIndex;
                _colorTypes[i] = color;
            }
        }

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UpdateEditorDisplay();
    }
    
    private void UpdateEditorDisplay()
    {
        _lockedIcon.gameObject?.SetActive(!_isClickable);
        _redFull.SetActive(false);
        _redTop.SetActive(false);
        _greenFull.SetActive(false);
        _greenTop.SetActive(false);
        _greenBottom.SetActive(false);
        _blueFull.SetActive(false);
        _blueBottom.SetActive(false);


        if (ColorTypes.Contains(ColorType.Red) && !(ColorTypes.Contains(ColorType.Green) || ColorTypes.Contains(ColorType.Blue)))
        {
            _redFull.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Green) && !(ColorTypes.Contains(ColorType.Red) || ColorTypes.Contains(ColorType.Blue)))
        {
            _greenFull.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Blue) && !(ColorTypes.Contains(ColorType.Red) || ColorTypes.Contains(ColorType.Green)))
        {
            _blueFull.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Red) && ColorTypes.Contains(ColorType.Green) && !ColorTypes.Contains(ColorType.Blue))
        {
            _redTop.SetActive(true);
            _greenBottom.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Red) && ColorTypes.Contains(ColorType.Blue) && !ColorTypes.Contains(ColorType.Green))
        {
            _redTop.SetActive(true);
            _blueBottom.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Green) && ColorTypes.Contains(ColorType.Blue) && !ColorTypes.Contains(ColorType.Red))
        {
            _greenTop.SetActive(true);
            _blueBottom.SetActive(true);
        }

    }
}
