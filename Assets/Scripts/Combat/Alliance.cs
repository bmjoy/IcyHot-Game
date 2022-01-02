using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 
/// </summary>
public enum Allegiance {
    Golem,
    Venom,
    Dragon,
    Wolf,
}

/// <summary>
/// 
/// </summary>
public class Alliance : MonoBehaviour {
    public Allegiance     type;
    public List<Alliance> team    = new List<Alliance>();
    public List<Health>    members = new List<Health>();



    public Health GetTeamMember(Vector3 position, float sightRange) {
        Health closest  = null;
        float bestDist = float.MaxValue;
        foreach (Health member in members) {
            float dist = (position - member.transform.position).sqrMagnitude;
            if (dist < sightRange * sightRange && dist < bestDist) {
                closest = member;
                bestDist = dist;
            }
        }

        return closest;
    }

    public Health FindClosestAlly(Vector3 position, float sightRange) {
        Health closest  = null;
        float bestDist = float.MaxValue;

        foreach (Alliance ally in team) {
            Health closestMember = ally.GetTeamMember(position, sightRange);
            if (closestMember == null) { continue; }
            float distance = (position - closestMember.transform.position).sqrMagnitude;
            if (distance < bestDist) {
                closest = closestMember;
                bestDist = distance;
            }
        }

        return closest;
    }
}
