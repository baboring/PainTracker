using UnityEngine;
using System.Collections;

namespace HC {

	public enum ePartOfBody {
		none = 0,
		head,
		chest,
		stomach,
		hand_left,
		hand_right,
		leg_left,
		leg_right,
		foot_left,
		foot_right,
		thigh_left,
		thigh_right,
		arm_left,
		arm_right,
		upper_arm_left,
		upper_arm_right,
		body,
		tops,
		bottoms,
		hair,
		eye_left,
		eye_right,
	}

	public class TouchPart : MonoBehaviour {

		// own id
		public ePartOfBody id;
		public static TouchPart touchedObj;

		// 외부에서 컨트롤 하자
		public static void Reset() {
			touchedObj = null;
		}

		// Use this for initialization
		void OnClick() {
			if(touchedObj == null) {
				touchedObj = this;
				Logger.Debug(id.ToString());
			}
		}


	}
}

