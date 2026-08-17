using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void OnCollisionEnter(Collision other)
    {
        MainManager.Instance.GameOver();
    }
}
