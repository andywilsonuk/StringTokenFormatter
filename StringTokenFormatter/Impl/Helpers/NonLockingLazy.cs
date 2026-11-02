namespace StringTokenFormatter.Impl;

/// <summary>
/// This class mimics the System.Lazy type except it specifically does not have locking implemented.
/// </summary>
internal class NonLockingLazy(Func<object> creator)
{
    private Func<object>? creator = creator;

    public bool IsValueCreated { get; private set; }
    public object? CreatedValue { get; private set; }

    public object? Value
    {
        get
        {
            //Defensive copy
            var cachedcreator = creator;

            if (!IsValueCreated && cachedcreator is { })
            {
                CreatedValue = cachedcreator();
                IsValueCreated = true;

                //Null this out so we don't keep captured values around
                creator = null;
            }
            return CreatedValue;
        }
    }
}