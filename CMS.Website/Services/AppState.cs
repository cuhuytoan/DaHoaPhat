using CMS.Data.ModelEntity;
using System;
using System.Collections.Generic;

namespace CMS.Website.Services
{
    public static class AppState
    {
        public static event Action OnChange;

        public static List<SpUserNotifySearchResult> lstUserNoti { get; set; } = new List<SpUserNotifySearchResult>();

        public static void UpdateNoti() => NotifyStateChanged();

        private static void NotifyStateChanged() => OnChange?.Invoke();
    }
}
