using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace HC {

	public class Human : ManualSingletonMB<Human> {

		public Animator ani;
		public CapsuleCollider colliderSpin;

		public GameObject objHair;

		private Dictionary<ePartOfBody, PartOfBody> bodyParts = new Dictionary<ePartOfBody, PartOfBody>();
		private Dictionary<ePartOfBody, TouchPart> touchParts = new Dictionary<ePartOfBody, TouchPart>();

		void Awake() {
			instance = this;
			if (null == ani)
				ani = GetComponent<Animator>();

			// 시작할때는 꺼두자
			colliderSpin.enabled = false;

		}
		// Use this for initialization

		void Start() {

			FetchChildParts(bodyParts, this.gameObject);
			FetchChildParts(touchParts, this.gameObject);

			//ani.CrossFade("standard_idle", 0.1f);
		}

		public void Reset() {
			this.colliderSpin.transform.localRotation = Quaternion.identity;
			this.colliderSpin.enabled = false;
			this.colliderSpin.radius = 120;
			foreach (var v in touchParts.Values)
				v.gameObject.SetActive(true);

		}

		// 집중해서 보자..
		public void Focus(TouchPart obj) {

			// 볼것이 없으면 복구시켜야지
			if (null == obj) {
				foreach (var v in bodyParts.Values)
					v.gameObject.SetActive(true);
				Reset();
				return;
			}

			// 집중해서 보자
			this.colliderSpin.enabled = true;
			this.colliderSpin.radius = 60;
			foreach (var v in touchParts.Values)
				v.gameObject.SetActive(false);


			IEnumerable<PartOfBody> val;
			List<ePartOfBody> except;
			switch (obj.id) {
				case ePartOfBody.head:
					this.colliderSpin.radius = 22;
					except = new List<ePartOfBody>() {
						ePartOfBody.body,
						ePartOfBody.eye_left,
						ePartOfBody.eye_right,
					};
					break;
				case ePartOfBody.eye_left:
				case ePartOfBody.eye_right:
					except = new List<ePartOfBody>() {
						ePartOfBody.eye_left,
						ePartOfBody.eye_right,
					};
					break;
				case ePartOfBody.stomach:
				case ePartOfBody.chest:
					this.colliderSpin.radius = 40;
					except = new List<ePartOfBody>() {
						ePartOfBody.tops
					};
					break;
				case ePartOfBody.thigh_left:
				case ePartOfBody.thigh_right:
					except = new List<ePartOfBody>() {
						ePartOfBody.bottoms,
						ePartOfBody.body

					};
					break;
				case ePartOfBody.foot_left:
				case ePartOfBody.foot_right:
					except = new List<ePartOfBody>() {
						ePartOfBody.foot_left,
						ePartOfBody.foot_right
					};
					break;
				case ePartOfBody.hand_left:
				case ePartOfBody.hand_right:
				case ePartOfBody.upper_arm_left:
				case ePartOfBody.upper_arm_right:
				case ePartOfBody.arm_left:
				case ePartOfBody.arm_right:
				case ePartOfBody.leg_left:
				case ePartOfBody.leg_right:
					except = new List<ePartOfBody>() {
						ePartOfBody.body
					};
					break;
				default:
					except = new List<ePartOfBody>() {
						obj.id
					};
					break;
			}
			// 골라 내기
			val = bodyParts.Values.Where(va => except.FindIndex(vb=>vb == va.id) < 0);

			// 관련부를 제외한 부분은 모두 끄기
			foreach (var v in val)
				v.gameObject.SetActive(false);
		}

		static public void FetchChildParts(Dictionary<ePartOfBody, PartOfBody> lstChild, GameObject _object) {
			Logger.Assert(null != lstChild, "lstChild is null");
			PartOfBody uiWidget = _object.GetComponent<PartOfBody>();
			if (null != uiWidget)
				lstChild.Add(uiWidget.id, uiWidget);

			foreach (Transform child in _object.transform) {
				if (child == null)
					continue;
				FetchChildParts(lstChild, child.gameObject);
			};
		}

		static public void FetchChildParts(Dictionary<ePartOfBody, TouchPart> lstChild, GameObject _object) {
			Logger.Assert(null != lstChild, "lstChild is null");
			TouchPart uiWidget = _object.GetComponent<TouchPart>();
			if (null != uiWidget) {
				//Logger.DebugFormat("{0}<{1}>",uiWidget.id,uiWidget.name );
				lstChild.Add(uiWidget.id, uiWidget);
			}

			foreach (Transform child in _object.transform) {
				if (child == null)
					continue;
				FetchChildParts(lstChild, child.gameObject);
			};
		}

		public void OnPlay() {

		}
		void OnTouch() {
			string[] anyList = {
			"idle",
			"breathing_idle",
			"standard_idle",
			"waving",
			"fist_pump",
			"standing_greeting",
			"sad_idle",
			"closing",
			"relieved_sigh",
			"yelling",
			"searching_pockets",
			"yawn",
			"shaking_hands_2"
		};
			System.Random Ramdom = new System.Random();
			string aniName = anyList[Ramdom.Next(0, anyList.Length - 1)];
			//aniName = "dance";
			Debug.Log(aniName);
			ani.CrossFade(aniName, 0.1f);
		}

		public void filterAnimation(string text) {

			if (text.Contains("hi") || text.Contains("hello"))
				ani.CrossFade("standing_greeting", 0.05f);
			//else if (text.Contains("head") || text.Contains("arm"))
			//	ani.CrossFade("sad_idle", 0.05f);
			//else if (text.Contains("thigh"))
			//	ani.CrossFade("yelling", 0.05f);
			//else if (text.Contains("foot"))
			//	ani.CrossFade("relieved_sigh", 0.05f);
			//else
			//	ani.CrossFade("breathing_idle", 0.05f);
		}

		public void Motion(string animation) {
			if (animation.Length > 0)
				ani.CrossFade(animation, 0.03f);
		}

	}
}