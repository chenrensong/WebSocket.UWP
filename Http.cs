using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            public Header(String line)
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
            String method;
            String requestURI;
            String httpVersion;

            public String getMethod()
            {
                return method;
            }

            public String getRequestURI()
            {
                return requestURI;
            }

            public String getHttpVersion()
            {
                return httpVersion;
            }

            public RequestLine(String method, String requestURI, String httpVersion)
            {
                this.method = method;
                this.requestURI = requestURI;
                this.httpVersion = httpVersion;
            }

            public String toString()
            {
                return method + " " + requestURI + " " + httpVersion;
            }

            public RequestLine(String line)
            {
                StringTokenizer st = new StringTokenizer(line);
                method = st.nextToken();
                requestURI = st.nextToken();
                httpVersion = st.nextToken();
            }
        }

        public class StatusLine
        {
            public String getHttpVersion()
            {
                return httpVersion;
            }

            public int getStatusCode()
            {
                return statusCode;
            }

            public String getReasonPhrase()
            {
                return reasonPhrase;
            }

            String httpVersion;
            int statusCode;
            String reasonPhrase;

            public String toString()
            {
                return httpVersion + " " + statusCode + " " + reasonPhrase;
            }

            public StatusLine(String httpVersion, int statusCode, String reasonPhrase)
            {
                if (statusCode < 100 || statusCode > 999)
                    throw new Exception("status code must be XXX");
                this.httpVersion = httpVersion;
                this.statusCode = statusCode;
                this.reasonPhrase = reasonPhrase;
            }

            public StatusLine(String line)
            {
                int colon1 = line.IndexOf(' ');
                if (colon1 == -1)
                    throw new Exception("wrong status line - no the 1st space");
                httpVersion = line.Substring(0, colon1);
                int colon2 = line.IndexOf(' ', colon1 + 1);
                if (colon2 == -1)
                    throw new Exception("wrong status line - no the 2nd space");
                String strStatusCode = line.Substring(colon1 + 1, colon2 - colon1 - 1);
                statusCode = Convert.ToInt32(strStatusCode);
                if (statusCode < 100 || statusCode > 999)
                    throw new Exception("status code must be XXX");
                reasonPhrase = line.Substring(colon2 + 1);
            }
        }
    }

}
