using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EditorCoroutine
{

    public static EditorCoroutine StartCoroutine(IEnumerator routine, Action<Exception> onFail = null)
    {
        var seconds = -1f;
        var dateTime = DateTime.Now;
        EditorApplication.update += Update;

        void Update()
        {
            try
            {
                if (seconds > 0f)
                {
                    var now = DateTime.Now;
                    if (now.Subtract(dateTime).TotalSeconds >= seconds)
                    {
                        seconds = -1;
                    }
                    else
                    {
                        return;
                    }
                }
                var loop = true;
                object current = null;
                while (loop)
                {
                    loop = routine.MoveNext();
                    current = routine.Current;

                    if (!loop)
                    {
                        EditorApplication.update -= Update;
                    }
                    if (current is WaitForSeconds waitForSeconds)
                    {
                        var field = waitForSeconds.GetType().GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (field != null)
                        {
                            if( field.GetValue(waitForSeconds) is float f)
                            {
                                seconds = f;
                                dateTime = DateTime.Now;
                            }
                        }
                    }
                    if (current == null || current is YieldInstruction)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occured! Halting coroutine!\n\n{e}");
                EditorApplication.update -= Update;
                onFail?.Invoke(e);
            }
        }

        return new EditorCoroutine(() => EditorApplication.update -= Update);
    }
    private EditorCoroutine(Action cancel)
    {
        _cancel = cancel;
    }
    public void Stop()
    {
        _cancel?.Invoke();
    }
    private Action _cancel;
}
