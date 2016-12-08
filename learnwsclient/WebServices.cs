/**
 * Project: learnwsclient
 * 
 * @file    WebServices.cs
 * @author  Allen Vanderlinde, 2016 (refer to LICENSE.txt for license details)
 * @date    September 19, 2016
 * @brief   This class handles establishing a connection,
 *          initializing the web services, logging in,
 *          and tool registration.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BbWsClient;

namespace learnwsclient
{
    /**
     * @brief   This class handles establishing a connection, initializing
     *          the web services, loggin in, and tool registration.
     */
    public class WebServices {
        /** @brief  String to match to register a new proxy tool. */
        public const string                                             REGISTER_STRING         = "register";
        /** @brief  Expected lifetime of session. */
        public const long                                               lifetime                = 60000;

        /** @brief  Recent event results/message. */
        private static string                                           eventInfo;

        /** @brief  Host environment to connect to. */
        private static string                                           host;

        /** @brief  Proxy tool's name. */
        private static string                                           tool;
        /** @brief  Proxy tool's vendor. */
        private static string                                           vendor;
        /** @brief  Proxy tool's description. */
        private static string                                           description;

        /** @brief  Session ID generated after successful connection. */
        private static string                                           sessionId;

        /** @brief  Learn web services general wrapper. */
        private static WebserviceWrapper                                webservices;
        /** @brief  Learn web services context wrapper. */
        private static ContextWrapper                                   ws_context;
        /** @brief  Learn web services course wrapper. */
        private static CourseWrapper                                    ws_course;
        /** @brief  Learn web services course membership wrapper. */
        private static CourseMembershipWrapper                          ws_membership;
        /** @brief  Learn web services user wrapper. */
        private static UserWrapper                                      ws_user;
        /** @brief  Learn web services utility wrapper. */
        private static UtilWrapper                                      ws_util;

        /** @brief  Type of user for the current session (e.g., proxy tool or user). */
        private static string                                           sessionUser;
        /** @brief  Learn proxy tool registration result object. */
        private static RegisterToolResultVO                             registrationResult;

        /** @brief  List of entitlements to grant proxy tool upon registration and login. */
        public static string[]                                          entitlements            = {
                                                                                                    "Context.WS:emulateUser",
                                                                                                    "Context.WS:logout",
                                                                                                    "Context.WS:getMemberships",
                                                                                                    "Context.WS:getMyMemberships",
                                                                                                    "Util.WS:checkEntitlement",
                                                                                                    "Course.WS:changeCourseCategoryBatchUid",
                                                                                                    "Course.WS:changeCourseBatchUid",
                                                                                                    "Course.WS:changeCourseDataSourceId",
                                                                                                    "Course.WS:createCourse",
                                                                                                    "Course.WS:createOrg",
                                                                                                    "Course.WS:deleteCartridge",
                                                                                                    "Course.WS:deleteCourse",
                                                                                                    "Course.WS:deleteCourseCategory",
                                                                                                    "Course.WS:deleteCourseCategoryMembership",
                                                                                                    "Course.WS:deleteGroup",
                                                                                                    "Course.WS:deleteOrg",
                                                                                                    "Course.WS:deleteOrgCategory",
                                                                                                    "Course.WS:deleteOrgCategoryMembership",
                                                                                                    "Course.WS:deleteStaffInfo",
                                                                                                    "Course.WS:getAvailableGroupTools",
                                                                                                    "Course.WS:getCartridge",
                                                                                                    "Course.WS:getCategories",
                                                                                                    "Course.WS:getCategoryMembership",
                                                                                                    "Course.WS:getClassifications",
                                                                                                    "Course.WS:getCourse",
                                                                                                    "Course.WS:getGroup",
                                                                                                    "Course.WS:getOrg",
                                                                                                    "Course.WS:getStaffInfo",
                                                                                                    "Course.WS:initializeCourseWS",
                                                                                                    "Course.WS:saveCartridge",
                                                                                                    "Course.WS:saveCourse",
                                                                                                    "Course.WS:saveCourseCategory",
                                                                                                    "Course.WS:saveCourseCategoryMembership",
                                                                                                    "Course.WS:saveGroup",
                                                                                                    "Course.WS:saveOrgCategory",
                                                                                                    "Course.WS:saveOrgCategoryMembership",
                                                                                                    "Course.WS:saveStaffInfo",
                                                                                                    "Course.WS:updateCourse",
                                                                                                    "Course.WS:updateOrg",
                                                                                                    "CourseMembership.WS:deleteCourseMembership",
                                                                                                    "CourseMembership.WS:deleteGroupMembership",
                                                                                                    "CourseMembership.WS:getCourseMembership",
                                                                                                    "CourseMembership.WS:getCourseRoles",
                                                                                                    "CourseMembership.WS:getGroupMembership",
                                                                                                    "CourseMembership.WS:getServerVersion",
                                                                                                    "CourseMembership.WS:initializeCourseMembershipWS",
                                                                                                    "CourseMembership.WS:saveCourseMembership",
                                                                                                    "CourseMembership.WS:saveGroupMembership",
                                                                                                    "User.WS:getServerVersion",
                                                                                                    "User.WS:initializeUserWS",
                                                                                                    "User.WS:saveUser",
                                                                                                    "User.WS:getUser",
                                                                                                    "User.WS:deleteUser",
                                                                                                    "User.WS:saveObserverAssociation",
                                                                                                    "User.WS:getObservee",
                                                                                                    "User.WS:deleteAddressBookEntry",
                                                                                                    "User.WS:getAddressBookEntry",
                                                                                                    "User.WS:saveAddressBookEntry"
                                                                                                  };

        /**
         * @brief   Connect to the web services host and initialize services.
         * @retval  bool    True if successful.
         */
        public static bool connectAndInit() {
            try {
                webservices = new WebserviceWrapper(host,
                                                    vendor,
                                                    tool,
                                                    lifetime);
            } catch(Exception e) {
                eventInfo = String.Format(" FAIL: Unable to initialize web services wrapper!\n\tMore info: {0}", e.Message);
                return false;
            }

            webservices.initialize_v1();

            if(webservices.debugInfo() == "NO session") {
                eventInfo = String.Format(" FAIL: Unable to generate a session! Initialization failed. Are services discoverable and available?\n\tMore info: {0}", webservices.getLastError());

                return false;
            }

            sessionId = webservices.debugInfo();
            eventInfo = String.Format(" SUCCESS: Session generated: {0}", sessionId);

            return true;
        }

        /**
         * @brief   Initialize the various Learn web services wrappers.
         * @retval  bool    True if wrappers initialized successfully.
         */
        public static bool initWrappers() {
            try {
                Console.Write("\tContext.WS... ");
                ws_context = webservices.getContextWrapper();
                if(ws_context != null) {
                    Console.WriteLine("SUCCESS");
                    /* The following is a workaround to allow
                        proxy tools to expose all users and courses
                        by forcing administrator emulation.
                        
                        Bug still at: 9/27/2016 */
                    ws_context.emulateUser("administrator");
                }
            } catch(System.Web.Services.Protocols.SoapException e) {
                eventInfo = String.Format(" WARNING: Web service unable to be initialized.\n\tLast Error: {0}\n\tException info: {1}\n", webservices.getLastError(), e.Message);
                Console.WriteLine(eventInfo);
            }
            try {
                Console.Write("\tCourse.WS... ");
                ws_course = webservices.getCourseWrapper();
                if(ws_course != null) {
                    Console.WriteLine("SUCCESS");
                }
            } catch(System.Web.Services.Protocols.SoapException e) {
                eventInfo = String.Format(" WARNING: Web service unable to be initialized.\n\tLast Error: {0}\n\tException info: {1}\n", webservices.getLastError(), e.Message);
                Console.WriteLine(eventInfo);
            } try {
                Console.Write("\tCourseMembership.WS... ");
                ws_membership = webservices.getCourseMembershipWrapper();
                if(ws_membership != null) {
                    Console.WriteLine("SUCCESS");
                }
            } catch(System.Web.Services.Protocols.SoapException e) {
                eventInfo = String.Format(" WARNING: Web service unable to be initialized.\n\tLast Error: {0}\n\tException info: {1}\n", webservices.getLastError(), e.Message);
                Console.WriteLine(eventInfo);
            } try {
                Console.Write("\tUser.WS... ");
                ws_user = webservices.getUserWrapper();
                if(ws_user != null) {
                    Console.WriteLine("SUCCESS");
                }
            } catch(System.Web.Services.Protocols.SoapException e) {
                eventInfo = String.Format(" WARNING: Web service unable to be initialized.\n\tLast Error: {0}\n\tException info: {1}\n", webservices.getLastError(), e.Message);
                Console.WriteLine(eventInfo);
            } try {
                Console.Write("\tUtil.WS... ");
                ws_util = webservices.getUtilWrapper();
                if(ws_util != null) {
                    Console.WriteLine("SUCCESS");
                }
            } catch(System.Web.Services.Protocols.SoapException e) {
                eventInfo = String.Format(" WARNING: Web service unable to be initialized.\n\tLast Error: {0}\n\tException info: {1}\n", webservices.getLastError(), e.Message);
                Console.WriteLine(eventInfo);
            }

            return true;
        }

        /**
         * @brief   Determine operation to perform on data
         */
        public static bool determineOperation(string _eop) {
            try {
                DataRequest.setEntityOperation((ENTITY_OPERATION)Convert.ToInt32(_eop));

                return true;
            } catch(Exception e) {
                eventInfo = String.Format(" FAIL: Unable to determine operation from argument \"{0}\"!\n\tMore info: {1}", _eop, e.Message);

                return false;
            }
        }

        /**
         * @brief   Register new proxy tool to host environment.
         * @retval  bool    True if tool successfully registered.
         */
        public static bool registerTool(Arguments args) {
            host = args.host;
            tool = args.tool;
            vendor = args.vendor;
            description = args.description;

            if(!connectAndInit()) {
                return false;
            }

            registrationResult = webservices.registerTool(description, args.initialsecret, args.sharedsecret, entitlements, null);

            if(registrationResult.status) {
                eventInfo = String.Format("\n Tool \"{0}\" registered with host \"{1}\" successfully", tool, host);
                return true;
            } else {
                eventInfo = String.Format("\n Tool \"{0}\" registration unsuccessful!\n\tMore info: {1}", tool, registrationResult.failureErrors[0]);
                return false;
            }
        }

        /**
         * @brief   Connect to the web services as a proxy tool.
         * @retval  bool    True if successful.
         */
        public static bool loginAsTool(string sharedsecret) {
            if(!webservices.loginTool(sharedsecret)) {
                eventInfo = String.Format(" FAIL: Unable to log in to \"{0}\" as tool \"{1}\"!\n\tMore info: {2}\n\t*Is proxy tool registered in host?", host, tool, webservices.getLastError());
                return false;
            } else {
                eventInfo = String.Format(" Logged into \"{0}\" as tool \"{1}\" successfully", host, tool);
                return true;
            }
        }

        /**
         * @brief   Log out of the current web services session.
         * @retval  bool    True if no issues logging out.
         */
        public static bool logout() {
            if(!getWS().logout()) {
                eventInfo = String.Format("\n Session ended with issue(s): {0}", getWS().getLastError());
                return false;
            }

            eventInfo = String.Format(" Session ended for \"{0}\" successfully.", sessionUser);

            return true;
        }

        /**
         * @brief   Return a string translated from Unix
         *          epoch time representing a date.
         * @param[in]   ticks   The number of ticks returned by the Learn
         *                      web service for dates and birth dates.
         * @retval      string    The string representation of the date.
         */
        public static string getDateFromEpoch(long ticks) {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(ticks).ToShortDateString();
        }

        /**
         * @brief   Writes the most recently set event information immediately
         *          to the console or some other medium.
         */
        public static void postEventInfo() {
            Console.WriteLine(eventInfo);
        }

        /**
         * @brief   Set the host to connect to.
         * @param[in]   _host   The Learn environment site to connect to.
         */
        public static void setHost(string _host) { host = _host; }
        /**
         * @brief   Set the name of the proxy tool to connect as.
         * @param[in]   _tool   The name of the tool.
         */
        public static void setTool(string _tool) { tool = _tool; }
        /**
         * @brief   Set the vendor of the proxy tool/client being used.
         * @param[in]   _vendor The name of the tool's vendor.
         */
        public static void setToolVendor(string _vendor) { vendor = _vendor; }
        /**
         * @brief   Set the description of the proxy tool/client
         *          being registered.
         * @param[in]   _desc   The description of the proxy tool.
         */
        public static void setToolDescription(string _desc) { description = _desc; }
        /**
         * @brief   Set the session user.
         * @param[in]   _su  The name of the user or proxy tool logging in.
         */
        public static void setSessionUser(string _su) { sessionUser = _su; }
        /**
         * @brief   Set's the most recent information for an event.
         * @param[in]   _info    String describing recent activity or event.
         */
        public static void setEventInfo(string _info) { eventInfo = _info; }

        /**
         * @brief   Get the host URL.
         * @retval  string  The Learn environment connected to.
         */
        public static string getHost() { return host; }
        /**
         * @brief   Get the name of the proxy tool being
         *          used/connected as.
         * @retval  string  The name of the tool.
         */
        public static string getTool() { return tool; }
        /**
         * @brief   Get the vendor of the proxy tool/client
         *          being used.
         * @retval  string  The name of the proxy tool's vendor.
         */
        public static string getToolVendor() { return vendor; }
        /**
         * @brief   Get the proxy tool's/client's description.
         * @retval  string  The description of the tool.
         */
        public static string getToolDescription() { return description; }
        /**
         * @brief   Get the username or proxy tool name logged in.
         * @retval  string  The username or tool name logged in.
         */
        public static string getSessionUser() { return sessionUser; }

        /**
         * @brief   Get the Learn web service general wrapper object.
         * @retval  WebserviceWrapper   Learn's web services general wrapper object.
         */
        public static WebserviceWrapper getWS() { return webservices; }
        /**
         * @brief   Get the web service's context object.
         * @retval  ContextWrapper  Learn's web services context object.
         */
        public static ContextWrapper getContextWS() { return ws_context; }
        /**
         * @brief   Get the Learn course web service object.
         * @retval  CourseWrapper   Learn's web serivces course object.
         */
        public static CourseWrapper getCourseWS() { return ws_course; }
        /**
         * @brief   Get the Learn course membership web service object.
         * @retval  CourseMembershipWrapper Learn's web services enrollments object.
         */
        public static CourseMembershipWrapper getCourseMembershipWS() { return ws_membership; }
        /**
         * @brief   Get the Learn user web service object.
         * @retval  UserWrapper Learn's web services user object.
         */
        public static UserWrapper getUserWS() { return ws_user; }
        /**
         * @brief   Get the Learn user utility web service object.
         * @retval  UtilWrapper Learn's web serivces utility object.
         */
        public static UtilWrapper getUtilWS() { return ws_util; }

        /**
         * @brief   Get the most recent event's result information to
         *          report to user.
         * @retval  string  Most recent event's results/message.
         */
        public static string getEventInfo() { return eventInfo; }
    }
}