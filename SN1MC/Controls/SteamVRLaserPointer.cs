using UnityEngine;
using UnityEngine.EventSystems;
//Might use this instead of my custom laser pointer
namespace SN1MC.Controls
{
    extern alias SteamVRRef;

    public class SteamVRLaserWrapper : MonoBehaviour
    {
        private SteamVRRef.Valve.VR.Extras.SteamVR_LaserPointer steamVrLaserPointer;

        private void Awake()
        {
            steamVrLaserPointer = gameObject.GetComponent<SteamVRRef.Valve.VR.Extras.SteamVR_LaserPointer>();
            steamVrLaserPointer.PointerIn += OnPointerIn;
            steamVrLaserPointer.PointerOut += OnPointerOut;
            steamVrLaserPointer.PointerClick += OnPointerClick;
        }

        private void OnPointerClick(object sender, SteamVRRef.Valve.VR.Extras.PointerEventArgs e)
        {
            IPointerClickHandler clickHandler = e.target.GetComponent<IPointerClickHandler>();
            if (clickHandler == null)
            {
                return;
            }


            clickHandler.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        private void OnPointerOut(object sender, SteamVRRef.Valve.VR.Extras.PointerEventArgs e)
        {
            IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();
            if (pointerExitHandler == null)
            {
                return;
            }

            pointerExitHandler.OnPointerExit(new PointerEventData(EventSystem.current));
        }

        private void OnPointerIn(object sender, SteamVRRef.Valve.VR.Extras.PointerEventArgs e)
        {
            IPointerEnterHandler pointerEnterHandler = e.target.GetComponent<IPointerEnterHandler>();
            if (pointerEnterHandler == null)
            {
                return;
            }

            pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));
        }
    }
}