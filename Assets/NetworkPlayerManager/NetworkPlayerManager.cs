using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;

public class NetworkPlayerManager : NetworkComponent
{

    public NetworkPlayerManager[] players;
    public NetworkPlayerController[] playerControllers;

    public string PName;
    public bool IsReady;
    public int playersJoined;

    public Text ScoreDisplay;
    public Text HealthDisplay;
    public Text SpeedDisplay;
    public Text DamageDisplay;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "CHECK")
        {
            if (IsClient)
            {
                playerControllers = GameObject.FindObjectsOfType<NetworkPlayerController>();
            }
        }


        if(flag == "NEWPLAYER")
        {
            if(IsClient)
            {
                players = GameObject.FindObjectsOfType<NetworkPlayerManager>();

                for(int i = 0; i < players.Length - 1; i++)
                {
                    this.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        if(flag == "READY")
        {
            IsReady = bool.Parse(value);
            if(IsServer)
            {
                SendUpdate("READY", value);
            }
        }

        if(flag == "HUP")
        {
            if(IsServer)
            {
                foreach(NetworkPlayerController player in playerControllers)
                {
                        Debug.Log("Setting health");
                        NetworkPlayerController tempCon = player;
                        tempCon.health += 50;
                        tempCon.score -= 50;

                        tempCon.SendUpdate("HEALTH", tempCon.health.ToString());
                        tempCon.SendUpdate("SCORE", tempCon.score.ToString());
                    
                }
            }
        }

        if (flag == "NAME")
        {
            PName = value;
            if(IsServer)
            {
                SendUpdate("NAME", value);
            }
        }

        if(flag == "UPG")
        {
            if(IsLocalPlayer)
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
                this.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                this.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
        }

        if(flag == "ST")
        {
            string updatedText = value;

            if(IsServer)
            {
                ScoreDisplay.text = updatedText;
                SendUpdate("ST", ScoreDisplay.text);
            }
            if(IsLocalPlayer)
            {
                ScoreDisplay.text = updatedText;
            }
        }

        if (flag == "HT")
        {
            string updatedText = value;

            if (IsServer)
            {
                HealthDisplay.text = updatedText;
                SendUpdate("HT", HealthDisplay.text); ;
            }
            if (IsLocalPlayer)
            {
                HealthDisplay.text = updatedText;
            }
        }

        if (flag == "SPT")
        {
            string updatedText = value;

            if (IsServer)
            {
                SpeedDisplay.text = updatedText;
                SendUpdate("ST", SpeedDisplay.text);
            }
            if (IsLocalPlayer)
            {
                SpeedDisplay.text = updatedText;
            }
        }
    } 

    public void UI_Ready(bool r)
    {
        if(IsLocalPlayer)
        {
            SendCommand("READY", r.ToString());
        }
    }

    public void UI_NameInput(string s)
    {
        if(IsLocalPlayer)
        {
            SendCommand("NAME", s);
        }
    }


    public void IncreaseHealth()
    {
       if(IsLocalPlayer)
       {
            foreach (NetworkPlayerController playercharacter in playerControllers)
            {
                if (playercharacter.Owner == this.Owner)
                {
                    playercharacter.SendCommand("SCORE", (playercharacter.score - 50).ToString());
                    playercharacter.SendCommand("HEALTH", (playercharacter.health + 5).ToString());
                }

            }
        }
    }

    public void DecreaseHealth()
    {
        if (IsClient)
        {
            foreach (NetworkPlayerController playercharacter in playerControllers)
            {
                if (playercharacter.Owner == this.Owner)
                {
                    playercharacter.SendCommand("SCORE", (playercharacter.score + 50).ToString());
                    playercharacter.SendCommand("HEALTH", (playercharacter.health - 5).ToString());
                }

            }
        }
    }

    public void IncreaseSpeed()
    {
        if (IsLocalPlayer)
        {
            foreach(NetworkPlayerController playercharacter in playerControllers)
            {
                if(playercharacter.Owner == this.Owner)
                {
                    playercharacter.SendCommand("SCORE", (playercharacter.score - 50).ToString());
                    playercharacter.SendCommand("SPEED", (playercharacter.speed + 5).ToString());
                }
                
            }

        }
    }

    public void DecreaseSpeed()
    {
        if (IsLocalPlayer)
        {
            foreach (NetworkPlayerController playercharacter in playerControllers)
            {
                if (playercharacter.Owner == this.Owner)
                {
                    playercharacter.SendCommand("SCORE", (playercharacter.score + 50).ToString());
                    playercharacter.SendCommand("SPEED", (playercharacter.speed - 5).ToString());
                }

            }
        }
    }

    /*
    public void IncreaseDamage()
    {
        if (IsServer)
        {
            SendUpdate("SCORE", (score - 50).ToString());
            SendUpdate("HEALTH", (health + 5).ToString());
        }
    }

    public void DecreaseDamage()
    {
        if (IsServer)
        {
            SendUpdate("SCORE", (score - 50).ToString());
            SendUpdate("HEALTH", (health + 5).ToString());
        }
    }
    */


    public override void NetworkedStart()
    {
        if(!IsLocalPlayer)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
        
        
        if(IsClient)
        {
            for (int i = 0; i < 3; i++)
            {
                this.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(i).gameObject.SetActive(false);
            }
        }
        
    }

    public override IEnumerator SlowUpdate()
    {
        while(IsConnected)
        {
            if(IsServer)
            {

                players = GameObject.FindObjectsOfType<NetworkPlayerManager>();

                SendUpdate("CHECK", "");

                


                playersJoined = players.Length;

                if(playersJoined > 1)
                {
                    SendUpdate("NEWPLAYER", "playerJoined");
                }

                if (IsDirty)
                {
                    SendUpdate("NAME", PName);
                    SendUpdate("ST", ScoreDisplay.text);
                    SendUpdate("HT", HealthDisplay.text);
                    SendUpdate("SPT", SpeedDisplay.text);
                    IsDirty = false;
                }
            }

            foreach(NetworkPlayerController player in playerControllers)
            {
                foreach(NetworkPlayerManager playerM in players)
                {
                    if(player.Owner == playerM.Owner)
                    {
                        playerM.ScoreDisplay.text = player.score.ToString();
                        playerM.HealthDisplay.text = player.health.ToString();
                        playerM.SpeedDisplay.text = player.speed.ToString();
                        //SendCommand("ST", player.score.ToString());
                    }
                }
            }

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
}
