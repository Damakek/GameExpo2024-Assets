using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;


public class Hitbox : NetworkComponent
{
    public delegate void MyEventHandler();
    public static event MyEventHandler enemyHit;

    public override void HandleMessage(string flag, string value)
    {
        throw new System.NotImplementedException();
    }

    public override void NetworkedStart()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator SlowUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            enemyHit?.Invoke();
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
