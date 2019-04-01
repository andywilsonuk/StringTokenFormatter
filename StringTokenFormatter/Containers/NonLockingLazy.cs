using System;

namespace StringTokenFormatter
{
    /// <summary>
    /// This class mimics the System.Lazy type except it specifically does not have locking implemented
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class NonLockingLazy<T>
    {
        private readonly Func<T> creator;

        public NonLockingLazy(Func<T> creator)
        {
            this.creator = creator;
        }

        public bool IsValueCreated { get; private set; }
        public T CreatedValue { get; private set; }

        public T Value
        {
            get
            {
                if (!IsValueCreated)
                {
                    CreatedValue = creator();
                    IsValueCreated = true;
                }
                return CreatedValue;
            }
        }
    }
}