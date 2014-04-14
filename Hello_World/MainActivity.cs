using System;
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
		int count = 1;

		protected override void OnCreate(Bundle bundle)
		{
			// base = super
			base.OnCreate(bundle);

			SetContentView (Resource.Layout.Main);
			Button countButton = FindViewById<Button> (Resource.Id.countButton);
			Button newActivityButton = FindViewById<Button> (Resource.Id.newActivityButton);

			// Could also use
			//   button.Click += (sender, e) => { ... };
			// or
			//   button.Click += delegate(object sender, EventArgs e) { ... };
			countButton.Click += delegate {
//				countButton.Text = string.Format(Resources.GetString(Resource.String.number_of_clicks), count++);
				countButton.SetText(string.Format(Resources.GetString(Resource.String.number_of_clicks), count++), TextView.BufferType.Normal);
			};
			newActivityButton.Click += delegate {
				StartActivity(typeof(SecondActivity));
			};
		}
	}
}


