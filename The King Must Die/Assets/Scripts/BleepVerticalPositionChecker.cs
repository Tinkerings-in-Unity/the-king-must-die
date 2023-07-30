using System;
using UnityEngine;
using System.Collections;

public class BleepVerticalPositionChecker : MonoBehaviour
{
    private float _distanceToFace;
    private float _yValue;

    public void Setup(Transform parentTransform, float zValue, float verHeight)
    {
        _yValue = verHeight;
        var forwardPos = parentTransform.position + (parentTransform.forward * zValue);
        transform.position = new Vector3(forwardPos.x,_yValue, forwardPos.z);
    }
    
    private void Update()
    {
        if (Application.isEditor)
        {
            GetGroundSliding();
        }
    }

    private void FixedUpdate()
    {
        if (!Application.isEditor)
        {
            GetGroundSliding();
        }
    }

    private void GetGroundSliding()
    {
        Vector3 Origin = transform.position;
        Vector3 dir = Vector3.down;
        RaycastHit hitRay = new RaycastHit();

        if (Physics.Raycast(Origin, dir, out hitRay,Mathf.Infinity)) 
        {
            _distanceToFace = hitRay.distance;
            Debug.DrawLine(Origin, Origin + dir * hitRay.distance, Color.green);
        }
        else
        {
            _distanceToFace = Mathf.Infinity;
        }
    }
    
    
    public Vector3 GetPositionToDashTo()
    {
        var pos = transform.position;

        return new Vector3(pos.x, pos.y - _distanceToFace, pos.z);
    }

    public void SetHorizontalPosition(Transform parentTransform, float zValue)
    {
        var forwardPos = parentTransform.position + (parentTransform.forward * zValue);
        transform.position = new Vector3(forwardPos.x,_yValue, forwardPos.z);
    }
}