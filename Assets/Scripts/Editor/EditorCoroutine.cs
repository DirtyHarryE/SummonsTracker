using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EditorCoroutine
{

    public static EditorCoroutine StartCoroutine(IEnumerator routine, Action<Exception> onFail = null)
    {
        var stack = new Stack<IEnumerator>();
        stack.Push(routine);
        var seconds = -1f;
        var dateTime = DateTime.Now;
        IEnumerator r = null;
        int counter = 0;


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

                var immediate = false;
                var next = true;
                do
                {
                    immediate = false;
                    if (r == null)
                    {
                        r = stack.Pop();
                    }

                    next = r.MoveNext();
                    if (!next)
                    {
                        if (stack.Any())
                        {
                            r = stack.Pop();
                            next = r.MoveNext();
                        }
                    }
                    if (!next)
                    {
                        EditorApplication.update -= Update;
                        immediate = false;
                    }
                    else
                    {
                        var current = r.Current;
                        if (current is IEnumerator subroutine)
                        {
                            stack.Push(r);
                            r = subroutine;
                            immediate = true;
                        }
                        else if (current is WaitForSeconds waitForSeconds)
                        {
                            var field = waitForSeconds.GetType().GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic);
                            if (field != null)
                            {
                                if (field.GetValue(waitForSeconds) is float f)
                                {
                                    seconds = f;
                                    dateTime = DateTime.Now;
                                }

                            }
                            immediate = false;
                        }
                        else if (current == null || current is YieldInstruction)
                        {
                            immediate = false;
                        }
                        else
                        {
                            immediate = true;
                        }
                    }
                } while (immediate && next);

            }
            catch (Exception e)
            {
                Debug.LogError($"An error occured! Halting coroutine!\n{e}");
                EditorApplication.update -= Update;
                onFail?.Invoke(e);
                throw;
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
