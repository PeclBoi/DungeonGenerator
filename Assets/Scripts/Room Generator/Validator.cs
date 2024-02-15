using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Validator : MonoBehaviour
{

    public bool IsValid;

    public int framesTillValid { get; private set; } = 5;

    IEnumerator coroutine;

    public bool _isStateSet;

    private void Awake()
    {
        IsValid = false;
        coroutine = ValidCountdown();
        StartCoroutine(coroutine);
    }

    public void StopCoroutine()
    {
        StopCoroutine(coroutine);
        _isStateSet = true;
    }


    IEnumerator ValidCountdown()
    {
        IsValid = false;
        while (framesTillValid > 0)
        {
            framesTillValid--;
            yield return null;
        }
        IsValid = true;
        _isStateSet = true;
        Debug.Log("Valid frfr");
    }
}
