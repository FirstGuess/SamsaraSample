using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OrientedPoint
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 normal;

    public OrientedPoint(Vector3 _pos, Vector3 _forward, Vector3 _normal) {
        pos = _pos;
        rot = Quaternion.LookRotation(_forward.normalized, _normal.normalized);
        normal = _normal;
    }

    public OrientedPoint(Vector3 _pos, Quaternion _rot) {
        pos = _pos;
        rot = _rot;
        normal = Vector3.zero;
    }

    public OrientedPoint(Vector3 _pos, Vector3 _forward) {
        pos = _pos;
        rot = Quaternion.LookRotation(_forward);
        normal = Vector3.zero;
    }

    

    public Vector3 LocalToWorld(Vector3 localPos) {

        return pos + rot * localPos;

    }

    public Vector3 LocalToWorldVector(Vector3 localPos) {

        return rot * localPos;

    }

}
