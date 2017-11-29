using System;

namespace StringTokenFormatter
{
    public interface IMatchedToken
    {
        string Original { get; }
        string Token { get; }
    }
}