/********************************************************************
	created:	2013/11/25
	filename:	Singleton.cs
	author:		Benjamin
	purpose:	[MonoBehaviour을 위한 싱글톤]
/// Non-thread-safe convenience class
/// @bluegol 201403
/// 주안점
/// (1) 보일러플레이트 코드를 최소화! 편하게 사용 가능할 것. (즉, 상속만 하면 되게)
/// (2) 싱글톤이어야 함!!!
/// (3) T를 인젝트하여 사용할 수 있게. 즉 싱글톤으로도, 싱글톤이 아니게도 사용 가능하게.
/// 이렇게 하려면 컨스트럭터를 건드릴 수는 없다. 따라서, Instance만을 거쳐 사용하도록 한다.
/// (4) 명확히 초기화가 필요하도록 할 것. 즉 초기화 없이 바로 .Instance를 콜하면 죽어버리도록.
///
/// * c# generics는 type parameter에서 derive하는 걸 허용하지 않는다. 그래서 베이스 클래스를 지정할 수가 없다 ㅠㅠ
/// 그래서, 베이스 클래스에 따라서 여러버전을 만들 수 밖에 없다.
/// * MB의 경우 컨스트럭터에서 뭐 하기가 굉장히 힘들다. 따라서 Awake에서 수동을 Instance = this; 를 넣도록 한다.
*********************************************************************/
using UnityEngine;
using System;


public class SingletonMB<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;
	private static object _lock = new object();

	protected Action DestroyEventHandler;	// 삭제시 호출될 핸들러

	// Returns the instance of the singleton
	public static T instance
	{
		get
		{
			lock (_lock) {
				if (null == _instance) {
					_instance = (T)FindObjectOfType(typeof(T));
					if (null == _instance) {
						GameObject obj = new GameObject(typeof(T).ToString());
						_instance = obj.AddComponent<T>();
						if (null == _instance)
							Debug.LogError("FATAL! Cannot create an instance of " + typeof(T) + ".");
						else {
							DontDestroyOnLoad(obj);
						}
					}
					else {
						Debug.Log("Aleady Instance of " + typeof(T) + " exists in the scene.");
					}
				}
			}
			return _instance;
		}
	}
	protected virtual void Awake()	{
		Debug.Log("Awake singleton : " + typeof(T));
	}

	public virtual void Initial() {

	}

	public static bool IsInstanced { get { return  null != _instance; } }
	public static void SelfDestroy()
	{
        if (null != _instance) {
			DestroyImmediate( _instance.gameObject );
			//_instance = null;
		}
	}

	void OnApplicationQuit()
	{
		_instance = null;
		_lock = null;
	}

	void OnDestroy()
	{
        if (this != _instance)
            return;
		_instance = null;

		if (DestroyEventHandler != null)
			DestroyEventHandler();

		//Debug.Log("Singleton object destroy");
	}
}


public abstract class ManualSingleton<T> where T : class, new()
{
	static protected T _instance;
	static public T instance
	{
		get { return _instance; }
		set
		{
			if (_instance != null)
				throw new System.ApplicationException("cannot set Instance twice!");

			_instance = value;
		}
	}
    public ManualSingleton() {
        if (_instance != null)
            throw new System.ApplicationException("cannot set Instance twice!");
    }

	static public T Instantiate()
	{
		instance = new T();
		return instance;
	}

	static public T SetInstance(T ins)
	{
		instance = ins;
		return ins;
	}

	static public void Destroy()
	{
		_instance = null;
	}



}


public abstract class Singleton<T> : ManualSingleton<T> where T : class, new()
{
	static new public T instance
	{
		get
		{
			if (_instance == null)
				Instantiate();
			return _instance;
		}
		set
		{
			if (_instance != null)
				throw new System.ApplicationException("cannot set Instance twice!");

			_instance = value;
		}
	}

    public Singleton() {
        if (_instance != null)
            throw new System.ApplicationException("cannot set Instance twice!");
    }
}


public abstract class ManualSingletonMB<T> : UnityEngine.MonoBehaviour where T : ManualSingletonMB<T>
{
	static private T _instance;
	static public T instance
	{
		get { return _instance; }
		set
		{
			if (_instance != null)
				throw new System.ApplicationException("cannot set Instance twice!");

			_instance = value;
		}
	}

	protected Action DestroyEventHandler;

	void OnDestroy()
	{
		// @bluegol 201405
		// 테스트 등을 위해, 게임 상에 2개가 생길 수도 있는데 이런 경우, Awake에서 바로 disable시킨다.
		// 따라서 이 체크가 필요함
		if (_instance != this)
			return;

		if (DestroyEventHandler != null)
			DestroyEventHandler();
		_instance = null;
		//Debug.Log("인스턴스 파괴 {0}", typeof(T));
	}
}

/*
/// @bluegol C#은 제네릭 파라메터에서 상속받는 걸 허용하지 않으므로, 이 부분의 코드 카피는 어쩔 수 없다.
public abstract class ManualSingletonUMB<T> : uLink.MonoBehaviour where T : ManualSingletonUMB<T>
{
	static private T _instance;
	static public T Instance
	{
		get { return _instance; }
		set
		{
			if (_instance != null)
				throw new System.ApplicationException("cannot set Instance twice!");

			_instance = value;
		}
	}

	protected Action DestroyEventHandler;

	void OnDestroy()
	{
		// @bluegol 201405
		// 테스트 등을 위해, 게임 상에 2개가 생길 수도 있는데 이런 경우, Awake에서 바로 disable시킨다.
		// 따라서 이 체크가 필요함
		if (_instance != this)
			return;

		if (DestroyEventHandler != null)
			DestroyEventHandler();
		_instance = null;
		//Logger.Dev.InfoFormat2("인스턴스 파괴 {0}", typeof(T));
	}
}
*/