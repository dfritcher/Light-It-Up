using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Bulb)), CanEditMultipleObjects]
public class CustomBulbDrawer : Editor
{
    GameObject _redFull = null;
    GameObject _greenFull = null;
    GameObject _blueFull = null;
    
    GameObject _redTop = null;
    GameObject _greenTop = null;
    GameObject _greenBottom = null;
    GameObject _blueBottom = null;

    GameObject _redSmallTop = null;
    GameObject _greenSmallMiddle = null;
    GameObject _blueSmallBottom = null;

    SerializedProperty _colorTypesProperty = null;
    ColorType[] _colorTypes = new ColorType[4];
    ColorType[] ColorTypes { get { return _colorTypes; } }
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
        _redSmallTop = serializedObject.FindProperty("_redSmallTopUnLit").objectReferenceValue as GameObject;
        _greenSmallMiddle = serializedObject.FindProperty("_greenSmallMiddleUnLit").objectReferenceValue as GameObject;
        _blueSmallBottom = serializedObject.FindProperty("_blueSmallBottomUnLit").objectReferenceValue as GameObject;

        if (_colorTypesProperty.isArray)
        {
            for(var i = 0; i < _colorTypesProperty.arraySize; i++)
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
        _redFull?.SetActive(false);
        _redTop?.SetActive(false);
        _greenFull?.SetActive(false);
        _greenTop?.SetActive(false);
        _greenBottom?.SetActive(false);
        _blueFull?.SetActive(false);
        _blueBottom?.SetActive(false);
        _redSmallTop?.SetActive(false);
        _greenSmallMiddle?.SetActive(false);
        _blueSmallBottom?.SetActive(false);

        if (ColorTypes.Contains(ColorType.Red) && !(ColorTypes.Contains(ColorType.Green) || ColorTypes.Contains(ColorType.Blue)))
        {
            _redFull?.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Green) && !(ColorTypes.Contains(ColorType.Red) || ColorTypes.Contains(ColorType.Blue)))
        {
            _greenFull?.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Blue) && !(ColorTypes.Contains(ColorType.Red) || ColorTypes.Contains(ColorType.Green)))
        {
            _blueFull?.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Red) && ColorTypes.Contains(ColorType.Green) && !ColorTypes.Contains(ColorType.Blue))
        {
            _redTop?.SetActive(true);
            _greenBottom?.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Red) && ColorTypes.Contains(ColorType.Blue) && !ColorTypes.Contains(ColorType.Green))
        {
            _redTop?.SetActive(true);
            _blueBottom?.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Green) && ColorTypes.Contains(ColorType.Blue) && !ColorTypes.Contains(ColorType.Red))
        {
            _greenTop?.SetActive(true);
            _blueBottom?.SetActive(true);
        }
        else if (ColorTypes.Contains(ColorType.Red) && ColorTypes.Contains(ColorType.Green) && ColorTypes.Contains(ColorType.Blue))
        {
            _redSmallTop?.SetActive(true);
            _greenSmallMiddle?.SetActive(true);
            _blueSmallBottom?.SetActive(true);
        }
    }
}
