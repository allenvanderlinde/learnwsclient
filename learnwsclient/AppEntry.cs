/**
 * Project: learnwsclient
 * 
 * @file    AppEntry.cs
 * @author  Allen Vanderlinde, 2016 (refer to LICENSE.txt for license details)
 * @date    September 19, 2016
 * @brief   Class which houses the application's main execution.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BbWsClient;   // Include the Learn web services library previously built

/**
 * @brief   .NET web services client that pulls various data from
 *          a Blackboard Learn environment.
 */
namespace learnwsclient
{
    /**
     * @brief   Structure of arguments to use
     *          between classes/objects from
     *          command line.
     */
    public struct Arguments
    {
        public string host;                 /**< Host to connect to. */
        public string tool;                 /**< Name of proxy tool to connect as. */
        public string vendor;               /**< Name of proxy tool's vendor. */
        public string description;          /**< Description of proxy tool (for registration). */
        public string initialsecret;        /**< Password needed to register tool on host. */
        public string sharedsecret;         /**< Proxy tool's shared secret. */
        public string entityOperation;      /**< Type of operation to perform on entity. */
        public string entity;               /**< Entity to pull data from. */
        public bool useInputFile;           /**< Use entities in a file to use for request? */
        public string exportFile;           /**< File to write data to. */
        public bool export;                 /**< Export to file? */
        public string enableOrDisable;      /**< String which requests to enable or disable the user entity also specified in argument list. */
    };

    /**
     * @brief   Main application execution class.
     */
    public class AppEntry {
        /** @brief  Application's arguments list to pass to various classes and objects. */
        private static Arguments                                        argList;

