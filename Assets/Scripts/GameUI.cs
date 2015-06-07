/// <summary>
/// Отвечает за отображение игрового интерфейса, меню и прочего. Непосредственно управляющих функций не несет, передает все в Game и оттуда же берет данные.
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Game))]

public class GameUI : MonoBehaviour 
{
    private static Game game;

    private List<GameObject> SBElements = new List<GameObject>();

    public GameObject Scoreboard;
    public RectTransform SBRed;
    public RectTransform SBBlue;

    public GameObject EscMenu;

    private bool scoreboardFirstDraw = true;

    public Text RoundTimer;
    public Text HP;
    public GameObject TextFieldPrefab;

    void Awake()
    {
        game = GetComponent<Game>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscMenu.SetActive(!EscMenu.activeSelf);
            if (EscMenu.activeSelf)
            {
                Game.lockControls = true;
            }
            else
            {
                Game.lockControls = false;
            }
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            DrawScoreboard();
        }
        else
        {
            Scoreboard.SetActive(false);
            scoreboardFirstDraw = true;
        }
    }


    void DrawScoreboard()
    {
        Scoreboard.SetActive(true);
        if (scoreboardFirstDraw)
        {
            //Do something
            foreach (GameObject obj in SBElements)
            {
                Destroy(obj);
            }

            SBElements.Clear();

            int redIndex = 0;
            int bluIndex = 0;
            int YSpacing = 25;

            foreach (PhotonPlayer ply in PhotonNetwork.playerList)
            {
                GameObject line = Instantiate(TextFieldPrefab) as GameObject;

                object ded;
                string key = "Deaths";
                ply.customProperties.TryGetValue((object)key, out ded);

                line.GetComponent<Text>().text = ply.name + "     " + ply.GetScore() + "     " + ded.ToString();

                SBElements.Add(line);
                line.transform.position = Vector3.zero;

                if (ply.GetTeam() == PunTeams.Team.red)
                {
                    line.GetComponent<Text>().color = Color.red;
                    line.GetComponent<RectTransform>().SetParent(SBRed, false);
                    line.GetComponent<RectTransform>().anchoredPosition = new Vector3(8f, -6f - YSpacing * redIndex, 0f);
                    redIndex++;
                }
                else if (ply.GetTeam() == PunTeams.Team.blue)
                {
                    line.GetComponent<Text>().color = Color.blue;
                    line.GetComponent<RectTransform>().SetParent(SBBlue, false);
                    line.GetComponent<RectTransform>().anchoredPosition = new Vector3(8f, -6f - YSpacing * bluIndex, 0f);
                    bluIndex++;
                }
                
            }

            scoreboardFirstDraw = false;
        }
    }
}
