using System;

public class ValueObservable<T> // реализация паттерна observer в виде класса-обертки под значение
{
    private Action observers;
    private T _value;
    
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            observers?.Invoke();
        }
    }

    public ValueObservable(T value)
    {
        _value = value;
    }

    public void Subscribe(Action onChange)
    {
        observers += onChange;
    }

    public void UnSubscribe(Action onChange)
    {
        observers -= onChange;
    }

    public void Reset()
    {
        observers = null;
    }
}
