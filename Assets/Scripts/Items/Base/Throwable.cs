using UnityEngine;

public class Throwable : Usable
{
    public ThrowableData throwableData;
    public Throwable(ItemData data) : base(data)
    {
        Init(data);
        
        if(data is ThrowableData td){
            throwableData = td;
        }
    }
    
    protected override void OnUse()
    {
        base.OnUse();
        
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        GameObject go = GameObject.Instantiate(throwableData.prefab, GameController.p.shootPoint.transform.position, Quaternion.identity);
        go.transform.forward = mouseRay.direction;
        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.AddForce(mouseRay.direction*throwableData.throwForce);
    }
}
