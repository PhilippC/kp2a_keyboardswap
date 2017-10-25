using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Java.Lang;
using System.Linq;
using Android.Views.InputMethods;

namespace KeyboardSwapPlugin
{
	[Activity(Label = "KeyboardSwap Plugin", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		bool TryActivateIme(string newIme)
		{
			string imes = Settings.Secure.GetString(ContentResolver, Settings.Secure.EnabledInputMethods);
			var imesList = imes.Split(':');
			Android.Util.Log.Debug("KP2A", imes);
			
			if (imesList.Contains(newIme))
				try
				{
					Settings.Secure.PutString(ContentResolver, Settings.Secure.DefaultInputMethod, newIme);
				}
				catch (SecurityException)
				{
					FindViewById<TextView>(Resource.Id.result).Text = GetString(Resource.String.NoPermission);
					return false;
				}

			else
			{
				FindViewById<TextView>(Resource.Id.result).Text =  "Did not find " + newIme + " in list of enabled input methods";
				return false;
			}
			FindViewById<TextView>(Resource.Id.result).Text = GetString(Resource.String.result_ok);
			return true;

		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			FindViewById<Button>(Resource.Id.btntest).Click += (sender, args) =>
			{
				string imes = Settings.Secure.GetString(ContentResolver, Settings.Secure.EnabledInputMethods);
				var imesList = imes.Split(':');
			
				string currentIme = Settings.Secure.GetString(ContentResolver, Settings.Secure.DefaultInputMethod);

				if (!imesList.Any())
				{
					FindViewById<TextView>(Resource.Id.result).Text = GetString(Resource.String.NoPermission);
					return;
				}

				if (!imesList.Contains(currentIme))
				{
					currentIme = imesList.First();
				}

				TryActivateIme(currentIme);
				
			};

		}

		protected override void OnResume()
		{

			if (Intent.HasExtra("ImeName"))
			{
				string imeName = Intent.GetStringExtra("ImeName");
				if ((!imeName.StartsWith("keepass2android.keepass2android"))
					||
					(!imeName.EndsWith("/keepass2android.softkeyboard.KP2AKeyboard")))
					return; //don't switch to other IMEs

				if (TryActivateIme(imeName))
					Finish();
			}
			base.OnResume();
		}
	}
}

