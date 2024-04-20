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
    public bool upgPhase;

    public bool allPlayersReady = false;
    public int playersJoined;
    public NetworkPlayerManager[] players;
    public NetworkPlayerController[] characters;
    public List<NetworkPlayerController> playerInfo = new List<NetworkPlayerController>();
    public GameObject temp;
    
    public Text position1;
    public Text position2;
    public Text position3;
    public Text position4;

    public bool uiI = true;

    public int phase1_done = 30;
    public int phase2_done = 30;

    public Text timerPanel;

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
                GameStarted = bool.Parse(value);
                phase_1 = true;

                players = GameObject.FindObjectsOfType<NetworkPlayerManager>();

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
                /*
                this.transform.GetChild(0).gameObject.SetActive(false);
                foreach (NetworkPlayerManager player in players)
                {
                    player.transform.GetChild(0).gameObject.SetActive(true);
                    player.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    player.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                }
                //this.transform.GetChild(1).gameObject.SetActive(true);
                */
                upgPhase = bool.Parse(value);

            }
        }

        if (flag == "PHASE")
        {
            if (IsClient)
            {
                phase_1 = false;
                phase_2 = true;

                //this.transform.GetChild(1).gameObject.SetActive(false);
                foreach (NetworkPlayerManager player in players)
                {
                    player.transform.GetChild(0).gameObject.SetActive(false);
                }
                this.transform.GetChild(0).gameObject.SetActive(true);
                
            }
        }

        if (flag == "TIMER1")
        {
            if(IsClient)
            {
                phase1_done = int.Parse(value);
                timerPanel.text = phase1_done.ToString();
            }
        }

        /*
        if (flag == "TIMER2")
        {
            if (IsClient)
            {

                phase2_done = int.Parse(value);
                this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = phase2_done.ToString();
            }
        }
        */

        if(flag == "KILL")
        {
            if(IsClient)
            {
                MyCore.NetObjs[int.Parse(value)].gameObject.SetActive(false);
            }
        }

        if (flag == "GAMEDONE")
        {
            if(IsClient)
            {


                foreach(NetworkPlayerController character in characters)
                {
                    playerInfo.Add(character);
                }

                NetworkPlayerController temp;
                bool swapped;

                for (int i = 0; i < playerInfo.Count - 1; i++)
                {
                    swapped = false;
                    for (int j = 0; j < playerInfo.Count - i - 1; j++)
                    {
                        if (playerInfo[j].score > playerInfo[j + 1].score)
                        {

                            // Swap arr[j] and arr[j+1]
                            temp = playerInfo[j];
                            playerInfo[j] = playerInfo[j + 1];
                            playerInfo[j + 1] = temp;
                            swapped = true;
                        }
                    }

                    // If no two elements were
                    // swapped by inner loop, then break
                    if (swapped == false)
                        break;
                }

                for(int i = 0; i < playerInfo.Count; i++)
                {
                    if(i == 0)
                    {
                        position4.text = playerInfo[i].score.ToString();
                    }
                    if (i == 1)
                    {
                        position3.text = playerInfo[i].score.ToString();
                    }
                    if (i == 2)
                    {
                        position2.text = playerInfo[i].score.ToString();
                    }
                    if (i == 3)
                    {
                        position1.text = playerInfo[i].score.ToString();
                    }
                }

                this.transform.GetChild(0).gameObject.SetActive(false);
                this.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
    }

    public override void NetworkedStart()
    {
        
    }



    public override IEnumerator SlowUpdate()
    {

        if (IsServer)
        {
            players = GameObject.FindObjectsOfType<NetworkPlayerManager>();

            while (!GameStarted)
            {
                GameStarted = true;
                players = GameObject.FindObjectsOfType<NetworkPlayerManager>();
                if (players.Length < 2)
                {
                    GameStarted = false;
                }
                foreach (NetworkPlayerManager pm in players)
                {
                    if (!pm.IsReady)
                    {
                        GameStarted = false;
                    }
                }
                yield return new WaitForSeconds(.1f);
            }
            SendUpdate("GAMESTART", GameStarted.ToString());
            try
            {
                GameObject.Find("LanNetworkManager").transform.GetChild(0).gameObject.SetActive(false);
            }
            catch
            {

            }
            phase_1 = true;
            StartCoroutine(phase1());

            foreach (NetworkPlayerManager player in players)
            {
                temp = MyCore.NetCreateObject(1, player.Owner, GameObject.Find("P" + (player.Owner + 1).ToString() + "Start").transform.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(1);
        }
        //players = GameObject.FindObjectsOfType<NetworkPlayerManager>();
            while (IsServer)
            {
        

                SendUpdate("CHECK", "check");
                SendUpdate("UI", "ui");

                players = GameObject.FindObjectsOfType<NetworkPlayerManager>();
                characters = GameObject.FindObjectsOfType<NetworkPlayerController>();
             
                if (phase1_done == 0)
                {
                    EnemyMovement[] enemies = GameObject.FindObjectsOfType<EnemyMovement>();
                    EnemyHitbox[] eHitboxes = GameObject.FindObjectsOfType<EnemyHitbox>();

                    foreach (EnemyMovement enemy in enemies)
                    {
                        MyCore.NetDestroyObject(enemy.NetId);
                    }
                    foreach (EnemyHitbox ehitbox in eHitboxes)
                    {
                        MyCore.NetDestroyObject(ehitbox.NetId);
                    }
                }

                if (IsDirty)
                {
                    IsDirty = false;
                }
            
                yield return new WaitForSeconds(.1f);
            }
        /*
        if (IsServer)
        {
           //yield return StartCoroutine(checkForReady());
           StartCoroutine(checkForReady());
        }

        while (MyCore.IsConnected)
        {


            characters = GameObject.FindObjectsOfType<NetworkPlayerController>();
            players = GameObject.FindObjectsOfType<NetworkPlayerManager>();

            if (IsServer)
            {

                SendUpdate("CHECK", "check");
                SendUpdate("UI", "ui");


                if (phase1_done == 0)
                {
                    EnemyMovement[] enemies = GameObject.FindObjectsOfType<EnemyMovement>();
                    EnemyHitbox[] eHitboxes = GameObject.FindObjectsOfType<EnemyHitbox>();

                    foreach (EnemyMovement enemy in enemies)
                    {
                        MyCore.NetDestroyObject(enemy.NetId);
                    }
                    foreach (EnemyHitbox ehitbox in eHitboxes)
                    {
                        MyCore.NetDestroyObject(ehitbox.NetId);
                    }
                }

                if (IsDirty)
                {
                    IsDirty = false;
                }
            }

            yield return new WaitForSeconds(.1f);
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 4; i++)
        {
            if(GameObject.Find("P" + (i+1).ToString() + "Health") == null || GameObject.Find("P" + (i + 1).ToString() + "Score") == null)
            {
                uiI = false;
            }
        }


        if(uiI == true)
        {
            foreach (NetworkPlayerController character in characters)
            {
                if (character.Owner == 0 && character != null)
                {
                    //GameObject.Find("P1Name").GetComponent<Text>().text = character.name;
                    GameObject.Find("P1Health").GetComponent<Text>().text = character.health.ToString();
                    GameObject.Find("P1Score").GetComponent<Text>().text = character.score.ToString();
                }
                if (character.Owner == 1 && character != null)
                {
                    //GameObject.Find("P2Name").GetComponent<Text>().text = character.name;
                    GameObject.Find("P2Health").GetComponent<Text>().text = character.health.ToString();
                    GameObject.Find("P2Score").GetComponent<Text>().text = character.score.ToString();
                }
                if (character.Owner == 2 && character != null)
                {
                    //GameObject.Find("P3Name").GetComponent<Text>().text = character.name;
                    GameObject.Find("P3Health").GetComponent<Text>().text = character.health.ToString();
                    GameObject.Find("P3Score").GetComponent<Text>().text = character.score.ToString();
                }
                if (character.Owner == 3 && character != null)
                {
                    //GameObject.Find("P4Name").GetComponent<Text>().text = character.name;
                    GameObject.Find("P4Health").GetComponent<Text>().text = character.health.ToString();
                    GameObject.Find("P4Score").GetComponent<Text>().text = character.score.ToString();
                }
            }
        }
    }

    public IEnumerator checkForReady()
    {
        int playersReady = 0;
        if(IsServer)
        {

        }
        while (allPlayersReady == false)
        {
            players = GameObject.FindObjectsOfType<NetworkPlayerManager>();
            if (players.Length > 1)
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
                    try
                    {
                        GameObject.Find("LanNetworkManager").transform.GetChild(0).gameObject.SetActive(false);
                    }
                    catch
                    {
                       
                    }
                    allPlayersReady = true;
                    phase_1 = true;
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
            timerPanel.text = phase1_done.ToString();

            SendUpdate("TIMER1", phase1_done.ToString());

            if(phase1_done == 0)
            {
                if(IsServer)
                {
                    phase_1 = false;
                    EnemyMovement[] enemies = GameObject.FindObjectsOfType<EnemyMovement>();
                    EnemyHitbox[] eHitboxes = GameObject.FindObjectsOfType<EnemyHitbox>();

                    foreach (EnemyMovement enemy in enemies)
                    {
                        MyCore.NetDestroyObject(enemy.NetId);
                    }

                    StartCoroutine(scoreboard());
                    upgPhase = true;
                    SendUpdate("SCORE", upgPhase.ToString());
                    foreach (EnemyMovement enemy in enemies)
                    {
                        MyCore.NetDestroyObject(enemy.NetId);
                    }
                    foreach(EnemyHitbox ehitbox in eHitboxes)
                    {
                        MyCore.NetDestroyObject(ehitbox.NetId);
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator scoreboard()
    {
        if(IsServer)
        {
            EnemyMovement[] enemies = GameObject.FindObjectsOfType<EnemyMovement>();
            EnemyHitbox[] eHitboxes = GameObject.FindObjectsOfType<EnemyHitbox>();

            foreach (EnemyMovement enemy in enemies)
            {
                MyCore.NetDestroyObject(enemy.NetId);
            }
            foreach (EnemyHitbox ehitbox in eHitboxes)
            {
                MyCore.NetDestroyObject(ehitbox.NetId);
            }
        }
        

        this.transform.GetChild(0).gameObject.SetActive(false);
        
        if(IsServer)
        {
            foreach(NetworkPlayerManager player in players)
            {
                player.SendUpdate("UPG", upgPhase.ToString());
            }
        }

        if(IsServer)
        {
            EnemyMovement[] enemies = GameObject.FindObjectsOfType<EnemyMovement>();
            EnemyHitbox[] eHitboxes = GameObject.FindObjectsOfType<EnemyHitbox>();

            foreach (EnemyMovement enemy in enemies)
            {
                MyCore.NetDestroyObject(enemy.NetId);
            }
            foreach (EnemyHitbox ehitbox in eHitboxes)
            {
                MyCore.NetDestroyObject(ehitbox.NetId);
            }
        }

        yield return new WaitForSeconds(15f);

        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(false);

        upgPhase = false;
        phase_2 = true;
        SendUpdate("SCORE", upgPhase.ToString());
        SendUpdate("PHASE", "phase2");
        StartCoroutine(phase2());
    }

    
    public IEnumerator phase2()
    {


        int playersDead = 0;

        while(playersDead < players.Length - 1)
        {
            /*
            phase2_done -= 1;
            this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = phase2_done.ToString();
            SendUpdate("TIMER2", phase2_done.ToString());
            */
            foreach(NetworkPlayerController playercharacter in characters)
            {
                if(playercharacter.health <= 0)
                {
                    SendUpdate("KILL", playercharacter.GetComponent<NetworkComponent>().NetId.ToString());
                    playercharacter.gameObject.SetActive(false);
                    playersDead++;
                }
            }


            if (playersDead == players.Length - 1)
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


        StartCoroutine(MyCore.DisconnectServer());

    }
}
