using Bumptech.Glide.Load;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform;
using Org.Apache.Http.Client;
using Org.Apache.Http.Protocol;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project2;
class Weather
{
    public double latitude { get; set; }
    public double longitude { get; set; }
    public double generationtime_ms { get; set; }
    public double utc_offset_seconds { get; set; }
    public string timezone { get; set; }
    public string timezone_abbreviation { get; set; }
    public double elevation { get; set; }
    public Units daily_units { get; set; }
    public Forecast daily { get; set; }
}
class Units
{
    public string time {  get; set; }
    public string temperature_2m_max {  get; set; }
    public string temperature_2m_min {  get; set; }
    public string rain_sum {  get; set; }
    public string weather_code {  get; set; }
    public string wind_speed_10m_max {  get; set; }
    public string wind_gusts_10m_max {  get; set; }
    public string shortwave_radiation_sum {  get; set; }
    public string wind_direction_10m_dominant {  get; set; }
    public string sunrise {  get; set; }
    public string sunset {  get; set; }
    public string precipitation_probability_mean {  get; set; }
    public string snowfall_sum {  get; set; }
    public string sunshine_duration {  get; set; }
}
class Forecast
{
    public List<string> time { get; set;}
    public List<double> temperature_2m_max { get; set;}
    public List<double> temperature_2m_min { get; set;}
    public List<double> rain_sum { get; set;}
    public List<int> weather_code { get; set;}
    public List<double> wind_speed_10m_max { get; set;}
    public List<double> wind_gusts_10m_max { get; set;}
    public List<double> shortwave_radiation_sum { get; set;}
    public List<int> wind_direction_10m_dominant { get; set;}
    public List<string> sunrise { get; set;}
    public List<string> sunset { get; set;}
    public List<int> precipitation_probability_mean { get; set;}
    public List<double> snowfall_sum { get; set;}
    public List<double> sunshine_duration { get; set;}
}
public partial class NewPage1 : ContentPage
{
    // Przechowuje obecn¹ lokalizacjê
    Location currentLocation;
    public NewPage1()
    {
        InitializeComponent();
    }
    // Metoda pobieraj¹ca lokalizacjê
    async Task<Location> GetLocation()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);

            if (location != null)
            {
                return location;
            }
            else
            {
                // Nie uda³o siê pobraæ lokalizacji
                return null;
            }
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Lokalizacja nie jest obs³ugiwana
            await DisplayAlert("B³¹d",$"Lokalizacja nie jest obs³ugiwana na tym urz¹dzeniu: {fnsEx.Message}", "OK");
            return null;
        }
        catch (PermissionException pEx)
        {
            // Brak uprawnieñ
            await DisplayAlert("B³¹d",$"Brak uprawnieñ do lokalizacji: {pEx.Message}", "OK");
            return null;
        }
        catch (Exception ex)
        {
            // Inny b³¹d
            await DisplayAlert("B³¹d",$"B³¹d podczas pobierania lokalizacji: {ex.Message}", "OK");
            return null;
        }
    }
    // Metoda ³¹cz¹ca siê z API
    private async Task<string> CallApi(string apiURL)
    {
        HttpClient _httpClient = new HttpClient();
        try
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response = await _httpClient.GetAsync(apiURL);

            if (response.IsSuccessStatusCode)
            {
                string responseFromAPI = await response.Content.ReadAsStringAsync();
                return responseFromAPI;
            }
            else
            {
                await DisplayAlert("B³¹d",$"Nieudana odpowiedŸ z serwera: {response.StatusCode}", "OK");
                return null;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d",$"Wyst¹pi³ b³¹d podczas komunikacji z serwerem: {ex.Message}", "OK");
            return null;
        }
    }

    //Metoda pobieraj¹ca dane pogodowe
    private async void GetWeatherData(object sender, EventArgs e)
    {
        loadingProgress.IsVisible = true;
        loadingProgress.IsRunning = true;
        currentLocation = await GetLocation();
        if( currentLocation != null )
        {
            string lat_string = currentLocation.Latitude.ToString();
            string lon_string = currentLocation.Longitude.ToString();

            lat_string = lat_string.Replace(",", ".");
            lon_string = lon_string.Replace(",", ".");

            string apiURL = $"https://api.open-meteo.com/v1/forecast?latitude={lat_string}&longitude={lon_string}&daily=temperature_2m_max&daily=temperature_2m_min&daily=rain_sum&daily=weather_code&daily=wind_speed_10m_max&daily=wind_gusts_10m_max&daily=shortwave_radiation_sum&daily=wind_direction_10m_dominant&daily=sunrise&daily=sunset&daily=precipitation_probability_mean&daily=snowfall_sum&daily=sunshine_duration&timezone=auto";
            string result = await CallApi(apiURL);

            var weatherData = JsonSerializer.Deserialize<Weather>(result);
            ShowWeatherData(weatherData);
        }
        loadingProgress.IsVisible = false;
        loadingProgress.IsRunning = false;
    }
    private async void ShowWeatherData(Weather weatherData)
    {
        weather_info.IsVisible = true;
        lblDay.Text = $"{await GetPlacemark(currentLocation)} Dziœ, {weatherData.daily.time[0]}";
        Debug.WriteLine(weatherData.daily.weather_code[0]);
        lblWeatherState.Text = $"{GetWeatherConditions(weatherData.daily.weather_code[0])}";
        lblTemperature.Text = $"{weatherData.daily.temperature_2m_max[0]} {weatherData.daily_units.temperature_2m_max}";
        lblTemperatureMin.Text = $"minimalna: {weatherData.daily.temperature_2m_min[0]} {weatherData.daily_units.temperature_2m_min}";
        lblWind.Text = $"{weatherData.daily.wind_speed_10m_max[0]} {weatherData.daily_units.wind_speed_10m_max}";
        lblWindGusts.Text = $"w porywach do {weatherData.daily.wind_gusts_10m_max[0]} {weatherData.daily_units.wind_gusts_10m_max}";
        lblWindDirection.Text = $"kierunek: {GetWindDirection(weatherData.daily.wind_direction_10m_dominant[0])}";
        lblRain.Text = $"{weatherData.daily.rain_sum[0]} {weatherData.daily_units.rain_sum}";
        lblSunrise.Text = $"wschód s³oñca: {weatherData.daily.sunrise[0].Substring(11,5)}";
        lblSunset.Text = $"zachód s³oñca: {weatherData.daily.sunset[0].Substring(11, 5)}";
        ShowWeatherDataForNextDays(weatherData);
    }
    private void ExpanderChange(object sender, ExpandedChangedEventArgs e) 
    {
        Expander expander = sender as Expander;
        var expanderHeaderLayout = (HorizontalStackLayout)expander.Header;
        var expanderHeaderImage = (Image)expanderHeaderLayout.Children.OfType<Image>().FirstOrDefault();
        expanderHeaderImage.Source = expander.IsExpanded ? "up_arrow_icon.png" : "down_arrow_icon.png";
        Debug.WriteLine(headerImage.Source);
        
        //headerLayout.Children.Add(headerImage);
        //expander.Header = null;
        //expander.Header = headerLayout;
    }
    Image headerImage = null;
    HorizontalStackLayout headerLayout = null;
    private void ShowWeatherDataForNextDays(Weather weatherData)
    {
        expanderContainer.Children.Clear(); 
        for (int i = 1; i < weatherData.daily.time.Count - 1; i++)
        {
            headerImage = new Image
            {
                Source = "down_arrow_icon.png",
                WidthRequest = 20,
                Margin = new Thickness(10, 0, 0, 0)
            };
            headerLayout = new HorizontalStackLayout
            {
                Children =
                {
                    new Label
                    {
                        Text = weatherData.daily.time[i],
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 24,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    headerImage
                }
            };
            var expander = new Expander
            {
                Header = headerLayout,
                IsExpanded = false,
                IsVisible = true,
                Direction = ExpandDirection.Down,
            };
            expander.ExpandedChanged += ExpanderChange;
            expander.Content = new VerticalStackLayout
            {
                Padding = new Thickness(10),
                Children =
                {
                    new Label
                    {
                        Text =$"Temperatura: {weatherData.daily.temperature_2m_max[i]}"
                    },
                    new Label
                    {
                        Text =$"Wiatr: {weatherData.daily.wind_speed_10m_max[i]}"
                    },
                    new Label
                    {
                        Text =$"Deszcz: {weatherData.daily.rain_sum[i]}"
                    },
                    new Label
                    {
                        Text =$"Wschód s³oñca: {weatherData.daily.sunrise[i].Substring(11,5)}, Zachód s³oñca: {weatherData.daily.sunset[i].Substring(11,5)}"
                    },
                },
            };
            expanderContainer.Children.Add(expander);
        }
        foreach(Object obj in mainContainer.Children) {
            Debug.WriteLine(obj);
        }
    }
    private async void information_Show(object sender, EventArgs e)
    {
        await DisplayAlert("Informacja", $"ród³o danych z API: open-meteo.com\nAPI jest darmowe i nie wymaga klucza, jednak mo¿liwe jest wywo³anie API maksymalnie 10 000 razy dziennie", "OK");
    }
    static async Task<string> GetPlacemark(Location location)
    {

        var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
        var placemark = placemarks?.FirstOrDefault();
        if (placemark != null)
        {
            return placemark.Locality;
        }
        else
            return null;
    }
    static string GetWindDirection(int windDirection)
    {
        if (windDirection >= 0 && windDirection <= 22)
            return "pó³nocny";
        if (windDirection > 22 && windDirection <= 67)
            return "pó³nocno-wschodni";
        if (windDirection > 67 && windDirection < 112)
            return "wschodni";
        if (windDirection > 112 && windDirection < 157)
            return "po³udniowo-wschodni";
        if (windDirection > 157 && windDirection < 202)
            return "po³udniowy";
        if (windDirection > 202 && windDirection < 247)
            return "po³udniowo-zachodni";
        if (windDirection > 247 && windDirection < 292)
            return "zachodni";
        if (windDirection > 292 && windDirection < 337)
            return "pó³nocno-zachodni";
        if (windDirection >= 337)
            return "pó³nocny";

        return null;
    }
    static string GetWeatherConditions(int weatherCode)
    {
        switch(weatherCode)
        {
            case 0:
                return "Czyste niebo";
            case 1:
                return "Przewa¿nie bezchmurnie";
            case 2:
                return "Czêœciowo pochmurno";
            case 3:
                return "Zachmurzenie";
            case 45:
                goto case 48;
            case 48:
                return "Mg³a";
            case 51:
                return "Lekka m¿awka";
            case 53:
                return "Umiarkowana m¿awka";
            case 55:
                return "Intensywna m¿awka";
            case 56:
                return "Marzn¹ca m¿awka - lekka";
            case 57:
                return "Marzn¹ca m¿awka - intensywna";
            case 61:
                return "Niewielki deszcz";
            case 63:
                return "Umiarkowany deszcz";
            case 65:
                return "Obfite opady deszczu";
            case 66:
                return "Marzn¹cy deszcz - lekki";
            case 67:
                return "Marzn¹cy deszcz - intensywny";
            case 71:
                return "Niewielkie opady œniegu";
            case 73:
                return "Umiarkowane opady œniegu";
            case 75:
                return "Obfite opady œniegu";
            case 77:
                return "Ziarnisty œnieg";
            case 80:
                goto case 61;
            case 81:
                goto case 63;
            case 82:
                goto case 65;
            case 85:
                goto case 71;
            case 86:
                goto case 75;
            case 95:
                return "Burza";
            case 96:
                return "Burza z niewielkim gradem";
            case 99:
                return "Burza z du¿ym gradem";
            default:
                return null;
        }
    }
}