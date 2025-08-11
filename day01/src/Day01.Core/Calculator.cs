namespace Day01.Core;

/// <summary>
/// class Calculator - 展示基本數學運算，用於示範 FIRST 原則中的測試案例
/// </summary>
public class Calculator
{
    /// <summary>
    /// 加法運算
    /// </summary>
    /// <param name="a">被加數</param>
    /// <param name="b">加數</param>
    /// <returns>兩數之和</returns>
    public int Add(int a, int b)
    {
        return a + b;
    }

    /// <summary>
    /// 除法運算
    /// </summary>
    /// <param name="dividend">被除數</param>
    /// <param name="divisor">除數</param>
    /// <returns>除法結果</returns>
    /// <exception cref="DivideByZeroException">當除數為0時拋出</exception>
    public decimal Divide(decimal dividend, decimal divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException("除數不能為零");
        }

        return dividend / divisor;
    }

    /// <summary>
    /// 乘法運算
    /// </summary>
    /// <param name="a">被乘數</param>
    /// <param name="b">乘數</param>
    /// <returns>兩數之積</returns>
    public int Multiply(int a, int b)
    {
        return a * b;
    }
}