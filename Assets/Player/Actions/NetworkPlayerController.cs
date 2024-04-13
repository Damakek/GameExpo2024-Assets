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

    public int position;

    public int knockback = 10;
    public int stun;

    public int scorePerHit = 100;

    public Rigidbody MyRig;
    public GameObject temp;

    public Vector2 lastDirection;
    public float speed;

    public Animator MyAnime;
    public float animationSpeed;

    public Vector3 lastFace = Vector3.zero;
    public bool isMoving = false;
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
                string[] numbers = value.Split(',');
                if(value != "0,0")
                {
                    isMoving = true;
                    animationSpeed = Mathf.Max(Mathf.Abs(float.Parse(numbers[0])), Mathf.Abs(float.Parse(numbers[1])));
                    float speed = MyRig.velocity.magnitude;
                }
                else
                {
                    isMoving = false;
                    animationSpeed = Mathf.Max(0, 0);
                }

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

           if(IsServer)
           {
                score = updatedScore;
                SendUpdate("SCORE", score.ToString());
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
        Hitbox[] hitboxes = GameObject.FindObjectsOfType<Hitbox>();

        foreach(Hitbox hitbox in hitboxes)
        {
            if(hitbox.Owner == this.Owner)
            {
                SendCommand("EHIT", "enemy hit");
            }
        }
    }


    public void OnDirectionChanged(InputAction.CallbackContext context)
    {

        if (IsLocalPlayer)
        {

            if (context.action.phase == InputActionPhase.Started || context.action.phase == InputActionPhase.Performed)
            {
                SendCommand("MOVE", context.ReadValue<Vector2>().x.ToString() + "," +  context.ReadValue<Vector2>().y.ToString());
                animationSpeed = Mathf.Max(Mathf.Abs(context.ReadValue<Vector2>().x), Mathf.Abs(context.ReadValue<Vector2>().y));

            }
            if (context.action.phase == InputActionPhase.Canceled)
            {
                SendCommand("MOVE", "0,0");
                animationSpeed =  Mathf.Max(Mathf.Abs(0), Mathf.Abs(0));
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
                    SendUpdate("SCORE", score.ToString());
                    
                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(.5f);
        }

    }

    public IEnumerator Attack()
    {
        
        if(temp != null)
        {
            MyCore.NetDestroyObject(temp.GetComponent<NetworkComponent>().NetId);
        }
        

        if(position == 0)
        {
            temp = MyCore.NetCreateObject(14, this.Owner, this.transform.position + this.transform.forward, Quaternion.identity);
        }
        else if(position == 1)
        {
            temp = MyCore.NetCreateObject(14, this.Owner, this.transform.position + this.transform.forward, Quaternion.identity);
        }
        else if (position == 2)
        {
            temp = MyCore.NetCreateObject(6, this.Owner, this.transform.position + this.transform.forward, Quaternion.identity);
        }
        else if (position == 3)
        {
            temp = MyCore.NetCreateObject(13, this.Owner, this.transform.position + this.transform.forward, Quaternion.identity);
        }
        else if (position == 4)
        {
            temp = MyCore.NetCreateObject(12, this.Owner, this.transform.position + this.transform.forward, Quaternion.identity);
        }
        

        
        yield return new WaitForSeconds(0.1f);

        if(temp != null)
        {
            MyCore.NetDestroyObject(temp.GetComponent<NetworkComponent>().NetId);
        }
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
            Vector3 tv = new Vector3(lastDirection.x, 0, lastDirection.y).normalized * speed;
            MyRig.velocity = tv + new Vector3(0, MyRig.velocity.y, 0);

            if(tv  == Vector3.zero)
            {
                transform.forward = lastFace;
            }
            else
            {
                transform.forward = tv;
                lastFace = tv;
            }
        }

        if (IsClient)
        {
            MyAnime.SetFloat("speedh", animationSpeed);
            

        }
    }
}
