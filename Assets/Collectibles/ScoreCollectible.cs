using NETWORK_ENGINE;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreCollectible : BaseCollectible
{
    
    public bool hasRun = false;

    public NetworkPlayerController[] playerControllers;
    public NetworkPlayerManager[] playerManagers;
    // List<GameObject> cosmetic;

    //public bool isCosmetic = false;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        playerControllers = FindObjectsOfType<NetworkPlayerController>();
        playerManagers = FindObjectsOfType<NetworkPlayerManager>();
        SendUpdate("UPD", "");
    }

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "UPD")
        {
            if (IsClient)
            {
                playerControllers = FindObjectsOfType<NetworkPlayerController>();
                playerManagers = FindObjectsOfType<NetworkPlayerManager>();
            }
        }

        if (flag == "SPH")
        {
            if (IsClient)
            {
                if(value == "RESET")
                {
                    for (int i = 0; i < playerControllers.Length; i++)
                    {
                        playerControllers[i].scorePerHit = playerControllers[i].scorePerHit / 2;

                    }
                }
                else
                {
                    for (int i = 0; i < playerControllers.Length; i++)
                    {
                        playerControllers[i].scorePerHit = playerControllers[i].scorePerHit * 2;

                    }
                }
            }
        }
        if (flag == "DST")
        {
            if (IsClient)
            {
                this.gameObject.GetComponent<MeshCollider>().enabled = false;
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
    public override void NetworkedStart()
    {
        if (IsServer)
        {
            playerControllers = FindObjectsOfType<NetworkPlayerController>();
            playerManagers = FindObjectsOfType<NetworkPlayerManager>();
            SendUpdate("UPD", "");
        }
    }

    public override IEnumerator SlowUpdate()
    {

        if (IsServer)
        {

            if (IsDirty)
            {

                IsDirty = false;
            }
        }

        yield return new WaitForSeconds(0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {

            if (other.CompareTag("Player"))
            {
                Debug.Log("collision!");


                if (other.GetComponent<NetworkPlayerController>() != null)
                {
                    this.gameObject.GetComponent<MeshCollider>().enabled = false;
                    this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    SendUpdate("DST", "");
                    StartCoroutine(SpeedBoost());
                }
            }
        }
    }

    public override IEnumerator SpeedBoost()
    {
        
        Debug.Log("in coroutine");
        if (!hasRun)
        {

            for (int i = 0; i < playerControllers.Length; i++)
            {
                playerControllers[i].scorePerHit = playerControllers[i].scorePerHit * 2;
                SendUpdate("SPH", "");
            }
            hasRun = true;
        }

        yield return new WaitForSeconds(10f);

        Debug.Log("inside 2nd part");
        for (int i = 0; i < playerControllers.Length; i++)
        {

            playerControllers[i].scorePerHit = playerControllers[i].scorePerHit / 2;
            SendUpdate("SPH", "RESET");
        }
        
        hasRun = false;
        MyCore.NetDestroyObject(this.NetId);

    }
}
