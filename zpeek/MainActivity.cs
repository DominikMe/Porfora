using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Speech.Tts;

namespace zpeek
{
	[Activity(Label = "zpeek", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		private EditText _editText;
		private TextToSpeech _tts;
		private MediaRecorder _recorder;
		private MediaPlayer _player;
		private Button _btnRecord;
		private Button _btnPlay;
		private string _audioTmp;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			_audioTmp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/audioTmp.3gpp";

			var btnSpeak = FindViewById<Button>(Resource.Id.btnSpeak);
			btnSpeak.Click += OnSpeakClick;

			_btnRecord = FindViewById<Button>(Resource.Id.btnRecord);
			_btnRecord.Touch += OnRecordTouch;

			_btnPlay = FindViewById<Button>(Resource.Id.btnPlay);
			_btnPlay.Enabled = false;
			_btnPlay.Click += OnPlayClick;

			_editText = FindViewById<EditText>(Resource.Id.wordTextInput);
			var ttsInit = new TTSInit();
            _tts = new TextToSpeech(this, ttsInit);
			ttsInit.TTS = _tts;
		}

		protected override void OnResume()
		{
			base.OnResume();

			_recorder = new MediaRecorder();
			_player = new MediaPlayer();

			_player.Completion += (sender, e) =>
			{
				_player.Reset();
				_btnRecord.Enabled = true;
				_btnPlay.Enabled = true;
			};
		}

		private void OnPlayClick(object sender, EventArgs e)
		{
			_btnRecord.Enabled = false;
			_btnPlay.Enabled = false;

			_player.SetDataSource(_audioTmp);
			_player.Prepare();
			_player.Start();
		}

		protected override void OnPause()
		{
			base.OnPause();

			_player.Release();
			_recorder.Release();
			_player.Dispose();
			_recorder.Dispose();
			_player = null;
			_recorder = null;
		}

		private void OnRecordTouch(object sender, View.TouchEventArgs e)
		{
			if (e.Event.Action == MotionEventActions.Down)
			{
				_recorder.SetAudioSource(AudioSource.Mic);
				_recorder.SetOutputFormat(OutputFormat.ThreeGpp);
				_recorder.SetAudioEncoder(AudioEncoder.AmrNb);
				_recorder.SetOutputFile(_audioTmp);
				_recorder.Prepare();
				_recorder.Start();
			}
			else if (e.Event.Action == MotionEventActions.Up)
			{
				_recorder.Stop();
				_recorder.Reset();
				_btnPlay.Enabled = true;
			}
		}

		private void OnSpeakClick(object sender, EventArgs e)
		{
			var word = _editText.Text;
			_tts.Speak(word, QueueMode.Flush, Bundle.Empty, word);
		}
	}

	internal class TTSInit : TextToSpeech.IOnInitListener
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

