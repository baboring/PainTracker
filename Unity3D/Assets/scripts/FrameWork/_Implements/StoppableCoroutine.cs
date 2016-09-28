using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Wrapper around coroutines that allows to start them without using
/// their lexical name while still being able to stop them.
/// </summary>
public class StoppableCoroutine : IEnumerator
{
    // Wrapped generator method
    protected IEnumerator generator;

	public bool IsExit { get; protected set; }

    public StoppableCoroutine(IEnumerator generator)
    {
        this.generator = generator;
    }

    // Stop the coroutine form being called again
    public void Stop()
    {
        generator = null;
		IsExit = true;
	}

    // IEnumerator.MoveNext
    public bool MoveNext()
    {
        if (generator != null)
        {
			if (generator.MoveNext())
				return true;
			IsExit = true;
			return false;
		}
        else {
            return false;
        }
    }

    // IEnumerator.Reset
    public void Reset()
    {
        if (generator != null)
        {
			if(IsExit) {
				IsExit = false;
				generator.Reset();
			}
		}
    }

    // IEnumerator.Current
    public object Current
    {
        get
        {
            if (generator != null)
            {
                return generator.Current;
            }
            else {
                throw new InvalidOperationException();
            }
        }
    }
}

/// <summary>
/// Syntactic sugar to create stoppable coroutines.
/// </summary>
public static class StoppableCoroutineExtensions
{
    public static StoppableCoroutine MakeStoppable(this IEnumerator generator)
    {
        return new StoppableCoroutine(generator);
    }
	public static StoppableCoroutine MakeStoppable(this IEnumerable generator) {
		return new StoppableCoroutine(generator.GetEnumerator());
	}
}

/// <summary>
/// Example of using the StoppableCoroutine wrapper.
/// </summary>
/*
public class Example : MonoBehaviour
{
    // MonoBehaviour.Start
    protected IEnumerator Start()
    {
        // Create the stoppable coroutine and store it
        var routine = MyCoroutine().MakeStoppable();
        // Pass the wrapper to StartCoroutine
        StartCoroutine(routine);

        // Do stuff...
        yield return new WaitForSeconds(5);

        // Abort the coroutine by calling Stop() on the wrapper
        routine.Stop();
    }

    // Coroutine that runs indefinitely and can only
    // be stopped from the outside
    protected IEnumerator MyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("running...");
        }
    }
}
*/
