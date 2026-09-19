using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelChange : MonoBehaviour
{
    [SerializeField] private int levelToChange;
    private Collider _collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(levelToChange);
        }
    }
}
