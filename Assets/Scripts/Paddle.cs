using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Paddle : MonoBehaviour
{
    public float Speed = 2.0f;
    float minX;
    float maxX;
    float minY;
    float maxY;
    // Start is called before the first frame update

    void Start()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        //Instantiate a new ball with paddle as parent
        //Ball.Instance ball = gameObject.transform.Find("Ball").gameObject;
        MeshRenderer meshRendererBall = Ball.Instance.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            //Debug.Log($"meshRenderer: {meshRenderer.name}");

            if (MainManager.Instance != null)
            {
                GameLevelData levelData = GameUIData.Instance.GetGameLevelData(GameUIData.Instance.CurrentLevel);
                meshRenderer.material = levelData.levelMaterial;
                meshRendererBall.material = levelData.levelMaterial;
                Speed = levelData.paddleSpeed;

            }
            else
            {
                Debug.LogError("MainManager.Instance is null");
            }
        }
        minX = MainManager.Instance.GetMinX();
        maxX = MainManager.Instance.GetMaxX();
        minY = MainManager.Instance.GetMinY();
        maxY = MainManager.Instance.GetMaxY();
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
            if (pos.x < minX)
            {
                pos.x = minX;
            }
            transform.position = pos;
        }
        //bool RightDirectionIsPressed = Keyboard.current[Key.RightArrow].isPressed;
        bool RightDirectionIsPressed = MainManager.Instance.PlayerControlsShared.Gameplay.MoveRight.IsPressed();
        if (RightDirectionIsPressed)
        {
            //Debug.Log("Direct Right arrow key is held down");
            Vector3 pos = transform.position;
            pos.x += (1) * Speed * Time.deltaTime;
            if (pos.x > maxX)
            {
                pos.x = maxX;
            }
            transform.position = pos;
        }
        bool UpDirectionIsPressed = MainManager.Instance.PlayerControlsShared.Gameplay.MoveUp.IsPressed();
        if (UpDirectionIsPressed)
        {
            //Debug.Log("Direct Up arrow key is held down");
            Vector3 pos = transform.position;
            pos.y += (1) * Speed * Time.deltaTime;
            if (pos.y > maxY)
            {
                pos.y = maxY;
            }
            transform.position = pos;
        }
        bool DownDirectionIsPressed = MainManager.Instance.PlayerControlsShared.Gameplay.MoveDown.IsPressed();
        if (DownDirectionIsPressed)
        {
            //Debug.Log("Direct Up arrow key is held down");
            Vector3 pos = transform.position;
            pos.y -= (1) * Speed * Time.deltaTime;
            if (pos.y < minY)
            {
                pos.y = minY;
            }
            transform.position = pos;
        }
    }
}
