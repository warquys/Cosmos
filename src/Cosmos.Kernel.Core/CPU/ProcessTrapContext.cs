namespace Cosmos.Kernel.Core.CPU;

/// <summary>
/// Public, stable process-facing view of an interrupt frame. The architecture-specific
/// IRQContext remains private to Cosmos; process runtimes must use this façade.
/// </summary>
public struct ProcessTrapContext
{
    /// <summary>Interrupt or exception vector.</summary>
    public ulong Vector;
    /// <summary>Interrupted instruction pointer.</summary>
    public ulong InstructionPointer;
    /// <summary>Interrupted stack pointer.</summary>
    public ulong StackPointer;
    /// <summary>Interrupted code segment, when available.</summary>
    public ulong CodeSegment;
    /// <summary>Interrupted stack segment, when available.</summary>
    public ulong StackSegment;
    /// <summary>Saved processor flags.</summary>
    public ulong Flags;
    /// <summary>Faulting linear address for page faults.</summary>
    public ulong FaultAddress;
    /// <summary>Syscall number carried by the architecture syscall convention.</summary>
    public ulong SyscallNumber;
    /// <summary>Set this flag when the process scheduler should run on interrupt exit.</summary>
    public bool RescheduleRequested;

    internal static ProcessTrapContext From(ref IRQContext context)
    {
        return new ProcessTrapContext
        {
            Vector = context.interrupt,
#if ARCH_ARM64
            InstructionPointer = context.elr,
            StackPointer = context.sp,
            Flags = context.spsr,
            SyscallNumber = context.x0,
#else
            InstructionPointer = context.rip,
            StackPointer = context.rsp,
            Flags = context.rflags,
            SyscallNumber = context.rax,
#endif
            CodeSegment = 0,
            StackSegment = 0,
            FaultAddress = context.fault_address,
        };
    }

    internal readonly void ApplyTo(ref IRQContext context)
    {
    #if ARCH_ARM64
        context.elr = InstructionPointer;
        context.sp = StackPointer;
        context.spsr = Flags;
        context.x0 = SyscallNumber;
    #else
        context.rip = InstructionPointer;
        context.rsp = StackPointer;
        context.rflags = Flags;
        context.rax = SyscallNumber;
    #endif
    }
}