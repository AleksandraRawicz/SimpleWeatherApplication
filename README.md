# Weather application

### Mobile application made with Xamarin Forms

## Table of contents
1. [Description](#description)
2. [Installation](#installation)

## Description

Draft of a simple weather application for Android OS that gets weather data from openweathermap.org

Design of this app was highly inspired by huawei weather apk


## Installation

In order to install and run the application:
1. Clone repository on your computer
2. You will also need an API Key from openweathermap.org. In order to get one, you'll need to sign in there. Once you have it, just change url address in GetWeatherData() method in CurrentWeatherPageModel.cs file:

```c#
string url = $"https://api.openweathermap.org/data/2.5/onecall?lat={Latitude}&lon={Longitude}&exclude=minutely&appid=[YOUR API Key]&units=metric";
```
