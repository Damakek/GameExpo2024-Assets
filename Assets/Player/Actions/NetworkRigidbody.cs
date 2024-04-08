using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NETWORK_ENGINE;

public class NetworkRigidbody : NetworkComponent
{
    public float threshold = .1f;
    public float eThreshold = 2.5f;

    //synced vars
    Vector3 syncPosition;
    Vector3 syncRotation;
    Vector3 syncVelocity;
    Vector3 syncAngVelocity;

    //pointer to rigidbody
    Rigidbody myRig;
    public bool useAdapt = true;
    public Vector3 adaptVelocity;
    public Vector3 adaptAngular;

    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if(flag == "POS")
            {
                syncPosition = NetworkCore.Vector3FromString(value);
                if((syncPosition-myRig.position).magnitude > eThreshold)
                {
                    //We should think about this
                    myRig.position = syncPosition;
                }
                else if((syncPosition - myRig.position).magnitude > threshold)
                {
                    adaptVelocity = (syncPosition - myRig.position) / .1f;
                }
                else
                {
                    adaptVelocity = Vector3.zero;
                }
            }
            if(flag == "ROT")
            {
                syncRotation = NetworkCore.Vector3FromString(value);
                if((syncRotation-myRig.rotation.eulerAngles).magnitude > eThreshold && useAdapt)
                {
                    myRig.rotation = Quaternion.Euler(syncRotation);
                }
                else if((syncRotation-myRig.rotation.eulerAngles).magnitude > threshold)
                {

                }
            }
            if(flag == "VEL")
            {
                syncVelocity = NetworkCore.Vector3FromString(value);
                //if velocity == Vector3.zero;
                if (useAdapt)
                {
                    syncAngVelocity += adaptVelocity;
                }
            }
            if(flag == "ANG")
            {
                syncAngVelocity = NetworkCore.Vector3FromString(value);
            }
        }
    }

    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            
                SendUpdate("POS", myRig.position.ToString());
                syncPosition = myRig.position;
            
            
                SendUpdate("VEL", myRig.velocity.ToString());
                syncVelocity = myRig.velocity;
            

            
                SendUpdate("ROT", myRig.rotation.eulerAngles.ToString());
                syncRotation = myRig.rotation.eulerAngles;
            

            
                SendUpdate("ANG", myRig.angularVelocity.ToString());
                syncAngVelocity = myRig.angularVelocity;
            
            //Check if position has changed > threshold
            //if so send new position
            //check and see if rotation has changed > threshold
            //if so - send new rotation
            if (IsDirty)
            {
                SendUpdate("POS", myRig.position.ToString());
                SendUpdate("VEL", myRig.velocity.ToString());
                SendUpdate("ROT", myRig.rotation.eulerAngles.ToString());
                SendUpdate("ANG", myRig.angularVelocity.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient && myRig != null)
        {
            myRig.velocity = syncVelocity;
            myRig.angularVelocity = syncAngVelocity;
        }
    }
}
