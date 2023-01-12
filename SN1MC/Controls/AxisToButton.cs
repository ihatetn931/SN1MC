/*using System.Collections;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SN1MC.Controls
{
    class AxisToButton
    {
        public static GameInput.Button _lastInputAxisStateLeft1;
        public static bool _lastInputAxisStateLeft;
        public static bool _lastInputAxisStateRight;
        public static bool _lastInputAxisStateUp;
        public static bool _lastInputAxisStateDown;
        public static bool selectingQuickslots;
        //public static bool deConstructing;
        public static GameInput.Button currentButton;

        public static bool DPadLeftPressed1()
        {
            var currentInputValue = GameInput.axisValues[2] < -0.9 && !deConstructing;
            if (currentInputValue)
                currentButton = GameInput.Button.CyclePrev;
            
            _lastInputAxisStateLeft1 = currentButton;


            //return currentButton;
            return (GameInput.GetInputStateForButton(currentButton).flags & GameInput.InputStateFlags.Down) > (GameInput.InputStateFlags)0U;

        }

        public static bool DPadLeftPressed()
        {
            var currentInputValue = GameInput.axisValues[2] < -0.1 && !deConstructing;
            if (currentInputValue && _lastInputAxisStateLeft)
            {
                return false;
            }
            _lastInputAxisStateLeft = currentInputValue;
            return currentInputValue;

        }
        public static bool DPadLeftHeld()
        {
            var currentInputValue = GameInput.axisValues[2] < -0.9 && !deConstructing;
            if (currentInputValue && _lastInputAxisStateLeft)
            {
                return false;
            }
            _lastInputAxisStateLeft = currentInputValue;
            return currentInputValue;
        }
        public static bool DPadLeftReleased()
        {
            var currentInputValue = GameInput.axisValues[2] == -0.0 && !deConstructing;
            if (currentInputValue && _lastInputAxisStateLeft)
            {
                return false;
            }
            _lastInputAxisStateLeft = currentInputValue;
            return currentInputValue;
        }

        public static bool DPadRightPressed()
        {
            var currentInputValue = GameInput.axisValues[2] > 0.1 && !deConstructing;
            if (currentInputValue && _lastInputAxisStateRight)
            {
                return false;
            }
            _lastInputAxisStateRight = currentInputValue;
            return currentInputValue;
        }
        public static bool DPadRightHeld()
        {
            var currentInputValue = GameInput.axisValues[2] > 0.9 && !deConstructing;
            if (currentInputValue && _lastInputAxisStateRight)
            {
                return false;
            }
            _lastInputAxisStateRight = currentInputValue;
            return currentInputValue;
        }
        public static bool DPadRightReleased()
        {
            var currentInputValue = GameInput.axisValues[2] == 0.0 && !deConstructing;
            if (currentInputValue && _lastInputAxisStateRight)
            {
                return false;
            }
            _lastInputAxisStateRight = currentInputValue;
            return currentInputValue;
        }


        public static bool DPadUpPressed()
        {
            var currentInputValue = GameInput.axisValues[3] != -0.0 && !selectingQuickslots;
            if (currentInputValue && _lastInputAxisStateUp)
            {
                return false;
            }
            _lastInputAxisStateUp = currentInputValue;
            return currentInputValue;
        }
        public static bool DPadUpHeld()
        {
            var currentInputValue = GameInput.axisValues[3] < -0.9 && !selectingQuickslots;
            if (currentInputValue && _lastInputAxisStateUp)
            {
                return false;
            }
            _lastInputAxisStateUp = currentInputValue;
            return currentInputValue;
        }
        public static bool DPadUpReleased()
        {
            var currentInputValue = GameInput.axisValues[3] == -0.0 && !selectingQuickslots;
            if (currentInputValue && _lastInputAxisStateUp)
            {
                return false;
            }
            _lastInputAxisStateUp = currentInputValue;
            return currentInputValue;
        }

        public static bool DPadDownPressed()
        {
            var currentInputValue = GameInput.axisValues[3] > 0.1 && !selectingQuickslots;
            if (currentInputValue && _lastInputAxisStateDown)
            {
                return false;
            }
            _lastInputAxisStateDown = currentInputValue;
            return currentInputValue;
        }
        public static bool DPadDownHeld()
        {
            var currentInputValue = GameInput.axisValues[3] > 0.9 && !selectingQuickslots;
            if (currentInputValue && _lastInputAxisStateDown)
            {
                return false;
            }
            _lastInputAxisStateDown = currentInputValue;
            return currentInputValue;
        }
        public static bool DPadDownReleased()
        {
            var currentInputValue = GameInput.axisValues[3] == 0.0 && !selectingQuickslots;
            if (currentInputValue && _lastInputAxisStateDown)
            {
                return false;
            }
            _lastInputAxisStateDown = currentInputValue;
            return currentInputValue;
        }



    }
}*/














        /*public static bool _lastInputAxisStateLeft;
        public static bool _lastInputAxisStateRight;
        public static bool _lastInputAxisStateUp;
        public static bool _lastInputAxisStateDown;
        public static bool toolSelect;
        public static bool deConstruct;

        public static bool DePadLeft()
        { 
            var currentInputValue = GameInput.axisValues[2] < -0.9 && !deConstruct;

            // prevent keep returning true when axis still pressed.
            if (currentInputValue && _lastInputAxisStateLeft)
            {
                return false;
            }

            _lastInputAxisStateLeft = currentInputValue;
            return currentInputValue;
        }

        public static bool DePadRight()
        {
            var currentInputValue = GameInput.axisValues[2] > 0.9 && !deConstruct;

            // prevent keep returning true when axis still pressed.
            if (currentInputValue && _lastInputAxisStateRight)
            {
                return false;
            }

            _lastInputAxisStateRight = currentInputValue;
            return currentInputValue;
        }



        public static bool DPadDownPressed()
        {
            var currentInputValue = GameInput.axisValues[3] > 0.1 && !toolSelect;

            // prevent keep returning true when axis still pressed.
            if (currentInputValue && _lastInputAxisStateDown)
            {
                return false;
            }

            _lastInputAxisStateDown = currentInputValue;

            return currentInputValue;
        }


        public static bool DPadDownHeld()
        {
            var currentInputValue = GameInput.axisValues[3] != 0.0 && !toolSelect;

            // prevent keep returning true when axis still pressed.
            if (currentInputValue && _lastInputAxisStateDown)
            {
                return false;
            }

            _lastInputAxisStateDown = currentInputValue;

            return currentInputValue;
        }


        public static bool DPadDownReleased()
        {
            var currentInputValue = GameInput.axisValues[3] < 0.1 && !toolSelect;

            // prevent keep returning true when axis still pressed.
            if (currentInputValue && _lastInputAxisStateDown)
            {
                return false;
            }

            _lastInputAxisStateDown = currentInputValue;

            return currentInputValue;
        }



        public static bool DePadUpPressed()
        {
            var currentInputValue = GameInput.axisValues[3] < -0.1 && !toolSelect;

            // prevent keep returning true when axis still pressed.
            if (currentInputValue && _lastInputAxisStateUp)
            {
                return false;
            }

            _lastInputAxisStateUp = currentInputValue;

            return currentInputValue;
        }
        public static bool DePadUpHeld()
        {
            var currentInputValue = GameInput.axisValues[3] < -0.2 && !toolSelect;

            // prevent keep returning true when axis still pressed.
            if (currentInputValue && _lastInputAxisStateUp)
            {
                return false;
            }

            _lastInputAxisStateUp = currentInputValue;

            return currentInputValue;
        }

        public static bool DePadUpReleased()
        {
            var currentInputValue = GameInput.axisValues[3] > -0.1 && !toolSelect;

            // prevent keep returning true when axis still pressed.
            if (currentInputValue && _lastInputAxisStateUp)
            {
                return false;
            }

            _lastInputAxisStateUp = currentInputValue;

            return currentInputValue;
        }
    }
}*/
