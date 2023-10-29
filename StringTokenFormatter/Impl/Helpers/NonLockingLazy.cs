namespace StringTokenFormatter.Impl;

/// <summary>
/// This class mimics the System.Lazy type except it specifically does not have locking implemented
/// </summary>
internal class NonLockingLazy<T>
{
    private Func<T>? creator;

    public NonLockingLazy(Func<T>? creator)
    {
        this.creator = creator;
    }

    public bool IsValueCreated { get; private set; }
    public T? CreatedValue { get; private set; }

    public T? Value
    {
        get
        {
            //Defensive copy
            var cachedcreator = creator;

            if (!IsValueCreated && cachedcreator is { })
            {
                CreatedValue = cachedcreator();
                IsValueCreated = true;

                //Null this out so we don't keep captured values around.
                creator = default;
            }
            return CreatedValue;
        }
    }
}