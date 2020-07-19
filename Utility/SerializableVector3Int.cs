using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SerializableVector3Int
{
    /// <summary>
    /// x component
    /// </summary>
    public int x;

    /// <summary>
    /// y component
    /// </summary>
    public int y;

    /// <summary>
    /// z component
    /// </summary>
    public int z;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SerializableVector3Int(int rX, int rY, int rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    public SerializableVector3Int(Vector3Int vek) {
        x = vek.x;
        y = vek.y;
        z = vek.z;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3Int(SerializableVector3Int rValue)
    {
        return new Vector3Int(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3Int(Vector3Int rValue)
    {
        return new SerializableVector3Int(rValue.x, rValue.y, rValue.z);
    }
}