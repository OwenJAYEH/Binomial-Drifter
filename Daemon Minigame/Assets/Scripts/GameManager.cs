using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject Checkpoint1, Checkpoint2, Checkpoint3, Checkpoint4, Checkpoint5;

    public GameObject playerCar, ghostCar;

    private Rigidbody ghostRB, playerRB;

    public TextMeshProUGUI currentTimeText, bestTimeText, speedText;

    private int checkpointCounter = 0;

    public bool ghostPlaying = false;

    public bool GhostTime = false;

    public bool firstLap = true;

    public bool completeLap = false;

    private float lapTime = 0f;
    private float bestTime = 1000f;

    private bool bestRecord = true;

    public bool gamePaused = false;

    public List<PointInTime> pointsInTime;
    public List<PointInTime> bestsInTime;
    public List<PointInTime> bestsInTimeCopy;

    public GameObject gameUI;
    public GameObject pauseUI;

    private void Start()
    {
        pauseUI.SetActive(false);

        pointsInTime = new List<PointInTime>();
        bestsInTime = new List<PointInTime>();
        bestsInTimeCopy = new List<PointInTime>();

        ghostRB = ghostCar.GetComponent<Rigidbody>();

        playerRB = playerCar.GetComponent<Rigidbody>();

        Checkpoint1.SetActive(true);
        Checkpoint2.SetActive(false);
        Checkpoint3.SetActive(false);
        Checkpoint4.SetActive(false);
        Checkpoint5.SetActive(false);
    }
    public void CheckpointIncrease()
    {
        checkpointCounter = checkpointCounter + 1;
    }

    private void Update()
    {
        speedText.text = (playerRB.velocity.magnitude * 5).ToString("F2") + (" km/h");

        lapTime += Time.deltaTime;
        currentTimeText.text = lapTime.ToString("F2");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePaused)
            {
                pauseUI.SetActive(true);
                gameUI.SetActive(false);
                Time.timeScale = 0f;
                gamePaused = true;

            }
            else if (gamePaused)
            {
                pauseUI.SetActive(false);
                gameUI.SetActive(true);
                Time.timeScale = 1f;
                gamePaused = false;
            }
            
        }

        switch (checkpointCounter)
        {
            case 0:
                Checkpoint1.SetActive(true);

                if (!firstLap)
                {
                    StartGhost(); 
                }
                break;
            case 1:
                Checkpoint2.SetActive(true);
                Checkpoint1.SetActive(false);
                break;
            case 2:
                Checkpoint3.SetActive(true);
                Checkpoint2.SetActive(false);
                break;
            case 3:
                Checkpoint4.SetActive(true);
                Checkpoint3.SetActive(false);
                break;
            case 4:
                Checkpoint5.SetActive(true);
                Checkpoint4.SetActive(false);
                break;
            case 5:

                Checkpoint5.SetActive(false);
                Debug.Log(lapTime);

                if (firstLap)
                {
                    bestsInTime = pointsInTime;
                    bestsInTimeCopy = bestsInTime;

                    bestTime = lapTime;
                    bestTimeText.text = ("Best Lap: ") + (bestTime.ToString("F2"));
                    bestsInTime = new List<PointInTime>();
                }

                if (!firstLap)
                {
                    if (lapTime < bestTime)
                    {
                        bestsInTimeCopy = pointsInTime;
                        //bestsInTimeCopy = bestsInTime;

                        bestTime = lapTime;
                        bestTimeText.text = ("Best Lap: ") + (bestTime.ToString("F2"));
                    }
                    else
                    {
                        bestsInTimeCopy = bestsInTime;
                    }
                }
                pointsInTime = new List<PointInTime>();
                bestsInTime = new List<PointInTime>();

                firstLap = false;
                lapTime = 0f;
                checkpointCounter = 0;
                break;
            }
    }

    private void FixedUpdate()
    {
        Record();

        if (bestRecord)
        {
            BestRecord();
        }
        if (ghostPlaying)
        {
            Ghost();
        }
    }

    void Ghost()
    {
       if (bestsInTimeCopy.Count > 0)
       {
            //Debug.Log("Ghosting");
            PointInTime bestInTime = bestsInTimeCopy[0];
            ghostCar.transform.position = bestInTime.position;
            ghostCar.transform.rotation = bestInTime.rotation;
            bestsInTimeCopy.RemoveAt(0);
       }
        else
       {
           StopGhost();
       }
    }

    void Record()
    {
        pointsInTime.Add(new PointInTime(playerCar.transform.position, playerCar.transform.rotation));
    }

    void BestRecord()
    {
        bestsInTime.Add(new PointInTime(ghostCar.transform.position, ghostCar.transform.rotation));
    }


    public void StartGhost()
    {
        ghostPlaying = true;
    }

    public void StopGhost()
    {
        ghostPlaying = false;
    }

    void ClearList()
    {
        pointsInTime.Clear();
    }

    public void RestartGame()
    {
        Application.Quit();
    }
}
