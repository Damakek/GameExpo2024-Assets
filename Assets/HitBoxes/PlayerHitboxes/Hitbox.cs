using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;


public class Hitbox : NetworkComponent
{

    public NetworkPlayerController[] players;

    public delegate void MyEventHandler();
    public static event MyEventHandler enemyHit;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "PLAYER")
        {
            if(IsClient)
            {
                players = GameObject.FindObjectsOfType<NetworkPlayerController>();
            }
        }
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        while(MyCore.IsConnected)
        {

            players = GameObject.FindObjectsOfType<NetworkPlayerController>();
            
            SendUpdate("PLAYER", "get players");

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyHit?.Invoke();

            collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.transform.position - transform.position).normalized * 20, ForceMode.Impulse);

            collision.gameObject.GetComponent<EnemyMovement>().health -= 1;

        }

        if(GameObject.Find("GameMaster").GetComponent<GameMaster>().phase_2 == true)
        {
            if(collision.gameObject.GetComponent<NetworkPlayerController>().Owner != this.Owner)
            {

                collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.transform.position - transform.position).normalized * 50, ForceMode.Impulse);

                NetworkPlayerController tempCont = collision.gameObject.GetComponent<NetworkPlayerController>();
                tempCont.health = tempCont.health - 10;
                tempCont.SendUpdate("HEALTH", tempCont.health.ToString());
            }
        }
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
