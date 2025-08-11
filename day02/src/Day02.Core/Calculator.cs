namespace Day02.Core;

/// <summary>
/// class Calculator
/// </summary>
public class Calculator
{
    /// <summary>
    /// 將兩個整數相加並返回結果。
    /// </summary>
    /// <param name="a">要相加的第一個整數。</param>
    /// <param name="b">要相加的第二個整數。</param>
    /// <returns><paramref name="a"/> 和 <paramref name="b"/> 的和。</returns>
    public int Add(int a, int b)
    {
        return a + b;
    }

    /// <summary>
    /// 將兩個整數相減並返回結果。
    /// </summary>
    /// <param name="a">要相減的第一個整數。</param>
    /// <param name="b">要相減的第二個整數。</param>
    /// <returns><paramref name="a"/> 和 <paramref name="b"/> 的差。</returns>
    public int Subtract(int a, int b)
    {
        return a - b;
    }

    /// <summary>
    /// 將兩個整數相乘並返回結果。
    /// </summary>
    /// <param name="a">要相乘的第一個整數。</param>
    /// <param name="b">要相乘的第二個整數。</param>
    /// <returns><paramref name="a"/> 和 <paramref name="b"/> 的積。</returns>
    public int Multiply(int a, int b)
    {
        return a * b;
    }

    /// <summary>
    /// 將兩個整數相除並返回結果。
    /// </summary>
    /// <param name="dividend">被除數。</param>
    /// <param name="divisor">除數。</param>
    /// <returns>兩個整數相除的結果。</returns>
    public double Divide(int dividend, int divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException("除數不能為零");
        }

        return (double)dividend / divisor;
    }

    /// <summary>
    /// 判斷指定的整數是否為偶數。
    /// </summary>
    /// <param name="number">要判斷的整數。</param>
    /// <returns>如果 <paramref name="number"/> 是偶數，則為 <c>true</c>；否則為 <c>false</c>。</returns>
    public bool IsEven(int number)
    {
        return number % 2 == 0;
    }
}