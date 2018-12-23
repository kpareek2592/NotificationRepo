using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Configuration;
using SendGridEmailApplication.Exceptions;
using SendGridEmailApplication.Entities;


// ReSharper disable once CheckNamespace
namespace SendGridEmailApplication
{
    /// <summary>
    /// Base class for all controllers hosting reusable methods
    /// </summary>
    public abstract class ApiControllerBase : ApiController
    {
        /// <summary>
        /// Version information for REST URI
        /// </summary>
        protected const string _apiV1 = "api/v1";

        /// <summary>
        /// Correlation Id for Logging
        /// </summary>
        protected string _cid;

        /// <summary>
        /// Correlation Id as provided by Adopter for loggnig
        /// </summary>
        protected string _logTrackingId;

        private Exception exceptionToLog = null;

        private int logResponseMs;

        private int logMaxCharsResponse;

        private int maxEmailBodyLength;

        /// <summary>
        /// The response of the API call is logged if this is set true.
        /// </summary>
        protected bool _logResponse = false;

        /// <summary>
        /// IoTHub - used as ingress name
        /// </summary>
        public const string IoTHub = "iothub";

        /// <summary>
        /// RabbitMQ - used as ingress name
        /// </summary>
        protected const string RabbitMQ = "rabbitmq";

        /// <summary>
        /// None - used as ingress name
        /// </summary>
        protected const string None = "none";



        //ToDo: Move the cache in common project once its established
        //private ObjectCache _deviceProfilesCache = MemoryCache.Default;
        private object _lock = new object();
        private static readonly string _adopterAdminRole = "3d42846d-84df-4f7b-8b92-6cb46876bbfd";
        private static readonly string _securityAdminRole = "f7b05eda-ecbd-4cba-ae2f-95a06bab624b";


        /// <summary>
        /// Initialize the Controller base class
        /// </summary>
        public ApiControllerBase()
        {
            // If response time exceeds specified time it will be logged
            //int.TryParse(PlatformService.GetConfigurationSettingValue("LogIfTimeExceeds"), out logResponseMs);
            int.TryParse(ConfigurationManager.AppSettings["MaxCharacterToLog"], out logMaxCharsResponse);
            int.TryParse(ConfigurationManager.AppSettings["MaxEmailBodyLength"], out maxEmailBodyLength);

            if (logResponseMs == 0) logResponseMs = 100;
            if (logMaxCharsResponse == 0) logMaxCharsResponse = 100;
            if (maxEmailBodyLength == 0) maxEmailBodyLength = 5000;
        }

        /// <summary>
        /// Processes an API request.
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        //protected HttpResponseMessage ProcessAPI(ScheduledApiBase api)
        //{
        //    return ProcessRequest(() =>
        //    {
        //        //set correlation id for logging.
        //        api.cid = _cid;

        //        api.AccessControlEnabled = PlatformService.AccessControlInstance.GetAccessControlFlag();
        //        api.Request = Request;

        //        var resp = api.Execute();
        //        exceptionToLog = resp.ApiException;
        //        return CreateResponse(resp.Payload, resp.StatusCode);

        //    }, Request, api.GetOperation(), resourceId: api.GetResource(), bypassAuthorization: true);
        //}

        /// <summary>
        /// Generic function for processes API requests.
        /// </summary>
        /// <param name="requestFunction"></param>
        /// <param name="bypassAuthorization"></param>
        /// <param name="bypassAuthentication"></param>
        /// <param name="currentUser"></param>
        /// <param name="operation"></param>
        /// <param name="request"></param>
        /// <param name="resourceId"></param>
        /// <param name="contentLoggingEnable"></param>
        /// <returns></returns>
        protected HttpResponseMessage ProcessRequest(Func<HttpResponseMessage> requestFunction, HttpRequestMessage request, Operations operation, String resourceId, bool bypassAuthorization = false, bool bypassAuthentication = false, string currentUser = null, bool contentLoggingEnable = true)
        {
            return ProcessRequest(requestFunction, request, operation, resourceId, bypassAuthorization, bypassAuthentication, currentUser, contentLoggingEnable);
        }

        /// <summary>
        /// Generic function for processes API requests.
        /// </summary>
        /// <param name="requestFunction"></param>
        /// <param name="bypassAuthorization"></param>
        /// <param name="bypassAuthentication"></param>
        /// <param name="currentUser"></param>
        /// <param name="operation"></param>
        /// <param name="request"></param>
        /// <param name="resourceId"></param>
        /// <param name="contentLoggingEnable"></param>
        /// <returns></returns>
        //protected HttpResponseMessage ProcessRequest(Func<HttpResponseMessage> requestFunction, HttpRequestMessage request, String operation, String resourceId, bool bypassAuthorization = false, bool bypassAuthentication = false, string currentUser = null, bool contentLoggingEnable = true)
        //{
        //    HttpResponseMessage response = null;
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    //check for access control flag
        //    //for unit tests, it is set to false
        //    bool _accessControlEnabled = PlatformService.AccessControlInstance.GetAccessControlFlag();

        //    try
        //    {

        //        _cid = Guid.NewGuid().ToString();

        //        IEnumerable<string> headerValues = null;
        //        request?.Headers?.TryGetValues("log-tracking-id", out headerValues);

        //        if (headerValues?.Count() == 1)
        //        {
        //            _logTrackingId = ParseGuidAsString(headerValues.First(), "log-tracking-id");
        //        }
        //        else
        //        {
        //            _logTrackingId = "";
        //        }

        //        if (_accessControlEnabled)
        //        {

        //            //Check for token validation/expiration
        //            if (!bypassAuthentication && !PlatformService.AccessControlInstance.CheckAuthentication(Request, out var userIdentity, _cid, _logTrackingId))
        //            {
        //                var message = string.Format(Resources.UserAuthenticationFailed);
        //                response = CreateNotAuthorizedResponse(userMessage: message); // User is not authenticated
        //            }
        //            //Check for authorization
        //            if (!bypassAuthorization && response == null)
        //            {
        //                //check whether user is authorized
        //                //skip authorization for self
        //                if (currentUser != null)
        //                {
        //                    //check users identity
        //                    string emailIdFromToken = PlatformService.IdentityProviderInstance.ExtractEmailAddressFromToken(Request);

        //                    if (!(currentUser.Equals(emailIdFromToken)) && !IsAuthorized(Request, operation, resourceId))
        //                        response = CreateNotAuthorizedResponse();

