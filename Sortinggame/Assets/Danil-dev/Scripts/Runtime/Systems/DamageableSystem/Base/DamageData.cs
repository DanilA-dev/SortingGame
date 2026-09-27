using UnityEngine;

namespace D_Dev.DamageableSystem
{
    [System.Serializable]
    public struct DamageData
    {
       public int Damage;
       public GameObject DamageSource;
       public float Force;
       public Vector3 ForceDirection;
       public ForceMode ForceMode;
    }
}