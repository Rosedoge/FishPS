using UnityEngine;
using System.Collections;
using System;

public class FloatingTextController : MonoBehaviour
{
    private static FloatingText popupText;
    private static GameObject canvas;

    public static void Initialize()
    {
        canvas = GameObject.Find("Canvas");
        if (!popupText)
            popupText = Resources.Load<FloatingText>("Prefabs/PopupTextParent");
    }

    public static void CreateFloatingText(string text, Transform location)
    {
        try
        {
            FloatingText instance = Instantiate(popupText);
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(/*location.position*/
                new Vector3(location.position.x + UnityEngine.Random.Range(-1.2f, 1.2f),
                            location.position.y + UnityEngine.Random.Range(-1.2f, 1.2f),
                            location.position.z));

            instance.transform.SetParent(canvas.transform, false);
            instance.transform.position = screenPosition;
            instance.SetText(text);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}