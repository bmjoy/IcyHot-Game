using System.Collections;
using System.Linq;
using UnityEngine;



///<remarks>Based On: https://github.com/TrinityCore/TrinityCore</remarks>
public class Spell : MonoBehaviour, IPoolableObject {

    public  Avatar               caster;
    private float                spellTime;
    private ConditionalEffects[] conditionals;
    public  bool                 anchored;
    public  bool                 deflection;

    public delegate void    CastAction();
    public event CastAction OnCast;
    public event CastAction OnPostpone;
    public event CastAction OnRelease;
    public event CastAction OnCancel;
    public event CastAction OnDone;


    public delegate void         HitHealthAction(Health target);
    public event HitHealthAction OnHitHealth;
    public delegate void         HitAnyResponse(GameObject other);
    public event HitAnyResponse  OnHitOut;




    public void Initiate() {
        OnCast += () => {};
        OnPostpone += () => {};
        OnRelease += () => {};
        OnHitOut += (target) => {};
        OnHitHealth += (Health target) => { };
        OnDone += () => { };

        conditionals = GetComponents<ConditionalEffects>();
        foreach (ConditionalEffects condition in conditionals) {
            condition.Register(this);
        }
    }



    public void Start() {
        ObjectPool.pingleton.spawnedObjects.Add(GetComponent<ObjectInfo>().ObjectId, gameObject);
    }



    // --------------------------------------------Events-------------------------------------------- \\
    /// <summary>
    /// 
    /// </summary>
    public void Cast() {
        transform.SetParent(caster.aimPoint);
        OnCast?.Invoke();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Release() {
        OnRelease?.Invoke();
    }


    /// <summary>
    /// 
    /// </summary>
    public void Postpone() {
        OnPostpone?.Invoke();
    }


    /// <summary>
    /// Damage dealing collision response. 
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other) {
        OnHitOut?.Invoke(other.gameObject);
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool Done() {
        if (conditionals.Any(effect => !effect.RgstDone())) { return false; }
        Despawn();
        return true;
    }



    // ---------------------------------------------------- Remote Command Call-Backs ---------------------------------------------------- \\
    /// <summary>
    /// Clean-up. Call to inform that a remote subroutine has finished.
    /// </summary>
    
    /// <summary>
    /// Client call that Initiates the despawning process.
    /// </summary>
    public void Despawn() {
        StartCoroutine(WaitThenDespawn(5f));
    }


    /// <summary>
    /// Initiates the process of despawning the object from the network 
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator WaitThenDespawn(float duration) {
        yield return new WaitForSeconds(duration);
        transform.SetParent(null);
        gameObject.SetActive(false);
        GameManager.UnSpawn(this.gameObject);
    }


}
