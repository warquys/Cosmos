namespace Cosmos.Kernel.Core.CPU;

/// <summary>Public process-runtime integration point. IRQContext and InterruptManager remain internal.</summary>
public static class ProcessTrapApi
{
    /// <summary>Observer invoked for the timer, syscall and user-fault vectors.</summary>
    public delegate void Observer(ref ProcessTrapContext context);

    private static Observer? s_observer;

    /// <summary>Sets or clears the process observer.</summary>
    public static void SetObserver(Observer? observer)
    {
        s_observer = observer;
        InterruptManager.SetProcessTrapObserver(observer is null ? null : Dispatch);
    }

    private static void Dispatch(ref ProcessTrapContext context)
    {
        s_observer?.Invoke(ref context);
    }
}