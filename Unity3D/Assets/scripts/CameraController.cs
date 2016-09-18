using UnityEngine;
using System.Collections;

namespace HC {
    public enum ZOOM_ID {
        HEAD,
        THIGH,
        STOMACK,
        NONE,
    }

    public class CameraController : ManualSingletonMB<CameraController>
	{
        /// <summary>
        /// camera
        /// </summary>
        public LookAtTarget camLookAt;
        public Transform posBody;

        public Transform posHead;
        public Transform posCameraHead;
        public Transform posThigh;
        public Transform posCameraThigh;
        public Transform posStomach;
        public Transform posCameraStomach;


        // Use this for initialization
        void Awake() {
			instance = this;
			DontDestroyOnLoad(instance);
		}

		void Start() {

			Initial();
		}

		TransformData td;
        void Initial() {
            td = cameraTrans.Clone();
        }


        public string myName = "Benjamin";
        public void OnSay() {

            // test code
            //int id = (int)zoom_id;
            //if (zoom_mode_IN)
            //    SetZoomInOut((ZOOM_ID)(++id % 3), false);
            //else
            //    SetZoomInOut((ZOOM_ID)(id % 3), true);

            //string saySomething = string.Format("My name is %s.", uiInput.value);
   //         if (uiInput.value.Length > 0)
   //             myName = uiInput.value;
			//WindowManager.GetWindow<wndMain>().objHuman.OnSay("My name is " + myName, "shaking_hands_2");
   //         SetZoomInOut(ZOOM_ID.NONE, false);
        }
        // Update is called once per frame
        void Update() {

            UpdateZoomInOut();
        }


        public Transform cameraTrans;

        bool zoom_mode_IN = false;
        ZOOM_ID zoom_id = 0;
        float zoom_speed = 4.0f;



        public void SetZoomInOut(ZOOM_ID id, bool bZoomIn, float speed = 4.0f) {

            if (bZoomIn) {
                switch (id) {
                    case ZOOM_ID.HEAD:
                        camLookAt.target = posHead;
                        break;
                    case ZOOM_ID.THIGH:
                        camLookAt.target = posThigh;
                        break;
                    case ZOOM_ID.STOMACK:
                        camLookAt.target = posStomach;
                        break;
                }
            }
            else {
                camLookAt.target = posBody;
            }

            zoom_id = id;
            zoom_mode_IN = bZoomIn;
            zoom_speed = speed;
        }

        void UpdateZoomInOut() {

            if (zoom_mode_IN) {
                Vector3 target = Vector3.zero;
                switch (zoom_id) {
                    case ZOOM_ID.HEAD:
                        target = posCameraHead.position;
                        break;
                    case ZOOM_ID.THIGH:
                        target = posCameraThigh.position;
                        break;
                    case ZOOM_ID.STOMACK:
                        target = posCameraStomach.position;
                        break;
                }

                if (!cameraTrans.position.Equals(target))
                    cameraTrans.position = Vector3.Slerp(cameraTrans.position, target, Time.deltaTime * zoom_speed);
            }
            else {
                if (!cameraTrans.position.Equals(td.position))
                    cameraTrans.position = Vector3.Slerp(cameraTrans.position, td.position, Time.deltaTime * zoom_speed);
            }
        }

	}

}