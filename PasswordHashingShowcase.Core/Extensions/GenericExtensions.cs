namespace PasswordHashingShowcase.Core.Extensions
{
    internal static class GenericExtensions
    {
        public static TResult Then<TInput, TResult>(this TInput input, Func<TInput, TResult> @do) => @do(input);
    }
}
