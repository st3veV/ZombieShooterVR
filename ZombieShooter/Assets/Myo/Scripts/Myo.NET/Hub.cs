using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Thalmic.Myo
{
    public class Hub : IDisposable
    {
        private static readonly DateTime TIMESTAMP_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        private bool _disposed = false;
        private IntPtr _handle;

        private Thread _eventThread;
        private bool _eventThreadShutdown = false;

        private Dictionary<IntPtr, Myo> _myos = new Dictionary<IntPtr, Myo>();

        public Hub(string applicationIdentifier, EventHandler<MyoEventArgs> OnPaired)
        {
            if (OnPaired != null) {
                Paired += OnPaired;
            }

#if !UNITY_ANDROID || UNITY_EDITOR
            if (libmyo.init_hub(out _handle, applicationIdentifier, IntPtr.Zero) != libmyo.Result.Success)
            {
                throw new InvalidOperationException("Unable to initialize Hub.");
            }

            // spawn the event thread
            StartEventThread();
#endif
        }

        // Deterministic destructor
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
#if !UNITY_ANDROID || UNITY_EDITOR
                StopEventThread();

                if (disposing)
                {
                    // free IDisposable managed objects (none right now)
                }

                // free unmanaged objects
                libmyo.shutdown_hub(_handle, IntPtr.Zero);
#endif
                _disposed = true;
            }
        }

        // Finalizer (non-deterministic)
        ~Hub()
        {
            Dispose(false);
        }

        public void SetLockingPolicy(LockingPolicy lockingPolicy)
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            libmyo.set_locking_policy(_handle, (libmyo.LockingPolicy)lockingPolicy, IntPtr.Zero);
#endif
        }

        public event EventHandler<MyoEventArgs> Paired;

        internal void StartEventThread()
        {
            _eventThreadShutdown = false;
            _eventThread = new Thread(new ThreadStart(EventThreadFn));
            _eventThread.Start();
        }

        internal void StopEventThread()
        {
            _eventThreadShutdown = true;
            if (_eventThread != null)
            {
                _eventThread.Join();
            }
        }
        
        private void EventThreadFn()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            while (!_eventThreadShutdown)
            {
                GCHandle gch = GCHandle.Alloc(this);

                libmyo.run(_handle, 1000, (libmyo.Handler)HandleEvent, (IntPtr)gch, IntPtr.Zero);
            }
#endif
        }

        private static libmyo.HandlerResult HandleEvent(IntPtr userData, IntPtr evt)
        {
            GCHandle handle = (GCHandle)userData;
            Hub self = (Hub)handle.Target;
#if !UNITY_ANDROID || UNITY_EDITOR
            var type = libmyo.event_get_type(evt);
            var timestamp = TIMESTAMP_EPOCH.AddMilliseconds(libmyo.event_get_timestamp(evt) / 1000);
            var myoHandle = libmyo.event_get_myo(evt);

            switch (type)
            {
                case libmyo.EventType.Paired:
                    var myo = new Myo(self, myoHandle);
                    self._myos.Add(myoHandle, myo);
                    if (self.Paired != null)
                    {
                        self.Paired(self, new MyoEventArgs(myo, DateTime.Now));
                    }
                    break;

                default:
                    Debug.Assert(self._myos[myoHandle] != null);
                    self._myos[myoHandle].HandleEvent(type, timestamp, evt);
                    break;
            }
#endif
            return libmyo.HandlerResult.Continue;
        }
    }

    public enum LockingPolicy
    {
        None,
        Standard
    }
}
