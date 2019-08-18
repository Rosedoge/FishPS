using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract bool IsUltimate { get; set; }
    public abstract bool UltimateReady { get; set; }
    public abstract bool Toggled { get; set; }
    public abstract float AdditionRate { get; set; }
    public abstract float UltCharge { get; set; }
    public abstract float CurUltCharge { get; set; }
    public abstract float Cooldown { get; set; }

    public abstract void Activate();


    public abstract void Deactivate();

    public abstract void AddCharge();
}
