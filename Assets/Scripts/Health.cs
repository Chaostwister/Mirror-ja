using System;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [ReadOnly] [SyncVar] [SerializeField] private float hp;

    public float Hp => hp;
    
    public Action OnDeath;

    [Command(requiresAuthority = false)]
    public void CmdChangeHealth(float amount)
    {
        hp += amount;
        
        TryDeath();
    }

    [Command(requiresAuthority = false)]
    public void CmdSetHealth(float amount)
    {
        hp = amount;
        
        TryDeath();
    }

    [Server]
    private void TryDeath()
    {
        if (hp <= 0 ) OnDeath?.Invoke();
    }
}
