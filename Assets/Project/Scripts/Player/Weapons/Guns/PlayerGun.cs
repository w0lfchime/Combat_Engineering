using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [Header("Gun Stats")]
    public string gunName;
    public float reloadTime;
    public float bulletSpeed;

    [Header("Gun Params")]
    public GameObject bullet;
    public Transform gripOffset;

    public void Fire()
    {

    }
}
