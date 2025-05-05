using Mirror;
using UnityEngine;
using static Helper;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private Transform cam;
    public Transform Cam => cam;

    public bool inUI;

    private void Start()
    {
        if (!isLocalPlayer) return;
        cam.GetComponent<Camera>().enabled = true;
        cam.GetComponent<AudioListener>().enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //print($"{(isServer? "Server":"Client")} : {itemData}");

        if (!isLocalPlayer) return;

        TempHandleUi();

        if (Input.GetKeyDown(KeyCode.G))
        {
            CmdSpawnItem("item sphere");
        }
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
        print($"CmdAssignAuth called. isClient: {isClient}, isServer: {isServer}");
        if (!isServer)
        {
            Debug.LogWarning("not called on server????");
            return;
        } // if cmd isn't called on server return

        if (obj == null)
        {
            Debug.LogWarning("Obj null in Assign Auth (probably doesnt matter tho)");
            return;
        }

        //Remove old auth and add auth for calling client
        obj.RemoveClientAuthority();
        obj.AssignClientAuthority(connectionToClient);
        print(obj.connectionToClient);
    }

    [Command]
    public void CmdSpawnItem(string id)
    {
        NetworkInstantiate(ItemDatabase.GetItem(id).prefab, connectionToClient);
    }
}