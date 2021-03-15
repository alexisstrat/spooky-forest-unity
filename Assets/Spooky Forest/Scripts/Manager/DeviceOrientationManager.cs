using UnityEngine;

namespace Spooky_Forest.Scripts.Manager
{
	public class DeviceOrientationManager : MonoBehaviour
	{
		public const float ORIENTATION_CHECK_INTERVAL = 0.1f;

		private float nextOrientationCheckTime;

		private static ScreenOrientation _currentOrientation;
		public static ScreenOrientation CurrentOrientation
		{
			get
			{
				return _currentOrientation;
			}
			set
			{
				if( _currentOrientation != value )
				{
					_currentOrientation = value;
					Screen.orientation = value;

					if( OnScreenOrientationChanged != null )
						OnScreenOrientationChanged( value );
				}
			}
		}

		public static bool AutoRotateScreen = true;
		public static event System.Action<ScreenOrientation> OnScreenOrientationChanged = null;

		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
		private static void Init()
		{
			DontDestroyOnLoad( new GameObject( "DeviceOrientationManager", typeof( DeviceOrientationManager ) ) );
		}

		void Awake()
		{
			_currentOrientation = Screen.orientation;
			nextOrientationCheckTime = Time.realtimeSinceStartup + 1f;
		}

		void Update()
		{
			if( !AutoRotateScreen )
				return;
		
			if( Time.realtimeSinceStartup >= nextOrientationCheckTime )
			{
				DeviceOrientation orientation = Input.deviceOrientation;
				if( orientation == DeviceOrientation.Portrait || orientation == DeviceOrientation.PortraitUpsideDown ||
				    orientation == DeviceOrientation.LandscapeLeft || orientation == DeviceOrientation.LandscapeRight )
				{
					if( orientation == DeviceOrientation.LandscapeLeft )
					{
						if( Screen.autorotateToLandscapeLeft )
							CurrentOrientation = ScreenOrientation.LandscapeLeft;
					}
					else if( orientation == DeviceOrientation.LandscapeRight )
					{
						if( Screen.autorotateToLandscapeRight )
							CurrentOrientation = ScreenOrientation.LandscapeRight;
					}
					else if( orientation == DeviceOrientation.PortraitUpsideDown )
					{
						if( Screen.autorotateToPortraitUpsideDown )
							CurrentOrientation = ScreenOrientation.PortraitUpsideDown;
					}
					else
					{
						if( Screen.autorotateToPortrait )
							CurrentOrientation = ScreenOrientation.Portrait;
					}
				}

				nextOrientationCheckTime = Time.realtimeSinceStartup + ORIENTATION_CHECK_INTERVAL;
			}
		}

		public static void ForceOrientation( ScreenOrientation orientation )
		{
			if( orientation == ScreenOrientation.AutoRotation )
				AutoRotateScreen = true;
#pragma warning disable 618
			else if( orientation != ScreenOrientation.Unknown )
#pragma warning restore 618
			{
				AutoRotateScreen = false;
				CurrentOrientation = orientation;
			}
		}
	}
}
