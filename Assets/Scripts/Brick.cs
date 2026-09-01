using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(ParticleSystem))]

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Brick))]
public class Brick : MonoBehaviour
{
    public UnityEvent<int> onDestroyed;

    public int PointValue;
    public int row;

    void Start()
    {
        var renderer = GetComponentInChildren<Renderer>();

        MaterialPropertyBlock block = new();
        block.SetColor("_BaseColor", GameUIData.Instance.gameLevelData[row - 1].levelMaterial.color);
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
            GameUIData.Instance.RemoveBrick();
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
            // Get the Particle System's Renderer
            ParticleSystemRenderer psr = emitter.GetComponent<ParticleSystemRenderer>();
            psr.SetMaterials(materials);

        }
    }
    public void AddPoints(int points)
    {
        Brick brickScript = GetComponent<Brick>();
        brickScript.PointValue += points;
        SetTextOnBrick($"{brickScript.PointValue:000}");
        //Debug.Log($"brick PointValue: {brickScript.PointValue}");
        //Set the text

    }
    public void SetTextOnBrick(string newText)
    {
        Transform myCanvas = transform.Find("Canvas");
        if (myCanvas != null)
        {
            Transform myText = myCanvas.transform.Find("Text");
            if (myText != null)
            {
                if (myText.TryGetComponent<TMPro.TextMeshProUGUI>(out var tmpText))
                {
                    tmpText.text = newText;
                }
            }
            //ToDo: Set text color to be same as brick
        }

    }
    public int GetPoints()
    {
        Brick brickScript = GetComponent<Brick>();
        //Debug.Log($"brick PointValue: {brickScript.PointValue:000}");
        return brickScript.PointValue;
    }
}
