namespace sudo
{
    public static class StringHelpers
    {
        public static string ToArgumentString(this string[] argumentArray)
        {
            var argumentString = "";
            for (var i = 1; i < argumentArray.Length; i++)
            {
                if (i != 1 && i != argumentArray.Length - 1)
                {
                    argumentString += " ";
                }
                argumentString += argumentArray[i];
            }
            return argumentString;
        }

    }
}
