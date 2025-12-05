using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class MainMenuEvents : MonoBehaviour
{

    private UIDocument document;

    private Button button;

    private List<Button> menuButtons = new List<Button>();  
    
    [SerializeField] private AudioSource AudioSource;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        button = document.rootVisualElement.Q("StartGameButton") as Button;
        button.RegisterCallback<ClickEvent>(onPlayGameClick);

        menuButtons = document.rootVisualElement.Query<Button>().ToList();

        for (int i = 0; i < menuButtons.Count; i++)
        {
            menuButtons[i].RegisterCallback<ClickEvent>(OnAllButtonClick);
        }
    }

    private void OnDisable()
    {
        button.UnregisterCallback<ClickEvent>(onPlayGameClick);


        for (int i = 0; i < menuButtons.Count; i++)
        {
            menuButtons[i].RegisterCallback<ClickEvent>(OnAllButtonClick);
        }
    }

    private void onPlayGameClick(ClickEvent evt)
    {
        Debug.Log("you pressed start the button");
    }

    private void OnAllButtonClick(ClickEvent evt)
    {
        AudioSource.Play();
    }

}
