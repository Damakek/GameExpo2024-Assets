using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;


public class Hitbox : NetworkComponent
{

    public NetworkPlayerController[] players;

    public delegate void MyEventHandler();
    public static event MyEventHandler enemyHit;

    public int damage;
    GameObject gameMaster;

    bool hasHitEnemy = false;

    Collider[] overlappingColliders;
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
        HandleCollision(collision);
        /*if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyHit?.Invoke();
            
            foreach(NetworkPlayerController playercharacter in players)
            {
                if(playercharacter.Owner == this.Owner)
                {
                    collision.gameObject.GetComponent<EnemyMovement>().health -= playercharacter.damage;
                }
            }
            if(!collision.gameObject.GetComponent<EnemyMovement>().isHit) 
            {
                collision.gameObject.GetComponent<EnemyMovement>().isHit = true;
                //collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.transform.position - transform.position).normalized * 20, ForceMode.VelocityChange);
            }
        }

        if (GameObject.Find("GameMaster") != null)
        {
            if(GameObject.Find("GameMaster").GetComponent<GameMaster>().phase_2 == true)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    NetworkPlayerController tempCont = collision.gameObject.GetComponent<NetworkPlayerController>();

                    foreach (NetworkPlayerController playercharacter in players)
                    {
                        if (playercharacter.Owner == this.Owner)
                        {
                            tempCont.isHit = true;
                            tempCont.health = tempCont.health - playercharacter.damage;
                        }
                    }

                    tempCont.SendUpdate("HEALTH", tempCont.health.ToString());
                }*/


        /*if(collision.gameObject.GetComponent<NetworkPlayerController>().Owner != this.Owner)
        {

            //collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.transform.position - transform.position).normalized * 50, ForceMode.Impulse);

            NetworkPlayerController tempCont = collision.gameObject.GetComponent<NetworkPlayerController>();

            foreach(NetworkPlayerController playercharacter in players)
            {
                if(playercharacter.Owner == this.Owner)
                {
                    tempCont.isHit = true;
                    tempCont.health = tempCont.health - playercharacter.damage;
                }
            }

            tempCont.SendUpdate("HEALTH", tempCont.health.ToString());
        }*/
    }
   
 
    

    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("Game Master");

        GameObject parentObject = transform.parent.gameObject;
        damage = parentObject.GetComponent<NetworkPlayerController>().damage;
        overlappingColliders = Physics.OverlapBox(transform.position, transform.localScale / 2);
        
        
        if (overlappingColliders.Length > 0)
        {
           
            for (int i = 0; i < overlappingColliders.Length; i++)
            {
                if(overlappingColliders[i] != parentObject.GetComponent<BoxCollider>())
                {
                    HandleCollision(overlappingColliders[i]);
                }
                
                
            }
        }
    }

    
       
    

    // Update is called once per frame
    void Update()
    {
        if(!hasHitEnemy && gameMaster.GetComponent<GameMaster>().phase_2 == true)

        {
            GameObject parentObject = transform.parent.gameObject;
            overlappingColliders = Physics.OverlapBox(transform.position, transform.localScale / 2);


            if (overlappingColliders.Length > 0)
            {

                for (int i = 0; i < overlappingColliders.Length; i++)
                {
                    if (overlappingColliders[i] != parentObject.GetComponent<BoxCollider>())
                    {
                        if (overlappingColliders[i].gameObject.CompareTag("Player"))
                        {


                            NetworkPlayerController tempCont = overlappingColliders[i].gameObject.GetComponent<NetworkPlayerController>();

                           
                   
                            tempCont.isHit = true;
                            tempCont.health = tempCont.health - damage;
                                
                            

                            tempCont.SendUpdate("HEALTH", tempCont.health.ToString());
                            hasHitEnemy = true;
                        }
                    }


                }
            }
        }
    }

    public void HandleCollision(Collider collision)
    {
       
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyHit?.Invoke();

            foreach (NetworkPlayerController playercharacter in players)
            {
                if (playercharacter.Owner == this.Owner)
                {
                    collision.gameObject.GetComponent<EnemyMovement>().health -= playercharacter.damage;
                }
            }
            if (!collision.gameObject.GetComponent<EnemyMovement>().isHit)
            {
                collision.gameObject.GetComponent<EnemyMovement>().isHit = true;
                //collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.transform.position - transform.position).normalized * 20, ForceMode.VelocityChange);
            }
        }

        if (GameObject.Find("GameMaster") != null)
        {
            
            if (GameObject.Find("GameMaster").GetComponent<GameMaster>().phase_2 == true)
            {
               
                if (collision.gameObject.CompareTag("Player"))
                {
                   
                    
                    NetworkPlayerController tempCont = collision.gameObject.GetComponent<NetworkPlayerController>();

                    foreach (NetworkPlayerController playercharacter in players)
                    {
                        if (playercharacter.Owner == this.Owner)
                        {
                            
                            tempCont.isHit = true;
                            tempCont.health = tempCont.health - playercharacter.damage;
                        }
                    }

                    tempCont.SendUpdate("HEALTH", tempCont.health.ToString());
                }
                /*if(collision.gameObject.GetComponent<NetworkPlayerController>().Owner != this.Owner)
                {

                    //collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.transform.position - transform.position).normalized * 50, ForceMode.Impulse);

                    NetworkPlayerController tempCont = collision.gameObject.GetComponent<NetworkPlayerController>();
                    
                    foreach(NetworkPlayerController playercharacter in players)
                    {
                        if(playercharacter.Owner == this.Owner)
                        {
                            tempCont.isHit = true;
                            tempCont.health = tempCont.health - playercharacter.damage;
                        }
                    }
                    
                    tempCont.SendUpdate("HEALTH", tempCont.health.ToString());
                }*/
            }
        }
    }
}
