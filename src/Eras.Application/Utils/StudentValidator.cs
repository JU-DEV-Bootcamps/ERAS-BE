using System.Text.RegularExpressions;
using Eras.Application.DTOs;

namespace Eras.Application.Utils
{
    public static class StudentValidator
    {
        public static bool isStudentValid(StudentImportDto StudentDto)
        {
            return isStudentNameValid(StudentDto.Name) && isStudentEmailValid(StudentDto.Email);
        }

        private static bool isStudentEmailValid(string Email)
        {
            string regexEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(Email, regexEmail); 

        }

        private static bool isStudentNameValid(string Name)
        {
            string regexName = @"^\p{L}+(?:[ '-]\p{L}+)*$";

            return Regex.IsMatch(Name, regexName);
        }
    }
}