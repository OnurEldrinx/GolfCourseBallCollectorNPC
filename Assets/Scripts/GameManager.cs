using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private CinemachineVirtualCamera startCamera;
    
    private void OnEnable()
    {
        startButton.onClick.AddListener(() =>
        {
            startCamera.Priority = 1;
            startScreen.SetActive(false);
            gameScreen.SetActive(true);
            BallCollector.OnGameStarted.Invoke(true);
        });
        
        homeButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
        
        exitButton.onClick.AddListener(Application.Quit);
        
    }
}
