namespace TUnit.Demo.Core;

/// <summary>
/// 基本計算器類別，提供數學運算功能
/// </summary>
public class Calculator
{
    /// <summary>
    /// 加法運算
    /// </summary>
    public int Add(int a, int b)
    {
        return a + b;
    }

    /// <summary>
    /// 減法運算
    /// </summary>
    public int Subtract(int a, int b)
    {
        return a - b;
    }

    /// <summary>
    /// 乘法運算
    /// </summary>
    public int Multiply(int a, int b)
    {
        return a * b;
    }

    /// <summary>
    /// 除法運算
    /// </summary>
    public double Divide(int dividend, int divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException("除數不能為零");
        }

        return (double)dividend / divisor;
    }

    /// <summary>
    /// 判斷是否為正數
    /// </summary>
    public bool IsPositive(int number)
    {
        return number > 0;
    }
}