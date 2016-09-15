using UnityEngine;
using System;
using System.Collections;

namespace HC
{
    public class InitialWindows : MonoBehaviour
    {
        public GameObject[] objList;

		[NonSerialized]
		static public bool IsLoadComplete = false;

        void Awake() {
            IsLoadComplete = false;
            WindowManager.AddInitialWindows(this);
        }

        // Use this for initialization
		IEnumerator Start()
        {
            // 별도 component를 가지고 있는 오브젝트 들은 Active 해준다.(자신이 Deactive 하는것은 본인에게 맞김 )
            foreach (GameObject go in objList)
            {
				// 등록 안되어 있는놈만 하자.
                if (null != go && !go.activeSelf && !WindowManager.IsRegisterWindow(go))
                {
                    //Debug.Log("Active Window !! ( " + go.name + " )");
                    go.SetActive(true);
					yield return null;
                }
            }
			IsLoadComplete = true;
            //DestroyObject(this.gameObject);
        }

		// 코루틴으로 시작해보자
		
    }
}