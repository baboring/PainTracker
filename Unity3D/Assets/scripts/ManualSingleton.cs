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

using System;


public abstract class ManualSingleton<T> where T : class, new()
{
	static protected T _instance;
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

	static public T Instantiate()
	{
		Instance = new T();
		return Instance;
	}

	static public T SetInstance(T ins)
	{
		Instance = ins;
		return ins;
	}

	static public void Destroy()
	{
		_instance = null;
	}

	public ManualSingleton()
	{
		if (_instance != null)
			throw new System.ApplicationException("cannot set Instance twice!");
	}

}


public abstract class AutoSingleton<T> : ManualSingleton<T> where T : class, new()
{
	static new public T Instance
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

	public AutoSingleton()
	{
		if (_instance != null)
			throw new System.ApplicationException("cannot set Instance twice!");
	}
}


public abstract class ManualSingletonMB<T> : UnityEngine.MonoBehaviour where T : ManualSingletonMB<T>
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
		//Debug.Log("인스턴스 파괴 {0}", typeof(T));
	}
}