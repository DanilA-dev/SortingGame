using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace D_Dev.ServerTimerSystem
{
    [Serializable]
    public class HttpDateServerTimeProvider : IServerTimeProvider
    {
        #region Fields

        [SerializeField] private List<string> _urls = new()
        {
            "https://www.google.com",
            "https://www.yandex.com"
        };
        [SerializeField] private int _timeoutSeconds = 5;

        #endregion

        #region IServerTimeProvider

        public async Awaitable<DateTime?> RequestUtcTimeAsync()
        {
            foreach (string url in _urls)
            {
                if (string.IsNullOrEmpty(url))
                    continue;

                using UnityWebRequest request = UnityWebRequest.Head(url);
                request.timeout = _timeoutSeconds;

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                    continue;

                if (TryParseDate(request, out DateTime utcTime))
                    return utcTime;
            }

            return null;
        }

        #endregion

        #region Private

        private static bool TryParseDate(UnityWebRequest request, out DateTime utcTime)
        {
            string date = request.GetResponseHeader("Date");

            if (string.IsNullOrEmpty(date) || !DateTime.TryParseExact(date, "r", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out utcTime))
            {
                utcTime = default;
                return false;
            }

            string age = request.GetResponseHeader("Age");
            if (!string.IsNullOrEmpty(age) && int.TryParse(age, out int ageSeconds) && ageSeconds > 0)
                utcTime = utcTime.AddSeconds(ageSeconds);

            return true;
        }

        #endregion
    }
}
