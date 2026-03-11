using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Paddle : MonoBehaviour
{
    public float Speed = 2.0f;
    public float MaxMovement = 2.0f;

    // Start is called before the first frame update

    void Start()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            //Debug.Log($"meshRenderer: {meshRenderer.name}");

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
        if (Keyboard.current[Key.LeftArrow].isPressed)
        {
            //Debug.Log("Direct Left arrow key is held down");
            Vector3 pos = transform.position;
            pos.x += (-1) * Speed * Time.deltaTime;
            if (pos.x < -MaxMovement)
                pos.x = -MaxMovement;

            transform.position = pos;

        }
        if (Keyboard.current[Key.RightArrow].isPressed)
        {
            //Debug.Log("Direct Right arrow key is held down");
            Vector3 pos = transform.position;
            pos.x += (1) * Speed * Time.deltaTime;
            if (pos.x > MaxMovement)
                pos.x = MaxMovement;

            transform.position = pos;

        }
    }
}
