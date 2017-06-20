namespace Destiny.Utility
{
    public sealed class Doublet<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }

        public Doublet(T1 first, T2 second)
        {
            this.First = first;
            this.Second = second;
        }
    }
}
