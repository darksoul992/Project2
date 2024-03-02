namespace Project2;

public partial class NewPage3 : ContentPage
{
	public NewPage3()
	{
		InitializeComponent();
        Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
    }
    SensorSpeed speed = SensorSpeed.UI;

    // Sta�a definiuj�ca minimaln� warto�� przyspieszenia potrzebn� do zarejestrowania kroku
    const double StepThreshold = 1.2;

    // Zmienna przechowuj�ca poprzedni� warto�� przyspieszenia
    double lastAccelerationValue = 0;

    // Zmienna przechowuj�ca liczb� krok�w
    int stepCount = 0;


    void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        // Pobranie danych odczytanych z akcelerometru
        var data = e.Reading.Acceleration;

        // Obliczenie warto�ci przyspieszenia (d�ugo�� wektora przyspieszenia)
        double accelerationValue = Math.Sqrt(data.X * data.X + data.Y * data.Y + data.Z * data.Z);

        // Detekcja kroku na podstawie zmiany warto�ci przyspieszenia
        if (Math.Abs(accelerationValue - lastAccelerationValue) > StepThreshold)
        {
            // Zarejestrowano krok
            stepCount++;
            Device.BeginInvokeOnMainThread(() =>
            {
                StepLabel.Text = $"Steps: {stepCount}";
            });
        }

        // Aktualizacja poprzedniej warto�ci przyspieszenia
        lastAccelerationValue = accelerationValue;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Rozpocz�cie nas�uchiwania danych z akcelerometru przy pojawieniu si� strony
        Accelerometer.Start(speed);


    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Zatrzymanie nas�uchiwania danych z akcelerometru przy znikni�ciu strony
        Accelerometer.Stop();
    }
}