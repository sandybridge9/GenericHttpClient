using FluentAssertions;
using GenericHttpClient.Models;
using Moq;

namespace GenericHttpClient.UnitTests
{
    public class GenericHttpClientTests
    {
        private const string DefaultUrl = "DefaultUrl";

        private readonly Mock<HttpClient> httpClientMock = new Mock<HttpClient>();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task When_SendRequestAsync_With_InvalidUrl_Should_ThrowArgumentException(string url)
        {
            // Arrange
            var requestType = HttpRequestType.GET;

            // Act
            var exception = await Record.ExceptionAsync(async () =>
                await Sut().SendRequestAsync<object>(
                    url,
                    requestType));

            // Assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public async Task When_SendRequestAsync_With_PostRequestTypeAndNullPayload_Should_ThrowArgumentException()
        {
            // Arrange
            var requestType = HttpRequestType.POST;

            // Act
            var exception = await Record.ExceptionAsync(async () =>
                await Sut().SendRequestAsync<object>(
                    DefaultUrl,
                    requestType));

            // Assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public async Task When_SendRequestAsync_With_PutRequestTypeAndNullPayload_Should_ThrowArgumentException()
        {
            // Arrange
            var requestType = HttpRequestType.PUT;

            // Act
            var exception = await Record.ExceptionAsync(async () =>
                await Sut().SendRequestAsync<object>(
                    DefaultUrl,
                    requestType));

            // Assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public async Task When_SendRequestAsync_With_DeleteRequestTypeAndNonBoolGenericType_Should_ThrowArgumentException()
        {
            // Arrange
            var requestType = HttpRequestType.DELETE;

            // Act
            var exception = await Record.ExceptionAsync(async () =>
                await Sut().SendRequestAsync<object>(
                    DefaultUrl,
                    requestType));

            // Assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public async Task When_SendRequestAsync_With_GetRequestType_Should_ReturnExpectedResult()
        {
            // Arrange
            var requestType = HttpRequestType.GET;

            httpClientMock.Setup(() => )

            // Act
            var result = await Sut().SendRequestAsync<object>(
                DefaultUrl,
                requestType));


            // Assert
            exception.Should().BeOfType<ArgumentException>();
        }

        private struct TestSubject
        {
            int a;

            string b;

            bool c;
        }

        //[Theory]
        //[InlineData("", HttpRequestType.GET)]
        //[InlineData(null, HttpRequestType.POST)]
        //public async Task SendRequestAsync_ThrowsArgumentException_WhenUrlIsInvalid(string url, HttpRequestType requestType)
        //{
        //    await Assert.ThrowsAsync<ArgumentException>(() => _genericHttpClient.SendRequestAsync(url, requestType));
        //}

        //[Theory]
        //[InlineData(HttpRequestType.POST)]
        //[InlineData(HttpRequestType.PUT)]
        //public async Task SendRequestAsync_ThrowsArgumentException_WhenPayloadIsNullForPostOrPut(HttpRequestType requestType)
        //{
        //    await Assert.ThrowsAsync<ArgumentException>(() => _genericHttpClient.SendRequestAsync("http://example.com", requestType, null));
        //}

        //[Theory]
        //[InlineData(HttpRequestType.GET, typeof(bool))]
        //[InlineData(HttpRequestType.DELETE, typeof(bool))]
        //public async Task SendRequestAsync_ThrowsArgumentException_WhenInvalidGenericTypeForGetOrDelete(HttpRequestType requestType, Type type)
        //{
        //    var genericMethod = _genericHttpClient.GetType().GetMethod("SendRequestAsync").MakeGenericMethod(type);
        //    await Assert.ThrowsAsync<ArgumentException>(() => (Task)genericMethod.Invoke(_genericHttpClient, new object[] { "http://example.com", requestType, null }));
        //}

        //[Theory]
        //[InlineData(HttpRequestType.GET, HttpStatusCode.OK)]
        //[InlineData(HttpRequestType.POST, HttpStatusCode.Created)]
        //[InlineData(HttpRequestType.PUT, HttpStatusCode.NoContent)]
        //[InlineData(HttpRequestType.DELETE, HttpStatusCode.OK)]
        //public async Task SendRequestAsync_ReturnsSuccess_WhenRequestIsSuccessful(HttpRequestType requestType, HttpStatusCode statusCode)
        //{
        //    SetupHttpResponse(statusCode, "{\"data\":true}");
        //    var response = await _genericHttpClient.SendRequestAsync<bool>("http://example.com", requestType, true);
        //    Assert.Equal(HttpResponseType.Success, response.ResponseType);
        //}

        //[Theory]
        //[InlineData(HttpRequestType.GET, HttpStatusCode.BadRequest)]
        //[InlineData(HttpRequestType.POST, HttpStatusCode.InternalServerError)]
        //public async Task SendRequestAsync_ReturnsFailure_WhenRequestFails(HttpRequestType requestType, HttpStatusCode statusCode)
        //{
        //    SetupHttpResponse(statusCode);
        //    var response = await _genericHttpClient.SendRequestAsync<bool>("http://example.com", requestType, true);
        //    Assert.Equal(HttpResponseType.Failure, response.ResponseType);
        //}

        //private void SetupHttpResponse(HttpStatusCode statusCode, string content = null)
        //{
        //    var responseMessage = new HttpResponseMessage
        //    {
        //        StatusCode = statusCode,
        //        Content = content != null ? new StringContent(content, Encoding.UTF8, "application/json") : null
        //    };

        //    _httpMessageHandlerMock
        //        .Protected()
        //        .Setup<Task<HttpResponseMessage>>(
        //            "SendAsync",
        //            ItExpr.IsAny<HttpRequestMessage>(),
        //            ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(responseMessage);
        //}

        private Clients.GenericHttpClient Sut()
            => new Clients.GenericHttpClient(httpClientMock.Object);
    }
}