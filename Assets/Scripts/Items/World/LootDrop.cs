using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootDrop : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity = Vector3.up;
    private Vector3 startPos;
    
    private Collider col;
    
    public Action onDropped;
    
    private bool dropDelayStarted;

    public void Awake()
    {
        // startPos = transform.position;
        // velocity *= Random.Range(6, 7);
        // velocity = Quaternion.LookRotation(transform.forward, Vector3.up) * velocity;
        // //velocity += new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(3, 4f));
        // velocity += Helper.GetRandPointOnUnitSphereCap(velocity, 25);
        // //velocity *= 2;
        
        // rb = GetComponent<Rigidbody>();
        // col = GetComponent<Collider>();
        // if(rb == null){
        //     rb = gameObject.AddComponent<Rigidbody>();
        // }
        // //rb.AddForce(velocity);
        
        // col.isTrigger = true;
        // rb.useGravity = false;
        // rb.isKinematic = false;
        
        enabled = false;
    }
    
    public void StartDrop()
    {
        startPos = transform.position;
        velocity *= Random.Range(6, 7);
        velocity = Quaternion.LookRotation(transform.forward, Vector3.up) * velocity;
        //velocity += new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(3, 4f));
        velocity += Helper.GetRandPointOnUnitSphereCap(velocity, 25);
        //velocity *= 2;
        
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        if(rb == null){
            rb = gameObject.AddComponent<Rigidbody>();
        }
        //rb.AddForce(velocity);
        
        col.isTrigger = true;
        rb.useGravity = false;
        rb.isKinematic = false;
        
        enabled = true;
        
        StartCoroutine(DropDelay(1f));
    }
    public void StartDrop(Vector3 direction, float angleDisp = 25f, float velocityMultiplier = 1f)
    {
        startPos = transform.position;
        velocity = direction;
        velocity *= Random.Range(6, 7);
        velocity = Quaternion.LookRotation(transform.forward, Vector3.up) * velocity;
        //velocity += new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(3, 4f));
        velocity += Helper.GetRandPointOnUnitSphereCap(velocity, angleDisp);
        velocity *= velocityMultiplier;
        
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        if(rb == null){
            rb = gameObject.AddComponent<Rigidbody>();
        }
        //rb.AddForce(velocity);
        
        col.isTrigger = true;
        rb.useGravity = false;
        rb.isKinematic = false;
        
        enabled = true;
        
        StartCoroutine(DropDelay(1f));
    }


    void FixedUpdate()
    {
        if(!dropDelayStarted){
            rb.position += velocity * Time.deltaTime;
            
            Quaternion deltaRotation = Quaternion.Euler(new Vector3(
                Random.Range(-150f, 150f), 
                Random.Range(150f, 250f),
                Random.Range(-150f, 150f)) 
                * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
        
        
        if(velocity.y < -20)
            velocity.y = -20;
        else
            velocity -= Vector3.up * 23 * Time.deltaTime;
            
        
        /*if(Mathf.Abs(rb.position.y - (startPos.y-0.5f)) < 0.25f && velocity.y < 0f && !dropDelayStarted)
        {
            //onDropped?.Invoke();
            StartCoroutine(DropDelay(0.25f));
            //col.isTrigger = true;
            //Destroy(rb);
            //Destroy(this);
            //rb.useGravity = true;
            //rb.isKinematic = false;
            //rb.linearVelocity = velocity;
            //this.enabled = false;
        }*/
    }
    
    private IEnumerator DropDelay(float delay = 0.25f)
    {
        //dropDelayStarted = true;
        yield return new WaitForSeconds(delay);
        onDropped?.Invoke();
        
        col.isTrigger = true;
        //Destroy(rb);
        //Destroy(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground")){
            rb.isKinematic = true;
            Destroy(rb);
            //StartCoroutine(DropDelay(0.25f));
            
            onDropped?.Invoke();
            col.isTrigger = true;
            Destroy(this);
        }
    }
}
