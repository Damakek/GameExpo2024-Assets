using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseScore : ScoreCollectible
{
    public override IEnumerator SpeedBoost()
    {

        Debug.Log("in coroutine");
        if (!hasRun)
        {

            for (int i = 0; i < playerControllers.Length; i++)
            {
                playerControllers[i].collectibleType = 5;
                playerControllers[i].hasPowerup = true;
                playerControllers[i].scorePerHit = playerControllers[i].scorePerHit / 2;
                SendUpdate("SPH", "RESET");
            }
            hasRun = true;
        }

        yield return new WaitForSeconds(10f);

        Debug.Log("inside 2nd part");
        for (int i = 0; i < playerControllers.Length; i++)
        {
            playerControllers[i].hasPowerup = false;
            playerControllers[i].scorePerHit = playerControllers[i].scorePerHit * 2;
            SendUpdate("SPH", "");
        }

        hasRun = false;
        MyCore.NetDestroyObject(this.NetId);

    }
}
