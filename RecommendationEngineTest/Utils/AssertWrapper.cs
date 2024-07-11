using NUnit.Framework;

public static class AssertWrapper
{
    public static void AssertExpectedMatchesActual<T>(T expected, T actual, string message = "")
    {
        Assert.That(actual, Is.EqualTo(expected), message);
    }
}
