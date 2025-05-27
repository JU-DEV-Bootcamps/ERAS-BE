namespace Eras.Application.Utils
{
    public static class CommonHelper
    {
        public static bool ValidateZeroNumber (this int Number) {  return Number == 0; }
        public static bool ValidateZeroNumber(this decimal Number) { return Number == 0; }
    }
}
