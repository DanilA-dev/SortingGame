namespace D_Dev.Base
{
    public static class TimeFormatter
    {
        #region Public

        public static string Format(long totalSeconds)
        {
            if (totalSeconds < 0)
                totalSeconds = 0;

            long hours = totalSeconds / 3600;
            long minutes = totalSeconds / 60 % 60;
            long seconds = totalSeconds % 60;

            return hours > 0
                ? $"{hours}:{minutes:00}:{seconds:00}"
                : $"{minutes:00}:{seconds:00}";
        }

        #endregion
    }
}
