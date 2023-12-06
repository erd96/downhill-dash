using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float terrainCount = 0f;

    public float playerSpeed = 0.5f;
    public float maxSlope = 20f;

    public float playerTrackLeftZ;
    public float playerTrackMiddleZ;
    public float playerTrackRightZ;



    private void Awake()
    {
        Instance = this; 
    }


}

