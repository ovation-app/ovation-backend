using CoreTweet;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using Ovation.Application.Constants;
using Ovation.Application.DTOs.X;
using Ovation.Application.DTOs.X.AiResponse;
using Ovation.Domain.Entities;
using Ovation.Persistence.Data;
using Ovation.Persistence.Observability.Interface;
using Ovation.Persistence.Services.Apis;
using System.Text.Json;
using Type = Ovation.Application.DTOs.SignUp.Type;

namespace Ovation.Persistence.Services
{
    internal class XService(IHttpClientFactory _factory, OvationDbContext _context, IX _xApi, ISentryService _sentry)
    {
        private readonly string _consumerKey = Environment.GetEnvironmentVariable("X_CONSUMER_KEY") ?? "";
        private readonly string _consumerSecret = Environment.GetEnvironmentVariable("X_CONSUMER_SECRET") ?? "";
        private readonly string _accessToken = Environment.GetEnvironmentVariable("X_ACCESS_TOKEN") ?? "";
        private readonly string _accessTokenSecret = Environment.GetEnvironmentVariable("X_ACCESS_TOKEN_SECRET") ?? "";
        private readonly string _modelApiKey = Environment.GetEnvironmentVariable("GOOGLE_AI_MODEL_API_KEY") ?? "";
        private readonly string _model = Environment.GetEnvironmentVariable("GOOGLE_AI_MODEL") ?? "";
        private const int limit = 20;

        internal async Task<bool> PostCommentAsync(string tweetId, string comment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_consumerKey) ||
                   string.IsNullOrWhiteSpace(_consumerSecret) ||
                   string.IsNullOrWhiteSpace(_accessToken) ||
                   string.IsNullOrWhiteSpace(_accessTokenSecret))
                {
                    throw new InvalidOperationException("X API credentials are not set.");
                }

                if (string.IsNullOrWhiteSpace(tweetId) || string.IsNullOrWhiteSpace(comment))
                {
                    return false; // Invalid input
                }

                var request = new TweetRequest
                {
                    Text = comment,
                    Reply = new Reply { InReplyToTweetId = tweetId }
                };

                var client = _factory.CreateClient(Constant.XOAuth);

                if (client == null)
                {
                    throw new InvalidOperationException("HTTP client for X OAuth is not configured.");
                }

                Tokens tokens = new Tokens(new Tokens
                {
                    ConsumerSecret = _consumerSecret,
                    ConsumerKey = _consumerKey,
                    AccessToken = _accessToken,
                    AccessTokenSecret = _accessTokenSecret
                });

                var url = "https://api.x.com/2/tweets";

                var authHeader = tokens.CreateAuthorizationHeader(MethodType.Post, new Uri(url), null);

