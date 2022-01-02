using UnityEngine;


/*DELETE*/
[RequireComponent(typeof(Rigidbody))]
public class BasicSpellProjectile : MonoBehaviour {
    public float     destroyAfter = 5;
    public Rigidbody rigidBody;
    public float     force = 1000;


    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddForce(transform.forward * force);
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    // destroy for everyone on the server
    void DestroySelf() {
        GameManager.Destroy(gameObject);
    }
    
    
    void OnTriggerEnter(Collider co) {
        
    }
}
