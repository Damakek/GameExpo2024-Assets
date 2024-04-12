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
    public int detectionRange = 4;

    public GameObject[] collectiblePrefabs;


    public int health = 50;


    public Animator MyAnime;

    public override void HandleMessage(string flag, string value)
    {
        if(IsClient)
        {
            if(flag == "MV")
            {
                if(value == "HI")
                {
                    MyAnime.Play("Run");
                }
            }
        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        /*while (IsServer)
        {
            GameObject player = null;
            foreach (NetworkPlayerController go in players)
            {
                if (Mathf.Pow(Mathf.Pow(go.transform.position.x - this.transform.position.x, 2f) + Mathf.Pow(go.transform.position.y - this.transform.position.y, 2f) + Mathf.Pow(go.transform.position.z - this.transform.position.z, 2f), 0.5f) < detectionRange)
                {
                    if (player == null || Mathf.Abs((this.transform.position - go.transform.position).magnitude) < Mathf.Abs((this.transform.position - player.transform.position).magnitude))
                    {
                        player = go.gameObject;
                    }
                }
            }
            if (player != null && canAtk)
            {
                MyAgent.destination = player.transform.position;
                isMoving = true;
                MyAgent.Resume();
            }
            else if (player != null && !canAtk)
            {
                MyAgent.destination = this.transform.position - (this.transform.position - player.transform.position);
                isMoving = true;
                MyAgent.Resume();
            }*/

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

        MyAnime.Play("Run");
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
                Debug.Log("assigning player");
                foreach (NetworkPlayerController gameO in FindObjectsOfType<NetworkPlayerController>())
                {
                    players.Add(gameO);
                }
            }

            if (IsServer)
            {
                foreach (NetworkPlayerController go in players)
                {
                    if (Vector3.Distance(go.transform.position, MyAgent.transform.position) <= detectionRange)
                    {
                        //Debug.Log(Vector3.Distance(go.transform.position, MyAgent.transform.position));
                        MyAgent.SetDestination(go.transform.position);
                        isMoving = true;
                    }

                }

                if (isMoving == false)
                {
                    targetInd = PickTarget();
                    MyAgent.SetDestination(Goals[targetInd]);
                    isMoving = true;
                    Vector3 direction = Goals[targetInd] - transform.position;
                    transform.forward = direction;
                    SendUpdate("MV", "HI");

                }
                else if (MyAgent.remainingDistance <= MyAgent.stoppingDistance)
                {
                    isMoving = false;
                }

            }
        }
        
    }

    public int PickTarget()
    {
        int temp = Random.Range(0, Goals.Count);
        return temp;
    }

    void OnCollisionEnter(Collision C)
    {
        if (C.gameObject.tag == "Player" && IsServer)
        {
            NetworkPlayerController tempCont = C.gameObject.GetComponent<NetworkPlayerController>();
            tempCont.health = tempCont.health - 10;
            tempCont.SendUpdate("HEALTH", tempCont.health.ToString());
            StartCoroutine(AtkCd());
        }
    }

    public IEnumerator AtkCd(float time = 0.5f)
    {
        canAtk = false;
        yield return new WaitForSeconds(time);
        canAtk = true;
    }

    public void HealthCheck()
    {
        if (health == 0)
        {
            int odds = Random.Range(0, 99);
            if(odds < 99)
            {
                int ind = Random.Range(0, collectiblePrefabs.Length);
                MyCore.NetCreateObject(ind + 2, Owner, new Vector3(this.transform.position.x, this.transform.position.y - 0.3f, this.transform.position.z), Quaternion.identity);
                MyCore.NetDestroyObject(this.NetId);
            }
            
        }
    }
}
