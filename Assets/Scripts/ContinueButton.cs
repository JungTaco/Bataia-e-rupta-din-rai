using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{
    public void LoadNextScene()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void LoadSceneAfterNext()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
	}
}
