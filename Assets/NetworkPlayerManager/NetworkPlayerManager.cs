using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;

public class NetworkPlayerManager : NetworkComponent
{

    public GameObject newPlayerPanel;
    public NetworkPlayerManager[] players;
    public GameObject temp;

    public string PName;
    public bool IsReady;
    public int playersJoined = 1;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "READYPANEL")
        {
            if(IsLocalPlayer)
            {
                players = GameObject.FindObjectsOfType<NetworkPlayerManager>();
                
                foreach(NetworkPlayerManager player in players)
                {

                    if(player.Owner != this.Owner)
                    {
                        temp = Instantiate(newPlayerPanel);
                        temp.transform.parent = this.transform.GetChild(0).GetChild(0).GetChild(2);
                        temp.transform.GetChild(1).GetComponent<Toggle>().isOn = player.IsReady;
                        temp.transform.GetChild(1).GetComponent<Toggle>().interactable = false;
                    }
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

        if (flag == "NAME")
        {
            PName = value;
            if(IsServer)
            {
                SendUpdate("NAME", value);
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

    public override void NetworkedStart()
    {
        if(!IsLocalPlayer)
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }



        if(IsServer)
        {
           
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while(IsConnected)
        {
            if(IsServer)
            {

                players = GameObject.FindObjectsOfType<NetworkPlayerManager>();
                if (players.Length > 1 && players.Length > playersJoined)
                {
                    playersJoined++;
                    SendUpdate("READYPANEL", "add");
                }

                if (IsDirty)
                {
                    SendUpdate("NAME", PName);
                    IsDirty = false;
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
