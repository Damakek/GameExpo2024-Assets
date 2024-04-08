using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class BaseEnemyBehavior : NetworkComponent
{
    GameObject[] goalObjs;
    ArrayList goals = new ArrayList();
    UnityEngine.AI.NavMeshAgent myNav = null;
    public int goal = 0;
    public int enemyNumber = 0;
    public GameObject[] players;
    public int detectionRange = 4;
    // Start is called before the first frame update
    public override void HandleMessage(string flag, string value) {

    }

    public override IEnumerator SlowUpdate() {
        while (IsServer) {
            if (players == null) {
                players = GameObject.FindGameObjectsWithTag("Player");
            }
            GameObject player = null;
            foreach(GameObject go in players) {
                if(Mathf.Pow(Mathf.Pow(go.transform.position.x - this.transform.position.x,2f) + Mathf.Pow(go.transform.position.y - this.transform.position.y,2f) + Mathf.Pow(go.transform.position.z - this.transform.position.z,2f),0.5f) < detectionRange) {
                    if(player == null || Mathf.Abs((this.transform.position - go.transform.position).magnitude) < Mathf.Abs((this.transform.position - player.transform.position).magnitude)) {
                        player = go;
                    }
                }
            }
            if (player != null) {
                myNav.destination = player.transform.position;
                myNav.Resume();
            } else if(goals.Count > 0 && myNav.remainingDistance==0) {
                goal++;
                if(goal >= goals.Count) {
                    goal = 0;
                }
                myNav.destination = (Vector3)goals[goal];
                myNav.Resume();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public override void NetworkedStart() {
        myNav = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        goalObjs = GameObject.FindGameObjectsWithTag("Goal");
        /*for(int i = 0; i < goalObjs.Length; i++) {
            if(goalObjs[i].GetComponent<GoalTagging>().tagNumber == enemyNumber) {
                goals.Add(goalObjs[i].transform.position);
            }
        }*/
        goals.Add(new Vector3(this.transform.position.x - 2, this.transform.position.y, 0));
        goals.Add(new Vector3(this.transform.position.x + 2, this.transform.position.y, 0));
        if(goals.Count > 0 && IsServer) {
            myNav.destination = (Vector3)goals[goal];
            myNav.Resume();
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
