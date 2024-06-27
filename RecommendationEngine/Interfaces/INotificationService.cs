using System.Collections.Generic;

namespace RecommendationEngine.Interfaces
{
    public interface I

    {
        void SendRecommendationNotification(string message);
        void SendNewItemNotification(string message);
        void SendAvailabilityStatusNotification(string message);
        void SendNotification(string message);
    }
}
