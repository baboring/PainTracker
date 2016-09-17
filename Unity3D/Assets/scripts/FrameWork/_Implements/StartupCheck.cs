using UnityEngine;
using System.Collections;

namespace HC {
		
    public class StartupCheck : MonoBehaviour {

        // Use this for initialization
        void Start() {
            
            // check main and game system
            if (null == Main.instance) {
                Application.LoadLevel(0);
            }

            DestroyObject(this.gameObject);
	    }
    }

}
