using System.Collections;
using System.Collections.Generic;
using Items;
using Mirror;
using Network_Scripts;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;
using static Helper;

namespace Player
{
    public class PlayerManager : NetworkBehaviour
    {
        private CustomNetworkManager networkManager;
        [SerializeField] private Transform cam;
        public Transform Cam => cam;

        [ReadOnly] public bool inUI;

        [SerializeField] private GameObject crossHair;

        [SerializeField] public bool isActive;

        [ReadOnly] [SerializeField] private bool isRespawning;

        [SerializeField] private MonoBehaviour[] disableOnDeath;

        private void Start()
        {
            
            networkManager = FindAnyObjectByType<CustomNetworkManager>();

            GetComponent<Health>().OnDeath += OnDeath;

            if (!isLocalPlayer) return;
            crossHair.SetActive(true);
            cam.GetComponent<Camera>().enabled = true;
            cam.GetComponent<AudioListener>().enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            OnSpawn();
        }

        private void Update()
        {
            //print($"{(isServer? "Server":"Client")} : {itemData}");

            if (!isLocalPlayer) return;

            TempHandleUi();

            if (Input.GetKeyDown(KeyCode.G))
            {
                CmdSpawnItem("Cube");
            }

            // if (!isActive && !isRespawning)
            // {
            //     OnDeath();
            // }
        }


        private void TempHandleUi()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                inUI = true;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (inUI)
                {
                    inUI = false;
                    return;
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            crossHair.SetActive(!Input.GetMouseButton(1));
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (!isLocalPlayer) return;

            collision.transform.TryGetComponent(out Item item);
            collision.transform.TryGetComponent(out NetworkIdentity itemNetID);
            if (item == null || itemNetID == null) return;

            CmdAssignAuth(itemNetID);
        }

        [Command]
        public void CmdAssignAuth(NetworkIdentity obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("Obj null in Assign Auth (probably doesnt matter tho)");
                return;
            }

            //Remove old auth and add auth for calling client
            obj.RemoveClientAuthority();
            obj.AssignClientAuthority(connectionToClient);
        }

        [Command]
        public void CmdSpawnItem(string id)
        {
            NetworkInstantiate(ItemDatabase.GetItem(id).prefab, connectionToClient);
        }

        public void OnSpawn()
        {
            transform.position = Vector3.zero;

            isActive = true;
            SetMonoB(disableOnDeath, true);
            SetChildrenState(true);
            networkManager.ServerCam.SetActive(false);
            GetComponent<Health>().CmdSetHealth(100);
            
        }
        
        [Server]
        public void OnDeath()
        {
            TRpcOnDeath();
        }

        [ClientRpc]
        private void TRpcOnDeath()
        {
            isActive = false;
            SetMonoB(disableOnDeath, false);
            SetChildrenState(false);
            networkManager.ServerCam.SetActive(true);
            StartCoroutine(IRespawnTimer());
        }

        private IEnumerator IRespawnTimer()
        {
            isRespawning = true;
            yield return new WaitForSeconds(5);
            OnSpawn();
            isRespawning = false;
        }

        private void SetMonoB(IEnumerable<MonoBehaviour> monoB, bool active)
        {
            foreach (var behaviour in monoB)
            {
                behaviour.enabled = active;
            }
        }

        private void SetChildrenState(bool active)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(active);
            }
        }
    }
}