        static int Main(string[] args)
        {
            welcome();
            if(args.Length == 0) {
                show_usage();

                return 0;
            }

            /* Populate arguments list
                with command line inputs. */
            int exportFileIndex;
            try {
                /* Check for tool
                    registration. */
                if(args[0].ToUpper().Equals("REGISTER")) {
                    argList.host = args[1];
                    argList.tool = args[2];
                    argList.vendor = args[3];
                    argList.description = args[4];
                    argList.initialsecret = args[5];
                    argList.sharedsecret = args[6];

                    Console.WriteLine(" Registering tool...\n\n\tHost: {0}\n\tTool: {1}\n\tVendor: {2}\n\tDescription: {3}", argList.host, argList.tool, argList.vendor, argList.description);
                    if(!WebServices.registerTool(argList)) {
                        Console.WriteLine(WebServices.getEventInfo());
                        return -1;
                    }
                    Console.WriteLine(WebServices.getEventInfo());

                    return 0;
                }
                argList.host = args[0];
                argList.tool = args[1];
                argList.vendor = args[2];
                argList.sharedsecret = args[3];

                argList.entityOperation = args[4];

                /* Refine value assignment to
                    client objects from
                    arguments. */
                if(Convert.ToInt32(argList.entityOperation) == (int)ENTITY_OPERATION.USERS_INFO_ALL ||
                   Convert.ToInt32(argList.entityOperation) == (int)ENTITY_OPERATION.COURSES_INFO_ALL) {  // Set entity argument to arbitrary string for "_INFO_ALL" operations
                    argList.entity = "all";
                    exportFileIndex = 5;
                } else if(Convert.ToInt32(argList.entityOperation) == (int)ENTITY_OPERATION.USERS_INFO_BY_USERIDS_FROM_FILE ||
                          Convert.ToInt32(argList.entityOperation) == (int)ENTITY_OPERATION.COURSES_INFO_BY_COURSEIDS_FROM_FILE ||
                          Convert.ToInt32(argList.entityOperation) == (int)ENTITY_OPERATION.USERS_ENROLLMENT_INFO_BY_USERIDS_FROM_FILE ||
                          Convert.ToInt32(argList.entityOperation) == (int)ENTITY_OPERATION.COURSES_ENROLLMENT_INFO_BY_COURSEIDS_FROM_FILE) {   // Set entity argument to file specified for "_FROM_FILE" operations
                    argList.useInputFile = true;
                    argList.entity = args[5];
                    exportFileIndex = 6;
                } else if(Convert.ToInt32(argList.entityOperation) == (int)ENTITY_OPERATION.USER_MODIFY_ROW_STATUS) {   // Enable or disable an individual user record
                    argList.useInputFile = false;
                    argList.entity = args[5];
                    argList.enableOrDisable = args[6];
                    exportFileIndex = 7;
                } else {
                    argList.useInputFile = false;
                    argList.entity = args[5];
                    exportFileIndex = 6;
                }
            } catch(Exception e) {
                Console.WriteLine(" FAIL: One or more invalid or empty arguments!\n\tMore info: {0}", e.Message);
                return -1;
            }

            try {
                argList.exportFile = args[exportFileIndex];
                argList.export = true;
                string ext = argList.exportFile.Substring(argList.exportFile.IndexOf('.') + 1);
                DataRequest.setExportFormat(ext);
                Console.WriteLine(" Export data to file \"{0}\"\n", argList.exportFile);
            } catch (Exception e) {
                argList.export = false;
            }

            /* Set necessary members for WebServices
                class. */
            WebServices.setHost(argList.host);
            WebServices.setTool(argList.tool);
            WebServices.setToolVendor(argList.vendor);

            WebServices.setSessionUser(WebServices.getTool());

            if(!WebServices.determineOperation(argList.entityOperation)) {
                Console.WriteLine(WebServices.getEventInfo());
                return -1;
            }

            /* Connect to the host's
                web services endpoints. */
            Console.WriteLine(" Establishing connection to {0}...", WebServices.getHost());
            if(!WebServices.connectAndInit()) {
                Console.WriteLine(WebServices.getEventInfo());
                return -1;
            }
            Console.WriteLine(WebServices.getEventInfo());

            /* Log into the web
                services. */
            Console.WriteLine("\n Attempting to log in as proxy tool \"{0}\"...", WebServices.getTool());

            if(!WebServices.loginAsTool(argList.sharedsecret)) {
                Console.WriteLine(WebServices.getEventInfo());
                Console.WriteLine("\n Shutting down...");
                WebServices.logout();
                Console.WriteLine(WebServices.getEventInfo());
                return -1;
            }
            Console.WriteLine(WebServices.getEventInfo());

            /* Initialize
                wrappers. */
            Console.WriteLine("\n Initializing services...");
            WebServices.initWrappers();

            /* Send request and pull
                desired data based on user's
                arguments. */
            Console.WriteLine("\n Sending request type \"{0}\" for entity \"{1}\"...", argList.entityOperation, argList.entity);
            DataRequest request = new DataRequest(argList);
            Console.WriteLine(WebServices.getEventInfo());

            /* Export requested data to a file
                or print to console. */
            int entitiesStored = 0;
            if(argList.export && request.isSuccessful()) {
                request.export();
            } else {
                Console.WriteLine("");
                if(request.getUsers() != null) {
                    entitiesStored = request.getUsers().getCount();
                    if(entitiesStored > 0) {
                        for(int i = 0; i <= request.getUsers().getCount(); i++) {
                            Console.WriteLine(" {0}", request.getUsers().getData()[i]);
                        }
                    }
                } else if(request.getCourses() != null) {
                    entitiesStored = request.getCourses().getCount();
                    if(entitiesStored > 0) {
                        for(int i = 0; i <= request.getCourses().getCount(); i++) {
                            Console.WriteLine(" {0}", request.getCourses().getData()[i]);
                        }
                    }
                } else if(request.getEnrollments() != null) {
                    entitiesStored = request.getEnrollments().getCount();
                    if(entitiesStored > 0) {
                        for(int i = 0; i <= request.getEnrollments().getCount(); i++) {
                            Console.WriteLine(" {0}", request.getEnrollments().getData()[i]);
                        }
                    }
                } else {
                    Console.WriteLine("\t--> No data pulled for specified criteria or the entity is disabled.");
                }

                Console.WriteLine(String.Format("\n Records pulled:\t{0}", entitiesStored));
            }

            /* Log out of the
                web services session. */
            Console.WriteLine("\n Ending session for \"{0}\"...", WebServices.getSessionUser());
            if(!WebServices.logout()) {
                Console.WriteLine(WebServices.getEventInfo());
                return -1;
            }
            Console.WriteLine(WebServices.getEventInfo());

            return 0;
        }

