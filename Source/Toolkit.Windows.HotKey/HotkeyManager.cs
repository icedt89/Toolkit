namespace JanHafner.Toolkit.Windows.HotKey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    public abstract class HotKeyManager : IDisposable
    {
        [NotNull]
        private readonly IDictionary<GlobalHotKey, Action> registeredHotKeys;

        protected Boolean isDisposed;

        protected HotKeyManager()
        {
            this.registeredHotKeys = new Dictionary<GlobalHotKey, Action>();
        }
        
        public Boolean HasRegisteredHotKeys
        {
            get { return this.registeredHotKeys.Any(); }
        }

        public Boolean HandleHotKey(HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode)
        {
            this.CheckDisposed();

            var registeredHotKey = this.registeredHotKeys.SingleOrDefault(hotKey => hotKey.Key.Equals(hotKeyModifier, virtualKeyCode));
            if (registeredHotKey.Key != null)
            {
                registeredHotKey.Value();

                return true;
            }

            return false;
        }

        protected static Boolean ProbeHotKey(HotKeyModifier hotKeyModifier, UInt32 virtualkeyCode)
        {
            return GlobalHotKey.Probe(hotKeyModifier, virtualkeyCode);
        }

        protected Int32 Register(IntPtr windowHandle, HotKeyModifier hotKeyModifier, UInt32 virtualkeyCode, [NotNull] Action action)
        {
            this.CheckDisposed();

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var hotKey = GlobalHotKey.Register(windowHandle, hotKeyModifier, virtualkeyCode);
            this.registeredHotKeys.Add(hotKey, action);

            return hotKey.Id;
        }

        [CanBeNull]
        protected Int32? FindHotKeyId(HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode)
        {
            this.CheckDisposed();

            return this.registeredHotKeys.SingleOrDefault(hotKey => hotKey.Key.HotKeyModifier == hotKeyModifier && hotKey.Key.VirtualKeyCode == virtualKeyCode).Key?.Id;
        }

        public void Unregister(HotKeyModifier hotKeyModifier, UInt32 virtualKeyCode)
        {
            this.CheckDisposed();

            var hotKeyId = this.FindHotKeyId(hotKeyModifier, virtualKeyCode);
            if (hotKeyId.HasValue)
            {
                this.Unregister(hotKeyId.Value);
            }
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(null);
            }
        }

        public void Unregister(Int32 hotKeyId)
        {
            this.CheckDisposed();

            var registeredHotKey = this.registeredHotKeys.SingleOrDefault(hotKey => hotKey.Key.Id == hotKeyId);
            if (registeredHotKey.Key != null)
            {
                registeredHotKey.Key.Dispose();

                this.registeredHotKeys.Remove(registeredHotKey);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~HotKeyManager()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var hotKey in this.registeredHotKeys.Keys.ToList())
                {
                    hotKey.Dispose();

                    this.registeredHotKeys.Remove(hotKey);
                }
            }

            this.isDisposed = true;
        }
    }
}
