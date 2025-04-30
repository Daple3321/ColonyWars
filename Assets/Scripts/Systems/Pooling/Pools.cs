using UnityEngine;

public static class Pools
{
    public static ParticlePool particle;
    
    public static void Init()
    {
        ClearStaticFields();
        
        GameObject go = GameObject.Instantiate(GameAssets.objectPooler);
        particle = go.GetComponent<ParticlePool>();
    }
    
    static void ClearStaticFields()
    {
        particle = null;
    }
}
