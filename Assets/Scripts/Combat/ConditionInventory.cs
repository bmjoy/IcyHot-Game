using System.Collections;
using UnityEngine;



/**/
public abstract class ConditionInventory : ScriptableObject {
    public abstract Condition GetCondition(Health tHealth, CharacterSkeleton origin);
}


public class ConditionInventory<T, Typename> : ConditionInventory 
    where T : ConditionTemplate 
    where Typename : Condition<T>, new() {
    public T _BuffData;
    public override Condition GetCondition(Health tHealth, CharacterSkeleton origin) {
        return new Typename { _BuffData = this._BuffData, _OtherHealth = tHealth };
    }
}
