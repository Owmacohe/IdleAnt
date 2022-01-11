using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DecisionManager : MonoBehaviour
{
    private GameObject decision;
    [HideInInspector]
    public float workerSatisfaction;
    [HideInInspector]
    public float baseSatisfaction;
    private AntSpawner spawn;
    private TMP_Text satisfaction;

    private bool isAnswering;
    private int currentQuestion;
    private string[,] questions = {
        { "Commit tax fraud?",
            "0", "50", "-0.4",
            "0", "0", "0" },
        { "Implement universal basic ant income?",
            "0", "-30", "0.3",
            "0", "30", "-0.3" },
        { "Termites have set up nearby! Send soldiers off to invade?",
            "-50", "0", "-0.2",
            "0", "-30", "0" }
    };

    private void Start()
    {
        decision = GameObject.FindGameObjectWithTag("Decision");
        decision.SetActive(false);
        baseSatisfaction = 1;
        spawn = FindObjectOfType<AntSpawner>();
        satisfaction = GameObject.FindGameObjectWithTag("Satisfaction").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        workerSatisfaction = roundFloat(
            baseSatisfaction + // default 1, but may rise or lower depending on events
            (spawn.antCount / 1000f) + // each ant increases morale by a factor of 0.001%
            ((spawn.leafCount - spawn.antCount) / 500f) // each ant needs 1 leaf, which affects morale by a factor of 0.002%
        , 2);

        satisfaction.text = "Worker satisfaction: " + (workerSatisfaction * 100) + "%";

        if (spawn.doesHaveQueen && !isAnswering && Random.Range(0, 5000) <= 1)
        {
            askQuestion();
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
            isAnswering = false;
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
            isAnswering = false;
            currentQuestion++;
        }
    }

    private float roundFloat(float f, int d)
    {
        return (float)decimal.Round((decimal)f, d);
    }
}
