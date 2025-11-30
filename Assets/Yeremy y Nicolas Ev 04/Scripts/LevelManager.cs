using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void GoToLevel(int index)
    {
        SceneManager.LoadScene(index);
    }
}
