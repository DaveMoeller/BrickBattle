using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    public float Speed = 2.0f;
    public float MaxMovement = 2.0f;

    // Start is called before the first frame update

    void Start()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshRenderer meshRendererBall = gameObject.transform.Find("Ball").gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            //Debug.Log($"meshRenderer: {meshRenderer.name}");

            if (MainManager.Instance != null)
            {
                GameLevelData levelData = GameUIData.Instance.GetGameLevelData(GameUIData.Instance.CurrentLevel);
                meshRenderer.material = levelData.levelMaterial;
                meshRendererBall.material = levelData.levelMaterial;

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
        //bool LeftDirectionIsPressed = Keyboard.current[Key.LeftArrow].isPressed;
        bool LeftDirectionIsPressed = MainManager.Instance.PlayerControlsShared.Gameplay.MoveLeft.IsPressed();
        if (LeftDirectionIsPressed)
        {
            //Debug.Log("Direct Left arrow key is held down");
            Vector3 pos = transform.position;
            pos.x += (-1) * Speed * Time.deltaTime;
            if (pos.x < -MaxMovement)
                pos.x = -MaxMovement;

            transform.position = pos;

        }
        //bool RightDirectionIsPressed = Keyboard.current[Key.RightArrow].isPressed;
        bool RightDirectionIsPressed = MainManager.Instance.PlayerControlsShared.Gameplay.MoveRight.IsPressed();
        if (RightDirectionIsPressed)
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
