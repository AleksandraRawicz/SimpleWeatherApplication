using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;
using Weather.Services;
using Weather.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Linq;

namespace Weather.ViewModels
{
    class CurrentWeatherPageModel:BindableObject
    {
        public CurrentWeatherPageModel()
        {            
            CurrentTemp = 19;
            CurrentMaxTemp = 26;
            CurrentMinTemp = 2;
            Hours = new List<Hour>();
            Days = new List<Day>();
            Humidity = 70;
            Sunset = new Time() { time_sec= 1622140864+ 7200, time_string= TextHour(new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(1622140864+ 7200).Hour, new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(1622140864+ 7200).Minute) };
            Sunrise = new Time() { time_sec = 1622082330+ 7200, time_string = TextHour(new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(1622082330+ 7200).Hour, new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(1622082330+ 7200).Minute) }; ;
            GetLocation();
        }

        private int currentTemp;
        public int CurrentTemp
        {
            get => currentTemp;
            set => UpdateProprty(ref currentTemp, value);
        }

        private int currentMinTemp;
        public int CurrentMinTemp
        {
            get => currentMinTemp;
            set => UpdateProprty(ref currentMinTemp, value);
        }

        private int currentMaxTemp;
        public int CurrentMaxTemp
        {
            get => currentMaxTemp;
            set => UpdateProprty(ref currentMaxTemp, value);
        }

        private string currentIcon;
        public string CurrentIcon
        {
            get => currentIcon;
            set => UpdateProprty(ref currentIcon, value);
        }

        private int windDeg;
        public int WindDeg
        {
            get => windDeg;
            set => UpdateProprty(ref windDeg, value);
        }

        private float windSpeed;
        public float WindSpeed
        {
            get => windSpeed;
            set => UpdateProprty(ref windSpeed, value);
        }

        private Time sunset;
        public Time Sunset
        {
            get => sunset;
            set => UpdateProprty(ref sunset, value);
        }

        private Time sunrise;
        public Time Sunrise
        {
            get => sunrise;
            set => UpdateProprty(ref sunrise, value);
        }

        private int humidity;
        public int Humidity
        {
            get => humidity;
            set => UpdateProprty(ref humidity, value);
        }

        private string locationCity;
        public string LocationCity
        {
            get => locationCity;
            set => UpdateProprty(ref locationCity, value);
        }

        private string locationCountry;
        public string LocationCountry
        {
            get => locationCountry;
            set => UpdateProprty(ref locationCountry, value);
        }
        private double Latitude { get; set; } = 52.2298;
        private double Longitude { get; set; } = 21.0118;

        private async void GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = Math.Round(location.Latitude, 3);
                    Longitude = Math.Round(location.Longitude, 3);
                    GetLocationData(location.Latitude,location.Longitude);
                    GetWeatherData();
                }
            }
            catch (Exception ex)
            {
                //
            }

        }
        private async Task GetLocationData(double lat, double lon)
        {
            var place = await Geocoding.GetPlacemarksAsync(lat, lon);
            var currentPlace = place?.FirstOrDefault();

            if(currentPlace!=null)
            {
                LocationCity = currentPlace.Locality;
                LocationCountry = currentPlace.CountryName;
            }
        }
        bool UpdateProprty<T>(ref T current, T value, [CallerMemberName] string propertyName=null)
        {
            if(EqualityComparer<T>.Default.Equals(current,value))
            {
                return false;
            }

            current = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private List<Hour> hours;
        public List<Hour> Hours { get => hours; set => UpdateProprty(ref hours, value); }

        private List<Day> days;
        public List<Day> Days { get => days; set => UpdateProprty(ref days, value); }

        public async void GetWeatherData()
        {
            string url = $"https://api.openweathermap.org/data/2.5/onecall?lat={Latitude}&lon={Longitude}&exclude=minutely&appid=[YOUR API key]&units=metric";

            var response = await ApiCaller.Get(url);
            
            if(response.Succesfull)
            {
                try
                {
                    Rootobject weather = JsonConvert.DeserializeObject<Rootobject>(response.Response);

                    CurrentTemp = Convert.ToInt32(weather.current.temp);
                    CurrentMaxTemp = Convert.ToInt32(weather.daily[0].temp.max);
                    CurrentMinTemp = Convert.ToInt32(weather.daily[0].temp.min);
                    CurrentIcon = "i" + weather.current.weather[0].icon + ".png";
                    WindDeg = Convert.ToInt32(weather.current.wind_deg);
                    WindSpeed = Convert.ToInt32(weather.current.wind_speed);


                    var sunrisetime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(weather.current.sunrise + weather.timezone_offset);
                    Sunrise = new Time() { time_sec= (weather.current.sunrise + weather.timezone_offset), time_string = TextHour(Convert.ToInt32(sunrisetime.Hour), Convert.ToInt32(sunrisetime.Minute))};
                    var sunsettime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(weather.current.sunset + weather.timezone_offset);
                    Sunset = new Time(){ time_sec= (weather.current.sunset + weather.timezone_offset), time_string = TextHour(Convert.ToInt32(sunsettime.Hour), Convert.ToInt32(sunsettime.Minute))};
                    Humidity = Convert.ToInt32(weather.current.humidity);

                    List<Day> list1 = new List<Day>();
                    List<Hour> list2 = new List<Hour>();

                    foreach (var d in weather.daily)
                    {
                        var daytime2 = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(d.dt + weather.timezone_offset);
                        Day day = new Day() {
                            temp_max = Convert.ToInt32(d.temp.max),
                            temp_min = Convert.ToInt32(d.temp.min),                            
                            day = daytime2.DayOfWeek.ToString() + ", \n" + TextDate(daytime2.Month, daytime2.Day),                                               
                            icon = "i" + d.weather[0].icon+".png" };                 
                        list1.Add(day);
                    }
                    Days = list1;

                    for(int i=0; i<24; i++)
                    {
                        var h = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(weather.hourly[i].dt + weather.timezone_offset);
                        Hour hour = new Hour() {
                            temp = Convert.ToInt32(weather.hourly[i].temp),
                            hour = TextHour(h.Hour), 
                            icon = "i" + weather.hourly[i].weather[0].icon + ".png"
                        };
                        list2.Add(hour);
                    }

                    Hours = list2;
                }
                catch(Exception e)
                {
                    App.Current.MainPage.DisplayAlert("error", e.Message, "ok");
                }
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Weather Info", "No data found", "ok");
            }
        }
        string TextHour(int h, int min=0)
        {
            string result;
            if (h < 10)
                result= "0" + h + ":" ;
            else
                result =  h +":" ;

            if (min < 10)
                result += "0" + min;
            else
                result += "" + min;

            return result;
        }        
        string TextDate(int month, int day)
        {
            string[] months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            return day + " " + months[month - 1];
        }
    }  
}
