using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NETWORK_ENGINE;
public abstract class BaseCollectible : NetworkComponent
{
    

    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
      
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract IEnumerator SpeedBoost();
}
