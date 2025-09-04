namespace Calculator.Core;

/// <summary>
/// class Calculator
/// </summary>
/// <remarks>
/// 基本計算器功能
/// </remarks>
public class Calculator
{
    /// <summary>
    /// 加法運算
    /// </summary>
    /// <param name="a">第一個數字</param>
    /// <param name="b">第二個數字</param>
    /// <returns>兩數相加的結果</returns>
    public int Add(int a, int b)
    {
        return a + b;
    }

    /// <summary>
    /// 減法運算
    /// </summary>
    /// <param name="a">被減數</param>
    /// <param name="b">減數</param>
    /// <returns>兩數相減的結果</returns>
    public int Subtract(int a, int b)
    {
        return a - b;
    }

    /// <summary>
    /// 乘法運算
    /// </summary>
    /// <param name="a">第一個數字</param>
    /// <param name="b">第二個數字</param>
    /// <returns>兩數相乘的結果</returns>
    public int Multiply(int a, int b)
    {
        return a * b;
    }

    /// <summary>
    /// 除法運算
    /// </summary>
    /// <param name="a">被除數</param>
    /// <param name="b">除數</param>
    /// <returns>兩數相除的結果</returns>
    /// <exception cref="DivideByZeroException">除數為零時拋出例外</exception>
    public double Divide(int a, int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("除數不能為零");
        }

        return (double)a / b;
    }

    /// <summary>
    /// 判斷數字是否為奇數
    /// </summary>
    /// <param name="number">要檢查的數字</param>
    /// <returns>如果是奇數回傳 true，否則回傳 false</returns>
    public bool IsOdd(int number)
    {
        return number % 2 != 0;
    }

    /// <summary>
    /// 計算平方
    /// </summary>
    /// <param name="number">要計算平方的數字</param>
    /// <returns>數字的平方值</returns>
    public int Square(int number)
    {
        return number * number;
    }

    /// <summary>
    /// 非同步計算斐波那契數列
    /// </summary>
    /// <param name="n">序列位置</param>
    /// <returns>斐波那契數列的值</returns>
    public async Task<long> CalculateFibonacciAsync(int n)
    {
        if (n < 0)
        {
            throw new ArgumentException("位置不能為負數", nameof(n));
        }

        if (n is 0 or 1)
        {
            return n;
        }

        // 模擬非同步計算
        await Task.Delay(10);

        long a = 0, b = 1;
        for (var i = 2; i <= n; i++)
        {
            var temp = a + b;
            a = b;
            b = temp;
        }

        return b;
    }
}