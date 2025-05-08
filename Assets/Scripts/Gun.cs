using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Gun : RangedWeapon
{
    private Transform cam;

    [SerializeField] private Transform body;
    [SerializeField] private Transform bulletOrigin;
    [SerializeField] private Transform aimPoint;

    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    private float lastFired;

    [SerializeField] private float magSize;
    //[SerializeField] private float 
    [SerializeField] private float aimDistance;
    [SerializeField] private float aimSpeed;

    [Space] 
    [SerializeField] private float curMag;

    [SerializeField] private Vector2 curRecoil;

    private Vector3 bodyPos;


    public override void OnEquip(PlayerItemController itemController)
    {
        cam = itemController.manager.Cam;
        bodyPos = transform.position;
    }

    public override void WhileEquipped()
    {
        if (Input.GetMouseButton(1))
        {
            bodyPos = cam.position + cam.forward * aimDistance + (body.position - aimPoint.position);
        }
        else
        {
            bodyPos = transform.position;
        }

        body.position = Vector3.Lerp(body.position, bodyPos,aimSpeed);

        if (Input.GetMouseButton(0))
        {
            TryShoot();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    private void TryShoot()
    {
        if (!(Time.time - lastFired >= 1 / fireRate) || !(curMag > 0)) return;

        if (Physics.Raycast(cam.position, cam.forward, out var hit))
        {
            print($"hit {hit.transform.gameObject.name}");
        }

        lastFired = Time.time;
        curMag--;
    }


    private void Reload()
    {
        curMag = magSize;
    }
}