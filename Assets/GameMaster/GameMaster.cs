using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;


public class GameMaster : NetworkComponent
{ 
    public bool GameStarted = false;
    public bool allPlayersReady = false;
    public NetworkPlayerManager[] players;
    public GameObject temp;

    public int phase1_done = 10000;
    public int phase2_done = 30;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "CHECK")
        {
            if (IsClient)
            {
                players = GameObject.FindObjectsOfType<NetworkPlayerManager>();
            }
        }

        if (flag == "GAMESTART")
        {
            if (IsClient)
            {
                foreach (NetworkPlayerManager player in players)
                {
                    player.transform.GetChild(0).gameObject.SetActive(false);
                    this.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

        if(flag == "TIMER")
        {
            if(IsClient)
            {

                phase1_done = int.Parse(value);
                this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = phase1_done.ToString();
            }
        }

        if(flag == "GAMEDONE")
        {
            if(IsClient)
            {
                this.transform.GetChild(0).gameObject.SetActive(false);
                this.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    public override void NetworkedStart()
    {
        if (IsServer)
        {
            StartCoroutine(checkForReady());
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (MyCore.IsConnected)
        {

            players = GameObject.FindObjectsOfType<NetworkPlayerManager>();

            SendUpdate("CHECK", "check");

            yield return new WaitForSeconds(.1f);
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

    public IEnumerator checkForReady()
    {
        int playersReady = 0;

        while (allPlayersReady == false)
        {
            if (players.Length != 0)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].gameObject.GetComponent<NetworkPlayerManager>().IsReady == true)
                    {
                        playersReady++;
                    }
                }

                if (playersReady == players.Length)
                {
                    allPlayersReady = true;
                    GameObject.Find("LanNetworkManager").transform.GetChild(0).gameObject.SetActive(false);
                    
                    SendUpdate("GAMESTART", "start");
                    StartCoroutine(phase1());

                    foreach(NetworkPlayerManager player in players)
                    {   
                        temp = MyCore.NetCreateObject(1, player.Owner, GameObject.Find("P" + (player.Owner + 1).ToString() + "Start").transform.position, Quaternion.identity);
                    }

                }
            }


            playersReady = 0;

            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator phase1()
    {
        while(phase1_done != 0)
        {

            this.transform.GetChild(0).gameObject.SetActive(true);
            phase1_done -= 1;
            this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = phase1_done.ToString();

            SendUpdate("TIMER", phase1_done.ToString());

            if(phase1_done == 0)
            {
                if(IsServer)
                {
                    StartCoroutine(gameDone());
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator gameDone()
    {

        SendUpdate("GAMEDONE", "done");
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
    }
}
