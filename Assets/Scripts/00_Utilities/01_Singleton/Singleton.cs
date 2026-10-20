public class Singleton<T> where T : class, new()
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            _instance ??= new T();
            return _instance;
        }
    }
}