                client.DefaultRequestHeaders.Add("Authorization", authHeader);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true // just for readability
                };

                string json = JsonSerializer.Serialize(request, options);

                var content = new StringContent(json, null, "application/json");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    //var contentt = await response.Content.ReadAsStringAsync();
                    //var result = JsonConvert.DeserializeObject<TweetResponse>(contentt);

                    return true;
                }
                else
                {
                    // Handle the error response
                    var errorContent = await response.Content.ReadAsStringAsync();

                    SentrySdk.CaptureException(new Exception($"Failed to post X comment: {response.StatusCode} - {errorContent}"));

                    return false;
                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return false;
            }

        }

        internal async Task<(bool, NftParticipantResponse?)> ClassifyTweetAsync(string tweet, string username)
        {
            try
            {
                var model = new GenerativeModel();

                model.ApiKey = _modelApiKey;
                model.Model = _model;
                bool isTargetClassification = false;

                _sentry.AddBreadcrumb("Classifying tweet for NFT participation", "classfy.tweet",  new Dictionary<string, string>
                {
                    { "tweet", tweet },
                    { "username", username }
                });

                string systemPrompt = @"
                    You are a social media content analyzer specializing in identifying NFT market participants.
                    Your task is to analyze tweet content and determine if the author actively owns, trades, or creates NFTs (not just discussing them casually).
                    Focus on first-person experiences and personal involvement.
                    Always respond with valid JSON format only, using the exact schema provided.
                    ";

                string detectionCriteria = @"
                {
                  ""active_participant_indicators"": {
                    ""ownership_signals"": [""my NFT"", ""my collection"", ""just bought"", ""added to my wallet""],
                    ""trading_activity"": [""just sold"", ""flipped for profit"", ""diamond hands"", ""paper hands"", ""bought the dip""],
                    ""creation_minting"": [""just dropped my collection"", ""minting now"", ""my art is live"", ""created this piece""],
                    ""community_involvement"": [""fellow holders"", ""our community"", ""floor price of my collection""],
                    ""portfolio_management"": [""diversifying my collection"", ""rarity hunting"", ""holding since mint""],
                    ""personal_transactions"": [""specific amounts spent/earned"", ""gas fee complaints from personal experience""]
                  },
                  ""exclude_signals"": [
                    ""News sharing without personal involvement"",
                    ""General market commentary"", 
                    ""Educational content about NFTs"",
                    ""Criticism without ownership context"",
                    ""Third-person observations""
                  ]
                }
                ";

                string requestFormat = $@"
                {{
                  ""action"": ""analyze_nft_participant"",
                  ""tweet_content"": ""{tweet}"",
                  ""user_handle"": ""@{username}"",
                }}
                ";

                string responseSchema = @"
                {
                  ""user_handle"": ""@username"",
                  ""answer"": ""YES | NO"",
                  ""reasoning"": ""Brief explanation focusing on evidence of personal NFT involvement"",
                  ""confidence"": ""High | Medium | Low"",
                  ""participant_type"": ""owner | trader | creator | multiple | none"",
                  ""detected_keywords"": [""array"", ""of"", ""relevant"", ""keywords""]
                }
                ";

                NftParticipantResponse parsedResponse = null;
                int retries = 3;

                for (int attempt = 1; attempt <= retries; attempt++)
                {
                    string finalPrompt = $@"
                        SYSTEM PROMPT:
                        {systemPrompt}

                        DETECTION CRITERIA:
                        {detectionCriteria}

                        REQUEST:
                        {requestFormat}

                        Return the analysis strictly as valid JSON matching this schema:
                        {responseSchema}

                        IMPORTANT: 
                        - Do NOT include any text outside the JSON.
                        - Do NOT add explanations before or after.
                        - Always return valid JSON even if the tweet does not indicate NFT participation.
                        - Always return plain JSON without code fences
                        ";

                    var result = await model.GenerateContent(finalPrompt);
                    string rawOutput = result.Text.Trim();

                    rawOutput = CleanJson(rawOutput);

                    try
                    {
                        parsedResponse = JsonSerializer.Deserialize<NftParticipantResponse>(rawOutput)!;
                        if (parsedResponse != null &&
                            !string.IsNullOrWhiteSpace(parsedResponse.UserHandle) &&
                            !string.IsNullOrWhiteSpace(parsedResponse.Answer))
                        {
                            isTargetClassification = parsedResponse.Answer.Equals("YES", StringComparison.OrdinalIgnoreCase);

                            _sentry.AddBreadcrumb("Parsed NFT classification response", "classfy.tweet", new Dictionary<string, string>
                            {
                                { "user_handle", parsedResponse.UserHandle },
                                { "answer", parsedResponse.Answer },
                                { "reasoning", parsedResponse.Reasoning },
                                { "confidence", parsedResponse.Confidence },
                                { "participant_type", parsedResponse.ParticipantType },
                                { "detected_keywords", string.Join(", ", parsedResponse.DetectedKeywords) },
                                { "is_target_classification", isTargetClassification.ToString() },
                                {"tweet", tweet }
                            });

                            return (isTargetClassification, parsedResponse);
                        }
                        else
                        {
                            SentrySdk.CaptureMessage($"⚠️ Invalid JSON content on attempt {attempt}, retrying...");
                        }
                    }
                    catch (JsonException ex)
                    {
                        SentrySdk.CaptureException(ex);
                    }

                    if (attempt == retries)
                    {
                        SentrySdk.CaptureMessage("❌ Failed to get valid JSON after retries.");
                    }
                }
                return (isTargetClassification, null);
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return (false, null);
            }
        }

        internal async Task<List<XTargetAccount>?> GetXTargetAccountsAsync()
        {
            try
            {
                return await _context.XTargetAccounts
                    .Where(x => x.Engaged == 0)
                    .OrderByDescending(x => x.Id)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return null;
            }
        }

        internal async Task<bool> UpdateXTargetAccountToEngagedAsync(XTargetAccount targetAccount)
        {
            try
            {
                if (targetAccount == null || targetAccount.Id <= 0)
                    return false;

                targetAccount.Engaged = 1; // Mark as engaged
                _context.XTargetAccounts.Update(targetAccount);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return false;
            }
        }

        internal async Task XMarketingSchemeAsync()
        {
            try
            {
                var targetAccounts = await GetXTargetAccountsAsync();
                if (targetAccounts == null || !targetAccounts.Any())
                    return;

                foreach (var targetAccount in targetAccounts)
                {
                    if (await IsExistingUser(targetAccount.Username))
                    {
                        SentrySdk.CaptureMessage($"User @{targetAccount.Username} already exists, skipping engagement.");
                        continue;
                    }

                    var xAcct = await _xApi.GetUserByUsernameAsync(targetAccount.Username);
                    if (xAcct == null || xAcct.Data == null)
                    {
                        SentrySdk.CaptureMessage($"Target account {targetAccount.Username} not found.");
                        continue;
                    }

                    var userTweets = await _xApi.GetUserTimelineAsync(xAcct.Data.Id);

                    if (userTweets == null || userTweets.Data == null || !userTweets.Data.Any())
                    {
                        SentrySdk.CaptureMessage($"No tweets found for target account {targetAccount.Username}.");
                        continue;
                    }

                    // Check if the target account has tweeted about NFTs/Web3
                    foreach (var tweet in userTweets.Data)
                    {
                        var (isNFTRelated, data) = await ClassifyTweetAsync(tweet.Text, targetAccount.Username);

                        if (isNFTRelated && data != null)
                        {
                            var isSuccessful = await PostCommentAsync(tweet.Id, await GenerateCommentTemplate(tweet.Text, data));
                            if(isSuccessful)
                            {
                                // Update the target account to engaged
                                await UpdateXTargetAccountToEngagedAsync(targetAccount);
                                SentrySdk.CaptureMessage($"Successfully engaged with @{targetAccount.Username} on tweet {tweet.Id}.");
                            }
                            else
                            {
                                SentrySdk.CaptureMessage($"Failed to post comment on @{targetAccount.Username}'s tweet {tweet.Id}.");
                            }
                            break; // Stop after engaging with the first relevant tweet
                        }
                    }
                }
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
            }
        }

        private async Task<string> GenerateCommentTemplate(string tweet, NftParticipantResponse response)
        {
            var model = new GenerativeModel();

            model.ApiKey = _modelApiKey;
            model.Model = _model;

            string ovationFeaturesMarkdown = File.ReadAllText("OvationFeature.md");
            string messageType = "public_reply";

            _sentry.AddBreadcrumb("Generating engagement message", "generate.engagement.message", new Dictionary<string, string>
            {
                { "tweet", tweet },
                { "user_handle", response.UserHandle },
                { "participant_type", response.ParticipantType },
                { "detected_keywords", string.Join(", ", response.DetectedKeywords) }
            });

            // === System Prompt ===
            string systemPrompt = @"
                You are representing the official Ovation Network account, engaging authentically with NFT enthusiasts in the Web3 community.
                Your goal is to create personal, professional messages that naturally introduce Ovation Network's features and benefits.
                Be genuine, helpful, and avoid being overly salesy while maintaining a professional brand voice.
                End with a soft call-to-action about trying Ovation's free NFT profile creation.
                Adapt your message format based on the communication channel (public tweet reply or private direct message).
                You will receive a markdown document with detailed information about Ovation Network's features and capabilities to provide accurate, helpful information.
                Never more than 10 words, respond with slang informal language and avoid correct grammar and syntax. No emoji.
                ";

            // === Engagement Strategy ===
            string engagementStrategy = @"
            {
                ""engagement_strategy"": {
                ""brand_voice"": ""Professional yet friendly, knowledgeable, helpful"",
                ""tone"": ""Warm, genuine, and informative"",
                ""approach"": ""Celebrate their NFT journey and offer relevant solutions"",
                ""connection"": ""Show understanding of NFT culture and pain points"",
                ""introduction"": ""Naturally mention specific Ovation features that solve their needs"",
                ""cta"": ""Soft invitation to try for free with specific benefits"",
                ""adaptability"": ""Adjust format for public vs private communication"",
                ""authority"": ""Speak as the official brand with expertise and credibility""
                },
                ""response_elements"": [
                ""Acknowledge their specific NFT activity"",
                ""Show genuine interest and understanding"",
                ""Identify relevant pain points or opportunities"",
                ""Introduce specific Ovation features that help"",
                ""Provide clear value proposition"",
                ""Include call-to-action with link""
                ],
                ""format_considerations"": {
                ""public_reply"": ""Professional, engaging, Twitter-appropriate length, branded tone"",
                ""direct_message"": ""More detailed, personal, comprehensive feature explanation""
                }
            }
            ";

            // === Request Format ===
            string requestFormat = $@"
            {{
              ""action"": ""generate_engagement_message"",
              ""message_type"": ""{messageType}"",
              ""ovation_features"": ""{ovationFeaturesMarkdown.Replace("\"", "\\\"")}"",
              ""original_tweet"": {{
                ""user_handle"": ""{response.UserHandle}"",
                ""tweet_content"": ""{tweet}"",
                ""participant_type"": ""{response.ParticipantType}"",
                ""detected_keywords"": [{string.Join(", ", Array.ConvertAll(response.DetectedKeywords, k => $"\"{k}\""))}]
              }}
            }}
            ";

            // === Response Schema ===
            string responseSchema = @"
            {
              ""user_handle"": ""@username"",
              ""message_type"": ""public_reply | direct_message"",
              ""message_content"": ""Generated message content with call-to-action"",
              ""engagement_type"": ""congratulatory | supportive | informative | community"",
              ""featured_benefits"": [""list"", ""of"", ""ovation"", ""features"", ""mentioned""],
              ""contains_cta"": true,
              ""estimated_character_count"": 250,
              ""formatting_notes"": ""Any specific formatting considerations"",
              ""brand_compliance"": ""Professional brand voice maintained""
            }
            ";


            EngagementResponse parsedResponse = null;
            var finalPrompt = $@"
                SYSTEM PROMPT:
                {systemPrompt}

                ENGAGEMENT STRATEGY:
                {engagementStrategy}

                REQUEST:
                {requestFormat}

                Return the analysis strictly as valid JSON matching this schema:
                {responseSchema}

                IMPORTANT: 
                - Do NOT include any text outside the JSON.
                - Do NOT add explanations before or after.
                - Always return valid JSON even if the tweet does not indicate NFT participation.
                - Always return plain JSON without code fences
                ";

            var result = await model.GenerateContent(finalPrompt);
            string rawOutput = result.Text.Trim();

            rawOutput = CleanJson(rawOutput);
            try
            {
                parsedResponse = JsonSerializer.Deserialize<EngagementResponse>(rawOutput)!;

                _sentry.AddBreadcrumb("Parsed engagement response", "generate.engagement.message", new Dictionary<string, string>
                {
                    { "user_handle", parsedResponse.UserHandle },
                    { "message_type", parsedResponse.MessageType },
                    { "original_tweet", tweet },
                    { "message_content", parsedResponse.MessageContent },
                    { "engagement_type", parsedResponse.EngagementType },
                    { "featured_benefits", string.Join(", ", parsedResponse.FeaturedBenefits) },
                    { "contains_cta", parsedResponse.ContainsCta.ToString() },
                    { "estimated_character_count", parsedResponse.EstimatedCharacterCount.ToString() },
                    { "formatting_notes", parsedResponse.FormattingNotes },
                    { "brand_compliance", parsedResponse.BrandCompliance }
                });

                return parsedResponse?.MessageContent ?? string.Empty;
            }
            catch (JsonException ex)
            {
                SentrySdk.CaptureException(ex);
                SentrySdk.CaptureMessage($"Raw Output: {rawOutput}");
                return string.Empty;
            }
        }

        private async Task<bool> IsExistingUser(string username)
        {
            try
            {
                var existingUser = await _context.VerifiedUsers
                    .FirstOrDefaultAsync(x => x.Handle.ToLower() == username.ToLower() && x.Type == Type.X.ToString() );
                return existingUser != null;
            }
            catch (Exception _)
            {
                SentrySdk.CaptureException(_);
                return false;
            }
        }

        private string CleanJson(string json)
        {
            string cleaned = json.Trim();

            if (cleaned.StartsWith("```"))
            {
                // Find the first newline after the opening fence
                int firstNewline = cleaned.IndexOf('\n');
                if (firstNewline >= 0)
                    cleaned = cleaned.Substring(firstNewline + 1);

                // Remove trailing fence if present
                int lastFence = cleaned.LastIndexOf("```", StringComparison.Ordinal);
                if (lastFence >= 0)
                    cleaned = cleaned.Substring(0, lastFence);
            }

            return cleaned;
        }
    }
}
