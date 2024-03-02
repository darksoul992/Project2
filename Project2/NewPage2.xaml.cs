namespace Project2;

public partial class NewPage2 : ContentPage
{
	public NewPage2()
	{
		InitializeComponent();
		StartCompass();
	}
	public void StartCompass()
	{
		if (Compass.Default.IsSupported)
		{
			if(!Compass.Default.IsMonitoring)
			{
				Compass.Default.ReadingChanged += CompassReadingChanged;
				Compass.Default.Start(SensorSpeed.UI);
                compassImage.IsVisible = true;

            }
			else
			{
				Compass.Default.Stop();
				Compass.Default.ReadingChanged -= CompassReadingChanged;
			}
		}
		else lblInfo.Text = "Urz¹dzenie nie posiada kompasu";
    }
	double previousRotation = 0;
	private void CompassReadingChanged(object sender, CompassChangedEventArgs e) 
	{

        double newRotation = 360 - e.Reading.HeadingMagneticNorth;
        double rotationDifference = newRotation - compassImage.Rotation;
        if (Math.Abs(rotationDifference) > 180)
        {
            // Ustaw ró¿nicê na krótszy obrót
            if (rotationDifference > 0)
                rotationDifference -= 360;
            else
                rotationDifference += 360;
        }
        compassImage.RelRotateTo(rotationDifference);
        lblInfo.Text = $"Aktualny odczyt kompasu: {Math.Round(e.Reading.HeadingMagneticNorth)}";

        previousRotation = newRotation;

    }
}