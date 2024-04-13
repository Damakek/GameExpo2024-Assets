using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NETWORK_ENGINE;

public class NetworkTransform : NetworkComponent
{
    //Synched variables
    Vector3 syncPosition;
    Vector3 syncRotation;
    //scale

    float threshold = 0.1f;
    public float tempSpeed;


    public override void HandleMessage(string flag, string value)
    {
        if (IsClient)
        {
            if(flag == "POS")
            {
               syncPosition = NetworkCore.Vector3FromString(value);
               // float tempSpeed = (this.syncPosition - transform.position).magnitude * MyId.UpdateFrequency;
            }
            if(flag == "ROT")
            {
                syncRotation = NetworkCore.Vector3FromString(value);
            }


        }
       
    }

    public override void NetworkedStart()
    {
        if (!IsServer)
        {
            Rigidbody rd = GetComponent<Rigidbody>();
            rd.isKinematic = true;
            rd.useGravity = false;
        }
        syncPosition = this.transform.position;
        syncRotation = this.transform.rotation.eulerAngles;
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            if ((transform.position - syncPosition).magnitude > threshold)
            {
                SendUpdate("POS", this.transform.position.ToString());
                syncPosition = transform.position;
            }
            if((transform.rotation.eulerAngles - syncRotation).magnitude > threshold)
            {
                SendUpdate("ROT", this.transform.rotation.eulerAngles.ToString());
                syncRotation = this.transform.rotation.eulerAngles;
            }
            //Check if position has changed > threshold
            //if so send new position
            //check and see if rotation has changed > threshold
            //if so - send new rotation
            if (IsDirty)
            {
                SendUpdate("POS", this.transform.position.ToString());
                SendUpdate("ROT", this.transform.rotation.eulerAngles.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            //float tempSpeed = (this.syncPosition - transform.position).magnitude * MyId.UpdateFrequency;
            this.transform.position = Vector3.Lerp(this.transform.position, this.syncPosition, tempSpeed * Time.deltaTime);

            //float tempRotation = (this.syncRotation - transform.rotation.eulerAngles).magnitude * MyId.UpdateFrequency;
            this.transform.rotation = Quaternion.Euler(this.syncRotation);
            //this.transform.rotation = Quaternion.Euler(Vector3.MoveTowards(this.transform.rotation.eulerAngles, this.syncRotation, tempRotation * Time.deltaTime));
        }

    }
}
