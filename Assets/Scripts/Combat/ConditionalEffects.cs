using System.Collections;
using UnityEngine;



public class ConditionalEffects : MonoBehaviour {
    public Spell spellCast;
    // public Action              spellCast;
    public ConditionalEffects parentAffected;


    public delegate void    CastAction();
    public event CastAction OnCast;
    public event CastAction OnHold;
    public event CastAction OnRelease;
    public event CastAction OnFinish;

    public delegate void     HitRegister(GameObject obj);
    public event HitRegister OnHitRegister;




    public virtual void Register(Spell spellCasted) {
        spellCast = spellCasted;

        OnCast += () => {};
        OnFinish += () => {};

        OnHold += () => {};
        OnRelease += () => {};

        OnHitRegister += (obj) => {};

        if (parentAffected) {
            parentAffected.OnHitRegister += RgstHit;
            parentAffected.OnFinish += RgstCleanUp;
        }
        else {
            spellCasted.OnHitOut += RgstHit;
        }
    }


    public virtual void RgstCast() { OnCast?.Invoke(); }

    public virtual void RgstHit(GameObject obj) { OnHitRegister?.Invoke(obj); }

    public virtual void RgstCleanUp() { OnFinish?.Invoke(); }

    public virtual bool RgstDone() { return true; }


    protected IEnumerator WaitThenDo(float duration, System.Action action) {
        yield return new WaitForSeconds(duration);
        action();
    }

}
