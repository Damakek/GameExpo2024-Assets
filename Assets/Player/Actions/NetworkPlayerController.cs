using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;

public class NetworkPlayerController : NetworkComponent
{

    

    public int health = 50;
    public int score = 0;
    public int updatedHealth;
    public int updatedScore;
    
    public int knockback = 10;
    public int stun;

    public int scorePerHit = 100;

    public Rigidbody MyRig;
    public GameObject temp;

    public Vector2 lastDirection;
    public float speed = 5.0f;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "MOVE")
        {
            if(IsServer)
            {
                string[] numbers = value.Split(',');

                lastDirection.x = float.Parse(numbers[0]);
                if(numbers.Length > 1)
                {
                    lastDirection.y = float.Parse(numbers[1]);
                }
                else
                {
                    lastDirection.y = float.Parse(numbers[0]);
                }

                transform.forward = new Vector3(lastDirection.x, 0, lastDirection.y);

                float speed = MyRig.velocity.magnitude;
                SendUpdate("MOVE", "moving");

            }
            if(IsClient)
            {
                float speed = MyRig.velocity.magnitude;
            }
        }

        if(flag == "ATK")
        {
            if(IsServer && flag == "ATK")
            {
                StartCoroutine(Attack());

                SendUpdate("ATK", "startattack");
            }
            if(IsClient && flag == "ATK")
            {
                //Stuff
            }
        }

        if(flag == "HEALTH")
        {
            updatedHealth = int.Parse(value);

            if(IsServer)
            {
                health = updatedHealth;
                SendUpdate("HEALTH", value);
            }
            if(IsClient)
            {
                health = updatedHealth;
            }
        }

        if(flag == "SCORE")
        {
            updatedScore = int.Parse(value);

            if(IsServer)
            {
                score = updatedScore;
                SendUpdate("SCORE", value);
            }
            if(IsClient)
            {
                score = updatedScore;
            }
        }

        if(flag == "EHIT")
        {

            updatedScore = score + scorePerHit;

           if(IsClient)
           {
                SendCommand("SCORE", updatedScore.ToString());
           }
        }
    }

    public override void NetworkedStart()
    {
        MyRig = this.GetComponent<Rigidbody>();

        Hitbox.enemyHit += HandleMyEvent;

    }

    public void OnDestroy()
    {
        Hitbox.enemyHit -= HandleMyEvent;
    }

    public void HandleMyEvent()
    {
        if(IsServer)
        {
            SendUpdate("EHIT", "updateScore");
        }
    }


    public void OnDirectionChanged(InputAction.CallbackContext context)
    {

        if (IsLocalPlayer)
        {

            if (context.action.phase == InputActionPhase.Started || context.action.phase == InputActionPhase.Performed)
            {
                SendCommand("MOVE", context.ReadValue<Vector2>().x.ToString() + "," +  context.ReadValue<Vector2>().y.ToString());
            }
            if (context.action.phase == InputActionPhase.Canceled)
            {
                SendCommand("MOVE", "0");
            }
        }
        
    }

    public void OnFire(InputAction.CallbackContext context)
    {

        if(IsLocalPlayer)
        {
            if (context.action.phase == InputActionPhase.Started)
            {
                SendCommand("ATK", "start attack");
            }
        }
        
    }

    public override IEnumerator SlowUpdate()
    {
        while(MyCore.IsConnected)
        {
            if(IsServer)
            {
                if(IsDirty)
                {
                    SendUpdate("HEALTH", health.ToString());
                    
                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(.1f);
        }

    }

    public IEnumerator Attack()
    {
        
        if(temp != null)
        {
            MyCore.NetDestroyObject(temp.GetComponent<NetworkComponent>().NetId);
        }
        

        temp = MyCore.NetCreateObject(7, this.Owner, this.transform.position + this.transform.forward, Quaternion.identity);
        
        yield return new WaitForSeconds(0.1f);

        MyCore.NetDestroyObject(temp.GetComponent<NetworkComponent>().NetId);
    }

    // Start is called before the first frame update
    void Start()
    {
        MyRig = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer && MyRig != null)
        {
            MyRig.velocity = new Vector3(lastDirection.x, MyRig.velocity.y, lastDirection.y).normalized * speed;
        }
    }
}