        public static void welcome() {
            Console.WriteLine("\n ///////////////////////////////////////////////////////////////");
            Console.WriteLine("   _                                        _ _            _   ");
            Console.WriteLine("  | |                                      | (_)          | |  ");
            Console.WriteLine("  | | ___  __ _ _ __ _ ____      _____  ___| |_  ___ _ __ | |_ ");
            Console.WriteLine("  | |/ _ \\/ _` | '__| '_ \\ \\ /\\ / / __|/ __| | |/ _ \\ '_ \\| __|");
            Console.WriteLine("  | |  __/ (_| | |  | | | \\ V  V /\\__ \\ (__| | |  __/ | | | |_ ");
            Console.WriteLine("  |_|\\___|\\__,_|_|  |_| |_|\\_/\\_/ |___/\\___|_|_|\\___|_| |_|\\__|\n");
            Console.WriteLine(" ///////////////////////////////////////////////////////////////\n");
        }

        public static void show_usage() {
            Console.WriteLine("\n Written by Allen Vanderlinde, 2016\n");
            Console.WriteLine("\n learnwsclient USAGE:\n\n Learnwsclient has a single option to register a proxy tool with the following syntax:");
            Console.WriteLine("\n learnwsclient register [HOST] [TOOL_NAME] [TOOL_VENDOR] [TOOL_DESCRIPTION] [INITIAL_SECRET] [SHARED_SECRET]\n");
            Console.WriteLine("\t[HOST]\t\t\tThe fully-qualified URL of the Learn environment to register to the tool in");
            Console.WriteLine("\t[TOOL_NAME]\t\tThe chosen name to register the proxy tool as");
            Console.WriteLine("\t[TOOL_VENDOR]\t\tThe chosen vendor of the tool");
            Console.WriteLine("\t[TOOL_DESCRIPTION]\tA brief description of the proxy tool. Must be enclosed in \"\"");
            Console.WriteLine("\t[INITIAL_SECRET]\tThis is the required registration password configured in\n\t\t\t\tBuilding Blocks > Proxy Tools > Manage Global Properties");
            Console.WriteLine("\t[SHARED_SECRET]\t\tThis is the password the registered tool will use to connect");
            Console.WriteLine("\n\tExample:\n\tlearnwsclient register https://sub.client.edu learnwsclient bb \"WS client.\" globalpassword toolpassword");
            Console.WriteLine("\n\n To pull data from an environment with learnwsclient:");
            Console.WriteLine("\n learnwsclient [HOST] [TOOL_NAME] [TOOL_VENDOR] [SHARED_SECRET] [ENTITY_OPERATION] [ENTITY] {EXPORT_FILE}");
            Console.WriteLine("\n\tNote: {EXPORT_FILE} is an optional argument. Currently supported file types are .CSV and .TXT");
            Console.WriteLine("\n\tExample:\n\tlearnwsclient https://sub.client.edu learnwsclient bb toolpassword 7 bbsupport");
            Console.WriteLine("\n\t- This pulls all user enrollments for user \"bbsupport\"");
            Console.WriteLine("\n\n The following entity operations are available:");
            Console.WriteLine("\n\t1\tRequest general user data for individual user entity.");
            Console.WriteLine("\t2\tRequest general users data for all user entities specified in a file.");
            Console.WriteLine("\t3\tRequest all users and their information on the system.");
            Console.WriteLine("\t4\tRequest general course data for an individual course entity.");
            Console.WriteLine("\t5\tRequest general courses data for all course entities specified in a file.");
            Console.WriteLine("\t6\tRequest all courses and their information on the system.");
            Console.WriteLine("\t7\tRequest course enrollments for a user by user ID.");
            Console.WriteLine("\t8\tRequest all course enrollments for all user entities specified in a file.");
            Console.WriteLine("\t9\tRequest enrollments for an individual course entity.");
            Console.WriteLine("\t10\tRequest enrollments for all course entities specified in a file.");
            Console.WriteLine("\n *Note: A learnwsclient \"entity\" is the name, ID, or file of items which the client is pulling data on.");
        }
    }
}