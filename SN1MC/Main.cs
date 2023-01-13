using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UWE;
using System.Collections;
using BepInEx;
using UnityEngine.XR;
using System.Collections.Generic;
using System.IO;
using Valve.VR;
using System;

namespace SN1MC
{
    //The Main Patch to start the mod and everything else
    extern alias SteamVRRef;
    extern alias SteamVRActions;
    [BepInPlugin(pluginGUID, pluginGUID, pluginVersion)]
    [BepInProcess("subnautica.exe")]
    public class Loader : BaseUnityPlugin
    {
        private const string pluginGUID = "mod.ihatetn931.subnautica.motioncontrols";
        private const string pluginName = "Motion Controls Subnautica";
        private const string pluginVersion = "1.0.0";

        void Awake()
        {
            //I guess if they don't want to play in vr they don't have to.
            if (!UnityEngine.XR.XRSettings.enabled)
            {
                return;
            }

            new GameObject("_SN1MC").AddComponent<SN1MC>();
            Debug.Log(pluginName + " " + pluginVersion + " " + "Patching...");
            Harmony harmony = new Harmony(pluginGUID);
            harmony.PatchAll();
            Debug.Log(pluginName + " " + pluginVersion + " " + "Patched");
           // Debug.Log(pluginName + " " + pluginVersion + " " + "Loading Assets...");
            //new AssetLoader();
           // Debug.Log(pluginName + " " + pluginVersion + " " + "Assets And Mod Loaded");
        }
    }

    public class SN1MC : MonoBehaviour
    {
        public static bool UsingSteamVR = false;

        public SN1MC()
        {
            DontDestroyOnLoad(gameObject);
        }

        internal void Awake()
        {
            SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);
        }

        void Start()
        {
            if (XRSettings.loadedDeviceName == "OpenVR")
                InitSteamVR();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CoroutineHost.StartCoroutine(RemoveNRecenter());
        }

        private static IEnumerator RemoveNRecenter()
        {
            yield return new WaitForSeconds(1);
            Recenter();
            yield break;
        }

        internal void Update()
        {
            if (FPSInputModule.current != null && UsingSteamVR)
            {
                if (SteamVRRef.Valve.VR.SteamVR_Input.GetState("Sprint", SteamVRRef.Valve.VR.SteamVR_Input_Sources.Any))
                {
                    FPSInputModule.current.lockMovement = true;
                    FPSInputModule.current.lockRotation = true;
                }
                else
                {
                    FPSInputModule.current.lockMovement = false;
                    FPSInputModule.current.lockRotation = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                Recenter();
            }
        }

        public static void Recenter()
        {
            if (XRSettings.loadedDeviceName == "Oculus")
            {
                VRUtil.Recenter();
                UsingSteamVR = false;
                return;
            }

            if (XRSettings.loadedDeviceName == "OpenVR")
            {
                SteamVRRef.Valve.VR.SteamVR.settings.trackingSpace = SteamVRRef.Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated;
                OpenVR.Compositor.SetTrackingSpace(ETrackingUniverseOrigin.TrackingUniverseSeated);
                OpenVR.System.ResetSeatedZeroPose();
                return;
            }
        }

        public static void InitSteamVR()
        {
            UsingSteamVR = true;

            Debug.Log("Initializing SteamVR_Actions...");
            SteamVRActions.Valve.VR.SteamVR_Actions.PreInitialize();
            Debug.Log("SteamVR Actions Initialize");
            Debug.Log("Initializing SteamVR...");
            SteamVRRef.Valve.VR.SteamVR.Initialize();
            Debug.Log("SteamVR Initialize");
            Debug.Log("Initializing SteamVR Input...");
            SteamVRRef.Valve.VR.SteamVR_Input.Initialize();
            Debug.Log("SteamVR Input Initialize");
            EVRInitError error = EVRInitError.None;
            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Other);
            if (error != EVRInitError.None)
                Debug.Log("There is a error it is " + error);

            SteamVRRef.Valve.VR.EVRInitError error1 = SteamVRRef.Valve.VR.EVRInitError.None;
            SteamVRRef.Valve.VR.OpenVR.Init(ref error1, SteamVRRef.Valve.VR.EVRApplicationType.VRApplication_Other);
            if (error1 != SteamVRRef.Valve.VR.EVRInitError.None)
                Debug.Log("There is a error it is " + error1);

            Recenter();
            //Get the type of HMD (for Pimax bugfixing)
            // PIMAX 5K Plus = Vive MV
            // HMDModel = UnityEngine.XR.XRDevice.model;
            //Logs.WriteInfo(HMDModel);
            //HideTab("init");
        }
    }
}