using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iAbility
{



    bool isUltimate { get; set; }
    bool ultimateReady { get; set; }
    bool toggled { get; set; }

    /// <summary>
    /// Rate by which the ability, if ultimate, gets charged
    /// </summary>
    float additionRate { get; set; }
    float ultCharge { get; }

    float cooldown { get; }

    
    void Activate();
    void Deactivate();
}
