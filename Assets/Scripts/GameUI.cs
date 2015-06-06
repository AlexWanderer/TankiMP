/// <summary>
/// Отвечает за отображение игрового интерфейса, меню и прочего. Непосредственно управляющих функций не несет, передает все в Game и оттуда же берет данные.
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Game))]

public class GameUI : MonoBehaviour 
{
    private static Game game;

    public Text RoundTimer;
    public Text HP;
    void Awake()
    {
        game = GetComponent<Game>();

    }
	
}
