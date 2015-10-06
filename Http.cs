using System;
using WebSocket4UWP.ToolBox;

namespace WebSocket4UWP
{
    public class Http
    {

        // message-header = field-name ":" [ field-value ]
        public class Header
        {
            private string headerName;
            private string headerValue;

            public Header(string headerName, string headerValue)
            {
                this.headerName = headerName;
                this.headerValue = headerValue;
            }

            public Header(string line)
            {
                int colon = line.IndexOf(':');
                if (colon == -1)
                {
                    throw new Exception("http header without ':', line=" + line);
                }
                headerName = line.Substring(0, colon).Trim();
                headerValue = line.Substring(colon + 1).Trim();
            }

            public override string ToString()
            {
                return headerName + ": " + headerValue;
            }

            public string HeaderName
            {
                get
                {
                    return headerName;
                }
            }

            public string HeaderValue
            {
                get
                {
                    return headerValue;
                }
            }
        }

        // Method SP Request-URI SP HTTP-Version CRLF
        public class RequestLine
        {
            private string method;
            private string requestURI;
            private string httpVersion;

            public string Method
            {
                get
                {
                    return method;
                }
            }



            public string RequestURI
            {
                get
                {
                    return requestURI;
                }
            }

            public string HttpVersion
            {
                get
                {
                    return httpVersion;
                }
            }

            public RequestLine(string method, string requestURI, string httpVersion)
            {
                this.method = method;
                this.requestURI = requestURI;
                this.httpVersion = httpVersion;
            }

            public override string ToString()
            {
                return method + " " + requestURI + " " + httpVersion;
            }

            public RequestLine(string line)
            {
                StringTokenizer st = new StringTokenizer(line);
                method = st.NextToken();
                requestURI = st.NextToken();
                httpVersion = st.NextToken();
            }
        }

        public class StatusLine
        {
            public string HttpVersion
            {
                get
                {
                    return httpVersion;
                }
            }

            public int StatusCode
            {
                get
                {
                    return statusCode;
                }
            }

            public string ReasonPhrase
            {
                get
                {
                    return reasonPhrase;
                }
            }

            private string httpVersion;
            private int statusCode;
            private string reasonPhrase;

            public override string ToString()
            {
                return httpVersion + " " + statusCode + " " + reasonPhrase;
            }
   
            public StatusLine(string httpVersion, int statusCode, string reasonPhrase)
            {
                if (statusCode < 100 || statusCode > 999)
                    throw new Exception("status code must be XXX");
                this.httpVersion = httpVersion;
                this.statusCode = statusCode;
                this.reasonPhrase = reasonPhrase;
            }

            public StatusLine(string line)
            {
                int colon1 = line.IndexOf(' ');
                if (colon1 == -1)
                    throw new Exception("wrong status line - no the 1st space");
                httpVersion = line.Substring(0, colon1);
                int colon2 = line.IndexOf(' ', colon1 + 1);
                if (colon2 == -1)
                    throw new Exception("wrong status line - no the 2nd space");
                string strStatusCode = line.Substring(colon1 + 1, colon2 - colon1 - 1);
                statusCode = Convert.ToInt32(strStatusCode);
                if (statusCode < 100 || statusCode > 999)
                    throw new Exception("status code must be XXX");
                reasonPhrase = line.Substring(colon2 + 1);
            }
        }
    }

}
