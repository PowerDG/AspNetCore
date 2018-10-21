// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.DataProtection
{
    /// <summary>
    /// Provides global options for the Data Protection system.
    /// </summary>
    public class DataProtectionOptions
    {
        /// <summary>
        /// An identifier that uniquely discriminates this application from all other
        /// applications on the machine. The discriminator value is implicitly included
        /// in all protected payloads generated by the data protection system to isolate
        /// multiple logical applications that all happen to be using the same key material.
        /// </summary>
        /// <remarks>
        /// If two different applications need to share protected payloads, they should
        /// ensure that this property is set to the same value across both applications.
        /// </remarks>
        public string ApplicationDiscriminator { get; set; }
    }
}
