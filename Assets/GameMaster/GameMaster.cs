using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;


public class GameMaster : NetworkComponent
{ 
    public bool GameStarted = false;
    public bool phase_1;
    public bool phase_2;
    public bool allPlayersReady = false;
    public NetworkPlayerManager[] players;
    public NetworkPlayerController[] characters;
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

        if(flag == "UI")
        {
            if(IsClient)
            {
                characters = GameObject.FindObjectsOfType<NetworkPlayerController>();

                foreach(NetworkPlayerController character in characters)
                {
                    if(character.Owner == 0)
                    {
                        //GameObject.Find("P1Name").GetComponent<Text>().text = character.name;
                        GameObject.Find("P1Health").GetComponent<Text>().text = character.health.ToString();
                        GameObject.Find("P1Score").GetComponent<Text>().text = character.score.ToString();
                    }
                    if (character.Owner == 1)
                    {
                        //GameObject.Find("P2Name").GetComponent<Text>().text = character.name;
                        GameObject.Find("P2Health").GetComponent<Text>().text = character.health.ToString();
                        GameObject.Find("P2Score").GetComponent<Text>().text = character.score.ToString();
                    }
                    if (character.Owner == 2)
                    {
                        //GameObject.Find("P3Name").GetComponent<Text>().text = character.name;
                        GameObject.Find("P3Health").GetComponent<Text>().text = character.health.ToString();
                        GameObject.Find("P3Score").GetComponent<Text>().text = character.score.ToString();
                    }
                    if (character.Owner == 3)
                    {
                        //GameObject.Find("P4Name").GetComponent<Text>().text = character.name;
                        GameObject.Find("P4Health").GetComponent<Text>().text = character.health.ToString();
                        GameObject.Find("P4Score").GetComponent<Text>().text = character.score.ToString();
                    }

                }
            }
        }

        if (flag == "GAMESTART")
        {
            if (IsClient)
            {

                phase_1 = true;

                foreach (NetworkPlayerManager player in players)
                {
                    player.transform.GetChild(0).gameObject.SetActive(false);
                    this.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

        if(flag == "SCORE")
        {
            if(IsClient)
            {

                this.transform.GetChild(0).gameObject.SetActive(false);
                this.transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        if (flag == "PHASE")
        {
            if (IsClient)
            {
                phase_1 = false;
                phase_2 = true;

                this.transform.GetChild(1).gameObject.SetActive(false);
                this.transform.GetChild(0).gameObject.SetActive(true);
                
            }
        }

        if (flag == "TIMER1")
        {
            if(IsClient)
            {

                phase1_done = int.Parse(value);
                this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = phase1_done.ToString();
            }
        }

        if (flag == "TIMER2")
        {
            if (IsClient)
            {

                phase2_done = int.Parse(value);
                this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = phase2_done.ToString();
            }
        }

        if (flag == "GAMEDONE")
        {
            if(IsClient)
            {
                this.transform.GetChild(0).gameObject.SetActive(false);
                this.transform.GetChild(2).gameObject.SetActive(true);
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
            characters = GameObject.FindObjectsOfType<NetworkPlayerController>();

            SendUpdate("CHECK", "check");
            SendUpdate("UI", "ui");

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

            SendUpdate("TIMER1", phase1_done.ToString());

            if(phase1_done == 0)
            {
                if(IsServer)
                {
                    phase_1 = false;
                    StartCoroutine(scoreboard());
                    SendUpdate("SCORE", "scoreboard");
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator scoreboard()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(false);

        phase_2 = true;
        SendUpdate("PHASE", "phase2");
        StartCoroutine(phase2());
    }

    
    public IEnumerator phase2()
    {
        while(phase2_done != 0)
        {

            phase2_done -= 1;
            this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = phase2_done.ToString();
            SendUpdate("TIMER2", phase2_done.ToString());

            if (phase2_done == 0)
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
        this.transform.GetChild(2).gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
    }
}
