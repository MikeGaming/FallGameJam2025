using UnityEngine;

public class Goal : MonoBehaviour
{

    bool finished;
    float timer = 0f;

    private void Update()
    {
        if (finished)
        {
            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                LevelManager.Instance.LevelComplete();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            finished = true;
        }
    }
}
