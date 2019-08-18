using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AbilityController : MonoBehaviour
{

    [SerializeField] public Ability[] abilities;
    [SerializeField] Text ultPercentage;
    //[SerializeField] Image ultButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //ultCD
        ultPercentage.text = (abilities[0].CurUltCharge / abilities[0].UltCharge) * 100 + "%";

    }
}
