using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenBulbAnimationManager : MonoBehaviour
{
    #region Fields, Properties
    [SerializeField]
    private Bulb _bulb1 = null;
    [SerializeField]
    private Bulb _bulb2 = null;
    [SerializeField]
    private Bulb _bulb3 = null;
    [SerializeField]
    private Bulb _bulb4 = null;

    private BulbType _bulbType;
    private bool _animationStarted = false;
    private LevelManager _levelManager = null;
    #endregion Fields, Properties (end)

    #region Methods
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void Setup(LevelManager levelManager, Bulb bulbBeingBroken)
    {
        //if (_animationStarted)
        //    return;
        gameObject.SetActive(true);
        _levelManager = levelManager;
        _bulbType = bulbBeingBroken.BulbType;
        //Set the bulb colors
        switch (_bulbType)
        {
            case BulbType.Bulb1:
                _bulb1.BrokenBulbAnimationEnd += Bulb_BrokenBulbAnimationEnd;
                _bulb1.SetColorTypes(bulbBeingBroken.OriginalColorTypes);
                _bulb1.UpdateUI();
                _bulb1.GetComponent<Animator>().SetTrigger("IsBroken");
                break;
            case BulbType.Bulb2:
                _bulb2.BrokenBulbAnimationEnd += Bulb_BrokenBulbAnimationEnd;
                _bulb2.SetColorTypes(bulbBeingBroken.OriginalColorTypes);
                _bulb2.UpdatePowerState(null);
                _bulb2.GetComponent<Animator>().SetTrigger("IsBroken");
                break;
            case BulbType.Bulb3:
                _bulb3.BrokenBulbAnimationEnd += Bulb_BrokenBulbAnimationEnd;
                _bulb3.SetColorTypes(bulbBeingBroken.OriginalColorTypes);
                _bulb3.UpdatePowerState(null);
                _bulb3.GetComponent<Animator>().SetTrigger("IsBroken");
                break;
            case BulbType.Bulb4:
                _bulb4.BrokenBulbAnimationEnd += Bulb_BrokenBulbAnimationEnd;
                _bulb4.SetColorTypes(bulbBeingBroken.OriginalColorTypes);
                _bulb4.UpdatePowerState(null);
                _bulb4.GetComponent<Animator>().SetTrigger("IsBroken");
                break;
        }
    }

    private void Bulb_BrokenBulbAnimationEnd(Bulb bulb)
    {
        StartCoroutine(Bulb_BrokenAnimationEndCoroutine(bulb));
    }

    private IEnumerator Bulb_BrokenAnimationEndCoroutine(Bulb bulb)
    {
        yield return new WaitForSeconds(2f);
        bulb.BrokenBulbAnimationEnd -= Bulb_BrokenBulbAnimationEnd;
        bulb.ResetBrokenAnimation();
        yield return new WaitForEndOfFrame();
        Reset();        
        yield return null;
    }

    private void Reset()
    {
        gameObject.SetActive(false);
        _levelManager.OnBrokenBulbAnimationEnd();
    }
    #endregion Methods (end)
}
