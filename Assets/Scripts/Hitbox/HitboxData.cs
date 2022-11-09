using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    None,
    Poke,
    Pierce,
    Slash,
    Impact
}

public enum DamageResponse
{
    None,
    Flinch,
    Trip,
    Launch
}

public class HitboxData
{
    public int Damage { get; set; }
    public DamageType Type { get; set; }
    public DamageResponse Response { get; set; }

    public HitboxData(int damage = 10, 
            DamageType type = DamageType.None, 
            DamageResponse response = DamageResponse.None)
    {
        this.Damage = damage;
        this.Type = type;
        this.Response = response;
    }
}