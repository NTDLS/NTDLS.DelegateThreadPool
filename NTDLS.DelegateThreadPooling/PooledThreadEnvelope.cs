﻿using System.Diagnostics;

namespace NTDLS.DelegateThreadPooling
{
    /// <summary>
    /// Worker thread envelope used for managing and interrogating thread states.
    /// </summary>
    public class PooledThreadEnvelope
    {
        /// <summary>
        /// State of thread.
        /// </summary>
        public enum PooledThreadState
        {
            /// <summary>
            /// The thread is currently waiting on an item to deque.
            /// </summary>
            Waiting,
            /// <summary>
            /// Thread is executing. 
            /// </summary>
            Executing
        }

        /// <summary>
        /// Managed thread which is used to execute workloads.
        /// </summary>
        public Thread ManagedThread { get; private set; }

        /// <summary>
        /// Native process thread which is used to execute workloads.
        /// </summary>
        public ProcessThread? NativeThread { get; internal set; }

        /// <summary>
        /// Wait event that is used to signal the worker thread to deque items.
        /// </summary>
        public AutoResetEvent WaitEvent { get; private set; } = new(true);

        /// <summary>
        /// Current execution state of the thread.
        /// </summary>
        public PooledThreadState State { get; set; }

        internal void Signal() => WaitEvent.Set();
        internal bool Wait() => WaitEvent.WaitOne();

        internal void Join() => ManagedThread.Join();

        internal PooledThreadEnvelope(ParameterizedThreadStart proc)
        {
            State = PooledThreadState.Waiting;
            ManagedThread = new Thread(proc);
            ManagedThread.Start(this);
        }
    }
}
