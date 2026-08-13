using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using static Unity.Collections.AllocatorManager;
[RequireComponent(typeof(ParticleSystem))]

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(BoxCollider))]
public class Brick : MonoBehaviour
{
    public UnityEvent<int> onDestroyed;

    public int PointValue;
    public int row;

    void Start()
    {
        var renderer = GetComponentInChildren<Renderer>();

        MaterialPropertyBlock block = new();
        switch (row)
        {
            case 1:
                //008000
                //block.SetColor("_BaseColor", Color.green);
                //Debug.Log($"row {row} color: {GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color}");
                block.SetColor("_BaseColor", GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color);
                break;
            case 2:
                //800080
                //block.SetColor("_BaseColor", Color.purple);
                //Debug.Log($"row {row} color: {GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color}");
                block.SetColor("_BaseColor", GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color);
                break;
            case 3:
                //D2691E
                //block.SetColor("_BaseColor", Color.chocolate);
                //Debug.Log($"row {row} color: {GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color}");
                block.SetColor("_BaseColor", GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color);
                break;
            case 4:
                //0000FF
                //block.SetColor("_BaseColor", Color.blue);
                //Debug.Log($"row {row} color: {GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color}");
                block.SetColor("_BaseColor", GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color);
                break;
            case 5:
                //C0C0C0
                //block.SetColor("_BaseColor", Color.silver);
                //Debug.Log($"row {row} color: {GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color}");
                block.SetColor("_BaseColor", GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color);
                break;
            case 6:
                //FFD700
                //block.SetColor("_BaseColor", Color.gold);
                //Debug.Log($"row {row} color: {GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color}");
                block.SetColor("_BaseColor", GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color);
                break;
            default:
                //block.SetColor("_BaseColor", Color.red);
                //Debug.Log($"row {row} color: {GameUIData.Instance.gameLevelData[0].levelMaterial.color}");
                block.SetColor("_BaseColor", GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color);
                break;
        }
        renderer.SetPropertyBlock(block);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            //Hide the brick
            var mr = GetComponent<Renderer>();
            mr.enabled = false;
            //Remove collider
            BoxCollider col = GetComponent<BoxCollider>();
            col.enabled = false;
            //Get the particle system of brick (this) and play it
            ParticleSystem ps = GetComponent<ParticleSystem>();
            List<Material> materialsBrick = new();
            mr.GetMaterials(materialsBrick);
            SetParticleSystemMaterials(ps, materialsBrick);
            ps.Play();
            onDestroyed.Invoke(PointValue);
            var main = ps.main;

            //slight delay to be sure the ball have time to bounce
            Destroy(gameObject, main.duration);
        }
    }
    void SetParticleSystemMaterials(ParticleSystem emitter, List<Material> materials)
    {
        if (emitter != null)
        {
            var main = emitter.main;
            main.startColor = materials[0].color;
            //main.SetColor("_BaseColor", materialsBrick[0].color);
            //Debug.Log($"main.startColor:{main.startColor}");
            // Get the Particle System's Renderer
            ParticleSystemRenderer psr = emitter.GetComponent<ParticleSystemRenderer>();
            psr.SetMaterials(materials);

        }
    }
}
