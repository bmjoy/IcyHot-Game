using System.Collections;
using UnityEngine;



///<remarks>Based On: https://github.com/TrinityCore/TrinityCore</remarks>
public abstract class Condition {
    public abstract ConditionTemplate GetCondition();

    public delegate void            DropConditionEvent();
    public event DropConditionEvent OnDropBuffCondition;
    public event DropConditionEvent OnDropDebuffCondition;

    public delegate void           AddConditionEvent();
    public event AddConditionEvent OnAddBuffCondition;
    public event AddConditionEvent OnAddDebuffCondition;


    public virtual void AddBuffCondition()   { OnAddBuffCondition += () => {}; }
    public virtual void AddDebuffCondition() { OnAddDebuffCondition += () => {}; }

    public virtual void DropBuffCondition()   { OnDropBuffCondition?.Invoke(); }
    public virtual void DropDebuffCondition() { OnDropDebuffCondition?.Invoke(); }

    public abstract void DoT();



    public IEnumerator DebuffEffectOvertime(float duration) {
        float dotTick = duration / 32f; // 0.32 seconds World of Warcraft design (adding diminishing)     
        float now     = Time.fixedTime;
        float end     = now + duration;

        float last = 0f;
        while (now < end) {
            now = Time.fixedTime;
            if (now > last + dotTick) {

                DoT();

                last = now;
            }
            yield return null;
        }

        DropDebuffCondition();
    }
}


public abstract class Condition<T> : Condition where T : ConditionTemplate {
    public T      _BuffData;
    public Health _OtherHealth;
    public Avatar _OriginalEntity;

    public override ConditionTemplate GetCondition() {
        return _BuffData;
    }
}
