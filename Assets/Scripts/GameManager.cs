using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool gameOver = false;
    private GameStates.State GameState;


    [SerializeField] //Adding the pause menu instance
    GameObject PauseMenu;
    
    public void Start()
    {
        GameState = new GameStates.PlayState(PauseMenu);
        GameState.OnEnter();
    }

    public void Update()
    {
        HandleNewState(GameState.OnUpdate(), GameState);
    }

    public void EndGame()
    {
        if(gameOver == false)
        {
            Debug.Log("end");
            gameOver = true;
            Restart();
        }
        
    }
  
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void HandleNewState(GameStates.State newState, GameStates.State oldState)
    {
        if (newState != oldState)
        {
            GameState = newState;
            GameState.OnEnter();
        }
    }

}
