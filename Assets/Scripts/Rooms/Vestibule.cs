using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vestibule : Room
{
    private void Start()
    {
        GetComponentInChildren<Door>().IsLocked = true;
    }
}
