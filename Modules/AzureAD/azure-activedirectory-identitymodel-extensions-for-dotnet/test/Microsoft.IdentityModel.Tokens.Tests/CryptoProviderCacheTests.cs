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
using Microsoft.IdentityModel.Tests;
using Xunit;

using ALG = Microsoft.IdentityModel.Tokens.SecurityAlgorithms;
using EE = Microsoft.IdentityModel.Tests.ExpectedException;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Microsoft.IdentityModel.Tokens.Tests
{
    /// <summary>
    /// Tests for CryptoProviderCache with SignatureProviders
    /// </summary>
    public class CryptoProviderCacheTests
    {
        /// <summary>
        /// Tests that a cache key generated from a <see cref="SignatureProvider"/> or a set of components are equal.
        /// </summary>
        /// <param name="theoryData"></param>
        [Theory, MemberData(nameof(GetCacheKeyTheoryData))]
        public void GetCacheKey(CryptoProviderCacheTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.GetCacheKey", theoryData);

            try
            {
                var keyFromComponents = theoryData.InMemoryCryptoProviderCachePublic.GetCacheKeyPublic(theoryData.SecurityKey, theoryData.Algorithm, theoryData.TypeofProvider);
                var keyFromProvider = theoryData.InMemoryCryptoProviderCachePublic.GetCacheKeyPublic(theoryData.SignatureProvider);
                if (keyFromProvider.Equals(keyFromComponents) != theoryData.ShouldCacheKeysMatch)
                    context.Diffs.Add($"theoryData.CacheKeysMatch:{Environment.NewLine}keyFromComponents: '{keyFromComponents}'{Environment.NewLine}!={Environment.NewLine}keyFromProvider:    '{keyFromProvider}'.");

                theoryData.ExpectedException.ProcessNoException(context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<CryptoProviderCacheTheoryData> GetCacheKeyTheoryData
        {
            get
            {
                var theoryData = new TheoryData<CryptoProviderCacheTheoryData>
                {
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = ALG.RsaSha384,
                        ExpectedException = EE.ArgumentNullException(),
                        First = true,
                        SecurityKey = Default.AsymmetricSigningKey,
                        TestId = "SignatureProviderNULL",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = Default.AsymmetricSigningAlgorithm,
                        ExpectedException = EE.ArgumentNullException(),
                        TestId = "SecurityKeyNULL",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        ExpectedException = EE.ArgumentNullException(),
                        SecurityKey = Default.AsymmetricSigningKey,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, Default.AsymmetricSigningAlgorithm, true),
                        TestId = "AlgorithmNULL",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = string.Empty,
                        ExpectedException = EE.ArgumentNullException(),
                        SecurityKey = Default.AsymmetricSigningKey,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, Default.AsymmetricSigningAlgorithm, true),
                        TestId = "AlgorithmString.Empty",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = ALG.RsaSha384,
                        ExpectedException = EE.ArgumentNullException(),
                        SecurityKey = Default.AsymmetricSigningKey,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, Default.AsymmetricSigningAlgorithm, true),
                        TestId = "TypeofProviderNULL"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = ALG.RsaSha384,
                        ExpectedException = EE.ArgumentNullException(),
                        SecurityKey = Default.AsymmetricSigningKey,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, Default.AsymmetricSigningAlgorithm, true),
                        TestId = "TypeofProviderString.Empty",
                        TypeofProvider = string.Empty
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = Default.AsymmetricSigningAlgorithm,
                        SecurityKey = Default.AsymmetricSigningKey,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, Default.AsymmetricSigningAlgorithm, true),
                        TestId = "AsymmetricSignatureProvider1",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = Default.AsymmetricSigningAlgorithm,
                        SecurityKey = Default.AsymmetricSigningKey,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, Default.AsymmetricSigningAlgorithm, false),
                        TestId = "AsymmetricSignatureProvider2",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = ALG.RsaSha384,
                        SecurityKey = Default.AsymmetricSigningKey,
                        ShouldCacheKeysMatch = false,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, ALG.RsaSha256, false),
                        TestId = "AlgorithmDifferent",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = ALG.HmacSha512,
                        SecurityKey = Default.AsymmetricSigningKey,
                        ShouldCacheKeysMatch = false,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, Default.AsymmetricSigningAlgorithm, false),
                        TestId = "TypeofProviderDifferent",
                        TypeofProvider = typeof(SymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = ALG.RsaSha384,
                        SecurityKey = Default.AsymmetricSigningKey,
                        ShouldCacheKeysMatch = true,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, ALG.RsaSha384, false),
                        TestId = "WillCreateSignaturesDifferent",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    }
                };

                return theoryData;
            }
        }

        [Theory, MemberData(nameof(TryAddTheoryData))]
        public void TryAdd(CryptoProviderCacheTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.TryAdd", theoryData);

            try
            {
                var added = theoryData.CryptoProviderCache.TryAdd(theoryData.SignatureProvider);
                if (theoryData.Added != added)
                    context.Diffs.Add($"theoryData.Added:'{theoryData.Added}' != theoryData.CryptoProviderCache.TryAdd(theoryData.SignatureProvider)");

                theoryData.ExpectedException.ProcessNoException(context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<CryptoProviderCacheTheoryData> TryAddTheoryData
        {
            get
            {
                var sharedCache = new InMemoryCryptoProviderCache();
                var theoryData = new TheoryData<CryptoProviderCacheTheoryData>
                {
                    new CryptoProviderCacheTheoryData
                    {
                        ExpectedException = EE.ArgumentNullException(),
                        First = true,
                        TestId = "SignatureProviderNULL"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Added = true,
                        CryptoProviderCache = sharedCache,
                        SignatureProvider = new SymmetricSignatureProvider(Default.SymmetricSigningKey256, ALG.HmacSha256),
                        TestId = "SymmetricSignatureProviderAdded"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Added = false,
                        CryptoProviderCache = sharedCache,
                        SignatureProvider = new SymmetricSignatureProvider(Default.SymmetricSigningKey256, ALG.HmacSha256),
                        TestId = "SymmetricSignatureProviderAddedSecondTime"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Added = true,
                        CryptoProviderCache = sharedCache,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, Default.AsymmetricSigningAlgorithm),
                        TestId = "AsymmetricSignatureProviderAdded"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Added = false,
                        CryptoProviderCache = sharedCache,
                        SignatureProvider = new AsymmetricSignatureProvider(Default.AsymmetricSigningKey, Default.AsymmetricSigningAlgorithm),
                        TestId = "AsymmetricSignatureProviderAddedSecondTime"
                    }
                };

                return theoryData;
            }
        }

        /// <summary>
        /// Tests that a cache key generated from a <see cref="SignatureProvider"/> or a set of components are equal.
        /// </summary>
        /// <param name="theoryData"></param>
        [Theory, MemberData(nameof(TryGetSignatureProviderTheoryData))]
        public void TryGetSignatureProvider(CryptoProviderCacheTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.TryGetSignatureProvider", theoryData);

            try
            {
                var providerFound = theoryData.CryptoProviderCache.TryGetSignatureProvider(theoryData.SecurityKey, theoryData.Algorithm, theoryData.TypeofProvider, theoryData.WillCreateSignatures, out SignatureProvider signatureProvider);
                if (theoryData.Found != providerFound)
                    context.Diffs.Add($"theoryData.Found: '{theoryData.Found}' != theoryData.CryptoProviderCache.TryGetSignatureProvider: '{providerFound}'");
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<CryptoProviderCacheTheoryData> TryGetSignatureProviderTheoryData
        {
            get
            {
                var theoryData = new TheoryData<CryptoProviderCacheTheoryData>
                {
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = ALG.RsaSha256,
                        ExpectedException = EE.ArgumentNullException(),
                        First = true,
                        TestId = "SecurityKeyNULL",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        ExpectedException = EE.ArgumentNullException(),
                        SecurityKey = Default.AsymmetricSigningKey,
                        TestId = "AlgorithmNULL",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = string.Empty,
                        ExpectedException = EE.ArgumentNullException(),
                        SecurityKey = Default.AsymmetricSigningKey,
                        TestId = "AlgorithmString.Empty",
                        TypeofProvider = typeof(AsymmetricSignatureProvider).ToString()
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = ALG.RsaSha256,
                        ExpectedException = EE.ArgumentNullException(),
                        SecurityKey = Default.AsymmetricSigningKey,
                        TestId = "TypeofProviderNULL"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Algorithm = ALG.RsaSha256,
                        ExpectedException = EE.ArgumentNullException(),
                        SecurityKey = Default.AsymmetricSigningKey,
                        TestId = "TypeofProviderString.Empty",
                        TypeofProvider = string.Empty
                    },
                };

                // Test different methods of finding
                var cryptoProviderCache = new InMemoryCryptoProviderCache();
                var derivedKey = new DerivedSecurityKey("kid", 256);
                var derivedKey2 = new DerivedSecurityKey("kid2", 256);
                var signatureProvider = new CustomSignatureProvider(derivedKey, ALG.RsaSha256, true);
                cryptoProviderCache.TryAdd(signatureProvider);
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    Algorithm = ALG.RsaSha256,
                    CryptoProviderCache = cryptoProviderCache,
                    Found = true,
                    SecurityKey = derivedKey,
                    TestId = "SignatureProviderFound",
                    TypeofProvider = typeof(CustomSignatureProvider).ToString(),
                    WillCreateSignatures = true
                });
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    Algorithm = ALG.RsaSha256,
                    CryptoProviderCache = cryptoProviderCache,
                    Found = false,
                    SecurityKey = derivedKey,
                    TestId = "SignatureProviderTypeMismatch",
                    TypeofProvider = typeof(SignatureProviderForTestingCache).ToString(),
                    WillCreateSignatures = true
                });
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    Algorithm = ALG.RsaSha256,
                    CryptoProviderCache = cryptoProviderCache,
                    Found = false,
                    SecurityKey = derivedKey2,
                    TestId = "SignatureProviderKeyIdMismatch",
                    TypeofProvider = typeof(CustomSignatureProvider).ToString(),
                    WillCreateSignatures = true
                });
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    Algorithm = ALG.RsaSha384,
                    CryptoProviderCache = cryptoProviderCache,
                    Found = false,
                    SecurityKey = derivedKey,
                    TestId = "SignatureProviderAlgorithmMismatch",
                    TypeofProvider = typeof(CustomSignatureProvider).ToString(),
                    WillCreateSignatures = true
                });
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    Algorithm = ALG.RsaSha256,
                    CryptoProviderCache = cryptoProviderCache,
                    Found = false,
                    SecurityKey = derivedKey,
                    TestId = "SignatureProviderWillCreateSignaturesMismatch",
                    TypeofProvider = typeof(CustomSignatureProvider).ToString(),
                    WillCreateSignatures = false
                });

                return theoryData;
            }
        }

        [Theory, MemberData(nameof(TryRemoveTheoryData))]
        public void TryRemove(CryptoProviderCacheTheoryData theoryData)
        {
            var context = TestUtilities.WriteHeader($"{this}.TryRemove", theoryData);

            try
            {
                if (theoryData.Removed != theoryData.CryptoProviderCache.TryRemove(theoryData.SignatureProvider))
                    context.Diffs.Add($"theoryData.Removed:'{theoryData.Removed}' != theoryData.CryptoProviderCache.TryRemove(theoryData.SignatureProvider)");

                theoryData.ExpectedException.ProcessNoException(context);
            }
            catch (Exception ex)
            {
                theoryData.ExpectedException.ProcessException(ex, context);
            }

            TestUtilities.AssertFailIfErrors(context);
        }

        public static TheoryData<CryptoProviderCacheTheoryData> TryRemoveTheoryData
        {
            get
            {
                var theoryData =  new TheoryData<CryptoProviderCacheTheoryData>
                {
                    new CryptoProviderCacheTheoryData
                    {
                        ExpectedException = EE.ArgumentNullException(),
                        First = true,
                        TestId = "SignatureProviderNULL"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Removed = false,
                        SignatureProvider = new SymmetricSignatureProvider(Default.SymmetricSigningKey256, ALG.HmacSha256),
                        TestId = "EmptyCache"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Removed = false,
                        SignatureProvider = new CustomSignatureProvider(new DerivedSecurityKey(null, 256), ALG.HmacSha256),
                        TestId = "KeyIdnull"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Removed = false,
                        SignatureProvider = new CustomSignatureProvider(new DerivedSecurityKey(string.Empty, 256), ALG.HmacSha256),
                        TestId = "KeyIdString.empty"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Removed = false,
                        SignatureProvider = new CustomSignatureProvider(new DerivedSecurityKey("kid", 256), ALG.HmacSha256),
                        TestId = "CryptoProviderFactorynull"
                    },
                    new CryptoProviderCacheTheoryData
                    {
                        Removed = false,
                        SignatureProvider = new CustomSignatureProvider(new DerivedSecurityKey("kid", 256), ALG.HmacSha256){CryptoProviderCache = new InMemoryCryptoProviderCache()},
                        TestId = "CryptoProvider!=ReferenceEquals"
                    }
                };

                // SignatureProvider found and removed
                var cryptoProviderCache = new InMemoryCryptoProviderCache();
                var signatureProvider = new CustomSignatureProvider(new DerivedSecurityKey("kid", 256), ALG.HmacSha256);
                cryptoProviderCache.TryAdd(signatureProvider);
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    CryptoProviderCache = cryptoProviderCache,
                    SignatureProvider = signatureProvider,
                    TestId = "SignatureProviderRemoved"
                });
                
                // SignatureProvider was removed above, so should not be found
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    CryptoProviderCache = cryptoProviderCache,
                    Removed = false,
                    SignatureProvider = signatureProvider,
                    TestId = "SignatureProviderWasNotRemoved"
                });

                cryptoProviderCache = new InMemoryCryptoProviderCache();
                signatureProvider = new CustomSignatureProvider(new DerivedSecurityKey("kid", 256), ALG.HmacSha256);
                cryptoProviderCache.TryAdd(signatureProvider);

                // signatureprovider with different algorithm
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    CryptoProviderCache = cryptoProviderCache,
                    Removed = false,
                    SignatureProvider = new CustomSignatureProvider(new DerivedSecurityKey("kid", 256), ALG.HmacSha384),
                    TestId = "AlgorithmNotMatched"
                });

                // signatureprovider with different kid
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    CryptoProviderCache = cryptoProviderCache,
                    Removed = false,
                    SignatureProvider = new CustomSignatureProvider(new DerivedSecurityKey("kid2", 256), ALG.HmacSha256),
                    TestId = "KeyIdNotMatched"
                });

                // signatureprovider with different type, the remove will fail
                signatureProvider = new CustomSignatureProvider(new DerivedSecurityKey("kid", 256), ALG.HmacSha256);
                cryptoProviderCache.TryAdd(signatureProvider);
                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    CryptoProviderCache = cryptoProviderCache,
                    Removed = false,
                    SignatureProvider = new SignatureProviderForTestingCache(new DerivedSecurityKey("kid", 256), ALG.HmacSha256) { CryptoProviderCache = cryptoProviderCache },
                    TestId = "SignatureProviderTypeNotMatched"
                });

                theoryData.Add(new CryptoProviderCacheTheoryData
                {
                    CryptoProviderCache = cryptoProviderCache,
                    Removed = false,
                    SignatureProvider = new CustomSignatureProvider(new DerivedSecurityKey("kid", 256), ALG.HmacSha256, false) { CryptoProviderCache = cryptoProviderCache },
                    TestId = "SignatureProviderTypeMatchedWillCreateSignaturesDifferent"
                });

                return theoryData;
            }
        }
    }

    public class SignatureProviderForTestingCache : CustomSignatureProvider
    {
        public SignatureProviderForTestingCache(SecurityKey key, string algorithm)
            : base(key, algorithm)
        {
        }
    }

    public class CryptoProviderCacheTheoryData : TheoryDataBase
    {
        public bool Added { get; set; } = true;

        public string Algorithm { get; set; }

        public CryptoProviderCache CryptoProviderCache { get; set; } = new InMemoryCryptoProviderCache();

        public bool Found { get; set; }

        public InMemoryCryptoProviderCachePublic InMemoryCryptoProviderCachePublic { get; set; } = new InMemoryCryptoProviderCachePublic();

        public bool Removed { get; set; } = true;

        public SecurityKey SecurityKey { get; set; }

        public bool ShouldCacheKeysMatch { get; set; } = true;

        public SignatureProvider SignatureProvider { get; set; }

        public string TypeofProvider { get; set; }

        public bool WillCreateSignatures { get; set; }
    }

    public class InMemoryCryptoProviderCachePublic : InMemoryCryptoProviderCache
    {
        public string GetCacheKeyPublic(SignatureProvider signatureProvider)
        {
            return base.GetCacheKey(signatureProvider);
        }

        public string GetCacheKeyPublic(SecurityKey securityKey, string algorithm, string typeofProvider)
        {
            return base.GetCacheKey(securityKey, algorithm, typeofProvider);
        }
    }
}

#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
