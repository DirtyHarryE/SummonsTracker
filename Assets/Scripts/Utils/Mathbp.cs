namespace BoilerPlate
{
    public static class Mathbp
    {
        public static int Wrap(int input, int divisor)
        {
            return (input % divisor + divisor) % divisor;
        }
    }
}