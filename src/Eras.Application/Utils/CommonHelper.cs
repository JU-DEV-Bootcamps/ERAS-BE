namespace Eras.Application.Utils
{
    public static class CommonHelper
    {
        public static bool ValidateZeroNumber (this int number) {  return number == 0; }
        public static bool ValidateZeroNumber(this decimal number) { return number == 0; }
    }
}
