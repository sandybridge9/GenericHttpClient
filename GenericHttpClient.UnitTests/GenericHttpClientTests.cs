using FluentAssertions;
using GenericHttpClient.Clients;
using GenericHttpClient.Models;
using Moq;

namespace GenericHttpClient.UnitTests
{
    public class GenericHttpClientTests
    {
        private const string DefaultUrl = "DefaultUrl";

        private readonly Mock<IHttpClientWrapper> httpClientMock = new Mock<IHttpClientWrapper>();

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

        [Theory]
        [InlineData(HttpRequestType.POST)]
        [InlineData(HttpRequestType.PUT)]
        public async Task When_SendRequestAsync_With_PostRequestTypeAndNullPayload_Should_ThrowArgumentException(HttpRequestType requestType)
        {
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
        public async Task When_SendRequestAsync_With_InvalidRequestType_Should_ThrowNotImplementedException()
        {
            // Act
            var exception = await Record.ExceptionAsync(async () =>
                await Sut().SendRequestAsync<object>(
                    DefaultUrl,
                    (HttpRequestType) int.MaxValue));

            // Assert
            exception.Should().BeOfType<NotImplementedException>();
        }

        [Fact]
        public async Task When_SendRequestAsync_With_GetRequestType_Should_ReturnExpectedResult()
        {
            // Arrange
            var requestType = HttpRequestType.GET;

            httpClientMock.Setup(x => x.GetAsync(DefaultUrl)).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(@"{""a"": 0, ""b"": null,""c"": false}")
            });

            // Act
            var result = await Sut().SendRequestAsync<TestSubject>(DefaultUrl, requestType);

            // Assert
            result.ResponseType.Should().Be(HttpResponseType.Success);
            result.Data.Should().BeOfType<TestSubject>();
            result.Data.As<TestSubject>().a.Should().Be(0);
            result.Data.As<TestSubject>().b.Should().Be(null);
            result.Data.As<TestSubject>().c.Should().Be(false);
        }

        [Fact]
        public async Task When_SendRequestAsync_With_GetRequestTypeReturnsUnsuccessfulStatusCode_Should_ReturnFailureResponseType()
        {
            // Arrange
            var requestType = HttpRequestType.GET;

            httpClientMock.Setup(x => x.GetAsync(DefaultUrl)).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                ReasonPhrase = "Url not found.",
                Content = new StringContent("")
            });

            // Act
            var result = await Sut().SendRequestAsync<TestSubject>(DefaultUrl, requestType);

            // Assert
            result.ResponseType.Should().Be(HttpResponseType.Failure);
            result.Message.Should().NotBeNullOrWhiteSpace();
            result.Data.Should().Be(null);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task When_SendRequestAsync_With_GetRequestTypeReturnsEmptyContent_Should_ReturnEmptyResponseType(string stringContent)
        {
            // Arrange
            var requestType = HttpRequestType.GET;

            httpClientMock.Setup(x => x.GetAsync(DefaultUrl)).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(stringContent)
            });

            // Act
            var result = await Sut().SendRequestAsync<TestSubject>(DefaultUrl, requestType);

            // Assert
            result.ResponseType.Should().Be(HttpResponseType.Empty);
            result.Message.Should().NotBeNullOrWhiteSpace();
            result.Data.Should().Be(null);
        }

        [Fact]
        public async Task When_SendRequestAsync_With_GetRequestTypeReturnsUndeserializableContent_Should_ReturnUndeserializableResponseType()
        {
            // Arrange
            var requestType = HttpRequestType.GET;

            httpClientMock.Setup(x => x.GetAsync(DefaultUrl)).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(@"Undeserializable content.")
            });

            // Act
            var result = await Sut().SendRequestAsync<TestSubject>(DefaultUrl, requestType);

            // Assert
            result.ResponseType.Should().Be(HttpResponseType.Undeserializable);
            result.Message.Should().NotBeNullOrWhiteSpace();
            result.Data.Should().Be(null);
        }

        [Theory]
        [InlineData(HttpRequestType.PUT)]
        [InlineData(HttpRequestType.POST)]
        public async Task When_SendRequestAsync_With_PutOrPostRequestType_Should_ReturnExpectedResult(HttpRequestType requestType)
        {
            // Arrange
            var payload = new TestSubject
            {
                a = 10,
                b = "",
                c = false
            };

            if (requestType == HttpRequestType.PUT)
            {
                httpClientMock.Setup(x => x.PutAsync(DefaultUrl, payload)).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }
            else
            {
                httpClientMock.Setup(x => x.PostAsync(DefaultUrl, payload)).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }

            // Act
            var result = await Sut().SendRequestAsync(DefaultUrl, requestType, payload);

            // Assert
            result.ResponseType.Should().Be(HttpResponseType.Success);
        }

        [Theory]
        [InlineData(HttpRequestType.PUT)]
        [InlineData(HttpRequestType.POST)]
        public async Task When_SendRequestAsync_With_PutOrPostRequestReturnsUnsuccessfulStatusCode_Should_ReturnFailureResponseType(HttpRequestType requestType)
        {
            // Arrange
            var payload = new TestSubject
            {
                a = 10,
                b = "",
                c = false
            };

            if(requestType == HttpRequestType.PUT)
            {
                httpClientMock.Setup(x => x.PutAsync(DefaultUrl, payload)).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            }
            else
            {
                httpClientMock.Setup(x => x.PostAsync(DefaultUrl, payload)).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            }

            // Act
            var result = await Sut().SendRequestAsync(DefaultUrl, requestType, payload);

            // Assert
            result.ResponseType.Should().Be(HttpResponseType.Failure);
            result.Message.Should().NotBeNullOrWhiteSpace();
            result.Data.Should().Be(null);
        }

        [Fact]
        public async Task When_SendRequestAsync_With_DeleteRequestType_Should_ReturnExpectedResult()
        {
            // Arrange
            var requestType = HttpRequestType.DELETE;

            httpClientMock.Setup(x => x.DeleteAsync(DefaultUrl)).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            });

            // Act
            var result = await Sut().SendRequestAsync<bool>(DefaultUrl, requestType);

            // Assert
            result.ResponseType.Should().Be(HttpResponseType.Success);
            result.Data.Should().BeTrue();
        }

        [Fact]
        public async Task When_SendRequestAsync_With_DeleteRequestReturnsUnsuccessfulStatusCode_Should_ReturnFailureResponseType()
        {
            // Arrange
            var requestType = HttpRequestType.DELETE;

            httpClientMock.Setup(x => x.DeleteAsync(DefaultUrl)).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.NotFound
            });

            // Act
            var result = await Sut().SendRequestAsync<bool>(DefaultUrl, requestType);

            // Assert
            result.ResponseType.Should().Be(HttpResponseType.Failure);
            result.Data.Should().BeFalse();
        }

        internal class TestSubject
        {
            public int a;

            public string b;

            public bool c;
        }

        private Clients.GenericHttpClient Sut()
            => new(httpClientMock.Object);
    }
}