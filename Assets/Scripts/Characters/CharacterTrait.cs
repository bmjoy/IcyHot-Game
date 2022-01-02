using System;



/* Tutorial: https://kybernetik.com.au/animancer/docs/introduction/features */
[Serializable]
public class CharacterTrait {

    public float standardVal;
    public float bonus = 0;
    public float activeMultiplier = 0;
    public float diminishedReturns = 0;
    public float CurrentValue { get { return (standardVal + bonus) * activeMultiplier; } }

}
