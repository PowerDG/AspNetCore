//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.IdentityModel.Tests;
using Xunit;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Microsoft.IdentityModel.Protocols.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpDocumentRetrieverTests
    {
        [Fact]
        public void Constructors()
        {
            HttpDocumentRetriever docRetriever = new HttpDocumentRetriever();
            Assert.Throws<ArgumentNullException>(() => new HttpDocumentRetriever(null));
        }

        [Fact]
        public void Defaults()
        {
        }

        [Fact]
        public void GetSets()
        {
            HttpDocumentRetriever docRetriever = new HttpDocumentRetriever();
            Type type = typeof(HttpDocumentRetriever);
            PropertyInfo[] properties = type.GetProperties();
            if (properties.Length != 1)
                Assert.True(true, "Number of properties has changed from 1 to: " + properties.Length + ", adjust tests");

            GetSetContext context =
                new GetSetContext
                {
                    PropertyNamesAndSetGetValue = new List<KeyValuePair<string, List<object>>>
                    {
                        new KeyValuePair<string, List<object>>("RequireHttps", new List<object>{true, false, true}),
                    },
                    Object = docRetriever,
                };

            TestUtilities.GetSet(context);
            TestUtilities.AssertFailIfErrors("HttpDocumentRetrieverTests_GetSets", context.Errors);
        }

        [Theory, MemberData(nameof(GetMetadataTheoryData))]
        public void GetMetadataTest(DocumentRetrieverTheoryData theoryData)
        {
            TestUtilities.WriteHeader($"{this}.GetMetadataTest", theoryData);
            try
            {
                string doc = theoryData.DocumentRetriever.GetDocumentAsync(theoryData.Address, CancellationToken.None).Result;
                Assert.NotNull(doc);
                theoryData.ExpectedException.ProcessNoException();
            }
            catch (AggregateException aex)
            {
                aex.Handle((x) =>
                {
                    theoryData.ExpectedException.ProcessException(x);
                    return true;
                });
            }
        }

        public static TheoryData<DocumentRetrieverTheoryData> GetMetadataTheoryData
        {
            get
            {
                var theoryData = new TheoryData<DocumentRetrieverTheoryData>();

                var documentRetriever = new HttpDocumentRetriever();
                theoryData.Add(new DocumentRetrieverTheoryData
                {
                    Address = null,
                    DocumentRetriever = documentRetriever,
                    ExpectedException = ExpectedException.ArgumentNullException(),
                    First = true,
                    TestId = "Address NULL"
                });

                theoryData.Add(new DocumentRetrieverTheoryData
                {
                    Address = "OpenIdConnectMetadata.json",
                    DocumentRetriever = documentRetriever,
                    ExpectedException = new ExpectedException(typeof(ArgumentException), "IDX20108:"),
                    TestId = "Require https, using file: 'OpenIdConnectMetadata.json'"
                });

                theoryData.Add(new DocumentRetrieverTheoryData
                {
                    Address = "httpss://OpenIdConnectMetadata.json",
                    DocumentRetriever = documentRetriever,
                    ExpectedException = new ExpectedException(typeof(ArgumentException), "IDX20108:"),
                    TestId = "Require https, Address: 'httpss://OpenIdConnectMetadata.json'"
                });

                theoryData.Add(new DocumentRetrieverTheoryData
                {
                    Address = "https://login.microsoftonline.com/common/.well-known/openid-configuration",
                    DocumentRetriever = documentRetriever,
                    TestId = "AAD common: https"
                });

                theoryData.Add(new DocumentRetrieverTheoryData
                {
                    Address = "HTTPS://login.microsoftonline.com/common/.well-known/openid-configuration",
                    DocumentRetriever = documentRetriever,
                    TestId = "AAD common: HTTPS"
                });

                documentRetriever = new HttpDocumentRetriever() { RequireHttps = false };
                theoryData.Add(new DocumentRetrieverTheoryData
                {
                    Address = "OpenIdConnectMetadata.json",
                    DocumentRetriever = documentRetriever,
                    ExpectedException = new ExpectedException(typeof(IOException), "IDX20804:", typeof(InvalidOperationException)),
                    TestId = "RequireHttps == false, Address: 'OpenIdConnectMetadata.json'"
                });

                theoryData.Add(new DocumentRetrieverTheoryData
                {
                    Address = "https://login.microsoftonline.com/common/FederationMetadata/2007-06/FederationMetadata.xml",
                    DocumentRetriever = documentRetriever,
                    TestId = "AAD common: WsFed"
                });

                return theoryData;
            }
        }
    }

    public class DocumentRetrieverTheoryData : TheoryDataBase
    {
        public string Address { get; set; }

        public IDocumentRetriever DocumentRetriever { get; set; }

        public override string ToString()
        {
            return $"{TestId}, {Address}, {ExpectedException}";
        }
    }
}

#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
