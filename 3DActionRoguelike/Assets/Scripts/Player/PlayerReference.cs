using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class PlayerReference : MonoBehaviour
{
    public static GameObject Player { get; private set; }



    private void Awake()
    {
        Player = gameObject;
    }


}
