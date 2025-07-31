using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    public int almond { get; private set; }
    public int sugar  { get; private set; }
    public int egg    { get; private set; }
    public UnityEvent OnResourceChanged;

    public void AddResource(string type, int amount)
    {
        switch (type)
        {
            case "Almond": almond += amount; break;
            case "Sugar": sugar += amount; break;
            case "Egg": egg += amount; break;
        }
        OnResourceChanged?.Invoke();
    }

    public bool ConsumeResource(string type, int amount)
    {
        switch(type)
        {
            case "Almond":
                if (almond < amount) { almond = 0; return false; }
                almond -= amount; return true;
            case "Sugar":
                if (sugar < amount) { sugar = 0; return false; }
                sugar -= amount; return true;
            case "Egg":
                if (egg < amount) { egg = 0; return false; }
                egg -= amount; return true;
        }
        return false;
    }
}