        //                }
        //                else
        //                {
        //                    if (!IsAuthorized(Request, operation, resourceId))
        //                        response = CreateNotAuthorizedResponse();
        //                }
        //            }

        //            if (response == null)
        //                response = requestFunction();
        //        }
        //        else
        //            response = requestFunction();
        //    }
        //    catch (BadRequestException ex)
        //    {
        //        var responseResult = ex.GetExceptionResponse(request);

        //        if (responseResult != null)
        //        {
        //            response = responseResult.Response;
        //        }
        //        else
        //        {
        //            response = CreateEmptyResponse();
        //            if (ex.ErrorProperties != null)
        //                response.StatusCode = ex.ErrorProperties.ErrorCode;
        //            else
        //                response.StatusCode = HttpStatusCode.BadRequest;
        //        }
        //        exceptionToLog = ex;
        //    }
        //    catch (APIException ex)
        //    {
        //        var responseResult = ex.GetExceptionResponse(request);

        //        if (responseResult != null)
        //        {
        //            response = responseResult.Response;
        //        }
        //        else
        //        {
        //            response = CreateEmptyResponse();
        //            if (ex.ErrorProperties != null)
        //                response.StatusCode = ex.ErrorProperties.ErrorCode;
        //            else
        //                response.StatusCode = HttpStatusCode.BadRequest;
        //        }
        //        exceptionToLog = ex;
        //    }
        //    catch (Exception ex)
        //    {
        //        response = CreateEmptyResponse();
        //        response.StatusCode = HttpStatusCode.InternalServerError;
        //        exceptionToLog = ex;
        //    }

        //    sw.Stop();

        //    //User Id and App Id can only be determined if valid token is provided, but request should 
        //    //be logged regardless of whether token is valid or not
        //    String userId = "???";
        //    String appId = "???";
        //    try
        //    {
        //        userId = GetLoggingUserId();
        //        appId = GetLoggingApplicationId();
        //    }
        //    catch (Exception e)
        //    {
        //        exceptionToLog = e;
        //    }

        //    LogUtilities.LogRequest(request, _cid, userId, operation, appId, response, sw.ElapsedMilliseconds, _logResponse, PlatformService.InstanceId, _logTrackingId, contentLoggingEnable);

        //    if (exceptionToLog != null)
        //        LogUtilities.LogError(exceptionToLog.Message, _cid, _logTrackingId, exceptionToLog);

        //    return response;
        //}

