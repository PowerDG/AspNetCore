﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class HttpClientDeviceAuthorizationExtensionsResponseTests
    {
        const string Endpoint = "http://server/device";

        [Fact]
        public async Task Valid_protocol_response_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_device_authorization_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = Endpoint,
                ClientId = "client"
            });

            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);

            response.DeviceCode.Should().Be("GMMhmHCXhWEzkobqIHGG_EnNYYsAkukHspeYUk9E8");
            response.UserCode.Should().Be("WDJB-MJHT");
            response.VerificationUri.Should().Be("https://www.example.com/device");
            response.VerificationUriComplete.Should().Be("https://www.example.com/device?user_code=WDJB-MJHT");

            response.ExpiresIn.Should().Be(1800);
            response.Interval.Should().Be(10);
        }

        [Fact]
        public async Task Valid_protocol_error_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("failure_device_authorization_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.BadRequest);

            var client = new HttpClient(handler);
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = Endpoint,
                ClientId = "client"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Protocol);
            response.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Error.Should().Be("error");
            response.ErrorDescription.Should().Be("error_description");
            response.TryGet("custom").Should().Be("custom");
        }

        [Fact]
        public async Task Malformed_response_document_should_be_handled_correctly()
        {
            var document = "invalid";
            var handler = new NetworkHandler(document, HttpStatusCode.OK);

            var client = new HttpClient(handler);
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = Endpoint,
                ClientId = "client"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Raw.Should().Be("invalid");
            response.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task Exception_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(new Exception("exception"));

            var client = new HttpClient(handler);
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = Endpoint,
                ClientId = "client"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Exception);
            response.Error.Should().Be("exception");
            response.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task Http_error_should_be_handled_correctly()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = Endpoint,
                ClientId = "client"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("not found");
        }

        [Fact]
        public async Task Http_error_with_non_json_content_should_be_handled_correctly()
        {
            var handler = new NetworkHandler("not_json", HttpStatusCode.Unauthorized);

            var client = new HttpClient(handler);
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = Endpoint,
                ClientId = "client"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Error.Should().Be("Unauthorized");
            response.Raw.Should().Be("not_json");
        }

        [Fact]
        public async Task Http_error_with_json_content_should_be_handled_correctly()
        {
            var content = new
            {
                foo = "foo",
                bar = "bar"
            };

            var handler = new NetworkHandler(JsonConvert.SerializeObject(content), HttpStatusCode.Unauthorized);

            var client = new HttpClient(handler);
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = Endpoint,
                ClientId = "client"
            });

            response.IsError.Should().BeTrue();
            response.ErrorType.Should().Be(ResponseErrorType.Http);
            response.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Error.Should().Be("Unauthorized");

            response.Json.TryGetString("foo").Should().Be("foo");
            response.Json.TryGetString("bar").Should().Be("bar");
        }
    }
}