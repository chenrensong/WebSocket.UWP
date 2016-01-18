using System.Collections.Generic;

namespace WebSocket4UWP.Internal
{
    internal sealed class Error
    {
        internal const string HandshakeInvalidSecWebSocketAccept = "Handshake failed: Invalid Sec-WebSocket-Accept header.";
        internal const string HandshakeInvalidStatusCode = "Handshake failed: Invalid status sode.";
        internal const string HandshakeVersionNotSupported = "Handshake failed: Server version not supported.";
        internal const string None = "None.";
        internal const string CompressedNonDataFrame = "A non data frame is compressed.";
        internal const string ExtensionsAlreadyRegistered = "An web socket extension with the same name is already registered:";
        internal const string FragmentedControlFrame = "A control frame is fragmented.";
        internal const string InvalidResponseLine = "Invalid response line:";
        internal const string InvalidScheme = "Invalid scheme:";
        internal const string InvalidState = "Invalid state:";
        internal const string MustNotBeNullOrEmpty = "Must not be null or empty.";
        internal const string MustNotContainAFragment = "Must not contain a fragment.";
        internal const string NoHeaderLines = "No header lines.";
        internal const string NotAnAbsoluteUri = "Not an absolute uri.";
        internal const string PayloadLengthControlFrame = "The payload data length of a control frame is greater than 125 bytes.";
        internal const string SecureConnectionsAreNotYetSupported = "Secure connections are not yet supported.";


    }
}
