using UnityEngine;
using System.Collections;

namespace HC {
    public enum CAMERA_ID {
		NONE,
		HEAD,
        THIGH,
        STOMACK,
    }

    public class CameraController : ManualSingletonMB<CameraController>
	{
        /// <summary>
        /// camera
        /// </summary>
		/// 
        public LookAtTarget camLookAt;
		public MoveToTarget camMover;
		public Camera cam;

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

			initCamTrans = cameraTrans.Clone();

			camMover.target = cameraTrans;
		}
		Transform centerTrans {
			get {
				this.transform.position = initCamTrans.position;
				return this.transform;
			}
		}

		TransformData initCamTrans;

        public Transform cameraTrans;

        public CAMERA_ID zoom_id = CAMERA_ID.NONE;
        float zoom_speed = 4.0f;


		public void SetLookatPart(TouchPart which) {

			camLookAt.target = (null != which)? which.transform : posBody;
			if (which != null)
				this.transform.localPosition = camLookAt.target.localPosition + new Vector3(0, 20, -150);
			else
				this.transform.localPosition = initCamTrans.localPosition;
			camMover.target = this.transform;
		}

		public void SetCameraView(CAMERA_ID id, float speed = 4.0f) {

            switch (id) {
                case CAMERA_ID.HEAD:
                    camLookAt.target = posHead;
					camMover.target = posCameraHead;
					break;
                case CAMERA_ID.THIGH:
                    camLookAt.target = posThigh;
					camMover.target = posCameraThigh;
					break;
                case CAMERA_ID.STOMACK:
                    camLookAt.target = posStomach;
					camMover.target = posCameraStomach;
					break;
				case CAMERA_ID.NONE:
					camLookAt.target = posBody;
					this.transform.localPosition = initCamTrans.localPosition;
					camMover.target = this.transform;
					break;
            }
            zoom_id = id;
            zoom_speed = speed;
        }

		// Update is called once per frame
		void UpdateLate() {

			// 
			if (Input.GetMouseButtonUp(0)) {
				OnMouseUp();
			}
		}

		void Update() {
			if (null == cam)
				return;
			RaycastHit hit;
			if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
				return;

			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider == null || meshCollider.sharedMesh == null)
				return;

			Mesh mesh = meshCollider.sharedMesh;
			Vector3[] vertices = mesh.vertices;
			int[] triangles = mesh.triangles;
			Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
			Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
			Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
			Transform hitTransform = hit.collider.transform;
			p0 = hitTransform.TransformPoint(p0);
			p1 = hitTransform.TransformPoint(p1);
			p2 = hitTransform.TransformPoint(p2);
			Debug.DrawLine(p0, p1);
			Debug.DrawLine(p1, p2);
			Debug.DrawLine(p2, p0);
		}

		void OnMouseUp() {
			/*
				Ray ray = cam.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) {
					//clear edge colors
					for (int i = 0; i < m_edgePoints.Count; i++)
						m_edges[i].GetComponent<LineRenderer>().renderer.material.color = Color.red;
					//find vertices of the triangle hit                
					Mesh m = (hit.collider as MeshCollider).sharedMesh;
					Vector3[] vertices = new Vector3[3] { m.vertices[m.triangles[3 * hit.triangleIndex]],
					 m.vertices[m.triangles[3 * hit.triangleIndex + 1]], m.vertices[m.triangles[3 * hit.triangleIndex + 2]]
				 };

					Debug.Log(string.Format("Triangle: {0}; {1}; {2}", vertices[0], vertices[1], vertices[2]));
					//find selected face
					foreach (var face in m_faces) {
						if (face.Contains(vertices[0]) && face.Contains(vertices[1]) && face.Contains(vertices[2])) {
							Debug.Log("face: " + string.Join("; ", (from v in face select v.ToString()).ToArray()));
							//color edges of the face
							for (int i = 0; i < m_edgePoints.Count; i++) {
								if (face.Contains(m_edgePoints[i][0]) && face.Contains(m_edgePoints[i][1])) {
									m_edges[i].GetComponent<LineRenderer>().renderer.material.color = Color.blue;
									Debug.Log(string.Format("Edge: {0}; {1}", m_edgePoints[i][0], m_edgePoints[i][1]));
								}
							}
							break;
						}
					}
				}
			*/
		}

	}

}