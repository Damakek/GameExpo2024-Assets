using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseSpeed : SpeedCollectible
{

    public override IEnumerator SpeedBoost()
    {
        Debug.Log("in inverse coroutine");
        if (!hasRun)
        {

            for (int i = 0; i < playerControllers.Length; i++)
            {
                
                playerControllers[i].speed = 2f;
                SendUpdate("SPD", "2");
            }
            hasRun = true;
        }

        yield return new WaitForSeconds(10f);

        Debug.Log("inside 2nd part inverse");
        for (int i = 0; i < playerControllers.Length; i++)
        {
            
            playerControllers[i].speed = 5f;
            SendUpdate("SPD", "5");
        }
        
        hasRun = false;
        MyCore.NetDestroyObject(this.NetId);

    }
}
