using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public static Loader instance;
    [SerializeField] GameObject loadingScreen;
    private AsyncOperation asyncOperation;
    private float progress;
    bool loadingScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
    private void OnEnable() {
        loadingScreen.SetActive(false);
    }
    public void LoadScene(string scene)
    {
        if(loadingScene) return;
        StartCoroutine(LoadSceneAsync(scene));
        loadingScene = true;
    }

    IEnumerator LoadSceneAsync(string scene)
    {
        loadingScreen.SetActive(true);
        
        yield return null;

        asyncOperation = SceneManager.LoadSceneAsync(scene);

        while (!asyncOperation.isDone)
        {
            progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;
    }

    public AsyncOperation GetAsyncOperation()
    {
        return asyncOperation;
    }
    public float GetLoadingProgress()
    {
        return progress;
    }


}
