using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> // реализация паттерна singleton в виде класса, который можно унаследовать
{
    private static T _instance;

    public static T Instance // поле привязывает объект со сцены, если он еще не был привязан, иначе просто возвращает его
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<T>();
                if (!_instance)
                {
                    Debug.LogError($"Singleton of type {typeof(T)} not contains in scene");
                    return null;
                }

                _instance.AwakeSingletone();
            }
            
            return _instance;
        }
    }

    public static bool InstanceIsNotNull => _instance;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = GetComponent<T>();
            AwakeSingletone();
        }
        else if (_instance != this)
            Debug.LogError($"Dublicated singleton instance {nameof(T)}", this); // предупреждаем программиста, что нельзя создавать более одного инстанса синглтона на сцене

    }  
    
    protected virtual void AwakeSingletone() { }
}