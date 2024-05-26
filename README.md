# Generic HTTP Client

GenericHttpClient is a library that provides a simple and flexible way to send HTTP requests using a generic HTTP client. It supports GET, POST, PUT, and DELETE requests and handles responses with built-in deserialization.

## Features
- Generic HTTP client interface for easy integration
- Support for GET, POST, PUT, and DELETE requests
- Automatic deserialization of JSON responses
- Detailed error handling and response status

## Installation
You can install the GenericHttpClient NuGet package using the .NET CLI:
```sh
dotnet add package Generic-Http-Client --version 1.0.0
```
Or via the NuGet Package Manager:
```sh
Install-Package Generic-Http-Client -Version 1.0.0
```
## Usage
### 1. Create an instance of HttpClientWrapper
```csharp
var httpClientWrapper = new HttpClientWrapper();
```
### 2. Create an instance of GenericHttpClient
```csharp
var genericHttpClient = new GenericHttpClient(httpClientWrapper);
```
### 3. Send a request
```csharp
var url = "https://api.example.com/data";
var response = await genericHttpClient.SendRequestAsync<MyResponseType>(url, HttpRequestType.GET);

if (response.ResponseType == HttpResponseType.Success)
{
    var data = response.Data;
    // Handle success
}
else
{
    // Handle failure
    Console.WriteLine(response.Message);
}
```
## Classes and Interfaces
### GenericHttpClient
| Method | Description |
| ------ | ----------- |
|SendRequestAsync<T>(url, httpRequestType, payload)|Sends a request of the specified type to the provided URL with an optional payload.|

### HttpClientWrapper
| Method | Description |
| ------ | ----------- |
|GetAsync(url)|Sends a GET request to the specified URL.|
|PostAsync<T>(url, payload)|Sends a POST request with the provided payload to the specified URL.|
|PutAsync<T>(url, payload)|Sends a PUT request with the provided payload to the specified URL.|
|DeleteAsync(url)|Sends a DELETE request to the specified URL.|

### HttpResponse<T>
| Property | Description |
| -------- | ----------- |
|ResponseType|Indicates the type of response (Success, Failure, etc.)|
|Message|Provides a message in case of failure or empty response|
|Data|Contains the deserialized data from the response|

### HttpRequestType
| Value | Description |
| ----- | ----------- |
|GET|GET request|
|POST|POST request|
|PUT|PUT request|
|DELETE|DELETE request|

### HttpResponseType
| Value | Description |
| ----- | ----------- |
|Success|The request was successful and the response contains data.|
|Failure|The request failed.|
|Empty|The request succeeded but the response was empty.|
|Undeserializable|The request succeeded but the response could not be deserialized.|

## Example
Here's an example of sending a POST request with a payload:

```csharp
var url = "https://api.example.com/data";
var payload = new MyRequestType { Property1 = "value1", Property2 = "value2" };
var response = await genericHttpClient.SendRequestAsync<MyResponseType>(url, HttpRequestType.POST, payload);

if (response.ResponseType == HttpResponseType.Success)
{
    var data = response.Data;
    // Handle success
}
else
{
    // Handle failure
    Console.WriteLine(response.Message);
}
```

## License
This project is licensed under the MIT License.

For contributions and problem reports:
[![GitHub](https://img.shields.io/badge/GitHub-Repository-blue?logo=github)](https://github.com/sandybridge9/GenericHttpClient)