using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Hello_World
{
	[Activity (Label = "SecondActivity")]			
	public class SecondActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Second);

			var label = FindViewById<TextView> (Resource.Id.text);
			// Use null coalescing operator to check for null
			label.Text = Intent.GetStringExtra (Resources.GetString(Resource.String.intent_key_1)) ?? Resources.GetString (Resource.String.second_activity_text_no_intent);
		}
	}
}

