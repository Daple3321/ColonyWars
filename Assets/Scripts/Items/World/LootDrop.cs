using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootDrop : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity = Vector3.up;
    private Vector3 startPos;
    
    private Collider col;
    
    public Action onDropped;

    public void Awake()
    {
        startPos = transform.position;
        velocity *= Random.Range(4f, 6f);
        velocity += new Vector3(Random.Range(-1f, 1f), 0, Random.Range(1, 2f));
        
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        if(rb == null){
            rb = gameObject.AddComponent<Rigidbody>();
        }
        col.isTrigger = true;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        rb.position += velocity * Time.deltaTime;
        
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(
            Random.Range(-150f, 150f), 
            Random.Range(150f, 250f),
            Random.Range(-150f, 150f)) 
            * Time.deltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
        
        if(velocity.y < -20)
            velocity.y = -20;
        else
            velocity -= Vector3.up * 23 * Time.deltaTime;
            
        
        if(Mathf.Abs(rb.position.y - (startPos.y-1f)) < 0.25f && velocity.y < 0f)
        {
            onDropped?.Invoke();
            col.isTrigger = true;
            Destroy(rb);
            Destroy(this);
            //rb.useGravity = true;
            //rb.isKinematic = false;
            //rb.linearVelocity = velocity;
            //this.enabled = false;
        }
    }
}
