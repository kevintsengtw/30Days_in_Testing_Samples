namespace Day01.Core;

/// <summary>
/// class Counter -用於示範 FIRST 原則中的獨立性和可重複性 
/// </summary>
public class Counter
{
    private int _value = 0;

    /// <summary>
    /// 目前的計數值
    /// </summary>
    public int Value => this._value;

    /// <summary>
    /// 增加計數值
    /// </summary>
    public void Increment()
    {
        this._value++;
    }

    /// <summary>
    /// 減少計數值
    /// </summary>
    public void Decrement()
    {
        this._value--;
    }

    /// <summary>
    /// 重設計數值為 0
    /// </summary>
    public void Reset()
    {
        this._value = 0;
    }

    /// <summary>
    /// 設定特定的計數值
    /// </summary>
    /// <param name="value">要設定的值</param>
    public void SetValue(int value)
    {
        this._value = value;
    }
}