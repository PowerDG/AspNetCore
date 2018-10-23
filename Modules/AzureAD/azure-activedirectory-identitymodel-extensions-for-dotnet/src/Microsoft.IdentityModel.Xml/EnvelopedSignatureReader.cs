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
using System.Xml;
using static Microsoft.IdentityModel.Logging.LogHelper;

namespace Microsoft.IdentityModel.Xml
{
    /// <summary>
    /// Wraps a <see cref="XmlReader"/> pointing to a root element of XML that may contain a signature.
    /// If a Signature element is found, a <see cref="Signature"/> will be populated and <see cref="SignedInfo.References"/> will
    /// have <see cref="XmlTokenStream"/> set for future validation.
    /// </summary>
    public class EnvelopedSignatureReader : DelegatingXmlDictionaryReader
    {
        private DSigSerializer _dsigSerializer = DSigSerializer.Default;
        private int _elementCount;
        private XmlTokenStreamReader _tokenStreamReader;

        /// <summary>
        /// Initializes an instance of <see cref="EnvelopedSignatureReader"/>
        /// </summary>
        /// <param name="reader">a <see cref="XmlReader"/> pointing to XML that may contain an enveloped signature.</param>
        /// <remarks>If a &lt;Signature> element is found, the <see cref="Signature"/> will be set.</remarks>
        /// <exception cref="ArgumentNullException">if <paramref name="reader"/> is null.</exception>
        public EnvelopedSignatureReader(XmlReader reader)
        {
            if (reader == null)
                throw LogArgumentNullException(nameof(reader));

            _tokenStreamReader = new XmlTokenStreamReader(CreateDictionaryReader(reader));
            InnerReader  = _tokenStreamReader;
        }

        /// <summary>
        /// Gets or sets the <see cref="DSigSerializer"/> to use when reading XmlDSig elements.
        /// </summary>
        /// <exception cref="ArgumentNullException">if 'value' is null.</exception>
        public DSigSerializer Serializer
        {
            get => _dsigSerializer;
            set => _dsigSerializer = value ?? throw LogArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Called after the root element has been completely read.
        /// Attaches a <see cref="XmlTokenStream"/> to the first Reference for future processing if
        /// a signature was found.
        /// </summary>
        protected virtual void OnEndOfRootElement()
        {
            if (Signature != null)
                Signature.SignedInfo.References[0].TokenStream = _tokenStreamReader.TokenStream;
        }

        /// <summary>
        /// Keeps track of the XML Element count. If a signature is detected it is read.
        /// </summary>
        /// <returns>'true' if the next node was read successfully; 'false' if there are no more nodes.</returns>
        /// <exception cref="XmlReadException">if more than one signature is found.</exception>
        /// <exception cref="XmlReadException">if a &lt;Reference> element was not found in the &lt;SignedInfo>.</exception>
        public override bool Read()
        {
            if ((NodeType == XmlNodeType.Element) && (!base.IsEmptyElement))
                _elementCount++;

            if (NodeType == XmlNodeType.EndElement)
            {
                _elementCount--;
                if (_elementCount == 0)
                    OnEndOfRootElement();
            }

            bool result = InnerReader.Read();
            if (result
                && InnerReader.IsLocalName(XmlSignatureConstants.Elements.Signature)
                && InnerReader.IsNamespaceUri(XmlSignatureConstants.Namespace))
            {
                if (Signature != null)
                    throw XmlUtil.LogReadException(LogMessages.IDX30019);

                Signature = Serializer.ReadSignature(InnerReader);
            }

            return result;
        }

        /// <summary>
        /// Gets the <see cref="Xml.Signature"/> that was found inside the XML.
        /// </summary>
        /// <remarks><see cref="Xml.Signature"/> may be null.</remarks>
        public Signature Signature
        {
            get;
            protected set;
        }
    }
}
