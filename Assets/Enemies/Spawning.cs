using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NETWORK_ENGINE;
using UnityEngine.UIElements;

public class Spawning : NetworkComponent
{
    public bool isRunning = false;

    public List<Transform> spawners;

    public override void HandleMessage(string flag, string value)
    {
        
    }

    public override void NetworkedStart()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Spawner");

        if(temp != null)
        {
            for(int i = 0; i <temp.Length; i++)
            {
                spawners.Add(temp[i].transform);
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        if(IsServer)
        {
            if(IsDirty)
            {
                IsDirty = false;
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (FindObjectOfType<GameMaster>().GameStarted == true && isRunning == false)
            {
                StartCoroutine(SpawnEnemies());
            }
        }
        
    }

    public IEnumerator SpawnEnemies() 
    {
        //slope allows for minimum time of 5 seconds for 4 players
        isRunning = true;
        float time = (5f / -3f) * FindObjectOfType<GameMaster>().players.Length + 10;
        yield return new WaitForSeconds(time);

        int spawnsight = Random.Range(0, spawners.Count);
        int enemyType = Random.Range(0, 3);
        MyCore.NetCreateObject(enemyType + 7, this.Owner, spawners[spawnsight].transform.position, Quaternion.identity);
        isRunning = false;

    }
}
