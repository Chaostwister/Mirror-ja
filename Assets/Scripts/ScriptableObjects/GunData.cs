using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    
    [CreateAssetMenu(menuName = "Items/Gun")]
    public class GunData : RangedWeaponData
    {
        [Space]
        [Header("BASICS")]
        public float damage = 20;
        public float fireRate = 5;
        public float magSize = 15;
        public bool isAutomatic;
        [Space]
        
        [Header("AIMING")]
        public float aimDistance = 0.05f;
        public float aimSpeed = .5f;
        [Space]
        
        [Header("RECOIL")]
        public float recoilResetSpeed = .5f;
        public float recoilPerShot = 5;
        public float verticalRecoilMulti = 0.1f;
        public Vector2 maxRecoil = new Vector2(50,180);
        public float recoilTranslationSpeed = .25f;
        [Space]
        
        [Header("MISC")]
        public float itemKnockBackForce = 5;
    }
}