        #region HTTP Response Methods
        /// <summary>
        /// Response is empty, send No content code
        /// </summary>
        /// <returns></returns>
        protected HttpResponseMessage CreateEmptyResponse()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };
        }


        /// <summary>
        /// Response is resource created
        /// </summary>
        /// <returns></returns>
        protected HttpResponseMessage CreateResourceCreatedResponse()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.Created };
        }

        /// <summary>
        /// Create the Json Response message and send it to client
        /// </summary>
        /// <returns></returns>
        protected HttpResponseMessage CreateJsonResponse(string jsonResponse, HttpStatusCode statuscode = HttpStatusCode.OK)
        {
            var resp = new HttpResponseMessage
            {
                Content = new StringContent(jsonResponse),
                StatusCode = statuscode
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return resp;
        }

        /// <summary>
        /// Create the Xml Response message and send it to client
        /// </summary>
        /// <returns></returns>
        protected HttpResponseMessage CreateXmlResponse(string xmlResponse, HttpStatusCode statuscode = HttpStatusCode.OK)
        {
            var resp = new HttpResponseMessage
            {
                Content = new StringContent(xmlResponse),
                StatusCode = statuscode
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

            return resp;
        }

        /// <summary>
        /// Create a not authorized message and send it to the client
        /// </summary>
        /// <returns></returns>
        //protected HttpResponseMessage CreateNotAuthorizedResponse(string userMessage = "", string developerMessage = "")
        //{
        //    StringBuilder message = new StringBuilder();
        //    if (!string.IsNullOrEmpty(developerMessage))
        //    {
        //        message.Append(" Developer Message : " + developerMessage);
        //    }
        //    if (!string.IsNullOrEmpty(userMessage))
        //    {
        //        message.Append(" User Message : " + userMessage);
        //    }

        //    LogInfo($"Unauthorized Response - {message}");
        //    return CreateResponse(Resources.NotAuthorized + userMessage, HttpStatusCode.Unauthorized);
        //}

        /// <summary>
        /// Create the response message and send it to client
        /// </summary>
        /// <param name="obj">Native .Net Object</param>
        /// <param name="statuscode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected HttpResponseMessage CreateResponse<T>(T obj, HttpStatusCode statuscode = HttpStatusCode.OK, string token = "")
        {
            var resp = new HttpResponseMessage();
            string str = "";

            // empty result
            if (obj == null)
            {
                resp.Dispose();
                resp = Request.CreateResponse(statuscode);
            }

            // add security token to header
            if (!string.IsNullOrEmpty(token))
            {
                resp.Headers.Add("Authorization", "Bearer " + token);
            }

            // add the logging correlation id to the header to make it easier to query logs
            if (!string.IsNullOrEmpty(_cid))
            {
                resp.Headers.Add("logging-cid", _cid);
            }

            switch (GetRequestedFormat(Request))
            {
                case RequestedFormat.Json:
                    {
                        if (obj is string)
                        {
                            str = obj.ToString();
                            resp.Content = new StringContent(str);
                            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("text/json");
                        }
                        else
                        { //TODO: modify StringContent UTF8 reference to leverage unicode once multiple languages are supported
                            str = SerializeToJson(obj);
                            resp.Content = new StringContent(str, System.Text.Encoding.UTF8, "application/json");
                        }

                        resp.StatusCode = statuscode;

                        break;
                    }
                case RequestedFormat.Xml:
                    {
                        if (obj is string)
                        {
                            str = obj.ToString();
                            resp.Content = new StringContent(str);
                            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                        }
                        else
                        {
                            str = SerializeToXml(obj);
                            resp.Content = new StringContent(str);
                            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                        }
                        resp.StatusCode = statuscode;
                    }
                    break;
                default:
                    resp.Dispose();
                    resp = Request.CreateResponse(HttpStatusCode.BadRequest);
                    break;
            }
            return resp;
        }
        /// <summary>
        /// Serialize content to JSON with new line character indentation
        /// </summary>
        /// <param name="obj">Native .Net Object</param>
        /// <returns></returns>
        protected string SerializeToJsonWithIndentation(Object obj)
        {

            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
                //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //MissingMemberHandling = MissingMemberHandling.Ignore,

            }
            );
        }
        /// <summary>
        /// Serialize content to JSON
        /// </summary>
        /// <param name="obj">Native .Net Object</param>
        /// <returns></returns>
        protected string SerializeToJson(Object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
                //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //MissingMemberHandling = MissingMemberHandling.Ignore,

            });
        }

        /// <summary>
        /// Serialize content to XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Native .Net Object</param>
        /// <returns></returns>
        protected String SerializeToXml<T>(T obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));

            using (StringWriter sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, obj);
                return sww.ToString();
            }
        }

        /// <summary>
        /// Write an info log entry using a standard format.
        /// </summary>
        /// <param name="msg">Log message to write.</param>
        //protected void LogInfo(string msg)
        //{
        //    LogUtilities.LogInfo(msg, _cid, _logTrackingId);
        //}

        /// <summary>
        /// Write an warning log entry using a standard format.
        /// </summary>
        /// <param name="msg">Log message to write.</param>
        //protected void LogWarn(string msg)
        //{
        //    LogUtilities.LogInfo(msg, _cid, _logTrackingId);
        //}

        /// <summary>
        /// Write an error log entry using a standard format.
        /// </summary>
        /// <param name="msg">Error message to write. Note the the Exception message will also be written. So no need to pass it here.</param>
        /// <param name="e">The exception to log (optional)</param>
        //protected void LogError(string msg, Exception e = null)
        //{
        //    LogUtilities.LogError(msg, _cid, _logTrackingId, e);
        //}

        /// <summary>
        /// Write an audit log entry using a standard format.
        /// </summary>
        /// <param name="type">Data type being worked with (ie Device, Partition, etc)</param>
        /// <param name="id">id of the data element being worked with</param>
        /// <param name="operation">An enumeration representing the web api operation being performed</param>
        /// <param name="auditOperation">An enumeration representing the audit operation being performed</param>        
        /// <param name="msg">Optional message to include in the audit log</param>
        /// <param name="contentLoggingEnable">If true, request content will be logged. If false, content will be ignored</param>
        //protected void LogAudit(string type, string id, Operations operation, AuditOperations auditOperation, string msg = null, bool contentLoggingEnable = true)
        //{
        //    LogUtilities.LogAudit(Request, GetLoggingUserId(), type, id, operation.ToString(), GetLoggingApplicationId(), auditOperation, msg, _cid, _logTrackingId, contentLoggingEnable);
        //}

        /// <summary>
        /// Write an audit log entry using a standard format.
        /// </summary>
        /// <param name="type">Data type being worked with (ie Device, Partition, etc)</param>
        /// <param name="id">id of the data element being worked with</param>
        /// <param name="operationStr">Operation being performed</param>
        /// <param name="auditOperation">An enumeration representing the audit operation being performed</param>        
        /// <param name="msg">Optional message to include in the audit log</param>
        /// <param name="contentLoggingEnable">If true, request content will be logged. If false, content will be ignored</param>
        //protected void LogAudit(string type, string id, String operationStr, AuditOperations auditOperation, string msg = null, bool contentLoggingEnable = true)
        //{
        //    LogUtilities.LogAudit(Request, GetLoggingUserId(), type, id, operationStr, GetLoggingApplicationId(), auditOperation, msg, _cid, _logTrackingId, contentLoggingEnable);
        //}

        /// <summary>
        /// Get the format parameter.
        /// </summary>
        public static RequestedFormat GetRequestedFormat(HttpRequestMessage request)
        {
            if (request == null)        // default, used for unit tests.
                return RequestedFormat.Json;
            var pair = request.GetQueryNameValuePairs().SingleOrDefault(nv => String.Compare(nv.Key, "format", StringComparison.OrdinalIgnoreCase) == 0);
            if (pair.Value == null) // Default to JSON if no format is specified.
                return RequestedFormat.Json;
            if (String.Compare(pair.Value, "xml", StringComparison.OrdinalIgnoreCase) == 0)
                return RequestedFormat.Xml;
            if (String.Compare(pair.Value, "json", StringComparison.OrdinalIgnoreCase) == 0)
                return RequestedFormat.Json;
            throw new BadRequestException(Resources.InvalidRequest, request, "format");
        }

        #endregion

        #region Helper Methods



        //ToDo: Remove this method
        //internal HashSet<AccessControlAdopterResult> GetAdoptersForAdminInternal(string userId)
        //{
        //    // result list to be returned
        //    HashSet<AccessControlAdopterResult> adopters = new HashSet<AccessControlAdopterResult>();
        //    if (string.IsNullOrEmpty(userId)) return adopters;

        //    // determine if user has the Adopter role, access to all Adopters
        //    bool isAdopterAdminUser = PlatformService.AccessControlInstance.GetPlatformAccessControl().IsUserAdopterAdminRole(userId);
        //    bool isSecurityAdminUser = PlatformService.AccessControlInstance.GetPlatformAccessControl().IsUserSecurityAdminRole(userId);

        //    //validate and get document store connection
        //    ValidateMongoConnection();
        //    var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();
        //    var userProfileReader = documentConnection?.CreateUserAccessProfileReader();
        //    var adopterReader = documentConnection?.CreateAdopterReader();
        //    var userProfile = userProfileReader?.GetUserAccessProfileById(userId);
        //    if (userProfile != null)
        //    {
        //        foreach (var rule in userProfile?.Rules)
        //        {
        //            if (isAdopterAdminUser)
        //            {
        //                if (string.Equals(rule?.RoleId, _adopterAdminRole))
        //                {
        //                    if (!string.IsNullOrEmpty(rule.AdopterId))
        //                    {
        //                        var adopter = adopterReader.GetAdopter(rule.AdopterId);
        //                        if (adopter != null)
        //                            adopters.Add(CreateDtoAdopter(adopter));
        //                    }
        //                }
        //            }
        //            if (isSecurityAdminUser)
        //            {
        //                if (string.Equals(rule?.RoleId, _securityAdminRole))
        //                {
        //                    if (!string.IsNullOrEmpty(rule.SiteId))
        //                    {
        //                        var adopter = adopterReader.GetAdopter(GetSiteAdopter(rule.SiteId));
        //                        if (adopter != null)
        //                            adopters.Add(CreateDtoAdopter(adopter));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return adopters;
        //}

        //private string GetLoggingUserId()
        //{
        //    string userId = GetUserIdFromRequestToken();
        //    if (string.IsNullOrEmpty(userId)) userId = Resources.AnonymousUser;

        //    return userId;
        //}

        //private string GetLoggingApplicationId()
        //{
        //    return PlatformService.IdentityProviderInstance.ExtractApplicationIdFromToken(Request);
        //}

        /* /// <summary>
         /// Get the Date Time in ISO 8601 format
         /// </summary>
         /// <param name="dt">DateTime Object</param>
         /// <returns>
         /// DateTime in Specific Format
         /// </returns>
         protected static string GetISODateTimeString(DateTime dt)
         {
             if (dt == null || dt == DateTime.MinValue)
                 return "";
             return dt.ToUniversalTime().ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z";
         }*/

        /// <summary>
        /// Get the DateTime object from ISO 8601 string
        /// Throw exception if not in correct format
        /// </summary>
        /// <param name="datetime">DateTime String</param>
        /// <param name="defaultValue"> Provides current datetime in case of null</param>
        /// <param name="parametername">Name of Date Parameter</param>
        /// <returns></returns>
        protected DateTime ParseDateTime(string datetime, string parametername, bool defaultValue = false)
        {
            if (defaultValue && string.IsNullOrEmpty(datetime))
                return DateTime.Now.ToUniversalTime();

            if (string.IsNullOrEmpty(datetime))
                throw new BadRequestException(Resources.InvalidDateTime, Request, parametername);

            datetime = datetime.Trim();
            DateTime dtDateTime;
            bool isParsed = DateTime.TryParse(datetime, null, DateTimeStyles.RoundtripKind, out dtDateTime);

            if (!isParsed)
                throw new BadRequestException(Resources.InvalidDateTime, Request, parametername);
            if (dtDateTime.Kind == DateTimeKind.Utc)
                return dtDateTime;
            else
                // set the kind to UTC without changing the time.
                return new DateTime(dtDateTime.Year, dtDateTime.Month, dtDateTime.Day, dtDateTime.Hour, dtDateTime.Minute, dtDateTime.Second, dtDateTime.Millisecond, DateTimeKind.Utc);
        }

        /// <summary>
        /// Data Partition - Geogrophical, Military or Government specific
        /// </summary>
        //protected string PartitionId
        //{
        //    get
        //    {
        //        var pid = PlatformService.ConnectionManager.PartitionId;
        //        if (string.IsNullOrEmpty(pid))
        //            throw new APIException(Resources.InvalidPartition, Request);

        //        return pid;
        //    }
        //}


        /// <summary>
        /// Get the Date Time in Epoch format
        /// </summary>
        /// <param name="dt">DateTime Object</param>
        /// <returns>DateTime in Epoch Format</returns>
        protected long GetEpochDateTime(DateTime dt)
        {
            TimeSpan start_t = dt - new DateTime(1970, 1, 1);
            return (long)start_t.TotalSeconds;
        }

        /// <summary>
        /// Validate Start Date is not greater then the End Date, throw exception if not validated
        /// </summary>
        /// <param name="start">ISO 8601 Format Date</param>
        /// <param name="end">ISO 8601 Format Date</param>
        /// <param name="parameterName">Parameter Name for Start and End Date</param>
        protected void ValidateTimeRange(DateTime start, DateTime end, string parameterName)
        {
            if (start == null || end == null ||
                (start != DateTime.MinValue && end != DateTime.MinValue && end < start))
                throw new BadRequestException(Resources.InvalidTimeRange, Request, parameterName);
        }

        /// <summary>
        /// Validate Start Date is not greater then the End Date, throw exception if not validated
        /// </summary>
        /// <param name="dateTime">ISO 8601 Format Date</param>
        /// <param name="parameterName">Parameter Name for Start and End Date</param>
        protected void ValidatePastDate(DateTime dateTime, string parameterName)
        {
            if (dateTime == null || dateTime < DateTime.UtcNow)
                throw new BadRequestException(Resources.PastDateTime, Request, parameterName);
        }


        /// <summary>
        /// Validates the object parameter for null.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">name of the parameter to add in error detail</param>
        /// <exception cref="BadRequestException">
        /// </exception>
        protected void ValidateParameterForNull(object parameter, string parameterName = "")
        {
            if (parameter == null)
                throw new BadRequestException(Resources.InvalidRequest, Request, parameterName);
        }



        /// <summary>
        /// Validates input is a valid email address. 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="parameterName"></param>
        protected void ValidateEmail(string email, string parameterName = "")
        {
            if (!string.IsNullOrEmpty(email))
            {
                bool isEmail = Regex.IsMatch(email,
                    @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                    RegexOptions.IgnoreCase);
                if (!isEmail)
                    throw new BadRequestException(Resources.InvalidEmail, Request, parameterName);
            }
        }

        /// <summary>
        /// Validates input is a valid phone number. 
        /// </summary>
        /// <param name="number">The phone number to validate</param>
        /// <param name="parameterName"></param>
        protected void ValidatePhoneNumber(string number, string parameterName = "")
        {

            if (!string.IsNullOrEmpty(number))
            {
                bool isValid = Regex.IsMatch(number,
                     @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}",
                    RegexOptions.IgnoreCase);
                if (!isValid)
                    throw new BadRequestException(Resources.InvalidPhoneNumber, Request, parameterName);
            }
        }


        /// <summary>
        /// Validates input is a valid password. 
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <param name="parameterName"></param>
        public void ValidatePassword(string password, string parameterName = "password")
        {
            var count = 0;
            if (password != null)
            {
                password = password.Trim();
                if (password.Length < 8 || password.Length > 16)
                {
                    throw new BadRequestException(Resources.InvalidPasswordLength, Request, parameterName);
                }

                //Conditions : Strong password should pass atleast 3 policies
                var hasNumber = new Regex(@"[0-9]+");
                var hasUpperChar = new Regex(@"[A-Z]+");
                var hasLowerChar = new Regex(@"[a-z]+");
                var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

                if (hasLowerChar.IsMatch(password))
                {
                    count++;
                }
                if (hasUpperChar.IsMatch(password))
                {
                    count++;
                }
                if (hasNumber.IsMatch(password))
                {
                    count++;
                }
                if (hasSymbols.IsMatch(password))
                {
                    count++;
                }

                if (count < 3)
                    throw new BadRequestException(Resources.InvalidPassword, Request, parameterName);
            }


        }

        /// <summary>
        /// Validates a string does not have any special characters. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        protected string ValidateEntityName(string name, string parameterName = "")
        {
            name = name.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                char[] specialCharacters = new[] { ' ', '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '+', '=', '{', '}', '[', ']', ':', ';', '"', '<', '>', ',', '.', '?', '/', '|', '\\' };
                if (name.IndexOfAny(specialCharacters) != -1)
                    throw new BadRequestException(Resources.InvalidEntityName, Request, parameterName);
            }
            return name;
        }

        /// <summary>
        /// Validates max length of a field. 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="len"></param> 
        /// <param name="parameterName"></param>
        protected void ValidateMaxFieldLength(string field, int len, string parameterName = "")
        {
            if (!string.IsNullOrEmpty(field))
            {
                if (field.Length > len)
                    throw new BadRequestException(Resources.InvalidMaxFieldLength, Request, parameterName);
            }
        }

        /// <summary>
        /// Validates min length of a field. 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="len"></param> 
        /// <param name="parameterName"></param>
        protected void ValidateMinFieldLength(string field, int len, string parameterName = "")
        {
            if (!string.IsNullOrEmpty(field))
            {
                if (field.Length < len)
                    throw new BadRequestException(Resources.InvalidMinFieldLength, Request, parameterName);
            }
        }

        /// <summary>
        /// Validates max length for email body. 
        /// </summary>
        /// <param name="field"></param>      
        /// <param name="parameterName"></param>
        protected void ValidateMaxLengthForEmailBody(string field, string parameterName = "")
        {
            if (!string.IsNullOrEmpty(field))
            {
                if (field.Length > maxEmailBodyLength)
                    throw new BadRequestException(Resources.InvalidMaxFieldLength, Request, parameterName);
            }
        }


        /// <summary>
        /// Validates string for empty or null and return the trimed value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">name of the parameter to add in error detail</param>
        /// <exception cref="BadRequestException">
        /// </exception>
        protected string ParseString(string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                throw new BadRequestException(Resources.InvalidRequest, Request, parameterName);

            return parameter.Trim();
        }

        /// <summary>
        /// Validates string for either 'true' or 'false'
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        protected bool ParseBoolean(string parameter, string parameterName)
        {
            if (!string.IsNullOrEmpty(parameter))
                parameter = parameter.ToLower().Trim();
            if (string.IsNullOrEmpty(parameter))
                throw new BadRequestException(Resources.InvalidRequest, Request, parameterName);
            if (parameter == "true" || parameter == "false")
                return bool.Parse(parameter);
            else
                throw new BadRequestException(Resources.InvalidRequest, Request, parameterName);
        }

        /// <summary>
        /// Validates string for empty or null and return trim value in lower case.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">name of the parameter to add in error detail</param>
        /// <exception cref="BadRequestException">
        /// </exception>
        protected string ParseStringToLower(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
                throw new BadRequestException(Resources.InvalidRequest, Request, parameterName);
            return parameter.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Validates string to make sure that it only contains letters and/or digits, returns the trim value in lower case.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">name of the parameter to add in error detail</param>
        /// <exception cref="BadRequestException">
        /// </exception>
        protected string ParseStringToLowerLettersDigitsOnly(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
                throw new BadRequestException(Resources.InvalidRequest, Request, parameterName);
            foreach (char c in parameter)
            {
                if (!Char.IsLetterOrDigit(c))
                    throw new BadRequestException(Resources.InvalidRequestLettersOrDigitsOnly, Request, parameterName);
            }
            return parameter.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Validates the uuid and return it in lower case
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        public static string ParseGuidAsString(string value, string parameterName)
        {
            Guid result;
            if (string.IsNullOrWhiteSpace(value))
                throw new BadRequestException(Resources.InvalidUUID, null, parameterName);

            value = value.Trim();
            if (Guid.TryParse(value, out result) && result != Guid.Empty)
            {
                // Return the guid as a string. Force the output to include dashes and force alpha characters to be lower case
                return result.ToString("D").ToLowerInvariant();
            }
            throw new BadRequestException(Resources.InvalidUUID, null, parameterName);
        }

        /// <summary>
        /// Parse and validate the trait, throw exception if invalid.
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="trait">Output the valid lower case trait, which must be "v", "min", "max", or "avg".</param>
        protected void ParseTraitToLower(string value, string parameterName, out string trait)
        {
            trait = "v";
            if (!string.IsNullOrEmpty(value))
                trait = value.Trim();
            if (!string.IsNullOrEmpty(trait))
                trait = value.ToLowerInvariant();
            if (trait != "v" && trait != "avg" && trait != "max" && trait != "min")
                throw new BadRequestException(String.Format(Resources.InvalidTrait, trait), Request, parameterName);
        }


        /// <summary>
        /// Validate Parameter passed for channel tag.trait, throw exception if its null
        /// </summary>
        /// <param name="parameter">Parameter to validate</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="tag">The validated channel tag. The tag is case sensitive.</param>
        /// <param name="trait">The validated trait. The trait is always converted to the lower case. The default trait is "v".</param>
        protected void ValidateTagTrait(string parameter, string parameterName, out string tag, out string trait)
        {
            parameter = ParseString(parameter, parameterName);
            var tokens = parameter.Split('.');
            if (tokens.Length < 1 || tokens.Length > 2)
                throw new BadRequestException(Resources.InvalidTagTrait, Request, parameterName);
            tag = tokens[0];
            if (!string.IsNullOrEmpty(tag))
                tag = tag.Trim();
            if (string.IsNullOrEmpty(tag))
                throw new BadRequestException(Resources.InvalidTagTrait, Request, parameterName);

            if (tokens.Length == 2)
                ParseTraitToLower(tokens[1], parameterName, out trait);
            else
                trait = "v";
        }

        /// <summary>
        /// Validates the parameter for empty or null.
        /// </summary>
        /// <exception cref="APIException"></exception>
        /// <exception cref="System.Exception"></exception>
        //public static void ValidateMongoConnection()
        //{
        //    if (PlatformService.ConnectionManager.IsMongoConnectionBroken || PlatformService.ConnectionManager.GetMongoConnection() == null)
        //    {
        //        PlatformService.ConnectionManager.IsMongoConnectionBroken = true;
        //        LogUtilities.LogInfo(Resources.MongoConnectionBroken);
        //        throw new APIException(Resources.MongoConnectionBroken, null, new Exception(), HttpStatusCode.ServiceUnavailable);
        //    }
        //}


        /// <summary>
        /// Validates an adopter id.
        /// </summary>
        /// <param name="adopterId"></param>
        /// <returns></returns>
        //internal virtual bool ValidateAdopterId(string adopterId)
        //{
        //    try
        //    {
        //        var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();
        //        var adopterReader = documentConnection.CreateAdopterReader();
        //        var document = adopterReader.GetAdopter(adopterId);

        //        if (document == null)
        //            return false;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// Gets the publisher id based on the device id from Mongo.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        //public static string GetPublisherId(string deviceId)
        //{
        //    ValidateMongoConnection();

        //    //get mongo connection
        //    var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();

        //    try
        //    {
        //        var deviceReader = documentConnection.CreateDeviceReader();
        //        var publisherId = deviceReader.GetPublisherId(deviceId);
        //        if (string.IsNullOrEmpty(publisherId))
        //            throw new APIException(Resources.UnregisteredDevice, null, null, HttpStatusCode.NotFound);
        //        return publisherId;
        //    }
        //    catch (MongoDbDalcReadDataException ex)
        //    {
        //        throw new APIException(Resources.ErrorReadingDeviceData, null, ex);
        //    }
        //}

        /// <summary>
        /// Gets the adopter id based on the device id from Mongo.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        //protected string GetAdopterIdFromDeviceId(string deviceId)
        //{
        //    ValidateMongoConnection();

        //    //get mongo connection
        //    var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();

        //    try
        //    {
        //        var deviceReader = documentConnection.CreateDeviceReader();
        //        var device = deviceReader.GetDevice(deviceId);
        //        if (device == null)
        //            throw new APIException(Resources.UnregisteredDevice, Request, null, HttpStatusCode.NotFound);
        //        var siteReader = documentConnection.CreateSiteReader();
        //        var site = siteReader.GetSite(device.SiteId);
        //        if (site == null)
        //            throw new APIException(Resources.SiteNotFound, Request, null, HttpStatusCode.NotFound);

        //        return site.AdopterId;
        //    }
        //    catch (MongoDbDalcReadDataException ex)
        //    {
        //        throw new APIException(Resources.ErrorReadingDeviceData, Request, ex);
        //    }
        //}

        /// <summary>
        /// Gets the adopter id based on the site id from Mongo.
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        //protected string GetAdopterIdFromSiteId(string siteId)
        //{
        //    ValidateMongoConnection();

        //    //get mongo connection
        //    var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();

        //    try
        //    {
        //        var siteReader = documentConnection.CreateSiteReader();
        //        var site = siteReader.GetSite(siteId);
        //        if (site == null)
        //            throw new APIException(Resources.SiteNotFound, Request, null, HttpStatusCode.NotFound);

        //        return site.AdopterId;
        //    }
        //    catch (MongoDbDalcReadDataException ex)
        //    {
        //        throw new APIException(Resources.ErrorReadingDeviceData, Request, ex);
        //    }
        //}

        /// <summary>
        /// Gets the device profile and cache it in dictionary
        /// </summary>
        /// <param name="id">Device Uuid</param>
        /// <param name="parametername"> parameter name to provide error message in case fetching of device profile fails</param>
        /// <returns></returns>
        //public virtual DeviceProfileChannelLookup GetDeviceProfile(string id, string parametername)
        //{
        //    var profileId = PlatformService.DeviceToDeviceProfileCache.GetValue(id);
        //    if (profileId == null) return null;
        //    return PlatformService.DeviceProfileToChannelsCache.GetValue(profileId.ToString());
        //}

        /// <summary>
        /// Validates if specified tag exist or not in device profile
        /// </summary>
        /// <param name="id">Device Uuid</param>
        /// <param name="tags">Channel tags</param>
        /// <param name="parametername">Channel parameter name</param>
        //public virtual void ValidateTagExistsInDeviceProfile(string id, string[] tags, string parametername)
        //{
        //    var profile = GetDeviceProfile(id, parametername);
        //    if (profile == null)
        //        throw new APIException(Resources.DeviceorProfileNotFound, Request, new Exception(), HttpStatusCode.NotFound);

        //    foreach (var tag in tags)
        //    {
        //        if (string.IsNullOrEmpty(tag) || PlatformService.MonitoringChannelTags.Contains(tag)) continue;

        //        if (!profile.channelsLookup.ContainsKey(tag.Trim()))
        //        {
        //            throw new BadRequestException(String.Format(Resources.UnknownTag, tag, profile.Id, id), Request, parametername);
        //        }
        //    }
        //}

        /// <summary>
        /// Validate ingress type. Exception will be thrown if the validation failed.
        /// </summary>
        /// <param name="ingress">Ingress type.</param>
        /// <param name="parameterName">Parameter name.</param>
        public virtual void ValidateIngressType(string ingress, string parameterName)
        {
            // Valid: "iothub", "none" or "rabbitmq".
            if (ingress == IoTHub || ingress == RabbitMQ || ingress == None)
                return;

            throw new BadRequestException(Resources.InvalidIngressType, Request, parameterName);
        }

        /// <summary>
        /// Determines if device command is supported for the specified device.
        /// For example, device come from software ingress pipeline does not support command.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public virtual bool IsDeviceCommandSupported(string id)
        //{
        //    ValidateMongoConnection();

        //    //get mongo connection
        //    var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();

        //    try
        //    {
        //        var deviceReader = documentConnection.CreateDeviceReader();

        //        var device = deviceReader.GetDevice(id, false);
        //        if (device == null)
        //            throw new APIException(Resources.UnregisteredDevice, Request, null, HttpStatusCode.NotFound);

        //        if (String.IsNullOrEmpty(device.Ingress) || device.Ingress == IoTHub)
        //            return true;
        //    }
        //    catch (MongoDbDalcReadDataException ex)
        //    {
        //        throw new APIException(Resources.ErrorReadingDeviceData, Request, ex);
        //    }
        //    return false;
        //}

        /// <summary>
        /// Determines if device has specific capability.
        /// </summary>
        /// <param name="id">Device id.</param>
        /// <param name="capability">Capability to determine.</param>
        /// <returns>True if the device has the capability, false otherwise.</returns>
        //public virtual bool IsDeviceCapable(string id, string capability)
        //{
        //    var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();
        //    var reader = documentConnection.CreateDeviceReader();
        //    var profile = reader.GetDeviceProfile(id);
        //    if (profile == null)
        //        throw new APIException(Resources.UnregisteredDevice, Request, null, HttpStatusCode.NotFound);
        //    if (profile.Device.Capabilities == null)
        //        return false;
        //    return profile.Device.Capabilities.DirectMethod;
        //}

        /// <summary>
        /// Returns list of user/profile ids registered with the adopter
        /// </summary>
        /// <param name="adopterId">The adopter id</param>
        /// <returns>List user/profile ids</returns>
        //internal List<AccessControlUserResult> GetAdopterUsersInternal(string adopterId)
        //{
        //    //validate and get document store connection
        //    ValidateMongoConnection();
        //    var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();
        //    List<AccessControlUserResult> users = new List<AccessControlUserResult>();

        //    try
        //    {
        //        var reader = documentConnection.CreateAdopterReader();
        //        var adopter = reader.GetAdopter(adopterId);
        //        if (adopter == null)
        //            throw new APIException(Resources.AdopterNotFound, Request, null, HttpStatusCode.NotFound);
        //        else
        //        {
        //            if (adopter.Users != null)
        //            {
        //                foreach (var dalcUser in adopter.Users)
        //                {
        //                    if (dalcUser.Active == true)
        //                    {
        //                        var user = new AccessControlUserResult();
        //                        user.Id = dalcUser.UserId;
        //                        user.Email = dalcUser.Email;
        //                        users.Add(user);
        //                    }

        //                }
        //            }
        //        }
        //        return users;
        //    }
        //    catch (MongoDbDalcReadDataException ex)
        //    {
        //        throw new APIException(Resources.ErrorReadingAdopter, Request, ex);
        //    }
        //}


        /// <summary>
        /// Gets the adopter Id for a site.
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        //internal virtual string GetSiteAdopter(string siteId)
        //{
        //    string adopterId = null;
        //    try
        //    {
        //        var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();
        //        var siteReader = documentConnection.CreateSiteReader();
        //        var document = siteReader.GetSite(siteId);

        //        if (document != null)
        //            adopterId = document.AdopterId;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new APIException(string.Format("Error in getting adopter for site {0}.", siteId), Request, ex);
        //    }

        //    return adopterId;
        //}



        //internal AccessControlAdopterResult CreateDtoAdopter(Adopter dalcAdopter)
        //{
        //    AccessControlAdopterResult result = new AccessControlAdopterResult();
        //    result.Id = dalcAdopter.Id;
        //    result.Name = dalcAdopter.Name;
        //    result.Description = dalcAdopter.Description;
        //    result.Created = dalcAdopter.Created;
        //    result.CreatedBy = dalcAdopter.CreatedBy;
        //    result.Modified = dalcAdopter.Modified;
        //    result.ModifiedBy = dalcAdopter.ModifiedBy;

        //    return result;
        //}

        /// <summary>
        /// Retrieves user details from keyvault
        /// </summary>
        /// <param name="requestId">Represents the user request id in the form of an 8-4-4-4-12 uuid string</param>
        /// <returns>User details</returns>
        //internal KeyVaultValue GetUserDetailsFromKeyVaultInternal(string requestId)
        //{
        //    KeyVaultUtilities vault = null;
        //    KeyVaultValue secret = null;
        //    try
        //    {
        //        vault = new KeyVaultUtilities(GetVaultKey());

        //        //Fetch secret from key vault
        //        var jsonSecret = vault.GetSecret(requestId);
        //        if (jsonSecret != null)

        //            //Deserialize key vault data
        //            secret = JsonConvert.DeserializeObject<KeyVaultValue>(jsonSecret);
        //    }
        //    catch (Exception ex)
        //    {

        //        //Log exception
        //        LogUtilities.LogError(Resources.ErrorRetrivingSecret, ex);

        //        //If secret doesn't exists for user or secret exists but verification code is null or empty, then throw resource not found exception  
        //        if ((ex.InnerException == null || ex.InnerException.Message.IndexOf(Resources.SecretNotFound) > -1 || ex.InnerException.Message.IndexOf(Resources.GetSecretFailed) > -1))
        //            throw new APIException(Resources.FailedGetUser, Request, null, HttpStatusCode.NotFound);
        //    }

        //    if (secret != null)
        //        return secret;
        //    else
        //        return null;
        //}

        /// <summary>
        /// Get application secret key for vault
        /// </summary>
        /// <returns></returns>
        internal virtual string GetVaultKey()
        {
            return ConfigurationManager.AppSettings["ApplicationSecret"];
        }

        /// <summary>
        /// Determines if a device is in a site.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        //public virtual bool ValidateDeviceInSite(string deviceId, string siteId)
        //{
        //    try
        //    {
        //        ValidateMongoConnection();
        //        var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();
        //        var deviceReader = documentConnection.CreateDeviceReader();
        //        var document = deviceReader.GetDevice(deviceId);

        //        if (document == null)
        //            throw new APIException(Resources.DeviceNotFound, Request, null, HttpStatusCode.NotFound);

        //        if (document.SiteId == null || document.SiteId.ToLower() != siteId.ToLower())
        //            throw new APIException(String.Format(Resources.DeviceNotInSite, deviceId), Request, null, HttpStatusCode.NotFound);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return true;
        //}

        /// <summary>
        /// Validates a site id.
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        //public virtual bool ValidateSiteId(string siteId)
        //{
        //    try
        //    {
        //        var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();
        //        var siteReader = documentConnection.CreateSiteReader();
        //        var document = siteReader.GetSite(siteId);

        //        if (document == null)
        //            return false;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        /// <summary>
        /// check whether no of custom attributes and size of custom attributes are within the maximum limits.
        /// </summary>
        /// <param name="maxNoOfAttributes">max no of custom attributes limit</param>
        /// <param name="maxCustomAttributeSize">max custom attribute size limit</param>
        /// <param name="customAttributes">custom attributes</param>
        //public virtual void CheckCustomAttributeSizeAndCount(int maxNoOfAttributes, int maxCustomAttributeSize, IDictionary<string, Object> customAttributes)
        //      {
        //          Encoding utf8 = Encoding.UTF8;
        //          int keySize;
        //          int valueSize;

        //          if (customAttributes.Count > maxNoOfAttributes)
        //              throw new BadRequestException(Resources.NumberOfCustomAttributesLimitExceeded, Request, "custom_attributes");

        //          foreach (var kv in customAttributes)
        //          {
        //              if (string.IsNullOrWhiteSpace(kv.Key) || kv.Value == null || string.IsNullOrWhiteSpace(kv.Value.ToString()))
        //                  throw new BadRequestException(Resources.CustomAttributeKeyValueEmpty, Request, "custom_attributes");
        //          }

        //          foreach (var item in customAttributes)
        //          {
        //              if (!(item.Value is String))
        //                  throw new BadRequestException(Resources.CustomAttributeNotString, Request, "custom_attributes");
        //              try
        //              {
        //                  // FIND SIZE IN UTF8 BECAUSE MONGODB SERIALIZE DATA AS BSON FOR STORING AND BSON ENCODES STRING IN UTF8 FORMAT
        //                  keySize = utf8.GetByteCount(item.Key);
        //                  valueSize = utf8.GetByteCount(item.Value.ToString());
        //              }
        //              catch (ArgumentOutOfRangeException ex)
        //              {
        //                  LogUtilities.LogError(Resources.CustomAttributeSizeLimitExceeded, ex);
        //                  throw new BadRequestException(Resources.CustomAttributeSizeLimitExceeded, Request, "custom_attributes");
        //              }

        //              // if custom attribute size exceeds maximum limit then throw error
        //              if (keySize + valueSize > maxCustomAttributeSize)
        //                  throw new BadRequestException(Resources.CustomAttributeSizeLimitExceeded, Request, "custom_attributes");
        //          }
        //      }

        #endregion

        //#region security
        ///// <summary>
        ///// Check if user is authorized to perform given operation on the resource.
        ///// </summary>
        ///// <param name="request">HttpRequest Message</param>
        ///// <param name="operation">Operation Name</param>
        ///// <param name="resourceId">Resource Id on which user is performing operation. It can be null.</param>
        ///// 
        ///// <returns></returns>
        //public bool IsAuthorized(HttpRequestMessage request, string operation, string resourceId)
        //{
        //    return PlatformService.AccessControlInstance.IsAuthorized(request, operation, resourceId, _cid, _logTrackingId);
        //}



        ///// <summary>
        ///// Returns the user id based on the token in the current Request object.
        ///// </summary>
        ///// <returns>userId of user in token</returns>
        //public virtual string GetUserIdFromRequestToken()
        //{
        //    return PlatformService.IdentityProviderInstance.ExtractUserIdFromToken(Request);
        //}
        //#endregion security


        /// <summary>
        /// Convert to user object with only user id and email id
        /// </summary>
        /// <param name="users">List of users</param>
        /// <returns></returns>
        //protected List<AccessControlUserResult> ExtractUserIdAndEmail(List<UserEmailAndId> users)
        //{
        //    List<AccessControlUserResult> items = null;
        //    if (users != null && users.Count > 0)
        //    {
        //        items = new List<AccessControlUserResult>();
        //        foreach (var user in users)
        //        {
        //            var _user = new AccessControlUserResult()
        //            {
        //                Id = user.UserId,
        //                Email = user.Email
        //            };
        //            items.Add(_user);
        //        }
        //    }
        //    return items;
        //}

        /// <summary>
        /// Validates and create mongo connection
        /// </summary>
        /// <returns></returns>
        //protected IDocumentConnection ValidateAndCreateMongoConnection()
        //{
        //    IDocumentConnection documentConnection = null;
        //    try
        //    {
        //        // validate mongo connection
        //        ValidateMongoConnection();

        //        // create mongo connection
        //        documentConnection = PlatformService.ConnectionManager.GetMongoConnection();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new APIException(Resources.ErrorCreatingConnection, Request, ex);
        //    }
        //    return documentConnection;
        //}

        /// <summary>
        /// Check whether input string is Guid 
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>if guid returns true, otherwise returns false</returns>
        //protected bool CheckForGuid(string input)
        //{
        //    Guid guid;
        //    try
        //    {
        //        guid = Guid.Parse(input);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtilities.LogError("Failed to parse guid.", ex);
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// check whether user exists or not based on user key. 
        /// </summary>
        /// <param name="userKey">User key is either email or id</param>
        /// <returns></returns>
        //protected User CheckUserExistsByUserKey(string userKey)
        //{
        //    // validate input
        //    ValidateParameterForNull(userKey, nameof(userKey));

        //    // check whether userKey is email or id
        //    var isUserId = CheckForGuid(userKey);

        //    try
        //    {
        //        if (!isUserId)
        //            ValidateEmail(userKey);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtilities.LogError("Failed to validate email.", ex);
        //        throw new BadRequestException(Resources.InvalidUserKey, Request, "userKey");
        //    }

        //    // get mongo connection
        //    var documentConnection = ValidateAndCreateMongoConnection();

        //    User user = null;

        //    // fetch user data
        //    if (isUserId)
        //        user = FetchUserData(documentConnection, null, userKey);
        //    else
        //        user = FetchUserData(documentConnection, userKey);
        //    return user;
        //}

        /// <summary>
        /// fetch user from mongo
        /// </summary>
        /// <param name="connection">mongo connection object</param>
        /// <param name="email">user email</param>
        /// <param name="id">user id</param>
        /// <returns></returns>
        //protected User FetchUserData(IDocumentConnection connection, string email = null, string id = null)
        //{
        //    User user = null;
        //    try
        //    {
        //        var reader = connection.CreateUserReader();
        //        if (email != null)
        //        {
        //            user = reader.GetUserByEmail(email);
        //            return user;
        //        }
        //        if (id != null)
        //        {
        //            user = reader.GetUserByIdAndAdopter(id);
        //            return user;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new APIException(Resources.FailedToFetchUser, Request, ex);
        //    }
        //    return user;
        //}

        /// <summary>
        /// Check user exists in mongodb and having account enabled.
        /// </summary>
        /// <param name="email">Query by email id</param>
        /// <returns>Return true if user exists and account is enabled else return false</returns>
        //protected bool? ValidateUser(string email)
        //{
        //    ValidateMongoConnection();
        //    var documentConnection = PlatformService.ConnectionManager.GetMongoConnection();

        //    // Read user from mongo
        //    var user = FetchUserData(documentConnection, email);

        //    return user?.AccountEnabled;
        //}
    }
}
