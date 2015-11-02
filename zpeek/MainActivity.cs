using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Speech.Tts;

namespace Profora
{
	[Activity(Label = "Profora", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;
		private EditText _editText;
		private TextToSpeech _tts;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.btnSpeak);

			button.Click += OnSpeakClick;

			_editText = FindViewById<EditText>(Resource.Id.wordTextInput);
			var ttsInit = new TTSInit();
            _tts = new TextToSpeech(this, ttsInit);
			ttsInit.TTS = _tts;
		}

		private void OnSpeakClick(object sender, EventArgs e)
		{
			var word = _editText.Text;
			_tts.Speak(word, QueueMode.Flush, Bundle.Empty, word);
		}
	}

	class TTSInit : TextToSpeech.IOnInitListener
	{
		internal TextToSpeech TTS { get; set; }

		internal TTSInit()
		{
		}

		public void Dispose()
		{
			TTS.Dispose();
		}

		public IntPtr Handle { get; }
		public void OnInit(OperationResult status)
		{
			switch (status)
			{
				case OperationResult.Success:
					break;
				case OperationResult.Error:
					throw new Exception("TTS Init Error");
				case OperationResult.Stopped:
					throw new Exception("TTS Init Stopped");
				default:
					throw new ArgumentOutOfRangeException(nameof(status), status, null);
			}
		}
	}
}

