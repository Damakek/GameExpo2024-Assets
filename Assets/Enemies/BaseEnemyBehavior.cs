using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Linq;

public class BaseEnemyBehavior : NetworkComponent
{
    public GameObject[] goalObjs;
    public ArrayList goals = new ArrayList();
    UnityEngine.AI.NavMeshAgent myNav = null;
    public int goal = 0;
    public int enemyNumber = 0;
    //public GameObject[] players;
    public List<NetworkPlayerController> players;
    public int detectionRange = 4;
    public bool canAtk = true;
    // Start is called before the first frame update
    public override void HandleMessage(string flag, string value) {

    }

    public override IEnumerator SlowUpdate() {
        while (IsServer) {
            /*if (players == null) {
                Debug.Log("assigning player");
                foreach(GameObject gameO in GameObject.FindGameObjectsWithTag("Player"))
                {
                    players.Add(gameO);
                }
            }*/
            GameObject player = null;
            foreach(NetworkPlayerController go in players) {
                if(Mathf.Pow(Mathf.Pow(go.transform.position.x - this.transform.position.x,2f) + Mathf.Pow(go.transform.position.y - this.transform.position.y,2f) + Mathf.Pow(go.transform.position.z - this.transform.position.z,2f),0.5f) < detectionRange) {
                    if(player == null || Mathf.Abs((this.transform.position - go.transform.position).magnitude) < Mathf.Abs((this.transform.position - player.transform.position).magnitude)) {
                        player = go.gameObject;
                    }
                }
            }
            if (player != null && canAtk) {
                myNav.destination = player.transform.position;
                myNav.Resume();
            } else if(player != null && !canAtk) {
                myNav.destination = this.transform.position - (this.transform.position - player.transform.position);
                myNav.Resume();
            } else if(goalObjs.Length > 0 && myNav.remainingDistance==0) {
                int temp = Random.Range(0, goalObjs.Length); 
                myNav.destination = goalObjs[temp].transform.position;
                myNav.Resume();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public override void NetworkedStart() {
        myNav = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        goalObjs = GameObject.FindGameObjectsWithTag("Goal");
        //for(int i = 0; i < goalObjs.Length; i++) {
            //if(goalObjs[i].GetComponent<GoalTagging>().tagNumber == enemyNumber) {
                //goals.Add(goalObjs[i].transform.position);
            //}
       // }
        //goals.Add(new Vector3(this.transform.position.x - 2, this.transform.position.y, 0));
        //goals.Add(new Vector3(this.transform.position.x + 2, this.transform.position.y, 0));
        if(goalObjs.Length > 0 && IsServer) {
            int temp = Random.Range(0, goalObjs.Length);
            myNav.destination = goalObjs[temp].transform.position;
            myNav.Resume();
        }
    }

    void OnCollisionEnter(Collision C) {
        if(C.gameObject.tag == "Player" && IsServer) {
            NetworkPlayerController tempCont = C.gameObject.GetComponent<NetworkPlayerController>();
            tempCont.health = tempCont.health - 10;
            tempCont.SendUpdate("HEALTH",tempCont.health.ToString());
            StartCoroutine(AtkCd());
        }
    }

    public IEnumerator AtkCd(float time = 0.5f) {
        canAtk = false;
        yield return new WaitForSeconds(time);
        canAtk = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Count <= 0)
        {
            Debug.Log("assigning player");
            foreach (NetworkPlayerController gameO in FindObjectsOfType<NetworkPlayerController>())
            {
                players.Add(gameO);
            }
        }
    }
}
