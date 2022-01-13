using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DecisionManager : MonoBehaviour
{
    private GameObject decision, consequences;
    [HideInInspector]
    public float workerSatisfaction;
    [HideInInspector]
    public float baseSatisfaction;
    private AntSpawner spawn;
    private TMP_Text satisfaction;
    private GameObject flags, loseScreen;
    private bool hasShownLoseScreen;

    private bool isAnswering;
    private int currentQuestion;
    private string[,] questions = {
        { "Invest in public transport?",
            "20", "-20", "0.3",
            "-30", "0", "-0.3" },
        { "Commit tax fraud?",
            "0", "50", "-0.4",
            "0", "0", "0" },
        { "Implement universal basic ant income?",
            "0", "-30", "0.3",
            "0", "30", "-0.3" },
        { "Termites have set up nearby! Send soldiers off to invade?",
            "-50", "0", "-0.2",
            "0", "-30", "0" },
        { "Break up private leaf monopolies?",
            "0", "-50", "0.4",
            "0", "50", "-0.4" },
        { "Ban opposing literature?",
            "0", "0", "-0.3",
            "0", "0", "0.3" }
    };

    private void Start()
    {
        decision = GameObject.FindGameObjectWithTag("Decision");
        decision.SetActive(false);
        consequences = GameObject.FindGameObjectWithTag("Consequences");
        consequences.SetActive(false);
        baseSatisfaction = 1;
        spawn = FindObjectOfType<AntSpawner>();
        satisfaction = GameObject.FindGameObjectWithTag("Satisfaction").GetComponent<TMP_Text>();
        flags = GameObject.FindGameObjectWithTag("Flags");
        flags.SetActive(false);
        loseScreen = GameObject.FindGameObjectWithTag("Lose");
        loseScreen.SetActive(false);
    }

    private void Update()
    {
        workerSatisfaction = roundFloat(
            baseSatisfaction + // default 1, but may rise or lower depending on events
            (spawn.antCount / 1000f) + // each ant increases morale by a factor of 0.001%
            ((spawn.leafCount - spawn.antCount) / 500f) // each ant needs 1 leaf, which affects morale by a factor of 0.002%
        , 2);

        if (!spawn.isGamePaused)
        {
            satisfaction.text = "Worker satisfaction: " + (workerSatisfaction * 100) + "%";

            if ((workerSatisfaction * 100) <= 50)
            {
                spawn.isGamePaused = true;
                decision.SetActive(false);
            }

            if (spawn.doesHaveQueen && currentQuestion < questions.GetLength(0) && !isAnswering && Random.Range(0, 1500) <= 1)
            {
                askQuestion();
            }
        }
        else
        {
            flags.SetActive(true);

            if (flags.transform.localPosition.y < 0)
            {
                flags.transform.position += Vector3.up * 0.008f;
            }
            else
            {
                if (!hasShownLoseScreen)
                {
                    Invoke("showLoseScreen", 3);
                }
            }
        }
    }

    private void askQuestion()
    {
        decision.SetActive(true);
        isAnswering = true;

        decision.GetComponentInChildren<TMP_Text>().text = questions[currentQuestion, 0];
    }

    public void answer(bool isYes)
    {
        if (!spawn.isGamePaused)
        {
            int[] answerValues = {
                int.Parse(questions[currentQuestion, 1]),
                int.Parse(questions[currentQuestion, 2]),
                int.Parse(questions[currentQuestion, 4]),
                int.Parse(questions[currentQuestion, 5])
            };

            if (
                isYes &&
                (spawn.antCount + answerValues[0] >= 0) &&
                (spawn.leafCount + answerValues[1] >= 0))
            {
                spawn.antCount += answerValues[0];
                spawn.leafCount += answerValues[1];
                baseSatisfaction += float.Parse(questions[currentQuestion, 3]);

                decision.SetActive(false);

                consequences.SetActive(true);
                consequences.GetComponentInChildren<TMP_Text>().text =
                    "Ants: " + answerValues[0] +
                    "\nLeaves: " + answerValues[1] +
                    "\nSatisfaction: " + (float.Parse(questions[currentQuestion, 3]) * 100) + "%";

                Invoke("hideConsequences", 3);

                currentQuestion++;
            }
            else if (
                !isYes &&
                (spawn.antCount + answerValues[2] >= 0) &&
                (spawn.leafCount + answerValues[3] >= 0))
            {
                spawn.antCount += answerValues[2];
                spawn.leafCount += answerValues[3];
                baseSatisfaction += float.Parse(questions[currentQuestion, 6]);

                decision.SetActive(false);

                consequences.SetActive(true);
                consequences.GetComponentInChildren<TMP_Text>().text =
                    "Ants: " + answerValues[2] +
                    "\nLeaves: " + answerValues[3] +
                    "\nSatisfaction: " + (float.Parse(questions[currentQuestion, 6]) * 100) + "%";

                Invoke("hideConsequences", 3);

                currentQuestion++;
            }
        }
    }

    private float roundFloat(float f, int d)
    {
        return (float)decimal.Round((decimal)f, d);
    }

    public void restartGame()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }

    private void hideConsequences()
    {
        consequences.SetActive(false);
        isAnswering = false;
    }

    private void showLoseScreen()
    {
        loseScreen.SetActive(true);
        hasShownLoseScreen = true;
    }
}
