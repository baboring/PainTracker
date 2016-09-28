using UnityEngine;

namespace HC
{
    public class MoveToTarget : MonoBehaviour
    {
        public int level = 0;

        public Transform target;
		public float speed = 8f;

        Transform mTrans;

        void Start()
        {
            mTrans = transform;
        }

		void LateUpdate()
        {

			if (target != null)
            {
				Vector3 targetPos = mTrans.position;
				targetPos = target.position;

				Vector3 dir = targetPos - mTrans.position;

                float mag = dir.magnitude;

                if (mag > 0.001f) {
                    mTrans.position = Vector3.Slerp(mTrans.position, targetPos, Time.deltaTime * speed);
                }
            }
        }
    }
}