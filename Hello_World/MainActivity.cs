using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SQLite;
using Newtonsoft.Json;
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
		private Button countButton, newActivityButton, networkRequestButton;
		private SQLiteAsyncConnection conn;

		protected override void OnCreate(Bundle bundle)
		{
			// base = super
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);
			countButton = FindViewById<Button>(Resource.Id.countButton);
			newActivityButton = FindViewById<Button>(Resource.Id.newActivityButton);
			networkRequestButton = FindViewById<Button>(Resource.Id.networkRequestButton);

			String folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			conn = new SQLiteAsyncConnection(Path.Combine(folder, "test.db"));
			conn.CreateTableAsync<JSONResponse>().ContinueWith(t => {
				Console.WriteLine("Connected to DB");
			});

			// Could also use
			//   button.Click += (sender, e) => { ... };
			// or
			//   button.Click += delegate(object sender, EventArgs e) { ... };
			countButton.Click += delegate {
				// Could also set the text directly via
				//   countButton.Text = string.Format(Resources.GetString(Resource.String.number_of_clicks), count++);
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
			var badurl = new Uri(String.Format("http://validate.bad.jsontest.com/?json={0}", json));

			// Method 1 - WebClient
			/*
			networkRequestButton.Click += delegate {
				var webClient = new WebClient ();
				webClient.DownloadStringCompleted += (sender, eventArgs) => {
					try
					{
						var text = eventArgs.Result;
						PrintResult(1, text);
					}
					catch (Exception e)
					{
						PrintResult(1, e.InnerException.Message);
					}
				};
				webClient.DownloadStringAsync(url);
			};
			*/

			// Method 2 - WebRequest
			/*
			networkRequestButton.Click += delegate {
				var request = WebRequest.Create(url);
				request.BeginGetResponse(Method2, request);
			};
			*/

			// Method 3 - HttpClient (requires async and await)
			networkRequestButton.Click += async delegate {
				Task<String> contentsTask = Method3(url);
				try
				{
					// "await" returns control to the caller and the task continues to run on another thread
					String result = await contentsTask;
//					PrintResult(3, result);

					JSONResponse resultObj = JsonConvert.DeserializeObject<JSONResponse>(result);
					conn.InsertAsync(resultObj).ContinueWith(t => {
						PrintTable();
					});
				}
				catch (Exception e)
				{
					// Exceptions that occur inside an async method are stored in the task and thrown when the task is awaited
					PrintResult(3, e.Message);
				}
			};
		}

		private async void PrintTable()
		{
			Console.WriteLine("{0} items in table", await conn.Table<JSONResponse>().CountAsync());
		}

		// Used with "Method 2" above
		/*
		private void Method2(IAsyncResult result)
		{
			var request = result.AsyncState as HttpWebRequest;
			try
			{
				var response = request.EndGetResponse(result);
				var reader = new System.IO.StreamReader(response.GetResponseStream());
				PrintResult(2, reader.ReadToEnd());
			}
			catch (Exception e)
			{
				PrintResult(2, e.Message);
			}
		}
		*/

		// Used with "Method 3" above
		private async Task<String> Method3(Uri url)
		{
			var httpClient = new HttpClient();
			Task<String> contentsTask = httpClient.GetStringAsync(url);
			// Wait on the Task returned by GetStringAsync
			String contents = await contentsTask;
			// Task<TResult> returns an object of type TResult, in this case String
			return contents;
		}

		private void PrintResult(int methodNum, String result)
		{
			Console.WriteLine("----- Method " + methodNum + " -----");
			Console.WriteLine(result);
		}
	}

	public class JSONResponse
	{
		// These are attributes
		[PrimaryKey, AutoIncrement]
		// This is a property
		public int Id { get; set; }
		public String ObjOrArr { get; set; }
		public bool Empty { get; set; }
		public long ParseTimeNs { get; set; }
		public bool Valid { get; set; }
		public int Size { get; set; }
	}
}


