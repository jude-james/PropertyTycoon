public static class Maths
{
    /// <summary>
    /// Calculates modulus, C# '%' is only the remainder and does not work for negative numbers 
    /// </summary>
    /// <param name="x"> the dividend </param>
    /// <param name="m"> the divisor </param>
    /// <returns></returns>
    public static int Mod(int x, int m)
    {
        return (x%m + m)%m;
    }
}