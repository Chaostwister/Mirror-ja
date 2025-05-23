using System;
using Mirror;
using Player;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Items
{
    public class Gun : RangedWeapon
    {
        private PlayerManager manager;
        private GunData gunData => itemData as GunData;

        private Transform cam;
        private PlayerMovementController camController;

        [SerializeField] private Transform body;
        [SerializeField] private Transform bulletOrigin;
        [SerializeField] private Transform aimPoint;


        private float lastFired;


        [ReadOnly] [SerializeField] private Vector2 curRecoil;

        [ReadOnly] [SerializeField] private Vector2 recoilTranslation;


        [Space] [ReadOnly] [SerializeField] private float curMag;

        [Space] [SerializeField] private Transform hitPointObj;

        private Vector3 bodyPos;

        private void Start()
        {
            if (gunData == null) Debug.LogError("mismatch in item and item data", this);

            if (hitPointObj != null) hitPointObj.parent = null;
        }


        public override void OnEquip(PlayerItemController itemController)
        {
            manager = itemController.manager;
            cam = itemController.manager.Cam;
            camController = itemController.movementController;
            bodyPos = transform.position;
        }

        public override void WhileEquipped()
        {
            HandleAiming();

            HandleShooting();

            HandleRecoil();

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }

        private void HandleRecoil()
        {
            if (curRecoil.y > 0) curRecoil.y -= gunData.recoilResetSpeed;
            else curRecoil.y = 0;

            if ((curRecoil.x > 0 && curRecoil.x < gunData.recoilResetSpeed) || (curRecoil.x < 0 && curRecoil.x >
                    -gunData.recoilResetSpeed)) curRecoil.x = 0;
            else switch (curRecoil.x)
            {
                case > 0:
                    curRecoil.x -= gunData.recoilResetSpeed;
                    break;
                case < 0:
                    curRecoil.x += gunData.recoilResetSpeed;
                    break;
            }

            recoilTranslation.y = Mathf.Lerp(recoilTranslation.y, curRecoil.y, gunData.recoilTranslationSpeed);
            recoilTranslation.x = Mathf.Lerp(recoilTranslation.x, curRecoil.x, gunData.recoilTranslationSpeed);

            camController.AddCameraLayer(recoilTranslation.x, recoilTranslation.y);
        }

        private void HandleShooting()
        {
            if ((gunData.isAutomatic && Input.GetMouseButton(0)) || (!gunData.isAutomatic && Input.GetMouseButtonDown(0)))
            {
                TryShoot();
            }
        }

        private void HandleAiming()
        {
            if (Input.GetMouseButton(1))
            {
                bodyPos = cam.position + cam.forward * gunData.aimDistance + (body.position - aimPoint.position);
            }
            else
            {
                bodyPos = transform.position;
            }

            body.position = Vector3.Lerp(body.position, bodyPos, gunData.aimSpeed);
        }


        private void TryShoot()
        {
            if (!(Time.time - lastFired >= 1 / gunData.fireRate) || !(curMag > 0)) return;

            if (Physics.Raycast(cam.position, cam.forward, out var hit))
            {
                if (hitPointObj != null) hitPointObj.position = hit.point;
                //print($"hit {hit.transform.gameObject.name}");

                hit.transform.TryGetComponent(out Item item);
                hit.transform.TryGetComponent(out NetworkIdentity itemNetID);

                if (item != null && itemNetID != null)
                {
                    manager.CmdAssignAuth(itemNetID);
                    item.rb.AddForce(cam.forward * gunData.itemKnockBackForce, ForceMode.Impulse);
                }

                hit.transform.TryGetComponent(out Health health);
                if (health != null)
                {
                    health.CmdChangeHealth(-gunData.damage);
                }
            }

            lastFired = Time.time;
            curMag--;
            
            curRecoil.y += gunData.recoilPerShot;
            if (curRecoil.y > gunData.maxRecoil.y) curRecoil.y = gunData.maxRecoil.y;

            curRecoil.x +=  (Random.value > 0.5f ? 1f : -1f) * curRecoil.y * gunData.verticalRecoilMulti;
            if (curRecoil.x > gunData.maxRecoil.x) curRecoil.x = gunData.maxRecoil.x;
            else if (curRecoil.x < -gunData.maxRecoil.x) curRecoil.x = -gunData.maxRecoil.x;
        }


        private void Reload()
        {
            curMag = gunData.magSize;
        }
    }
}