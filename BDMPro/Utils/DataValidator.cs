using System.Linq;

namespace BDMPro.Utils
{
    public class DataValidator
    {
        public bool HasNonLetterOrDigit(string value)
        {
            return value.Any(ch => !char.IsLetterOrDigit(ch));
        }
        public bool HasDigit(string value)
        {
            return value.Any(ch => char.IsDigit(ch));
        }
        public bool HasLowercase(string value)
        {
            return value.Any(ch => char.IsLower(ch));
        }
        public bool HasUppercase(string value)
        {
            return value.Any(ch => char.IsUpper(ch));
        }
    }
}
