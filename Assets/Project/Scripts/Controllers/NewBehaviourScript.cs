using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayer
{
    public bool GetAiming();
    public Vector3 GetPosition();
}
