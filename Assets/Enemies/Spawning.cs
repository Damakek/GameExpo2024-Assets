using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NETWORK_ENGINE;


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
            if (FindObjectOfType<GameMaster>().phase1_done != 0 && isRunning == false)
            {
                StartCoroutine(SpawnEnemies());
            }
        }
        
    }

    public IEnumerator SpawnEnemies() 
    {
        isRunning = true;

        yield return new WaitForSeconds(3f);

        int spawnsight = Random.Range(0, spawners.Count);
        int enemyType = Random.Range(0, 3);
        MyCore.NetCreateObject(enemyType + 7, this.Owner, spawners[spawnsight].transform.position, Quaternion.identity);
        isRunning = false;

    }
}
