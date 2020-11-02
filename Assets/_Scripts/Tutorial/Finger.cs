using UnityEngine;
using System.Collections;

public class Finger : MonoBehaviour
{

    #region Fields, Properties
    [SerializeField]
    private TutorialResolverBase _tutorialResolver = null;


    #endregion Fields, Properties (end)

    #region Methods
    public void OnFingerAnimationEnd(int index)
    {
        _tutorialResolver.OnFingerAnimationEnd(index);
    }
    #endregion Methods (end)
}
