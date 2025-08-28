using DG.Tweening;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Headbob", menuName = "Scriptable Object/Camera Effects/Headbob")]
//https://www.youtube.com/watch?v=2ysd9uWmUfo (reference tutorial)
public class Headbob_Effect: ScriptableObject
{
    [Range(0.001f,0.02f), SerializeField, Tooltip("Headbob intensity")] 
    public float intensity = 0.002f;

    [Range(1f,30f), SerializeField, Tooltip("Headbob Frequency")]
    public float frequency = 10f;

    [Range(10f,100f), SerializeField, Tooltip("Headbob Smoothing")]
    public float smoothing = 10f;

    private Vector3 headbobDelta;
    private Vector3 headbobStartPos;
    private Transform cameraParentTransform;
    private Tween tween;

    public Action OnHeadbobUpdate;
    public Action OnHeadbobStop;


    public void Initialize(Transform cameraParent)
    {
        cameraParentTransform = cameraParent;
        headbobStartPos = cameraParentTransform.localPosition;

        OnHeadbobUpdate += StartHeadbob;
        OnHeadbobStop += StopHeadbob;
    }

    public void terminate()
    {
        OnHeadbobUpdate -= StartHeadbob;
        OnHeadbobStop -= StopHeadbob;

        if (tween != null && tween.IsActive())
        {
            tween.Kill();
        }
    }

    private void StartHeadbob()
    {
        if (tween != null && tween.IsActive())
        {
            tween.Kill();
        }
        headbobDelta = Vector3.zero;
        headbobDelta.y = Mathf.Lerp(headbobDelta.y, Mathf.Sin(Time.time * frequency) * intensity, Time.deltaTime * smoothing);
        headbobDelta.x = Mathf.Lerp(headbobDelta.x, Mathf.Cos(Time.time * frequency/2f) * intensity, Time.deltaTime * smoothing);
        cameraParentTransform.localPosition += headbobDelta;
    }

    private void StopHeadbob()
    {
        if (cameraParentTransform.localPosition == headbobStartPos) return;
        tween = cameraParentTransform.DOLocalMove(headbobStartPos,Time.deltaTime* smoothing);
    }


}