using HarmonyLib;
using RootMotion;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;
//This is the laserpointer that pops up when you're using the Exosuit so you can aim both arms
namespace SN1MC.Controls
{
    public class LaserPointerLeft : MonoBehaviour
    {
        public enum AxisType
        {
            XAxis,
            ZAxis
        }

        public Color color;
        public static float thickness = 0.005f;
        public float length = FPSInputModule.current.maxInteractionDistance;
        GameObject holder;
        public GameObject pointerDot;
        public LineRenderer line;
        public static RaycastHit triggerObject;
        public static Image cImage;
        public static Graphic graph;
        public static GameObject cCursor;
        //public static RaycastHit useableObject;

        //public EventSystem eventSystem = null;

        public static Canvas canvas;
        public static Color colorRed = new Color(1, 0, 0, 1f);
        public static Color colorWhite = new Color(1, 1, 1, 1f);
        public static Color colorBlue = new Color(0, 0, 1, 1f);
        public static Color colorCyan = new Color(0, 1, 1, 1f);
        public Color colorOrange = new Color(1.0f, 0.64f, 0.0f, 1f);
        public static bool inMenu = false;

        void Start()
        {
            Material newMaterial = new Material(ShaderManager.preloadedShaders.scannerToolScanning);
            newMaterial.SetColor(ShaderPropertyID._Color, VRCustomOptionsMenu.laserPointerColor);
            // newMaterial.SetColor(ShaderPropertyID._Color, Color.cyan);

            holder = new GameObject();
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.zero;

            //newMaterial.SetColor(ShaderPropertyID.targetColor, colorWhite);
            //newMaterial.SetFloat(ShaderPropertyID.intensity, 1);

            pointerDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            if (pointerDot.GetComponent<SphereCollider>() != null)
                pointerDot.GetComponent<SphereCollider>().enabled = false;

            if (pointerDot.GetComponent<Renderer>() == null)
                pointerDot.AddComponent<Renderer>();

            if (pointerDot.GetComponent<Renderer>() != null)
                pointerDot.GetComponent<Renderer>().material = newMaterial;

            if (pointerDot.GetComponent<Canvas>() == null)
                pointerDot.AddComponent<Canvas>();

            //if (pointerDot.GetComponent<VRPointerInput>() == null)
            //    pointerDot.AddComponent<VRPointerInput>();
            // if (pointerDot.GetComponent<VRPointerInput>() != null)
            //     pointerDot.GetComponent<VRPointerInput>();

            if (pointerDot.GetComponent<Canvas>() != null)
                pointerDot.GetComponent<Canvas>();

            if (pointerDot.GetComponent<Camera>() == null)
                pointerDot.AddComponent<Camera>();
            if (pointerDot.GetComponent<Camera>() != null)
                pointerDot.GetComponent<Camera>();

            pointerDot.GetComponent<Camera>().enabled = false;

            if (line == null)
                line = holder.transform.gameObject.AddComponent<LineRenderer>();
            line = holder.transform.gameObject.GetComponent<LineRenderer>();

            line.material = newMaterial;
            // line.startColor = colorCyan;
            //  line.endColor = colorBlue;
            line.startColor = new Color(0f, 1f, 1f, 1f);
            line.endColor = new Color(1f, 0f, 0f, 1f);
            line.startWidth = 0.004f;
            line.endWidth = 0.005f;
            pointerDot.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            var layerNames = new string[] { "Default", "Interior", "TerrainCollider", "Trigger", "UI", "Useable" };
            foreach(var layers in layerNames)
            {
                line.gameObject.layer = 0;//LayerMask.NameToLayer(layers);
                pointerDot.layer = 0;//LayerMask.NameToLayer(layers);
            }
        }
        public void LaserPointerSet(Vector3 end)
        {
            if (VRHandsController.leftController != null)
            {
                line.SetPosition(0, transform.position);
                line.SetPosition(1, end);
                pointerDot.transform.position = new Vector3(end.x, end.y, end.z);
                pointerDot.transform.localPosition = new Vector3(end.x, end.y, end.z);
                //cCursor.transform.localPosition = end;
                //cCursor.transform.position = end;
            }
            /*else if (VRMenuController.leftController != null)
            {
                line.SetPosition(0, VRHandsController.leftController.transform.position + VRHandsController.leftController.transform.forward);
                line.SetPosition(1, end);
                pointerDot.transform.position = new Vector3(end.x, end.y, end.z);
                pointerDot.transform.localPosition = new Vector3(end.x, end.y, end.z);
                //cCursor.transform.localPosition = end;
                //cCursor.transform.position = end;
            }*/
            else
            {
                //  line.SetPosition(0, transform.position);
                //   line.SetPosition(1, transform.forward);
                //  pointerDot.transform.position = transform.position;
                //  pointerDot.transform.localPosition = transform.position;
                //  cCursor.transform.localPosition = transform.position;
                //  cCursor.transform.position = transform.forward;
            }
        }
        void SetNewLaserPointer()
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, FPSInputModule.current.GetCursorScreenPosition());
        }


        void SetLaserPointer()
        {
            // Debug.Log("SetLaserPointer1");
            Ray raycast = new Ray(transform.position, transform.forward);
            // Debug.Log("SetLaserPointer2");
            var layerNames = new string[] { "Default", "Interior", "TerrainCollider", "Trigger", "UI", "Useable" };
            // Debug.Log("SetLaserPointer3");

            var layerMask = LayerMask.GetMask(layerNames);
            // Debug.Log("SetLaserPointer4");
            bool triggerHit = Physics.Raycast(raycast, out triggerObject, layerMask);
            // Debug.Log("SetLaserPointer5");
            //  Debug.Log("Line: " + line);
            // Debug.Log("Transform: " + transform);
            line.SetPosition(0, transform.position);
            //  Debug.Log("SetLaserPointer6");
            if (FPSInputModule.current.lastRaycastResult.isValid)
            {
                //   Debug.Log("SetLaserPointer7");
                int layer = FPSInputModule.current.lastRaycastResult.gameObject.layer;
                // Debug.Log("SetLaserPointer8");
                var screenPointToRay = MainCamera.camera.ScreenPointToRay(FPSInputModule.current.lastRaycastResult.screenPosition).GetPoint(FPSInputModule.current.lastRaycastResult.distance);
                //Debug.Log("SetLaserPointer9");
                var screenPointToRay1 = FPSInputModule.current.lastRaycastResult.module.eventCamera.ScreenPointToRay(FPSInputModule.current.lastRaycastResult.worldPosition).GetPoint(FPSInputModule.current.lastRaycastResult.distance);
                //Debug.Log("SetLaserPointer10");

                if (layer == 0 && !VRHandsController.Started)
                {
                    //pointerDot.layer = LayerID.UI;
                    line.gameObject.layer = LayerID.UI;
                    //Debug.Log("SetLaserPointer11");
                }
                else
                {
                    //pointerDot.layer = LayerID.Default;
                    line.gameObject.layer = LayerID.Default;
                    //Debug.Log("SetLaserPointer12");
                }
                //Debug.Log("SetLaserPointer13");
                Vector3 pos = VRHandsController.rightController.transform.position + VRHandsController.rightController.transform.forward * FPSInputModule.current.maxInteractionDistance;
                line.SetPosition(1, Vector3.MoveTowards(transform.position, pos, FPSInputModule.current.maxInteractionDistance));
                //Debug.Log("SetLaserPointer14");
                //pointerDot.transform.position = Vector3.MoveTowards(transform.position, screenPointToRay, FPSInputModule.current.maxInteractionDistance);
                FPSInputModule.current.lastRaycastResult.Clear();
                //Debug.Log("SetLaserPointer15");

            }
            else
            {
                if (triggerHit)
                {
                    if (!triggerObject.transform.name.Contains("Sphere"))
                    {
                        //Debug.Log("SetLaserPointer16");
                        //float beamLength = GetBeamLength(triggerHit, triggerObject);
                        // Debug.Log("SetLaserPointer17");
                        line.SetPosition(0, transform.position);
                        //line.endColor = colorBlue;
                        //Debug.Log("SetLaserPointer18");
                        line.SetPosition(1, Vector3.MoveTowards(transform.position, triggerObject.point, FPSInputModule.current.maxInteractionDistance));
                        //Debug.Log("SetLaserPointer19");
                        // pointerDot.transform.position = Vector3.MoveTowards(transform.position, triggerObject.point, FPSInputModule.current.maxInteractionDistance);
                        // pointerDot.GetComponent<Renderer>().material.color = colorBlue;

                        FPSInputModule.current.lastRaycastResult.Clear();
                        //Debug.Log("SetLaserPointer20");
                    }

                }
                else
                {
                    // Debug.Log("SetLaserPointer1");
                    // line.SetPosition(0, transform.position);
                    //Debug.Log("SetLaserPointer1");
                    //line.endColor = colorRed;
                    line.SetPosition(1, transform.position + transform.forward * FPSInputModule.current.maxInteractionDistance);
                    // Debug.Log("SetLaserPointer1");

                    //pointerDot.transform.position = transform.position + transform.forward * FPSInputModule.current.maxInteractionDistance;
                    // pointerDot.GetComponent<Renderer>().material.color = colorRed;
                }

            }
        }

        void LateUpdate()
        {
            if (Player.main != null)
            {
                //disableArmAnimations();
                //logAnimations();
            }

            //SetLaserPointer();
            //SetNewLaserPointer();
        }
        void FixedUpdate()
        {
            if (Player.main != null)
            {
                // disableArmAnimations();
                //logAnimations();
            }
        }
        public void disableArmAnimations()
        {
            var player = global::Utils.GetLocalPlayerComp();
            //
            ErrorMessage.AddDebug("disableArmAnimationsStart");
            Animator anim = player.playerAnimator;
            Transform leftLowerArm = VRHandsController.ik.references.leftUpperArm;
            Transform rightLowerArm = VRHandsController.ik.references.rightUpperArm;
            AvatarMask mask = new AvatarMask();
            ErrorMessage.AddDebug("MaskNew: " + mask.GetTransformPath(1));
            ErrorMessage.AddDebug("MaskGet: " + player.GetComponent<AvatarMask>());
            mask.AddTransformPath(leftLowerArm, false);
            mask.AddTransformPath(rightLowerArm, false);
            PlayableGraph graph = player.playerAnimator.playableGraph;
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(graph, "LayerMixer", anim);
            AnimationMixerPlayable mixer = AnimationMixerPlayable.Create(graph, 2);
            playableOutput.SetSourcePlayable(mixer);
            // ErrorMessage.AddDebug("disableArmAnimationsEnd");
        }

        void AddFBBIK(GameObject go, BipedReferences references = null)
        {
            if (references == null)
            { // Auto-detect the biped definition if we don't have it yet
                BipedReferences.AutoDetectReferences(ref references, go.transform, BipedReferences.AutoDetectParams.Default);
            }
        }

        public void logAnimations()
        {
            AnimatorClipInfo[] anims = null;
            var player = global::Utils.GetLocalPlayerComp();
            Animator anim = player.playerAnimator;

            for (int i = 0; i < anim.layerCount; i++)
            {
                anims = Player.main.playerAnimator.GetCurrentAnimatorClipInfo(i);

                ErrorMessage.AddDebug("Animation: " + anims[i].clip.name);

            }
        }
    }
}




