using NETWORK_ENGINE;
using System;
using System.Collections;
using System.Xml;
using UnityEngine;
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
    public int damage = 5;
    public int updatedDamage;


    public Rigidbody MyRig;
    public GameObject temp;

    public Vector2 lastDirection;
    public float speed;
    public float speedUpdated;

    public Animator MyAnime;
    public float animationSpeed;

    public Vector3 lastFace = Vector3.zero;
    public bool isMoving = false;
    public bool isBlocking = false;
    public bool canMove = true;

    public bool canAtk = true;

    public Vector2 lastMoveCmd = Vector2.zero;

    public bool isHit = false;
    public bool isHitRunning = false;

    public bool hasPowerup = false;
    public int collectibleType;
    public bool isPowerUpRunning = false;
    public override void HandleMessage(string flag, string value)
    {

        
        if(flag == "SPEED")
        {
            speedUpdated = float.Parse(value);

            if(IsServer)
            {
                speed = speedUpdated;
                SendUpdate("SPEED", speed.ToString());
            }
            if(IsClient)
            {
                speed = speedUpdated;
            }
        }
        
        if (flag == "MOVE")
        {
            if (IsServer)
            {
                if (isBlocking)
                {
                    if(value != "-1,-1")
                    {
                        lastDirection.x = 0;
                        lastDirection.y = 0;
                        SendUpdate("MOVE", "-1,-1");
                    }
                }
                if(!isBlocking && canMove)
                {
                    if(value != "-1,-1")
                    {
                        string[] numbers = value.Split(',');
                        
                        lastDirection.x = float.Parse(numbers[0]);
                        if (numbers.Length > 1)
                        {
                            lastDirection.y = float.Parse(numbers[1]);
                        }
                        else
                        {
                            lastDirection.y = float.Parse(numbers[0]);
                        }

                        transform.forward = new Vector3(lastDirection.x, 0, lastDirection.y);
                        animationSpeed = float.Parse(numbers[2]);

                        float speed = MyRig.velocity.magnitude;
                        SendUpdate("MOVE", lastDirection.x.ToString() +"," + lastDirection.y.ToString() + "," + animationSpeed.ToString());
                    }
                    
                }
            }
            if (IsClient)
            {
                if (!isBlocking && canMove)
                {
                    string[] numbers = value.Split(',');
                    if (new Vector2(float.Parse(numbers[0]), float.Parse(numbers[1])) != Vector2.zero)
                    {
                        MyAnime.SetBool("isMoving", true);
                        isMoving = true;
                        //animationSpeed = Mathf.Max(Mathf.Abs(float.Parse(numbers[0])), Mathf.Abs(float.Parse(numbers[1])));
                        animationSpeed = float.Parse(numbers[2]);
                        float speed = MyRig.velocity.magnitude;
                    }
                    else
                    {
                        isMoving = false;
                        MyAnime.SetBool("isMoving", false);
                        animationSpeed = Mathf.Max(0, 0);
                    }
                }
            }
        }

        if(flag == "CHLD")
        {
            if (IsClient)
            {
                MyCore.NetObjs[int.Parse(value)].gameObject.transform.parent = this.transform;
            }
        }

        if (flag == "ATK")
        {
            if (IsServer && flag == "ATK")
            {
                if (!isBlocking)
                {
                    if (canAtk)
                    {
                        StartCoroutine(Attack());
                    }
                    

                    SendUpdate("ATK", "startattack");
                }
            }
            if (IsClient && flag == "ATK")
            {
                if (!isBlocking)
                {
                    canAtk = false;
                    MyAnime.SetTrigger("Attack");
                    StartCoroutine(AtkStop());
                }
            }
        }

        if(flag == "BLK")
        {
            if(IsServer && flag  == "BLK")
            {
                if (value == "end")
                {
                    isBlocking = false;
                }
                else
                {
                    isBlocking = true;
                    
                }
           
                SendUpdate("BLK", isBlocking.ToString());
            }
            if(IsClient && flag == "BLK")
            {
                isBlocking = bool.Parse(value);
                MyAnime.SetBool("isBlocking", isBlocking);
                if (isBlocking)
                {
                    SendCommand("MOVE", "0,0");
                }
                else
                {
                    SendCommand("MOVE", lastMoveCmd.x.ToString() + "," + lastMoveCmd.y.ToString());

                }
            }
        }

        if (flag == "HEALTH")
        {
            updatedHealth = int.Parse(value);

            if (IsServer)
            {
                health = updatedHealth;
                SendUpdate("HEALTH", value);
            }
            if (IsClient)
            {
                health = updatedHealth;
            }
        }

        if (flag == "SCORE")
        {
            updatedScore = int.Parse(value);

            if (IsServer)
            {
                score = updatedScore;
                SendUpdate("SCORE", value);
            }
            if (IsClient)
            {
                score = updatedScore;
            }
        }

        if(flag == "DAMAGE")
        {
            updatedDamage = int.Parse(value);

            if(IsServer)
            {
                damage = updatedDamage;
                SendUpdate("DAMAGE", value);
            }
            if(IsClient)
            {
                damage = updatedDamage;
            }

        }
        if (flag == "CLDMOVE") {
            if(IsClient) {
                canMove = bool.Parse(value);
            }
        }
        if (flag == "EHIT")
        {

            updatedScore = score + scorePerHit;

            if (IsServer)
            {
                score = updatedScore;
                SendUpdate("SCORE", score.ToString());
            }
        }
        if(flag == "HIT")
        {
            if (IsClient)
            {
                MyAnime.SetTrigger("Flinch");
            }
            
        }
        if(flag == "CLDOWN")
        {
            if (IsClient)
            {
                canAtk = true;
            }
        }

        if (flag == "COLLEC")
        {
            if (IsClient)
            {
                MyCore.NetObjs[int.Parse(value)].gameObject.GetComponent<BoxCollider>().enabled = false;
                MyCore.NetObjs[int.Parse(value)].gameObject.transform.parent = this.transform;
                MyCore.NetObjs[int.Parse(value)].gameObject.transform.position = this.transform.position + new Vector3(0, 7f, 0);

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

        foreach (Hitbox hitbox in hitboxes)
        {
            if (hitbox.Owner == this.Owner)
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
                lastMoveCmd = new Vector2(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
                animationSpeed = Mathf.Max(Mathf.Abs(context.ReadValue<Vector2>().x), Mathf.Abs(context.ReadValue<Vector2>().y));
                SendCommand("MOVE", context.ReadValue<Vector2>().x.ToString() + "," + context.ReadValue<Vector2>().y.ToString() + "," + animationSpeed.ToString());
                //animationSpeed = Mathf.Max(Mathf.Abs(context.ReadValue<Vector2>().x), Mathf.Abs(context.ReadValue<Vector2>().y));

            }
            if (context.action.phase == InputActionPhase.Canceled)
            {
                lastMoveCmd = Vector2.zero;
                animationSpeed = Mathf.Max(Mathf.Abs(0), Mathf.Abs(0));
                SendCommand("MOVE", "0,0" + "," + animationSpeed.ToString());
                //animationSpeed = Mathf.Max(Mathf.Abs(0), Mathf.Abs(0));
            }
        }

    }

    public void OnFire(InputAction.CallbackContext context)
    {

        if (IsLocalPlayer)
        {
            if (context.action.phase == InputActionPhase.Started)
            {
                if (canAtk && GameObject.FindObjectOfType<GameMaster>().upgPhase == false)
                {
                    SendCommand("ATK", "start attack");
                }
                
            }
        }

    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if(IsLocalPlayer)
        {
            if(context.action.phase == InputActionPhase.Performed)
            {
                SendCommand("BLK", "start block");
            }
            if(context.action.phase == InputActionPhase.Canceled)
            {
                SendCommand("BLK", "end");
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while(MyCore.IsConnected)
        {
            if (IsServer)
            {
                if (health <= 0)
                {
                    score -= 100;
                    SendUpdate("SCORE", (this.score - 100).ToString());
                    if (GameObject.FindObjectOfType<GameMaster>().phase_1 == true)
                    {
                        health = 50;
                        SendUpdate("HEALTH", 50.ToString());
                    }
                }


            }

            if (IsServer)
            {
                if(IsDirty)
                {
                    SendUpdate("HEALTH", health.ToString());
                    SendUpdate("SCORE", score.ToString());
                    SendUpdate("SPEED", speed.ToString());
                    SendUpdate("DAMAGE", damage.ToString());
                    
                    IsDirty = false;
                }
            }
            yield return new WaitForSeconds(.5f);
        }

    }

    public IEnumerator AtkStop() {
        if(isMoving) {
            yield return new WaitForSeconds(1f);
            canMove = false;
            yield return new WaitForSeconds(.5f);
        } else {
            canMove = false;
            yield return new WaitForSeconds(1f);
        }
        canMove = true;
    }
    public IEnumerator Attack()
    {
        bool moveAtk = false;
        if (canAtk)
        {
            if (temp != null)
            {
                MyCore.NetDestroyObject(temp.GetComponent<NetworkComponent>().NetId);
            }

            temp = MyCore.NetCreateObject(6, this.Owner, this.transform.position + this.transform.forward + new Vector3(1,2,1), Quaternion.identity);
            
            temp.transform.parent = this.transform;
            SendUpdate("CHLD", temp.GetComponent<NetworkID>().NetId.ToString());
            canAtk = false;
            if(MyRig.velocity.magnitude > 0.05) {
                moveAtk = true;
            }
            canMove = moveAtk;
            //SendUpdate("CLDMOVE",canMove.ToString());
        }
        yield return new WaitForSeconds(1f);
        canMove = moveAtk;
        yield return new WaitForSeconds(0.5f);

        if (temp != null)
        {
            MyCore.NetDestroyObject(temp.GetComponent<NetworkComponent>().NetId);
        }
        canAtk = true;
        SendUpdate("CLDOWN", "");
        canMove = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        MyRig = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isHit)
        {
            SendUpdate("HIT", "");
            StartCoroutine(Hit());
        }

        if(hasPowerup && !isPowerUpRunning)
        {
            StartCoroutine(SpawnPowerup());
        }

        

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
            //MyAnime.SetFloat("speedh", 1f);

        }
    }

    public IEnumerator Hit()
    {
        isHitRunning = true;

        yield return new WaitForSeconds(0.7f);

        isHit = false;
        isHitRunning = false;
    }

    public IEnumerator SpawnPowerup()
    {
        isPowerUpRunning = true;
        GameObject collec = MyCore.NetCreateObject(collectibleType, Owner, this.transform.position + new Vector3(0, 7f, 0), Quaternion.identity);
        collec.transform.parent = this.transform;
        collec.transform.position = this.transform.position + new Vector3(0, 7f, 0);
        SendUpdate("COLLEC", collec.GetComponent<NetworkID>().NetId.ToString());
        collec.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(10f);

        MyCore.NetDestroyObject(collec.GetComponent<NetworkID>().NetId);
        hasPowerup = false;
        isPowerUpRunning = false;
    }
}
