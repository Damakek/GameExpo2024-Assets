using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;

public class NetworkPlayerManager : NetworkComponent
{

    public NetworkPlayerManager[] players;

    public string PName;
    public bool IsReady;
    public int playersJoined;
    

    public override void HandleMessage(string flag, string value)
    {
        
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

                playersJoined = players.Length;

                if(playersJoined > 1)
                {
                    SendUpdate("NEWPLAYER", "playerJoined");
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
