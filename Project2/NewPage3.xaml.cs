namespace Project2;

public partial class NewPage3 : ContentPage
{
	public NewPage3()
	{
		InitializeComponent();
        Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
    }
    SensorSpeed speed = SensorSpeed.UI;

    // Sta³a definiuj¹ca minimaln¹ wartoœæ przyspieszenia potrzebn¹ do zarejestrowania kroku
    const double StepThreshold = 1.2;

    // Zmienna przechowuj¹ca poprzedni¹ wartoœæ przyspieszenia
    double lastAccelerationValue = 0;

    // Zmienna przechowuj¹ca liczbê kroków
    int stepCount = 0;


    void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        // Pobranie danych odczytanych z akcelerometru
        var data = e.Reading.Acceleration;

        // Obliczenie wartoœci przyspieszenia (d³ugoœæ wektora przyspieszenia)
        double accelerationValue = Math.Sqrt(data.X * data.X + data.Y * data.Y + data.Z * data.Z);

        // Detekcja kroku na podstawie zmiany wartoœci przyspieszenia
        if (Math.Abs(accelerationValue - lastAccelerationValue) > StepThreshold)
        {
            // Zarejestrowano krok
            stepCount++;
            Device.BeginInvokeOnMainThread(() =>
            {
                StepLabel.Text = $"Steps: {stepCount}";
            });
        }

        // Aktualizacja poprzedniej wartoœci przyspieszenia
        lastAccelerationValue = accelerationValue;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Rozpoczêcie nas³uchiwania danych z akcelerometru przy pojawieniu siê strony
        Accelerometer.Start(speed);


    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Zatrzymanie nas³uchiwania danych z akcelerometru przy znikniêciu strony
        Accelerometer.Stop();
    }
}