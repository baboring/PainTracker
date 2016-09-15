/********************************************************************
	created:	2016/09/04
	filename:	AutoCameraOff.cs
	author:		Benjamin
	purpose:	[]
*********************************************************************/

using UnityEngine;
using System.Collections;

namespace HC {
    // 카메라 오브젝트가 있으면 끈다 ( UI 작업용 카메라 끄기 )
    public class AutoCameraOff : MonoBehaviour {
        // Use this for initialization
        void Awake() {
             if (null != gameObject.GetComponent<UICamera>()) {
                this.gameObject.SetActive(false);
                DestroyObject(this.gameObject);
            }
             if (null != gameObject.GetComponent<Camera>()) {
                this.gameObject.SetActive(false);
                DestroyObject(this.gameObject);
            }
                

        }
    }

}