using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float Speed = 2.0f;
    public float MaxMovement = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        //Renderer Renderer = gameObject.GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            Debug.Log($"meshRenderer: {meshRenderer.name}");

            //Debug.Log($"MainManager.MaterialGreen: {MainManager.Instance.MaterialGreen}");

            //Material material = meshRenderer.material;
            //meshRenderer.material
            if (MainManager.Instance != null)
            {
                switch (MenuManager.Instance.CurrentLevel)
                {
                    case
                        "Green":
                        {
                            meshRenderer.material = MainManager.Instance.MaterialGreen;
                            break;
                        }
                    case
                        "Purple":
                        {
                            meshRenderer.material = MainManager.Instance.MaterialPurple;
                            break;
                        }
                    case
                         "Chocolate":
                        {
                            meshRenderer.material = MainManager.Instance.MaterialChocolate;
                            break;
                        }
                    case
                         "Blue":
                        {
                            meshRenderer.material = MainManager.Instance.MaterialBlue;
                            break;
                        }
                    case
                         "Silver":
                        {
                            meshRenderer.material = MainManager.Instance.MaterialSilver;
                            break;
                        }
                    case
                         "Gold":
                        {
                            meshRenderer.material = MainManager.Instance.MaterialGold;
                            break;
                        }
                    default:
                        meshRenderer.material = MainManager.Instance.MaterialGreen;
                        break;
                }
            }
            else
            {
                Debug.LogError("MainManager.Instance is null");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float input = Input.GetAxis("Horizontal");

        Vector3 pos = transform.position;
        pos.x += input * Speed * Time.deltaTime;

        if (pos.x > MaxMovement)
            pos.x = MaxMovement;
        else if (pos.x < -MaxMovement)
            pos.x = -MaxMovement;

        transform.position = pos;
    }
}
