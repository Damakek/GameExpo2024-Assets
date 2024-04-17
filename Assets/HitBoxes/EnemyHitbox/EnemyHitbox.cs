using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class EnemyHitbox : NetworkComponent
{
    public override void HandleMessage(string flag, string value)
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player") == true)
        {
            if(!collision.gameObject.GetComponent<NetworkPlayerController>().isBlocking == true)
            {
                NetworkPlayerController tempCont = collision.gameObject.GetComponent<NetworkPlayerController>();
                if(tempCont.canAtk == true)
                {
                    tempCont.isHit = true;
                    tempCont.health = tempCont.health - 10;
                    tempCont.SendUpdate("HEALTH", tempCont.health.ToString());
                }
                /*tempCont.isHit = true;
                tempCont.health = tempCont.health - 10;
                tempCont.SendUpdate("HEALTH", tempCont.health.ToString());*/
            }
       
        }
    }

    public override void NetworkedStart()
    {
        //stuff
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
