﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace PG
{
    /// <summary>
    /// The game controller is responsible for initializing the player's car.
    /// </summary>
    public class GameController :Singleton<GameController>
    {
        public TextMeshProUGUI TimeScaleText;
        public Transform[] StartPositions;
        public List<CarController> AllCars = new List<CarController>();

        /// <summary>
        /// For split screen please use version with dependencies https://u3d.as/1ZdE (InputSystem + FMOD required for split screen)
        /// FMOD Required to play sound for two players, this cannot be done with the built-in audio system.
        /// </summary>
        public static bool SplitScreen => false;

        public InitializePlayer Player1 { get; private set; }
        public InitializePlayer Player2 { get; private set; }
        public CarController PlayerCar1 { get; private set; }
        public CarController PlayerCar2 { get; private set; }

        List<VehicleController> AllVehicles = new List<VehicleController>();
        List<VehicleController> VehiclePrefabs = new List<VehicleController>();

        public static bool IsMobilePlatform
        {
            get
            {
#if UNITY_EDITOR

                return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android ||
                    UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS;
#else
                return Application.isMobilePlatform;
#endif
            }
        }

        void Start ()
        {
            if (TimeScaleText)
            {
                TimeScaleText.SetActive (false);
            }

            if (IsMobilePlatform && MobileUI.Instance)
            {
                if (MobileUI.Instance.SelectNextCarButton)
                    MobileUI.Instance.SelectNextCarButton.onClick.AddListener (SetNextCar);
                if (MobileUI.Instance.RepairButton)
                    MobileUI.Instance.RepairButton.onClick.AddListener (TryResetCar);
                if (MobileUI.Instance.RestartSceneButton)
                    MobileUI.Instance.RestartSceneButton.onClick.AddListener (RestartScene);
            }

            if (StartPositions == null || StartPositions.Length <= 0)
            {
                var respawns = GameObject.FindGameObjectsWithTag ("Respawn");
                StartPositions = new Transform[respawns.Length];
                for (int i = 0; i < respawns.Length; i++)
                {
                    StartPositions[i] = respawns[i].transform;
                }
            }
            AllCars.RemoveAll (c => c == null);
            AllVehicles = FindObjectsOfType<VehicleController> ().ToList();
            var allCars = FindObjectsOfType<CarController> ().ToList ();
            AllCars.AddRange (allCars.Where(c => !AllCars.Contains(c)));


            if (!PlayerCar1 && AllCars.Count == 0)
            {
                PlayerCar1 = Instantiate (B.GameSettings.AvailableVehicles.First(v => v as CarController) as CarController);
                if (StartPositions != null && StartPositions.Length > 0)
                {
                    PlayerCar1.transform.position = StartPositions[0].position;
                    PlayerCar1.transform.rotation = StartPositions[0].rotation;
                }
                AllVehicles.Add (PlayerCar1);
                AllCars.Add (PlayerCar1);
            }
            else if (!PlayerCar1)
            {
                PlayerCar1 = AllCars[0];
            }

            if (SplitScreen)
            {
                if (!PlayerCar2 && AllCars.Count <= 1)
                {
                    PlayerCar2 = Instantiate (B.GameSettings.AvailableVehicles.First (v => v as CarController) as CarController);
                    if (StartPositions != null && StartPositions.Length > 1)
                    {
                        PlayerCar2.transform.position = StartPositions[0].position;
                        PlayerCar2.transform.rotation = StartPositions[0].rotation;
                    }
                    AllVehicles.Add (PlayerCar2);
                    AllCars.Add (PlayerCar2);
                }
                else if (!PlayerCar2)
                {
                    PlayerCar2 = AllCars[1];
                }
            }

            foreach (var vehicle in AllVehicles)
            {
                var prefab = B.GameSettings.AvailableVehicles.FirstOrDefault (v => vehicle.VehicleName == v.VehicleName);
                if (prefab == null)
                {
                    prefab = AllVehicles[0];
                }
                VehiclePrefabs.Add (prefab);
            }

            UpdateSelectedCars ();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Update ()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                TryResetCar ();
            }

            if (Input.GetKeyDown (KeyCode.F3))
            {
                RestartScene ();
            }

            if (Input.GetKeyDown (KeyCode.N))
            {
                SetNextCar ();
            }

            if (Input.GetKeyDown (KeyCode.KeypadPlus))
            {
                ChangeTimeScale (0.1f);
            }

            if (Input.GetKeyDown (KeyCode.KeypadMinus))
            {
                ChangeTimeScale (-0.1f);
            }
        }

        private void OnDestroy ()
        {
            if (IsMobilePlatform && MobileUI.Instance)
            {
                if (MobileUI.Instance.SelectNextCarButton)
                    MobileUI.Instance.SelectNextCarButton.onClick.RemoveListener (SetNextCar);
                if (MobileUI.Instance.RepairButton)
                    MobileUI.Instance.RepairButton.onClick.RemoveListener (TryResetCar);
                if (MobileUI.Instance.RestartSceneButton)
                    MobileUI.Instance.RestartSceneButton.onClick.RemoveListener (RestartScene);
            }
        }

        public void RestartScene ()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene (scene.buildIndex);
        }

        public void SetNextCar ()
        {
            if (PlayerCar1)
            {
                var studioListiner = PlayerCar1.GetComponent<AudioListener> ();

                if (studioListiner)
                {
                    studioListiner.enabled = false;
                }
            }

            var index = PlayerCar1? AllCars.IndexOf(PlayerCar1): 0;
            index = MathExtentions.Repeat (index + 1, 0, AllCars.Count - 1);

            PlayerCar1 = AllCars[index];
            UpdateSelectedCars ();
        }


        public void TryResetCar ()
        {
            var startPosition = StartPositions != null && StartPositions.Length > 0? StartPositions[0].position: Vector3.zero;
            var startRotation = StartPositions != null && StartPositions.Length > 0? StartPositions[0].rotation: Quaternion.identity;

            if (PlayerCar1)
            {
                startPosition = PlayerCar1.transform.position;
                startRotation = PlayerCar1.transform.rotation;
                var oldPlayerCar = PlayerCar1;

                var index = AllVehicles.IndexOf(oldPlayerCar);
                PlayerCar1 = Instantiate (VehiclePrefabs[index] as CarController);

                AllVehicles[index] = PlayerCar1;

                index = AllCars.IndexOf (oldPlayerCar);
                AllCars[index] = PlayerCar1;

                Destroy (oldPlayerCar.gameObject);
            }
            else
            {
                PlayerCar1 = Instantiate (VehiclePrefabs.First(v => v as CarController) as CarController);
            }
        
            PlayerCar1.transform.position = startPosition + Vector3.up * 2;
            PlayerCar1.transform.rotation = startRotation;

            if (SplitScreen)
            {
                startPosition = StartPositions != null && StartPositions.Length > 1 ? StartPositions[1].position : Vector3.zero;
                startRotation = StartPositions != null && StartPositions.Length > 1 ? StartPositions[1].rotation : Quaternion.identity;

                if (PlayerCar2)
                {
                    startPosition = PlayerCar2.transform.position;
                    startRotation = PlayerCar2.transform.rotation;
                    var oldPlayerCar = PlayerCar2;

                    var index = AllVehicles.IndexOf(oldPlayerCar);
                    PlayerCar2 = Instantiate (VehiclePrefabs[index] as CarController);

                    AllVehicles[index] = PlayerCar2;

                    index = AllCars.IndexOf (oldPlayerCar);
                    AllCars[index] = PlayerCar2;

                    Destroy (oldPlayerCar.gameObject);
                }
                else
                {
                    PlayerCar2 = Instantiate (VehiclePrefabs.First (v => v as CarController) as CarController);
                }

                PlayerCar2.transform.position = startPosition + Vector3.up * 2;
                PlayerCar2.transform.rotation = startRotation;
            }

            UpdateSelectedCars ();
        }

        void UpdateSelectedCars ()
        {
            Player1 = UpdateSelectedCar (Player1, PlayerCar1);
        }

        InitializePlayer UpdateSelectedCar (InitializePlayer player, CarController car)
        {
            PlayerController playerPrefab;
            
            playerPrefab = IsMobilePlatform?
                B.ResourcesSettings.PlayerControllerPrefab_ForMobile :
                B.ResourcesSettings.PlayerControllerPrefab;

            if (player && player.Car != car)
            {
                Destroy (player.gameObject);
                player = GameObject.Instantiate (playerPrefab);
            }
            if (!player)
            {
                player = GameObject.Instantiate (playerPrefab);
            }

            if (player.Initialize (car))
            {
                player.name = string.Format ("PlayerController_{0}", player.Vehicle.name);
                Debug.LogFormat ("Player for {0} is initialized", player.Vehicle.name);
            }

            var studioListiner = car.GetComponent<AudioListener> ();

            if (!studioListiner)
            {
                studioListiner = car.gameObject.AddComponent<AudioListener> ();
            }

            studioListiner.enabled = true;

            return player;
        }

        void ChangeTimeScale (float delta)
        {
            Time.timeScale = (Time.timeScale + delta).Clamp (0.1f, 2f);
            if (TimeScaleText)
            {
                TimeScaleText.SetActive (!Mathf.Approximately (Time.timeScale, 1));
                TimeScaleText.text = string.Format ("Time scale: {0}", Time.timeScale);
            }
        }
    }
}
