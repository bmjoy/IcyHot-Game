using UnityEngine;



[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class SpellCast : MonoBehaviour, IPoolableObject {
    public float              destroyAfter = 5;
    public Rigidbody          spellBody;
    public float              force = 1000;
    public ConditionalEffects triggerCondition;

    public GameObject ExplosionPrefab;
    public float      DestroyExplosion = 4.0f;
    public float      DestroyChildren  = 2.0f;

    
    // UNUSED
    public void Initiate() { throw new System.NotImplementedException(); }
    
    

    private void Start() {
        spellBody = GetComponent<Rigidbody>();
        spellBody.AddForce(transform.forward * force);
        Invoke(nameof(DestroySelf), destroyAfter);
    }


    // destroy for everyone on the server
    void DestroySelf() {
        GameManager.Destroy(gameObject);
    }


    // ServerCallback because we don't want a warning if OnTriggerEnter is
    // called on the client
    /*[ServerCallback]
    private void OnTriggerEnter(Collider co) {
        NetworkServer.Destroy(spell);
    }*/
    

    private void OnCollisionEnter(Collision other) {
        // triggerCondition.OnHit(other.gameObject);
        spellBody.velocity = Vector3.zero;
        spellBody.angularVelocity = Vector3.zero;
        spellBody.isKinematic = true;

        GameObject exp = Instantiate(ExplosionPrefab, transform.position, ExplosionPrefab.transform.rotation);
        Destroy(exp, DestroyExplosion);
        Transform child = transform.GetChild(0);
        transform.DetachChildren();
        Destroy(child.gameObject, DestroyChildren);
        Destroy(gameObject);
    }
}
