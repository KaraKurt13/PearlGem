using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Main 
{
    public class InputController : MonoBehaviour
    {
        public bool TouchHasBegun()
        {
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        }

        public bool IsHolding()
        {
            return Input.touchCount > 0 &&
                (Input.GetTouch(0).phase == TouchPhase.Stationary ||
                Input.GetTouch(0).phase == TouchPhase.Moved);
        }

        public bool IsReleasing()
        {
            return Input.touchCount > 0 &&
                Input.GetTouch(0).phase == TouchPhase.Ended;
        }

        public Vector3 GetTouchPosition()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                return new Vector3(touch.position.x, touch.position.y, 0);
            }
            return Vector3.zero;
        }

        public Vector3 GetWorldTouchPosition()
        {
            var touchPosition = GetTouchPosition();
            touchPosition.z = -Camera.main.transform.position.z;
            return Camera.main.ScreenToWorldPoint(touchPosition);
        }
    }
}
