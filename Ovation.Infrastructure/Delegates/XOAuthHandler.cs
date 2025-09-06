using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Ovation.Persistence.Delegates
{
    internal class XOAuthHandler : DelegatingHandler
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _accessToken;
        private readonly string _accessTokenSecret;

        public XOAuthHandler()
        {
            _consumerKey = "LMJYJYOik4q4YYlhStUfEveUJ";//Environment.GetEnvironmentVariable("ConsumerKey");
            _consumerSecret = "C0MTJyLbOJdm4PmAP2f8PGaIjknYuwJ9x01q17Z2GxZandzBL5"; //Environment.GetEnvironmentVariable("ConsumerSecret");
            _accessToken = "1451236072327946252-K2WSjtBmthWqKt3uRLXqwxELyvwz1j"; //Environment.GetEnvironmentVariable("AccessToken");
            _accessTokenSecret = "yF2onaUdTqpl6J6FoNzfLpedemMs1iF4hUSXH5jeirWMF";//Environment.GetEnvironmentVariable("AccessTokenSecret");
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri;
            var method = request.Method.Method.ToUpperInvariant();

            if (method != "POST")
            {
                throw new NotSupportedException("Only POST methods are supported.");
            }

            request.Headers.Remove("Authorization");

            var oauthParams = new Dictionary<string, string>
            {
                {"oauth_consumer_key", _consumerKey },
                { "oauth_nonce", Guid.NewGuid().ToString("N") },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
                { "oauth_token", _accessToken },
                { "oauth_version", "1.0" }
            };


            var allParams = new SortedDictionary<string, string>(oauthParams);

            if (request.Content is FormUrlEncodedContent content)
            {
                var contentString = await content.ReadAsStringAsync();
                var parsed = HttpUtility.ParseQueryString(contentString);
                foreach (string key in parsed)
                {
                    allParams[key] = parsed[key];
                }
            }

            var baseString = BuildBaseString(method, uri, allParams);
            var signingKey = $"{UrlEncode(_consumerSecret)}&{UrlEncode(_accessTokenSecret)}";
            var signature = ComputeHmacSha1(baseString, signingKey);

            oauthParams["oauth_signature"] = signature;

            var authHeader = "OAuth " + string.Join(", ", oauthParams.Select(kvp =>
                $"{kvp.Key}=\"{UrlEncode(kvp.Value)}\""));

            request.Headers.Add("Authorization", authHeader);

            return await base.SendAsync(request, cancellationToken);
        }

        //public string GenerateOAuthHeader(string url, string method, IDictionary<string, string> parameters)
        //{
        //    var nonce = Guid.NewGuid().ToString("N");
        //    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        //    var oauthParams = new SortedDictionary<string, string>
        //{
        //    { "oauth_consumer_key", _consumerKey },
        //    { "oauth_nonce", nonce },
        //    { "oauth_signature_method", "HMAC-SHA1" },
        //    { "oauth_timestamp", timestamp },
        //    { "oauth_token", _accessToken },
        //    { "oauth_version", "1.0" }
        //};

        //    // Merge all parameters (OAuth + request body)
        //    var allParams = oauthParams.Concat(parameters).ToDictionary(k => k.Key, v => v.Value);

        //    // Percent encode all keys/values
        //    string Encode(string s) => Uri.EscapeDataString(s ?? "");

        //    var sortedParams = allParams.OrderBy(k => k.Key).ThenBy(v => v.Value);
        //    var paramString = string.Join("&", sortedParams.Select(kvp => $"{Encode(kvp.Key)}={Encode(kvp.Value)}"));

        //    var signatureBaseString = $"{method.ToUpper()}&{Encode(url)}&{Encode(paramString)}";

        //    var signingKey = $"{Encode(_consumerSecret)}&{Encode(_accessTokenSecret)}";
        //    using var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
        //    var hash = hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
        //    var signature = Convert.ToBase64String(hash);

        //    oauthParams.Add("oauth_signature", signature);

        //    var header = "OAuth " + string.Join(", ", oauthParams.Select(kvp =>
        //        $"{Encode(kvp.Key)}=\"{Encode(kvp.Value)}\""));

        //    return header;
        //}

        private string BuildBaseString(string method, Uri uri, SortedDictionary<string, string> parameters)
        {
            var normalizedParams = string.Join("&", parameters.Select(kvp =>
                $"{UrlEncode(kvp.Key)}={UrlEncode(kvp.Value)}"));

            var normalizedUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
            return $"{method}&{UrlEncode(normalizedUrl)}&{UrlEncode(normalizedParams)}";
        }

        private string ComputeHmacSha1(string data, string key)
        {
            using var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(key));
            var hash = hasher.ComputeHash(Encoding.ASCII.GetBytes(data));
            return Convert.ToBase64String(hash);
        }

        private string UrlEncode(string value)
        {
            return HttpUtility.UrlEncode(value)
                             .Replace("+", "%20")
                             .Replace("*", "%2A")
                             .Replace("%7E", "~");
        }
    }
}
