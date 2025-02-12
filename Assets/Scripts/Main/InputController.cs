using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Main 
{
    public class InputController : MonoBehaviour
    {
        public bool IsHolding()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return false;

                return Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved;
            }

            return false;
        }

        public bool IsReleasing()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return false;

                return Input.GetTouch(0).phase == TouchPhase.Ended;
            }

            return false;
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
