using Mirror.Cloud.ListServerService;
using System;
using UnityEngine.Events;

namespace Mirror.Cloud {
    [Serializable]
    public class ServerListEvent : UnityEvent<ServerCollectionJson> {
    }

    [Serializable]
    public class MatchFoundEvent : UnityEvent<ServerJson> {
    }
}
