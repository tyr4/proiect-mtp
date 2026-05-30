using System.Collections;

public static class UIState
{
    public static bool IsLocked { get; private set; }

    public static void Lock() => IsLocked = true;
    public static void Unlock() => IsLocked = false;

    public static IEnumerator WaitUntilUnlocked()
    {
        while (IsLocked)
            yield return null;
        
        Lock();
    }
}