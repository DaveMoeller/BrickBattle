using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using static Unity.Collections.AllocatorManager;
[RequireComponent(typeof(ParticleSystem))]

[RequireComponent(typeof(MeshRenderer))]
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
        if (other.gameObject.CompareTag("Ball"))
        {
            //ToDo:Fix ps color
            //Hide the brick
            var mr = GetComponent<Renderer>();
            Material[] materialBricks = mr.materials;
            Debug.Log($"materialBricks[0].name:{materialBricks[0].name}");
            Debug.Log($"materialBricks[0].color:{materialBricks[0].color}");
            mr.enabled = false;
            //Remove collider
            BoxCollider col = GetComponent<BoxCollider>();
            col.enabled = false;
            //Get the particle system of brick (this) and play it
            ParticleSystem ps = GetComponent<ParticleSystem>();
            //
            //MaterialPropertyBlock block = new();
            List<Material> materials = new();
            mr.GetMaterials(materials);
            //
            SetParticleSystemMaterials(ps, materials);
            //SetParticleSystemMaterial(ps, materialBricks[0]);
            ps.Play();
            onDestroyed.Invoke(PointValue);
            var main = ps.main;

            //slight delay to be sure the ball have time to bounce
            Destroy(gameObject, main.duration);
        }
    }
    void SetParticleSystemMaterial(ParticleSystem emitter, Material material)
    {
        if (emitter != null)
        {
            var main = emitter.main;
            main.startColor = material.color;
            Debug.Log($"main.startColor:{main.startColor}");
            // Get the Particle System's Renderer
            ParticleSystemRenderer psr = emitter.GetComponent<ParticleSystemRenderer>();
            List<Material> materials = new List<Material>();
            materials.Add(material);
            psr.SetMaterials(materials);

        }
    }
    void SetParticleSystemMaterials(ParticleSystem emitter, List<Material> materials)
    {
        if (emitter != null)
        {
            var main = emitter.main;
            main.startColor = materials[0].color;
            //main.SetColor("_BaseColor", materials[0].color);
            Debug.Log($"main.startColor:{main.startColor}");
            // Get the Particle System's Renderer
            ParticleSystemRenderer psr = emitter.GetComponent<ParticleSystemRenderer>();
            psr.SetMaterials(materials);

        }
    }
}
