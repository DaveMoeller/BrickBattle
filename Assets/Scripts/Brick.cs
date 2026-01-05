using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{
    public UnityEvent<int> onDestroyed;
    
    public int PointValue;
    public int row;

    void Start()
    {
        var renderer = GetComponentInChildren<Renderer>();

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        switch (row)
        {
            case 1 :
                //008000
                block.SetColor("_BaseColor", Color.green);
                break;
            case 2:
                //800080
                block.SetColor("_BaseColor", Color.purple);
                break;
            case 3:
                //D2691E
                block.SetColor("_BaseColor", Color.chocolate);
                break;
            case 4:
                //0000FF
                block.SetColor("_BaseColor", Color.blue);
                break;
            case 5:
                //C0C0C0
                block.SetColor("_BaseColor", Color.silver);
                break;
            case 6:
                //FFD700
                block.SetColor("_BaseColor", Color.gold);
                break;
            default:
                block.SetColor("_BaseColor", Color.red);
                break;
        }
        renderer.SetPropertyBlock(block);
    }

    private void OnCollisionEnter(Collision other)
    {
        onDestroyed.Invoke(PointValue);
        
        //slight delay to be sure the ball have time to bounce
        Destroy(gameObject, 0.2f);
    }
}
