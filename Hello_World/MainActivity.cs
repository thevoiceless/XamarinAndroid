using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Hello_World
{
	// This is a custom attribute used to convey information at runtime (inserted into the manifest)
	[Activity (Label = "Hello World", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private int count = 1;
		private Toast t;

		protected override void OnCreate(Bundle bundle)
		{
			// base = super
			base.OnCreate(bundle);

			SetContentView (Resource.Layout.Main);
			Button countButton = FindViewById<Button> (Resource.Id.countButton);
			Button newActivityButton = FindViewById<Button> (Resource.Id.newActivityButton);
			Button networkRequestButton = FindViewById<Button> (Resource.Id.networkRequestButton);

			// Could also use
			//   button.Click += (sender, e) => { ... };
			// or
			//   button.Click += delegate(object sender, EventArgs e) { ... };
			countButton.Click += delegate {
//				countButton.Text = string.Format(Resources.GetString(Resource.String.number_of_clicks), count++);
				countButton.SetText(string.Format(Resources.GetString(Resource.String.number_of_clicks), count++), TextView.BufferType.Normal);
			};

			newActivityButton.Click += delegate {
				var secondIntent = new Intent(this, typeof(SecondActivity));
				secondIntent.PutExtra(Resources.GetString(Resource.String.intent_key_1), Resources.GetString(Resource.String.second_activity_text));
				StartActivity(secondIntent);
			};

			/*
			 * Multiple ways to send GET request
			 */


			String json = "{\"key\":\"value\"}";
			var url = new Uri(String.Format("http://validate.jsontest.com/?json={0}", json));

			// Method 1 - WebClient
			networkRequestButton.Click += delegate {
				var webClient = new WebClient ();
				webClient.DownloadStringCompleted += (s, e) => {
					var text = e.Result;
					PrintResult(1, text);
				};
				webClient.DownloadStringAsync (url);
			};

			// Method 2 - WebRequest
			networkRequestButton.Click += delegate {
				var request = WebRequest.Create (url);
				request.BeginGetResponse(Method2, request);
			};

			// Method 3 - HttpClient
			networkRequestButton.Click += async delegate {
				Task<String> contentsTask = Method3(url);
				String result = await contentsTask;
				PrintResult(3, result);
			};
		}

		// Used with "Method 2" above
		private void Method2(IAsyncResult result)
		{
			var request = result.AsyncState as HttpWebRequest;
			try
			{
				var response = request.EndGetResponse(result);
				var reader = new System.IO.StreamReader(response.GetResponseStream());
				PrintResult(2, reader.ReadToEnd());
			}
			catch (Exception e) { }
		}

		// Used with "Method 3" above
		private async Task<String> Method3(Uri url)
		{
			var httpClient = new HttpClient();
			Task<String> contentsTask = httpClient.GetStringAsync(url);
			// "await" returns control to the caller and the task continues to run on another thread
			String contents = await contentsTask;
			return contents;
		}

		private void PrintResult(int methodNum, String result)
		{
			Console.WriteLine("Method " + methodNum);
			Console.WriteLine(result);
//			t = Toast.MakeText(this, Resources.GetString(Resource.String.network_request) + " " + methodNum, ToastLength.Short);
//			t.Show();
		}
	}
}


