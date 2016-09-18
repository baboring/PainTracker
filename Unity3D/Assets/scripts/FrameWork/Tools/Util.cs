/********************************************************************
	created:	2013/06/26 
	filename:	StaticFunction.cs
	author:		ehalshbest
	purpose:	[]
*********************************************************************/
using UnityEngine;

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HC
{
    // 유틸함수 모음
	public class Util
	{

        // 자식을 돌면서 레이어 제설정.
		static public void SetLayerRecursively(GameObject _object, System.Int32 _layer)
		{
			if (null == _object || _layer < 0)
				return;
			_object.layer = _layer;
			foreach( Transform child in _object.transform ) {
				if( child == null ) {
					continue;
				};
				SetLayerRecursively(child.gameObject, _layer);
			};

			return;
		}

        // 랜덤값 얻기.
		static public System.Int32 GetRandomInt( System.Int32 _max_value )
		{
            return UnityEngine.Random.Range(0, _max_value);
		}

        // @cloud
        // 사용하는곳이 없다.
		static public void ReSizeObject(GameObject _game_object)
		{
			var origin_size_x = _game_object.transform.localScale.x;
			var origin_size_y = _game_object.transform.localScale.y;

			var resize_x = (origin_size_x * Screen.width) / SystemConfig.DefScreenWidth;
			var resize_y = (origin_size_y * Screen.height) / SystemConfig.DefScreenHeight;

			_game_object.transform.localScale = new UnityEngine.Vector3(resize_x, resize_y, _game_object.transform.localScale.z);
		}

		// 오프셋 재설정
		// - 해상도에 따라서 변경될수 있도록함.
		static public void ReOffest(GameObject _game_object)
		{
			var origin_offest_x = _game_object.transform.localPosition.x;
			var origin_offest_y = _game_object.transform.localPosition.y;

			var offest_x = (origin_offest_x * Screen.width) / SystemConfig.DefScreenWidth;
			var offest_y = (origin_offest_y * Screen.height) / SystemConfig.DefScreenHeight;

			_game_object.transform.localPosition = new UnityEngine.Vector3(offest_x, offest_y, _game_object.transform.localPosition.z);
		}

		// @cloud
		// 사용하는곳이 없다.
		static public void ReColider(GameObject _game_object, UnityEngine.Vector3 _resize)
		{
			_game_object.GetComponent<BoxCollider>().size = new UnityEngine.Vector3(_resize.x, _resize.y, 0);
		}

		// 클리핑영역 리사이즈 처리
		// - 해상도에 따라서 변경될수 있도록함.
		static public void ReSizeClipping(UIPanel _panel)
		{
			var origin_size_w = _panel.baseClipRegion.w;
			var origin_size_x = _panel.baseClipRegion.x;
			var origin_size_y = _panel.baseClipRegion.y;
			var origin_size_z = _panel.baseClipRegion.z;

			var resize_z = (origin_size_z * Screen.width) / SystemConfig.DefScreenWidth;
			var resize_w = (origin_size_w * Screen.height) / SystemConfig.DefScreenHeight;

			_panel.baseClipRegion = new Vector4(origin_size_x, origin_size_y, resize_z, resize_w);
		}

		// @cloud
		// 사용하는곳이 없다.
		static public void ReSizePlanObject(GameObject _game_object)
		{
			var origin_size_x = _game_object.transform.localScale.x;
			var origin_size_z = _game_object.transform.localScale.z;

			var resize_x = (origin_size_x * Screen.width) / SystemConfig.DefScreenWidth;
			var resize_z = (origin_size_z * Screen.height) / SystemConfig.DefScreenHeight;

			_game_object.transform.localScale = new UnityEngine.Vector3(resize_x, _game_object.transform.localScale.y, resize_z);
		}

		// 스프라이트의 스케일을 원래 크기로 맞춤
		// 스프라이트 교체 시 이전 스프라이트의 스케일로 적용되는 경우 사용
		static public void OriginScaleUISprite(UISprite sprite, float scaleRate = 1f)
		{
			Logger.Assert(sprite.GetAtlasSprite() != null, "GetAtlasSprite is null");
			if (null == sprite.GetAtlasSprite())
				return;
			float width = sprite.GetAtlasSprite().width * scaleRate;
			float height = sprite.GetAtlasSprite().height * scaleRate;
			sprite.transform.localScale = new Vector3(width, height, 1);
		}

		// 자식을 돌면서 카메라 붙이기
		static public void SetCameraChildrens(GameObject _object, Camera _gamera)
		{
			UIAnchor[] anchors = _object.GetComponentsInChildren<UIAnchor>();

			Array.ForEach<UIAnchor>(anchors, va => va.uiCamera = _gamera);
		}

		// 파일 Path 알아오기
		static public string pathForDocumentsFile(string filename)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
				path = path.Substring(0, path.LastIndexOf('/'));
				return Path.Combine(Path.Combine(path, "Documents"), filename);
			}
			else if (Application.platform == RuntimePlatform.Android) {
				string path = Application.persistentDataPath;
				path = path.Substring(0, path.LastIndexOf('/'));
				return Path.Combine(path, filename);
			}
			else {
				string path = Application.dataPath;
				path = path.Substring(0, path.LastIndexOf('/'));
				return Path.Combine(path, filename);
			}
		}

		// 하위의 위젯들만 모아 온다.
		static public void FetchChildWigets(Dictionary<UIWidget, Color> lstChild, GameObject _object)
		{
			Logger.Assert(null != lstChild, "lstChild is null");
			UIWidget uiWidget = _object.GetComponent<UIWidget>();
			if (null != uiWidget)
				lstChild.Add(uiWidget, uiWidget.color);

			foreach (Transform child in _object.transform) {
				if (child == null)
					continue;
				FetchChildWigets(lstChild, child.gameObject);
			};
		}

		static public void FetchChildWigets(List<UIWidget> lstChild, GameObject _object) {
			Logger.Assert(null != lstChild, "lstChild is null");
			UIWidget uiWidget = _object.GetComponent<UIWidget>();
			if (null != uiWidget)
				lstChild.Add(uiWidget);

			foreach (Transform child in _object.transform) {
				if (child == null)
					continue;
				FetchChildWigets(lstChild, child.gameObject);
			};
		}

		// 인터넷에서 긁어왔음. cpfnqla, 두 선분이 교차하는지 검사, 교차점은 result로 
		static public bool CheckCross(Vector2 AP1, Vector2 AP2, Vector2 BP1, Vector2 BP2, ref Vector2 result)
		{
			float t;
			float s;
			float under = (BP2.y - BP1.y) * (AP2.x - AP1.x) - (BP2.x - BP1.x) * (AP2.y - AP1.y);
			if (under == 0)
				return false;

			float _t = (BP2.x - BP1.x) * (AP1.y - BP1.y) - (BP2.y - BP1.y) * (AP1.x - BP1.x);
			float _s = (AP2.x - AP1.x) * (AP1.y - BP1.y) - (AP2.y - AP1.y) * (AP1.x - BP1.x);

			if (_t == 0 && _s == 0)
				return false;

			t = _t / under;
			s = _s / under;

			if (t < 0.0 || t > 1.0 || s < 0.0 || s > 1.0)
				return false;

			//result.x = AP1.x + t * (AP2.x-AP1.x);
			//result.y = AP1.y + t * (AP2.y-AP1.y);
			result.x = BP1.x + s * (BP2.x - BP1.x);
			result.y = BP1.y + s * (BP2.y - BP1.y);

			return true;
		}

		static public int RandomRange(int min_include, int max_include)	// max가 int는 exclusive 이고 float 는 include다 ( 설명 미스매치다~~~)
		{
			return UnityEngine.Random.Range(min_include, max_include + 1);
		}
		static public float RandomRange(float min_include, float max_include)	// max가 int는 exclusive 이고 float 는 include다 ( 설명 미스매치다~~~)
		{
			return UnityEngine.Random.Range(min_include, max_include);
		}

		// topParentObj 의 하위 컨포넌트에서 해당 이름의 GO를 찾는다.
		static public GameObject FindGameObjectInChildren(GameObject topParentObj, string objName)
		{
			Transform[] childTr = topParentObj.GetComponentsInChildren<Transform>();
			var result = childTr.Where(v => v.name == objName);
			if (result.Count() < 1)
				return null;
			Transform findTr = result.First();
			if (findTr != null)
				return findTr.gameObject;
			return null;
		}
	}

	public static class ExtensionMethods
	{
		static public Transform Search(this Transform target, string name)
		{
			if (target.name == name) return target;

			for (int i = 0; i < target.childCount; ++i) {
				var result = Search(target.GetChild(i), name);

				if (result != null) return result;
			}

			return null;
		}

		//컴퍼넌트 가져오고 없으면 추가하는 기능 한번에
		static public T GetOrAddComponent<T>(this Component child) where T : Component {
			T result = child.GetComponent<T>();
			if (result == null) {
				result = child.gameObject.AddComponent<T>();
			}
			return result;
		}

		//포지션 변환
		static public void SetLocalPosX(this Transform t, float newX) {
			t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);
		}
		static public void SetLocalPosY(this Transform t, float newY) {
			t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);
		}
		static public void SetLocalPosZ(this Transform t, float newZ) {
			t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);
		}
		static public void SetLocalPosXY(this Transform t, float newX, float newY) {
			t.localPosition = new Vector3(newX, newY, t.localPosition.z);
		}
		static public void SetLocalPosYZ(this Transform t, float newY, float newZ) {
			t.localPosition = new Vector3(t.localPosition.x, newY, newZ);
		}
		static public void SetLocalPosXZ(this Transform t, float newX, float newZ) {
			t.localPosition = new Vector3(newX, t.localPosition.y, newZ);
		}

		

	}
}