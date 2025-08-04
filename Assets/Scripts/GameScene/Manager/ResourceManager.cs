using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    public int almond { get; private set; }
    public int sugar  { get; private set; }
    public int egg    { get; private set; }
    public UnityEvent OnResourceChanged;

    public void AddResource(int a, int b, int c)
    {
        almond += a;
        sugar += b;
        egg += c;
        OnResourceChanged?.Invoke();
    }
}
