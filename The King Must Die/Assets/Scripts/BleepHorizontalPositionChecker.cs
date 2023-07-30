using System;
using UnityEngine;
using System.Collections;

public class BleepHorizontalPositionChecker: MonoBehaviour
{
    [SerializeField] private LayerMask layersToCheckForCollision;
    public float _distanceToFace;
    private float _maxBleepHorizontalDistance;
    private Transform _parentTransform;

    public void Setup(Transform parentTransform, float horDis, float verHeight)
    {
        _parentTransform = parentTransform;
        _maxBleepHorizontalDistance = horDis;
        var pos = _parentTransform.position;
        transform.position = new Vector3(pos.x, verHeight, pos.z);
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            GetHorizontalDistance();
        }
    }

    private void FixedUpdate()
    {
        if (!Application.isEditor)
        {
            GetHorizontalDistance();
        }
    }

    private void GetHorizontalDistance()
    {
        Vector3 origin = transform.position;
        Vector3 dir = _parentTransform.forward;
        RaycastHit hitRay = new RaycastHit();

        if (Physics.Raycast(origin, dir, out hitRay,_maxBleepHorizontalDistance, layersToCheckForCollision)) 
        {
            _distanceToFace = hitRay.distance;
            Debug.DrawLine(origin, origin + dir * hitRay.distance, Color.red);
        }
        else
        {
            _distanceToFace = _maxBleepHorizontalDistance;
        }
    }
    
    
    public float GetHorizontalPositionToBleepTo()
    {
        return _distanceToFace;
    }
}