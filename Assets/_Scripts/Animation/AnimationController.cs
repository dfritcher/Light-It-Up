using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private static AnimationController _instance = null;
    public static AnimationController Instance { get { return _instance; } }


    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(_instance.gameObject);
    }

    public void AnimateDirection(Vector2 startPosition, Vector2 endPosition, float delay, float animationTime, RectTransform transform, Action callback = null)
    {
        StartCoroutine(AnimateDirectionCoroutine(startPosition, endPosition, delay, animationTime, transform, callback));
    }

    public void StopAnimateDirection(Vector2 startPosition, Vector2 endPosition, float delay, float animationTime, RectTransform transform, Action callback = null)
    {
        StopCoroutine(AnimateDirectionCoroutine(startPosition, endPosition, delay, animationTime, transform, callback));
    }

    private IEnumerator AnimateDirectionCoroutine(Vector2 startPosition, Vector2 endPosition, float delay, float animationTime, RectTransform transform, Action callback = null)
    {
        float counter = animationTime;
        yield return null;
        var elapsedTime = 0f;
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        do
        {
            elapsedTime += Time.deltaTime;
            transform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / animationTime);
            yield return null;
            counter -= Time.deltaTime;
        }

        while (counter > 0);

        transform.anchoredPosition = endPosition;
        callback?.Invoke();
        callback = null;
    }

    public void AnimateWidth(float startWidth, float endWidth, float delay, float animationTime, RectTransform transform, Action callback = null)
    {
        StartCoroutine(AnimateWidthCoroutine(startWidth, endWidth, delay, animationTime, transform, callback));
    }

    private IEnumerator AnimateWidthCoroutine(float startWidth, float endWidth, float delay, float animationTime, RectTransform transform, Action callback = null)
    {
        float counter = animationTime;
        yield return null;
        var elapsedTime = 0f;

        do
        {
            elapsedTime += Time.deltaTime;
            transform.sizeDelta = new Vector2(Mathf.Lerp(startWidth, endWidth, elapsedTime / animationTime), transform.rect.height);
            yield return null;
            counter -= Time.deltaTime;
        }

        while (counter > 0);

        transform.sizeDelta = new Vector2(endWidth, transform.rect.height);
        callback?.Invoke();
    }

    public void AnimateHeight(float startHeight, float endHeight, float delay, float animationTime, RectTransform transform, Action callback = null)
    {
        StartCoroutine(AnimateHeightCoroutine(startHeight, endHeight, delay, animationTime, transform, callback));
    }

    private IEnumerator AnimateHeightCoroutine(float startHeight, float endHeight, float delay, float animationTime, RectTransform transform, Action callback = null)
    {
        float counter = animationTime;
        yield return null;
        var elapsedTime = 0f;

        do
        {
            elapsedTime += Time.deltaTime;
            transform.sizeDelta = new Vector2(transform.rect.width, Mathf.Lerp(startHeight, endHeight, elapsedTime / animationTime));
            yield return null;
            counter -= Time.deltaTime;
        }

        while (counter > 0);

        transform.sizeDelta = new Vector2(endHeight, transform.rect.height);
        callback?.Invoke();
    }
}
