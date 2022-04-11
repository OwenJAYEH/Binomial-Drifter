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

    public TextMeshProUGUI currentTimeText, bestTimeText;

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
        // Sets up Pause UI;
        pauseUI.SetActive(false);

        // These are lists that keep the positions of the car. pointsInTime is actively keeping the current lap, while bestsInTime/bestsInTimeCopy keep the best lap copy.
        pointsInTime = new List<PointInTime>();
        bestsInTime = new List<PointInTime>();
        bestsInTimeCopy = new List<PointInTime>();

        // Setting up rigidbody's for car to control rotation/movement
        ghostRB = ghostCar.GetComponent<Rigidbody>();
        playerRB = playerCar.GetComponent<Rigidbody>();

        //Checkpoints are controlled by GameObjects with Triggers. This prevents the player from cheating the system and going backwards, as all 5 must be activated in order to complete a lap
        Checkpoint1.SetActive(true);
        Checkpoint2.SetActive(false);
        Checkpoint3.SetActive(false);
        Checkpoint4.SetActive(false);
        Checkpoint5.SetActive(false);
    }

    public void CheckpointIncrease()
    {
        // Increases checkpoint counter
        checkpointCounter = checkpointCounter + 1;
    }

    private void Update()
    {
        // Reads out the current speed to the player in UI. Multiplied by 5 to make the numbers more fast and exciting.
        //speedText.text = (playerRB.velocity.magnitude * 5).ToString("F2") + (" km/h");

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("MainGame");
        }

        // Keeps control of lap time.
        lapTime += Time.deltaTime;
        currentTimeText.text = lapTime.ToString("F2");

        // Used to pause the game.
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

        // Switch case to activate and deactivate checkpoints as necessary to complete the lap
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

                // The first lap is always the fastest, so it does not need to check if it is a better lap or not
                if (firstLap)
                {
                    // Copies the current lap as the best lap, and puts it in another copy so it can be reused.
                    bestsInTime = pointsInTime;
                    bestsInTimeCopy = bestsInTime;

                    // Makes this lap the best time and displays it in UI
                    bestTime = lapTime;
                    bestTimeText.text = ("Best Lap: ") + (bestTime.ToString("F2"));
                    bestsInTime = new List<PointInTime>();
                }

                if (!firstLap)
                {
                    // If this is a better lap than your last lap
                    if (lapTime < bestTime)
                    {
                        // Make this the best lap that is read from
                        bestsInTimeCopy = pointsInTime;

                        // Update UI
                        bestTime = lapTime;
                        bestTimeText.text = ("Best Lap: ") + (bestTime.ToString("F2"));
                    }
                    else
                    {
                        // Otherwise reuse the best lap again
                        bestsInTimeCopy = bestsInTime;
                    }
                }
                // Reset these two lists so they can be reused
                pointsInTime = new List<PointInTime>();
                bestsInTime = new List<PointInTime>();

                // Reset the lap
                firstLap = false;
                lapTime = 0f;
                checkpointCounter = 0;
                break;
            }
    }

    private void FixedUpdate()
    {
        // FixedUpdate used to record physics updates of cars position;

        Record(); // Records player lap

        if (bestRecord) // Records ghost lap
        {
            BestRecord();
        }
        if (ghostPlaying) // Displays Ghost
        {
            Ghost();
        }
    }

    void Ghost()
    {
       if (bestsInTimeCopy.Count > 0)
       {
            // When the Ghost is being displayed, this function goes through the entire list one by one by recording the position and rotation of the recording,
            // applying it to the ghost, then clearing that number of the list, all the way until the list is completely cleared, making the illusion of a ghost
            // recording.
            PointInTime bestInTime = bestsInTimeCopy[0];
            ghostCar.transform.position = bestInTime.position;
            ghostCar.transform.rotation = bestInTime.rotation;
            bestsInTimeCopy.RemoveAt(0);
       }
        else
       {
           // Once the list is clear, stop the ghost.
           StopGhost();
       }
    }

    void Record()
    {
        // Every lap, the player's movement is being recorded. If they get a better lap, this will be used to update the ghost's position in the next lap
        pointsInTime.Add(new PointInTime(playerCar.transform.position, playerCar.transform.rotation));
    }

    void BestRecord()
    {
        // Also every lap, the ghost's movement is being recorded. If the lap is not better, this will be used to feed the smae exact movement back into the ghost's position,
        // making the illusion that it is repeating its previous movements.
        bestsInTime.Add(new PointInTime(ghostCar.transform.position, ghostCar.transform.rotation));
    }


    // Simple functions to take care of booleans, clearing lists, and quitting the game
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
