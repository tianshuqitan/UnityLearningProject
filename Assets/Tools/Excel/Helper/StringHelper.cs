namespace Tools.Excel
{
    public static class StringHelper
    {
        public static string FirstToUpper(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            
            var a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}
