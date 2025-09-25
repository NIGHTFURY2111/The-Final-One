using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveLight : MonoBehaviour
{
//script to continuously move the light to random points between the given range with a given speed
    public float Xrange = 5f;
    public float Yrange = 5f;
    public float Zrange = 5f;
    public float XrangeN = 5f;
    public float YrangeN = 5f;
    public float ZrangeN = 5f;
    public float speed = 1f;
    private Vector3 targetPosition;
    void Start()
    {
        SetNewTargetPosition();
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }
    void SetNewTargetPosition()
    {
        float x = Random.Range(XrangeN, Xrange);
        float y = Random.Range(YrangeN, Yrange);
        float z = Random.Range(ZrangeN, Zrange);
        targetPosition = new Vector3(x, y, z);
    }
}
