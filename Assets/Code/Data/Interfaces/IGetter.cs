namespace Code.Data
{
    public interface IGetter
    {
        void Get<T>(out T component) where T : class;

    }
}