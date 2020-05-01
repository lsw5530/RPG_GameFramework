using UnityEngine;

public class GlobalProto {
        public string AssetBundleServerUrl;
        public string Address;

        public string GetUrl() {
            string url = this.AssetBundleServerUrl;
#if UNITY_ANDROID
			url += "Android/";
#elif UNITY_IOS
			url += "IOS/";
#elif UNITY_WEBGL
			url += "WebGL/";
#elif UNITY_STANDALONE_OSX
			url += "MacOS/";
#else
            url += "PC/";
#endif
            Debug.LogError(url);
            return url;
        }
    }
