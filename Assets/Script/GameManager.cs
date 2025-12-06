using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public enum GameState
{
    MainMenu,
    Loading,
    Play,
    Paused,
    Cutscene,       //이건 사용할 일이 없을 것 같긴 해
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]
    private Hero hero;
    public Hero Hero => hero;
    
    [SerializeField]
    private ItemCollector itemCollector;
    public IReadOnlyList<Item> ItemCollector => itemCollector.Items;

    InputSystem_Actions input;

    private GameState currentState = GameState.MainMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        input = new InputSystem_Actions();
    }


    private void OnEnable()
    {
        input.Player.Enable();
        input.Player.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        input.Player.Pause.performed -= OnPause;
        input.Player.Disable();
    }

    private void Update()
    {

    }


    private void OnPause(InputAction.CallbackContext ctx)
    {
        OnOptionOpen();
    }

    public void OnOptionOpen()
    {
        if (currentState == GameState.MainMenu)
        {
            UIManager.Instance.Mainmenu.SetActive(false);
        }
        else
        {
            currentState = GameState.Paused;
        }
        
        Time.timeScale = 0;
        UIManager.Instance.Option = true;
    }

    public void OnOptionClose()
    {
        if (currentState == GameState.Paused)
        {
            currentState = GameState.Play;
        }
        else if(currentState == GameState.MainMenu)
        {
            UIManager.Instance.Mainmenu.SetActive(true);
        }

        Time.timeScale = 1;
        UIManager.Instance.Option = false;
    }


    public void StartButton()
    {
        UIManager.Instance.One.gameObject.SetActive(true);
        UIManager.Instance.Two.gameObject.SetActive(true);
        UIManager.Instance.Mainmenu.SetActive(false);
    }
    public void OptionButton()
    {
        OnOptionOpen();
    }
    public void ExitButton()
    {
        Application.Quit();
    }


    //그냥 코루틴 대신 실행하고 멈춰주는 메소드 LineUP 클래스가 유일하게 사용중
    public Coroutine RunCoroutine(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }

    public void StopCoroutineExtern(Coroutine enumerator)
    {
        StopCoroutine(enumerator);
    }
}
