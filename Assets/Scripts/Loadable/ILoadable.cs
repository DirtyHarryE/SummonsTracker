/*
    The Universal Loader automatically searches the project and adds scriptable objects that implement "ILoadable" to a list. 
    The problem this was meant to solve was to be able to access Scriptableobjects easily within code. One could have serialized
    references wherever they are needed, but those can be changed and if code needs to access the SAME scriptable object, this
    can be a problem. So the idea came to having a static reference to the scriptable object within it, but if the scriptable
    object isn't loaded, its Awake() method is never called. Having a Universal Loader Scriptable object that is referenced
    in the scene that automatically calls a method on that scriptable object that implements ILoadable will allow you to make
    that reference quickly and easily.
*/
namespace SummonsTracker.Loading
{
    public interface ILoadable
    {
        void Load();
        int Priority { get; }
    }
    public interface ILoadable<T> : ILoadable
        where T : UnityEngine.Object, ILoadable
    {
    }
}
