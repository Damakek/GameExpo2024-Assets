using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NETWORK_ENGINE;
using UnityEngine.AI;


public class EnemyMovement : NetworkComponent
{

    public NavMeshAgent MyAgent;
    public List<Vector3> Goals;
    public Vector3 CurrentGoal;
    public List<NetworkPlayerController> players;
    public bool isMoving = false;
    public bool isFrozen = false;
    public int targetInd;
    public bool canAtk = true;
    public int detectionRange;
    public bool isHit = false;
    public bool isHitRunning = false;

    public GameObject[] collectiblePrefabs;
    public GameObject temp;


    public int health = 5;


    public Animator MyAnime;

    public override void HandleMessage(string flag, string value)
    {
        if(IsClient)
        {
            if(flag == "MV")
            {
                if(value == "HI")
                {
                    //MyAnime.Play("Idle - Run");
                    MyAnime.SetFloat("speedh", 1);
                }
                if(value == "SP")
                {
                    MyAnime.SetFloat("speedh", 0);
                }
            }

            if(flag == "HIT")
            {
                MyAnime.SetTrigger("Flinch");
            }
            if(flag == "ATK")
            {
                MyAnime.SetTrigger("Attack");
            }
        }
    }

    public override void NetworkedStart()
    {
        if (IsClient)
        {
            MyAnime.SetFloat("speedh", 1f);
        }
    }

    public override IEnumerator SlowUpdate()
    {
       

            yield return new WaitForSeconds(0.1f);
        
    }

        // Start is called before the first frame update
        void Start()
        {
            GameObject[] temp = GameObject.FindGameObjectsWithTag("Goal");
            Goals = new List<Vector3>();
            foreach (GameObject g in temp)
            {
                Goals.Add(g.transform.position);
            }

            MyAnime.Play("Idle - Run");
        }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            HealthCheck();
        }
        

        if(health != 0)
        {
            if (players.Count <= 0)
            {
                //Debug.Log("assigning player");
                foreach (NetworkPlayerController gameO in FindObjectsOfType<NetworkPlayerController>())
                {
                    players.Add(gameO);
                }
            }

            if (IsServer)
            {
                if (isHit && !isHitRunning)
                {

                    StartCoroutine(BeenHit());
                }

                if (!isHit || canAtk)
                {
                    foreach (NetworkPlayerController go in players)
                    {
                        if (Vector3.Distance(go.transform.position, MyAgent.transform.position) <= detectionRange)
                        {
                            //Debug.Log(Vector3.Distance(go.transform.position, MyAgent.transform.position));
                            MyAgent.SetDestination(go.transform.position);
                            isMoving = true;
                            SendUpdate("MV", "HI");
                        }

                        if (canAtk == true && Vector3.Distance(go.transform.position, MyAgent.transform.position) < 3)
                        {
                            SendUpdate("ATK", "ATK");
                            StartCoroutine(AtkCd());
                        }
                    }

                    //Vector3 direction = Goals[targetInd] - transform.position;
                    //transform.forward = direction;

                    if (isMoving == false)
                    {
                        targetInd = PickTarget();
                        MyAgent.SetDestination(Goals[targetInd]);
                        isMoving = true;
                        //MyAnime.SetFloat("speedh", 1);
                        Vector3 direction = Goals[targetInd] - transform.position;
                        transform.forward = direction;
                        SendUpdate("MV", "HI");

                    }
                    else if (MyAgent.remainingDistance <= MyAgent.stoppingDistance)
                    {
                        isMoving = false;
                        //MyAnime.SetFloat("speedh", 0);
                        SendUpdate("MV", "SP");
                    }
                }
            }
        }
        
    }

    public int PickTarget()
    {
        int temp = Random.Range(0, Goals.Count);
        return temp;
    }

    public IEnumerator AtkCd(float time = 1.5f)
    {
        MyAgent.isStopped = true;
        if(temp != null)
        {
            MyCore.NetDestroyObject(temp.GetComponent<NetworkComponent>().NetId);
        }

        canAtk = false;

        /*if(IsServer)
        {
            SendUpdate("MV", "ATK");
        }*/

        temp = MyCore.NetCreateObject(15, -1, this.transform.position + this.transform.forward, Quaternion.identity);


        yield return new WaitForSeconds(time);

        if (temp != null)
        {
            MyCore.NetDestroyObject(temp.GetComponent<NetworkComponent>().NetId);
        }

        MyAgent.isStopped = false;
        canAtk = true;
        
        
    }

    public void HealthCheck()
    {
        if (health == 0)
        {
            int odds = Random.Range(0, 99);
            if(odds < 25)
            {
                int ind = Random.Range(0, collectiblePrefabs.Length);
                temp  = MyCore.NetCreateObject(ind + 2, Owner, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity);
                temp.transform.localScale = new Vector3(12,12,12);
                MyCore.NetDestroyObject(this.NetId);
            }
            
        }
    }

    public IEnumerator BeenHit()
    {
        isHitRunning = true;
        MyAgent.isStopped = true;
        SendUpdate("HIT", "");
        yield return new WaitForSeconds(2.5f);
        MyAgent.isStopped = false;
        isHitRunning = false;
        isHit = false;
    }
}
