using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

namespace TLab.Android.WebView
{
	public class TLabWebViewSample : MonoBehaviour
	{
		[SerializeField] private TLabWebView m_webView;
		[SerializeField] private SearchBar m_seachBar;

		public void StartWebView()
		{
			m_webView.StartWebView();
		}

		void Start()
		{
			StartWebView();
            LoggerUtility.LogMethodName(additionalInfo: "TLabWebViewSample: Start");
            scheduleJavaScriptInjectionHandling();
			m_seachBar.gameObject.SetActive(false);
		}

		void Update()
		{
#if UNITY_ANDROID
			m_webView.UpdateFrame();
#endif
		}

        #region JavaScript Injection

        public void scheduleJavaScriptInjectionHandling()
        {
            Invoke("handleJavaScriptInjection", 5); // Invoke the method after a delay of 5 seconds
        }

        public void handleJavaScriptInjection()
        {
            if (!FileUtility.CheckFileContentsNotEmpty(ElevateConfig.Instance.fileContent))
            {
                LoggerUtility.LogMethodName(additionalInfo: "file contents are empty");
            }
            else
            {
                LoggerUtility.LogMethodName(additionalInfo: "file contents are not empty");
                foreach (string line in ElevateConfig.Instance.fileContent)
                {
                    LoggerUtility.LogMethodName(additionalInfo: line);
                }
                InjectJavaScriptTo(type: ElevateConfig.Instance.testType, fileContent: ElevateConfig.Instance.fileContent);
            }
        }

        public void InjectJavaScriptTo(string type, string[] fileContent)
        {
            LoggerUtility.LogMethodName();
            string buttonId = WebUtility.GetHiddenButtonWebView(type); // Get the button ID based on the type
            if (string.IsNullOrEmpty(buttonId))
            {
                LoggerUtility.LogMethodName(additionalInfo: "Button ID is not defined for the given type: " + type);
                return;
            }

            // Convert the array of strings into a single JSON array string
            string arrayString = JsonConvert.SerializeObject(fileContent);

            // Construct the JavaScript code to set the value of the hidden button and trigger its click event
            string javaScriptString =
                $"var button = document.getElementById('{buttonId}');" +
                $"button.value = JSON.stringify({{ fileContent: {arrayString} }});" +
                "button.click();";

            // Load the JavaScript code into the WebView
            m_webView.EvaluateJS(javaScriptString);
        }

        #endregion JavaScript Injection

    }
}

